using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

namespace OSSCtrlR
{
    public class SerialClient : DataClient
    {
        private const int SerialPortBaudRate = 115200;
        private SerialPort m_SerialPort;
        private bool m_IsRunning = true;
        private Thread m_readThread = null;

        [SerializeField]
        private string m_SerialPortName = string.Empty;


        private void Start()
        {
            if (m_SerialPortName == string.Empty)
            {
                var serialNames = SerialPort.GetPortNames();
                m_SerialPortName = serialNames[serialNames.Length - 1];
            }

            Log("[Arduino] Trying to connect on the port: " + m_SerialPortName);

            m_SerialPort = new SerialPort(m_SerialPortName, SerialPortBaudRate);
            m_SerialPort.DtrEnable = true;
            m_SerialPort.ReadTimeout = 500;
            m_SerialPort.Open();

            m_readThread = new Thread(ReadMessage);
            m_readThread.Start();
        }

        private void OnDestroy()
        {
            m_IsRunning = false;
            m_readThread.Join();
            m_SerialPort.Close();
        }

        public void Stop()
        {
            m_IsRunning = false;
        }

        private void ReadMessage()
        {
            Log("[Arduino] Connected to the port: " + m_SerialPortName);

            var message = string.Empty;

            while (m_IsRunning)
            {
                try
                {
                    message = m_SerialPort.ReadLine();

                    if (!string.IsNullOrEmpty(message))
                        ParseData(ref message, 0);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
