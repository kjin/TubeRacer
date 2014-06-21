using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RibbonsRedux.Content;

namespace RibbonsRedux.Input
{
    /// <summary>
    /// Controls the input for the game.
    /// </summary>
    public class InputController
    {
        string[] controlNames = { "menuLeft",
                                         "menuRight",
                                         "menuUp",
                                         "menuDown",
                                         "menuForward",
                                         "menuBackward",
                                         "seamstressLeft",
                                         "seamstressRight",
                                         "seamstressJump",
                                         "ribbonLeft",
                                         "ribbonRight",
                                         "ribbonFlip",
                                         "cameraLeft",
                                         "cameraRight",
                                         "cameraUp",
                                         "cameraDown",
                                         "zoom",
                                         "pause",
                                         "debug1",
                                         "debug2",
                                         "screencap" };
        
        InputControl[] controls;
        InputControl allKeys;

        bool controllerConnected;

        public InputControl MenuLeft { get { return controls[0]; } }
        public InputControl MenuRight { get { return controls[1]; } }
        public InputControl MenuUp { get { return controls[2]; } }
        public InputControl MenuDown { get { return controls[3]; } }
        public InputControl MenuForward { get { return controls[4]; } }
        public InputControl MenuBackward { get { return controls[5]; } }
        public InputControl SeamstressLeft { get { return controls[6]; } }
        public InputControl SeamstressRight { get { return controls[7]; } }
        public InputControl SeamstressJump { get { return controls[8]; } }
        public InputControl RibbonLeft { get { return controls[9]; } }
        public InputControl RibbonRight { get { return controls[10]; } }
        public InputControl RibbonFlip { get { return controls[11]; } }
        public InputControl CameraLeft { get { return controls[12]; } }
        public InputControl CameraRight { get { return controls[13]; } }
        public InputControl CameraUp { get { return controls[14]; } }
        public InputControl CameraDown { get { return controls[15]; } }
        public InputControl Zoom { get { return controls[16]; } }
        public InputControl Pause { get { return controls[17]; } }
        public InputControl Debug { get { return controls[18]; } }
        public InputControl DebugAlt { get { return controls[19]; } }
        public InputControl ScreenCap { get { return controls[20]; } }
        public InputControl AllKeys { get { return allKeys; } }

        public bool ControllerConnected { get { return controllerConnected; } }

        /// <summary>
        /// Constructs a new InputController.
        /// </summary>
        /// <param name="assets">The AssetManager object used in the game.</param>
        public InputController(AssetManager assets)
        {
            controllerConnected = GamePad.GetState(PlayerIndex.One).IsConnected;
            TextDictionary dict = new TextDictionary(assets.GetText("controls"));
            controls = new InputControl[controlNames.Length];
            for (int i = 0; i < controls.Length; i++)
            {
                controls[i] = new InputControl();
                if (dict.CheckObjectExists(controlNames[i]))
                {
                    if (dict.CheckPropertyExists(controlNames[i], "key"))
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys), dict.LookupString(controlNames[i], "key"));
                        controls[i].AddKey(key);
                    }
                    else
                    {
                        int j = 0;
                        while (dict.CheckPropertyExists(controlNames[i], "key" + j))
                        {
                            Keys key = (Keys)Enum.Parse(typeof(Keys), dict.LookupString(controlNames[i], "key" + j));
                            controls[i].AddKey(key);
                            j++;
                        }
                    }
                    if (dict.CheckPropertyExists(controlNames[i], "button"))
                    {
                        Buttons button = (Buttons)Enum.Parse(typeof(Buttons), dict.LookupString(controlNames[i], "button"));
                        controls[i].AddButton(button);
                    }
                    else
                    {
                        int j = 0;
                        while (dict.CheckPropertyExists(controlNames[i], "button" + j))
                        {
                            Buttons button = (Buttons)Enum.Parse(typeof(Buttons), dict.LookupString(controlNames[i], "button" + j));
                            controls[i].AddButton(button);
                            j++;
                        }
                    }
                    if (dict.CheckPropertyExists(controlNames[i], "leftjoystick"))
                    {
                        Vector2 dir = dict.LookupVector2(controlNames[i], "leftjoystick");
                        controls[i].SetLeftJoystick(dir);
                    }
                    if (dict.CheckPropertyExists(controlNames[i], "rightjoystick"))
                    {
                        Vector2 dir = dict.LookupVector2(controlNames[i], "rightjoystick");
                        controls[i].SetRightJoystick(dir);
                    }
                    if (dict.CheckPropertyExists(controlNames[i], "lefttrigger"))
                    {
                        bool activated = dict.LookupBoolean(controlNames[i], "lefttrigger");
                        controls[i].SetLeftTrigger(activated);
                    }
                    if (dict.CheckPropertyExists(controlNames[i], "righttrigger"))
                    {
                        bool activated = dict.LookupBoolean(controlNames[i], "righttrigger");
                        controls[i].SetRightTrigger(activated);
                    }
                }
            }
            
            var keyNames = Enum.GetValues(typeof(Keys));
            allKeys = new InputControl();
            foreach (Keys key in keyNames)
                allKeys.AddKey(key);
            allKeys.AddButton(Buttons.A);
            allKeys.AddButton(Buttons.B);
            allKeys.AddButton(Buttons.Back);
            allKeys.AddButton(Buttons.LeftShoulder);
            allKeys.AddButton(Buttons.LeftTrigger);
            allKeys.AddButton(Buttons.RightShoulder);
            allKeys.AddButton(Buttons.RightTrigger);
            allKeys.AddButton(Buttons.Start);
            allKeys.AddButton(Buttons.X);
            allKeys.AddButton(Buttons.Y);
        }

        /// <summary>
        /// Updates the state of the input controller.
        /// </summary>
        public void Update()
        {
            KeyboardState keyState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            for (int i = 0; i < controls.Length; i++)
                controls[i].Update(keyState, padState);
            allKeys.Update(keyState, padState);
        }
    }
}
