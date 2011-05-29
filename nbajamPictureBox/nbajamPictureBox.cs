using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//NOT MY CODE \/ \/ \/ \/ \/ \/ 
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.HSB;
using SimplePaletteQuantizer.Quantizers.Median;
using SimplePaletteQuantizer.Quantizers.Octree;
using SimplePaletteQuantizer.Quantizers.Popularity;
using SimplePaletteQuantizer.Quantizers.Uniform;
//NOT MY CODE /\ /\ /\ /\ /\ /\
//Awesome project from here: http://www.codeproject.com/KB/recipes/SimplePaletteQuantizer.aspx



namespace nbajamPictureBox
{
    public partial class nbajamPictureBox : PictureBox
    {
        private IColorQuantizer the_quantizer = new PaletteQuantizer();         // Color quantizer object
        private Image sourceImage;  //load form function method


        // heres how it goes down
        // load an image file
        // jampicturebox.image = SOURCE FILE
        // shit auto quantizes and then draws (slow or fast, who cares? you dont!)
        // the in main program, gets the data from the control and you can
        // do whatever the fuck you want with it
        // - thank you


        private int data_size = 0;                                              // Length of raw data array
        private int palette_size = 0;                                           // Number of colors in the image
        private byte[] raw_data_bytes;                                          // Array of byte data which holds image data
        private Color[] palette;                                                // The color palette for the image

        private byte[,] backArray;                                              // An array to hold raw pixel data  
        private byte[,] new_back_array; //                                      // An array which holds the image data by palette value
        private Color[] optimized_palette = new Color[32];                      // Holds the converted image palette
        private byte[] new_color_pal = new byte[64]; //32 color @ 2 bytes/col
     //   private int[,] savedbackArray;

        private int tile_width = 6;                                             // Width of the control in 8px*8px tiles
        private int tile_height = 7;                                            // Height of the control in 8px*8px tiles
        private int scale_factor = 1; 

        public bool is4bpp = false;                                             // FOR FUTURE USE to pick between 4bpp and 5bpp de/encoding
                                                                                // assume 5bpp for now... (Other graphics, like team logos, may use 4bpp)
        public bool isPortrait = true;                                          // The data for a portrait contains an emebedded palette: provide for ability to process this data.

        private byte[,] single_tile;

        private bool redrawFlag = false;

