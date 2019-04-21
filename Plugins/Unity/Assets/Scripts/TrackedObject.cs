using UnityEngine;

namespace OSSCtrlR
{
    public class TrackedObject
    {
        protected Quaternion m_orientation;
        protected Quaternion m_offset;
        protected bool m_needRecenter;

        public Vector3 OrientationSign { get; set; }

        public TrackedObject()
        {
            m_needRecenter = true;
            m_orientation = Quaternion.identity;
            m_offset = Quaternion.identity;
            OrientationSign = Vector3.zero;
        }

        public void Recenter()
        {
            m_offset = Quaternion.Inverse(m_orientation);
            m_needRecenter = false;
        }

        public void GetOrientation(ref Quaternion quaternion)
        {
            quaternion = m_offset * m_orientation;
        }

        public void UpdateOrientation(ref string[] data, ref Vector3 sign)
        {
            if (OrientationSign == Vector3.zero)
                OrientationSign = sign;

            m_orientation.w = float.Parse(data[1]);
            m_orientation.x = float.Parse(data[2]) * OrientationSign[0];
            m_orientation.z = float.Parse(data[3]) * OrientationSign[1];
            m_orientation.y = float.Parse(data[4]) * OrientationSign[2];

            if (m_needRecenter)
                Recenter();
        }
    }
}
