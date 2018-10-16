# MonoGameUtilityClasses

Stand alone utility classes for monogame or c#

Note: (the FrameRate class uses the MgStringBuilder to avoid garbage collections).

Listing...

MgStringBuilder. 
This class is a wrapper around stringbuilder it can be used in place of stringbuilder in most cases.
It can also take or return a stringbuilder to work with it were necessary.
It also has quite a few convient operator overloads.
This class can bypass many garbage collections that stingbuilder doesn't especially for numerical appends and inserts. 
This class bypasses numeric.ToString() that causes garbage collections in C# itself which in turn affect monogame.
The performance of inserting is not as great due to the work arounds that are done.
The class despite the size which is unrolled, is pretty performant, i use this myself constantly.

MgFrameRate.
This class is used to display framerate.
The class displays basic information about memory usage, when collections occur and how much is being lost.
The class takes a initial setup typically in load, you call to it in update and draw, that is all that is required

MgTextBounder.
This class (like word wrap) takes a StringBuilder and wraps it to fit into a specified rectangle.
This class is new, and will probably see many more additions and revisions.
Unlike measure string this class copys or directly alters a stringbuilder to format it to fit.

<img src="https://github.com/willmotil/MonoGameUtilityClasses/blob/master/Images/ExampleFpsMgSbTextBounder.png?raw=true">


SurfaceMesh.
This class takes a array of vector4's and treats them as if they are to be made into a VertexPositionNormalTexture array.
It creates the u,v's along the surface proportionally to fit a single texture and creates smooth normals.
The smooth normals are best used when the surface area has curvature for light reflection.
