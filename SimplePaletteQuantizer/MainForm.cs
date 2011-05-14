using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.HSB;
using SimplePaletteQuantizer.Quantizers.Median;
using SimplePaletteQuantizer.Quantizers.Octree;
using SimplePaletteQuantizer.Quantizers.Popularity;
using SimplePaletteQuantizer.Quantizers.Uniform;

namespace SimplePaletteQuantizer
{
    public partial class MainForm : Form
    {
        private Image gifImage;
        private Image sourceImage;
        private Boolean turnOnEvents;
        private Int32 projectedGifSize;
        private FileInfo sourceFileInfo;
        private IColorQuantizer activeQuantizer;
        private List<IColorQuantizer> quantizerList;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
        }

        #region | Update methods |

        private void UpdateImages()
        {
            // prepares quantizer
            activeQuantizer.Clear();

            // updates source image
            UpdateSourceImage();

            // tries to retrieve an image based on HSB quantization

            try
            {
                DateTime before = DateTime.Now;
                Image targetImage = GetQuantizedImage(sourceImage);
                TimeSpan duration = DateTime.Now - before;
                TimeSpan perPixel = new TimeSpan(duration.Ticks/(sourceImage.Width*sourceImage.Height));
                pictureTarget.Image = targetImage;

                // new GIF and PNG sizes
                Int32 newGifSize, newPngSize;

                // retrieves a GIF image based on our HSB-quantized one
                GetConvertedImage(targetImage, ImageFormat.Gif, out newGifSize);

                // retrieves a PNG image based on our HSB-quantized one
                GetConvertedImage(targetImage, ImageFormat.Png, out newPngSize);

                // spits out the statistics
                Text = string.Format("Simple palette quantizer (duration {0}, per pixel {1})", duration, perPixel);
                editProjectedGifSize.Text = projectedGifSize.ToString();
                editProjectedPngSize.Text = sourceFileInfo.Length.ToString();
                editNewGifSize.Text = newGifSize.ToString();
                editNewPngSize.Text = newPngSize.ToString();
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateSourceImage()
        {
            switch (listSource.SelectedIndex)
            {
                case 0:
                    pictureSource.Image = sourceImage;
                    break;

                case 1:
                    pictureSource.Image = gifImage;
                    break;

                default:
                    throw new NotSupportedException("Not expected!");
            }
        }

        #endregion

        #region | Methods |

        private void ChangeQuantizer()
        {
            activeQuantizer = quantizerList[listMethod.SelectedIndex];

            // turns off the color option for the uniform quantizer, as it doesn't make sense
            listColors.Enabled = turnOnEvents && listMethod.SelectedIndex != 1;

            if (listMethod.SelectedIndex == 1)
            {
                turnOnEvents = false;
                listColors.SelectedIndex = 7;
                turnOnEvents = true;
            }
        }

        private void EnableChoices()
        {
            listSource.Enabled = true;
            listMethod.Enabled = true;
            listColors.Enabled = listMethod.SelectedIndex != 1;
        }

        private void GenerateProjectedGif()
        {
            // retrieves a projected GIF image (automatic C# conversion)
            gifImage = GetConvertedImage(sourceImage, ImageFormat.Gif, out projectedGifSize);
        }

        private static Image GetConvertedImage(Image image, ImageFormat newFormat, out Int32 imageSize)
        {
            Image result;

            // saves the image to the stream, and then reloads it as a new image format; thus conversion.. kind of
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, newFormat);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                imageSize = (Int32)stream.Length;
                result = Image.FromStream(stream);
            }

            return result;
        }

        private Int32 GetColorCount()
        {
            switch (listColors.SelectedIndex)
            {
                case 0: return 2;
                case 1: return 4;
                case 2: return 8;
                case 3: return 16;
                case 4: return 32;
                case 5: return 64;
                case 6: return 128;
                case 7: return 256;

                default:
                    throw new NotSupportedException("Only 2, 4, 8, 16, 32, 64, 128 and 256 colors are supported.");
            }
        }

        private Image GetQuantizedImage(Image image)
        {
            // checks whether a source image is valid
            if (image == null)
            {
                const String message = "Cannot quantize a null image.";
                throw new ArgumentNullException(message);
            }

            // locks the source image data
            Bitmap bitmap = (Bitmap)image;
            Rectangle bounds = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);
            BitmapData sourceData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // prepares time statistics variables
            TimeSpan duration = new TimeSpan(0);
            DateTime before;

