using UnityEngine;

namespace OSSCtrlR
{
    public class MotionController : MotionObject
    {
        private TrackedController _controller = null;

        [SerializeField]
        private bool _leftController = true;

        protected override void Start()
        {
            _trackerID = _leftController ? 0 : 1;
            base.Start();
            _controller = (TrackedController)_trackedObject;
        }
    }
}
