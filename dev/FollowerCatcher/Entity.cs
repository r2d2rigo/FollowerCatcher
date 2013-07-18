using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowerCatcher
{
    /// <summary>
    /// Represents the base class for all elements that will exist in a game.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>
        /// The model.
        /// </value>
        public Model Model { get; private set; }

        private Matrix transform;
        /// <summary>
        /// Gets or sets the transform.
        /// </summary>
        /// <value>
        /// The transform.
        /// </value>
        public Matrix Transform
        {
            get
            {
                if (Parent != null)
                {
                    return this.transform * Parent.Transform;
                }
                return this.transform;
            }
            set
            {
                if (transform != value)
                {
                    transform = value;

                    if (Model != null)
                    {
                        Vector4 min = Vector3.Transform(Model.BBox.Minimum, Transform);
                        Vector4 max = Vector3.Transform(Model.BBox.Maximum, Transform);
                        BBox = new BoundingBox(new Vector3(min.X, min.Y, min.Z), new Vector3(max.X, max.Y, max.Z));
                    }
                }
            }
        }
        /// <summary>
        /// Gets or sets the target destination.
        /// </summary>
        /// <value>
        /// The target destination.
        /// </value>
        public Vector3 MoveTo { get; set; }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Texture Texture { get; set; }

        /// <summary>
        /// Gets or sets the lightmap texture.
        /// </summary>
        /// <value>
        /// The lightmap texture.
        /// </value>
        public Texture Lightmap { get; set; }

        /// <summary>
        /// Gets the child entity.
        /// </summary>
        /// <value>
        /// The child entity.
        /// </value>
        public Entity Child { get; private set; }

        /// <summary>
        /// Gets the parent entity.
        /// </summary>
        /// <value>
        /// The parent entity.
        /// </value>
        public Entity Parent { get; private set; }

        /// <summary>
        /// The bounding box.
        /// </summary>
        public BoundingBox BBox;

        /// <summary>
        /// Gets or sets a value indicating whether the object is collidable.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is collidable; otherwise, <c>false</c>.
        /// </value>
        public bool IsCollidable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the player can pick up this object.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is pickable; otherwise, <c>false</c>.
        /// </value>
        public bool IsPickable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Entity"/> rotates.
        /// </summary>
        /// <value>
        ///   <c>true</c> if rotates; otherwise, <c>false</c>.
        /// </value>
        public bool Rotates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the matrix should be restored after drawing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [restore matrix]; otherwise, <c>false</c>.
        /// </value>
        public bool RestoreMatrix { get; set; }

        /// <summary>
        /// Gets or sets the effect delay.
        /// </summary>
        /// <value>
        /// The effect delay.
        /// </value>
        public float EffectDelay { get; set; }

        private Vector3 cachedPosition;
        internal Matrix oldMatrix;

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        public Entity()
        {
            Model = null;
            Transform = Matrix.Identity;
            MoveTo = Vector3.Zero;
            Lightmap = null;
            Child = null;
            Parent = null;
            IsPickable = false;
            cachedPosition = Vector3.Zero;
            EffectDelay = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        /// <param name="partModel">The part model.</param>
        /// <param name="transform">The transform.</param>
        public Entity(Model partModel, Matrix transform)
        {
            this.Model = partModel;
            this.Transform = transform;
            MoveTo = Vector3.Zero;
            Lightmap = null;
            Child = null;
            Parent = null;
            IsPickable = false;
            cachedPosition = Vector3.Zero;
            EffectDelay = 0;
        }

        /// <summary>
        /// Sets the child entity.
        /// </summary>
        /// <param name="child">The child entity.</param>
        public void SetChild(Entity child)
        {
            this.Child = child;
            child.Parent = this;
        }

        /// <summary>
        /// Updates the state of the entity.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time.</param>
        public void Update(float elapsedTime)
        {
            cachedPosition.X = Transform.M41;
            cachedPosition.Y = Transform.M42;
            cachedPosition.Z = Transform.M43;

            Vector3 moveDirection = MoveTo - cachedPosition;

            if (moveDirection.Length() > 1.0f)
            {
                Transform = Matrix.Translation(moveDirection * elapsedTime * (moveDirection.Length() / 2)) * Transform;
            }
            else
            {
                MoveTo = cachedPosition;
            }
        }
    }
}
