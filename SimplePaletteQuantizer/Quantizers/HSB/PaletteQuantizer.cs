using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SimplePaletteQuantizer.Helpers;

namespace SimplePaletteQuantizer.Quantizers.HSB
{
    /// <summary>
    /// This is my baby. Read more in the article on the Code Project:
    /// http://www.codeproject.com/KB/recipes/SimplePaletteQuantizer.aspx
    /// </summary>
    public class PaletteQuantizer : IColorQuantizer
    {
        private readonly List<Color> palette;
        private readonly Dictionary<Color, Int32> cache;
        private readonly Dictionary<Color, ColorInfo> colorMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteQuantizer"/> class.
        /// </summary>
        public PaletteQuantizer()
        {
            palette = new List<Color>();
            cache = new Dictionary<Color, Int32>();
            colorMap = new Dictionary<Color, ColorInfo>();
        }

        #region << IColorQuantizer >>

        /// <summary>
        /// Adds the color to quantizer, only unique colors are added.
        /// </summary>
        /// <param name="color">The color to be added.</param>
        public void AddColor(Color color)
        {
            // if alpha is higher then fully transparent, convert it to a RGB value for more precise processing
            color = QuantizationHelper.ConvertAlpha(color);
            ColorInfo value;

            if (colorMap.TryGetValue(color, out value))
            {
                value.IncreaseCount();
            }
            else
            {
                ColorInfo colorInfo = new ColorInfo(color);
                colorMap.Add(color, colorInfo);
            }
        }

        /// <summary>
        /// Gets the palette with a specified count of the colors.
        /// </summary>
        /// <param name="colorCount">The color count.</param>
        /// <returns></returns>
        public List<Color> GetPalette(Int32 colorCount)
        {
            palette.Clear();

            // lucky seed :)
            Random random = new Random(13);

            // shuffles the colormap
            IEnumerable<ColorInfo> colorInfoList = colorMap.
                OrderBy(entry => random.NextDouble()).
                Select(entry => entry.Value);

            // if there're less colors in the image then allowed, simply pass them all
            if (colorMap.Count > colorCount)
            {
                // solves the color quantization
                colorInfoList = SolveRootLevel(colorInfoList, colorCount);

                // if there're still too much colors, just snap them from the top))
                if (colorInfoList.Count() > colorCount)
                {
                    colorInfoList.OrderByDescending(colorInfo => colorInfo.Count);
                    colorInfoList = colorInfoList.Take(colorCount);
                }
            }

            // clears the hit cache
            cache.Clear();

            // adds the selected colors to a final palette
            palette.AddRange(colorInfoList.Select(colorInfo => colorInfo.Color));

            // returns our new palette
            return palette;
        }

        /// <summary>
        /// Gets the index of the palette for specific color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public Int32 GetPaletteIndex(Color color)
        {
            Int32 result;
            color = QuantizationHelper.ConvertAlpha(color);

            // checks whether color was already requested, in that case returns an index from a cache
            if (!cache.TryGetValue(color, out result))
            {
                // otherwise finds the nearest color
                result = QuantizationHelper.GetNearestColor(color, palette);
                cache[color] = result;
            }

            // returns a palette index
            return result; 
        }

        /// <summary>
        /// Gets the color count.
        /// </summary>
        /// <returns></returns>
        public Int32 GetColorCount()
        {
            return colorMap.Count;
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            // clears all the information
            cache.Clear();
            colorMap.Clear();
        }

        #endregion

        #region | Helper methods |

        /// <summary>
        /// Selects three lists, based on distinct values of each hue, saturation and brightness color
        /// components, in a single pass.
        /// </summary>
        private static void SelectDistinct(IEnumerable<ColorInfo> colors, out Dictionary<Single, ColorInfo> hueColors, out Dictionary<Single, ColorInfo> saturationColors, out Dictionary<Single, ColorInfo> brightnessColors)
        {
            hueColors = new Dictionary<Single, ColorInfo>();
            saturationColors = new Dictionary<Single, ColorInfo>();
            brightnessColors = new Dictionary<Single, ColorInfo>();

            foreach (ColorInfo colorInfo in colors)
            {
                if (!hueColors.ContainsKey(colorInfo.Hue))
                {
                    hueColors.Add(colorInfo.Hue, colorInfo);
                }

                if (!saturationColors.ContainsKey(colorInfo.Saturation))
                {
                    saturationColors.Add(colorInfo.Saturation, colorInfo);
                }

                if (!brightnessColors.ContainsKey(colorInfo.Brightness))
                {
                    brightnessColors.Add(colorInfo.Brightness, colorInfo);
                }
            }
        }

