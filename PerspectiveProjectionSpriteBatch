using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// https://community.monogame.net/t/minimal-example-of-drawing-a-quad-into-2d-space/11063/3

namespace PerspectiveQuadToSpritebatchAlignment
{
    public class Game_QuadDrawExample : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        public static SpriteFont font;
        public static Effect effect;
        Matrix World, View , Projection = Matrix.Identity;
        float fieldOfView = 1.4f;
        public static Texture2D generatedTexture;
        public static Vector3 cameraScrollPosition = new Vector3(0, 0, 0);

        public static bool UsePerspectiveSpriteBatchEquivillent { get; set; } = true;


        public Game_QuadDrawExample()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            this.Window.ClientSizeChanged += Resize;
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 500;
            graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("MgGenFont");
            effect = Content.Load<Effect>("CameraTestsEffect");
            generatedTexture = GenerateTexture2DWithTopLeftDiscoloration();
        }

        protected override void UnloadContent()
        {
            generatedTexture.Dispose();
        }

        public void Resize(object sender, EventArgs e) { }

        protected override void Update(GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float pixelsPerSecond = 100f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                cameraScrollPosition.Y += -pixelsPerSecond * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                cameraScrollPosition.Y += pixelsPerSecond * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                cameraScrollPosition.X += -pixelsPerSecond * elapsed;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                cameraScrollPosition.X += pixelsPerSecond * elapsed;

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
                UsePerspectiveSpriteBatchEquivillent = true;
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                UsePerspectiveSpriteBatchEquivillent = false;

            SetCameraPosition2D(cameraScrollPosition.X, cameraScrollPosition.Y);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.Transparent);

            Rectangle myColoredRectangle = new Rectangle(10, 40, 450, 260);

            SetStates();

            SetUpEffect();

            CreateAndThenDrawVertexRectangle(myColoredRectangle);

            //DrawOutMatrixInformationWithSpriteBatch();

            base.Draw(gameTime);
        }

        public void SetUpEffect()
        {
            effect.CurrentTechnique = effect.Techniques["QuadDraw"];
            World = Matrix.Identity;

            if (UsePerspectiveSpriteBatchEquivillent)
                PerspectiveAligned(GraphicsDevice, cameraScrollPosition, fieldOfView, out View, out Projection);
            else
                OrthographicAligned(GraphicsDevice, cameraScrollPosition, out View, out Projection);

            effect.Parameters["World"].SetValue(World);
            effect.Parameters["View"].SetValue(View);
            effect.Parameters["Projection"].SetValue(Projection);
            effect.Parameters["TextureA"].SetValue(generatedTexture);
        }

        public void PerspectiveAligned(GraphicsDevice device, Vector3 scollPositionOffset, float fieldOfView, out Matrix view, out Matrix projection)
        {
            var dist = -((1f / (float)Math.Tan(fieldOfView / 2)) * (device.Viewport.Height / 2));
            var pos = new Vector3(device.Viewport.Width / 2, device.Viewport.Height / 2, dist) + scollPositionOffset;
            var target = new Vector3(0, 0, 1) + pos;
            var cameraWorld = Matrix.CreateWorld(pos, target - pos, Vector3.Down);
            view = Matrix.Invert(cameraWorld);
            projection = CreateInfinitePerspectiveFieldOfViewRHLH(fieldOfView, device.Viewport.AspectRatio, 0, 2000, true);
            this.Window.Title = "QuadToSpritebatchAlignment (F1 or F2) Perspective use arrow keys to scroll";
        }
        public void OrthographicAligned(GraphicsDevice device, Vector3 scollPositionOffset, out Matrix view, out Matrix projection)
        {
            float forwardDepthDirection = 1f;
            view = Matrix.Invert(Matrix.CreateWorld(scollPositionOffset, new Vector3(0, 0, 1), Vector3.Down));
            projection = Matrix.CreateOrthographicOffCenter(0, device.Viewport.Width, -device.Viewport.Height, 0, forwardDepthDirection * 0, forwardDepthDirection * 1f);
            this.Window.Title = "QuadToSpritebatchAlignment (F1 or F2) Othographic use arrow keys to scroll";
        }

        public void CreateAndThenDrawVertexRectangle(Rectangle r)
        {
            VertexPositionColorTexture[] quad = new VertexPositionColorTexture[6];
            if (GraphicsDevice.RasterizerState == RasterizerState.CullClockwise)
            {
                quad[0] = new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f));  // p1
                quad[1] = new VertexPositionColorTexture(new Vector3(r.Left, r.Bottom, 0f), Color.Red, new Vector2(0f, 1f)); // p0
                quad[2] = new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f));// p3

                quad[3] = new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f));// p3
                quad[4] = new VertexPositionColorTexture(new Vector3(r.Right, r.Top, 0f), Color.Blue, new Vector2(1f, 0f));// p2
                quad[5] = new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f)); // p1
            }
            else
            {
                quad[0] = new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f));  // p1
                quad[2] = new VertexPositionColorTexture(new Vector3(r.Left, r.Bottom, 0f), Color.Red, new Vector2(0f, 1f)); // p0
                quad[1] = new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f));// p3

                quad[4] = new VertexPositionColorTexture(new Vector3(r.Right, r.Bottom, 0f), Color.Green, new Vector2(1f, 1f));// p3
                quad[3] = new VertexPositionColorTexture(new Vector3(r.Right, r.Top, 0f), Color.Blue, new Vector2(1f, 0f));// p2
                quad[5] = new VertexPositionColorTexture(new Vector3(r.Left, r.Top, 0f), Color.White, new Vector2(0f, 0f)); // p1
            }
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, quad, 0, 2);
            }
        }

        public void SetStates()
        {
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        public void SetCameraPosition2D(float x, float y)
        {
            cameraScrollPosition.X = x;
            cameraScrollPosition.Y = y;
            cameraScrollPosition.Z = 0;
        }

        public Texture2D GenerateTexture2DWithTopLeftDiscoloration()
        {
            Texture2D t = new Texture2D(this.GraphicsDevice, 250, 250);
            var cdata = new Color[250 * 250];
            for (int i = 0; i < 250; i++)
            {
                for (int j = 0; j < 250; j++)
                {
                    if (i < 50 && j < 50)
                        cdata[i * 250 + j] = new Color(120, 120, 120, 250);
                    else
                        cdata[i * 250 + j] = Color.White;
                }
            }
            t.SetData(cdata);
            return t;
        }

        public static Matrix CreateInfinitePerspectiveFieldOfViewRHLH(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance, bool isRightHanded)
        {
            /* RH
             m11= xscale           m12= 0                 m13= 0                  m14=  0
             m21= 0                  m22= yscale          m23= 0                  m24= 0
             m31= 0                  0                          m33= f/(f-n) ~        m34= -1 ~
             m41= 0                  m42= 0                m43= n*f/(n-f) ~     m44= 0  
             where:
             yScale = cot(fovY/2)
             xScale = yScale / aspect ratio
           */
            if ((fieldOfView <= 0f) || (fieldOfView >= 3.141593f)){ throw new ArgumentException("fieldOfView <= 0 or >= PI"); }

            Matrix result = new Matrix();
            float yscale = 1f / ((float)Math.Tan((double)(fieldOfView * 0.5f)));
            float xscale = yscale / aspectRatio;
            var negFarRange = float.IsPositiveInfinity(farPlaneDistance) ? -1.0f : farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.M11 = xscale;
            result.M12 = result.M13 = result.M14 = 0;
            result.M22 = yscale;
            result.M21 = result.M23 = result.M24 = 0;
            result.M31 = result.M32 = 0f;
            if (isRightHanded)
            {
                result.M33 = negFarRange;
                result.M34 = -1;
                result.M43 = nearPlaneDistance * negFarRange;
            }
            else
            {
                result.M33 = negFarRange;
                result.M34 = 1;
                result.M43 = -nearPlaneDistance * negFarRange;
            }
            result.M41 = result.M42 = result.M44 = 0;
            return result;
        }

        public void DrawOutMatrixInformationWithSpriteBatch()
        {
            spriteBatch.Begin();
            var drawpos = new Vector2(10, 10);
            drawpos = MatrixSpriteBatchOut(spriteBatch, font, drawpos, Color.White, World, "World");
            drawpos = MatrixSpriteBatchOut(spriteBatch, font, drawpos, Color.White, View, "View");
            drawpos = MatrixSpriteBatchOut(spriteBatch, font, drawpos, Color.White, Projection, "Projection");
            drawpos = MatrixSpriteBatchOut(spriteBatch, font, drawpos, Color.White, View * Projection, "VP");
            spriteBatch.End();
        }

        public static Vector2 MatrixSpriteBatchOut(SpriteBatch spriteBatch, SpriteFont font, Vector2 textPosition, Color col, Matrix m, string name)
        {
            var textPos = textPosition;
            spriteBatch.DrawString(font, name, textPos, col);
            textPos.Y += font.LineSpacing;
            float spacing = 110;
            spriteBatch.DrawString(font, " M11: " + m.M11.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M12: " + m.M12.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M13: " + m.M13.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M14: " + m.M14.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " Right ", textPos, col); textPos.X += spacing;
            textPos.X = textPosition.X; textPos.Y += font.LineSpacing;
            spriteBatch.DrawString(font, " M21: " + m.M21.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M22: " + m.M22.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M23: " + m.M23.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M24: " + m.M24.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " Up ", textPos, col); textPos.X += spacing;
            textPos.X = textPosition.X; textPos.Y += font.LineSpacing;
            spriteBatch.DrawString(font, " M31: " + m.M31.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M32: " + m.M32.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M33: " + m.M33.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M34: " + m.M34.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " Forward ", textPos, col); textPos.X += spacing;
            textPos.X = textPosition.X; textPos.Y += font.LineSpacing;
            spriteBatch.DrawString(font, " M41: " + m.M41.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M42: " + m.M42.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M43: " + m.M43.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " M44: " + m.M44.ToString("#0.000"), textPos, col); textPos.X += spacing;
            spriteBatch.DrawString(font, " Position ", textPos, col); textPos.X += spacing;
            textPos.X = textPosition.X; textPos.Y += font.LineSpacing;
            return textPos;
        }

    }
}
