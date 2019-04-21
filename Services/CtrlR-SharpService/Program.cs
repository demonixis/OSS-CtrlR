#define MAC
using Fleck;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net;
using System.Threading;

namespace OSSCtrlR
{
    class Program
    {
        private const int ServerPort = 8181;
        private static int SerialPortBaudRate = 115200;
        private static string SerialPortName = string.Empty;
        private static bool VerboseOutput = true;
        private static List<IWebSocketConnection> clients = new List<IWebSocketConnection>(1);

        private static SerialPort serial;
        private static int countClients = 0;
        private static string message = string.Empty;
        private static bool running = true;

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ParseCliParameters(ref args);

            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = "127.0.0.1";

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipAddress = ip.ToString();
                    break;
                }
            }

            var webSocketServer = new Fleck.WebSocketServer($"ws://{ipAddress}:{ServerPort}");

            webSocketServer.Start(socket =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine($"Connection Opened by {socket.ConnectionInfo.ClientIpAddress}.");
                    clients.Add(socket);
                    countClients++;
                };

                socket.OnClose = () =>
                {
                    Console.WriteLine($"Connection Closed by {socket.ConnectionInfo.ClientIpAddress}.");
                    clients.Remove(socket);
                    countClients--;
                };
            });

            Console.WriteLine("WebSocket Server Started!");

            StartArduino();

            Console.ReadKey();
        }

        /// <summary>
        /// Start the Arduino Controller.
        /// </summary>
        private static void StartArduino()
        {
            if (SerialPortName == string.Empty)
            {
                var serialNames = SerialPort.GetPortNames();
                SerialPortName = serialNames[serialNames.Length - 1];

#if MAC
                SerialPortName = "/dev/tty.wchusbserial1410";
#endif
            }

            Console.WriteLine("[Arduino] Trying to connect on the port: " + SerialPortName);

            var readThread = new Thread(ReadMessage);

            //using (serial = FindArduino())
            using (serial = new SerialPort(SerialPortName, SerialPortBaudRate))
            {
                serial.DtrEnable = true;
                serial.ReadTimeout = 500;

                if (!serial.IsOpen)
                    serial.Open();

                readThread.Start();

                Console.WriteLine("[Arduino] Connected to the port: " + SerialPortName);

                while (running)
                    Thread.Sleep(10);

                readThread.Join();
                serial.Close();
            }
        }

        private static SerialPort FindArduino()
        {
            var serialNames = SerialPort.GetPortNames();
            var serial = (SerialPort)null;

            foreach (var serialName in serialNames)
            {
                try
                {
                    serial = new SerialPort(serialName, SerialPortBaudRate);
                    serial.ReadTimeout = 500;
                    serial.Open();
                    serial.Write("AT?VERSION");

                    var founded = false;
                    var i = 0;
                    var max = 10;
                    var str = string.Empty;

                    while (i< max && !founded)
                    {
                        str = serial.ReadLine();
                        if (str == "OK")
                            return serial;

                        Thread.Sleep(15);
                    }

                    return serial;
                }
                catch (Exception ex)
                {
                    if (serial != null)
                    {
                        if (serial.IsOpen)
                            serial.Close();

                        serial.Dispose();
                        serial = null;
                        continue;
                    }
                }
            }

            return serial;
        }

        private static void ReadMessage()
        {
            while (running)
            {
                try
                {
                    message = serial.ReadLine();

                    if (VerboseOutput)
                        Console.WriteLine(message);

                    for (var i = 0; i < countClients; i++)
                        clients[i].Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void ParseCliParameters(ref string[] args)
        {
            string[] tmp = null;

            for (int i = 0, l = args.Length; i < l; i++)
            {
                tmp = args[i].Split('=');

                if (tmp.Length == 2)
                {
                    switch (tmp[0])
                    {
                        case "BaudRate":
                            SetInt(tmp[1], ref SerialPortBaudRate);
                            break;
                        case "PortName":
                            SerialPortName = tmp[1].Trim();
                            break;
                        case "Verbose":
                            SetBool(tmp[1], ref VerboseOutput);
                            break;
                    }
                }
            }
        }

        private static void SetInt(string str, ref int target)
        {
            int value;
            if (int.TryParse(str.Trim(), out value))
                target = value;
        }

        private static void SetBool(string str, ref bool target)
        {
            bool value;
            if (bool.TryParse(str.Trim(), out value))
                target = value;
        }
    }
}
