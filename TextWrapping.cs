using System;
//using System.Text;
using System.Collections.Generic;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;


namespace Microsoft.Xna.Framework
{
    public static class TextWrapping
    {
        private static string newline = Environment.NewLine;
        private static MgStringBuilder text = new MgStringBuilder();
        private static Dictionary<int, SpriteFontItem> spriteFonts = new Dictionary<int, SpriteFontItem>();
        private static SpriteFontItem lastSpriteFontItem;
        private static int lastHashCode = -1;

        /// <summary>
        /// This method wraps and cuts off the text within the Rectangle boundry this is a real time function.
        /// It is not as efficient as precomputing a altered stringbuilder then just drawing it.
        /// Effectively this can be considered at a cost of two drawstrings
        /// </summary>
        public static void DrawWordWrapTextWithinBounds(MgStringBuilder stringText, SpriteBatch sb, SpriteFont sf, Vector2 scale, Rectangle boundRect, Color color)
        {
            var sfc = SpriteFontItem.PrepFont(sf);
            text.Clear();
            text.Append(stringText);
            var _glyphs = sfc._glyphs;
            var Spacing = sfc.Spacing;
            var lineHeight = sfc.LineSpacing * scale.Y;
            Vector2 offset = Vector2.Zero;
            float yextent = offset.Y + lineHeight;
            Vector2 redrawOffset = Vector2.Zero;
            Rectangle dest = new Rectangle();
            var currentGlyph = SpriteFont.Glyph.Empty;
            var firstGlyphOfLine = true;

            int lastWordBreakCharPos = 0;
            bool firstwordonline = true;
            Vector2 rewindOffset = Vector2.Zero;

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
                if (!sfc.tsf.DefaultCharacter.HasValue)
                    throw new ArgumentException("Text Contains a Unresolvable Character");
                else
                    currentGlyph = sfc.defaultGlyph;

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
                    rewindOffset = offset;
                    if (firstwordonline)
                        firstwordonline = false;
                }
                // word wrapping calculations.
                bool executeEndOffset = true;
                if (yextent > boundRect.Height)
                {
                    // this essentially is function termination due to height boundry
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
                                    // i also pulled the +1 here which is i think inserting a improperly positioned like break using append at.
                                    text.AppendAt(lastWordBreakCharPos + 1, '\n'); // !!!  insert here is a gcollection problem... fixed
                                    if (text[lastWordBreakCharPos + 1] != ' ')
                                    {
                                        offset = rewindOffset;
                                        i = lastWordBreakCharPos;
                                    }
                                }
                            }
                            else // first word on the line true
                            {
                                if (text[i + 1] != '\n') // if its not already a new line char
                                {
                                    text.AppendAt(i + 1, '\n');
                                }
                            }
                        }
                        else
                        {
                            // handles the case were we cannot check the next char for a newline because it doesn't exist.
                            text.AppendAt(lastWordBreakCharPos + 1, '\n');
                            if (text[lastWordBreakCharPos + 1] != ' ')
                            {
                                offset = rewindOffset;
                                i = lastWordBreakCharPos;
                                executeEndOffset = false;
                            }
                        }
                    }
                }
                if (executeEndOffset)
                    offset.X += currentGlyph.Width + currentGlyph.RightSideBearing;
            }
            sb.DrawString(sfc.tsf, text, boundRect.Location.ToVector2(), color);
        }

        /// <summary>
        /// This method wraps and cuts off the text within the Rectangle boundry.
        /// This method copys the textIn to Out and alters the textOut which can then be feed to spriteBatch.
        /// This isn't the greatest method for dynamic text but it will in general with prudence be a simple precomputation.
        /// </summary>
        public static void WordWrapTextWithinBounds(MgStringBuilder stringTextIn, MgStringBuilder stringTextOut, SpriteFont sf, Vector2 scale, Rectangle boundRect)
        {
            var sfc = SpriteFontItem.PrepFont(sf);
            stringTextOut.Clear();
            stringTextOut.Append(stringTextIn);
            var _glyphs = sfc._glyphs;
            var Spacing = sfc.Spacing;
            var lineHeight = sfc.LineSpacing * scale.Y;
            Vector2 offset = Vector2.Zero;
            float yextent = offset.Y + lineHeight;
            Vector2 redrawOffset = Vector2.Zero;
            Rectangle dest = new Rectangle();
            var currentGlyph = SpriteFont.Glyph.Empty;
            var firstGlyphOfLine = true;

            int lastWordBreakCharPos = 0;
            bool firstwordonline = true;
            Vector2 rewindOffset = Vector2.Zero;

            for (int i = 0; i < stringTextOut.Length; i++)
            {
                char c = stringTextOut[i];

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
                if (!sfc.tsf.DefaultCharacter.HasValue)
                    throw new ArgumentException("Text Contains a Unresolvable Character");
                else
                    currentGlyph = sfc.defaultGlyph;

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
                // if char is a word break character e.g. white space.
                // atm only spaces are legitiment line breaks
                if (c == ' ')
                {
                    lastWordBreakCharPos = i;
                    rewindOffset = offset;
                    if (firstwordonline)
                        firstwordonline = false;
                }
                // word wrapping calculations.
                bool executeEndOffset = true;
                if (yextent > boundRect.Height)
                {
                    // this essentially is function termination due to height boundry
                    stringTextOut.Length = i;
                    i = stringTextOut.Length;
                }
                else
                {
                    if (dest.Right > boundRect.Width)
                    {
                        // is there enough length for the next character is a new line check
                        if (stringTextOut.Length > (i + 1))
                        {
                            if (firstwordonline == false)
                            {
                                if (stringTextOut[lastWordBreakCharPos + 1] != '\n') // skips appending new line if the next line is a line break.
                                {
                                    stringTextOut.AppendAt(lastWordBreakCharPos + 1, '\n'); // !!!  insert here is a gcollection problem... fixed
                                    if (stringTextOut[lastWordBreakCharPos + 1] != ' ')
                                    {
                                        offset = rewindOffset;
                                        i = lastWordBreakCharPos;
                                        executeEndOffset = false;
                                    }
                                }
                            }
                            else // first word on the line true
                            {
                                if (stringTextOut[i + 1] != '\n') // if its not already a new line char
                                {
                                    stringTextOut.AppendAt(i + 1, '\n'); // !!!  insert here is a gcollection problem... fixed via mystringbuilder
                                }
                            }
                        }
                        else
                        {
                            stringTextOut.AppendAt(lastWordBreakCharPos + 1, '\n'); // !!!  insert here is a gcollection problem... fixed via mystringbuilder
                            if (stringTextOut[lastWordBreakCharPos + 1] != ' ')
                            {
                                offset = rewindOffset;
                                i = lastWordBreakCharPos;
                                executeEndOffset = false;
                            }
                        }
                    }
                }
                if (executeEndOffset)
                    offset.X = offset.X + currentGlyph.Width + currentGlyph.RightSideBearing;
            }
            // we could return the string we sent in but i don't think i need to.
        }

        private class SpriteFontItem
        {
            public SpriteFont tsf;
            public Dictionary<char, SpriteFont.Glyph> _glyphs;
            public SpriteFont.Glyph defaultGlyph;
            public char defaultfontchar = ' ';
            public float Spacing = 0f;
            public int LineSpacing = 10;
            public string newline = Environment.NewLine;


            public void SetSpriteFont(SpriteFont s)
            {
                tsf = s;
                _glyphs = tsf.GetGlyphs();
                defaultGlyph = new SpriteFont.Glyph();
                Spacing = tsf.Spacing;
                LineSpacing = tsf.LineSpacing;
                if (tsf.DefaultCharacter.HasValue)
                {
                    defaultfontchar = (char)(tsf.DefaultCharacter.Value);
                    defaultGlyph = _glyphs[defaultfontchar];
                }
            }

            public static SpriteFontItem PrepFont(SpriteFont sf)
            {
                var hcode = sf.GetHashCode();
                if (hcode != lastHashCode)
                {
                    if (spriteFonts.ContainsKey(hcode) == false)
                    {
                        SpriteFontItem nval = new SpriteFontItem();
                        nval.SetSpriteFont(sf);
                        spriteFonts.Add(hcode, nval);
                    }
                    lastSpriteFontItem = spriteFonts[hcode];
                }
                return lastSpriteFontItem;
            }
        }
    }
}
