using System.Net;
using UnityEngine;
using WebSocketSharp;

namespace OSSCtrlR
{
    public class WebClient : DataClient
    {
        private WebSocket _webSocket = null;

        [SerializeField]
        private string _ipAddress = "";
        [SerializeField]
        private string _port = "8181";

        void Start()
        {
            Connect();
        }

        void OnDestroy()
        {
            if (_webSocket != null && _webSocket.IsAlive)
                _webSocket.Close();
        }

        public void Connect()
        {
            if (_ipAddress == string.Empty)
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());

                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        _ipAddress = ip.ToString();
                        break;
                    }
                }
            }

            if (_webSocket != null && _webSocket.IsAlive)
                _webSocket.Close();

            _webSocket = new WebSocket(string.Format("ws://{0}:{1}", _ipAddress, _port));
            _webSocket.OnOpen += (s, e) => Debug.Log("Connected to the server");
            _webSocket.OnError += (s, e) => Debug.LogFormat("Error detected.\n{0}", e.Message);
            _webSocket.OnMessage += (s, e) => ParseData(e.Data, 0);
            _webSocket.Connect();
        }
    }
}