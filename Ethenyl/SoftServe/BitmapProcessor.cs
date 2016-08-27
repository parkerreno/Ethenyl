using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using SpotifyAPI.Web.Models;
using Color = System.Drawing.Color;

namespace SoftServe
{
    /// <summary>
    /// Bitmap processor class (for album art)
    /// </summary>
    public class BitmapProcessor
    {
        /// <summary>
        /// Gets the mean color of the image
        /// </summary>
        /// <param name="bitmap">Bitmap image</param>
        /// <returns>Returns the mathematical mean color of the image</returns>
        public static Color AverageColor(Bitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int bl = 0;
            int total = bitmap.Height * bitmap.Width;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    r += pixel.R;
                    g += pixel.G;
                    bl += pixel.B;
                }
            }

            return Color.FromArgb(r / total, g / total, bl / total);
        }

        /// <summary>
        /// This is more the result of me playing around and shouldn't actually be used...  However, it tends to be quicker and provide a value close to the real one.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Color QuickAveragishColor(Bitmap bitmap)
        {
            int r = 0;
            int g = 0;
            int b = 0;

            var rand = new Random(DateTime.Now.Millisecond);
            int totalPixels = rand.Next(1, bitmap.Height * bitmap.Width / 6);

            for (int i = 0; i < totalPixels; i++)
            {
                var pixel = bitmap.GetPixel(rand.Next(0, bitmap.Width), rand.Next(0, bitmap.Height));
                r += pixel.R;
                g += pixel.G;
                b += pixel.B;
            }

            return Color.FromArgb(r / totalPixels, g / totalPixels, b / totalPixels);
        }



        public static Color MainColor(Bitmap bitmap)
        {
            Dictionary<RGB, int> savedColors = new Dictionary<RGB, int>();
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bool found = false;
                    var pixel = bitmap.GetPixel(x, y);
                    var thisRgb = new RGB() { Red = pixel.R, Blue = pixel.B, Green = pixel.G };
                    
                    List<RGB> toIncremement = new List<RGB>();

                    foreach (var c in savedColors.Keys)
                    {
                        if (c.IsSimilar(thisRgb))
                        {
                            toIncremement.Add(c);
                            found = true;
                        }
                    }
                    foreach (var c in toIncremement)
                    {
                        savedColors[c]++;
                    }
                    if (!found)
                        savedColors.Add(thisRgb, 1);
                }
            }

            var max = savedColors.OrderBy(x => x.Value).First();
            return Color.FromArgb(max.Key.Red, max.Key.Green, max.Key.Blue);
        }

        public static Color AveragesAreSometimesCool(Bitmap bitmap)
        {
            var avg = QuickAveragishColor(bitmap);
            var rgdiff = Math.Abs(avg.R - avg.G);
            var rbdiff = Math.Abs(avg.R - avg.B);
            var gbdiff = Math.Abs(avg.G - avg.B);

            if (rgdiff < 30 && rbdiff < 30 && gbdiff < 30)
            {
                return MainColor(bitmap);
            }
            else
            {
                return avg;
            }
        }

        private class RGB
        {
            public int Red;
            public int Green;
            public int Blue;

            public bool IsSimilar(RGB other)
            {
                if (other.Red >= Red - 3 && other.Red <= Red + 3 && other.Green >= Green - 3 &&
                    other.Green <= Green + 3 && other.Blue >= Blue - 3 && other.Blue <= Blue + 3)
                {
                    return true;
                }

                return false;
            }
        }
    }
}