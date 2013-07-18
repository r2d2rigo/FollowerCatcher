using System.Collections.Generic;

namespace FollowerCatcher
{
    /// <summary>
    /// Compares meshes for draw sorting.
    /// </summary>
    public class EntityMeshComparer : IComparer<Entity>
    {
        /// <summary>
        /// Compares if both entities' models are the same.
        /// </summary>
        /// <param name="x">First entity.</param>
        /// <param name="y">Second entity.</param>
        /// <returns>-1 or 1 if models differ, 0 if they are the same.</returns>
        public int Compare(Entity x, Entity y)
        {
            int hashX = x.Model.modelPath.GetHashCode();
            int hashY = y.Model.modelPath.GetHashCode();

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