        //methods
        //set/gets
        public int DataSize
        {
            get
            {
                return data_size;
            }
          
            set
            {
                data_size = value;
                //raw_data_bytes = new byte[data_size];
            }
        }
        public int PaletteSize
        {
            get
            {
                return palette_size;
            }

            set
            {
                palette_size = value;
              //  palette = new Color[palette_size];
            }
        }
        public int TilesWide
        {
            // Retrieves the value of the private variable tile_width
            get
            {
                return tile_width;
            }
            // Stores the value of number of tiles wide in variable tile_width
            set
            {
                tile_width = value;
                redrawFlag = true;
                this.Invalidate();
            }
        } 
        public int TilesHigh
        {
            // Retrieves the value of the private variable tile_height
            get
            {
                return tile_height;
            }
            // Stores the value of number of tiles wide in variable tile_height
            set
            {
                tile_height = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }
        public int ScaleFactor
        {
            // Retrieves the value of the private variable scale_factor
            get
            {
                return scale_factor;
            }
            // Stores the value of the scale factor of the image in variable scale factor
            // Resizes control based on the scale factor
            set
            {
                scale_factor = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }

        //Class constructor
        public nbajamPictureBox()
        {
            //raw_data_bytes = new byte[400];
           // palette = new Color[1];
        
            InitializeComponent();
        }
        //Base control class OnPaint override
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            byte[] layer = new byte[5];
            byte[] pixelbuilder = new byte[5];
            byte[] teh_pixels = new byte[64];
            Bitmap[] tiles = new Bitmap[42];
            Bitmap portraitBitmap = new Bitmap(scale_factor * 8 * tile_width , scale_factor*8*tile_height);

            Tile[] the_tiles = new Tile[tile_height * tile_width];

            for (int whocares = 0; whocares < tile_height * tile_width; whocares++)
            {
                the_tiles[whocares] = new Tile(8, 8);
            }

            int v = 0;
            int dumbcounter = 0;
    
          //  int pixelcounter = 0;
            int initial_offset = 0;
            int secondary_offset = initial_offset + 32;

            if (redrawFlag & raw_data_bytes != null)
            {
            
                backArray = new byte[8 * tile_width, 8 * tile_height];
                new_back_array = new byte[8 * tile_width, 8 * tile_height];

                if (raw_data_bytes.Length > 0)
                {

                    //    
                    for (int z = 0; z < (data_size - 64); z = z + 40)
                    {
                        the_tiles[v]  = get5bpptile(z);
                        v++;
                        //for (int y_tiles = 0; y_tiles < 8 * tile_height; y_tiles++)
                       // {
                         //   for (int x_tiles = 0; x_tiles < 8 * tile_width; x_tiles++)
                         //   {
                         // //      copyTiletoBackground(single_tile, (byte)x_tiles, (byte)y_tiles);
                         //   }
                       // }
                    }

                    for (int y = 0; y < 8* tile_height; y=y+8)
                    {
                        for (int x = 0; x < 8*tile_width; x=x+8)
                        {

                            copyTiletoBackground(the_tiles[dumbcounter],(byte)x,(byte)y);
                            dumbcounter++;
                        }
                    }
                }

                #region BLAST THIS GARBAGE OUT
                //         
                       // }
                 //   }
 
              //  }
                /* backArray = new byte[8 * tile_width, 8 * tile_height];
                savedbackArray = new int[8 * tile_width, 8 * tile_height];

                 this.Size = new Size(8 * tile_width * scale_factor, 8 * tile_height * scale_factor);

                for (int z = 0; z < 42; z++)
                {
                    //each tile has to be scaled based on the parent picturebox size...
                    tiles[z] = new Bitmap(8, 8);
                }
                //this gets 42 tiles

                if (raw_data_bytes.Length > 0)
                {
                 //   for (int total_tiles = 0; total_tiles < (tile_height * tile_width); total_tiles++)
                 //   {
  #region source
                        //the secondary_offset is 32 away from the initial_offset
                        //but only increments by 1 every loop pass
                       // secondary_offset = initial_offset + 32;

                        //debug
                        // Console.WriteLine("Tile #" + total_tiles.ToString());

                        //this gets a tiles worth of data
#endregion
                    for (int y_tiles = 0; y_tiles < tile_width; y_tiles++)
                    {
                        for (int x_tiles = 0; y_tiles < tile_height; x_tiles++)
                        {
                        //   single_tile = get5bpptile(initial_offset);
                            //copyTiletoBackground(single_tile, (byte)x_tiles, (byte)y_tiles);
                        }
                    }
 #region source
                        /*     for (int i = 0; i < 8; i++)
                        {
                            //debug
                            //Console.WriteLine(initial_offset.ToString() + "," + (initial_offset + 1).ToString() + "," + (initial_offset + 16).ToString() + "," + (initial_offset + 17).ToString() + "," + secondary_offset.ToString());

                            //this should go into loaddata
                            //drop in the initial offset, get a tile containing palette entries  (array of 64 bytes)
                            //increase initial offset by 2. repeat 
                            //just drop into the greater backArray

                            

                            //this should go into onpaint
                            //to draw, read from the backarray
                            //scale as neccesary (create bitmap sized as necessary, draw on it)
                         
                            layer[0] = raw_data_bytes[initial_offset];
                            layer[0] = (byte)(ReverseByteBits(layer[0]));
                            layer[1] = raw_data_bytes[initial_offset + 1];
                            layer[1] = (byte)(ReverseByteBits(layer[1]));
                            layer[2] = raw_data_bytes[initial_offset + 16];
                            layer[2] = (byte)(ReverseByteBits(layer[2]));
                            layer[3] = raw_data_bytes[initial_offset + 17];
                            layer[3] = (byte)(ReverseByteBits(layer[3]));
                            layer[4] = raw_data_bytes[secondary_offset];
                            layer[4] = (byte)(ReverseByteBits(layer[4]));


                            //flatten the layers here
                            for (int a = 0; a < 8; a++)
                            {
                                //these shifts allow us to just get the bit we are interested in
                                pixelbuilder[0] = (byte)((layer[0] >> a) & 1);
                                pixelbuilder[1] = (byte)((layer[1] >> a) & 1);
                                pixelbuilder[2] = (byte)((layer[2] >> a) & 1);
                                pixelbuilder[3] = (byte)((layer[3] >> a) & 1);
                                pixelbuilder[4] = (byte)((layer[4] >> a) & 1);
                                //this OR operation 'sandwiches' the layers into one color palette value
                                teh_pixels[pixelcounter] = (byte)((pixelbuilder[0]) | (pixelbuilder[1] << 1 | (pixelbuilder[2] << 2) | (pixelbuilder[3] << 3) | (pixelbuilder[4] << 4)));

                                //transfers pixels into a tile
                                //  tiles[total_tiles].SetPixel(a, i,Color.FromArgb(255,255,255) );
                                palette[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);
                                tiles[total_tiles].SetPixel(a, i, palette[teh_pixels[pixelcounter]]);
                                pixelcounter++;
                            }


                            initial_offset = initial_offset + 2;
                            secondary_offset++; //only increment this by 1 

                        }

                        //need to start right after last secondary_offset value
                        //and its already incremented for us, so just set
                        //the next starting position to the value of secondary_offset
                        initial_offset = secondary_offset;

                        //reset the pixel counter so it won't overflow pixel buffer bounds
                        pixelcounter = 0;
                  
#endregion  */
                //    }

                #endregion

                for (int y_tiles2 = 0; y_tiles2 < 8*tile_height; y_tiles2++)
                   {
                       for (int x_tiles2 = 0; x_tiles2 < 8*tile_width;  x_tiles2++)
                       {
                           Console.Write("{0:X2}",backArray[x_tiles2, y_tiles2]);
                           // this is also retarded /\/\/\/\ loluls
                           //i know this \/ \/ \/ \/ \/  is slow. dont care right now.
                           //TODO: Rewrite with LockBits
                            portraitBitmap.SetPixel(x_tiles2, y_tiles2, palette[backArray[x_tiles2,y_tiles2]] );
                       }
                        Console.WriteLine();
                    }
             
                    Console.WriteLine();
                    Console.WriteLine();              
                 
                    this.Image = portraitBitmap;

                }

                redrawFlag = false;
                
            }

        ////////////////////////////////////////////////
        // SNEStoRGB
        // Convert 15bit SNES BGR color to RGB color
        // 
        private Color SNEStoRGB(UInt16 snescolordata)
        {
            int red, green, blue;

            red = (snescolordata % 32) * 8;
            green = ((snescolordata / 32) % 32) * 8;
            blue = ((snescolordata / 1024) % 32) * 8;

            return System.Drawing.Color.FromArgb(red, green, blue);
        }

        //Convert RGB to 15bit SNES BGR color
        private UInt16 RGBtoSNES(Color inputColor)
        {
            UInt16 output = 0;
            int r, g, b;

            r = (int)inputColor.R / 8;
            g = (int)inputColor.G / 8;
            b = (int)inputColor.B / 8;

            output = (UInt16)(b * 1024 + g * 32 + r);

            return output;
        }

        //Load raw image data bytes
        public int loadImageData(byte[] data)
        {
            int bytesCopied = 0;
            int rgb_color = new UInt16();
            int palette_count = 0;

            single_tile = new byte[8,8];

            raw_data_bytes = new byte[data_size];
          //  backArray = new byte[8,8];

            if (raw_data_bytes != null)
            {
                for (int i = 0; i < data_size; i++)
                {
                    raw_data_bytes[i] = data[i];
                    bytesCopied++;
                }

                // If image is a portrait, load the color palette automatically

                // ?? lol why only if portrait? load a palette automatically
                // if not a portrait, load some random 32 colors??

                //ok if only portrait that means...something to do with the inital data. 
                //you werent really wrong here..
                palette = new Color[32];

                // First 64 bytes of the image data is the palette data
                if (isPortrait & (palette != null))
                {
                    //offset j to get the last 64 bytes of the data buffer
                    for (int j = 0x690; j < (64+0x690); j=j+2)
                    {

                        rgb_color = raw_data_bytes[j + 1];
                        rgb_color = rgb_color << 8;
                        rgb_color = rgb_color + raw_data_bytes[j];

                        palette[palette_count] = SNEStoRGB((ushort)rgb_color);
                        palette_count++;
                    }
                }



            }

            redrawFlag = true;
            this.Invalidate();
            return bytesCopied;

        }

        //Utility function to swap nibbles of a byte
        private byte swapByte(byte input)
        {
            byte output =0;
            byte copy;

            copy = input;

            //get bottom 4 bytes
            copy = (byte)(copy << 4);
            copy = (byte)(copy & (0xF0)); // get rid of any potential garbage

            //get top 4 bytes
            output =(byte)(input >> 4);
            output =(byte)(output & (byte)0x0F);
            
            //sammich time
            output = (byte)(copy |output );

            return output;
        }

        //Utility function to mirror byte
        public byte ReverseByteBits(byte value)
        {
            byte ret = 0;
            for (int i = 0; i < 8; i++)
                if ((value & (byte)(1 << i)) != 0) ret += (byte)(1 << (7 - i));
            return ret;
        }

        //Copy Tile object data to the background array
        private void copyTiletoBackground(Tile sourceTile, byte x_pos, byte y_pos)
        {
            for(int y=0;y<8;y++)
            {
                for (int x = 0; x<8 ; x++)
                {
                    backArray[x_pos + x, y_pos + y] = (byte)sourceTile.getPixel(x, y);
                }
            }
        }

        //Input data loction and returns a Tile object from said data
        private Tile get5bpptile(int initial_offset)
        {
            byte[,] tile = new byte[8, 8];
            byte[] layer = new byte[5];
            byte[] pixelbuilder = new byte[5];
            Tile the_tile = new Tile(8, 8);

            int secondary_offset = initial_offset + 32;

            for (int i = 0; i < 8; i++)
            {
                          
                layer[0] = raw_data_bytes[initial_offset];
                layer[0] = (byte)(ReverseByteBits(layer[0]));
                layer[1] = raw_data_bytes[initial_offset + 1];
                layer[1] = (byte)(ReverseByteBits(layer[1]));
                layer[2] = raw_data_bytes[initial_offset + 16];
                layer[2] = (byte)(ReverseByteBits(layer[2]));
                layer[3] = raw_data_bytes[initial_offset + 17];
                layer[3] = (byte)(ReverseByteBits(layer[3]));
                layer[4] = raw_data_bytes[secondary_offset];
                layer[4] = (byte)(ReverseByteBits(layer[4]));

                for (int a = 0; a < 8; a++)
                {
                    //these shifts allow us to just get the bit we are interested in
                    pixelbuilder[0] = (byte)((layer[0] >> a) & 1);
                    pixelbuilder[1] = (byte)((layer[1] >> a) & 1);
                    pixelbuilder[2] = (byte)((layer[2] >> a) & 1);
                    pixelbuilder[3] = (byte)((layer[3] >> a) & 1);
                    pixelbuilder[4] = (byte)((layer[4] >> a) & 1);
                    //this OR operation 'sandwiches' the layers into one color palette value
                    the_tile.SetPixel(a, i, (byte)((pixelbuilder[0]) | (pixelbuilder[1] << 1 | (pixelbuilder[2] << 2) | (pixelbuilder[3] << 3) | (pixelbuilder[4] << 4))));
                   // tile[a,i] = (byte)((pixelbuilder[0]) | (pixelbuilder[1] << 1 | (pixelbuilder[2] << 2) | (pixelbuilder[3] << 3) | (pixelbuilder[4] << 4)));
 
                }
                         
                initial_offset = initial_offset + 2;
                secondary_offset++; //only increment this by 1 
            }

            //need to start right after last secondary_offset value
            //and its already incremented for us, so just set
            //the next starting position to the value of secondary_offset
            initial_offset = secondary_offset;
                    
            return the_tile;
        }

        //Input image and returns a color quantized image (Hardcoded to 31 color - HSB Algorithm!)
        //31 color to allow for a transparent BG color
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
                     
                        //Ideas  to do the transparent color:
                        // make a mask
                        // AND the bytes 
                       the_quantizer.AddColor(color);
                        
                        duration += DateTime.Now - before;
                    }

