using SharpDX;

namespace FollowerCatcher
{
    /// <summary>
    /// Represents a vertex with position and texture coordinate.
    /// </summary>
    internal struct ModelVertex
    {
        public Vector3 Position;
        public Vector2 TexCoords;

        public ModelVertex(Vector3 pos, Vector2 tc)
        {
            this.Position = pos;
            this.TexCoords = tc;
        }
    }

}
