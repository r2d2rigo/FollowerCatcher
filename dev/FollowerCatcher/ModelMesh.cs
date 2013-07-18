using SharpDX;
using SharpDX.Direct3D11;

namespace FollowerCatcher
{
    /// <summary>
    /// Representes a mesh of a model, containing the vertex buffer and index buffer.
    /// </summary>
    internal class ModelMesh
    {
        public Buffer IndexBuffer;
        public Buffer VertexBuffer;
        public VertexBufferBinding Binding;
        public int IndexCount;

        public ModelMesh(Buffer vertexBuffer, Buffer indexBuffer, Buffer invertexIndexBuffer, int indexCount)
        {
            this.VertexBuffer = vertexBuffer;
            this.IndexBuffer = indexBuffer;
            this.Binding = new VertexBufferBinding(this.VertexBuffer, Utilities.SizeOf<ModelVertex>(), 0);
            this.IndexCount = indexCount;
        }
    }

}
