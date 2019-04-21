using UnityEngine;

namespace OSSCtrlR
{
    public class MotionObject : MonoBehaviour
    {
        protected Quaternion _rotation = Quaternion.identity;
        protected Transform _transform = null;
        protected TrackedObject _trackedObject = null;

        [SerializeField]
        protected int _trackerID = 0;
        [SerializeField]
        protected DataClient _dataClient = null;
        [SerializeField]
        protected float _damping = 0.0f;
        [SerializeField]
        private Vector3 _orientationSign = Vector3.zero;

        public float Damping
        {
            get { return _damping; }
            set { _damping = value; }
        }

        protected virtual void Start()
        {
            if (!_dataClient)
            {
                _dataClient = FindObjectOfType<DataClient>();

                if (!_dataClient)
                {
                    Debug.LogError("Data Client was not found");
                    enabled = false;
                    return;
                }
            }

            _trackedObject = _dataClient.GetTrackedObject(_trackerID);
            _transform = GetComponent<Transform>();
        }

        protected virtual void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
                _trackedObject.Recenter();

            _trackedObject.OrientationSign = _orientationSign;
            _trackedObject.GetOrientation(ref _rotation);

            if (_damping == 0)
                _transform.localRotation = _rotation;
            else
                _transform.localRotation = Quaternion.Slerp(_transform.localRotation, _rotation, Time.deltaTime * _damping);
        }
    }
}
