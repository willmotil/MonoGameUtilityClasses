using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
        /// <summary>
        /// Framerate display class.
        /// Example class to demonstrate no garbage numerical text, 
        /// this gives basic game info about frame and update rates as well as gc information.
        /// this also lets you set most of the framerate related stuff up.
        /// </summary>
        public class MgFrameRate
        {

            #region _region initial variable and object declarations
            public Game gameRef;
            public GraphicsDevice device;
            public SpriteBatch spriteBatch;
            public SpriteFont currentFont;
            public MgTimer timer = new MgTimer(1f);

            public Texture2D dotTexture;

            public SpriteFont Font
            {
                set { currentFont = value; }
                private get { return currentFont; }
            }
            public bool ShowVisulizations
            {
                get { return visualizations; }
                set { visualizations = value; }
            }
            public bool DisplayFrameRate
            {
                get { return displayFrameRate; }
                set { displayFrameRate = value; }
            }
            public bool UpdateDisplayOnGcChange
            {
                get { return updateDisplayOnGcChange; }
                set { updateDisplayOnGcChange = value; }
            }
            public float UpdatesBetweenMsgs
            {
                get { return (float)(updatesBetweenMsgs); }
                set { updatesBetweenMsgs = value; }
            }
            public double DesiredFramesPerSecond
            {
                get { return desiredFramesPerSecond; }
                set { desiredFramesPerSecond = value; }
            }
            public bool IsFixedTimeStepOn
            {
                get { return isFixedTimeStepOn; }
                set { if (gameRef != null) { isFixedTimeStepOn = value; gameRef.IsFixedTimeStep = value; } }
            }
            public double ElapsedTimeInSeconds
            {
                get { return elapsedFrameTimeMeasuredSeconds; }
                set { elapsedFrameTimeMeasuredSeconds = value; }
            }
            public Color TextColor
            {
                get { return textColor; }
                set { textColor = value; }
            }

            // Moccasin (255, 228, 190, 255) // cadet blue 95,160,160,255
            //
            public Color textColor = Color.Black;
            public Color backGroundColor = new Color(64, 64, 64, 100);
            public Color outlineColor = new Color(25, 25, 25, 128);

            private bool visualizations = true;
            private bool displayFrameRate = true;
            private bool updateDisplayOnGcChange = false;
            private double desiredFramesPerSecond = 60;
            private double updatesBetweenMsgs = 1.0f;
            private bool isFixedTimeStepOn = true;

            //private static StringBuilder sb_perf_msg = new StringBuilder(2048);
            private static MgStringBuilder mgsb_perf_msg = new MgStringBuilder(2048);
            private double timeRecordedLast = 0d;
            private double timeRecordedNow = 0d;
            private double elapsedFrameTimeMeasuredSeconds = 0d;
            private double timeToMsgUpdateCounter = 0f;
            private double targetFrameTimeSecs = .08d;
            private double updates = .01;
            private double frames = .01;
            private double fps = 1f;
            private double averageFps = 01d;
            private int samplesSize = 10;
            private double[] fpsSamples;
            private int fpsSamplePointer = 0;
            private long gcNow = 0;
            private long gcLast = 0;
            private long gcDiff = 0;
            private long gcAccumulatedInc = 0;
            private long gcTotalLost = 0;
            private long gcRecordedCollects = 0;
            //
            private MgStringBuilder sb_elapsed_time_msg = new StringBuilder("  Game Time (Seconds): ");
            private MgStringBuilder sb_time_measured_msg = new StringBuilder("\n  Time per Measure: ");
            private MgStringBuilder sb_updates_msg = new StringBuilder("\n  Updates Measured: ");
            private MgStringBuilder sb_draws_msg = new StringBuilder("\n  Draws Measured: ");
            private MgStringBuilder sb_drawupdateratio_msg = new StringBuilder("\n  Draw To Update Ratio: ");
            private MgStringBuilder sb_time_fps_msg = new StringBuilder("\n  Fps: ");
            private MgStringBuilder sb_time_avgfps_msg = new StringBuilder("\n  Sampled Fps: ");
            private MgStringBuilder sb_targettime_msg = new StringBuilder("\n  TargetTime (Seconds): ");
            private MgStringBuilder sb_nofixed_targettime_msg = new MgStringBuilder("\n  No fixed timing is set");
            private MgStringBuilder sb_gc_now_msg = new StringBuilder("\n  GC Memory(Kb) Now: ");
            private MgStringBuilder sb_gc_diff_msg = new StringBuilder("\n  GC Memory(Kb) Increased: ");
            private MgStringBuilder sb_gc_lost_msg = new StringBuilder("\n  GC Memory(Kb) Lost: ");
            private MgStringBuilder sb_gc_accumulated_inc_msg = new StringBuilder("\n  GC Memory(Kb) AccumulatedInc: ");
            private MgStringBuilder sb_gc_totallost_msg = new StringBuilder("\n  GC Memory(Kb) TotalLost: ");
            private MgStringBuilder sb_gc_recordedcollects_msg = new StringBuilder("\n  GC Memory collects: ");
            #endregion

            public MgFrameRate(Game passin_this, GraphicsDeviceManager gdm, SpriteBatch sb, SpriteFont sf, float desiredframerate, bool vysnc, bool fixedon, bool updateongcchange, float timeBetweenUpdatesInSeconds)
            {
                mgsb_perf_msg = new MgStringBuilder(2048);
                SetUpFrameRate(passin_this, gdm, sb, sf, desiredframerate, vysnc, fixedon, TextColor, updateongcchange, timeBetweenUpdatesInSeconds);
            }

            /// <summary>
            /// Its probably best to call in load;
            /// For the Game parameter pass in the this keyword
            /// Fixed timestep must be set to true to get the desired framerate
            /// Beware your graphics cards global settings. 
            /// They can override monogames settings and give strange results
            /// </summary>
            public void SetUpFrameRate(Game passin_this, GraphicsDeviceManager gdm, SpriteBatch sb, SpriteFont sf, float desiredframerate, bool vysnc, bool fixedon)
            {
                SetUpFrameRate(passin_this, gdm, sb, sf, desiredframerate, vysnc, fixedon, Color.MonoGameOrange, false, 1f);
            }
            public void SetUpFrameRate(Game passin_this, GraphicsDeviceManager gdm, SpriteBatch sb, SpriteFont sf, float desiredframerate, bool vysnc, bool fixedon, Color textcolor)
            {
                SetUpFrameRate(passin_this, gdm, sb, sf, desiredframerate, vysnc, fixedon, textcolor, false, 1f);
            }
            public void SetUpFrameRate(Game passin_this, GraphicsDeviceManager gdm, SpriteBatch sb, SpriteFont sf, float desiredframerate, bool vysnc, bool fixedon, Color textcolor, bool updatedisplayOnGcChange, float timeBetweenUpdateMsgsInSeconds)
            {
                // note you graphics card can override your settings
                gameRef = passin_this;
                graphics = gdm;
                device = passin_this.GraphicsDevice;
                spriteBatch = sb;
                currentFont = sf;
                targetFrameTimeSecs = 1.0d / desiredframerate;
                DesiredFramesPerSecond = desiredframerate;
                passin_this.TargetElapsedTime = TimeSpan.FromSeconds(1.0d / desiredframerate);
                passin_this.IsFixedTimeStep = fixedon;
                IsFixedTimeStepOn = fixedon;
                gdm.SynchronizeWithVerticalRetrace = vysnc;
                gdm.ApplyChanges();
                textColor = textcolor;
                UpdateDisplayOnGcChange = updatedisplayOnGcChange;
                updatesBetweenMsgs = DesiredFramesPerSecond * timeBetweenUpdateMsgsInSeconds;
                fpsSamples = new double[samplesSize];
                dotTexture = TextureDotCreate(gdm.GraphicsDevice);
            }

            /// <summary>
            /// Draws msgs.
            /// update needs to also be called to get a proper framerate display. 
            /// e.g. like you would in a actual game.
            /// </summary>
            public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
            {
                Draw(spriteBatch, gameTime, 10f, 16f, textColor);
            }
            public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float x, float y)
            {
                Draw(spriteBatch, gameTime, 10f, 16f, textColor);
            }
            public void Draw(SpriteBatch spriteBatch, GameTime gameTime, float x, float y, Color color)
            {
                if (DisplayFrameRate)
                {
                    //spriteBatch.DrawString(currentFont, mgsb_perf_msg.StringBuilder, new Vector2(x, y), textColor);
                    spriteBatch.DrawString(currentFont, mgsb_perf_msg, new Vector2(x, y), color);
                }
                if (ShowVisulizations) { DrawVisualizations(); }
                frames += 1;
            }

            public void DrawVisualizations()
            {
                // updates frames (gcnow / 1000d)
                var gcCoefficient = (150d / gcNow);
                var visualGcIncrease = (double)(gcAccumulatedInc * gcCoefficient);

                var visualSampledFps = averageFps;
                if (visualSampledFps > 120) { visualSampledFps = 120d + (averageFps / 20d); }

                //var visualDraws = frames * (1d / elapsedFrameTimeMeasuredSeconds);
                var visualDraws = frames / elapsedFrameTimeMeasuredSeconds;
                if (visualDraws > 120) { visualDraws = 120d + (visualDraws / 20d); }

                var visualUpdates = updates / elapsedFrameTimeMeasuredSeconds;
                if (visualUpdates > 120) { visualUpdates = 120d + (visualUpdates / 20d); }

                Rectangle visualGcRect = new Rectangle(10, 2, 150, 4);
                DrawSquare(visualGcRect, 1, Color.BlueViolet);

                Rectangle visualGcIncreaseRect = new Rectangle(10, 3, (int)(visualGcIncrease), 2);
                DrawSquare(visualGcIncreaseRect, 1, Color.Red);

                Rectangle visualSampledFpsRect = new Rectangle(10, 8, (int)(visualSampledFps), 6);
                DrawSquare(visualSampledFpsRect, 3, Color.DarkGray);

                Rectangle visualUpdatesRect = new Rectangle(10, 8, (int)(visualUpdates), 1);
                DrawSquare(visualUpdatesRect, 2, Color.Green);

                Rectangle visualDrawsRect = new Rectangle(10, 12, (int)(visualDraws), 1);
                DrawSquare(visualDrawsRect, 2, Color.Aqua);
            }

            /// <summary>
            /// Updates msgs.
            /// draw should be called to display the framerate
            /// </summary>
            public void Update(GameTime gameTime)
            {
                gcNow = GC.GetTotalMemory(false);
                gcDiff = gcNow - gcLast;
                timer.Update(gameTime);
                if (DisplayFrameRate)
                {
                    if((UpdateDisplayOnGcChange && gcDiff != 0) || (updates >= UpdatesBetweenMsgs))
                    MyStringBuilderMsg(gameTime);
                }
                updates += 1;
            }

            private void MyStringBuilderMsg(GameTime gameTime)
            {
                timeRecordedLast = timeRecordedNow;
                timeRecordedNow = gameTime.TotalGameTime.TotalSeconds;
                elapsedFrameTimeMeasuredSeconds = (timeRecordedNow - timeRecordedLast);
                fps = frames / elapsedFrameTimeMeasuredSeconds;
                float ufRatio = (float)frames / (float)updates;
                //
                // take a sample for averaging
                fpsSamplePointer++;
                if (fpsSamplePointer >= samplesSize) { fpsSamplePointer = 0; }
                fpsSamples[fpsSamplePointer] = fps;
                averageFps = 0;
                for (int i = 0; i < samplesSize; i++) { averageFps += fpsSamples[i]; }
                averageFps /= samplesSize;
                //
                mgsb_perf_msg.Length = 0;
                mgsb_perf_msg.Append(sb_elapsed_time_msg).AppendTrim(timeRecordedNow);
                mgsb_perf_msg.Append(sb_time_measured_msg).AppendTrim(elapsedFrameTimeMeasuredSeconds);
                mgsb_perf_msg.Append(sb_updates_msg).Append((int)(updates));
                mgsb_perf_msg.Append(sb_draws_msg).Append((int)(frames));
                mgsb_perf_msg.Append(sb_drawupdateratio_msg).AppendTrim(ufRatio);
                mgsb_perf_msg.Append(sb_time_fps_msg).AppendTrim(fps);
                mgsb_perf_msg.Append(sb_time_avgfps_msg).AppendTrim(averageFps);
                if (IsFixedTimeStepOn)
                    mgsb_perf_msg.Append(sb_targettime_msg).AppendTrim(targetFrameTimeSecs);
                else
                    mgsb_perf_msg.Append(sb_nofixed_targettime_msg);

                // Gc Messages
                mgsb_perf_msg.Append(sb_gc_now_msg).AppendTrim(gcNow / 1024d);

                if (gcDiff < 0)
                {
                    mgsb_perf_msg.Append(sb_gc_lost_msg).AppendTrim((double)(gcDiff) / 1024d);
                    //
                    if (timeRecordedNow > 10)
                    {
                        gcTotalLost += gcDiff;
                        gcRecordedCollects++;
                        gcAccumulatedInc = 0;
                        gcDiff = 0;
                    }
                }
                else
                {
                    mgsb_perf_msg.Append(sb_gc_diff_msg).AppendTrim((double)(gcDiff) / 1024d);
                    //
                    if (timeRecordedNow > 10)
                    {
                        gcAccumulatedInc += gcDiff;
                    }
                }
                mgsb_perf_msg.Append(sb_gc_accumulated_inc_msg).AppendTrim(gcAccumulatedInc / 1024d);
                mgsb_perf_msg.Append(sb_gc_totallost_msg).AppendTrim(gcTotalLost / 1024d);
                mgsb_perf_msg.Append(sb_gc_recordedcollects_msg).AppendTrim(gcRecordedCollects);

                frames = 0f;
                updates = 0f;
                gcLast = gcNow;
            }

            public static Texture2D TextureDotCreate(GraphicsDevice device)
            {
                Color[] data = new Color[1];
                data[0] = new Color(255, 255, 255, 255);

                return TextureFromColorArray(device, data, 1, 1);
            }
            public static Texture2D TextureFromColorArray(GraphicsDevice device, Color[] data, int width, int height)
            {
                if (width > 2047 || height > 2047)
                {
                    Console.WriteLine(" big ass array to texture !");
                }
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
