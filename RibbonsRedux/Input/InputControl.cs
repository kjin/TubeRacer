using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace RibbonsRedux.Input
{
    /// <summary>
    /// Represents a single control for the game.
    /// </summary>
    public class InputControl
    {
        //The keys that activate this control.
        List<Keys> keys;
        //The gamepad buttons that activate this control.
        List<Buttons> buttons;
        //The direction the left joystick must be tilted to activate this control.
        Vector2 leftJoystick;
        //The direction the right joystick must be tilted to activate this control.
        Vector2 rightJoystick;
        //Whether the left trigger can be used to activate this control.
        bool leftTrigger;
        //Whether the right trigger can be used to activate this control.
        bool rightTrigger;

        //The previous state of this control.
        //This is primarily used to tell if a control was "just" activated.
        float prevState;
        //The current state of this control.
        float currState;
        //The length of time this control has been activated.
        int activeTime;

        /// <summary>
        /// Constructs a new instance of InputControl.
        /// </summary>
        public InputControl()
        {
            keys = new List<Keys>();
            buttons = new List<Buttons>();
            activeTime = 0;
        }

        /// <summary>
        /// Adds a new key to this control.
        /// </summary>
        /// <param name="key">The key to add.</param>
        public void AddKey(Keys key)
        {
            keys.Add(key);
        }

        /// <summary>
        /// Adds a new button to this control.
        /// </summary>
        /// <param name="button">The button to add.</param>
        public void AddButton(Buttons button)
        {
            buttons.Add(button);
        }

        /// <summary>
        /// Allows the control to be activated when the left joystick is tilted in a certain direction.
        /// </summary>
        /// <param name="direction">The direction to tilt.</param>
        public void SetLeftJoystick(Vector2 direction)
        {
            leftJoystick = direction;
        }

        /// <summary>
        /// Allows the control to be activated when the right joystick is tilted in a certain direction.
        /// </summary>
        /// <param name="direction">The direction to tilt.</param>
        public void SetRightJoystick(Vector2 direction)
        {
            rightJoystick = direction;
        }

        /// <summary>
        /// Allows the control to be activated when the left trigger is pressed.
        /// </summary>
        /// <param name="activated">Whether the left trigger activates the control.</param>
        public void SetLeftTrigger(bool activated)
        {
            leftTrigger = activated;
        }

        /// <summary>
        /// Allows the control to be activated when the right trigger is pressed.
        /// </summary>
        /// <param name="activated">Whether the right trigger activates the control.</param>
        public void SetRightTrigger(bool activated)
        {
            rightTrigger = activated;
        }

        /// <summary>
        /// Updates this control.
        /// </summary>
        /// <param name="keyState">The state of the keyboard.</param>
        /// <param name="padState">The state of the gamepad.</param>
        public void Update(KeyboardState keyState, GamePadState padState)
        {
            prevState = currState;
            currState = 0;
            for (int i = 0; i < keys.Count; i++)
                currState += keyState.IsKeyDown(keys[i]) ? 1 : 0;
            for (int i = 0; i < buttons.Count; i++)
                currState += padState.IsButtonDown(buttons[i]) ? 1 : 0;
            currState += Math.Max(Vector2.Dot(padState.ThumbSticks.Left, leftJoystick), 0);
            currState += Math.Max(Vector2.Dot(padState.ThumbSticks.Right, rightJoystick), 0);
            currState += leftTrigger ? padState.Triggers.Left : 0;
            currState += rightTrigger ? padState.Triggers.Right : 0;
            currState = MathHelper.Clamp(currState, 0f, 1f);
            if (currState == 0f)
                activeTime = 0;
            else
                activeTime++;
        }

        /// <summary>
        /// Gets the value of this control. Zero means this control is not activated.
        /// </summary>
        public float Value
        {
            get
            {
                return currState;
            }
        }

        /// <summary>
        /// Gets the active time of this control. Zero means this control is not activated.
        /// </summary>
        public int ActiveTime
        {
            get
            {
                return activeTime;
            }
        }

        /// <summary>
        /// Gets whether the control is "pressed" (activated).
        /// </summary>
        public bool Pressed
        {
            get
            {
                return currState > 0;
            }
        }

        /// <summary>
        /// Gets whether the control was just "pressed" (activated).
        /// </summary>
        public bool JustPressed
        {
            get
            {
                return currState > 0 && prevState == 0;
            }
        }

        /// <summary>
        /// Gets whether the control is "released" (de-activated).
        /// </summary>
        public bool Released
        {
            get
            {
                return currState == 0;
            }
        }

        /// <summary>
        /// Gets whether the control was just "released" (de-activated).
        /// </summary>
        public bool JustReleased
        {
            get
            {
                return currState == 0 && prevState > 0;
            }
        }
    }
}