                    // increases a source offset by a row
                    sourceOffset += sourceData.Stride;
                }

                //    editTargetInfo.Text = string.Format("Quantized: {0} colors (duration {1})", 256, duration); // TODO
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
                Int32 colorCount = 31; //GetColorCount();
                List<Color> palette = the_quantizer.GetPalette(colorCount);

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
                        targetBuffer[index] = (Byte)the_quantizer.GetPaletteIndex(color);
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
            // editSourceInfo.Text = string.Format("Original: {0} colors ({1} x {2})", activeQuantizer.GetColorCount(), image.Width, image.Height);

            // returns the quantized image
            return result;
        }

        //load an image file (JPG,PNG,etc) to quantize, convert 5bpp etc
        public void loadNewImage(Image input)
        {
            int pal_index = 0;
            int q = 0;
         


            sourceImage = input;        //copy the image, dont really need. TODO: Change

            the_quantizer.Clear();       //clear the quantizer
            this.Image = this.GetQuantizedImage(sourceImage);
            Bitmap gayness = (Bitmap)this.Image; // uber temporary hack thing to basically let us redraw the image using palette values

            List<Color> yourColorList = the_quantizer.GetPalette(32); // Get a list of the optimized color palette...
            ///... and copy it into an array? 
            foreach (Color color in yourColorList)   
            {
                optimized_palette[pal_index] = color;
                pal_index++;
            }

            // horrible hack because i know what to swap ... 
            Color temp = optimized_palette[0];
            optimized_palette[0] = optimized_palette[23];
            optimized_palette[23] = temp;
            // please fix this shit!

            //converts the palette to a SNES palette
            foreach (Color color in optimized_palette)
            {
                new_color_pal[q] = (byte)(RGBtoSNES(color));
                new_color_pal[q + 1] = (byte)(RGBtoSNES(color) >> 8);
            //    Console.WriteLine("Snes: " + new_color_pal[q].ToString("X2") + new_color_pal[q + 1].ToString("X2"));
                q = q + 2;
            }

            for (int y = 0; y < (8*tile_height); y++)
            {
                for (int x = 0; x < (8*tile_width); x++)
                {
                    new_back_array[x, y] = (byte)colorIndexLookup(gayness.GetPixel(x, y)); // uber temporary hack thing to basically let us redraw the image using palette values
                }
            }
                


        }

        private int colorIndexLookup(Color input)
        {
            int the_color = 0;

            for (int x = 0; x < 32; x++)
            {
                if (input == optimized_palette[x])
                    the_color = x;
            }

            return the_color;
        }

        // maybe add something to return all the linear array IF PORTRAIT = picture+pallete? or who gives a fuck
        public byte[] get5bppLinearArray()
        {
            byte[] flat_5bpp_array = new byte[tile_height*tile_width*40]; //fix this kinda...i mean 40 is here because we know 5bpp but what if not 5bpp?
            byte[] tile = new byte[40];

            int locationx = 0;
            int locationy = 0;
            int counter = 0;
            int offset = 0;
            int sec_offset = 32;

            for (int z = 0; z < 7; z++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tile = get5bpp(0, locationx, locationy);

                    for (int i = 0; i < 8; i++)
                    {
                        flat_5bpp_array[offset] = tile[(counter)];
                        flat_5bpp_array[offset + 1] = tile[(counter + 1)];
                        flat_5bpp_array[offset + 16] = tile[(counter + 2)];
                        flat_5bpp_array[offset + 17] = tile[(counter + 3)];
                        flat_5bpp_array[sec_offset] = tile[(counter + 4)];

                        offset = offset + 2;
                        sec_offset = sec_offset + 1;
                        counter = counter + 5;
                    }
                    locationx = locationx + 8;
                    counter = 0;
                    offset = offset + 24;
                    sec_offset = offset + 32;
                }
                locationx = 0;
                locationy = locationy + 8;
            }

            return flat_5bpp_array;
        }

        public byte[] getPortraitPalette()
        {
            return new_color_pal;
        }

        //returns byte[40] array
        private byte[] get5bpp(int address, int startx, int starty)
        {
            int offset = address;
            byte[] layers = new byte[5];
            byte[] temp = new byte[5];
            byte[] pixels = new byte[8];
            int counter = 0;

            byte[] tile = new byte[40];

            for (int y = 0; y < 8; y++)
            {
                for (int i = 0; i < 8; i++)
                {
                    pixels[i] = (byte)new_back_array[startx + i, starty + y];

                    temp[0] = (byte)((pixels[i] >> 0) & 1);
                    temp[1] = (byte)((pixels[i] >> 1) & 1);
                    temp[2] = (byte)((pixels[i] >> 2) & 1);
                    temp[3] = (byte)((pixels[i] >> 3) & 1);
                    temp[4] = (byte)((pixels[i] >> 4) & 1);

                    layers[0] = (byte)(layers[0] << 1);
                    layers[0] = (byte)(layers[0] | temp[0]);
                    layers[1] = (byte)(layers[1] << 1);
                    layers[1] = (byte)(layers[1] | temp[1]);
                    layers[2] = (byte)(layers[2] << 1);
                    layers[2] = (byte)(layers[2] | temp[2]);
                    layers[3] = (byte)(layers[3] << 1);
                    layers[3] = (byte)(layers[3] | temp[3]);
                    layers[4] = (byte)(layers[4] << 1);
                    layers[4] = (byte)(layers[4] | temp[4]);


                    tile[counter] = layers[0];
                    tile[counter + 1] = layers[1];
                    tile[counter + 2] = layers[2];
                    tile[counter + 3] = layers[3];
                    tile[counter + 4] = layers[4];
                }

                counter = counter + 5;
            }

            return tile;

        }

        //Class definition for Tile object
        public class Tile
        {
            public int Width;
            public int Height;
            private int[,] pixels;

            public Tile(int x, int y)
            {
                Width = x;
                Height = y;
                pixels = new int[x, y];
            }

            public void SetPixel(int x, int y, int value)
            {
                pixels[x, y] = value;
            }

            public int getPixel(int x, int y)
            {
                return pixels[x, y];
            }
        }
    }
}
      