using System;
using System.Drawing;

namespace SimplePaletteQuantizer.Quantizers.Popularity
{
    internal class PopularityColorSlot
    {
        private Int32 red;
        private Int32 green;
        private Int32 blue;

        /// <summary>
        /// Gets or sets the pixel count.
        /// </summary>
        /// <value>The pixel count.</value>
        public Int32 PixelCount { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PopularityColorSlot"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        public PopularityColorSlot(Color color)
        {
            AddValue(color);
        }

        #region | Methods |

        /// <summary>
        /// Adds the value to the slot.
        /// </summary>
        /// <param name="color">The color to be added.</param>
        public void AddValue(Color color)
        {
            red += color.R;
            green += color.G;
            blue += color.B;
            PixelCount++;
        }

        /// <summary>
        /// Gets the average, just simple value divided by pixel presence.
        /// </summary>
        /// <returns>The average color component value.</returns>
        public Color GetAverage()
        {
            Color result;

            if (PixelCount == 1)
            {
                result = Color.FromArgb(255, red, green, blue);
            }
            else
            {
                result = Color.FromArgb(255, red/PixelCount, green/PixelCount, blue/PixelCount);
            }

            return result;
        }

        #endregion
    }
}