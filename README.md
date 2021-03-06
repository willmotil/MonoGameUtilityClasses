# MonoGameUtilityClasses

Utility classes for monogame or c#

Note: (the FrameRate class uses the MgStringBuilder to avoid garbage collections).

Listing...

_____________________________________

Game1_Fps... 

This class shows the usage of the MgFrameRate. MgStringBuilder. MgTextBounder.

You can put all of these into a MonoGameProject change the namespace for game1 
Make a font named MgFont and try it out. 
This project was tested on GL it should work on Dx as well.

<img src="https://github.com/willmotil/MonoGameUtilityClasses/blob/master/Images/ExampleFpsMgSbTextBounder.png?raw=true">

The old gl project 

https://drive.google.com/open?id=1KSrPglaYow8pVORL315pYaV6fb6MlpU0

_____________________________________

MgStringBuilder. 

This class is a wrapper around stringbuilder it can be used in place of stringbuilder in most cases.
It can take or return a stringbuilder to work with it were necessary.
It also has quite a few convenient operator overloads.
This class can bypass many garbage collections that stingbuilder doesn't, especially for numerical appends and inserts. 
This class bypasses numeric.ToString() which causes garbage collections in C# itself, which in turn affect monogame.
The performance of inserting is not as great due to the work arounds that are done using the stringbuilder indexer.
The class despite the size which is unrolled, is pretty performant otherwise, i use this myself constantly.

If you were to rip out the Monogame specific Append Methods. (Vectors Rectangle Color)
Then this class can be used as a standalone c# class. 
In that case you should be aware that super high precision or extremely large values may cause garbage as i have limited the number of digits checked reasonably to trade off for perfomance before defaulting to just letting stringbuilder append.
In practice this is doesn't happen as the number of digits set is pretty high, but it could.

_____________________________________

MgFrameRate.

This class is used to display framerate.
The class displays basic information about fps frame and update rates draw.
It also displays useful memory usage info in the game window, when collections occur and how much is being lost.
The class is setup typically in load. 
You call to it in update and draw, that is all that is required other then that you have loaded a font.

_____________________________________

MgTextBounder.

This class (like word wrap) takes a StringBuilder and wraps it to fit into a specified rectangle.
This class is new, and will probably see many more additions and revisions.
Unlike measure string this class copys or directly alters a stringbuilder to format it to fit on the fly.
This class was and is in another form a experimental prototype for a direct DrawString() overload.

_____________________________________

TextWrapping 

This class is very similar to the above but it is a real time stand alone class and method.
Measure string is not required as the method effectively includes it in the run but in a modified manner.
Which instead word wraps a altered version then draws it.
If used in combination with the mgstringbuilder no garbage allocations will be generated for dynamic text.

_____________________________________

SurfaceMesh.

Still not fully completed as its companion class is not done, who knows if i will ever get it done.
This class takes a array of vector4's and treats them as if they are to be made into a VertexPositionNormalTexture array.
It creates the u,v's along the surface proportionally to fit a single texture and creates smooth normals.
The smooth normals are best used when the surface area has curvature for light reflection.

_____________________________________

SpriteFontConverter.

Takes a loaded SpriteFont and turns it into a class that can self load a instance of spritefont.
This maybe useful maybe as a default font if something goes wrong.
While im not too sure it's really useful at all it is a neet little class.
It uses a simple RLE algorithm to compress the pixel data values so the texture is also in the resulting class file.

The class that is created is saved as a text file with a .cs extention that can be pulled into a visual studio project.
When you run the project after instatiating the class instance. 
Load can be called on the instance to load the hardcoded spritefont.

_____________________________________

WinFullscreenModeChangeTestApp.

In the Tests Folder is also a full screen testing class.
The full project for that can be found here.

https://drive.google.com/open?id=1SLpOZWz_whcPEcxz4fTk9OUd8wu9xPbw
