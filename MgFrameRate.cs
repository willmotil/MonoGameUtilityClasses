using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
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
        public double msgFrequency = 1.0f;

        private long gcDisplayDelay = 10;

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
        private long gcTotalLost = 0;
        private long gcRecordedCollects = 0;


        public void LoadSetUp(Game pass_in_this, GraphicsDeviceManager gdm, SpriteBatch spriteBatch, bool allowresizing, bool showmouse, bool fixedon, bool vsync, double desiredFramesPerSecond)
        {
            this.spriteBatch = spriteBatch;
            dotTexture = TextureDotCreate(pass_in_this.GraphicsDevice);
            pass_in_this.Window.AllowUserResizing = allowresizing;
            pass_in_this.IsMouseVisible = showmouse;
            pass_in_this.IsFixedTimeStep = fixedon;
            if (fixedon)
            {
                pass_in_this.TargetElapsedTime = TimeSpan.FromSeconds(1d / desiredFramesPerSecond);
                //long temp = (long)((1.000000d / desiredFramesPerSecond) * 10000000);// 10000000););
                //pass_in_this.TargetElapsedTime = TimeSpan.FromTicks(temp);
            }
            gdm.SynchronizeWithVerticalRetrace = vsync;
            gdm.ApplyChanges();
        }

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            now = gameTime.TotalGameTime.TotalSeconds; // TimeSpan.FromTicks(166666);
            elapsed = (double)(now - last);

            // fps msg's
            if (elapsed >= msgFrequency) // || (gcDiff != 0)
            {
                // time
                if (DisplayFrameRate)
                {
                    fps = (frames / elapsed);
                    ufRatio = (float)frames / (float)updates;

                    fpsmsg.Clear();
                    fpsmsg.Append(" Fps: ").AppendTrim(fps).AppendLine();
                    fpsmsg.Append(" Draw to Update Ratio: ").AppendTrim(ufRatio).AppendLine();
                    fpsmsg.Append(" Elapsed interval: ").AppendTrim(elapsed).AppendLine();
                    fpsmsg.Append(" Updates: ").Append(updates).AppendLine();
                    fpsmsg.Append(" Frames: ").Append(frames).AppendLine();
                    fpsmsg.Append(" Seconds Running: ").AppendTrim((now)).AppendLine();

                    elapsed = 0;
                    frames = 0;
                    updates = 0;
                    last = now;
                }
                // Gc Messages
                if (DisplayGarbageCollectionRate)
                {
                    gcNow = GC.GetTotalMemory(false);
                    gcDiff = gcNow - gcLast;

                    bool GcTrackingDelayCompleted = true;
                    if (gcDisplayDelay > 0) { gcDisplayDelay -= 1; GcTrackingDelayCompleted = false; gcDiff = 0; gcLast = gcNow; }

                    gcmsg.Clear();
                    gcmsg.Append(" GC Memory(Kb) Now: ").AppendTrim(gcNow / 1024d).AppendLine();

                    if (gcDiff < 0)
                    {
                        gcmsg.Append(" GC Memory(Kb) Lost: ").AppendTrim((double)(gcDiff) / 1024d).AppendLine();
                        //
                        if (GcTrackingDelayCompleted)
                        {
                            gcTotalLost += gcDiff;
                            gcRecordedCollects++;
                            gcAccumulatedInc = 0;
                            gcDiff = 0;
                            gcLast = gcNow;
                        }
                    }
                    if (gcDiff > 0)
                    {
                        gcmsg.Append(" GC Memory(Kb) Increased: ").AppendTrim((double)(gcDiff) / 1024d).AppendLine();
                        //
                        if (GcTrackingDelayCompleted)
                        {
                            gcAccumulatedInc += gcDiff;
                            gcLast = gcNow;
                        }
                    }
                    gcmsg.Append(" GC Memory(Kb) AccumulatedInc: ").AppendTrim(gcAccumulatedInc / 1024d).AppendLine();
                    gcmsg.Append(" GC Memory(Kb) TotalLost: ").AppendTrim(gcTotalLost / 1024d).AppendLine();
                    gcmsg.Append(" GC Memory collects: ").AppendTrim(gcRecordedCollects).AppendLine();
                }
                msg.Clear();
                msg.Append(fpsmsg).Append(gcmsg);
            }
            updates++;
        }

        public void DrawFps(SpriteBatch spriteBatch, SpriteFont font, Vector2 fpsDisplayPosition, Color fpsTextColor)
        {
            if (DisplayVisualizations)
            {
                DrawVisualizations(fpsTextColor);
                fpsDisplayPosition.Y += 10;
            }
            spriteBatch.DrawString(font, msg, fpsDisplayPosition, fpsTextColor);
            frames++;
        }

        public void DrawVisualizations(Color fpsTextColor)
        {
            var gcCoefficient = (150d / gcNow);
            var visualGcIncrease = (double)(gcAccumulatedInc * gcCoefficient);
            Rectangle visualGcRect = new Rectangle(10, 2, 150, 4);
            Rectangle visualGcIncreaseRect = new Rectangle(10, 3, (int)(visualGcIncrease), 2);

            var visualUpdates = updates;
            if (visualUpdates > 120) { visualUpdates = 120d + (visualUpdates / 20d); }
            Rectangle visualUpdatesRect = new Rectangle(10, 8, (int)(visualUpdates), 1);

            var visualDraws = frames;
            if (visualDraws > 120) { visualDraws = 120d + (visualDraws / 20d); }
            Rectangle visualDrawsRect = new Rectangle(10, 12, (int)(visualDraws), 1);

            var visualFps = fps;
            if (visualFps > 120) { visualFps = 120d + (visualFps / 20d); }
            Rectangle visualFpsRect = new Rectangle(10 + (int)(visualFps), 12, 4, 2);

            var visualFrame = frames / elapsed;
            if (visualFrame > 120) { visualFrame = 120d + (visualFrame / 20d); }
            Rectangle visualFrameRect = new Rectangle(10 + (int)(visualFrame), 14, 4, 2);

            DrawSquare(visualGcRect, 1, Color.BlueViolet);
            DrawSquare(visualGcIncreaseRect, 1, Color.Red);
            DrawSquare(visualUpdatesRect, 2, Color.Green);
            DrawSquare(visualDrawsRect, 2, Color.Aqua);
            DrawSquare(visualFpsRect, 3, Color.Aqua);
            DrawSquare(visualFrameRect, 3, fpsTextColor);
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
    }
}
