using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;

namespace FollowerCatcher
{
    /// <summary>
    /// Factory for creating entities with a specific model and texture. 
    /// </summary>
    public class EntityFactory
    {
        private Dictionary<string, Texture> cachedTextures;
        private Dictionary<string, Model> cachedModels;
        private Device1 device;

        /// <summary>
        /// Initializes the factory instance.
        /// </summary>
        /// <param name="device">DirectX device.</param>
        public EntityFactory(Device1 device)
        {
            this.cachedTextures = new Dictionary<string, Texture>();
            this.cachedModels = new Dictionary<string, Model>();
            this.device = device;
        }

        /// <summary>
        /// Creates an entity with a model and a texture. Both of them are cached so no re-load happens.
        /// </summary>
        /// <param name="modelPath">Path to the entity's model.</param>
        /// <param name="texturePath">Path to the entity's texture.</param>
        /// <returns></returns>
        public Entity CreateEntity(string modelPath, string texturePath)
        {
            Texture entityTexture;
            Model entityModel;

            if (cachedTextures.ContainsKey(texturePath))
            {
                entityTexture = cachedTextures[texturePath];
            }
            else
            {
                Texture newTexture = Texture.LoadFromFile(device, texturePath).Result;
                entityTexture = newTexture;
                cachedTextures.Add(texturePath, entityTexture);
            }

            if (cachedModels.ContainsKey(modelPath))
            {
                entityModel = cachedModels[modelPath];
            }
            else
            {
                Model newModel = Model.Load(device, modelPath);
                entityModel = newModel;
                cachedModels.Add(modelPath, entityModel);
            }

            Entity newEntity = new Entity(entityModel, Matrix.Identity);
            newEntity.Texture = entityTexture;

            return newEntity;
        }
    }
}
