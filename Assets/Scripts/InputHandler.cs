using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThanFramework
{
    public class InputHandler : MonoBehaviour
    {
        //These inputs are lists of common keys for said movement/action
        public static List<KeyCode> LeftKeyControl = new List<KeyCode>() { KeyCode.A, KeyCode.LeftArrow };
        public static List<KeyCode> RightKeyControl = new List<KeyCode>() { KeyCode.D, KeyCode.RightArrow };
        public static List<KeyCode> UpKeyControl = new List<KeyCode>() { KeyCode.W, KeyCode.UpArrow };
        public static List<KeyCode> DownKeyControl = new List<KeyCode>() { KeyCode.S, KeyCode.DownArrow };

        public static List<KeyCode> AnyJoystick1Button = new List<KeyCode>() { KeyCode.Joystick1Button0, KeyCode.Joystick1Button1, KeyCode.Joystick1Button2, KeyCode.Joystick1Button3, KeyCode.Joystick1Button4, KeyCode.Joystick1Button5, KeyCode.Joystick1Button6, KeyCode.Joystick1Button7, KeyCode.Joystick1Button8, KeyCode.Joystick1Button9, KeyCode.Joystick1Button10, KeyCode.Joystick1Button11, KeyCode.Joystick1Button12, KeyCode.Joystick1Button13, KeyCode.Joystick1Button14, KeyCode.Joystick1Button15, KeyCode.Joystick1Button16, KeyCode.Joystick1Button17, KeyCode.Joystick1Button18, KeyCode.Joystick1Button19 };
        public static List<KeyCode> AnyJoystickButton = new List<KeyCode>() { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3, KeyCode.JoystickButton4, KeyCode.JoystickButton5, KeyCode.JoystickButton6, KeyCode.JoystickButton7, KeyCode.JoystickButton8, KeyCode.JoystickButton9, KeyCode.JoystickButton10, KeyCode.JoystickButton11, KeyCode.JoystickButton12, KeyCode.JoystickButton13, KeyCode.JoystickButton14, KeyCode.JoystickButton15, KeyCode.JoystickButton16, KeyCode.JoystickButton17, KeyCode.JoystickButton18, KeyCode.JoystickButton19 };

        public static List<KeyCode> JoystickSprint = new List<KeyCode>() { KeyCode.JoystickButton0, KeyCode.JoystickButton1, KeyCode.JoystickButton2, KeyCode.JoystickButton3 };
        public static List<KeyCode> KeyboardSprint = new List<KeyCode>() { KeyCode.Space, KeyCode.LeftShift };

        //public static Vector2 LAxis;
        //public static Vector2 RAxis;

        //public static Vector2 RawRAxis; //not normalized version of RAxis, basically the mouse position relative to the centerpoint.

        public static bool isCancelled = false;

        //public static Vector2 savedRAxis;
        public static float angleThreshold = 10;

        /*
        //Setup controller buttons
        static Vector2 LStick;
        static Vector2 RStick;
        static float horAxis2;
        static float vertAxis2;
        static float horDpad;
        static float vertDpad;
        static float rTrigger;
        static float lTrigger;
        */

        //public static bool sprintButton;

        //Adjust controllers with better deadzones
        public static float deadzone = 0.25f;

        public static bool grabTrigger;

        public static Vector2 mouseScreenPos;
        public static Vector2 mousePos;
        public static bool mouseOutsideScreen;

        static public float buttonToggleTimeLength = .3f;
        static public float mouseToggleTimeLength = .15f;
        static float toggleTimeLength;
        static bool isTriggerToggled = false;
        public static bool initTriggerPress = false;
        static float saveToggleTime = 0;

        //STARTING WITH CONTROLLER OR NOT?
        public static bool isController = false;

        //Setup Mouse Buttons
        public static int mouseTriggerButton = 0; //if the user would like to change the trigger button (if using mouse), this allows it to.

        //enum MouseButton {zero, one, two};
        //Additional mouse button setup

        public static bool GetAnyButtonDown(List<KeyCode> currentInput)
        {
            //This checks to see if any input that takes multiple keys is pressed

            //Make a loop that checks every key in list to see if it's pressed, then return true if any are
            for (int i = 0; i < currentInput.Count; i++)
                if (Input.GetKeyDown(currentInput[i]))
                    return true;

            return false;
        }

        public static bool GetAnyButtonUp(List<KeyCode> currentInput)
        {
            //Ditto of previous function - but instead check if input is released
            for (int i = 0; i < currentInput.Count; i++)
                if (Input.GetKeyUp(currentInput[i]))
                    return true;

            return false;
        }

        public static bool GetAnyButton(List<KeyCode> currentInput)
        {
            //Ditto of previous function - but checks if the input is being pressed
            for (int i = 0; i < currentInput.Count; i++)
                if (Input.GetKey(currentInput[i]))
                    return true;

            return false;
        }

        /*
        static void KeyboardInputManager()
        {
            //See functions GetAnyButtonDown, GetAnyButtonUp, and GetAnyButton for more info, keep note that here they serve the same purpose as Input.GetKey etc.

            //These controls are set in a way that the player can pivot by pressing and releasing their buttons (even while holding the other)
            //If we press the up button - go up
            if (GetAnyButtonDown(UpKeyControl))
                LAxis.y = 1;
            //If we release the up button and the down button is pressed - go down
            if (GetAnyButtonUp(UpKeyControl) && GetAnyButton(DownKeyControl))
                LAxis.y = -1;

            //If we press the down button - go down
            if (GetAnyButtonDown(DownKeyControl))
                LAxis.y = -1;
            //If we release the down button and the up button is pressed - go up
            if (GetAnyButtonUp(DownKeyControl) && GetAnyButton(UpKeyControl))
                LAxis.y = 1;

            //If neither buttons are pressed - don't move
            if (!GetAnyButton(UpKeyControl) && !GetAnyButton(DownKeyControl))
                LAxis.y = 0;

            if (GetAnyButtonDown(RightKeyControl))
                LAxis.x = 1;
            if (GetAnyButtonUp(RightKeyControl) && GetAnyButton(LeftKeyControl))
                LAxis.x = -1;

            if (GetAnyButtonDown(LeftKeyControl))
                LAxis.x = -1;
            if (GetAnyButtonUp(LeftKeyControl) && GetAnyButton(RightKeyControl))
                LAxis.x = 1;

            if (!GetAnyButton(RightKeyControl) && !GetAnyButton(LeftKeyControl))
                LAxis.x = 0;

            //If moving diagonal, keep the input proportionate (keyboard only)
            //float axisProportion = .75f;
            if (LAxis.x != 0 && LAxis.y != 0)
            {
                if (Mathf.Abs(LAxis.x) > Definitions.axisProportion && Mathf.Abs(LAxis.y) > Definitions.axisProportion)
                {
                    LAxis = new Vector2(LAxis.x * Definitions.axisProportion, LAxis.y * Definitions.axisProportion);
                }
            }
            else //If we aren't moving diagonally, make sure that our active movement is Sign (either -1 or 1)
            {
                if (LAxis.x != 0)
                {
                    LAxis.x = Mathf.Sign(LAxis.x);
                }

                if (LAxis.y != 0)
                {
                    LAxis.y = Mathf.Sign(LAxis.y);
                }
            }

            //Sprint button
            sprintButton = GetAnyButton(KeyboardSprint);
        }
        */
        static void MouseInputManager()
        {
            //Get Mouse Position
            mouseScreenPos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            /*
            if (GameObject.Find("Player"))
            {
                //Makes sure our local mouse position calculates from the correct object (player or locked on object)
                //GameObject anchorObject = LineScript.aimState == AimState.grab ? GameObject.Find(player.GetComponent<LineScript>().aimTarget.obj.name) : GameObject.Find("Player");
                //RawRAxis = mousePos - anchorObject.GetComponent<Definitions>().TransformPosition2D;

                //GameObject anchorObject = Player.raySpec.focalObject;
                Vector2 rootPos = Player.teleState == TeleState.control ? Player.raySpec.grabCenter : Player.aimStartPoint;
                RawRAxis = mousePos - rootPos;

                //Set our RAxis to replicate a joystick
                RAxis = ApplyDeadzone(RawRAxis, deadzone);// / 2); //Probably could come up with a better deadzone equation that accounts for the key object size or collision
            }
            */

            if (Input.mousePosition.x <= 0 || Input.mousePosition.y <= 0 || Input.mousePosition.x >= Screen.width - 1 || Input.mousePosition.y >= Screen.height - 1)
            {
                mouseOutsideScreen = true;
            }
            else
            {
                mouseOutsideScreen = false;
            }

            toggleTimeLength = mouseToggleTimeLength;  //Our threshold for how long we can hold a button before it loses it's toggle ability, the threshold for the mouse button is shorter than controller buttons
            grabTrigger = CheckToggle(Input.GetMouseButton(mouseTriggerButton));

        }

        static void TouchInputManager()
        {
            if (Input.touchCount > 0)
            {
                grabTrigger = true;
                Touch touch = Input.GetTouch(0);
                mouseScreenPos = touch.position;
                mousePos = Camera.main.ScreenToWorldPoint(touch.position);
            }
            else
            {
                grabTrigger = false;
            }

        }
        /*
        static void JoystickInputManager()
        {
            //Left Joystick
            LAxis = ApplyDeadzone(LStick);

            //Right Joystick
            RAxis = ApplyDeadzone(RStick);

            RawRAxis = RAxis; //Since RawRAxis is used for mouse position, we can just make it RAxis for the controller

            //Sprint button
            sprintButton = GetAnyButton(JoystickSprint);

            //Trigger button
            toggleTimeLength = buttonToggleTimeLength; //Our threshold for how long we can hold a button before it loses it's toggle ability, the threshold for controller buttons is longer than the mouse button
            grabTrigger = CheckToggle((rTrigger > 0 ? true : false));
        }
        
        static Vector2 ApplyDeadzone(Vector2 joystick) //Applies the set deadzone to the given Vector2
        {
            if (joystick.magnitude < deadzone)
            {
                joystick = Vector2.zero;
            }
            else
            {
                joystick = joystick.normalized * ((joystick.magnitude - deadzone) / (1 - deadzone));

                if (joystick.magnitude > 1) //Make sure the joystick doesn't exceed bounds for some reason
                {
                    joystick = joystick.normalized;
                }
            }

            return joystick;
        }

        static Vector2 ApplyDeadzone(Vector2 joystick, float customDeadzone) //same as before but allows for a custom deadzone
        {
            if (joystick.magnitude < customDeadzone)
            {
                joystick = Vector2.zero;
            }
            else
            {
                joystick = joystick.normalized * ((joystick.magnitude - customDeadzone) / (1 - customDeadzone));

                if (joystick.magnitude > 1)
                {
                    joystick = joystick.normalized;
                }
            }

            return joystick;
        }

        static bool JoystickCheck()
        {
            Vector2 tempStick;

            //Left Joystick
            tempStick = LStick;
            if (tempStick.magnitude > deadzone)
                return true;

            //Right Joystick
            tempStick = RStick;
            if (tempStick.magnitude > deadzone)
                return true;

            //Check all triggers at once
            if (rTrigger + lTrigger + vertDpad + horDpad != 0)
                return true;

            //Check all buttons
            if (GetAnyButtonDown(AnyJoystick1Button))
                return true;

            return false;
        }

        static void ControlMapping()
        {
            //Setup for mapping controller
            LStick = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            RStick = new Vector2(Input.GetAxis("R_Horizontal"), Input.GetAxis("R_Vertical"));

            horDpad = Input.GetAxis("HorizontalDPad");
            vertDpad = Input.GetAxis("VerticalDPad");

            rTrigger = Input.GetAxis("R_Trigger");
            lTrigger = Input.GetAxis("L_Trigger");
        }
        */
        //Checks to see whether the player is planning on toggling the button or holding it
        static bool CheckToggle(bool isDown, bool triggerCondition1 = true, bool triggerCondition2 = true) //triggerConditions allow for additional checking before trigger activated
        {
            if (isDown) //if we are holding the button down
            {
                if (isTriggerToggled) //If we have already decided to toggle the button, we now set the button to false
                {
                    if (initTriggerPress == false) //was the button just pressed?
                    {
                        initTriggerPress = true;
                    }
                    return false;
                }
                else
                {
                    if (initTriggerPress == false) //was the button just pressed?
                    {
                        initTriggerPress = true;
                        saveToggleTime = Time.time; //set this time as the time to check later
                    }
                    return true;
                }
            }
            else if (Time.time > toggleTimeLength && Time.time - saveToggleTime <= toggleTimeLength && !isTriggerToggled && triggerCondition1 && triggerCondition2) //if the time we held the button is less than our toggleTimeLength threshhold (as well as a few bugfixing conditions)
            {
                //The previous condition checks for many a thing:
                //Time.time > toggleTimeLength -> if the game has been shorter than the elapsed time we can hold the trigger, than there will be bugs
                //Time.time - saveToggleTime <= toggleTimeLength -> The most important condition, are we check if the time that we held the trigger is less than our threshold to activate the button if it's more the player is probably planning on holding the button
                //!isTriggerToggled -> if this is trigger has not ALREADY been toggled
                //LineScript.aimState != AimState.deselect && LineScript.aimState != AimState.deselectCollide - > is the player able to grab an object?

                //We can now press the botton and allow us to toggle off
                initTriggerPress = false;
                isTriggerToggled = true;
                return true;
            }
            else if (isTriggerToggled) //if we just did the above, continue holding our value
            {
                if (initTriggerPress) //unless this is still the first button press
                {
                    isTriggerToggled = initTriggerPress = false;
                    return false;
                }
                return true;
            }
            else //If the player is planning on holding the button, then we deactivate on release
            {
                isTriggerToggled = initTriggerPress = false;
                return false;
            }
        }

        public static void ControlUpdate()
        {
            //ControlMapping();

            /*
            if (isController)
            {
                JoystickInputManager();

                //Check for keyboard input (switch to Keyboard and Mouse)
                //Checks for any key (and makes sure it's not a joystick button)
                foreach (KeyCode tempButton in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(tempButton))
                    {
                        for (int i = 0; i < AnyJoystickButton.Count; i++)
                            if (tempButton == AnyJoystickButton[i])
                            {
                                if (tempButton == AnyJoystick1Button[i])
                                {
                                    isController = false;
                                }
                            }
                    }
                }

                //Check for mouse input (switch to Keyboard and Mouse)
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    isController = false;
                }
            }
            else
            {
            */
            //KeyboardInputManager();


            if (Input.mousePosition.x == 0 || Input.mousePosition.y == 0 || Input.mousePosition.x == Screen.width - 1 || Input.mousePosition.y == Screen.height - 1)
                TouchInputManager();
            else
                MouseInputManager();

                //Check for controller input (switch to controller)
                //if (JoystickCheck())
                //{
                //    isController = true;
                //}
            //
        }
    }
}