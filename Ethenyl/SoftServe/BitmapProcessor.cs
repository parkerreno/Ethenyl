using System;
using System.Drawing;
using System.Threading.Tasks;

namespace SoftServe
{
    public class BitmapProcessor
    {
        public static Color AverageColor(Bitmap b)
        {
            int r = 0;
            int g = 0;
            int bl = 0;
            int total = b.Height*b.Width;
            for (int x = 0; x < b.Width; x++)
            {
                for (int y = 0; y < b.Height; y++)
                {
                    var pixel = b.GetPixel(x, y);
                    r += pixel.R;
                    g += pixel.G;
                    bl += pixel.B;
                }
            }

            return Color.FromArgb(r/total, g/total, bl/total);
        }
        
        /// <summary>
        /// This is more the result of me playing around and shouldn't actually be used...  However, it tends to be quicker and provide a value close to the real one.
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        public static Color QuickAveragishColor(Bitmap bmp)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            var rand = new Random(DateTime.Now.Millisecond);
            int totalPixels = rand.Next(1, bmp.Height*bmp.Width/6);

            for (int i = 0; i < totalPixels; i++)
            {
                var pixel = bmp.GetPixel(rand.Next(0, bmp.Width), rand.Next(0, bmp.Height));
                r += pixel.R;
                g += pixel.G;
                b += pixel.B;
            }

            return Color.FromArgb(r / totalPixels, g / totalPixels, b / totalPixels);
        }
    }
}