            try
            {
                // initalizes the pixel read buffer
                Int32[] sourceBuffer = new Int32[image.Width];

                // sets the offset to the first pixel in the image
                Int64 sourceOffset = sourceData.Scan0.ToInt64();

                for (Int32 row = 0; row < image.Height; row++)
                {
                    // copies the whole row of pixels to the buffer
                    Marshal.Copy(new IntPtr(sourceOffset), sourceBuffer, 0, image.Width);

                    // scans all the colors in the buffer
                    foreach (Color color in sourceBuffer.Select(argb => Color.FromArgb(argb)))
                    {
                        before = DateTime.Now;
                        activeQuantizer.AddColor(color);
                        duration += DateTime.Now - before;
                    }

                    // increases a source offset by a row
                    sourceOffset += sourceData.Stride;
                }

                editTargetInfo.Text = string.Format("Quantized: {0} colors (duration {1})", 256, duration); // TODO
            }
            catch
            {
                bitmap.UnlockBits(sourceData);
                throw;
            }

            Bitmap result = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

            // calculates the palette
            try
            {
                before = DateTime.Now;
                Int32 colorCount = GetColorCount();
                List<Color> palette = activeQuantizer.GetPalette(colorCount);

                // sets our newly calculated palette to the target image
                ColorPalette imagePalette = result.Palette;
                duration += DateTime.Now - before;

                for (Int32 index = 0; index < palette.Count; index++)
                {
                    imagePalette.Entries[index] = palette[index];
                }

                result.Palette = imagePalette;

            }
            catch (Exception)
            {
                bitmap.UnlockBits(sourceData);
                throw;
            }

            // locks the target image data
            BitmapData targetData = result.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            try
            {
                // initializes read/write buffers
                Byte[] targetBuffer = new Byte[result.Width];
                Int32[] sourceBuffer = new Int32[image.Width];

                // sets the offsets on the beginning of both source and target image
                Int64 sourceOffset = sourceData.Scan0.ToInt64();
                Int64 targetOffset = targetData.Scan0.ToInt64();

                for (Int32 row = 0; row < image.Height; row++)
                {
                    // reads the pixel row from the source image
                    Marshal.Copy(new IntPtr(sourceOffset), sourceBuffer, 0, image.Width);

                    // goes thru all the pixels, reads the color on the source image, and writes calculated palette index on the target
                    for (Int32 index = 0; index < image.Width; index++)
                    {
                        Color color = Color.FromArgb(sourceBuffer[index]);
                        before = DateTime.Now;
                        targetBuffer[index] = (Byte)activeQuantizer.GetPaletteIndex(color);
                        duration += DateTime.Now - before;
                    }

                    // writes the pixel row to the target image
                    Marshal.Copy(targetBuffer, 0, new IntPtr(targetOffset), result.Width);

                    // increases the offsets (on both images) by a row
                    sourceOffset += sourceData.Stride;
                    targetOffset += targetData.Stride;
                }
            }
            finally
            {
                // releases the locks on both images
                bitmap.UnlockBits(sourceData);
                result.UnlockBits(targetData);
            }

            // spits some duration statistics (those actually slow the processing quite a bit, turn them off to make it quicker)
            editSourceInfo.Text = string.Format("Original: {0} colors ({1} x {2})", activeQuantizer.GetColorCount(), image.Width, image.Height);

            // returns the quantized image
            return result;
        }

        #endregion

        #region << Events >>

        private void MainForm_Load(object sender, EventArgs e)
        {
            quantizerList = new List<IColorQuantizer>
            {
                new PaletteQuantizer(),
                new UniformQuantizer(),
                new PopularityQuantizer(),
                new MedianCutQuantizer(),
                new OctreeQuantizer()
            };

            turnOnEvents = false;
            
            listSource.SelectedIndex = 0;
            listMethod.SelectedIndex = 0;
            listColors.SelectedIndex = 7;

            ChangeQuantizer();

            turnOnEvents = true;
        }

        private void ButtonBrowseClick(object sender, EventArgs e)
        {
            if (dialogOpenFile.ShowDialog() == DialogResult.OK)
            {
                sourceFileInfo = new FileInfo(dialogOpenFile.FileName);
                sourceImage = Image.FromFile(dialogOpenFile.FileName);
                GenerateProjectedGif();
                EnableChoices();
                UpdateImages();
            }
        }

        private void ListSourceSelectedIndexChanged(object sender, EventArgs e)
        {
            if (turnOnEvents) UpdateSourceImage();
        }

        private void ListMethodSelectedIndexChanged(object sender, EventArgs e)
        {
            if (turnOnEvents)
            {
                ChangeQuantizer();
                UpdateImages();
            }
        }

        private void listColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (turnOnEvents) UpdateImages();
        }

        #endregion

        private void MainForm_Resize(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Color> yourColorList = quantizerList[0].GetPalette(32);

            foreach (Color color in yourColorList)
            {
               Console.WriteLine(color.ToString());
               Console.WriteLine(pictureTarget.Size.ToString());

                // add it to your CSV structure used for save operation here*/
            }
        }
    }
}
