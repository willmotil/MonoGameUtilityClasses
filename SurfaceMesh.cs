using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Microsoft.Xna.Framework
{
    /// <summary>
    /// Will Motill 2016.
    /// This class provides methods to build indexed triangle meshes from a grid of positional points.
    /// </summary>
    public class SurfaceMesh
    {
        public bool showConsoleInfo = false;
        public bool invertXtextureCoordinatesCreation = false;
        public bool invertYtextureCoordinatesCreation = true;
        public bool invertNormalsOnCreation = false;
        int surfacePointWidth = 0;

        public VertexPositionNormalTexture[] vertices;
        public int[] indices;

        public void CreateSurfaceMesh(Vector4[] inputSurfaceVectors, int w, int h)
        {
            surfacePointWidth = w;
            indices = SetUpIndices(w, h);
            vertices = VertexFromVectorArray(w, h, inputSurfaceVectors);
            vertices = CreateSmoothNormals(vertices, indices);
        }

        public void CreateSurfaceMesh(Vector4[] inputSurfaceVectors, int w, int h, out VertexPositionNormalTexture[] verts, out int[] indexs)
        {
            surfacePointWidth = w;
            indices = SetUpIndices(w, h);
            vertices = VertexFromVectorArray(w, h, inputSurfaceVectors);
            vertices = CreateSmoothNormals(vertices, indices);
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
        VertexPositionNormalTexture[] VertexFromVectorArray(int verts_width, int verts_height, Vector4[] inputSurfaceVectors)
        {
            surfacePointWidth = verts_width;
            VertexPositionNormalTexture[] MyMeshVertices = new VertexPositionNormalTexture[verts_width * verts_height];
            vertices = new VertexPositionNormalTexture[verts_width * verts_height];
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
        /// This loops thru the index array finding each triangle connected to a vertice.
        /// It then calculates the normal for those triangles and averages them for the vertice in question.
        /// This method can deal with abritrary numbers of connected triangles 0 to n connections.
        /// </summary>
        VertexPositionNormalTexture[] CreateSmoothNormals(VertexPositionNormalTexture[] vertices, int[] indexs)
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

        /// <summary>
        /// Draws this surface mesh using draw user index primitives..
        /// Doesn't set effect parameters or tequniques.
        /// </summary>
        public void Draw(GraphicsDevice gd, Effect effect)
        {
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gd.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, (indices.Length / 3), VertexPositionNormalTexture.VertexDeclaration);
            }
        }
    }
}
