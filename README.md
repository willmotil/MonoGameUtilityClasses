# MonoGameUtilityClasses

Utility classes for monogame or c#

Note: (the FrameRate class uses the MgStringBuilder to avoid garbage collections).

Listing...

Game1_Fps... 

This class shows the usage of the MgFrameRate. MgStringBuilder. MgTextBounder.
You can put all of these into a MonoGameProject change the namespace for game1 
Make a font named MgFont and try it out. This project was tested under Dx it should work on Gl as well.

The full gl project 

https://drive.google.com/open?id=1zSlqFKJfUUmTuAm-jYxm0RJRxuT8KAoY

MgStringBuilder. 

This class is a wrapper around stringbuilder it can be used in place of stringbuilder in most cases.
It can take or return a stringbuilder to work with it were necessary.
It also has quite a few convenient operator overloads.
This class can bypass many garbage collections that stingbuilder doesn't, especially for numerical appends and inserts. 
This class bypasses numeric.ToString() which causes garbage collections in C# itself, which in turn affect monogame.
The performance of inserting is not as great due to the work arounds that are done using the stringbuilder indexer.
The class despite the size which is unrolled, is pretty performant otherwise, i use this myself constantly.

MgFrameRate.

This class is used to display framerate.
The class displays basic information about fps frame and update rates draw.
It also displays useful memory usage info in the game window, when collections occur and how much is being lost.
The class is setup typically in load. 
You call to it in update and draw, that is all that is required other then that you have loaded a font.

MgTextBounder.

This class (like word wrap) takes a StringBuilder and wraps it to fit into a specified rectangle.
This class is new, and will probably see many more additions and revisions.
Unlike measure string this class copys or directly alters a stringbuilder to format it to fit on the fly.
This class was and is in another form a experimental prototype for a direct DrawString() overload.

<img src="https://github.com/willmotil/MonoGameUtilityClasses/blob/master/Images/ExampleFpsMgSbTextBounder.png?raw=true">


SurfaceMesh.

Still not fully completed as its companion class is not done, who knows if i will ever get it done.
This class takes a array of vector4's and treats them as if they are to be made into a VertexPositionNormalTexture array.
It creates the u,v's along the surface proportionally to fit a single texture and creates smooth normals.
The smooth normals are best used when the surface area has curvature for light reflection.
