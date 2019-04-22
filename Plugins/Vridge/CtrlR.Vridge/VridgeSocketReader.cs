using VRE.Vridge.API.Client.Remotes;

namespace CtrlR.Vridge
{
    public class VridgeSocketReader : SocketReader
    {
        private ArduinoController m_ArduinoController;
        private VridgeRemote m_VridgeRemote;

        public ArduinoController Controller => m_ArduinoController;

        public VridgeSocketReader() : this(null, true)
        {
        }

        public VridgeSocketReader(string portName, bool left)
        {
            m_ArduinoController = new ArduinoController(left);
            SerialportName = portName;
            m_VridgeRemote = new VridgeRemote("localhost", "OssCtrlR-Vridge", Capabilities.Controllers);
        }

        protected override void ParseData(ref string[] data)
        {
            if (data[0] == "q")
                m_ArduinoController.ParseOrientation(ref data);

            if (data[0] == "b")
                m_ArduinoController.ParseButtons(ref data);

            m_ArduinoController.SetControllerState(m_VridgeRemote);
        }
    }
}
