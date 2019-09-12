using System;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpDX;

namespace ExileCore.Shared.Nodes
{
    public class HotkeyNode
    {
        private bool _pressed;
        private bool _unPressed;
        [JsonIgnore] public Action OnValueChanged = delegate { };
        private Keys value;

        public HotkeyNode()
        {
            value = Keys.Space;
        }

        public HotkeyNode(Keys value)
        {
            Value = value;
        }

        public Keys Value
        {
            get => value;
            set
            {
                if (this.value != value)
                {
                    this.value = value;

                    try
                    {
                        OnValueChanged();
                    }
                    catch
                    {
                        DebugWindow.LogMsg("Error in function that subscribed for: HotkeyNode.OnValueChanged", 10, Color.Red);
                    }
                }
            }
        }

        public static implicit operator Keys(HotkeyNode node)
        {
            return node.Value;
        }

        public static implicit operator HotkeyNode(Keys value)
        {
            return new HotkeyNode(value);
        }

        public bool PressedOnce()
        {
            if (Input.IsKeyDown(value))
            {
                if (_pressed)
                    return false;

                _pressed = true;
                return true;
            }

            _pressed = false;
            return false;
        }

        public bool UnpressedOnce()
        {
            if (Input.GetKeyState(value))
                _unPressed = true;
            else
            {
                if (_unPressed)
                {
                    _unPressed = false;
                    return true;
                }
            }

            return false;
        }
    }
}
