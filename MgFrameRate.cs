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

