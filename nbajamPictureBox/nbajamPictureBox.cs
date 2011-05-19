using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nbajamPictureBox
{
    public partial class nbajamPictureBox : PictureBox
    {

        private int data_size = 0;              // Length of raw data array
        private int palette_size = 0;           // Number of colors in the image
        private byte[] raw_data_bytes;          // Array of byte data which holds image data
        private Color[] palette;                // The color palette for the image

        private byte[,] backArray;               // An array to hold raw pixel data  
        private int[,] savedbackArray;

        private int tile_width = 6;            // Width of the control in 8px*8px tiles
        private int tile_height = 7;            // Height of the control in 8px*8px tiles
        private int scale_factor = 1; 

        public bool is4bpp = false;             // FOR FUTURE USE to pick between 4bpp and 5bpp de/encoding
                                                // assume 5bpp for now...
        public bool isPortrait = true;          // The data for a portrait contains an emebedded palette 
                                                // Provide for ability to process this data.

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

        private Color SNESColortoRGBColor(UInt16 snescolordata)
        {
            int red, green, blue;

            red = (snescolordata % 32) * 8;
            green = ((snescolordata / 32) % 32) * 8;
            blue = ((snescolordata / 1024) % 32) * 8;

            return System.Drawing.Color.FromArgb(red, green, blue);
        }

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

                        palette[palette_count] = SNESColortoRGBColor((ushort)rgb_color);
                        palette_count++;
                    }
                }



            }

            redrawFlag = true;
            this.Invalidate();
            return bytesCopied;

        }

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
    
            int pixelcounter = 0;
            int initial_offset = 0;
            int secondary_offset = initial_offset + 32;

            if (redrawFlag & raw_data_bytes != null)
            {
            
                backArray = new byte[8 * tile_width, 8 * tile_height];

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


                    for (int y_tiles2 = 0; y_tiles2 < 8*tile_height; y_tiles2++)
                   {
                       for (int x_tiles2 = 0; x_tiles2 < 8*tile_width;  x_tiles2++)
                       {
                           Console.Write("{0:X2}",backArray[x_tiles2, y_tiles2]);

                           //i know this \/ \/ \/ \/ \/  is slow. dont care right now.
                            portraitBitmap.SetPixel(x_tiles2, y_tiles2, palette[backArray[x_tiles2,y_tiles2]] );
                       }
                        Console.WriteLine();
                    }

             
                    Console.WriteLine();
                    Console.WriteLine();

                    
                    


                    this.Image = portraitBitmap;

                /*
                    Bitmap temp = portraitBitmap;
                    Bitmap bmap = (Bitmap)temp.Clone();
                    Graphics gr = Graphics.FromImage(bmap);
                    Rectangle rect = new Rectangle(0, 0, 8, 8);
                    //tiles[7].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    int tilecounter = 0;

                    for (int i = 0; i < 7; i++)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            rect.X = j * 8;
                            rect.Y = i * 8;
                            //  Console.WriteLine(j.ToString() + "," + i.ToString() + "[" + tilecounter.ToString() + "]");
                           // tiles[tilecounter].RotateFlip(RotateFlipType.RotateNoneFlipX);
                            gr.DrawImage(tiles[tilecounter], rect);
                            tilecounter++;
                        }

                    }

                    portraitBitmap = (Bitmap)bmap.Clone();
                    this.Image = portraitBitmap; */

                }

                redrawFlag = false;
                
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

        public nbajamPictureBox()
        {
            //raw_data_bytes = new byte[400];
           // palette = new Color[1];
        
            InitializeComponent();
        }

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
      