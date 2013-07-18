using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Assimp.Configs;
using System.IO;
using SharpDX;

namespace ModelCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ModelCompiler.exe model.dae");
                return;
            }

            foreach (string filename in args)
            {
                AssimpImporter importer = new AssimpImporter();
                PostProcessSteps steps = PostProcessPreset.TargetRealTimeMaximumQuality;
                steps |= PostProcessSteps.OptimizeMeshes | PostProcessSteps.SplitLargeMeshes | PostProcessSteps.Triangulate
                    | PostProcessSteps.FlipUVs | PostProcessSteps.LimitBoneWeights;
                importer.SetConfig(new VertexBoneWeightLimitConfig(2));
                Scene scene = importer.ImportFile(filename, steps);

                Vector3D bbMin = new Vector3D(float.MaxValue, float.MaxValue, float.MaxValue);
                Vector3D bbMax = new Vector3D(float.MinValue, float.MinValue, float.MinValue);

                Node meshNode = scene.RootNode;

                while (!meshNode.HasMeshes)
                {
                    meshNode = meshNode.Children[0];
                }

                Mesh nodeGeometry = scene.Meshes[meshNode.MeshIndices[0]];
                List<Vector3D> transformedVertices = new List<Vector3D>();

                foreach (Vector3D vertex in nodeGeometry.Vertices)
                {
                    Vector4 tempv = new Vector4(vertex.X, vertex.Y, vertex.Z, 1.0f);
                    Matrix tempm = Matrix.Scaling(0.1f)  * Matrix.RotationYawPitchRoll(((float)Math.PI / 2.0f), ((float)Math.PI / 2.0f), 0.0f) *
                    new Matrix(meshNode.Transform.A1, meshNode.Transform.A2, meshNode.Transform.A3, meshNode.Transform.A3,
                        meshNode.Transform.B1, meshNode.Transform.B2, meshNode.Transform.B3, meshNode.Transform.B3,
                        meshNode.Transform.C1, meshNode.Transform.C2, meshNode.Transform.C3, meshNode.Transform.C3,
                        meshNode.Transform.D1, meshNode.Transform.D2, meshNode.Transform.D3, meshNode.Transform.D3);
                    Vector4 finalvec = Vector4.Transform(tempv, tempm);

                    transformedVertices.Add(new Vector3D(finalvec.X, finalvec.Y, finalvec.Z));
                }

                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename + ".mdl")))
                {
                    foreach (Vector3D vector in transformedVertices)
                    {
                        bbMin.X = Math.Min(vector.X, bbMin.X);
                        bbMin.Y = Math.Min(vector.Y, bbMin.Y);
                        bbMin.Z = Math.Min(vector.Z, bbMin.Z);

                        bbMax.X = Math.Max(vector.X, bbMax.X);
                        bbMax.Y = Math.Max(vector.Y, bbMax.Y);
                        bbMax.Z = Math.Max(vector.Z, bbMax.Z);
                    }

                    writer.Write(bbMin.X);
                    writer.Write(bbMin.Y);
                    writer.Write(bbMin.Z);

                    writer.Write(bbMax.X);
                    writer.Write(bbMax.Y);
                    writer.Write(bbMax.Z);

                    //writer.Write(scene.Meshes.Length);
                    writer.Write(1);

                    Mesh currentMesh = nodeGeometry;

                    if (!currentMesh.HasTextureCoords(0))
                    {
                        Console.WriteLine("ERROR: model must have texture coordinates");
                        return;
                    }

                    Vector3D[] texCoords = currentMesh.GetTextureCoords(0);

                    writer.Write(currentMesh.VertexCount);

                    for (int j = 0; j < currentMesh.VertexCount; j++)
                    {
                        writer.Write(transformedVertices[j].X);
                        writer.Write(transformedVertices[j].Y);
                        writer.Write(transformedVertices[j].Z);

                        writer.Write(texCoords[j].X);
                        writer.Write(texCoords[j].Y);
                    }

                    writer.Write(currentMesh.Faces.Length * 3);
                    for (int j = 0; j < currentMesh.Faces.Length; j++)
                    {
                        Face currentFace = currentMesh.Faces[j];

                        if (currentFace.IndexCount != 3)
                        {
                            Console.WriteLine("ERROR: non-triangle found!");
                            Console.ReadKey();
                            return;
                        }

                        foreach (uint index in currentFace.Indices)
                        {
                            writer.Write((int)index);
                        }
                    }

                    //writer.Write(currentMesh.Faces.Length * 3);
                    //for (int j = 0; j < currentMesh.Faces.Length; j++)
                    //{
                    //    Face currentFace = currentMesh.Faces[j];

                    //    if (currentFace.IndexCount != 3)
                    //    {
                    //        Console.WriteLine("ERROR: non-triangle found!");
                    //        Console.ReadKey();
                    //        return;
                    //    }

                    //    foreach (uint index in currentFace.Indices)
                    //    {
                    //        writer.Write(transformedVertices[(int)index].X);
                    //        writer.Write(transformedVertices[(int)index].Y);
                    //        writer.Write(transformedVertices[(int)index].Z);

                    //        writer.Write(texCoords[index].X);
                    //        writer.Write(texCoords[index].Y);
                    //    }
                    //}
                }
            }
        }
    }
}
