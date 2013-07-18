using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ManagedSquish;
using System.IO;

namespace TextureCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: TextureCompiler.exe texture.png");
                return;
            }

            foreach (string filename in args)
            {
                List<byte> imageData = new List<byte>();
                Bitmap image = (Bitmap)Bitmap.FromFile(filename);

                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color c = image.GetPixel(j, i);
                        imageData.Add(c.R);
                        imageData.Add(c.G);
                        imageData.Add(c.B);
                        imageData.Add(c.A);
                    }
                }

                byte[] compressedData = SquishWrapper.CompressImage(imageData.ToArray(), image.Width, image.Height, SquishFlags.Dxt5);

                using (BinaryWriter writer = new BinaryWriter(File.OpenWrite(filename + ".dxt")))
                {
                    writer.Write(image.Width);
                    writer.Write(image.Height);
                    writer.Write(compressedData.Length);
                    writer.Write(compressedData);
                }
            }
        }
    }
}
