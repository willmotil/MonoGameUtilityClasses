using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.Xna.Framework//SpriteFontEditToClassSomethingOrOther
{

    //// the file is used in game 1 as a method like so it will build a class that will list the content
    ///// <summary>
    ///// Its new i just thought it up it needs a lot of work.
    ///// I should just tree loop thru the entire content folder and sub folders.
    ///// So it can create a class that loads up everything.
    ///// </summary>
    //public void CreateOpenAssetClassTxtFile()
    //{
    //    // This is just for a new project its pretty cool

    //    MakeMeAnAssetClassFile.Prefix = "Textures_";
    //    MakeMeAnAssetClassFile.FolderToAddToTheAssets(Content);

    //    //MakeMeAnAssetClassFile.Prefix = "UiTextures_";
    //    //MakeMeAnAssetClassFile.AssetType = "Texture2D";
    //    //MakeMeAnAssetClassFile.FolderToAddToTheAssets("UiImages", Content);

    //    MakeMeAnAssetClassFile.WriteTheFileAndDisplayIt();
    //}

    /// <summary>
    /// 
    /// </summary>
    public static class MakeMeAnAssetClassFile
    {
        public static string Prefix { get; set; } = "";
        //public static string AssetType { get; set; } = "Texture2D" ;
        // they all end up being xnbs so this doesn't work out to well to specify types.
        public static string[] FileTypesToGet { get; set; } = { "any" };

        public static string header =
            "\n" + "using System;" 
            +"\n" + "using Microsoft.Xna.Framework;"
            +"\n" + "using Microsoft.Xna.Framework.Graphics; "
            +"\n" + "using System.Collections.Generic;"
            + "\n"
            + "\n" +"namespace Microsoft.Xna.Framework"
            +"\n{"
            +"\n public static class Asset"
            +"\n {"
            ;
        public static string closing = 
            "\n\n } \n}"
            ;
        public static string msg = "";
        
        /// <summary>
        /// using System; 
        /// using System.IO;
        /// This is a little cheating method so you don't have to type out everything in the content folder.
        /// you get a temp.txt file in my documents that is formatted to copy paste below game1 or into a new class.
        /// The file should automatically open and display.
        /// </summary>
        public static void GenerateContentAssetsFile(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            FolderToAddToTheAssets("", content);
            WriteTheFileAndDisplayIt();
        }
        /// <summary>
        /// using System; 
        /// using System.IO;
        /// This is a little cheating method so you don't have to type out everything in the content folder.
        /// you get a temp.txt file in my documents that is formatted to copy paste into game 1
        /// </summary>
        public static void FolderToAddToTheAssets(string folder, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            string envpath = Environment.CurrentDirectory;
            string contentpath = content.RootDirectory;
            string path = Path.Combine(contentpath, folder);
            content.RootDirectory = path;
            string FileSearchPath = Path.Combine(envpath, path);
            string[] filesPrefixs;
            string[] filesClassTypes;
            string[] files; 
            GetFileNamesInFolderWithoutExt(FileSearchPath, out files, out filesClassTypes, out filesPrefixs);
            content.RootDirectory = contentpath;
            string itemsprefix = Prefix;

            string tempword = path.Replace('/', '_');
            tempword = tempword.Replace(Path.DirectorySeparatorChar, '_');
            tempword = tempword.Replace(Path.AltDirectorySeparatorChar, '_');
            tempword = tempword.Replace(Path.PathSeparator, '_');

            msg += "\n\n";
            msg += "\n  #region "+Prefix+" assets in " + tempword;
            msg += "\n";
            msg += "\n   " + "public static List<Texture2D>Texture2DList = new List<Texture2D>();";
            msg += "\n   " + "public static List<SpriteFont>SpriteFontList = new List<SpriteFont>();";
            msg += "\n   " + "public static List<Effect>EffectList = new List<Effect>();";
            msg += "\n";
            for (int i = 0; i < files.Length; i++)
            {
                msg += "\n   " + "public static "+ filesClassTypes[i] + "  " + Prefix + filesPrefixs[i] + files[i] + ";";
            }
            msg += "\n";
            // the part were its loaded
            msg += "\n" + "   public static void LoadFrom_"+ tempword + "( Microsoft.Xna.Framework.Content.ContentManager Content )";
            msg += "\n   {";
            msg += "\n    Content.RootDirectory = " + '@' + '"' + path + '"' + ";";
            msg += "\n";
            for (int i = 0; i < files.Length; i++)
            {
                msg += "\n       " + Prefix + filesPrefixs[i] + files[i] + " = Content.Load<"+ filesClassTypes[i] + ">( " + '"' + files[i] + '"' + ");";
            }
            msg += "\n";
            for (int i = 0; i < files.Length; i++)
            {
                msg += "\n       " + filesClassTypes[i] + "List.Add("+Prefix + filesPrefixs[i] + files[i] + ");";
            }
            msg += "\n";
            msg += "\n\n    Content.RootDirectory = " + '@' + '"' + contentpath + '"' + ";";
            msg += "\n   }";
            msg += "\n\n  #endregion";
        }

        public static void WriteTheFileAndDisplayIt()
        {
            string fullpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "temp.txt");
            File.WriteAllText(fullpath, (header + msg + closing));
            Process.Start(fullpath);
        }

        /// <summary>
        /// Gets the files in a specific directory.
        /// </summary>
        private static void GetFileNamesInFolderWithoutExt(string path, out string[] nameArray, out string[] xnbTypeArray, out string[] prefixArray)
        {
            string[] prefixarray;
            string[] typearray;
            string[] namearray;
            if (path != null)
            {
                if (File.Exists(path))
                    path = PathRemoveFileName(path);
                // Use any all to get all the file types.
                namearray = Directory.GetFiles(path); // this returns the full path
                typearray = new string[namearray.Length];
                prefixarray = new string[namearray.Length];
                int i = 0;
                while (i < namearray.Length)
                {
                    namearray[i] = Path.GetFileName(namearray[i]);
                    string a;
                    string b;
                    DetermineXnbType(path, namearray[i], out a, out b);
                    typearray[i] = a;
                    prefixarray[i] = b;
                    // amend the name array.
                    namearray[i] = Path.GetFileNameWithoutExtension(namearray[i]);
                    i++;
                }
            }
            else
            {
                namearray = new string[0];
                typearray = new string[0];
                prefixarray = new string[0];
            }
            nameArray = namearray;
            xnbTypeArray = typearray;
            prefixArray = prefixarray;
        }
        /// <summary>
        /// Gets the files of a given type (ie.. png ect...) in a specific directory use "any" to get them all.
        /// </summary>
        private static string[] GetFileNamesInFolderWithoutExt(string path, string[] filetypes)
        {
            string[] namearray;
            List <string> parsedarray = new List<string>();
            if (path != null && filetypes.Length > 0)
            {
                if (File.Exists(path))
                    path = PathRemoveFileName(path);
                // Use any all to get all the file types.
                namearray = Directory.GetFiles(path); // this returns the full path
                int i = 0;
                while (i < namearray.Length)
                {
                    int j = 0;
                    while (j < filetypes.Length)
                    {
                        namearray[i] = Path.GetFileName(namearray[i]);
                        string comp = Path.GetExtension(namearray[i]);
                        if (filetypes[j] == comp)
                        {
                            namearray[i] = Path.GetFileNameWithoutExtension(namearray[i]);
                            parsedarray.Add(namearray[i]);
                        }
                        j++;
                    }
                    i++;
                }
            }
            else
            {
                namearray = new string[0];
            }
            return parsedarray.ToArray();
        }
        /// <summary>
        /// return just the path without the filename
        /// </summary>
        private static string PathRemoveFileName(string filenameorfolder)
        {
            return Path.GetDirectoryName(filenameorfolder);
        }

        /// <summary>
        /// just opens the xnb and searches for a string that matches a known type not the best way to do it but its fine.
        /// </summary>
        private static void DetermineXnbType(string directoryPath, string filename, out string filetype, out string prefix)
        {
            string xnbfiletype = "- Unknown";
            string prefixAbr = "- Unknown_";
            string fullfilePath = Path.Combine(directoryPath, filename);
            var textLinesArray = File.ReadAllLines(fullfilePath);
            string test = "";
            if (textLinesArray.Length > 0)
                test = textLinesArray[0];
            if (textLinesArray.Length > 1)
                test += textLinesArray[1];
            
            if (test.Contains("Texture2D"))
            {
                xnbfiletype = "Texture2D";
                prefixAbr = "texture_";
            }
            if (test.Contains("Effect"))
            {
                xnbfiletype = "Effect";
                prefixAbr = "effect_";
            }
            if (test.Contains("SpriteFont"))
            {
                xnbfiletype = "SpriteFont";
                prefixAbr = "font_";
            }
            filetype = xnbfiletype;
            prefix = prefixAbr;

            Console.WriteLine(filename + "   xnb Type:" + xnbfiletype);// + "  data: " + test);
        }
    }
}