        private static IEnumerable<ColorInfo> SolveRootLevel(IEnumerable<ColorInfo> colors, Int32 colorCount)
        {
            // initializes the comparers based on hue, saturation and brightness (HSB color model)
            ColorHueComparer hueComparer = new ColorHueComparer();
            ColorSaturationComparer saturationComparer = new ColorSaturationComparer();
            ColorBrightnessComparer brightnessComparer = new ColorBrightnessComparer();

            // selects three palettes: 1) hue is unique, 2) saturation is unique, 3) brightness is unique
            Dictionary<Single, ColorInfo> hueColors, saturationColors, brightnessColors;
            SelectDistinct(colors, out hueColors, out saturationColors, out brightnessColors);

            // selects the palette (from those 3) which has the most colors, because an image has some details in that category)
            if (hueColors.Count > saturationColors.Count && hueColors.Count > brightnessColors.Count)
            {
                colors = Solve2ndLevel(colors, hueColors, saturationComparer, brightnessComparer, colorCount);
            }
            else if (saturationColors.Count > hueColors.Count && saturationColors.Count > brightnessColors.Count)
            {
                colors = Solve2ndLevel(colors, saturationColors, hueComparer, brightnessComparer, colorCount);
            }
            else
            {
                colors = Solve2ndLevel(colors, brightnessColors, hueComparer, saturationComparer, colorCount);
            }

            return colors;
        }

        /// <summary>
        /// If the color count is still high, determine which of the remaining color components 
        /// are prevalent, and filter all the non-distinct values of that color component.
        /// </summary>
        private static IEnumerable<ColorInfo> Solve2ndLevel(IEnumerable<ColorInfo> colors, Dictionary<Single, ColorInfo> defaultColors, IEqualityComparer<ColorInfo> firstComparer, IEqualityComparer<ColorInfo> secondComparer, Int32 colorCount)
        {
            IEnumerable<ColorInfo> result = colors;

            if (defaultColors.Count() > colorCount)
            {
                result = defaultColors.Select(entry => entry.Value);

                IEnumerable<ColorInfo> firstColors = result.Distinct(firstComparer);
                IEnumerable<ColorInfo> secondColors = result.Distinct(secondComparer);

                Int32 firstColorsCount = firstColors.Count();
                Int32 secondColorsCount = secondColors.Count();

                if (firstColorsCount > secondColors.Count())
                {
                    if (firstColorsCount > colorCount)
                    {
                        result = Solve3rdLevel(result, firstColors, secondComparer, colorCount);
                    }
                }
                else
                {
                    if (secondColorsCount > colorCount)
                    {
                        result = Solve3rdLevel(result, secondColors, firstComparer, colorCount);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// If the color count is still high even so, filter all the non-distinct values of the last color component.
        /// </summary>
        private static IEnumerable<ColorInfo> Solve3rdLevel(IEnumerable<ColorInfo> colors, IEnumerable<ColorInfo> defaultColors, IEqualityComparer<ColorInfo> comparer, Int32 colorCount)
        {
            IEnumerable<ColorInfo> result = colors;

            if (result.Count() > colorCount)
            {
                result = defaultColors;

                IEnumerable<ColorInfo> filteredColors = result.Distinct(comparer);

                if (filteredColors.Count() >= colorCount)
                {
                    result = filteredColors;
                }
            }

            return result;
        }

        #endregion

        #region | Helper classes (comparers) |

        /// <summary>
        /// Compares a hue components of a color info.
        /// </summary>
        private class ColorHueComparer : IEqualityComparer<ColorInfo>
        {
            public Boolean Equals(ColorInfo x, ColorInfo y)
            {
                return x.Hue == y.Hue;
            }

            public Int32 GetHashCode(ColorInfo color)
            {
                return color.HueHashCode;
            }
        }

        /// <summary>
        /// Compares a saturation components of a color info.
        /// </summary>
        private class ColorSaturationComparer : IEqualityComparer<ColorInfo>
        {
            public Boolean Equals(ColorInfo x, ColorInfo y)
            {
                return x.Saturation == y.Saturation;
            }

            public Int32 GetHashCode(ColorInfo color)
            {
                return color.SaturationHashCode;
            }
        }

        /// <summary>
        /// Compares a brightness components of a color info.
        /// </summary>
        private class ColorBrightnessComparer : IEqualityComparer<ColorInfo>
        {
            public Boolean Equals(ColorInfo x, ColorInfo y)
            {
                return x.Brightness == y.Brightness;
            }

            public Int32 GetHashCode(ColorInfo color)
            {
                return color.BrightnessHashCode;
            }
        }

        #endregion
    }
}


