# MonoGameUtilityClasses

Stand alone utility classes for monogame or c#

Listing...

MgStringBuilder. 
This class can be used as a replacement for stringbuilder that bypasses garbage creation in monogame.
This class helps in preventing garbage collections from occuring.
The class despite the size is highly performant i use this myself constantly.

MgFrameRate.
This class is used to display framerate.
The class displays basic information about memory usage, when collections occur and how much is being lost.
The class takes a initial setup typically in load, you call to it in update and draw, that is all that is required

MgTextBounder.
This class (like word wrap) takes a StringBuilder and wraps it to fit into a specified rectangle.
This class is new, and will probably see many more additions and revisions.
Unlike measure string this class copys or directly alters a stringbuilder to format it to fit.
