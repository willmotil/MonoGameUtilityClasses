using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework
{

    /// <summary>
    /// Wraps a stringbuilder to a bounding box.
    /// This class of course works for a no garbage MgStringBuilder as well.
    /// </summary>
    public static class MgTextBounder
    {
        private static SpriteFont tsf;
        private static Dictionary<char, SpriteFont.Glyph> _glyphs;
        private static SpriteFont.Glyph defaultGlyph;
        private static char defaultfontchar = ' ';
        private static string newline = Environment.NewLine;

        /// <summary>
        /// Set the spritefont this needs to be done before the class is used.
        /// </summary>
        public static void SetSpriteFont(SpriteFont s)
        {
            tsf = s;
            _glyphs = tsf.GetGlyphs();
            defaultGlyph = new SpriteFont.Glyph();
            if (tsf.DefaultCharacter.HasValue)
            {
                defaultfontchar = (char)(tsf.DefaultCharacter.Value);
                defaultGlyph = _glyphs[defaultfontchar];
            }
        }

        /// <summary>
        /// Alter a character in this spritefont.
        /// requires ... using System.Collections.Generic;
        /// </summary>
        public static SpriteFont AlterSpriteFont(SpriteFont sf, char chartoalter, float width_amount_to_add)
        {
            Dictionary<char, SpriteFont.Glyph> dgyphs;
            SpriteFont.Glyph defaultglyph;
            char defaultchar = ' ';
            // Alter one of my methods a bit here for this purpose.
            // just drop all the alterd values into a new spritefont
            dgyphs = sf.GetGlyphs();
            defaultglyph = new SpriteFont.Glyph();
            if (sf.DefaultCharacter.HasValue)
            {
                defaultchar = (char)(sf.DefaultCharacter.Value);
                defaultglyph = dgyphs[defaultchar];
            }
            else
            {
                // we could create a default value from like a pixel in the sprite font and add the glyph.
            }
            var altered = dgyphs[chartoalter];
            altered.Width = altered.Width + width_amount_to_add;  // ect 
            dgyphs.Remove(chartoalter);
            dgyphs.Add(chartoalter, altered);

            //sf.Glyphs = _glyphs;  // cant do it as its readonly private that sucks hard we would of been done

            List<Rectangle> glyphBounds = new List<Rectangle>();
            List<Rectangle> cropping = new List<Rectangle>();
            List<char> characters = new List<char>();
            List<Vector3> kerning = new List<Vector3>();
            foreach (var item in dgyphs)
            {
                glyphBounds.Add(item.Value.BoundsInTexture);
                cropping.Add(item.Value.Cropping);
                characters.Add(item.Value.Character);
                kerning.Add(new Vector3(item.Value.LeftSideBearing, item.Value.Width, item.Value.RightSideBearing));
            }
            List<Rectangle> b = new List<Rectangle>();
            sf = new SpriteFont(sf.Texture, glyphBounds, cropping, characters, sf.LineSpacing, sf.Spacing, kerning, defaultchar);
            return sf;
        }

        /// <summary>
        /// Set the spritefont this needs to be done before the class is used.
        /// </summary>
        public static SpriteFont AlterSpriteFont(SpriteFont sf , char chartoalter, int widthchange)
        {
            //SpriteFont sf = s;
            _glyphs = sf.GetGlyphs();
            defaultGlyph = new SpriteFont.Glyph();
            if (sf.DefaultCharacter.HasValue)
            {
                defaultfontchar = (char)(sf.DefaultCharacter.Value);
                defaultGlyph = _glyphs[defaultfontchar];
            }
            var altered = _glyphs[chartoalter];
            altered.Width = widthchange;  // ect 
            _glyphs.Remove(' ');
            _glyphs.Add(' ', altered);

            // instead of holding the glyph as i was doing though just drop all the alterd values into a new spritefont

            return sf;
        }


        /// <summary>
        /// This changes the ref stringbuilder by word wrapping it to a bounding box.
        /// </summary>
        public static void WordWrapTextWithinBounds(ref StringBuilder text, Vector2 scale, Rectangle boundRect)
        {
            var Spacing = tsf.Spacing;
            var lineHeight = tsf.LineSpacing * scale.Y;
            Vector2 offset = Vector2.Zero;
            float yextent = offset.Y + lineHeight;
            Vector2 redrawOffset = Vector2.Zero;
            Rectangle dest = new Rectangle();
            var currentGlyph = SpriteFont.Glyph.Empty;
            var firstGlyphOfLine = true;

            int lastWordBreakCharPos = 0;
            bool firstwordonline = true;
            Vector2 rewind = Vector2.Zero;

            //int i = -1;

            //Console.WriteLine(" text.StringBuilder.Length " + text.StringBuilder.Length.ToString() + "  ");

            //while (i < text.Length)
            //{ i++;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                //Console.Write(" text[" + i + "]");
                //Console.Write(" = " + c + " ");

                if (c == '\r')
                    continue;
                if (c == '\n')
                {
                    offset.X = 0;
                    offset.Y += lineHeight;
                    yextent = offset.Y + lineHeight;
                    //Console.Write(" >> [" + i.ToString() + "]  newline is set.  lineHeight:" + lineHeight.ToString() + "   y offset:" + offset.Y.ToString() + " the y extent is " + yextent.ToString());
                    firstGlyphOfLine = true;
                    firstwordonline = true;
                    continue;
                }

                if (_glyphs.ContainsKey(c))
                    currentGlyph = _glyphs[c];
                else
                if (!tsf.DefaultCharacter.HasValue)
                    throw new ArgumentException("Text Contains a Unresolvable Character");
                else
                    currentGlyph = defaultGlyph;

                // Solves the problem- the first character on a line with a negative left side bearing.
                if (firstGlyphOfLine)
                {
                    offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                }
                else
                    offset.X += Spacing + currentGlyph.LeftSideBearing;

                // matrix calculations unrolled varys max 8 mults and up to 10 add subs.
                var m = offset;
                m.X += currentGlyph.Cropping.X;
                m.Y += currentGlyph.Cropping.Y;

                dest = new Rectangle(
                    (int)(m.X * scale.X),
                    (int)(m.Y * scale.Y),
                    (int)(currentGlyph.BoundsInTexture.Width * scale.X),
                    (int)(currentGlyph.BoundsInTexture.Height * scale.Y)
                    );

                //Console.WriteLine("   >>> dest.Height " + dest.Height + "  , dest.Bottom " + dest.Bottom);

                // if char == a word break character white space
                if (c == ' ')
                {
                    lastWordBreakCharPos = i;
                    rewind = offset;
                    if (firstwordonline)
                        firstwordonline = false;
                }

                // Begin word wrapping calculations.
                if (yextent >= boundRect.Height)
                {
                    //Console.WriteLine(" >>  dest.Bottom " + dest.Bottom + " >= boundRect.Height " + boundRect.Height);
                    //Console.WriteLine(" >>  text.Length = i + 1; i = text.Length;");
                    text.Length = i;
                    i = text.Length;
                }
                else
                {
                    if (dest.Right > boundRect.Width)
                    {
                        if (text.Length > (i + 1))
                        {
                            if (firstwordonline == false)
                            {
                                if (text[lastWordBreakCharPos + 1] != '\n')
                                {
                                    //Console.WriteLine(" >>  (" + dest.Right + " > " + boundRect.Width + "),   text.Insert(lastWordBreakCharPos " + lastWordBreakCharPos + " + 1, newline); offset = rewind; i = lastWordBreakCharPos; ");
                                    text.Insert(lastWordBreakCharPos + 1, '\n');
                                    if (text[lastWordBreakCharPos + 1] != ' ')
                                    {
                                        offset = rewind;
                                        i = lastWordBreakCharPos;
                                    }
                                }
                            }
                            else // first word on the line true
                            {
                                if (text[i + 1] != '\n')
                                {
                                    //Console.WriteLine(" >>  text.Insert(i + 1, newline);'");
                                    text.Insert(i + 1, '\n');
                                }
                            }
                        }
                    }
                }
                offset.X += currentGlyph.Width + currentGlyph.RightSideBearing;
            }
            //return lastLineBreakCharPos;
        }

        /// <summary>
        /// This vesion copys to a new stringbuilder and then wraps it before returning it.
        /// </summary>
        public static StringBuilder WordWrapTextWithinBounds(StringBuilder text, Vector2 scale, Rectangle boundRect)
        {
            var Spacing = tsf.Spacing;
            var lineHeight = tsf.LineSpacing * scale.Y;
            Vector2 offset = Vector2.Zero;
            float yextent = offset.Y + lineHeight;
            Vector2 redrawOffset = Vector2.Zero;
            Rectangle dest = new Rectangle();
            var currentGlyph = SpriteFont.Glyph.Empty;
            var firstGlyphOfLine = true;

            int lastWordBreakCharPos = 0;
            bool firstwordonline = true;
            Vector2 rewind = Vector2.Zero;

            //Console.WriteLine(" text.StringBuilder.Length " + text.StringBuilder.Length.ToString() + "  ");
            StringBuilder tmp = new StringBuilder();
            tmp.Append(text);

            for (int i = 0; i < tmp.Length; i++)
            {
                char c = tmp[i];
                //Console.Write(" text[" + i + "]");
                //Console.Write(" = " + c + " ");

                if (c == '\r')
                    continue;
                if (c == '\n')
                {
                    offset.X = 0;
                    offset.Y += lineHeight;
                    yextent = offset.Y + lineHeight;
                    //Console.Write(" >> [" + i.ToString() + "]  newline is set.  lineHeight:" + lineHeight.ToString() + "   y offset:" + offset.Y.ToString() + " the y extent is " + yextent.ToString());
                    firstGlyphOfLine = true;
                    firstwordonline = true;
                    continue;
                }

                if (_glyphs.ContainsKey(c))
                    currentGlyph = _glyphs[c];
                else
                if (!tsf.DefaultCharacter.HasValue)
                    throw new ArgumentException("Text Contains a Unresolvable Character");
                else
                    currentGlyph = defaultGlyph;

                // Solves the problem- the first character on a line with a negative left side bearing.
                if (firstGlyphOfLine)
                {
                    offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                }
                else
                    offset.X += Spacing + currentGlyph.LeftSideBearing;

                // matrix calculations unrolled varys max 8 mults and up to 10 add subs.
                var m = offset;
                m.X += currentGlyph.Cropping.X;
                m.Y += currentGlyph.Cropping.Y;

                dest = new Rectangle(
                    (int)(m.X * scale.X),
                    (int)(m.Y * scale.Y),
                    (int)(currentGlyph.BoundsInTexture.Width * scale.X),
                    (int)(currentGlyph.BoundsInTexture.Height * scale.Y)
                    );

                //Console.WriteLine("   >>> dest.Height " + dest.Height + "  , dest.Bottom " + dest.Bottom);

                // if char == a word break character white space
                if (c == ' ')
                {
                    lastWordBreakCharPos = i;
                    rewind = offset;
                    if (firstwordonline)
                        firstwordonline = false;
                }

                // Begin word wrapping calculations.
                if (yextent >= boundRect.Height)
                {
                    //Console.WriteLine(" >>  dest.Bottom " + dest.Bottom + " >= boundRect.Height " + boundRect.Height);
                    //Console.WriteLine(" >>  text.Length = i + 1; i = text.Length;");
                    tmp.Length = i;
                    i = tmp.Length;
                }
                else
                {
                    if (dest.Right > boundRect.Width)
                    {
                        if (tmp.Length > (i + 1))
                        {
                            if (firstwordonline == false)
                            {
                                if (tmp[lastWordBreakCharPos + 1] != '\n')
                                {
                                    //Console.WriteLine(" >>  (" + dest.Right + " > " + boundRect.Width + "),   text.Insert(lastWordBreakCharPos " + lastWordBreakCharPos + " + 1, newline); offset = rewind; i = lastWordBreakCharPos; ");
                                    tmp.Insert(lastWordBreakCharPos + 1, '\n');
                                    if (tmp[lastWordBreakCharPos + 1] != ' ')
                                    {
                                        offset = rewind;
                                        i = lastWordBreakCharPos;
                                    }
                                }
                            }
                            else // first word on the line true
                            {
                                if (tmp[i + 1] != '\n')
                                {
                                    //Console.WriteLine(" >>  text.Insert(i + 1, newline);'");
                                    tmp.Insert(i + 1, '\n');
                                }
                            }
                        }
                    }
                }
                offset.X += currentGlyph.Width + currentGlyph.RightSideBearing;
            }
            return tmp;
        }

        public static void WordWrapTextWithinBounds(ref MgStringBuilder text, Vector2 scale, Rectangle boundRect)
        {
            var Spacing = tsf.Spacing;
            var lineHeight = tsf.LineSpacing * scale.Y;
            Vector2 offset = Vector2.Zero;
            float yextent = offset.Y + lineHeight;
            Vector2 redrawOffset = Vector2.Zero;
            Rectangle dest = new Rectangle();
            var currentGlyph = SpriteFont.Glyph.Empty;
            var firstGlyphOfLine = true;

            int lastWordBreakCharPos = 0;
            bool firstwordonline = true;
            Vector2 rewind = Vector2.Zero;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == '\r')
                    continue;
                if (c == '\n')
                {
                    offset.X = 0;
                    offset.Y += lineHeight;
                    yextent = offset.Y + lineHeight;
                    firstGlyphOfLine = true;
                    firstwordonline = true;
                    continue;
                }

                if (_glyphs.ContainsKey(c))
                    currentGlyph = _glyphs[c];
                else
                if (!tsf.DefaultCharacter.HasValue)
                    throw new ArgumentException("Text Contains a Unresolvable Character");
                else
                    currentGlyph = defaultGlyph;

                // Solves the problem- the first character on a line with a negative left side bearing.
                if (firstGlyphOfLine)
                {
                    offset.X = Math.Max(currentGlyph.LeftSideBearing, 0);
                    firstGlyphOfLine = false;
                }
                else
                    offset.X += Spacing + currentGlyph.LeftSideBearing;

                // matrix calculations unrolled rotation is excluded for this version
                var m = offset;
                m.X += currentGlyph.Cropping.X;
                m.Y += currentGlyph.Cropping.Y;

                dest = new Rectangle(
                    (int)(m.X * scale.X),
                    (int)(m.Y * scale.Y),
                    (int)(currentGlyph.BoundsInTexture.Width * scale.X),
                    (int)(currentGlyph.BoundsInTexture.Height * scale.Y)
                    );

                // Begin word wrapping operations.
                // if char is a word break character e.g. white space
                if (c == ' ')
                {
                    lastWordBreakCharPos = i;
                    rewind = offset;
                    if (firstwordonline)
                        firstwordonline = false;
                }
                // word wrapping calculations.
                if (yextent >= boundRect.Height)
                {
                    text.Length = i;
                    i = text.Length;
                }
                else
                {
                    if (dest.Right > boundRect.Width)
                    {
                        if (text.Length > (i + 1))
                        {
                            if (firstwordonline == false)
                            {
                                if (text[lastWordBreakCharPos + 1] != '\n')
                                {
                                    text.AppendAt(lastWordBreakCharPos + 1, '\n'); // !!!  insert here is a gcollection problem... fixed
                                    if (text[lastWordBreakCharPos + 1] != ' ')
                                    {
                                        offset = rewind;
                                        i = lastWordBreakCharPos;
                                    }
                                }
                            }
                            else // first word on the line true
                            {
                                if (text[i + 1] != '\n') // if its not already a new line char
                                {
                                    text.AppendAt(i + 1, '\n'); // !!!  insert here is a gcollection problem... fixed
                                }
                            }
                        }
                    }
                }
                offset.X += currentGlyph.Width + currentGlyph.RightSideBearing;
            }
        }
    
    }
}
