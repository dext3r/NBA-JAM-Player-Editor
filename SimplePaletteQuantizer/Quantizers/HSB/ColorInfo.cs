using System;
using System.Drawing;

namespace SimplePaletteQuantizer.Quantizers.HSB
{
    /// <summary>
    /// Stores all the informations about single color only once, to be used later.
    /// </summary>
    internal struct ColorInfo
    {
        /// <summary>
        /// The original color.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// The pixel presence count in the image.
        /// </summary>
        public Int32 Count { get; private set; }

        /// <summary>
        /// A hue component of the color.
        /// </summary>
        public Single Hue { get; private set; }

        /// <summary>
        /// A saturation component of the color.
        /// </summary>
        public Single Saturation { get; private set; }

        /// <summary>
        /// A brightness component of the color.
        /// </summary>
        public Single Brightness { get; private set; }

        /// <summary>
        /// A cached hue hashcode.
        /// </summary>
        public Int32 HueHashCode { get; private set; }

        /// <summary>
        /// A cached saturation hashcode.
        /// </summary>
        public Int32 SaturationHashCode { get; private set; }

        /// <summary>
        /// A cached brightness hashcode.
        /// </summary>
        public Int32 BrightnessHashCode { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorInfo"/> struct.
        /// </summary>
        /// <param name="color">The color.</param>
        public ColorInfo(Color color)
            : this()
        {
            Color = color;
            Count = 1;

            Hue = color.GetHue();
            Saturation = color.GetSaturation();
            Brightness = color.GetBrightness();

            HueHashCode = Hue.GetHashCode();
            SaturationHashCode = Saturation.GetHashCode();
            BrightnessHashCode = Brightness.GetHashCode();
        }

        /// <summary>
        /// Increases the count of pixels of this color.
        /// </summary>
        public void IncreaseCount()
        {
            Count++;
        }
    }
}
