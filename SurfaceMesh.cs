using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Will Motill 2016. - 2018
    /// This class provides methods to build indexed triangle meshes from a grid of positional points.
    /// It also provides the ability to create smooth normals and generate simple tangents for normal mapping.
    /// </summary>
    public class SurfaceMesh
    {
        public bool showConsoleInfo = false;
        public bool invertXtextureCoordinatesCreation = false;
        public bool invertYtextureCoordinatesCreation = true;
        public bool invertNormalsOnCreation = false;
        int surfacePointWidth = 0;

        public VertexPositionNormalTextureTangent[] vertices;
        public int[] indices;

        public void CreateSurfaceMesh(Vector4[] inputSurfaceVectors, int w, int h)
        {
            surfacePointWidth = w;
            indices = SetUpIndices(w, h);
            vertices = VertexFromVectorArray(w, h, inputSurfaceVectors);
            vertices = CreateSmoothNormals(vertices, indices);
            CreateTangents();
        }

        public void CreateSurfaceMesh(Vector4[] inputSurfaceVectors, int w, int h, out VertexPositionNormalTextureTangent[] verts, out int[] indexs)
        {
            surfacePointWidth = w;
            indices = SetUpIndices(w, h);
            vertices = VertexFromVectorArray(w, h, inputSurfaceVectors);
            vertices = CreateSmoothNormals(vertices, indices);
            CreateTangents();
            verts = vertices;
            indexs = indices;
        }

        public Vector4[] Vector4FromVector3Array(Vector3[] inputSurfaceVectors)
        {
            var len = inputSurfaceVectors.Length;
            Vector4[] vector4_surface_pos_ary = new Vector4[len];
            for (int i = 0; i < inputSurfaceVectors.Length; i++)
            {
                vector4_surface_pos_ary[i] = new Vector4(
                    inputSurfaceVectors[i].X,
                    inputSurfaceVectors[i].Y,
                    inputSurfaceVectors[i].Z,
                    1f
                );
            }
            return vector4_surface_pos_ary;
        }

        int[] SetUpIndices(int w, int h)
        {
            int counter = 0;
            indices = new int[(w - 1) * (h - 1) * 6];
            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    int topLeft = x + (y + 1) * w;
                    int topRight = (x + 1) + (y + 1) * w;
                    int lowerRight = (x + 1) + y * w;
                    int lowerLeft = x + y * w;
                    indices[counter++] = (int)topLeft;
                    indices[counter++] = (int)lowerRight;
                    indices[counter++] = (int)lowerLeft;
                    indices[counter++] = (int)topLeft;
                    indices[counter++] = (int)topRight;
                    indices[counter++] = (int)lowerRight;
                }
            }
            return indices;
        }

        /// <summary>
        /// Couldn't really decide if i wanted this method to be public or not.
        /// </summary>
        VertexPositionNormalTextureTangent[] VertexFromVectorArray(int verts_width, int verts_height, Vector4[] inputSurfaceVectors)
        {
            surfacePointWidth = verts_width;
            VertexPositionNormalTextureTangent[] MyMeshVertices = new VertexPositionNormalTextureTangent[verts_width * verts_height];
            vertices = new VertexPositionNormalTextureTangent[verts_width * verts_height];
            for (int y = 0; y < verts_height; y++)
            {
                for (int x = 0; x < verts_width; x++)
                {
                    int index = x + y * verts_width;
                    float u = (float)x / (verts_width - 1);
                    float v = (float)y / (verts_height - 1);
                    if (invertXtextureCoordinatesCreation) { u = 1f - u; }
                    if (invertYtextureCoordinatesCreation) { v = 1f - v; }
                    var tmp = inputSurfaceVectors[index];
                    Vector3 vec = new Vector3(tmp.X, tmp.Y, tmp.Z);
                    MyMeshVertices[index].Position = vec;
                    MyMeshVertices[index].TextureCoordinate.X = u;
                    MyMeshVertices[index].TextureCoordinate.Y = v;
                }
            }
            return MyMeshVertices;
        }

        /// <summary>
        /// This method creates smoothed normals from a indexed vertice mesh array.
        /// This loops thru the index array finding the each triangle connected to a vertice.
        /// It then calculates the normal for those triangles and averages them for the vertice in question.
        /// This method can deal with abritrary numbers of connected triangles 0 to n connections.
        /// </summary>
        VertexPositionNormalTextureTangent[] CreateSmoothNormals(VertexPositionNormalTextureTangent[] vertices, int[] indexs)
        {
            // For each vertice we must calculate the surrounding triangles normals, average them and set the normal.
            int tvertmultiplier = 3;
            int triangles = (int)(indexs.Length / tvertmultiplier);
            for (int currentTestedVerticeIndex = 0; currentTestedVerticeIndex < vertices.Length; currentTestedVerticeIndex++)
            {
                Vector3 sum = Vector3.Zero;
                float total = 0;
                for (int t = 0; t < triangles; t++)
                {
                    int tvstart = t * tvertmultiplier;
                    int tindex0 = tvstart + 0;
                    int tindex1 = tvstart + 1;
                    int tindex2 = tvstart + 2;
                    var vindex0 = indices[tindex0];
                    var vindex1 = indices[tindex1];
                    var vindex2 = indices[tindex2];
                    if (vindex0 == currentTestedVerticeIndex || vindex1 == currentTestedVerticeIndex || vindex2 == currentTestedVerticeIndex)
                    {
                        var n0 = (vertices[vindex1].Position - vertices[vindex0].Position) * 10f; // * 10 math artifact avoidance.
                        var n1 = (vertices[vindex2].Position - vertices[vindex1].Position) * 10f;
                        var cnorm = Vector3.Cross(n0, n1);
                        sum += cnorm;
                        total += 1;
                    }
                }
                if (total > 0)
                {
                    var averagednormal = sum / total;
                    averagednormal.Normalize();
                    if (invertNormalsOnCreation)
                        averagednormal = -averagednormal;
                    vertices[currentTestedVerticeIndex].Normal = averagednormal;
                }
            }
            return vertices;
        }

        void CreateTangents()
        {
            for(int i = 0; i < vertices.Length;i++)
            {
                int y = i / surfacePointWidth;
                int x = i - (y * surfacePointWidth);
                int up = (y - 1) * surfacePointWidth + x;
                int down = (y + 1) * surfacePointWidth + x;
                Vector3 tangent = new Vector3();
                if(down >= vertices.Length)
                {
                    tangent = vertices[up].Position - vertices[i].Position;
                    tangent.Normalize();
                    vertices[i].Tangent = tangent;
                }
                else
                {
                    tangent = vertices[i].Position - vertices[down].Position;
                    tangent.Normalize();
                    vertices[i].Tangent = tangent;
                }
            }
        }

        /// <summary>
        /// Draws this surface mesh using user index primitives..
        /// Doesn't set effect parameters or tequniques.
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTextureTangent.VertexDeclaration);
            }
        }

        public struct VertexPositionNormalTextureTangent : IVertexType
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
            public Vector3 Tangent;

            public static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                  new VertexElement(VertexElementByteOffset.PositionStartOffset(), VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector2(), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                  new VertexElement(VertexElementByteOffset.OffsetVector3(), VertexElementFormat.Vector3, VertexElementUsage.Normal, 1)
            );
            VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
        }
        /// <summary>
        /// This is a helper struct for tallying byte offsets
        /// </summary>
        public struct VertexElementByteOffset
        {
            public static int currentByteSize = 0;
            [STAThread]
            public static int PositionStartOffset() { currentByteSize = 0; var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(float n) { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector2 n) { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Color n) { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector3 n) { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int Offset(Vector4 n) { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }

            public static int OffsetFloat() { var s = sizeof(float); currentByteSize += s; return currentByteSize - s; }
            public static int OffsetColor() { var s = sizeof(int); currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector2() { var s = sizeof(float) * 2; currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector3() { var s = sizeof(float) * 3; currentByteSize += s; return currentByteSize - s; }
            public static int OffsetVector4() { var s = sizeof(float) * 4; currentByteSize += s; return currentByteSize - s; }
        }
    }
}
