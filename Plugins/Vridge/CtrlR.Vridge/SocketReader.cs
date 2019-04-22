using System;
using System.IO.Ports;
using System.Threading;

namespace CtrlR.Vridge
{
    public abstract class SocketReader
    {
        public const int SerialPortBaudRate = 115200;
        private SerialPort m_SerialPort;
        private Thread m_Thread;

        public string SerialportName { get; set; }

        public bool IsRunning { get; private set; } = false;

        public string[] SerialPortNames => SerialPort.GetPortNames();

        public bool Connect()
        {
            if (m_SerialPort != null)
                Shutdown();

            if (string.IsNullOrEmpty(SerialportName))
            {
                var serialNames = SerialPort.GetPortNames();

                if (serialNames.Length == 0)
                    return false;

                SerialportName = serialNames[serialNames.Length - 1];
            }

            Console.WriteLine("[Arduino] Trying to connect on the port: " + SerialportName);

            m_SerialPort = new SerialPort(SerialportName, SerialPortBaudRate);
            m_SerialPort.DtrEnable = true;
            m_SerialPort.ReadTimeout = 500;

            if (!m_SerialPort.IsOpen)
                m_SerialPort.Open();

            IsRunning = true;

            m_Thread = new Thread(ReadMessage);
            m_Thread.Start();

            Console.WriteLine("[Arduino] Connected to the port: " + SerialportName);

            return true;
        }

        public void Shutdown()
        {
            IsRunning = false;

            if (m_Thread != null && m_Thread.IsAlive)
            {
                m_Thread.Join();
                m_Thread = null;
            }

            if (m_SerialPort != null && m_SerialPort.IsOpen)
            {
                m_SerialPort.Close();
                m_SerialPort = null;
            }
        }

        private void ReadMessage()
        {
            string data = string.Empty;
            string[] tmp = null;

            while (IsRunning)
            {
                try
                {
                    data = m_SerialPort.ReadLine();
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine(ex.Message);
#endif
                    continue;
                }

#if DEBUG
                Console.WriteLine(data);
#endif

                tmp = data.Split('|');

                if (tmp != null)
                    ParseData(ref tmp);
            }
        }

        protected abstract void ParseData(ref string[] data);
    }
}
