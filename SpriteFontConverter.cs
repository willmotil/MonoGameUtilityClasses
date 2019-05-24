using System;
using System.IO;
//using System.Text;
using System.Collections.Generic;
//using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{

    public class SpriteFontConverter
    {
        /// <summary>
        /// This holds the deconstructed sprite font data including a pixel color array.
        /// </summary>
        public SpriteFontData spriteFontData;
        private SpriteFont spriteFont;
        private SpriteFontToClassFileEncoderDecoder spriteFontToClassWriter = new SpriteFontToClassFileEncoderDecoder();

        /// <summary>
        /// Give this a sprite font to deconstruct it to data.
        /// </summary>
        public SpriteFontConverter(SpriteFont spriteFont)
        {
            this.spriteFont = spriteFont; // how to handle disposing the fonts texture in this particular case im not sure.
            spriteFontData = new SpriteFontData();
            DeConstruct(spriteFontData);
        }

        /// <summary>
        /// Call this to write the font the file will be saved to the full path file
        /// This writes the file as a cs text file that can be loaded by another project as a actual c sharp class file into visual studio.
        /// When that generated class is instantiated and load is called on it, it then returns a hardcoded spritefont.
        /// </summary>
        public void WriteFileAsCs(string fullFilePath)
        {
            spriteFontToClassWriter.WriteToFileAsCs(fullFilePath, spriteFontData);
        }

        // Deconstructs a spritefont.
        private void DeConstruct(SpriteFontData spriteFontData)
        {
            spriteFontData.fontTexture = spriteFont.Texture;
            spriteFontData.lineHeightSpaceing = spriteFont.LineSpacing;
            spriteFontData.spaceing = spriteFont.Spacing;
            spriteFontData.dgyphs = spriteFont.GetGlyphs();
            spriteFontData.defaultglyph = new SpriteFont.Glyph();
            if (spriteFont.DefaultCharacter.HasValue)
            {
                spriteFontData.defaultchar = (char)(spriteFont.DefaultCharacter.Value);
                spriteFontData.defaultglyph = spriteFontData.dgyphs[spriteFontData.defaultchar];
            }
            else
            {
                // we could create a default value from like a pixel in the sprite font and add the glyph.
            }
            foreach (var item in spriteFontData.dgyphs)
            {
                spriteFontData.glyphBounds.Add(item.Value.BoundsInTexture);
                spriteFontData.glyphCroppings.Add(item.Value.Cropping);
                spriteFontData.glyphCharacters.Add(item.Value.Character);
                spriteFontData.glyphKernings.Add(new Vector3(item.Value.LeftSideBearing, item.Value.Width, item.Value.RightSideBearing));
            }
            spriteFontData.numberOfGlyphs = spriteFontData.glyphCharacters.Count;

            spriteFontData.width = spriteFont.Texture.Width;
            spriteFontData.height = spriteFont.Texture.Height;

            Color[] colorarray = new Color[spriteFont.Texture.Width * spriteFont.Texture.Height];
            spriteFont.Texture.GetData<Color>(colorarray); //,0, width* height
            List<Color> pixels = new List<Color>();
            foreach (var c in colorarray)
                pixels.Add(c);
            spriteFontData.pixelColorData = pixels;
        }

        /// <summary>
        /// Holds the sprite fonts data including the pixel data.
        /// </summary>
        public class SpriteFontData
        {
            public Texture2D fontTexture;
            public List<Color> pixelColorData;
            public int width;
            public int height;

            public int numberOfGlyphs = 0;
            public Dictionary<char, SpriteFont.Glyph> dgyphs;
            public SpriteFont.Glyph defaultglyph;
            public float spaceing = 0;
            public int lineHeightSpaceing = 0;
            public char defaultchar = '*';
            public List<Rectangle> glyphBounds = new List<Rectangle>();
            public List<Rectangle> glyphCroppings = new List<Rectangle>();
            public List<char> glyphCharacters = new List<char>();
            public List<Vector3> glyphKernings = new List<Vector3>(); // left width right and side bering
        }

        /// <summary>
        /// Encoding and Decoding methods however the decoder is placed within the output class file.
        /// This allows the output class to be decoupled from the pipeline or even a ttf.
        /// This class doesn't have to be called unless you wish to do editing on a spriteFontData Instance.
        /// Then rewrite the encoded data to file later at a time of your own choosing
        /// </summary>
        public class SpriteFontToClassFileEncoderDecoder
        {
            MgStringBuilder theCsText = new MgStringBuilder();
            public List<byte> rleEncodedByteData = new List<byte>();
            public int width = 0;
            public int height = 0;

            // this is just a spacing thing to show the data a bit easier on the eyes.
            int dividerAmount = 10;
            int dividerCharSingleValueAmount = 50;
            int dividerSingleValueAmount = 200;
            private int pixels = 0;
            private int bytestallyed = 0;
            private int bytedataCount = 0;

            public void WriteToFileAsCs(string fullFilePath, SpriteFontData sfData)
            {
                var filename = Path.GetFileNameWithoutExtension(fullFilePath);
                var colordata = sfData.pixelColorData; 
                width = sfData.width;
                height = sfData.height;
                rleEncodedByteData = EncodeColorArrayToDataRLE(colordata, sfData.width, sfData.height);

                var charsAsString = CharArrayToStringClassFormat("chars", sfData.glyphCharacters.ToArray());
                var boundsAsString = RectangleToStringClassFormat("bounds", sfData.glyphBounds.ToArray());
                var croppingsAsString = RectangleToStringClassFormat("croppings", sfData.glyphCroppings.ToArray());
                var glyphKernings = Vector3ToStringClassFormat("kernings", sfData.glyphKernings.ToArray());
                var rleDataAsString = ByteArrayToStringClassFormat("rleByteData", rleEncodedByteData.ToArray());


                theCsText =
                    "\n//" +
                    "\n// This file is programatically generated this class is hard coded instance data for a instance of a spritefont." +
                    "\n// Use the LoadHardCodeSpriteFont to load it." +
                    "\n// Be aware i believe you should dispose its texture in game1 unload as this won't have been loaded thru the content manager." +
                    "\n//" +
                    "\n//";

                theCsText
                    .Append("\n using System;")
                    .Append("\n using System.Text;")
                    .Append("\n using System.Collections.Generic;")
                    .Append("\n using Microsoft.Xna.Framework;")
                    .Append("\n using Microsoft.Xna.Framework.Graphics;")
                    .Append("\n using Microsoft.Xna.Framework.Input;")
                    .Append("\n")
                    .Append("\n namespace Microsoft.Xna.Framework")
                    .Append("\n {")
                    .Append("\n ")
                    .Append("\n  public class SpriteFontAsClassFile_").Append(filename)
                    .Append("\n  {")
                    .Append("\n ")
                    .Append("\n  int width=").Append(width).Append(";")
                    .Append("\n  int height=").Append(height).Append(";")
                    .Append("\n  char defaultChar =  Char.Parse(\"").Append(sfData.defaultchar).Append("\");")
                    .Append("\n  int lineHeightSpaceing =").Append(sfData.lineHeightSpaceing).Append(";")
                    .Append("\n  float spaceing =").Append(sfData.spaceing).Append(";")
                    .Append("\n ")
                    .Append("\n   public SpriteFont LoadHardCodeSpriteFont(GraphicsDevice device)")
                    .Append("\n   {")
                    .Append("\n       Texture2D t = DecodeToTexture(device, rleByteData, width, height);")
                    .Append("\n       return new SpriteFont(t, bounds, croppings, chars, lineHeightSpaceing, spaceing, kernings, defaultChar);")
                    .Append("\n   }")
                    .Append("\n ")
                    .Append("\n   private Texture2D DecodeToTexture(GraphicsDevice device, List<byte> rleByteData, int _width, int _height)")
                    .Append("\n   {")
                    .Append("\n       Color[] colData = DecodeDataRLE(rleByteData);")
                    .Append("\n       Texture2D tex = new Texture2D(device, _width, _height);")
                    .Append("\n       tex.SetData<Color>(colData);")
                    .Append("\n       return tex;")
                    .Append("\n   }")
                    .Append("\n ")
                    .Append("\n   private Color[] DecodeDataRLE(List<byte> rleByteData)")
                    .Append("\n   {")
                    .Append("\n       List <Color> colAry = new List<Color>();")
                    .Append("\n       for (int i = 0; i < rleByteData.Count; i++)")
                    .Append("\n       {")
                    .Append("\n           var val = (rleByteData[i] & 0x7F) * 2;")
                    .Append("\n           if (val > 252)")
                    .Append("\n               val = 255;")
                    .Append("\n           Color color = new Color();")
                    .Append("\n           if (val > 0)")
                    .Append("\n               color = new Color(val, val, val, val);")
                    .Append("\n           if ((rleByteData[i] & 0x80) > 0)")
                    .Append("\n           {")
                    .Append("\n               var runlen = rleByteData[i + 1];")
                    .Append("\n               for (int j = 0; j < runlen; j++)")
                    .Append("\n                   colAry.Add(color);")
                    .Append("\n               i += 1;")
                    .Append("\n           }")
                    .Append("\n           colAry.Add(color);")
                    .Append("\n       }")
                    .Append("\n       return colAry.ToArray();")
                    .Append("\n   }")
                    .Append("\n ")
                    .Append("\n     ").Append(charsAsString)
                    .Append("\n     ").Append(boundsAsString)
                    .Append("\n     ").Append(croppingsAsString)
                    .Append("\n     ").Append(glyphKernings)
                    .Append("\n ")
                    .Append("\n       // pixelsCompressed: ").Append(pixels).Append(" bytesTallied: ").Append(bytestallyed).Append(" byteDataCount: ").Append(bytedataCount)
                    .Append("\n     ").Append(rleDataAsString)
                    .Append("\n ")
                    .Append("\n ")
                    .Append("\n  }")
                    .Append("\n ")
                    .Append("\n }") // end of namespace
                    .Append("\n ").Append(" ")
                    ;

                //MgPathFolderFileOps.WriteStringToFile(fullFilePath, theCsText.ToString());
                File.WriteAllText(fullFilePath, theCsText.ToString());
            }

            public string CharArrayToStringClassFormat(string variableName, char[] c)
            {
                string s =
                    "\n        // Item count = " + c.Length +
                    "\n        List <char> " + variableName + " = new List<char> " +
                    "\n        {" +
                    "\n        "
                    ;
                int divider = 0;
                for (int i = 0; i < c.Length; i++)
                {
                    divider++;
                    if (divider > dividerCharSingleValueAmount)
                    {
                        divider = 0;
                        s += "\n        ";
                    }
                    //s += $"new char(\"{c[i]})\"";
                    s += "(char)" + (int)c[i] + "";
                    if (i < c.Length - 1)
                        s += ",";
                }
                s += "\n        };";
                return s;
            }
            public string RectangleToStringClassFormat(string variableName, Rectangle[] r)
            {
                string s =
                    "\n        List < Rectangle > " + variableName + " = new List<Rectangle>" +
                    "\n        {" +
                    "\n        "
                    ;
                int divider = 0;
                for (int i = 0; i < r.Length; i++)
                {
                    divider++;
                    if (divider > dividerAmount)
                    {
                        divider = 0;
                        s += "\n        ";
                    }
                    s += $"new Rectangle({r[i].X},{r[i].Y},{r[i].Width},{r[i].Height})";
                    if (i < r.Length - 1)
                        s += ",";
                }
                s += "\n        };";
                return s;
            }
            public string Vector3ToStringClassFormat(string variableName, Vector3[] v)
            {
                string s =
                    "\n        List<Vector3> " + variableName + " = new List<Vector3>" +
                    "\n        {" +
                    "\n        "
                    ;
                int divider = 0;
                for (int i = 0; i < v.Length; i++)
                {
                    divider++;
                    if (divider > dividerAmount)
                    {
                        divider = 0;
                        s += "\n        ";
                    }
                    s += $"new Vector3({v[i].X},{v[i].Y},{v[i].Z})";
                    if (i < v.Length - 1)
                        s += ",";
                }
                s += "\n        };";
                return s;
            }
            public string ByteArrayToStringClassFormat(string variableName, byte[] b)
            {
                string s =
                    "\n        List<byte> " + variableName + " = new List<byte>" +
                    "\n        {" +
                    "\n        "
                    ;
                int divider = 0;
                for (int i = 0; i < b.Length; i++)
                {
                    divider++;
                    if (divider > dividerSingleValueAmount)
                    {
                        divider = 0;
                        s += "\n        ";
                    }
                    s += b[i];
                    if (i < b.Length - 1)
                        s += ",";
                }
                s += "\n        };";
                return s;
            }

            /// <summary>
            /// Turns the pixel data into run length encode text for a cs file.
            /// Technically this is only compressing the alpha byte of a array.
            /// I could pull out my old bit ziping algorithm but that is probably overkill.
            /// </summary>
            public List<byte> EncodeColorArrayToDataRLE(List<Color> colorArray, int pkdcolaryWidth, int pkdcolaryHeight)
            {
                List<byte> rleAry = new List<byte>();
                int colaryIndex = 0;
                int colaryLen = colorArray.Count;
                int templen = pkdcolaryWidth * pkdcolaryHeight;

                int pixelsAccountedFor = 0;

                while (colaryIndex < colaryLen)
                {
                    var colorMasked = (colorArray[colaryIndex].A / 2);  //(pkdcolAry[colaryIndex].A & 0xFE);
                    var encodedValue = colorMasked;
                    var runLength = 0;

                    // find the run length for this pixel.
                    for (int i = 1; i < 255; i++)
                    {
                        var indiceToTest = colaryIndex + i;
                        if (indiceToTest < colaryLen)
                        {
                            var testColorMasked = (colorArray[indiceToTest].A / 2); //(pkdcolAry[indiceToTest].A & 0xFE);
                            if (testColorMasked == colorMasked)
                                runLength = i;
                            else
                                i = 256; // break on diff
                        }
                        else
                            i = 256; // break on maximum run length
                    }
                    Console.WriteLine("colaryIndex: " + colaryIndex + "  rleAry index " + rleAry.Count + "  Alpha: " + colorMasked * 2 + "  runLength: " + runLength);

                    if (runLength > 0)
                    {
                        encodedValue += 0x80;
                        if (colaryIndex < colaryLen)
                        {
                            rleAry.Add((byte)encodedValue);
                            rleAry.Add((byte)runLength);
                            pixelsAccountedFor += 1 + runLength;
                        }
                        else
                            throw new Exception("bug check index write out of bounds");
                        colaryIndex = colaryIndex + 1 + runLength;
                    }
                    else
                    {
                        if (colaryIndex < colaryLen)
                        {
                            rleAry.Add((byte)encodedValue);
                            pixelsAccountedFor += 1;
                        }
                        else
                            throw new Exception("Encoding bug check index write out of bounds");
                        colaryIndex = colaryIndex + 1;
                    }
                }
                //
                pixels = pixelsAccountedFor;
                bytestallyed = pixelsAccountedFor * 4;
                bytedataCount = rleAry.Count;
                Console.WriteLine("EncodeColorArrayToDataRLE: rleAry.Count " + rleAry.Count + " pixels accounted for " + pixelsAccountedFor + " bytes tallied " + pixelsAccountedFor * 4);
                return rleAry;
            }

            /// <summary>
            /// This decodes the cs files hard coded rle pixel data to a texture.
            /// </summary>
            public Texture2D DecodeRleDataToTexture(GraphicsDevice device, List<byte> rleByteData, int _width, int _height)
            {
                Color[] colData = DecodeRleDataToPixelData(rleByteData);
                Texture2D tex = new Texture2D(device, _width, _height);
                tex.SetData<Color>(colData);
                return tex;
            }
            /// <summary>
            /// Decodes the class file hardcoded rle byte data to a color array.
            /// </summary>
            private Color[] DecodeRleDataToPixelData(List<byte> rleByteData)
            {
                List<Color> colAry = new List<Color>();
                for (int i = 0; i < rleByteData.Count; i++)
                {
                    var val = (rleByteData[i] & 0x7F) * 2;
                    if (val > 252)
                        val = 255;
                    Color color = new Color();
                    if (val > 0)
                        color = new Color(val, val, val, val);
                    if ((rleByteData[i] & 0x80) > 0)
                    {
                        var runlen = rleByteData[i + 1];
                        for (int j = 0; j < runlen; j++)
                            colAry.Add(color);
                        i += 1;
                    }
                    colAry.Add(color);
                }
                return colAry.ToArray();
            }
        }
    }
}
