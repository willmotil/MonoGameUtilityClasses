//using System;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    // mouse keyboard and text

    /// <summary>
    /// Will Motil 2010 last updated 2018
    /// </summary>
    public static class UserMouseInput
    {
        #region private methods or variables

        private static Vector2 ScreenSize { get; set; }
        private static Vector2 ScreenSizeMultiplier { get; set; }

        /// <summary>
        /// this is used in combination with another bool to prevent secondary updates per frame loop
        /// </summary>
        private static bool preventUpdatesRestOfThisFrame = false;
        /// <summary>
        /// when we actually are useing the preventSecondaryUpdates
        /// method we set this to true that allows for the prevent_update_rest_ofthisframe bool
        /// to be turned on and off as needed
        /// </summary>
        private static bool autoPreventMultipleUpdates_Flag = false;

        /// <summary>
        /// scroll wheel this will mainly affect the zoomvalue  its multiplyed by a small number to get the below
        /// </summary>
        private static int mouseWheelValue = 0;
        /// <summary>
        /// the old mouse wheel value depending on the frequency of this call 
        /// the discrepency between the two can be different
        /// </summary>
        private static int mouseOldWheelValue = 0;

        #endregion

        #region public methods or variables

        public static MouseState mouseState;

        /// <summary>
        /// set or get the X mouse position as a int
        /// </summary>
        public static int X
        {
            get;
            set;
        }
        /// <summary>
        /// set or get the Y mouse position as a int
        /// </summary>
        public static int Y
        {
            get;
            set;
        }
        /// <summary>
        /// This is calculated as the mouse position in relation to the size of the screens width height. 
        /// </summary>
        public static Vector2 VirtualPos = new Vector2(0.0f, 0.0f);// could use 2 floats here instead but
        /// <summary>
        /// the position of the mouse as a point
        /// </summary>
        public static Point Pos = new Point(-1, -1);
        /// <summary>
        /// <para>this can be used instead of the bool methods</para> 
        /// <para>+1 if the wheel is getting rolled upwards</para>
        /// <para>-1 if the wheel is getting rolled downwards</para>
        /// <para>and 0 when wheel is not being rolled</para>
        /// </summary>
        public static int mouseWheelUpOrDown = 0;// if its +1 mouse scoll wheel got moved up if its -1 it got scrolled down

        /// <summary>
        /// returns true if the wheel is being wheeled up
        /// </summary>
        public static bool WheelingUp
        {
            get
            {
                if (mouseWheelUpOrDown > 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// returns true if the wheel is wheeling down
        /// </summary>
        public static bool WheelingDown
        {
            get
            {
                if (mouseWheelUpOrDown < 0)
                    return true;
                else
                    return false;
            }
        }
        /// <summary>
        /// if the mouse wheel is not being wheeled up or down we return true
        /// </summary>
        public static bool WheelAtRest()
        {
            if (mouseWheelUpOrDown == 0)
                return true;
            else
                return false;
        }

        #endregion

        #region Left ______________________________

        /// <summary>
        /// is left being pressed now
        /// </summary>
        public static bool IsLeftDown = false;
        /// <summary>
        /// is left just clicked
        /// </summary>
        public static bool IsLeftClicked = false;
        /// <summary>
        /// is left being held down now
        /// </summary>
        public static bool IsLeftHeld = false;
        /// <summary>
        /// is true only in one single frame is the mouse just released
        /// </summary>
        public static bool IsLeftJustReleased = false;
        /// <summary>
        /// has the left mouse been dragged
        /// </summary>
        public static bool IsLeftDragged = false;
        /// <summary>
        /// left last position pressed while useing left mouse button
        /// </summary>
        public static Vector2 LastLeftPressedAt;
        /// <summary>
        /// left last position draged from before release while useing left mouse button
        /// </summary>
        public static Vector2 LastLeftDragReleased;
        /// <summary>
        /// Gets the direction and magnitude of left drag press to drag released
        /// </summary>
        public static Vector2 GetLeftDragVector()
        {
            return LastLeftDragReleased - LastLeftPressedAt;
        }
        /// <summary>
        /// Gets the left drag rectangle 
        /// this method doesn't ensure there has been one
        /// </summary>
        public static Rectangle GetLeftDragRectangle()
        {
            int x1 = (int)LastLeftPressedAt.X, x2 = (int)LastLeftDragReleased.X, y1 = (int)LastLeftPressedAt.Y, y2 = (int)LastLeftDragReleased.Y;
            if (x1 > x2) { int temp = x1; x1 = x2; x2 = temp; }
            if (y1 > y2) { int temp = y1; y1 = y2; y2 = temp; }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        #endregion

        #region Right ______________________________

        /// <summary>
        /// is right being pressed now
        /// </summary>
        public static bool IsRghtDown = false;
        /// <summary>
        /// is left just clicked
        /// </summary>
        public static bool IsRightClicked = false;
        /// <summary>
        /// is right being held down now
        /// </summary>
        public static bool IsRightHeld = false;
        /// <summary>
        /// right is true only in one single frame is the mouse just released
        /// </summary>
        public static bool IsRightJustReleased = false;
        /// <summary>
        /// has the right mouse been dragged
        /// </summary>
        public static bool IsRightDragged = false;
        /// <summary>
        /// right last position pressed while useing left mouse button
        /// </summary>
        public static Vector2 LastRightPressedAt;
        /// <summary>
        /// right last position draged from before release while useing left mouse button
        /// </summary>
        public static Vector2 LastRghtDragReleased;
        /// <summary>
        /// Gets the direction and magnitude of left drag press to drag released
        /// </summary>
        public static Vector2 GetRightDragVector()
        {
            return LastRghtDragReleased - LastRightPressedAt;
        }
        /// <summary>
        /// Gets the left drag rectangle 
        /// this method doesn't ensure there has been one
        /// </summary>
        public static Rectangle GetRightDragRectangle()
        {
            int x1 = (int)LastRightPressedAt.X, x2 = (int)LastRghtDragReleased.X, y1 = (int)LastRightPressedAt.Y, y2 = (int)LastRghtDragReleased.Y;
            if (x1 > x2) { int temp = x1; x1 = x2; x2 = temp; }
            if (y1 > y2) { int temp = y1; y1 = y2; y2 = temp; }
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        #endregion

        public static void PassTheScreenSize(int w, int h)
        {
            ScreenSize = new Vector2(w, h);
            ScreenSizeMultiplier = Vector2.One / ScreenSize;
        }

        /// <summary>
        /// When used this must be called seperately from update typically just before it
        /// </summary>
        public static void PreventMultipleUpdates()
        {
            autoPreventMultipleUpdates_Flag = true;
            preventUpdatesRestOfThisFrame = false;
        }

        /// <summary>
        /// Multi Frame logic function ( we dont time things here we rely on frame pass logic )
        /// this fuction ONLY needs to and should be called 1 time per frame 
        /// more then that will mess up the 
        /// tracking of the mousejustreleased value
        /// get the mouse values and save them to track what the mouse is doing exactly
        /// </summary>
        public static void Update()
        {
            if (preventUpdatesRestOfThisFrame == false)
            {
                // grab the mouse state from the input class
                mouseState = Mouse.GetState();
                // save the current mouse position into another variable for later use
                X = mouseState.Position.X;
                Y = mouseState.Position.Y;
                Pos = mouseState.Position;
                VirtualPos.X = (float)X * ScreenSizeMultiplier.X;
                VirtualPos.Y = (float)Y * ScreenSizeMultiplier.Y;

                // mouse wheel
                //  
                mouseWheelUpOrDown = 0;// 0 is the state of no action on the wheel
                mouseWheelValue = mouseState.ScrollWheelValue;
                //
                if (mouseWheelValue < mouseOldWheelValue)
                {
                    mouseWheelUpOrDown = -1;
                    mouseOldWheelValue = mouseWheelValue;
                }
                else
                {
                    if (mouseWheelValue > mouseOldWheelValue)
                    {
                        mouseWheelUpOrDown = +1;
                        mouseOldWheelValue = mouseWheelValue;
                    }
                }

                // mouse buttons
                // Processing when carrying out the left click of the mouse
                //_______________________________________________
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    // NOTE THESE ARE PLACED IN A SPECIFIC ORDER TO ENSURE THAT DRAG CLICK LOGIC IS CORRECT
                    // note to self thanks Never muck it up  , (this is a composite multi frame logic function)
                    // on the first pass the below will set the lastposmousepressed to the current position
                    if (IsLeftHeld == false) // this executes on the first pass and the second pass but not the third pass
                    {
                        IsLeftClicked = true;
                        if (IsLeftDown == true) // this condition will not execute untill the second pass thru the function
                        {
                            IsLeftHeld = true; // now we mark it as being held on the second pass 
                            IsLeftClicked = false;
                        }
                        else  // this executes on the first pass only
                        {
                            LastLeftPressedAt.X = X; // we save the click of the first point of holding 
                            LastLeftPressedAt.Y = Y; // so when released we know were we draged from
                        }
                    }
                    // set at the end of but still in the first pass
                    IsLeftDown = true;// we SET this after the call to is leftbutton pressed now to ensure next pass is active
                }
                else
                { // mouse itself is no longer registering the button pressed so.. toggle held and button pressed off
                    IsLeftDown = false;
                    IsLeftJustReleased = false; // added this so i can get a ... just now released value
                    IsLeftDragged = false;
                    if (IsLeftHeld == true)
                    {
                        LastLeftDragReleased.X = X;
                        LastLeftDragReleased.Y = Y;
                        IsLeftJustReleased = true; // this gets reset to zero on next pass its good for one frame
                        var ldr = GetLeftDragRectangle();
                        if (ldr.Width != 0 || ldr.Height != 0)
                            IsLeftDragged = true;
                    }
                    IsLeftHeld = false;
                }

                // Processing when carrying out the right click of the mouse
                //________________________________________________
                if (mouseState.RightButton == ButtonState.Pressed)
                {
                    //NOTE THESE ARE PLACED IN A SPECIFIC ORDER TO ENSURE THAT DRAG CLICK LOGIC IS CORRECT
                    // on the first pass the below will set the lastposmousepressed to the current position
                    if (IsRightHeld == false) // this executes on the first pass and the second pass but not the third pass
                    {
                        IsRightClicked = true;
                        if (IsRghtDown == true) // this condition will not execute untill the second pass thru the function
                        {
                            IsRightHeld = true; // now we mark it as being held on the second pass
                            IsRightClicked = false;
                        }
                        else  // this executes on the first pass only
                        {
                            LastRightPressedAt.X = X; // we save the click of the first point of holding 
                            LastRightPressedAt.Y = Y; // so when released we know were we draged from
                        }
                    }
                    // set at the end of the first pass
                    IsRghtDown = true;// we SET this after the call to is rightbutton pressed now to to ensure next pass is active
                }
                else
                { // right mouse button itself is no longer registering the button pressed so.. toggle held and button pressed off
                    IsRghtDown = false;
                    IsRightJustReleased = false;
                    IsRightDragged = false;
                    if (IsRightHeld == true)
                    {
                        LastRghtDragReleased.X = X;
                        LastRghtDragReleased.Y = Y;
                        IsRightJustReleased = true;
                        var rdr = GetRightDragRectangle();
                        if (rdr.Width != 0 || rdr.Height != 0)
                            IsRightDragged = true;
                    }
                    IsRightHeld = false;
                }
            }
            if (autoPreventMultipleUpdates_Flag)
            {
                preventUpdatesRestOfThisFrame = true;
            }
        }
    }
}
