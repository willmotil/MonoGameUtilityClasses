using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace YourNameSpaceHere
{
    /// <summary>
    /// This MonoGame class demonstrates how to setup... 
    /// The MgFrameRate class to see the fps and the associated garbage collection rates.
    /// The text bounding class to wrap text.
    /// The MgStringBuilder class is shown to be used to avoid garbage collections at runtime.
    /// </summary>
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        SpriteFont font;

        MgFrameRate fps = new MgFrameRate();
        Rectangle textBoundedArea;
        MgStringBuilder originalText = "This is a MgStringBuilder a wrapper around string builder";

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //
            // Add a font this ones created thru the pipeline
            //
            font = Content.Load<SpriteFont>("MgFont");

            //
            // Set the current font to the text bounding font so it can operate on the text.
            //
            MgTextBounder.SetSpriteFont(font);

            //
            // Set up the fps class. 
            // by passing your desired settings for, 
            // showing the mouse, resizing, desired frames per second ect.
            //
            fps.LoadSetUp(this, graphics, spriteBatch, true, true, false, false, 100d);

        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Escape))
                Exit();

            // Update fps
            //
            fps.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Moccasin);

            spriteBatch.Begin();

            //
            // Draw the fps msg
            //
            fps.DrawFps(spriteBatch, font, new Vector2(10f, 10f), Color.MonoGameOrange);

            //
            // Uncomment to test no garbage word wrapping.
            // This is of course faster if it is pre-computed where possible.
            //
            RunTimeWordWrapping(gameTime);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // Usage of the MgStringBuilder wrapper to make some text.
        public void RunTimeWordWrapping(GameTime gameTime)
        {
            // ______________________________________________________________________
            // Here we do some word wrapping.

            //
            // Make a rectangle text area and some text.
            //
            textBoundedArea = new Rectangle(200, 200, 300, 90);

            //
            // Repeatedly make a messege every frame.
            //
            originalText.Length = 0;
            var gt = (float)(gameTime.TotalGameTime.TotalSeconds);
            originalText.Append("Hello this is a mgsb test. ").AppendLine("The pupose of which is to demonstrate the fps and text rectangle wrapping functions. ").Append("In a Dynamic context ").AppendTrim(gt);

            //
            // Call the function on a MgStringBuilder object. 
            // Note while this can work for stringbuilder... 
            // You have to do it a specific way to not get garbage specifically precalculate it.
            //
            MgTextBounder.WordWrapTextWithinBounds(ref originalText, Vector2.One, textBoundedArea);
            
            //
            // Draw the background rectangle and the wrapped text.
            //
            spriteBatch.Draw(fps.dotTexture, textBoundedArea, Color.AntiqueWhite);

            //
            // Draw the wrapped text to the bounded xy position.
            //
            Vector2 pos = new Vector2(textBoundedArea.X, textBoundedArea.Y);
            
            spriteBatch.DrawString(font, originalText, pos, Color.Blue);
        }

    }
}
