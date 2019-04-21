using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OSSCtrlR
{
    public class VisualController : MonoBehaviour
    {
        private Color[] _colors = new Color[5];
        private TrackedController _controller = null;

        [Header("Renderers")]
        [SerializeField]
        private Renderer _home = null;
        [SerializeField]
        private Renderer _app = null;
        [SerializeField]
        private Renderer _dpad = null;
        [SerializeField]
        private Renderer _trigger = null;
        [SerializeField]
        private Renderer _grip = null;

        [Header("Settings")]
        [SerializeField]
        private int _controllerID = 0;

        private void Start()
        {
            _colors[0] = _home.material.color;
            _colors[1] = _app.material.color;
            _colors[2] = _dpad.material.color;
            _colors[3] = _trigger.material.color;
            _colors[4] = _grip.material.color;
            _controller = FindObjectOfType<DataClient>().GetController(_controllerID);
        }

        private void Update()
        {
            _home.material.color = _controller.GetButton(Buttons.Home) ? Color.red : _colors[0];
            _app.material.color = _controller.GetButton(Buttons.App) ? Color.red : _colors[1];
            _dpad.material.color = (_controller.GetButton(Buttons.Up) || _controller.GetButton(Buttons.Down) || _controller.GetButton(Buttons.Left) || _controller.GetButton(Buttons.Right)) ? Color.red : _colors[2];
            _trigger.material.color = _controller.GetAxis(Axis1D.Trigger) > 0.1f ? Color.red : _colors[3];
            _grip.material.color = _controller.GetAxis(Axis1D.Grip) > 0.1f ? Color.red : _colors[4];
        }
    }
}
