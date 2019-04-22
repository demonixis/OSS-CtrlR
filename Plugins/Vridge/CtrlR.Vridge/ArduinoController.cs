using System.Numerics;
using VRE.Vridge.API.Client.Messages.BasicTypes;
using VRE.Vridge.API.Client.Messages.v3.Controller;
using VRE.Vridge.API.Client.Remotes;

namespace CtrlR.Vridge
{
    public class ArduinoController
    {
        public enum Buttons
        {
            Trigger = 0, Grip, Menu
        }

        private const int ButtonsCount = 3;
        public int ControllerId = 0;
        public HeadRelation HeadRelation = HeadRelation.SticksToHead;
        public HandType SuggestedHand = HandType.Right;
        public Quaternion Orientation;
        public Vector3? Position;
        public double AnalogX;
        public double AnalogY;
        public double AnalogTrigger;
        public bool IsMenuPressed;
        public bool IsSystemPressed;
        public bool IsTriggerPressed;
        public bool IsGripPressed;
        public bool IsTouchpadPressed;
        public bool IsTouchpadTouched;

        public Vector3 OrientationSign = Vector3.One;

        public ArduinoController(bool left)
        {
            ControllerId = left ? 0 : 1;
        }

        public void SetHand(bool left)
        {
            SuggestedHand = left ? HandType.Left : HandType.Right;
            ControllerId = left ? 0 : 1;
        }

        public void SetPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public void SetHeadRelation(int value)
        {
            HeadRelation = (HeadRelation)value;
        }
          
        public void ParseOrientation(ref string[] data)
        {
            Orientation.W = float.Parse(data[1]);
            Orientation.X = float.Parse(data[2]) * OrientationSign.X;
            Orientation.Z = float.Parse(data[3]) * OrientationSign.Z;
            Orientation.Y = float.Parse(data[4]) * OrientationSign.Y;
        }

        public void ParseButtons(ref string[] data)
        {
            if (data.Length != ButtonsCount)
                return;

            for (int i = 0; i < ButtonsCount; i++)
            {
                var pressed = int.Parse(data[i]) > 0;
                switch ((Buttons)i)
                {
                    case Buttons.Grip: IsGripPressed = pressed; break;
                    case Buttons.Menu: IsMenuPressed = pressed; break;
                    case Buttons.Trigger: IsTriggerPressed = pressed; break;
                }
            }
        }

        public void SetControllerState(VridgeRemote vridge)
        {
            vridge.Controller.SetControllerState(
                ControllerId,
                HeadRelation,
                SuggestedHand,
                Orientation,
                Position,
                AnalogX,
                AnalogY,
                AnalogTrigger,
                IsMenuPressed,
                IsSystemPressed,
                IsTriggerPressed,
                IsGripPressed,
                IsTouchpadPressed,
                IsTouchpadTouched);
        }
    }
}
