using System.Collections.Generic;

namespace FollowerCatcher
{
    /// <summary>
    /// Compares textures for draw sorting.
    /// </summary>
    public class EntityTextureComparer : IComparer<Entity>
    {
        /// <summary>
        /// Compares if both entities' textures are the same.
        /// </summary>
        /// <param name="x">First entity.</param>
        /// <param name="y">Second entity.</param>
        /// <returns>-1 or 1 if textures differ, 0 if they are the same.</returns>
        public int Compare(Entity x, Entity y)
        {
            int hashX = x.Texture.TexturePath.GetHashCode();
            int hashY = y.Texture.TexturePath.GetHashCode();

            if (hashX < hashY)
            {
                return -1;
            }
            else if (hashX > hashY)
            {
                return 1;
            }

            return 0;
        }
    }
}
