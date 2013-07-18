using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Collections.Generic;

namespace FollowerCatcher
{
    /// <summary>
    /// Takes care of everything related to drawing the entities on scene.
    /// </summary>
    public class Renderer
    {
        internal Buffer constantBuffer;
        private BoundingFrustum frustum;

        private Matrix world;
        /// <summary>
        /// Current world matrix.
        /// </summary>
        public Matrix World
        {
            get { return world; }
            set
            {
                if (world != value)
                {
                    world = value;
                    WorldViewProjection = World * ViewProjection;
                }
            }
        }

        private Matrix view;
        /// <summary>
        /// Current view matrix.
        /// </summary>
        public Matrix View
        {
            get { return view; }
            set
            {
                if (view != value)
                {
                    view = value;
                    ViewProjection = View * Projection;
                }
            }
        }

        private Matrix projection;
        /// <summary>
        /// Current projection matrix.
        /// </summary>
        public Matrix Projection
        {
            get { return projection; }
            set
            {
                if (projection != value)
                {
                    projection = value;
                    ViewProjection = View * Projection;
                }
            }
        }

        private Matrix viewProjection;
        /// <summary>
        /// Current view * projection matrix.
        /// </summary>
        private Matrix ViewProjection
        {
            get { return viewProjection; }
            set
            {
                if (viewProjection != value)
                {
                    viewProjection = value;
                    // Less culling than the viewport
                    frustum = new BoundingFrustum(Matrix.Scaling(0.95f) * viewProjection);
                    WorldViewProjection = World * ViewProjection;
                }
            }
        }

        private Matrix worldViewProjection;
        /// <summary>
        /// Current world * view * projection matrix.
        /// </summary>
        private Matrix WorldViewProjection
        {
            get { return worldViewProjection; }
            set
            {
                if (worldViewProjection != value)
                {
                    worldViewProjection = value;
                }
            }
        }

        private SharpDX.Direct3D11.Device1 device;
        internal DeviceContext1 context;
        private Dictionary<int, Texture> cachedTextures;
        private Model cachedModel;

        /// <summary>
        /// Creates the renderer instance.
        /// </summary>
        /// <param name="device">DirectX device.</param>
        /// <param name="context">DirectX context.</param>
        public Renderer(SharpDX.Direct3D11.Device1 device, DeviceContext1 context)
        {
            this.device = device;
            this.context = context;
            this.cachedTextures = new Dictionary<int, Texture>();
            this.cachedModel = null;
            this.frustum = new BoundingFrustum();

            this.World = this.View = this.Projection = Matrix.Identity;
            
            // TODO: ToDispose
            this.constantBuffer = new SharpDX.Direct3D11.Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        }

        /// <summary>
        /// Binds a texture to the specified texture sampler.
        /// </summary>
        /// <param name="texture">Texture to be bound.</param>
        /// <param name="samplerSlot">Texture sampler slot.</param>
        public void SetActiveTexture(Texture texture, int samplerSlot)
        {
            if (cachedTextures.ContainsKey(samplerSlot) && cachedTextures[samplerSlot].Equals(texture))
            {
                return;
            }

            context.PixelShader.SetShaderResource(samplerSlot, texture.ResourceView);
            cachedTextures[samplerSlot] = texture;
        }

        /// <summary>
        /// Checks if the entity is in the current view frustum and draws its model accordingly.
        /// </summary>
        /// <param name="entity">Desired entity.</param>
        public void DrawEntity(Entity entity)
        {
            World = entity.Transform;
            context.UpdateSubresource(ref worldViewProjection, constantBuffer, 0);
            
            if (frustum.Contains(ref entity.BBox) == ContainmentType.Disjoint)
            {
                return;
            }

            SetActiveTexture(entity.Texture, 0);
            if (this.cachedModel != null && this.cachedModel.Equals(entity.Model))
            {
                context.InputAssembler.SetIndexBuffer(this.cachedModel.meshes[0].IndexBuffer, Format.R32_UInt, 0);
                context.DrawIndexed(this.cachedModel.meshes[0].IndexCount, 0, 0);
                return;
            }

            for (int i = 0; i < entity.Model.meshes.Count; i++)
            {
                ModelMesh currentMesh = entity.Model.meshes[i];

                context.InputAssembler.SetIndexBuffer(currentMesh.IndexBuffer, Format.R32_UInt, 0);
                context.InputAssembler.SetVertexBuffers(0, currentMesh.Binding);
                context.DrawIndexed(currentMesh.IndexCount, 0, 0);
            }

            this.cachedModel = entity.Model;
        }
    }
}
