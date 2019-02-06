using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamStor.Engine;

namespace TeamStor.AOD
{
    /// <summary>
    /// Input map that can be rebound.
    /// </summary>
    public static class InputMap
    {
        public struct Mapping
        {
            public Mapping(InputAction action, Keys key, Buttons gamepadButton)
            {
                Action = action;
                Key = key;
                GamepadButton = gamepadButton;
            }

            /// <summary>
            /// Action to be performed.
            /// </summary>
            public InputAction Action;

            /// <summary>
            /// Key on the keyboard to perform the action.
            /// Can be Keys.None
            /// </summary>
            public Keys Key;

            /// <summary>
            /// Gamepad button to perform this action.
            /// Can be (Buttons)0
            /// </summary>
            public Buttons GamepadButton;

            /// <param name="input">Input manager to check against.</param>
            /// <returns>If this mapping is pressed on either the keyboard or any gamepad.</returns>
            public bool Held(InputManager input)
            {
                return input.Key(Key) ||
                       input.Gamepad(0).IsButtonDown(GamepadButton) ||
                       input.Gamepad(1).IsButtonDown(GamepadButton) ||
                       input.Gamepad(2).IsButtonDown(GamepadButton) ||
                       input.Gamepad(3).IsButtonDown(GamepadButton);
            }

            /// <param name="input">Input manager to check against.</param>
            /// <returns>If this mapping was pressed this frame either the keyboard or any gamepad.</returns>
            public bool Pressed(InputManager input)
            {
                return input.KeyPressed(Key) || 
                    (input.Gamepad(0).IsButtonDown(GamepadButton) && input.LastGamepad(0).IsButtonUp(GamepadButton)) ||
                    (input.Gamepad(1).IsButtonDown(GamepadButton) && input.LastGamepad(1).IsButtonUp(GamepadButton)) ||
                    (input.Gamepad(2).IsButtonDown(GamepadButton) && input.LastGamepad(2).IsButtonUp(GamepadButton)) ||
                    (input.Gamepad(3).IsButtonDown(GamepadButton) && input.LastGamepad(3).IsButtonUp(GamepadButton));
            }

            /// <param name="input">Input manager to check against.</param>
            /// <returns>If this mapping was released this frame either the keyboard or any gamepad.</returns>
            public bool Released(InputManager input)
            {
                return input.KeyReleased(Key) ||
                    (input.Gamepad(0).IsButtonUp(GamepadButton) && input.LastGamepad(0).IsButtonDown(GamepadButton)) ||
                    (input.Gamepad(1).IsButtonUp(GamepadButton) && input.LastGamepad(1).IsButtonDown(GamepadButton)) ||
                    (input.Gamepad(2).IsButtonUp(GamepadButton) && input.LastGamepad(2).IsButtonDown(GamepadButton)) ||
                    (input.Gamepad(3).IsButtonUp(GamepadButton) && input.LastGamepad(3).IsButtonDown(GamepadButton));
            }
        }

        private static Dictionary<InputAction, Mapping> _defaultMappings = new Dictionary<InputAction, Mapping>();
        private static Dictionary<InputAction, Mapping> _mappings = new Dictionary<InputAction, Mapping>();

        static InputMap()
        {
            // Default buttons
            _defaultMappings.Add(InputAction.Up, new Mapping(InputAction.Up, Keys.Up, Buttons.DPadUp));
            _defaultMappings.Add(InputAction.Down, new Mapping(InputAction.Down, Keys.Down, Buttons.DPadDown));
            _defaultMappings.Add(InputAction.Left, new Mapping(InputAction.Left, Keys.Left, Buttons.DPadLeft));
            _defaultMappings.Add(InputAction.Right, new Mapping(InputAction.Right, Keys.Right, Buttons.DPadRight));

            _defaultMappings.Add(InputAction.Action, new Mapping(InputAction.Action, Keys.Z, Buttons.A));
            _defaultMappings.Add(InputAction.Back, new Mapping(InputAction.Back, Keys.Escape, Buttons.X));
            _defaultMappings.Add(InputAction.Inventory, new Mapping(InputAction.Inventory, Keys.E, Buttons.Y));
            _defaultMappings.Add(InputAction.Run, new Mapping(InputAction.Run, Keys.LeftShift, Buttons.LeftShoulder));

            foreach(InputAction action in Enum.GetValues(typeof(InputAction)))
                ResetMappingToDefault(action);
        }

        /// <param name="action">The action to find a mapping for.</param>
        /// <returns>The mapping for the specified action.</returns>
        public static Mapping FindMapping(InputAction action)
        {
            return _mappings[action];
        }

        /// <summary>
        /// Updates a mapping to the new value.
        /// </summary>
        /// <param name="newMapping">The mapping to update. newMapping.Action will be replaced.</param>
        /// <returns>The new mapping.</returns>
        public static Mapping UpdateMapping(Mapping newMapping)
        {
            _mappings[newMapping.Action] = newMapping;
            return newMapping;
        }

        /// <summary>
        /// Updates a mapping's key to the new value.
        /// </summary>
        /// <param name="action">The action to update.</param>
        /// <param name="newKey">The new key to give the action. Can be Keys.None.</param>
        /// <returns>The new mapping.</returns>
        public static Mapping UpdateMappingKey(InputAction action, Keys key)
        {
            return UpdateMapping(new Mapping(action, key, _mappings[action].GamepadButton));
        }

        /// <summary>
        /// Updates a mapping's gamepad button to the new value.
        /// </summary>
        /// <param name="action">The action to update.</param>
        /// <param name="newKey">The new button to give the action. Can be (Buttons)0.</param>
        /// <returns>The new mapping.</returns>
        public static Mapping UpdateMappingGamepad(InputAction action, Buttons button)
        {
            return UpdateMapping(new Mapping(action, _mappings[action].Key, button));
        }

        /// <summary>
        /// Resets a mapping to its default value.
        /// </summary>
        /// <param name="action">The action to update.</param>
        /// <returns>The default mapping for the action.</returns>
        public static Mapping ResetMappingToDefault(InputAction action)
        {
            if(!_mappings.ContainsKey(action))
                _mappings.Add(action, _defaultMappings[action]);
            else
                _mappings[action] = _defaultMappings[action];

            return _mappings[action];
        }
    }
}
