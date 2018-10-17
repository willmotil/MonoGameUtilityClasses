using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FullScrWinTests3_7_Gl
{
    // this is test version 16
    public class Game1 : Game
    {

        // change to false to see all variable info per change
        bool just_display_changes = true;

        // this only works if just_display_changes is true
        bool display_unchanged_stuff = false;

        // runs a scripted test at start
        bool run_test_script = false;

        // show code in test
        bool show_code = true;

        // scripted and user pause amount
        int input_pause_amount = 15;

        // display message.
        string msg = "";

        // options string;
        string optionsMsg = "";

        GraphicsDeviceManager gdm;
        GraphicsDevice gd;
        SpriteBatch spriteBatch;
        SpriteFont mgFont;
        Texture2D gridTexture;
        Color gridColor1 =  new Color(10, 30, 50, 255);
        Color gridColor2 =  new Color(10, 40, 50, 255);
        Color textColor = Color.Moccasin;
        KeyboardState kbs;
        Point started = new Point(GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight);
        Point preset = new Point(1200, 720);
        Point preset2 = new Point(600, 600);
        Point preferred; // = new Point(600, 600);
        private bool was_resize_called = false;
        private bool was_apply_called = false;
        private int onresizecounter = 0;
        private int wasapplycalledcounter = 0;

        List<DisplayMode> bestDisplayModes = new List<DisplayMode>();
        List<DisplayMode> supportedDisplayModes = new List<DisplayMode>();

        DisplayMode starteddm;
        int startedbestdmindex = -1;

        int auto_script_cmd_value = -1;
        int auto_script_cmd_execution = -1;

        static string nline = Environment.NewLine;
        static string tab4 = "    ";
        static string dline = "__________________________________________________________________________________________________________________________";
        static string ndnline = nline + dline + nline + nline;


        public Game1()
        {
            IsMouseVisible = true;
            gdm = new GraphicsDeviceManager(this);
            gd = gdm.GraphicsDevice;
            Console.WriteLine(nline + "*** Game1 Constructor()");
            Console.WriteLine
                (
                nline + " the below changes take effect in this constructor" +
                nline + " gdm.PreferredBackBufferWidth  = preferred.X; " + preferred.X +
                nline + " gdm.PreferredBackBufferHeight = preferred.Y; " + preferred.Y +
                nline + " allow user resizing and register a callback for window.clientsizechanged"
                );
            gdm.PreferredBackBufferWidth = preferred.X;
            gdm.PreferredBackBufferHeight = preferred.Y;
            gdm.PreferredBackBufferFormat = SurfaceFormat.Color;
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;

            Window.ClientSizeChanged += OnResize;

            Content.RootDirectory = "Content";
            //gdm.SynchronizeWithVerticalRetrace = false;
            //gdm.IsFullScreen = true;
            //gdm.PreparingDeviceSettings += GdmPreparingDeviceSettings;
        }

        protected override void Initialize()
        {
            Console.WriteLine(nline + "*** Game Initialize()");
            gd = gdm.GraphicsDevice;
            ListSupportedDisplayModes();
            //Window.Title = gd.Adapter.DeviceName; // not yet implemented in monogame
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Console.WriteLine(nline + "*** Game LoadContent()");
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // create a texture
            gridTexture = CreateCheckerBoard(GraphicsDevice, 4, 4, gridColor1, gridColor2);
            //
            mgFont = Content.Load<SpriteFont>("MgFont");
            // get current display mode and find best matches
            starteddm = gd.Adapter.CurrentDisplayMode;
            bestDisplayModes = FindBestDisplayModes();
            startedbestdmindex = FindCurrentOrClosestDisplayModeInList(bestDisplayModes, starteddm);
            // printout what we are doing
            Console.WriteLine(nline + " started backbuffer was " + started + " set to prefered " + preferred);
            DisplayGdmGdWindowRecordedInfo();
            Console.WriteLine(nline + " Vtrace period gdm.GraphicsDevice.PresentationParameters.PresentationInterval " + gdm.GraphicsDevice.PresentationParameters.PresentationInterval);

            //// undefined preferred backbuffer
            //DisplayGdmGdWindowRecordedInfo();

            // under GL this is a bug at the time of 3.7 if this is not called well get a undefined preferred back buffer.
            preferred.X = preset2.X;
            preferred.Y = preset2.Y;
            gdm.PreferredBackBufferWidth = preferred.X;
            gdm.PreferredBackBufferHeight = preferred.Y;
            gdm.ApplyChanges();
            DisplayGdmGdWindowRecordedInfo();

            gd = GraphicsDevice;
            Console.WriteLine(
                nline + "__________________________________" +
                nline + "*** Leaving Game LoadContent()" +
                nline + "__________________________________" +
                nline + "*** Game Running" +
                nline + "__________________________________" +
                nline
                );
            DisplayOptions();
            string temp = msg;
            msg =
                "Hello this is a testing demo for MonoGames Fullscreen and Window capabilitys \n This app may also be helpful in debuging for this turn on the projects console window to see more detailed information"
                + nline + nline + temp;

        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            was_resize_called = false;
            was_apply_called = false;

            kbs = Keyboard.GetState();

            if (kbs.IsKeyDown(Keys.Escape))
                this.Exit();

            // switch preferred

            if ((kbs.IsKeyDown(Keys.F1) || auto_script_cmd_value == 1) && Paused == false)
            {
                msg = ndnline + "INPUT  F1   Setting gdm preferred backbuffer" + preset + nline;
                Console.WriteLine(msg);
                SetPreferredBackBufferThruGdm(preset.X, preset.Y);
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.F2) || auto_script_cmd_value == 2) && Paused == false)
            {
                msg = ndnline + "INPUT  F2   Setting gdm preferred backbuffer " + preset2 + nline;
                Console.WriteLine(msg);
                SetPreferredBackBufferThruGdm(preset2.X, preset2.Y);
                ApplyTheChanges();
            }

            if ((kbs.IsKeyDown(Keys.F3) || auto_script_cmd_value == 3) && Paused == false)
            {
                msg = ndnline + "INPUT  F3   set backbuffer thru gd presentation parameters  " + preset2 + " (to a non standard resolution)" + nline;
                Console.WriteLine(msg);
                SetBackBufferDirectlyThruGdPresentationParams(preset2.X, preset2.Y);
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.F4) || auto_script_cmd_value == 4) && Paused == false)
            {
                msg = ndnline + "INPUT  F4   List supported display modes" + nline;
                msg += nline + "     ___________________________" + nline;
                msg += nline + "     Current display mode index  " + FindCurrentDisplayModeInList().ToString() + nline;
                Console.WriteLine(msg);
                ListSupportedDisplayModes();
                FindBestDisplayModes();
                DisplayOptions();
            }
            if ((kbs.IsKeyDown(Keys.F5) || auto_script_cmd_value == 6) && Paused == false)
            {
                msg = (ndnline + "INPUT  F5   " + nline);
                Console.WriteLine(msg);
            }
            if ((kbs.IsKeyDown(Keys.F6) || auto_script_cmd_value == 6) && Paused == false)
            {
                msg = (ndnline + "INPUT  F6   Go into fullscreen -- Setting gdm preferred backbuffer" + started + nline);
                Console.WriteLine(msg);
                SetFullScreen(true);
                SetPreferredBackBufferThruGdm(gd.DisplayMode.Width, gd.DisplayMode.Height);
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.F7) || auto_script_cmd_value == 7) && Paused == false)
            {
                msg = (ndnline + "INPUT  F7   Backout of fullscreen -- Setting gdm preferred backbuffer" + started + nline);
                Console.WriteLine(msg);
                SetFullScreen(false);
                SetPreferredBackBufferThruGdm(started.X, started.Y);
                ApplyTheChanges();
            }

            if ((kbs.IsKeyDown(Keys.F8) || auto_script_cmd_value == 8) && Paused == false)
            {
                msg = (ndnline + "INPUT  F8   " + nline);
                Console.WriteLine(msg);
            }

            // attempt to switch to different display mode resolutions

            if ((kbs.IsKeyDown(Keys.F9) || auto_script_cmd_value == 9) && Paused == false)
            {
                msg = (ndnline + "INPUT  F9   attempting next displaymode" + nline);
                msg+= nline + (ndnline + "Current display mode index  " + FindCurrentDisplayModeInList().ToString() + nline + gd.Adapter.CurrentDisplayMode.Width + " ," + gd.Adapter.CurrentDisplayMode.Height + nline);
                Console.WriteLine(msg);
                ChangeToNextDisplayMode(true);
                //ApplyTheChanges();
            }

            // different ways to just call fullscreen

            if ((kbs.IsKeyDown(Keys.F10) || auto_script_cmd_value == 10) && Paused == false)
            {
                msg = (ndnline + "INPUT  F10   set to fullscreen via gdm applychanges" + nline);
                Console.WriteLine(msg);
                SetFullScreen(true);
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.F11) || auto_script_cmd_value == 11) && Paused == false)
            {
                msg = (ndnline + "INPUT  F11   set to windowed   via gdm applychanges" + nline);
                Console.WriteLine(msg);
                SetFullScreen(false);
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.F12) || auto_script_cmd_value == 12) && Paused == false)
            {
                msg = ndnline + "INPUT  F12   toggle fullscreen (their is no call to apply changes like this)" + nline;
                Console.WriteLine(msg);
                TheFullScreenToggle();
                // no apply changes is called for this normally in xna or xna calls it on its own
                ApplyTheChanges();
            }
            if ((kbs.IsKeyDown(Keys.H) || kbs.IsKeyDown(Keys.O)) && Paused == false)
            {
                msg = (ndnline + "Options H or O" + nline);
                Console.WriteLine(msg);
                DisplayOptions();
            }
            if ((kbs.IsKeyDown(Keys.I) && Paused == false))
            {
                msg = (ndnline + "INPUT  I   current display Information" + nline);
                Console.WriteLine(msg);
                msg = RecordDiff.DisplayCurrentStatus(gdm, gd, Window);
            }
            if ((kbs.IsKeyDown(Keys.T) && Paused == false))
            {
                msg = (ndnline + "INPUT  T   run auto Test Script" + nline);
                Console.WriteLine(msg);
                run_test_script = true;
                auto_script_cmd_value = -1;
                auto_script_cmd_execution = -1;
                DisplayOptions();
            }

            CheckForDoubleCalls();

            RunAutoScript();

            ReducePause();

            base.Update(gameTime);
        }

        SamplerState ss = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp, MaxAnisotropy = 0 };
        RasterizerState rs = new RasterizerState() { MultiSampleAntiAlias = false };

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            gd = GraphicsDevice;

            //spriteBatch.Begin();
            //gd.SamplerStates[0] = ss; // hummm 
            //gd.SamplerStates[1] = ss;
            //gd.RasterizerState = rs;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp); 

            spriteBatch.Draw(gridTexture, new Rectangle(Point.Zero, GraphicsDevice.PresentationParameters.Bounds.Size), Color.White);
            spriteBatch.Draw(gridTexture, new Rectangle(0, 0, 100, 100), Color.Blue);
            spriteBatch.DrawString(mgFont, msg, new Vector2(10, 100), textColor);
            spriteBatch.End();
            base.Draw(gameTime);
        }


        public void DisplayOptions()
        {
            optionsMsg =
            nline + "_____Available Options_____________" +
            nline +
            nline + " H   Options  (or the O key, the game window must have focus)  " +
            nline + " I   Current display information  " +
            nline + " T   Run auto Test script  " +
            nline + " F1  Sets backbuffer by GDM preferred to preset value " + started +
            nline + " F2  Sets backbuffer by GDM preferred to preset2 value" + preset +
            nline + " F3  Sets the backbuffer directly thru the graphics device to preset2 value " + preset +
            nline + " F4  List supported display modes" +
            nline + " F5  " +
            nline + " F6  Goto fullscreen with prefferd backbuffer change" +
            nline + " F7  Back out of fullscreen with backbuffer change" +
            nline + " F8  " +
            nline + " F9  Next display mode  current=" + curdisplaymodeindex.ToString() + " next=" + nextdisplaymodeindex.ToString() +
            nline + " F10 Fullscreen true " +
            nline + " F11 Fullscreen false " +
            nline + " F12 Toggle fullscreen" +
            nline + " Alt F4 Exit" +
            nline + " ! Note ! gd denotes graphics device and gdm denotes the manager " +
            nline + "___________________________________"
            ;
            msg = optionsMsg;
            Console.WriteLine(msg);
        }

        public void RunAutoScript()
        {
            auto_script_cmd_value = -1;
            if (run_test_script && auto_script_cmd_execution < 13 && pause < 0)
            {
                auto_script_cmd_execution++;
                switch (auto_script_cmd_execution)
                {
                    case 0:
                        Console.WriteLine("");
                        input_pause_amount = input_pause_amount * 15;
                        pause = 100;
                        Console.WriteLine(nline + " Auto Script Commands About to Start in 5 Seconds Please Wait pause amount changed to " + input_pause_amount + nline + nline + nline);
                        Console.WriteLine("for options press H");
                        break;
                    case 1: // f12 toggle fullscreen
                        auto_script_cmd_value = 12;
                        break;
                    case 2: // f10 goto fullscreen
                        auto_script_cmd_value = 10;
                        break;
                    case 3: // F1 shrink window to started value
                        auto_script_cmd_value = 1;
                        break;
                    case 4: // f10 goto fullscreen
                        auto_script_cmd_value = 10;
                        break;
                    case 5: // f9 next fullscreen mode change
                        auto_script_cmd_value = 9;
                        break;
                    case 6: // f9 next fullscreen mode change
                        auto_script_cmd_value = 9;
                        break;
                    case 7: // f9 next fullscreen mode change
                        auto_script_cmd_value = 9;
                        break;
                    case 8: // f11 drop out of fullscreen with gdm apply
                        auto_script_cmd_value = 11;
                        break;                 
                    case 9: // f2 set backbuffer thru preferred smaller
                        auto_script_cmd_value = 2;
                        break;
                    case 10: // f1 set backbuffer thru preferred large
                        auto_script_cmd_value = 1;
                        break;
                    case 11: // f3 set backbuffer directly thru presentation parameters smaller
                        auto_script_cmd_value = 3;
                        break;
                    case 12: // f2 set backbuffer thru preferred smaller
                        auto_script_cmd_value = 2;
                        break;
                    default: // set pause amount back to normal
                        auto_script_cmd_value = -1;
                        input_pause_amount = (int)(input_pause_amount * .10f);
                        Console.WriteLine("");
                        Console.WriteLine("**********************************");
                        Console.WriteLine("***Auto Script Commands Ended*****");
                        Console.WriteLine("**********************************");
                        nextdisplaymodeindex = -1;
                        DisplayOptions();
                        break;
                }
            }
        }

        public void OnResize(object sender, EventArgs e)
        {
            Console.WriteLine(nline + "  \"A Resize Has Just Occured\"  Window.ClientSizeChanged has fired...OnResize");
            onresizecounter++;
            if (was_apply_called == false)
            {
                Console.WriteLine("  Within OnResize.... my ApplyTheChanges() Was NOT called");
            }
            else
            {
                Console.WriteLine("  Within OnResize.... my ApplyTheChanges() Was called");
            }
            was_resize_called = true;
            DisplayGdmGdWindowRecordedInfo();
        }

        private void ApplyTheChanges()
        {
            Console.WriteLine(nline + "  User or Script calling My ApplyTheChanges() which will make the call ");
            was_apply_called = true;
            wasapplycalledcounter++;
            Console.WriteLine("  Before gdm.ApplyingChanges ");
            DisplayGdmGdWindowRecordedInfo();
            Console.WriteLine(nline + "  Now we will call...  gdm.ApplyChanges()...");
            gdm.ApplyChanges();
            gd = gdm.GraphicsDevice;
            if (was_resize_called == true)
            {
                Console.WriteLine("  After  gdm.ApplyingChanges was called ... OnResize Was called from gdm.ApplyChanges");
            }
            else
            {
                Console.WriteLine("  After  gdm.ApplyingChanges was called ... OnResize was NOT Yet called");
            }
            DisplayGdmGdWindowRecordedInfo();
            Console.WriteLine("   ...leaving my ApplyTheChanges method");
        }

        private void CheckForDoubleCalls()
        {
            if (wasapplycalledcounter > 1)
            {
                Console.WriteLine(nline + " !!! Note !!! ApplyTheChanges was fired more then once ... this for the test so i dont do it by accident");
            }
            if (onresizecounter > 1)
            {
                Console.WriteLine(nline + " !!! Note !!! OnResize was fired more then once... this would only be normal if applychanges was called more then once");
            }
            if (wasapplycalledcounter > 0 || onresizecounter > 0)
            {
                Console.WriteLine("     onresizecounter " + onresizecounter.ToString());
                Console.WriteLine("     wasapplycalledcounter " + wasapplycalledcounter.ToString());
                onresizecounter = 0;
                wasapplycalledcounter = 0;
            }
        }

        private void DisplayGdmGdWindowRecordedInfo()
        {
            gd = gdm.GraphicsDevice; // just to show gd is in fact current
            new RecordDiff(gdm, gd, Window);
            if (just_display_changes && RecordDiff.EnoughRecords)
            {
                RecordDiff.DisplayDifferences(display_unchanged_stuff);
            }
            else
            {
                RecordDiff.DisplayCurrentStatus(gdm, gd, Window);
            }
        }

        public void SetBackBufferDirectlyThruGdPresentationParams(int w, int h)
        {
            if (show_code)
            {
                Console.WriteLine(
                    tab4 + "Code" +
                    nline + tab4 + "gd.PresentationParameters.BackBufferWidth = w;" +
                    nline + tab4 + "gd.PresentationParameters.BackBufferHeight = h;"
                    );
            }
            gd.PresentationParameters.BackBufferWidth = w;
            gd.PresentationParameters.BackBufferHeight = h;
        }

        public void SetPreferredBackBufferThruGdm(int w, int h)
        {
            if (show_code)
            {
                Console.WriteLine(
                    tab4 + "Code" +
                    nline + tab4 + "preferred.X = w;" +
                    nline + tab4 + "preferred.Y = h;" +
                    nline + tab4 + "gdm.PreferredBackBufferWidth = w;" +
                    nline + tab4 + "gdm.PreferredBackBufferHeight = h;"
                    );
            }
            Console.WriteLine(nline + " we have made a called to set gdm.PreferredBackBuffer.WH from " + preferred + " to (" + w + "," + h + ")");
            preferred.X = w;
            preferred.Y = h;
            gdm.PreferredBackBufferWidth = preferred.X;
            gdm.PreferredBackBufferHeight = preferred.Y;
        }

        public void SetFullScreen(bool tf)
        {
            if (show_code)
            {
                Console.WriteLine(
                    tab4 + "Code" +
                    nline + tab4 + "if (tf != gdm.IsFullScreen){gdm.IsFullScreen = tf;}"
                    );
            }
            Console.WriteLine(nline + " checking gdm.isfullscreen(" + gdm.IsFullScreen + ") requesting change to " + tf);
            if (tf != gdm.IsFullScreen)
            {
                Console.WriteLine(" Ok changing gdm.isfullscreen setting To " + tf);
                gdm.IsFullScreen = tf;
            }
            else
            {
                Console.WriteLine(" !!note!! gdm isfullscreen is " + tf + " Already !!!!!!!!");
            }
        }
        public void TheFullScreenToggle()
        {
            if (show_code)
            {
                Console.WriteLine(
                    tab4 + "Code" +
                    nline + tab4 + "gdm.ToggleFullScreen();"
                    );
            }
            gdm.ToggleFullScreen();
            DisplayGdmGdWindowRecordedInfo();
        }

        public void ListSupportedDisplayModes()
        {
            Console.WriteLine(nline + " list supported Display modes");
            Console.WriteLine("  Current Mode " + gd.Adapter.CurrentDisplayMode + nline);
            int counter = 0;
            counter = 0;
            //foreach (DisplayMode dm in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes) // either works
            foreach (DisplayMode dm in gd.Adapter.SupportedDisplayModes)
            {
                Console.WriteLine(
                    "   DisplayMode[" + counter.ToString() + "] " + dm.Width.ToString() + "," + dm.Height.ToString() +
                    "   AspectRatio " + dm.AspectRatio.ToString() +
                    "   SurfaceFormat " + dm.Format.ToString()
                    //+" RefreshRate " + dm.RefreshRate.ToString()  //in monogame but not in xna 4.0 that's required for arm i think
                    );
                counter++;
            }
        }

        int curdisplaymodeindex = 0;
        int nextdisplaymodeindex = 0;

        public void ChangeToNextDisplayMode(bool enterfullscreenfirst)
        {
            if (show_code)
            {
                Console.WriteLine(
                    tab4 + "Code   :relevant:" +
                    nline + tab4 + "gdm.PreferredBackBufferWidth = dm.Width;" +
                    nline + tab4 + "gdm.PreferredBackBufferHeight = dm.Height;" +
                    nline + tab4 + "gdm.PreferredBackBufferFormat = dm.Format;" +
                    nline + tab4 + "//gd.PresentationParameters.BackBufferWidth = dm.Width;" +
                    nline + tab4 + "//gd.PresentationParameters.BackBufferHeight = dm.Height;" +
                    nline + tab4 + "//gd.PresentationParameters.BackBufferFormat = dm.Format;" +
                    nline + tab4 + "gdm.IsFullScreen = true; // call this specifically here as this might fail windowed"
                    );
            }
            Console.WriteLine(nline + " Next Display mode coaxing setting fullscreen");

            DisplayMode currentdm = gd.Adapter.CurrentDisplayMode;
            curdisplaymodeindex = FindCurrentDisplayModeInList();

            Console.WriteLine(" Ensureing we are in fullscreen [code] gdm.IsFullScreen = true;");
            if (gdm.IsFullScreen == false && enterfullscreenfirst)
            {
                gdm.IsFullScreen = true;
                gdm.ApplyChanges();
            }

            int startmode = curdisplaymodeindex;
            int attempts = 1;
            if (curdisplaymodeindex > -1)
            {
                for (int i = 0; i < attempts; i++)
                {
                    nextdisplaymodeindex += 1;
                    if (nextdisplaymodeindex == curdisplaymodeindex)
                    {
                        nextdisplaymodeindex += 1;
                    }
                    if (nextdisplaymodeindex >= supportedDisplayModes.Count)
                    {
                        nextdisplaymodeindex = 0;
                    }

                    //DisplayGdmGdWindowRecordedInfo();
                    Console.WriteLine();
                    Console.WriteLine(" gd.displaymode [" + curdisplaymodeindex + "] ... wh " + gd.DisplayMode.Width + "," + gd.DisplayMode.Height + " aspct " + gd.DisplayMode.AspectRatio + " format " + gd.DisplayMode.Format);
                    Console.WriteLine();

                    DisplayMode dm = supportedDisplayModes[nextdisplaymodeindex];
                    Console.WriteLine(" Next displaymode [" + nextdisplaymodeindex + "] ... wh " + dm.Width + "," + dm.Height + " aspct " + dm.AspectRatio + " format " + dm.Format);
                    Console.WriteLine();

                    gdm.PreferredBackBufferWidth = dm.Width;
                    gdm.PreferredBackBufferHeight = dm.Height;
                    gdm.PreferredBackBufferFormat = dm.Format;
                    //gdm.GraphicsDevice.PresentationParameters.BackBufferWidth = dm.Width;
                    //gdm.GraphicsDevice.PresentationParameters.BackBufferHeight = dm.Height;
                    //gdm.GraphicsDevice.PresentationParameters.BackBufferFormat = dm.Format;
                    //gdm.HardwareModeSwitch = true;
                    gdm.ApplyChanges();

                    curdisplaymodeindex = FindCurrentDisplayModeInList();
                    if (startmode != curdisplaymodeindex)
                    {
                        i = attempts;
                        Console.WriteLine(" Display mode has changed  cur=" + curdisplaymodeindex);
                    }
                    else
                    {
                        Console.WriteLine(" Failed to change Display mode cur=" + curdisplaymodeindex);
                    }
                }
            }
            else
            {
                if (gdm.IsFullScreen)
                {
                    gdm.IsFullScreen = false;
                    gdm.ApplyChanges();
                }
                Console.WriteLine(" !!!! NO match found for the current display mode ? falling back to windowed");
            }

            DisplayGdmGdWindowRecordedInfo();

            Console.WriteLine(" ChangeToNextDisplayMode... ends");
        }

        public int FindCurrentDisplayModeInList()
        {
            DisplayMode tofind = gd.Adapter.CurrentDisplayMode;

            int index = -1;
            int matchlevel = 0;
            int j = 0;
            foreach (DisplayMode bd in gd.Adapter.SupportedDisplayModes)
            {
                if (tofind.Format == bd.Format)
                {
                    if (matchlevel < 1) { matchlevel = 1; index = j; }
                    if (tofind.Width == bd.Width)
                    {
                        if (matchlevel < 2) { matchlevel = 2; index = j; }
                        if (tofind.Height == bd.Height)
                        {
                            if (matchlevel < 3) { matchlevel = 3; index = j; }
                            if (tofind.AspectRatio == bd.AspectRatio)
                            {
                                if (matchlevel < 4) { matchlevel = 4; index = j; }
                                if (tofind.Format == bd.Format)
                                {
                                    if (matchlevel < 5) { matchlevel = 5; index = j; }
                                }
                            }
                        }
                    }
                }
                j++;
            }
            return index;
        }
        public int FindCurrentOrClosestDisplayModeInList(List<DisplayMode> bd, DisplayMode tofind)
        {
            int index = -1;
            int matchlevel = 0;
            for (int j = 0; j < bd.Count; j++)
            {
                if (tofind.Format == bd[j].Format)
                {
                    if (matchlevel < 1) { matchlevel = 1; index = j; }
                    if (tofind.Width == bd[j].Width)
                    {
                        if (matchlevel < 2) { matchlevel = 2; index = j; }
                        if (tofind.Height == bd[j].Height)
                        {
                            if (matchlevel < 3) { matchlevel = 3; index = j; j = bd.Count; }
                        }
                    }
                }
            }
            return index;
        }

        public List<DisplayMode> FindBestDisplayModes()
        {
            Console.WriteLine(nline + " Finding Best DisplayModes ");
            // rebuild viable list into sdm
            DisplayModeCollection dms = gdm.GraphicsDevice.Adapter.SupportedDisplayModes;
            bestDisplayModes = new List<DisplayMode>();
            supportedDisplayModes = new List<DisplayMode>();

            for (int i = 0; i < dms.Count(); i++)
            {
                DisplayMode now = dms.ElementAt(i);
                supportedDisplayModes.Add(now);
                if (now.Format == starteddm.Format && now.AspectRatio == starteddm.AspectRatio)
                {
                    bool isgood = true;
                    for (int j = 0; j < bestDisplayModes.Count; j++)
                    {
                        if (now.Width == bestDisplayModes[j].Width && now.Height == bestDisplayModes[j].Height)
                        {
                            isgood = false;
                        }
                    }
                    if (isgood)
                    {
                        bestDisplayModes.Add(now);
                        Console.WriteLine("  (" + i + ")-->(" + (bestDisplayModes.Count() - 1) + ")" + now + " aspct " + now.AspectRatio + " format " + now.Format);
                    }
                }
            }
            return bestDisplayModes;
        }

        int pause = 0;
        public void ReducePause()
        {
            if (pause > -1)
                pause--;
        }
        public bool Paused
        {
            get
            {
                if (pause < 1)
                {
                    pause = input_pause_amount;
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        //void GdmPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        //{
        //    //e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PlatformContents;
        //    e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 4;
        //    e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = 800;
        //    e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = 600;
        //    e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Immediate;
        //    e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = true;
        //}

        public class RecordDiff
        {
            private static List<RecordDiff> registered_list = new List<RecordDiff>();
            bool isfullscreen = false;
            bool isgdfullscreen = false;
            bool iswidescreen = false;
            Point preferedbackbuffwh = new Point();
            SurfaceFormat preferedbackbufferformat;
            Rectangle windowclientbounds = new Rectangle();
            DisplayOrientation displayorientation = DisplayOrientation.Default;
            Rectangle gdpresentationparambounds = new Rectangle();
            Point presentationparametersbackbufferwh = new Point();
            SurfaceFormat presentationparametersbackbufferformat;
            Viewport viewport = new Viewport();
            Rectangle viewporttilesafearea = new Rectangle();
            Point displaymodeWh = new Point();
            Rectangle displaymodetilesafearea = new Rectangle();
            bool isdefaultgraphicsadapter = true;
            Point gdcurrentdisplaymodewh = new Point();
            Rectangle gdcurrentdisplaymodetilesafearea = new Rectangle();

            public static bool EnoughRecords { get { return (registered_list.Count > 1); } }

            public RecordDiff(GraphicsDeviceManager gdm, GraphicsDevice gd, GameWindow Window)
            {
                isfullscreen = gdm.IsFullScreen;
                isgdfullscreen = gd.PresentationParameters.IsFullScreen;
                iswidescreen = false;
                preferedbackbuffwh = new Point(gdm.PreferredBackBufferWidth, gdm.PreferredBackBufferHeight);
                preferedbackbufferformat = gdm.PreferredBackBufferFormat;
                windowclientbounds = Window.ClientBounds;
                displayorientation = gd.PresentationParameters.DisplayOrientation;
                gdpresentationparambounds = gd.PresentationParameters.Bounds;
                presentationparametersbackbufferwh = new Point(gd.PresentationParameters.BackBufferWidth, gd.PresentationParameters.BackBufferHeight);
                presentationparametersbackbufferformat = gd.PresentationParameters.BackBufferFormat;
                viewport = gd.Viewport;
                viewporttilesafearea = gd.Viewport.TitleSafeArea;
                displaymodeWh = new Point(gd.DisplayMode.Width, gd.DisplayMode.Height);
                displaymodetilesafearea = gd.DisplayMode.TitleSafeArea;
                isdefaultgraphicsadapter = true;//gd.Adapter.IsDefaultAdapter; // not implemented in monogame
                gdcurrentdisplaymodewh = new Point(gd.Adapter.CurrentDisplayMode.Width, gd.Adapter.CurrentDisplayMode.Height);
                gdcurrentdisplaymodetilesafearea = gd.Adapter.CurrentDisplayMode.TitleSafeArea;
                // add it
                registered_list.Add(this);
            }
            public static void DisplayDifferences(bool displayunchanged)
            {
                if (EnoughRecords)
                {
                    int to = registered_list.Count - 1;
                    int from = to - 1;
                    TheDifferences(registered_list[from], registered_list[to], displayunchanged);
                }
            }
            private static void TheDifferences(RecordDiff A, RecordDiff B, bool displayunchanged)
            {
                string differences = "";
                string stayedthesame = "";
                string formating = "\n   diff {0} CHANGE FROM {1} TO {2}";
                string formating2 = "\n    nochange {0}";
                string changedvar = "";
                differences += string.Format("\n   ------ changes that occured -----------");
                differences += string.Format("\n   ---------------------------------------");
                stayedthesame += string.Format("\n   ------ remained unchanged -----------");
                stayedthesame += string.Format("\n   ---------------------------------------");
                if (A.isfullscreen != B.isfullscreen) { changedvar = "gdm.FullScreen"; differences += string.Format(formating, changedvar, A.isfullscreen, B.isfullscreen); }
                else { stayedthesame += string.Format(formating2, "gdm.FullScreen"); }
                if (A.isgdfullscreen != B.isgdfullscreen) { changedvar = "gd.PresentationParameters.IsFullScreen"; differences += string.Format(formating, changedvar, A.isgdfullscreen, B.isgdfullscreen); }
                else { stayedthesame += string.Format(formating2, "gd.PresentationParameters.IsFullScreen"); }
                if (A.iswidescreen != B.iswidescreen) { changedvar = "gd.Adapter.isWideScreen"; differences += string.Format(formating, changedvar, A.iswidescreen, B.iswidescreen); }
                else { stayedthesame += string.Format(formating2, "gd.Adapter.isWideScreen"); }
                if (A.preferedbackbuffwh != B.preferedbackbuffwh) { changedvar = "gdm.PreferredBackBuffer(Width Height)"; differences += string.Format(formating, changedvar, A.preferedbackbuffwh, B.preferedbackbuffwh); }
                else { stayedthesame += string.Format(formating2, "gdm.PreferredBackBuffer(Width Height)"); }
                if (A.preferedbackbufferformat != B.preferedbackbufferformat) { changedvar = "gdm.PreferredBackBufferFormat"; differences += string.Format(formating, changedvar, A.preferedbackbufferformat, B.preferedbackbufferformat); }
                else { stayedthesame += string.Format(formating2, "gdm.PreferredBackBufferFormat"); }
                if (A.windowclientbounds != B.windowclientbounds) { changedvar = "Window.ClientBounds"; differences += string.Format(formating, changedvar, A.windowclientbounds, B.windowclientbounds); }
                else { stayedthesame += string.Format(formating2, "Window.ClientBounds"); }
                if (A.displayorientation != B.displayorientation) { changedvar = "gd.PresentationParameters.DisplayOrientation"; differences += string.Format(formating, changedvar, A.displayorientation, B.displayorientation); }
                else { stayedthesame += string.Format(formating2, "gd.PresentationParameters.DisplayOrientation"); }
                if (A.gdpresentationparambounds != B.gdpresentationparambounds) { changedvar = "gd.PresentationParameters.Bounds"; differences += string.Format(formating, changedvar, A.gdpresentationparambounds, B.gdpresentationparambounds); }
                else { stayedthesame += string.Format(formating2, "gd.PresentationParameters.Bounds"); }
                if (A.presentationparametersbackbufferwh != B.presentationparametersbackbufferwh) { changedvar = "gd.PresentationParameters.BackBuffer(WidthHeight)"; differences += string.Format(formating, changedvar, A.presentationparametersbackbufferwh, B.presentationparametersbackbufferwh); }
                else { stayedthesame += string.Format(formating2, "gd.PresentationParameters.BackBuffer(WidthHeight)"); }
                if (A.presentationparametersbackbufferformat != B.presentationparametersbackbufferformat) { changedvar = "gd.PresentationParameters.BackBufferFormat"; differences += string.Format(formating, changedvar, A.presentationparametersbackbufferformat, B.presentationparametersbackbufferformat); }
                else { stayedthesame += string.Format(formating2, "gd.PresentationParameters.BackBufferFormat"); }
                if (A.viewport.Bounds != B.viewport.Bounds) { changedvar = "gd.Viewport"; differences += string.Format(formating, changedvar, A.viewport, B.viewport); }
                else { stayedthesame += string.Format(formating2, "gd.Viewport"); }
                if (A.viewporttilesafearea != B.viewporttilesafearea) { changedvar = "gd.Viewport.TitleSafeArea"; differences += string.Format(formating, changedvar, A.viewporttilesafearea, B.viewporttilesafearea); }
                else { stayedthesame += string.Format(formating2, "gd.Viewport.TitleSafeArea"); }
                if (A.displaymodeWh != B.displaymodeWh) { changedvar = "gd.DisplayMode.(Width, Height)"; differences += string.Format(formating, changedvar, A.displaymodeWh, B.displaymodeWh); }
                else { stayedthesame += string.Format(formating2, "gd.DisplayMode.(Width, Height)"); }
                if (A.displaymodetilesafearea != B.displaymodetilesafearea) { changedvar = "gd.DisplayMode.TitleSafeArea"; differences += string.Format(formating, changedvar, A.displaymodetilesafearea, B.displaymodetilesafearea); }
                else { stayedthesame += string.Format(formating2, "gd.DisplayMode.TitleSafeArea"); }
                if (A.isdefaultgraphicsadapter != B.isdefaultgraphicsadapter) { changedvar = "gd.Adapter.IsDefaultAdapter"; differences += string.Format(formating, changedvar, A.isdefaultgraphicsadapter, B.isdefaultgraphicsadapter); }
                else { stayedthesame += string.Format(formating2, "gd.Adapter.IsDefaultAdapter"); }
                if (A.gdcurrentdisplaymodewh != B.gdcurrentdisplaymodewh) { changedvar = "gd.Adapter.CurrentDisplayMode.WH"; differences += string.Format(formating, changedvar, A.gdcurrentdisplaymodewh, B.gdcurrentdisplaymodewh); }
                else { stayedthesame += string.Format(formating2, "gd.Adapter.CurrentDisplayMode.WH"); }
                if (A.gdcurrentdisplaymodetilesafearea != B.gdcurrentdisplaymodetilesafearea) { changedvar = "gd.Adapter.CurrentDisplayMode.TitleSafeArea"; differences += string.Format(formating, changedvar, A.gdcurrentdisplaymodetilesafearea, B.gdcurrentdisplaymodetilesafearea); }
                else { stayedthesame += string.Format(formating2, "gd.Adapter.CurrentDisplayMode.TitleSafeArea"); }
                differences += string.Format("\n   ---------------------------------------");
                stayedthesame += string.Format("\n   ---------------------------------------");
                //
                Console.WriteLine(differences);
                if (displayunchanged)
                    Console.WriteLine(stayedthesame);
            }
            public static string DisplayCurrentStatus(GraphicsDeviceManager gdm, GraphicsDevice gd, GameWindow Window)
            {
                string s = "";
                string nl = Environment.NewLine;
                s += nl + ("");
                s += nl + ("  DisplayCurrentStatus ");
                s += nl + ("  ___________Info______");
                if (gd.PresentationParameters.IsFullScreen)
                    s += nl + ("  gd.PresentationParameters.IsFullScreen       Currently FullScreen");
                else
                    s += nl + ("  gd.PresentationParameters.IsFullScreen       Currently Windowed");
                // common
                s += nl + ("  gdm.HardwareModeSwitch                       " + gdm.HardwareModeSwitch);
                s += nl + ("  gd.Adapter.IsWideScreen                      " + gd.Adapter.IsWideScreen);
                s += nl + ("  gdm.PreferredBackBufferFormat                " + gdm.PreferredBackBufferFormat);
                s += nl + ("  gdm.PreferredBackBuffer WH:                           Width:" + gdm.PreferredBackBufferWidth + " Height:" + gdm.PreferredBackBufferHeight);
                s += nl + ("  Window.ClientBounds                          " + Window.ClientBounds);
                s += nl + ("  gd.PresentationParameters.DisplayOrientation " + gd.PresentationParameters.DisplayOrientation);
                s += nl + ("  gd.PresentationParameters.Bounds             " + gd.PresentationParameters.Bounds);
                s += nl + ("  gd.PresentationParameters.BackBuffer WH:              Width:" + gd.PresentationParameters.BackBufferWidth + " Height:" + gd.PresentationParameters.BackBufferHeight);
                s += nl + ("  gd.PresentationParameters.BackBufferFormat   " + gd.PresentationParameters.BackBufferFormat);
                s += nl + ("  gd.Viewport                                  " + gd.Viewport);
                s += nl + ("  gd.Viewport.TitleSafeArea                    " + gd.Viewport.TitleSafeArea);
                s += nl + ("  gd.DisplayMode.Width, Height                          Width:" + gd.DisplayMode.Width + " Height:" + gd.DisplayMode.Height);
                s += nl + ("  gd.DisplayMode.TitleSafeArea                 " + gd.DisplayMode.TitleSafeArea);
                //Console.WriteLine("gd.Adapter.IsDefaultAdapter                  " + gd.Adapter.IsDefaultAdapter); //// not yet implemented in monogame
                s += nl + ("  gd.Adapter.IsDefaultAdapter                  not yet implemented in monogame");
                s += nl + ("  gd.Adapter.CurrentDisplayMode WH:                     Width:" + gd.Adapter.CurrentDisplayMode.Width + " Height:" + gd.Adapter.CurrentDisplayMode.Height);
                s += nl + ("  gd.Adapter.CurrentDisplayMode.TitleSafeArea  " + gd.Adapter.CurrentDisplayMode.TitleSafeArea);
                s += nl + ("  ________________________________");
                Console.WriteLine(s);
                return s;
            }
        }

        public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
        {
            Texture2D tex = new Texture2D(device, width, height);
            tex.SetData<Color>(data);
            return tex;
        }

        public static Texture2D CreateCheckerBoard(GraphicsDevice device, int w, int h, Color c0, Color c1)
        {
            Color[] data = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color c = c0;
                    if ((y % 2 == 0))
                    {
                        if ((x % 2 == 0))
                            c = c0;
                        else
                            c = c1;
                    }
                    else
                    {
                        if ((x % 2 == 0))
                            c = c1;
                        else
                            c = c0;
                    }
                    data[index] = c;
                }
            }
            return TextureFromColorArray(device, data, w, h);
        }

        public static Texture2D CreateGrid(GraphicsDevice device, int w, int h, Color c0, Color c1, Color c2)
        {
            Color[] data = new Color[w * h];
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int index = y * w + x;
                    Color c = c2;
                    if ((y % 4 == 0))
                        c = c0;
                    if ((x % 4 == 0))
                        c = c1;
                    if ((x % 4 == 0) && (y % 4 == 0))
                    {
                        var r = (c0.R + c1.R) / 2;
                        var g = (c0.R + c1.R) / 2;
                        var b = (c0.R + c1.R) / 2;
                        c = new Color(r, g, b, 255);
                    }
                    data[index] = c;
                }
            }
            return TextureFromColorArray(device, data, w, h);
        }

    }
}
