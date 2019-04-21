using UnityEngine;

namespace OSSCtrlR
{
    public abstract class DataClient : MonoBehaviour
    {
        public const int MaxTrackedObjects = 4;
        protected string[] _tmp = null;
        protected TrackedObject[] _trackedObjects = new TrackedObject[MaxTrackedObjects];

        [SerializeField]
        private bool _logEnabled = true;
        [SerializeField]
        private Vector3 _rotationSign = -Vector3.one;

        protected void Awake()
        {
            _trackedObjects[0] = new TrackedController();
            _trackedObjects[1] = new TrackedController();
            _trackedObjects[2] = new TrackedObject();
            _trackedObjects[3] = new TrackedObject();
        }

        public void GetLocalRotation(int which, ref Quaternion rotation)
        {
            _trackedObjects[which].GetOrientation(ref rotation);
        }

        public Quaternion GetLocalRotation(int which)
        {
            Quaternion quaternion = Quaternion.identity;
            GetLocalRotation(which, ref quaternion);
            return quaternion;
        }

        public TrackedController GetController(int which)
        {
            if (which > 1)
                return null;

            return (TrackedController)_trackedObjects[which];
        }

        public TrackedObject GetTrackedObject(int which)
        {
            if (which >= MaxTrackedObjects)
                return null;

            return _trackedObjects[which];
        }

        protected void ParseData(string data, int which)
        {
            ParseData(ref data, which);
        }

        protected virtual void ParseData(ref string data, int which)
        {
            _tmp = data.Split('|');

            if (_tmp[0] == "q")
                _trackedObjects[which].UpdateOrientation(ref _tmp, ref _rotationSign);

            if (which < 2)
            {
                var controller = (TrackedController)_trackedObjects[which];

                if (_tmp[0] == "b")
                    controller.SetDigitalButtons(ref _tmp);

                else if (_tmp[0] == "a")
                    controller.SetAnalogButtons(ref _tmp);
            }
        }

        protected void Log(string data)
        {
            if (_logEnabled)
                Debug.Log(data);
        }
    }
}
