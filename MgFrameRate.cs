using System;
using System.IO;
//using System.Text;
//using System.Linq;
//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
//using System.Reflection;
////using System.Xml.Serialization;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// BaseGameClass  tack this onto game1 instead of game for some basic extra stuff primarily for quick testing.
    /// </summary>
    public class BasicGame : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Basic3dExampleCamera cam;
        public Effect triangleEffect;

        /// <summary>
        /// This will most likely generate garbage unless used properly DefaultAllowCallbackOnKeyboardEvents = false is the default.
        /// </summary>
        //public KeyBoardTextInput KeyBoardTextInput = new KeyBoardTextInput();

        Grid3dOrientation grid3d;
        NavOrientation3d navPoint;
        OrientationArrows orientArrow;

        public Texture2D texture_arrows;

        public SamplerState ss_standard = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp };
        public DepthStencilState ds_standard_always = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Always };
        public DepthStencilState ds_standard_less = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Less };
        public DepthStencilState ds_standard_more = new DepthStencilState() { DepthBufferEnable = true, DepthBufferFunction = CompareFunction.Greater };
        public RasterizerState rs_nocullwireframe = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.WireFrame };
        public RasterizerState rs_nocullsolid = new RasterizerState() { CullMode = CullMode.None, FillMode = FillMode.Solid };
        public RasterizerState rs_cull_ccw_solid = new RasterizerState() { CullMode = CullMode.CullCounterClockwiseFace, FillMode = FillMode.Solid };
        public RasterizerState rs_cull_cw_solid = new RasterizerState() { CullMode = CullMode.CullClockwiseFace, FillMode = FillMode.Solid };

        public BasicGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 700;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            if (Gu.DefaultFontName == "" || Gu.DefaultFontName == null)
            {
                Debug.Assert(false, "Requisite add a spritefont to the content pipeline and set the name of it to the string DefaultFontName property.");
                Gu.DefaultFontName = "MgGenFont";
            }
            spriteBatch = Gu.SetUpLoad(this, graphics, Content, Gu.DefaultFontName, true, true, false, false, 120f);


            if (Gu.DefaultCameraSwitch)
                LoadUpDefaultCamera();
            if (Gu.DefaultOreintationArrow3dSwitch)
                LoadOrientationArrow3D();
            if (Gu.DefaultGrid3dSwitch)
                LoadGrid3D();

            //if (Gu.DefaultAllowCallbackOnKeyboardEvents)
            //{
            //    KeyBoardTextInput.Initialize(this.Window);
            //    //KeyBoardTextInput.AddMethodsToRaiseOnKeyBoardActivity(TextInputed, KeyPressed);
            //}

          //  just turn that off for now im going to make a lot of garbge as this is all just a test.
          //Gu.fps.DisplayGarbageCollectionRate = false;
        }

        public void LoadUpDefaultCamera()
        {
            cam = new Basic3dExampleCamera(GraphicsDevice, this.Window, Gu.DefaultUseOthographic);
            cam.CameraUi(Basic3dExampleCamera.CAM_TYPE_OPTION_FREE);
            cam.CameraType(Basic3dExampleCamera.CAM_UI_OPTION_EDIT_LAYOUT);
            cam.Position = new Vector3(2, 33, 52);
            cam.LookAtDirection = Vector3.Forward;
            cam.RightClickCentersCamera = Gu.DefaultCameraMouseRightClickCentering;
            Gu.DefaultCameraSwitch = true;
            LoadTriangleEffect();
        }
        public void LoadGrid3D()
        {
            grid3d = new Grid3dOrientation(10, 10, 600f, 600f, 1f);
            navPoint = new NavOrientation3d(30, 24, 24, .03f, .5f);
            Gu.DefaultGrid3dSwitch = true;
            LoadTriangleEffect();
        }
        public void LoadOrientationArrow3D()
        {
            texture_arrows = Content.Load<Texture2D>("OrientationImage_PartiallyTransparent");
            orientArrow = new OrientationArrows();
            Gu.DefaultOreintationArrow3dSwitch = true;
            //LoadTriangleEffect();
            triangleEffect.Parameters["TextureA"].SetValue(texture_arrows);
        }
        public void LoadTriangleEffect()
        {
            triangleEffect = Content.Load<Effect>("TriangleDraw"); //TriangleDrawEffect
            triangleEffect.Parameters["TextureA"].SetValue(BasicTextures.dotTexture);
            triangleEffect.Parameters["World"].SetValue(Matrix.Identity);
            triangleEffect.Parameters["View"].SetValue(cam.View);
            triangleEffect.Parameters["Projection"].SetValue(cam.Projection);
        }

        protected override void UnloadContent()
        {
            Gu.UnLoadContent();
        }

        public void SetDefaultStates()
        {
            GraphicsDevice.SamplerStates[0] = ss_standard;
            GraphicsDevice.DepthStencilState = ds_standard_less;
            GraphicsDevice.RasterizerState = rs_nocullsolid;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Gu.DefaultCameraSwitch)
                cam.Update(gameTime);

            //if (Gu.DefaultAllowCallbackOnKeyboardEvents)
            //    KeyBoardTextInput.Update(gameTime);

            Gu.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            if(Gu.DefaultStatesSwitch)
                SetDefaultStates();

            if (Gu.DefaultCameraSwitch)
            {
                triangleEffect.CurrentTechnique = triangleEffect.Techniques["TriangleDraw"];
                triangleEffect.Parameters["View"].SetValue(cam.View);
                triangleEffect.Parameters["World"].SetValue(Matrix.Identity);
            }

            if (Gu.DefaultGrid3dSwitch)
            {
                triangleEffect.CurrentTechnique = triangleEffect.Techniques["TriangleDraw"];
                triangleEffect.Parameters["World"].SetValue(Matrix.Identity);
                grid3d.Draw(GraphicsDevice, triangleEffect, Matrix.Identity, 1f, BasicTextures.green, BasicTextures.red, BasicTextures.blue);
                navPoint.DrawNavOrientation3DToTarget(GraphicsDevice, triangleEffect, new Vector3(0, 0, 0), 5f, Matrix.Identity, BasicTextures.yellow, BasicTextures.red, BasicTextures.aqua);
            }

            if (Gu.DefaultOreintationArrow3dSwitch)
            {
                triangleEffect.CurrentTechnique = triangleEffect.Techniques["TriangleDraw"];
                triangleEffect.Parameters["TextureA"].SetValue(texture_arrows);
                //triangleEffect.Parameters["World"].SetValue(Matrix.CreateWorld(cam.Position + cam.World.Down * 1.1f + cam.World.Forward * 2f + cam.World.Right * 1.1f, Vector3.Forward, Vector3.Up));
                orientArrow.Draw(GraphicsDevice, triangleEffect);
            }


            //spriteBatch.Begin(SpriteSortMode.Deferred, null, ss_standard, null, RasterizerState.CullNone, null, null);

            spriteBatch.Begin(SpriteSortMode.Immediate, null, ss_standard,null,RasterizerState.CullNone,null,null);
            Gu.Draw(gameTime);
            QuickDrawSpriteBatch(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void QuickDrawSpriteBatch(GameTime gameTime)
        {

        }

        //public virtual void KeyPressed(Keys k, Keys k2)
        //{

        //}

        //public virtual void TextInputed(StringBuilder s, Keys k)
        //{

        //}
    }

    /// <summary>
    /// Game Utilitys  Gu for short less typing.
    /// </summary>
    public static class Gu
    {
        #region Variables and objects

        // probably shouldn't use game and graphics from here but just gonna leave it here for now.
        // i can make it safe later if i need to use it.
        public static Game game;
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;
        public static Texture2D dotTexture;
        public static SpriteBatch spriteBatch;
        public static SpriteFont currentFont;
        public static Microsoft.Xna.Framework.Content.ContentManager content;

        public static MgFrameRate frameRate;

        public static float ScreenWidth { get; set; }
        public static float ScreenHeight { get; set; }
        public static Rectangle ScreenBounds { get { return new Rectangle(0, 0, (int)ScreenWidth, (int)ScreenHeight); } }
        public static Vector2 ScrMult { get; private set; }

        public delegate void ResizeDelegate(); // (int w, int h) // todo change this to a action delegate.
        private static ResizeDelegate resizeMethodsToFire;
        private static int resizeCallbacksBeenAddedCount = 0;

        public static Color ClearColor { get; set; } = Color.Moccasin;

        public static bool DefaultUseOthographic = true;
        public static string DefaultFontName { get; set; }
        public static bool DefaultStatesSwitch { get; set; } = false;
        public static bool DefaultCameraSwitch { get; set; } = false;
        public static bool DefaultCameraMouseRightClickCentering { get; set; } = false;
        public static bool DefaultGrid3dSwitch { get; set; } = false;
        public static bool DefaultOreintationArrow3dSwitch { get; set; } = false;
        public static bool DefaultAllowCallbackOnKeyboardEvents { get; set; } = false;
        public static bool DefaultDisplayFrameRate { get; set; } = true;
        public static bool DefaultDisplayGarbageCollections { get; set; } = true;
        public static bool DefaultUi { get; set; } = false;

        #endregion

        /// <summary>
        /// This should be called in load it allows you to set up a basic stuff.
        /// </summary>
        public static SpriteBatch SetUpLoad(Game pass_in_this, GraphicsDeviceManager gdm, Microsoft.Xna.Framework.Content.ContentManager Content, string spriteFontName)
        {
            currentFont = Content.Load<SpriteFont>(spriteFontName);
            return SetUpLoad(pass_in_this, gdm, Content, currentFont, true, true, true, false, 60f);
        }
        /// <summary>
        /// This should be called in load it allows you to set up a basic stuff.
        /// </summary>
        public static SpriteBatch SetUpLoad(Game pass_in_this, GraphicsDeviceManager gdm, Microsoft.Xna.Framework.Content.ContentManager Content, SpriteFont spriteFont)
        {
            return SetUpLoad(pass_in_this, gdm, Content, spriteFont, true, true, true, false, 60f);
        }
        /// <summary>
        /// This should be called in load it allows you to set up a lot of initial stuff.
        /// </summary>
        public static SpriteBatch SetUpLoad(Game pass_in_this, GraphicsDeviceManager gdm, Microsoft.Xna.Framework.Content.ContentManager Content, string spriteFontName, bool allowresizing, bool showmouse, bool fixedon, bool vsync, double desiredFramesPerSecond)
        {

            currentFont = Content.Load<SpriteFont>(spriteFontName);
            return SetUpLoad( pass_in_this, gdm, Content, currentFont,  allowresizing,  showmouse,  fixedon,  vsync,  desiredFramesPerSecond);
        }
        /// <summary>
        /// This should be called in load it allows you to set up a lot of initial stuff.
        /// </summary>
        public static SpriteBatch SetUpLoad(Game pass_in_this, GraphicsDeviceManager gdm, Microsoft.Xna.Framework.Content.ContentManager Content, SpriteFont spriteFont, bool allowresizing, bool showmouse, bool fixedon, bool vsync, double desiredFramesPerSecond)
        {
            game = pass_in_this;
            game.Window.ClientSizeChanged += OnWindowsResize;
            game.IsMouseVisible = showmouse;
            graphics = gdm;
            device = graphics.GraphicsDevice;
            //device.BlendState = BlendState.AlphaBlend; //BlendState.NonPremultiplied;
            device.BlendState = BlendState.NonPremultiplied;
            BasicTextures.Load(device);
            currentFont = spriteFont;
            Gu.spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            Gu.dotTexture = TextureDotCreate(pass_in_this.GraphicsDevice);
            content = Content;
            //if (Gu.DefaultAllowCallbackOnKeyboardEvents)
            //{
            //    CharacterVertexDrawBuffer.SetUpBasicEffect(Gu.currentFont);
            //    WrapEditTextBox11.SetSpriteFont(currentFont);
            //}
            ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            ScrMult = new Vector2(1f / ScreenWidth, 1f / ScreenHeight);
            UserMouseInput.PassTheScreenSize(ScreenWidth.ToInt(), ScreenHeight.ToInt());
            BasicTextures.Load(game.GraphicsDevice);
            // fps garbage display.
            frameRate = new MgFrameRate();
            frameRate.LoadSetUp(pass_in_this, graphics, spriteBatch, allowresizing, showmouse, fixedon, vsync, desiredFramesPerSecond, DefaultDisplayGarbageCollections);
            //if (Gu.DefaultAllowCallbackOnKeyboardEvents)
            //{
            //    // for the keyboardtextinput class
            //    KeyBoardTextInput.Initialize(game.Window);
            //}
            //ui might go here eventually
            if (DefaultUi)
                MyUi.LoadTheUi(Content);
            // this is depreciated for the most part.
            //MgTextBounder.SetSpriteFont(spriteFont);
            //Console.WriteLine( PathInfo());
            return spriteBatch;
        }

        public static void UnLoadContent()
        {
            BasicTextures.Dispose();
        }

        public static SpriteFont LoadFont(string fontname)
        {
            currentFont = content.Load<SpriteFont>(fontname);
            return currentFont;
        }

        public static void SetCurrentFont(SpriteFont spriteFont)
        {
            currentFont = spriteFont;
            //MgTextBounder.SetSpriteFont(spriteFont);
        }

        public static string PathInfo()
        {
            MgStringBuilder msg = "";
            msg += "\n\n Environment.CurrentDirectory \n" + Environment.CurrentDirectory;
            return msg.ToString();
        }

        /// <summary>
        /// This method is registered to the game class in LoadSetUp
        /// </summary>
        public static void OnWindowsResize(Object sender, EventArgs e)
        {
            ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            UserMouseInput.PassTheScreenSize(ScreenWidth.ToInt(), ScreenHeight.ToInt());
            ScrMult = new Vector2(1f / ScreenWidth, 1f / ScreenHeight);
            if (resizeCallbacksBeenAddedCount > 0)
            {
                resizeMethodsToFire();
            }
        }

        ///// <summary>
        ///// Used to add methods for text input or control key presses such as shortcuts or wasd
        ///// </summary>
        //public static void AddMethodsToRaiseOnKeyBoardActivity(Action<StringBuilder, Keys> textInputMethodToCall, Action<Keys, Keys> controlKeysMethodToCall)
        //{
        //    //KeyBoardTextInput.AddMethodsToRaiseOnKeyBoardActivity(textInputMethodToCall, controlKeysMethodToCall);
        //}

        /// <summary>
        /// This method allows you to designate a method in another class to be registered for notification of a window resize.
        /// This method can be called from anywere and any classes void method can be passed to register to it. 
        /// The methods ending parenthisis are excluded when passing a method to he function. 
        /// Example... 
        /// public void SomeMethod(){} is passed as...
        /// Gu.AddMethodToFireOnWindowReSizeEvent( SomeMethod );
        /// </summary>
        public static void AddMethodToFireOnWindowReSizeEvent(ResizeDelegate methodName)
        {
            resizeCallbacksBeenAddedCount++;
            resizeMethodsToFire += methodName;
        }

        public static void SetApplicationPreferredBackBuffer(int w, int h)
        {
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();
            ScreenWidth = graphics.GraphicsDevice.PresentationParameters.BackBufferWidth;
            ScreenHeight = graphics.GraphicsDevice.PresentationParameters.BackBufferHeight;
            UserMouseInput.PassTheScreenSize(ScreenWidth.ToInt(), ScreenHeight.ToInt());
            ScrMult = new Vector2(1f / ScreenWidth, 1f / ScreenHeight);
        }

        public static void Update(GameTime gameTime)
        {
            // Update mouse
            UserMouseInput.Update();

            //if (Gu.DefaultAllowCallbackOnKeyboardEvents) // ok so this will create garbage.
            //{
            //    // Update keyboard input methods
            //    KeyBoardTextInput.Update(gameTime);
            //}

            // Update Ui
            if (DefaultUi)
                MyUi.Update(gameTime);

            // Update fps
            if(Gu.DefaultDisplayFrameRate)
                 frameRate.Update(gameTime);
            // convience method to slow keypresses.
            delayPressTimer.Update(gameTime);
            // convienience method for simple graphics and things.
            Timer1SecondOcillating.UpdateContinuously(gameTime);
            Timer3SecondOcillating.UpdateContinuously(gameTime);
            Timer7SecondOcillating.UpdateContinuously(gameTime);
        }

        /// <summary>
        /// Place this between spritebatch begin end.
        /// </summary>
        public static void Draw(GameTime gameTime)
        {
            // Lets draw the ui
            if (DefaultUi)
                MyUi.Draw(gameTime);

            // Draw the fps.
            if (DefaultDisplayFrameRate)
                frameRate.DrawFps(spriteBatch, currentFont, new Vector2(10f, 10f), Color.LawnGreen);
        }

        /// <summary>
        /// This timer instance just continuously updates as long a gu's update function is called.
        /// Use the Get methods form some useful things.
        /// </summary>
        public static Timer Timer1SecondOcillating = new Timer(1f);
        public static Timer Timer3SecondOcillating = new Timer(3f);
        public static Timer Timer7SecondOcillating = new Timer(7f);

        private static Timer delayPressTimer = new Timer(.2f);
        /// <summary>
        /// This property can be used to check for a keypress delay to avoid massive repeats on a keypress.
        /// </summary>
        public static bool PauseExpired
        {
            get { return DelayPressExpired(); }
        }
        /// <summary>
        /// This method can be used to check for a keypress delay to avoid massive repeats on a keypress.
        /// </summary>
        public static bool DelayPressExpired()
        {
            bool result = delayPressTimer.IsTriggered;
            if (result)
            {
                delayPressTimer.ResetElapsedToZero();
                return true;
            }
            else
                return false;
        }
        
        #region 2d drawing methods

        /// <summary>
        /// Draw text onto a background rectangle.
        /// </summary>
        public static void DrawTextOnBackgroundRectangle(MgStringBuilder s, Rectangle BoundingArea, Color TextColor, Color BackGroundColor)
        {
            spriteBatch.Draw(dotTexture, BoundingArea, BackGroundColor);
            Vector2 pos = new Vector2(BoundingArea.X, BoundingArea.Y);
            spriteBatch.DrawString(currentFont, s, pos, TextColor);
        }
        public static void DrawTextOnBackgroundRectangle(MgStringBuilder s, Vector2 scalesize, Rectangle BoundingArea, Color TextColor, Color BackGroundColor)
        {
            Vector2 pos = new Vector2(BoundingArea.X, BoundingArea.Y);
            spriteBatch.Draw(dotTexture, BoundingArea, BackGroundColor);
            spriteBatch.DrawString(currentFont, s, pos, TextColor, 0f, Vector2.Zero, scalesize, SpriteEffects.None, 0f);
        }
        /// <summary>
        /// filled
        /// </summary>
        public static void DrawSquare(Rectangle r, Color c)
        {
            spriteBatch.Draw(dotTexture, r, c);
        }
        /// <summary>
        /// Outline
        /// </summary>
        public static void DrawSquareBorder(Rectangle r, int lineThickness, Color c)
        {
            Rectangle TLtoR = new Rectangle(r.Left, r.Top, r.Width, lineThickness);
            Rectangle BLtoR = new Rectangle(r.Left, r.Bottom - lineThickness, r.Width, lineThickness);
            Rectangle LTtoB = new Rectangle(r.Left, r.Top, lineThickness, r.Height);
            Rectangle RTtoB = new Rectangle(r.Right - lineThickness, r.Top, lineThickness, r.Height);
            spriteBatch.Draw(dotTexture, TLtoR, c);
            spriteBatch.Draw(dotTexture, BLtoR, c);
            spriteBatch.Draw(dotTexture, LTtoB, c);
            spriteBatch.Draw(dotTexture, RTtoB, c);
        }
        /// <summary>
        /// Draw a 2d point.
        /// </summary>
        public static void DrawBasicPoint(Vector2 p, Color c)
        {
            Rectangle screendrawrect = new Rectangle((int)p.X, (int)p.Y, 2, 2);
            spriteBatch.Draw(dotTexture, screendrawrect, new Rectangle(0, 0, 1, 1), c, 0.0f, Vector2.One, SpriteEffects.None, 0);
        }
        public static void DrawRaySegment(Vector2 postion, int length, int linethickness, float rot, Color c)
        {
            rot += 3.14159274f;
            Rectangle screendrawrect = new Rectangle((int)postion.X, (int)postion.Y, linethickness, length);
            spriteBatch.Draw(dotTexture, screendrawrect, new Rectangle(0, 0, 1, 1), c, rot, Vector2.Zero, SpriteEffects.None, 0);
        }
        public static void DrawBasicLine(Vector2 s, Vector2 e, int thickness, Color linecolor, float rot)
        {
            float distance = Vector2.Distance(s, e);
            float direction = (float)Atan2Xna(e.X - s.X, e.Y - s.Y);
            //direction = DirectionToRadians(e.X - s.X, e.Y - s.Y);
            direction += rot;
            Rectangle screendrawrect = new Rectangle((int)s.X, (int)s.Y, thickness, (int)distance);
            spriteBatch.Draw(dotTexture, screendrawrect, new Rectangle(0, 0, 1, 1), linecolor, direction, Vector2.Zero, SpriteEffects.None, 0);
        }
        public static void DrawBasicLine(Vector2 s, Vector2 e, int thickness, Color linecolor)
        {
            float distance = Vector2.Distance(e, s);
            float direction = (float)Atan2Xna(e.X - s.X, e.Y - s.Y);
            Rectangle screendrawrect = new Rectangle((int)s.X, (int)s.Y, thickness, (int)distance);
            spriteBatch.Draw(dotTexture, screendrawrect, new Rectangle(0, 0, 1, 1), linecolor, direction, Vector2.Zero, SpriteEffects.None, 0);
        }
        #endregion

        #region Graphics Helper methods

        public static SamplerState SS_PointBorder = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Border, AddressV = TextureAddressMode.Border };
        public static SamplerState SS_PointClamp = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp };
        public static SamplerState SS_PointWrap = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap };
        public static SamplerState SS_PointMirror = new SamplerState() { Filter = TextureFilter.Point, AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror };

        public static RasterizerState RS_scissors_on = new RasterizerState() { ScissorTestEnable = true };
        public static RasterizerState RS_scissors_off = new RasterizerState() { ScissorTestEnable = false };


        public static bool ScissorsTestEnable
        {
            set { if (value) { Gu.device.RasterizerState = RS_scissors_on; } else { Gu.device.RasterizerState = RS_scissors_off; } }
            get { return device.RasterizerState.ScissorTestEnable; }
        }

        /// <summary>
        /// let's you clip pixels outside the set rectangle. ScissorsTestEnable must be enabled.
        /// </summary>
        public static Rectangle ScissorsRectangle
        {
            set { device.ScissorRectangle = value; }
            get { return device.ScissorRectangle; }
        }

        public static void WrapText(MgStringBuilder s, Rectangle BoundingArea)
        {
            //MgTextBounder.WordWrapTextWithinBounds(s, Vector2.One, BoundingArea, true);
        }
        public static void WrapText(MgStringBuilder s, Vector2 size, Rectangle BoundingArea, bool cutsVerticalBounds)
        {
            //MgTextBounder.WordWrapTextWithinBounds(s, size, BoundingArea, cutsVerticalBounds);
        }

        public static Texture2D TextureDotCreate(GraphicsDevice device)
        {
            Color[] data = new Color[1];
            data[0] = new Color(255, 255, 255, 255);
            return TextureFromColorArray(device, data, 1, 1);
        }
        public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
        {
            Texture2D tex = new Texture2D(device, width, height);
            tex.SetData<Color>(data);
            return tex;
        }

        public static float Atan2Xna(float difx, float dify)
        {
            return (float)Math.Atan2(difx, dify) * -1f;
        }

        /// <summary>
        /// simplifyed writeExceptionMsg
        /// </summary>
        /// <param name="errmsg"></param>
        public static void WriteExceptionStackFramesToFile(string errmsg, int numberOfFramesToTrace)
        {
            string log_tempstring = "\n";
            log_tempstring += ("\n Exception Thrown");
            log_tempstring += ("\n _______________________________________________________");
            log_tempstring += ("\n " + errmsg);
            log_tempstring += ("\n _______________________________________________________");
            log_tempstring += ("\n StackTrace As Follows \n");
            try
            {
                // Create a StackTrace that captures filename,
                // linepieces number and column information.
                StackTrace st = new StackTrace(1, true);
                int count = st.FrameCount;
                if (numberOfFramesToTrace > count) { numberOfFramesToTrace = count; }
                //
                for (int i = 0; i < numberOfFramesToTrace; i++)
                {
                    StackFrame sf = st.GetFrame(i);

                    log_tempstring += " \n  stack # " + i.ToString();
                    log_tempstring += "  File: " + Path.GetFileNameWithoutExtension(sf.GetFileName());
                    log_tempstring += "  Method: " + sf.GetMethod().ToString();
                    log_tempstring += "  Line: " + sf.GetFileLineNumber().ToString();
                    log_tempstring += "  Column: " + sf.GetFileColumnNumber().ToString();
                    // output the full stack frame
                }
                throw new Exception();
            }
            catch (Exception e)
            {
                string fullpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ErrorLog.txt");
                File.WriteAllText(fullpath, log_tempstring);
                Process.Start(fullpath);
                throw e;
            }
        }

        #endregion

        #region Extension To Gu Till i make the inner properties public static ill do it later.

        /// <summary>
        /// This changes a vector4 that holds a floating screen rectangles positions and converts them into virtual coordinates
        /// </summary>
        public static Vector4 ToVirtualCoordinates(this Vector4 v)
        {
            return new Vector4(v.X * ScrMult.X, v.Y * ScrMult.Y, v.Z * ScrMult.X, v.W * ScrMult.Y);
        }
        /// <summary>
        /// This changes a Rectangle into Virtual coordinates relative to the screen size.
        /// </summary>
        public static Vector4 ToVirtualCoordinates(this Rectangle v)
        {
            return new Vector4((int)(v.X * ScrMult.X), (int)(v.Y * ScrMult.Y), (int)(v.Width * ScrMult.X), (int)(v.Height * ScrMult.Y));
        }
        /// <summary>
        /// Takes a Vector4 that holds virtual coordinates for positions in relation to the screen to return the actual positional bounds as a rectangle.
        /// </summary>
        public static Rectangle FromVirtualCoordinatesToRectangle(this Vector4 v)
        {
            return new Rectangle((int)(v.X * ScreenWidth), (int)(v.Y * ScreenHeight), (int)(v.Z * ScreenWidth), (int)(v.W * ScreenHeight));
        }

        #endregion

    }

    /// <summary>
    /// Timer class based on seconds.
    /// </summary>
    public class Timer
    {
        private float timer = 0f;
        private float totalElapsed = 0f;
        private float multiplier = 1f;
        bool isTriggered = false;
        const float pi = 3.141592653f;

        public Timer()
        {
            SetTimer = 1.0f;
            IsActivelyTiming = false;
            isTriggered = false;
        }
        public Timer(float seconds)
        {
            SetTimer = seconds;
            IsActivelyTiming = true;
            isTriggered = false;
        }
        public Timer(float seconds, bool startingStateActive)
        {
            SetTimer = seconds;
            IsActivelyTiming = startingStateActive;
            isTriggered = false;
        }

        public bool IsActivelyTiming { get; set; } = false;
        public float SetTimer
        {
            set
            {
                timer = value;
            }
        }
        public float GetTimer
        {
            get
            {
                return timer;
            }
        }
        public float SetTimerAndStart
        {
            set
            {
                timer = value;
                IsActivelyTiming = true;
                isTriggered = false;
            }
        }

        public float AddToTimer
        {
            set
            {
                timer += value;
            }
        }
        public float AddToElapsed
        {
            set
            {
                totalElapsed += value;
            }
        }
        public float AddToMultiplier
        {
            set
            {
                multiplier += value;
            }
        }
        public float Multiplier
        {
            set
            {
                multiplier = value;
            }
            get
            {
                return multiplier;
            }
        }

        /// <summary>
        /// When used as a stop watch.
        /// </summary>
        public bool IsTimedAmountReached
        {
            get { return isTriggered; }
        }
        /// <summary>
        /// When used as a stop watch.
        /// </summary>
        public bool IsTriggered
        {
            get { return isTriggered; }
        }

        /// <summary>
        /// performs a rotation of time multiplied by 2 pi
        /// </summary>
        public float GetElapsedTimeAsRadianRotation
        {
            get { return GetElapsedPercentageOfTimer * 2 * pi; }
        }
        /// <summary>
        /// Gets back the time in the form of the Sin portion of Sin Cos
        /// </summary>
        public float GetElapsedTimeAsSine
        {
            get { return (float)Math.Sin(GetElapsedPercentageOfTimer * 2 * pi); }
        }
        /// <summary>
        /// Gets back the time in the form of the Cos portion of Sin Cos
        /// </summary>
        public float GetElapsedTimeAsCosine
        {
            get { return (float)Math.Cos(GetElapsedPercentageOfTimer * 2 * pi); }
        }
        /// <summary>
        /// Get the elapsed time as a percentage from o to 1
        /// </summary>
        public float GetElapsedPercentageOfTimer
        {
            get
            {
                if (timer > 0f)
                    return totalElapsed / timer;
                else
                    return 0;
            }
        }
        /// <summary>
        /// Get the elapsed time as a percentage from 1 to 0
        /// </summary>
        public float GetElapsedPercentageOfTimerInverted
        {
            get
            {
                if (timer > 0f)
                    return 1f - (totalElapsed / timer);
                else
                    return 0;
            }
        }
        /// <summary>
        /// performs a linear oscilation on the given time this ranges from 0 to 1 to 0
        /// </summary>
        public float GetElapsedPercentageAsOscillation
        {
            get
            {
                if (timer > 0f)
                {
                    var half = timer * .5f;
                    var n = (totalElapsed - half) / half;
                    if (n < 0f)
                        n = -n;
                    return 1f - n;
                }
                else
                    return 0;
            }
        }
        /// <summary>
        /// performs a linear oscilation on the given time this ranges from 1 to 0 to 1
        /// </summary>
        public float GetElapsedPercentageAsOscillationInverted
        {
            get
            {
                return 1f - GetElapsedPercentageAsOscillation;
            }
        }

        public void StartTimer()
        {
            IsActivelyTiming = true;
        }
        public void StopTimer()
        {
            IsActivelyTiming = false;
        }
        /// <summary>
        /// Leaves the timer running, retains the timing amount and restarts the timer at zero.
        /// </summary>
        public void ResetElapsedToZero()
        {
            totalElapsed = 0;
            isTriggered = false;
            IsActivelyTiming = true;
        }
        /// <summary>
        /// Turns off the timer completely.
        /// </summary>
        public void ClearStopResetTimer()
        {
            totalElapsed = 0;
            isTriggered = false;
            multiplier = 1f;
            IsActivelyTiming = false;
        }

        /// <summary>
        /// Returns true if the time reaches its timer amount
        /// </summary>
        public bool Update(GameTime gameTime)
        {
            return Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        /// <summary>
        /// Returns true if the time reaches its timer amount
        /// </summary>
        public bool Update(float elapsedTimeInSeconds)
        {
            totalElapsed += elapsedTimeInSeconds * multiplier;
            if (totalElapsed >= timer)
            {
                isTriggered = true;
                totalElapsed = timer;
            }
            return isTriggered;
        }

        public void UpdateContinuously(GameTime gameTime)
        {
            UpdateContinuously((float)(gameTime.ElapsedGameTime.TotalSeconds));
        }
        /// <summary>
        /// Updates continuously typically used for oscillarions or looping algorithms
        /// </summary>
        public void UpdateContinuously(float elapsedTimeInSeconds)
        {
            totalElapsed += elapsedTimeInSeconds * multiplier;
            if (totalElapsed >= timer)
            {
                totalElapsed -= timer;
                IsActivelyTiming = true;
            }
        }
    }


    /// <summary>
    /// A more detailed frameRate class.
    /// </summary>
    public class MgFrameRate
    {
        public Texture2D dotTexture;
        SpriteBatch spriteBatch;
        MgStringBuilder msg = new MgStringBuilder();
        MgStringBuilder fpsmsg = new MgStringBuilder();
        MgStringBuilder gcmsg = new MgStringBuilder();

        public bool DisplayFrameRate = true;
        public bool DisplayGarbageCollectionRate = true;
        public bool DisplayVisualizations = true;
        public double DisplayedMessageFrequency = 1.0d;
        public bool DisplayCollectionAlert = true;

        //private long gcDisplayDelay = 0;

        private double fps = 0d;
        private double frames = 0;
        private double updates = 0;
        private double ufRatio = 1f;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;

        private long gcNow = 0;
        private long gcLast = 0;
        private long gcDiff = 0;
        private long gcAccumulatedInc = 0;
        private long gcAccumulatedSinceLastCollect = 0;
        private long gcTotalLost = 0;
        private long gcSizeOfLastCollect = 0;
        private long gcRecordedCollects = 0;


        private const double MEGABYTE = 1048576d;


        public void LoadSetUp(Game pass_in_this, GraphicsDeviceManager gdm, SpriteBatch spriteBatch, bool allowresizing, bool showmouse, bool fixedon, bool vsync, double desiredFramesPerSecond, bool displayGc)
        {
            this.spriteBatch = spriteBatch;
            DisplayGarbageCollectionRate = displayGc;
            dotTexture = TextureDotCreate(pass_in_this.GraphicsDevice);
            pass_in_this.Window.AllowUserResizing = allowresizing;
            pass_in_this.IsMouseVisible = showmouse;
            pass_in_this.IsFixedTimeStep = fixedon;
            if (fixedon)
            {
                //pass_in_this.TargetElapsedTime = TimeSpan.FromSeconds(1d / desiredFramesPerSecond);
                pass_in_this.TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond / desiredFramesPerSecond));
            }
            gdm.SynchronizeWithVerticalRetrace = vsync;
            gdm.ApplyChanges();
        }

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            //now = gameTime.TotalGameTime.TotalSeconds; // TimeSpan.FromTicks(166666);
            now = (double)gameTime.TotalGameTime.Ticks / (double)TimeSpan.TicksPerSecond;
            elapsed = (double)(now - last);

            // fps msg's
            if (elapsed >= DisplayedMessageFrequency) // || (gcDiff != 0)
            {
                // time
                if (DisplayFrameRate)
                {
                    fps = (frames / elapsed);
                    ufRatio = (float)frames / (float)updates;

                    fpsmsg.Clear();
                    fpsmsg.Append(" Minutes Running: ").AppendTrim(now / 60d, 3).AppendLine(); // .Append(now).AppendLine();
                    fpsmsg.Append(" Fps: ").AppendTrim(fps).AppendLine();
                    fpsmsg.Append(" Draw to Update Ratio: ").AppendTrim(ufRatio).AppendLine();
                    fpsmsg.Append(" Elapsed interval: ").AppendTrim(elapsed).AppendLine();
                    fpsmsg.Append(" Interval error: ").Append(elapsed - DisplayedMessageFrequency).AppendLine();
                    fpsmsg.Append(" Updates: ").Append(updates).AppendLine();
                    fpsmsg.Append(" Frames: ").Append(frames).AppendLine();

                    //elapsed -= msgFrequency;
                    frames = 0;
                    updates = 0;
                    last = now;
                }
                // Gc Messages
                if (DisplayGarbageCollectionRate)
                {
                    gcLast = gcNow;
                    gcNow = GC.GetTotalMemory(false);
                    gcDiff = gcNow - gcLast;

                    // give the app a little time to load and let the gc run a bit.
                    if (now < 5d)
                    {
                        gcSizeOfLastCollect = 0;
                        gcTotalLost = 0;
                        gcRecordedCollects = 0;
                        gcAccumulatedInc = 0;
                        gcAccumulatedSinceLastCollect = 0;
                        gcLast = gcNow;
                    }

                    gcmsg.Clear();
                    gcmsg.Append(" GC Memory(Mb) Now: ").AppendTrim((double)gcNow / MEGABYTE).AppendLine();

                    if (gcDiff == 0)
                    {
                        gcmsg.AppendLine(" GC Memory(Mb) No change");
                    }
                    if (gcDiff < 0)
                    {
                        gcmsg.AppendLine(" !!! COLLECTION OCCURED !!! ").Append(" GC Memory(Mb) Lost: ").AppendTrim((double)(gcDiff) / MEGABYTE).AppendLine();
                        var tmp = -gcDiff;
                        gcSizeOfLastCollect = tmp;
                        gcTotalLost += tmp;
                        gcRecordedCollects++;
                        gcAccumulatedInc += tmp;
                        gcAccumulatedSinceLastCollect = 0;
                        gcDiff = 0;
                        gcLast = gcNow;
                    }
                    if (gcDiff > 0)
                    {
                        gcmsg.Append(" GC Memory(Mb) Increased: ").AppendTrim((double)(gcDiff) / MEGABYTE).AppendLine();
                        //
                        //if (GcTrackingDelayCompleted)
                        //{
                        gcAccumulatedInc += gcDiff;
                        gcAccumulatedSinceLastCollect += gcDiff;
                        gcLast = gcNow;
                        //}
                    }
                    gcmsg.Append(" GC Memory(Mb) AccumulatedInc: ").AppendTrim(gcAccumulatedInc / MEGABYTE, 6).AppendLine();
                    if (gcRecordedCollects > 0)
                    {
                        gcmsg.Append(" GC Memory(Mb) Accumulated Since Collect: ").AppendTrim(gcAccumulatedSinceLastCollect / MEGABYTE).AppendLine();
                        gcmsg.Append(" GC Memory(Mb) Last Lost Collection: ").AppendTrim(gcSizeOfLastCollect / MEGABYTE).AppendLine();
                        gcmsg.Append(" GC Memory(Mb) Total Lost to Collections: ").AppendTrim(gcTotalLost / MEGABYTE).AppendLine();
                    }
                    gcmsg.Append(" GC Number Of Memory Collections: ").AppendTrim(gcRecordedCollects).AppendLine();
                }
                msg.Clear();
                msg.Append(fpsmsg).Append(gcmsg);
            }
            updates++;
        }

        public void DrawFps(SpriteBatch spritebatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            if (DisplayVisualizations)
            {
                DrawVisualizations(fpsTextColor);
                fpsDisplayPosition.Y += 10;
            }
            spriteBatch = spritebatch;
            if (DisplayCollectionAlert && gcRecordedCollects > 0)
                spriteBatch.DrawString(font, msg, fpsDisplayPosition, new Color(1f, fpsTextColor.G, fpsTextColor.B, fpsTextColor.A));
            else
                spriteBatch.DrawString(font, msg, fpsDisplayPosition, fpsTextColor);
            frames++;
        }

        public void DrawVisualizations(Color fpsTextColor)
        {
            var dist = 600d;
            var vmsrheight = 4;
            var vmsrWidth = 30;
            Rectangle visualMotionStutterRect = new Rectangle((int)(dist * elapsed) + 5, 2, vmsrWidth, vmsrheight);
            Rectangle visualMotionStutterRect2 = new Rectangle((int)(dist - (dist * elapsed)) + 5, 2, vmsrWidth, vmsrheight);
            DrawFilledSquare(visualMotionStutterRect, Color.LemonChiffon);
            DrawFilledSquare(visualMotionStutterRect2, Color.LemonChiffon);

            var visualFps = fps;
            if (visualFps > 120) { visualFps = 120d + (visualFps / 20d); }
            Rectangle visualFpsRect = new Rectangle(10 + (int)(visualFps), 10, 4, 2);

            var visualFrame = frames / elapsed;
            if (visualFrame > 120) { visualFrame = 120d + (visualFrame / 20d); }
            Rectangle visualFrameRect = new Rectangle(10 + (int)(visualFrame), 12, 4, 2);

            DrawSquare(visualFpsRect, 3, Color.Aqua);
            DrawSquare(visualFrameRect, 3, Color.Blue);
            DrawRaySegment(new Vector2(15, 15), 15, 2, (float)( (elapsed / DisplayedMessageFrequency) * Math.PI *2d), Color.Gray);

            var gcCoefficient = (150d / gcNow);
            var visualGcIncrease = (double)(gcAccumulatedInc * gcCoefficient);
            Rectangle visualGcRect = new Rectangle(10, 14, 150, 4);
            Rectangle visualGcIncreaseRect = new Rectangle(10, 15, (int)(visualGcIncrease), 2);

            DrawSquare(visualGcRect, 1, Color.Orange);
            DrawSquare(visualGcIncreaseRect, 1, Color.Red);
        }


        public static Texture2D TextureDotCreate(GraphicsDevice device)
        {
            Color[] data = new Color[1];
            data[0] = new Color(255, 255, 255, 255);
            return TextureFromColorArray(device, data, 1, 1);
        }
        public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
        {
            Texture2D tex = new Texture2D(device, width, height);
            tex.SetData<Color>(data);
            return tex;
        }
        public void DrawSquare(Rectangle r, int lineThickness, Color c)
        {
            Rectangle TLtoR = new Rectangle(r.Left, r.Top, r.Width, lineThickness);
            Rectangle BLtoR = new Rectangle(r.Left, r.Bottom - lineThickness, r.Width, lineThickness);
            Rectangle LTtoB = new Rectangle(r.Left, r.Top, lineThickness, r.Height);
            Rectangle RTtoB = new Rectangle(r.Right - lineThickness, r.Top, lineThickness, r.Height);
            spriteBatch.Draw(dotTexture, TLtoR, c);
            spriteBatch.Draw(dotTexture, BLtoR, c);
            spriteBatch.Draw(dotTexture, LTtoB, c);
            spriteBatch.Draw(dotTexture, RTtoB, c);
        }
        public void DrawFilledSquare(Rectangle r, Color c)
        {
            spriteBatch.Draw(dotTexture, r, c);
        }
        public void DrawRaySegment(Vector2 postion, int length, int linethickness, float rot, Color c)
        {
            rot += 3.141592f;
            Rectangle screendrawrect = new Rectangle((int)postion.X, (int)postion.Y, linethickness, length);
            spriteBatch.Draw(dotTexture, screendrawrect, new Rectangle(0, 0, 1, 1), c, rot, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}

