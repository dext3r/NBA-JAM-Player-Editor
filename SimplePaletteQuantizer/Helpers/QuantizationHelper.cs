using System;
using System.Collections.Generic;
using System.Drawing;

namespace SimplePaletteQuantizer.Helpers
{
    public class QuantizationHelper
    {
        private static readonly Color BackgroundColor;
        private static readonly Double[] Factors;

        static QuantizationHelper()
        {
            BackgroundColor = SystemColors.Control;
            Factors = PrecalculateFactors();
        }

        /// <summary>
        /// Precalculates the alpha-fix values for all the possible alpha values (0-255).
        /// </summary>
        private static Double[] PrecalculateFactors()
        {
            Double[] result = new Double[256];

            for (Int32 value = 0; value < 256; value++)
            {
                result[value] = value / 255.0;
            }

            return result;
        }

        /// <summary>
        /// Converts the alpha blended color to a non-alpha blended color.
        /// </summary>
        /// <param name="color">The alpha blended color (ARGB).</param>
        /// <returns>The non-alpha blended color (RGB).</returns>
        internal static Color ConvertAlpha(Color color)
        {
            Color result = color;

            if (color.A < 255)
            {
                // performs a alpha blending (second color is BackgroundColor, by default a Control color)
                Double colorFactor = Factors[color.A];
                Double backgroundFactor = Factors[255 - color.A];
                Int32 red = (Int32) (color.R*colorFactor + BackgroundColor.R*backgroundFactor);
                Int32 green = (Int32) (color.G*colorFactor + BackgroundColor.G*backgroundFactor);
                Int32 blue = (Int32) (color.B*colorFactor + BackgroundColor.B*backgroundFactor);
                result = Color.FromArgb(255, red, green, blue);
            }

            return result;
        }

        /// <summary>
        /// Finds the closest color match in a given palette using Euclidean distance.
        /// </summary>
        /// <param name="color">The color to be matched.</param>
        /// <param name="palette">The palette to search in.</param>
        /// <returns>The palette index of the closest match.</returns>
        internal static Int32 GetNearestColor(Color color, IList<Color> palette)
        {
            // initializes the best difference, set it for worst possible, it can only get better
            Int32 bestIndex = 0;
            Int32 leastDifference = Int32.MaxValue;

            // goes thru all the colors in the palette, looking for the best match
            for (Int32 index = 0; index < palette.Count; index++)
            {
                Color targetColor = palette[index];

                // calculates a difference for all the color components
                Int32 deltaA = color.A - targetColor.A;
                Int32 deltaR = color.R - targetColor.R;
                Int32 deltaG = color.G - targetColor.G;
                Int32 deltaB = color.B - targetColor.B;

                // calculates a power of two
                Int32 factorA = deltaA * deltaA;
                Int32 factorR = deltaR * deltaR;
                Int32 factorG = deltaG * deltaG;
                Int32 factorB = deltaB * deltaB;

                // calculates the Euclidean distance, a square-root is not need 
                // as we're only comparing distance, not measuring it
                Int32 difference = factorA + factorR + factorG + factorB;

                // if a difference is zero, we're good because it won't get better
                if (difference == 0)
                {
                    bestIndex = index;
                    break;
                }

                // if a difference is the best so far, stores it as our best candidate
                if (difference < leastDifference)
                {
                    leastDifference = difference;
                    bestIndex = index;
                }
            }

            // returns the palette index of the most similar color
            return bestIndex;
        }
    }
}
