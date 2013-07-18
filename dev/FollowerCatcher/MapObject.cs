namespace FollowerCatcher
{
    /// <summary>
    /// Represents a static map decoration (trees, windows...).
    /// </summary>
    public class MapObject
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the parent lightmap.
        /// </summary>
        /// <value>
        /// The parent lightmap.
        /// </value>
        public Texture ParentLightmap { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapObject"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="lightmap">The lightmap.</param>
        public MapObject(Model model, Texture lightmap)
        {
            this.Model = model;
            this.ParentLightmap = lightmap;
        }
    }
}
