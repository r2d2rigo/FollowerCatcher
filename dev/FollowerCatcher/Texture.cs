using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FollowerCatcher
{
    /// <summary>
    /// Encapsulates a SharpDX texture and add additional managing functionality.
    /// </summary>
    public class Texture : IEquatable<Texture>
    {
        private Texture2D texture;

        private ShaderResourceView resourceView;
        public ShaderResourceView ResourceView { get { return this.resourceView; } }

        private string texturePath;
        public string TexturePath { get { return this.texturePath; } }

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Texture()
        {
        }

        /// <summary>
        /// Public constructor.
        /// </summary>
        /// <param name="filePath">Path to the texture.</param>
        /// <param name="shaderResource">Resource view for shader binding.</param>
        public Texture(string filePath, ShaderResourceView shaderResource)
        {
            this.texturePath = filePath;
            this.resourceView = shaderResource;
            this.texture = shaderResource.Resource as Texture2D;
        }

        /// <summary>
        /// Loads a new Texture from a file on disk.
        /// </summary>
        /// <param name="device">DirectX device.</param>
        /// <param name="filename">Path to the texture file.</param>
        /// <returns></returns>
        public static async Task<Texture> LoadFromFile(SharpDX.Direct3D11.Device1 device, string filename)
        {
            Texture newTexture = new Texture();

            newTexture.texturePath = filename;
            NativeFileStream fileStream = new NativeFileStream(filename, NativeFileMode.Open, NativeFileAccess.Read);

            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                int dataLength = reader.ReadInt32();
                byte[] data = new byte[dataLength];

                data = reader.ReadBytes(dataLength);

                DataStream stream = new DataStream(dataLength, false, true);
                stream.Write(data, 0, dataLength);

                newTexture.texture = new Texture2D(device, new Texture2DDescription()
                {
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.BC3_UNorm,
                    Height = height,
                    MipLevels = 1,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Immutable,
                    Width = width,
                }, new DataRectangle(stream.DataPointer, width * 4));

                newTexture.resourceView = new ShaderResourceView(device, newTexture.texture);
            }

            return newTexture;
        }

        /// <summary>
        /// Used to check for duplicate textures.
        /// </summary>
        /// <param name="other">Other texture.</param>
        /// <returns>True if both are equal, false otherwise.</returns>
        public bool Equals(Texture other)
        {
            if (other == null)
            {
                return false;
            }

            return this.texturePath.Equals(other.texturePath, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
