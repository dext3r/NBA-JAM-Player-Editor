using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SimplePaletteQuantizer.Quantizers.Median
{
    internal class MedianCutCube
    {
        // red bounds
        private Byte redLowBound;
        private Byte redHighBound;

        // green bounds
        private Byte greenLowBound;
        private Byte greenHighBound;

        // blue bounds
        private Byte blueLowBound;
        private Byte blueHighBound;

        private readonly List<Color> colorList;

        /// <summary>
        /// Gets or sets the index of the palette.
        /// </summary>
        /// <value>The index of the palette.</value>
        public Int32 PaletteIndex { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MedianCutCube"/> class.
        /// </summary>
        /// <param name="colors">The colors.</param>
        public MedianCutCube(List<Color> colors)
        {
            colorList = colors;
            Shrink();
        }

        #region | Calculated properties |

        /// <summary>
        /// Gets the size of the red side of this cube.
        /// </summary>
        /// <value>The size of the red side of this cube.</value>
        public Int32 RedSize
        {
            get { return redHighBound - redLowBound; }
        }

        /// <summary>
        /// Gets the size of the green side of this cube.
        /// </summary>
        /// <value>The size of the green side of this cube.</value>
        public Int32 GreenSize
        {
            get { return greenHighBound - greenLowBound; }
        }

        /// <summary>
        /// Gets the size of the blue side of this cube.
        /// </summary>
        /// <value>The size of the blue side of this cube.</value>
        public Int32 BlueSize
        {
            get { return blueHighBound - blueLowBound; }
        }

        /// <summary>
        /// Gets the average color from the colors contained in this cube.
        /// </summary>
        /// <value>The average color.</value>
        public Color Color
        {
            get
            {
                Int32 red = 0, green = 0, blue = 0;

                colorList.ForEach(value =>
                {
                    red += value.R;
                    green += value.G;
                    blue += value.B;
                });

                red = colorList.Count == 0 ? 0 : red / colorList.Count;
                green = colorList.Count == 0 ? 0 : green / colorList.Count;
                blue = colorList.Count == 0 ? 0 : blue / colorList.Count;

                Color result = Color.FromArgb(255, red, green, blue);
                return result;
            }
        }

        #endregion

        #region | Methods |

        /// <summary>
        /// Shrinks this cube to the least dimensions that covers all the colors in the RGB space.
        /// </summary>
        private void Shrink()
        {
            redLowBound = greenLowBound = blueLowBound = 255;
            redHighBound = greenHighBound = blueHighBound = 0;

            foreach (Color color in colorList)
            {
                if (color.R < redLowBound) redLowBound = color.R;
                if (color.R > redHighBound) redHighBound = color.R;
                if (color.G < greenLowBound) greenLowBound = color.G;
                if (color.G > greenHighBound) greenHighBound = color.G;
                if (color.B < blueLowBound) blueLowBound = color.B;
                if (color.B > blueHighBound) blueHighBound = color.B;
            }
        }

        /// <summary>
        /// Splits this cube's color list at median index, and returns two newly created cubes.
        /// </summary>
        /// <param name="componentIndex">Index of the component (red = 0, green = 1, blue = 2).</param>
        /// <param name="firstMedianCutCube">The first created cube.</param>
        /// <param name="secondMedianCutCube">The second created cube.</param>
        public void SplitAtMedian(Byte componentIndex, out MedianCutCube firstMedianCutCube, out MedianCutCube secondMedianCutCube)
        {
            List<Color> colors;

            switch (componentIndex)
            {
                // red colors
                case 0:
                    colors = colorList.OrderBy(color => color.R).ToList();
                    break;

                // green colors
                case 1:
                    colors = colorList.OrderBy(color => color.G).ToList();
                    break;

                // blue colors
                case 2:
                    colors = colorList.OrderBy(color => color.B).ToList();
                    break;

                default:
                    throw new NotSupportedException("Only three color components are supported (R, G and B).");

            }

            // retrieves the median index (a half point)
            Int32 medianIndex = colorList.Count >> 1;

            // creates the two half-cubes
            firstMedianCutCube = new MedianCutCube(colors.GetRange(0, medianIndex));
            secondMedianCutCube = new MedianCutCube(colors.GetRange(medianIndex, colors.Count - medianIndex));
        }

        /// <summary>
        /// Assigns a palette index to this cube, to be later found by a GetPaletteIndex method.
        /// </summary>
        /// <param name="newPaletteIndex">The palette index to be assigned to this cube.</param>
        public void SetPaletteIndex(Int32 newPaletteIndex)
        {
            PaletteIndex = newPaletteIndex;
        }

        /// <summary>
        /// Determines whether the color is in the space of this cube.
        /// </summary>
        /// <param name="color">The color to be checked, if it's contained in this cube.</param>
        /// <returns>if true a color is in the space of this cube, otherwise returns false.</returns>
        public Boolean IsColorIn(Color color)
        {
            return (color.R >= redLowBound && color.R <= redHighBound) &&
                   (color.G >= greenLowBound && color.G <= greenHighBound) &&
                   (color.B >= blueLowBound && color.B <= blueHighBound);
        }

        #endregion
    }
}
