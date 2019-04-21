using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

namespace OSSCtrlR
{
    public class NeoSerialClient : DataClient
    {
        private const int SerialPortBaudRate = 115200;
        private SerialPort[] serials;
        private Thread readTreads = null;
        private bool running = true;

        [SerializeField]
        private readonly string[] _serialPortNames = new string[MaxTrackedObjects];

        private void Start()
        {
            serials = new SerialPort[MaxTrackedObjects];
            readTreads = new Thread(ReadData);

            var connexions = 0;

            for (var i = 0; i < MaxTrackedObjects; i++)
                if (!string.IsNullOrEmpty(_serialPortNames[i]))
                    CreateConnexion(connexions++);
        }

        private void CreateConnexion(int serialPortIndex)
        {
            Log("[Arduino] Trying to connect on the port: " + _serialPortNames[serialPortIndex]);

            serials[serialPortIndex] = new SerialPort(_serialPortNames[serialPortIndex], SerialPortBaudRate);
            serials[serialPortIndex].DtrEnable = true;
            serials[serialPortIndex].ReadTimeout = 500;
            serials[serialPortIndex].Open();

            Log("[Arduino] Connected to the port: " + _serialPortNames[serialPortIndex]);
        }

        private void OnDestroy()
        {
            running = false;

            readTreads.Join();

            foreach (var serial in serials)
                serial.Close();
        }

        public void Stop()
        {
            running = false;
        }

        private void ReadData()
        {
            var message = string.Empty;

            while (running)
            {
                for (var i = 0; i < MaxTrackedObjects; i++)
                {
                    if (serials[i] == null || !serials[i].IsOpen)
                        continue;

                    message = serials[i].ReadLine();

                    if (!string.IsNullOrEmpty(message))
                        ParseData(ref message, i);
                }
            }
        }
    }
}
