namespace OSSCtrlR
{
    public enum Buttons
    {
        App = 0, Home, Up, Down, Left, Right
    }

    public enum Axis1D
    {
        X, Y, Trigger, Grip
    }

    public class TrackedController : TrackedObject
    {
        private bool[] _buttons;
        private float[] _axis;

        public TrackedController()
            : base()
        {
            _buttons = new bool[2];
            _axis = new float[4];
        }

        public bool GetButton(Buttons button)
        {
            var id = (int)button;
            if (id < _buttons.Length)
                return _buttons[id];

            if (button == Buttons.Up)
                return _axis[2] < 0.5f;
            else if (button == Buttons.Down)
                return _axis[2] > 0.5f;
            else if (button == Buttons.Left)
                return _axis[3] < 0.5f;
            else if (button == Buttons.Right)
                return _axis[3] > 0.5f;

            return false;
        }

        public float GetAxis(Axis1D axis)
        {
            var id = (int)axis;
            if (id < _axis.Length)
                return _axis[(int)axis];

            return 0.0f;
        }

        public void SetDigitalButtons(ref string[] data)
        {
            if (data.Length - 1 != _buttons.Length)
                return;

            for (int i = 0; i < _buttons.Length; i++)
                _buttons[i] = int.Parse(data[i + 1]) > 0;
        }

        public void SetAnalogButtons(ref string[] data)
        {
            if (data.Length - 1 != _axis.Length)
                return;

            for (int i = 0; i < _axis.Length; i++)
                _axis[i] = float.Parse(data[i + 1]);
        }
    }
}
