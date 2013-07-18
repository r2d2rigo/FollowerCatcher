using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.IO;
using System.Collections.Generic;
using System.IO;

namespace FollowerCatcher
{
    /// <summary>
    /// Encapsulates a collection of meshes that will be drawn.
    /// </summary>
    public class Model : System.IEquatable<Model>
    {
        internal string modelPath;
        internal List<ModelMesh> meshes;
        public BoundingBox BBox { get; private set; }
        public Vector3 Bounds
        {
            get
            {
                return BBox.Maximum - BBox.Minimum;
            }
        }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Model()
        {
            meshes = new List<ModelMesh>();
        }

        /// <summary>
        /// Loads a model from a file on disk.
        /// </summary>
        /// <param name="device">DirectX device.</param>
        /// <param name="filename">Path of the model.</param>
        /// <returns>Loaded model.</returns>
        public static Model Load(SharpDX.Direct3D11.Device1 device, string filename)
        {
            Model newModel = new Model();
            newModel.modelPath = filename;

            NativeFileStream fileStream = new NativeFileStream(filename, NativeFileMode.Open, NativeFileAccess.Read);

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                Vector3 bbMin;
                Vector3 bbMax;

                bbMin.X = reader.ReadSingle();
                bbMin.Y = reader.ReadSingle();
                bbMin.Z = reader.ReadSingle();

                bbMax.X = reader.ReadSingle();
                bbMax.Y = reader.ReadSingle();
                bbMax.Z = reader.ReadSingle();

                newModel.BBox = new BoundingBox(bbMin, bbMax);

                int numMeshes = reader.ReadInt32();

                for (int i = 0; i < numMeshes; i++)
                {
                    List<ModelVertex> vertices = new List<ModelVertex>();
                    List<int> indices = new List<int>();
                    List<int> invertedIndices = new List<int>();

                    int numVertices = reader.ReadInt32();

                    for (int j = 0; j < numVertices; j++)
                    {
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        float z = reader.ReadSingle();

                        float s = reader.ReadSingle();
                        float t = reader.ReadSingle();

                        vertices.Add(new ModelVertex(new Vector3(x, y, z), new Vector2(s, t)));
                    }

                    int numIndices = reader.ReadInt32();

                    for (int j = 0; j < numIndices; j++)
                    {
                        int index = reader.ReadInt32();
                        indices.Add(index);
                        invertedIndices.Add(index);

                        if ((j > 0) && ((j + 1) % 3 == 0))
                        {
                            int tempIndex = invertedIndices[j - 1];
                            invertedIndices[j - 1] = invertedIndices[j];
                            invertedIndices[j] = tempIndex;
                        }
                    }

                    Buffer vertexBuffer = Buffer.Create(device, BindFlags.VertexBuffer, vertices.ToArray());
                    Buffer indexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, indices.ToArray());
                    Buffer invertedIndexBuffer = Buffer.Create(device, BindFlags.IndexBuffer, invertedIndices.ToArray());
                    newModel.meshes.Add(new ModelMesh(vertexBuffer, indexBuffer, invertedIndexBuffer, numIndices));
                }

            }

            return newModel;
        }

        /// <summary>
        /// Used to check for duplicate models.
        /// </summary>
        /// <param name="other">Other model.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(Model other)
        {
            if (other == null)
            {
                return false;
            }

            // Force setting always for > 1 meshes
            if (other.meshes.Count > 1)
            {
                return false;
            }

            return this.modelPath.Equals(other.modelPath, System.StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
