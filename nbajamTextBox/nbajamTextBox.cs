﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nbajamTextBox
{
    public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

    public class TextChangedEventArgs : EventArgs
    {

         public String NewValue { get; set; }

         public TextChangedEventArgs(String newValue)
        {
            NewValue = newValue;
        } 

       
    }
       
    public partial class nbajamTextBox : PictureBox
    {
        //event handler for changing text
        [Category("Action")]
        [Description("Fires when the text changes.")]
        public event TextChangedEventHandler textChanged;

        protected virtual void OntextChanged(TextChangedEventArgs e)
        {
            if (textChanged != null)
                textChanged(this, e);
        } 

        //Properties
        //font definition?
        //bit arrays?
        //palette definition?
        public enum FontColorOptions { Pallete_0, Pallete_1, Pallete_2, Pallete_3, Pallete_4, Pallete_5, Pallete_6, Pallete_7, Pallete_8, Pallete_9, Pallete_10, Pallete_11, Pallete_12, Pallete_13, Pallete_14, Pallete_15 };
        public enum TextJustifyOptions { Left, Right, Center, Manual };
        public enum TextAlignOptions { Top, Middle, Bottom, Manual };
        private int tile_width=14;                                                      // Width of the control in 8px*8px tiles
        private int tile_height = 1;                                                    // Height of the control in 8px*8px tiles
        private int scale_factor = 4;                                                   // Use the scale factor to maintain the control's physical size
        //private int fontColor = 1;                                                    // Color to use from the palette
        private String internal_text="";                                                // The text to display on the control  
        private System.Drawing.Color[] colorpalette = new System.Drawing.Color[16];     // A general Color palette
        fontTile[] letters = new fontTile[60];                                          // Object to hold the font data/color
        fontTile[] small_font = new fontTile[60];                                       // Smaller Font
        private Bitmap displayBitmap;                                                   // What the user sees on the control
        private Bitmap[] tiles;                                                         // create array of tiles based on known number of required tiles
        private int[,] backArray;                                                       // An array to hold raw pixel data (Need to optimise - use something instead of int...) 
        private int[,] savedbackArray;
        private bool redrawFlag = false;
        private int fontIndex = 0;
        private int offsetx = 0;
        private int offsety = 0;
      //  private int fontColor = 3;
        private Color colFColor; //experimental
        private FontColorOptions theFontColorOptions;
        private TextJustifyOptions theJustifyOptions;
        private TextAlignOptions theAlignOptions;
            
        //Methods
        //Get tile?
        //Get 4bpp array?

        //Property Methods
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
        public int FontNumber
        {
            get
            {
                return fontIndex;
            }
            set
            {
                fontIndex = value; 
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
                tile_height= value;
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
        public FontColorOptions FontColor
        {
            // Retrieves the value of the private variable tile_width
            get
            {
                return theFontColorOptions;
                
            }
            // Stores the value of number of tiles wide in variable tile_width
            set
            {
                theFontColorOptions = value;
            //    fontColor = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }
        public int TextOffsetX
        {
            // Retrieves the value of the private variable offsetx
            get
            {
                return offsetx;

            }
            // Stores the value of the text offset x
            set
            {
                offsetx = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }
        public int TextOffsetY
        {
            // Retrieves the value of the private variable offsetx
            get
            {
                return offsety;

            }
            // Stores the value of the text offset y
            set
            {
                offsety = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }
        public TextJustifyOptions TextJustify
        {
            // Retrieves the value of the private variable tile_width
            get
            {
                return theJustifyOptions;

            }
            // Stores the value of number of tiles wide in variable tile_width
            set
            {
                theJustifyOptions = value;
                //    fontColor = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }
        public TextAlignOptions TextAlign
        {
            // Retrieves the value of the private variable tile_width
            get
            {
                return theAlignOptions;

            }
            // Stores the value of number of tiles wide in variable tile_width
            set
            {
                theAlignOptions  = value;
                redrawFlag = true;
                this.Invalidate();
            }
        }

        [Browsable(true)]
        public override String Text
        {
            // Retrieves the value of the private variable internal_text
            get
            {
                return internal_text;
            }
            // Stores the text to display on the control in internal_text variable
            set
            {
                String newValue = value.ToUpper();
                String oldValue = internal_text;
                if (oldValue != newValue)
                {
                    OntextChanged(new TextChangedEventArgs(newValue));
                  //  Console.WriteLine("CHANGED from" + oldValue + " to " + newValue +" -Obama");
                   //extChangedEventArgs textChanged = new TextChangedEventArgs(oldValue, newValue);
                  //textChanged(this,txt);
                  // 
                 //   Change(this, textChanged);
                }
                internal_text = value.ToUpper();
                
             //   String oldValue = internal_text;
             //   if (oldValue != internal_text)
              //      OnTextChanged(new TextChangedEventArgs(oldValue, internal_text));
                redrawFlag = true;
                this.Invalidate();
            }
        }

        public nbajamTextBox()
        { 
            scale_factor= ScaleFactor;
            tile_width=TilesWide ;                                                 
            tile_height=  TilesHigh ;
            internal_text = this.Text;
            theFontColorOptions = FontColorOptions.Pallete_3; 

            InitializeComponent();
            InitializePalette();
            InitializeFont();

           
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            this.Size = new Size(8 * tile_width * scale_factor, 8 * tile_height * scale_factor);

            if (redrawFlag)
            {
                // Precalculate these values for future use
                int totalPixelsWide = (8 * scale_factor * tile_width);
                int totalPixelsHigh = (8 * scale_factor * tile_height);

                // Create new bitmap to hold the final displayed bitmap
                displayBitmap = new Bitmap(totalPixelsWide, totalPixelsHigh);
                // Define pixel array based on number of pixels actually needed
                backArray = new int[8 * tile_width, 8 * tile_height];
                savedbackArray = new int[8 * tile_width, 8 * tile_height];
                // Create new tiles based on amount needed
                tiles = new Bitmap[tile_width * tile_height];

                int z;
                int f = 0;
                int[] textname = new int[internal_text.Length];
                int total_tiles = tile_height * tile_width;
                int text_size = 0;

                byte[][] tiletest = new byte[tile_width * tile_height][];
                // ^^ some kind of array that holds tile info...not sure why actually needed

                // Initialize each tile in the array to the proper size based on the scale factor
                for (int i = 0; i < (total_tiles); i++)
                {
                    tiles[i] = new Bitmap(scale_factor * tile_width, scale_factor * tile_height);
                    tiletest[i] = new byte[64];
                }

                //fill array "textname" with offset adjusted character values
                foreach (char c in internal_text)
                {
                    z = (c - 32); //65 = A
                    textname[f] = z;
                    f++;
                }

                //get the total width of the text
                foreach (int fu in textname)
                {
                    if (fontIndex == 0)
                    {
                        if (letters[fu] != null)
                            text_size = text_size + letters[fu].Width;
                    }
                    if (fontIndex == 1)
                    {
                        if (small_font[fu] != null)
                            text_size = text_size + small_font[fu].Width+1;
                    }
                }

                int locationx = 0;
                int locationy = 0;

                //take care of X coord
                switch (theJustifyOptions)
                {
                        
                    case TextJustifyOptions.Left:
                        // I guess don't really do anything...
                        break;
                        //right
                    case TextJustifyOptions.Right:
                        text_size = tile_width * 8 - (text_size);
                        locationx = text_size;
                        break;
                    case TextJustifyOptions.Center:
                        if(fontIndex==0)
                            text_size = (tile_width * 8 - (text_size)) / 2;
                        if(fontIndex==1)
                            text_size = ((tile_width * 8 - (text_size)) / 2)+1;
                        locationx = text_size;
                        break;
                    case TextJustifyOptions.Manual:
                        locationx = offsetx;
                  //      locationy = offsety;
                        break;         
                }

                //take care of Y coord
                switch (theAlignOptions)
                {
                    case TextAlignOptions.Top:
                        //nothin
                        break;
                    case TextAlignOptions.Middle:
                        if (fontIndex == 0)
                            text_size = (tile_height * 8 - 8) / 2;
                        if (fontIndex == 1)
                            text_size = ((tile_height * 8 - 5) / 2);
                        locationy = text_size;
                        break;
                    case TextAlignOptions.Bottom:
                        if (fontIndex == 0)
                            text_size = (tile_height * 8 - 8);
                        if (fontIndex == 1)
                            text_size = (tile_height * 8 - 5);
                        locationy = text_size;
                        break;
                    case TextAlignOptions.Manual:
                        locationy = offsety;
                        break;
                }


                //copy each letter into the background array
                foreach (int fu in textname)
                {
                    if (fontIndex == 0)
                    {
                        if (letters[fu] != null)
                        {
                            for (int y = 0; y < letters[fu].Height; y++)
                            {
                                for (int x = 0; x < letters[fu].Width; x++)
                                {
                                    if (locationx + x < (8 * tile_width))
                                        if(locationy + y < (8 * tile_height))
                                        backArray[locationx + x, locationy + y] = letters[fu].getPixel(x, y);
                                }
                            }
                            //increase the x location after the letter is copied
                            locationx = locationx + letters[fu].Width;
                        }
                    }

                    if (fontIndex == 1)
                    {
                        if (small_font[fu] != null)
                        {
                            for (int y = 0; y < small_font[fu].Height; y++)
                            {

                                for (int x = 0; x < small_font[fu].Width; x++)
                                {
                                    if (locationx + x < (8 * tile_width))
                                        if (locationy + y < (8 * tile_height))
                                        backArray[locationx + x, locationy + y] = small_font[fu].getPixel(x, y);
                                }
                            }
                            //increase the x location after the letter is copied
                            locationx = locationx + small_font[fu].Width+1;
                        }
                    }
                }

                int rodcounter = 0;

                //get an individual tile from a background array
                for (int q = 0; q < (8 * tile_height); q = q + 8)
                {
                    for (int v = 0; v < (8 * tile_width); v = v + 8)
                    {
                        //getTileFromArray returns a 64 byte array
                        tiletest[rodcounter] = getTileFromArray(backArray, v, q);
                        rodcounter++;
                    }
                }

                for (int s = 0; s < total_tiles; s++)
                {

                    // tiletest[s] = getTileFromArray(backArray, 0, 0); 
                    tiles[s] = tile2bitmap(tiletest[s], colorpalette, scale_factor * 8, scale_factor * 8);
                }
                int countur = 0;

                Rectangle rect = new Rectangle(0, 0, 8 * scale_factor, 8 * scale_factor);
                Bitmap temps = displayBitmap;
                Bitmap bmap = (Bitmap)temps.Clone();
                Graphics gr = Graphics.FromImage(bmap);

                for (int i = 0; i < tile_height; i++)
                {
                    for (int j = 0; j < tile_width; j++)
                    {
                        rect.X = j * 8 * scale_factor;
                        rect.Y = i * 8 * scale_factor;
                        //Console.WriteLine(j.ToString() + "," + i.ToString() + "[" + tilecounter.ToString() + "]");
                        // tiles[countur].RotateFlip(RotateFlipType.RotateNoneFlipX);

                        gr.DrawImage(tiles[countur], rect);

                        countur++;
                    }
                }

                displayBitmap = (Bitmap)bmap.Clone();

                this.Image = displayBitmap;
                // nbajamTextBox1.Image = nametagBitmap;

                for (int q = 0; q < 8 * tile_height; q++)
                {
                    for (int v = 0; v < 8 * tile_width; v++)
                    {
                        savedbackArray[v, q] = backArray[v, q]; 
                        backArray[v, q] = 0; //clears background array. 

                    }
                }
            }

            redrawFlag = false;
        }
        private void InitializeFont()
        {
            //Define height and width for each letter
            #region Big Font
            #region Height and Width Definitions
            //punctuation
            letters[0] = new fontTile(4, 8);
            letters[1] = new fontTile(2, 8);
            letters[14] = new fontTile(2, 8);
            letters[26] = new fontTile(2, 8);
            //numbers
            letters[16] = new fontTile(5,8);
            letters[17] = new fontTile(4,8);
            letters[18] = new fontTile(5,8);
            letters[19] = new fontTile(5,8);
            letters[20] = new fontTile(4,8);
            letters[21] = new fontTile(5,8);
            letters[22] = new fontTile(5,8);
            letters[23] = new fontTile(5,8);
            letters[24] = new fontTile(5,8);
            letters[25] = new fontTile(5,8); 
            //letters
            letters[33] = new fontTile(4, 8);
            letters[34] = new fontTile(4, 8);
            letters[35] = new fontTile(4, 8);
            letters[36] = new fontTile(4, 8);
            letters[37] = new fontTile(4, 8);
            letters[38] = new fontTile(4, 8);
            letters[39] = new fontTile(4, 8);
            letters[40] = new fontTile(4, 8);
            letters[41] = new fontTile(2, 8);
            letters[42] = new fontTile(4, 8);
            letters[43] = new fontTile(4, 8);
            letters[44] = new fontTile(4, 8);
            letters[45] = new fontTile(6, 8);
            letters[46] = new fontTile(4, 8);
            letters[47] = new fontTile(4, 8);
            letters[48] = new fontTile(4, 8);
            letters[49] = new fontTile(4, 8);
            letters[50] = new fontTile(4, 8);
            letters[51] = new fontTile(4, 8);
            letters[52] = new fontTile(4, 8);
            letters[53] = new fontTile(4, 8);
            letters[54] = new fontTile(4, 8);
            letters[55] = new fontTile(6, 8);
            letters[56] = new fontTile(4, 8);
            letters[57] = new fontTile(4, 8);
            letters[58] = new fontTile(4, 8);
            #endregion

            //Letters defined in the following regions
            //SetPixel(x,y, [COLOR VALUE FROM PALETTE])
            #region A
            letters[33].SetPixel(0 ,0 ,0);
            letters[33].SetPixel(1 ,0 ,3);
            letters[33].SetPixel(2 ,0 ,0);
            letters[33].SetPixel(3 ,0 ,0);

            letters[33].SetPixel(0, 1, 3);
            letters[33].SetPixel(1, 1, 3);
            letters[33].SetPixel(2, 1, 3);
            letters[33].SetPixel(3, 1, 0);

            letters[33].SetPixel(0, 2, 3);
            letters[33].SetPixel(1, 2, 10);
            letters[33].SetPixel(2, 2, 3);
            letters[33].SetPixel(3, 2, 10);

            letters[33].SetPixel(0, 3, 3);
            letters[33].SetPixel(1, 3, 10);
            letters[33].SetPixel(2, 3, 3);
            letters[33].SetPixel(3, 3, 10);

            letters[33].SetPixel(0, 4, 3);
            letters[33].SetPixel(1, 4, 3);
            letters[33].SetPixel(2, 4, 3);
            letters[33].SetPixel(3, 4, 10);

            letters[33].SetPixel(0, 5, 3);
            letters[33].SetPixel(1, 5, 10);
            letters[33].SetPixel(2, 5, 3);
            letters[33].SetPixel(3, 5, 10);

            letters[33].SetPixel(0, 6, 3);
            letters[33].SetPixel(1, 6, 10);
            letters[33].SetPixel(2, 6, 3);
            letters[33].SetPixel(3, 6, 10);

            letters[33].SetPixel(0, 7, 0);
            letters[33].SetPixel(1, 7, 10);
            letters[33].SetPixel(2, 7, 0);
            letters[33].SetPixel(3, 7, 10);    
#endregion
            #region B
            letters[34].SetPixel(0, 0, 3);
            letters[34].SetPixel(1, 0, 3);
            letters[34].SetPixel(2, 0, 0);
            letters[34].SetPixel(3, 0, 0);

            letters[34].SetPixel(0, 1, 3);
            letters[34].SetPixel(1, 1, 3);
            letters[34].SetPixel(2, 1, 3);
            letters[34].SetPixel(3, 1, 0);

            letters[34].SetPixel(0, 2, 3);
            letters[34].SetPixel(1, 2, 10);
            letters[34].SetPixel(2, 2, 3);
            letters[34].SetPixel(3, 2, 10);

            letters[34].SetPixel(0, 3, 3);
            letters[34].SetPixel(1, 3, 3);
            letters[34].SetPixel(2, 3, 0);
            letters[34].SetPixel(3, 3, 10);

            letters[34].SetPixel(0, 4, 3);
            letters[34].SetPixel(1, 4, 10);
            letters[34].SetPixel(2, 4, 3);
            letters[34].SetPixel(3, 4, 0);

            letters[34].SetPixel(0, 5, 3);
            letters[34].SetPixel(1, 5, 3);
            letters[34].SetPixel(2, 5, 3);
            letters[34].SetPixel(3, 5, 10);

            letters[34].SetPixel(0, 6, 3);
            letters[34].SetPixel(1, 6, 3);
            letters[34].SetPixel(2, 6, 10);
            letters[34].SetPixel(3, 6, 10);

            letters[34].SetPixel(0, 7, 0);
            letters[34].SetPixel(1, 7, 10);
            letters[34].SetPixel(2, 7, 10);
            letters[34].SetPixel(3, 7, 0);
#endregion
            #region C
            letters[35].SetPixel(0, 0, 0);
            letters[35].SetPixel(1, 0, 3);
            letters[35].SetPixel(2, 0, 3);
            letters[35].SetPixel(3, 0, 0);

            letters[35].SetPixel(0, 1, 3);
            letters[35].SetPixel(1, 1, 3);
            letters[35].SetPixel(2, 1, 3);
            letters[35].SetPixel(3, 1, 10);

            letters[35].SetPixel(0, 2, 3);
            letters[35].SetPixel(1, 2, 10);
            letters[35].SetPixel(2, 2, 10);
            letters[35].SetPixel(3, 2, 10);

            letters[35].SetPixel(0, 3, 3);
            letters[35].SetPixel(1, 3, 10);
            letters[35].SetPixel(2, 3, 0);
            letters[35].SetPixel(3, 3, 0);

            letters[35].SetPixel(0, 4, 3);
            letters[35].SetPixel(1, 4, 10);
            letters[35].SetPixel(2, 4, 0);
            letters[35].SetPixel(3, 4, 0);

            letters[35].SetPixel(0, 5, 3);
            letters[35].SetPixel(1, 5, 3);
            letters[35].SetPixel(2, 5, 3);
            letters[35].SetPixel(3, 5, 0);

            letters[35].SetPixel(0, 6, 0);
            letters[35].SetPixel(1, 6, 3);
            letters[35].SetPixel(2, 6, 3);
            letters[35].SetPixel(3, 6, 10);

            letters[35].SetPixel(0, 7, 0);
            letters[35].SetPixel(1, 7, 0);
            letters[35].SetPixel(2, 7, 10);
            letters[35].SetPixel(3, 7, 10);
            #endregion  
            #region D
            letters[36].SetPixel(0, 0, 3);
            letters[36].SetPixel(1, 0, 3);
            letters[36].SetPixel(2, 0, 0);
            letters[36].SetPixel(3, 0, 0);

            letters[36].SetPixel(0, 1, 3);
            letters[36].SetPixel(1, 1, 3);
            letters[36].SetPixel(2, 1, 3);
            letters[36].SetPixel(3, 1, 0);

            letters[36].SetPixel(0, 2, 3);
            letters[36].SetPixel(1, 2, 10);
            letters[36].SetPixel(2, 2, 3);
            letters[36].SetPixel(3, 2, 10);

            letters[36].SetPixel(0, 3, 3);
            letters[36].SetPixel(1, 3, 10);
            letters[36].SetPixel(2, 3, 3);
            letters[36].SetPixel(3, 3, 10);

            letters[36].SetPixel(0, 4, 3);
            letters[36].SetPixel(1, 4, 10);
            letters[36].SetPixel(2, 4, 3);
            letters[36].SetPixel(3, 4, 10);

            letters[36].SetPixel(0, 5, 3);
            letters[36].SetPixel(1, 5, 3);
            letters[36].SetPixel(2, 5, 3);
            letters[36].SetPixel(3, 5, 10);

            letters[36].SetPixel(0, 6, 3);
            letters[36].SetPixel(1, 6, 3);
            letters[36].SetPixel(2, 6, 10);
            letters[36].SetPixel(3, 6, 10);

            letters[36].SetPixel(0, 7, 0);
            letters[36].SetPixel(1, 7, 10);
            letters[36].SetPixel(2, 7, 10);
            letters[36].SetPixel(3, 7, 0);
            #endregion
            #region E
            letters[37].SetPixel(0, 0, 3);
            letters[37].SetPixel(1, 0, 3);
            letters[37].SetPixel(2, 0, 3);
            letters[37].SetPixel(3, 0, 0);

            letters[37].SetPixel(0, 1, 3);
            letters[37].SetPixel(1, 1, 3);
            letters[37].SetPixel(2, 1, 3);
            letters[37].SetPixel(3, 1, 10);

            letters[37].SetPixel(0, 2, 3);
            letters[37].SetPixel(1, 2, 10);
            letters[37].SetPixel(2, 2, 10);
            letters[37].SetPixel(3, 2, 10);

            letters[37].SetPixel(0, 3, 3);
            letters[37].SetPixel(1, 3, 3);
            letters[37].SetPixel(2, 3, 3);
            letters[37].SetPixel(3, 3, 0);

            letters[37].SetPixel(0, 4, 3);
            letters[37].SetPixel(1, 4, 10);
            letters[37].SetPixel(2, 4, 10);
            letters[37].SetPixel(3, 4, 10);

            letters[37].SetPixel(0, 5, 3);
            letters[37].SetPixel(1, 5, 3);
            letters[37].SetPixel(2, 5, 3);
            letters[37].SetPixel(3, 5, 0);

            letters[37].SetPixel(0, 6, 3);
            letters[37].SetPixel(1, 6, 3);
            letters[37].SetPixel(2, 6, 3);
            letters[37].SetPixel(3, 6, 10);

            letters[37].SetPixel(0, 7, 0);
            letters[37].SetPixel(1, 7, 10);
            letters[37].SetPixel(2, 7, 10);
            letters[37].SetPixel(3, 7, 10);
            #endregion
            #region F
            letters[38].SetPixel(0, 0, 3);
            letters[38].SetPixel(1, 0, 3);
            letters[38].SetPixel(2, 0, 3);
            letters[38].SetPixel(3, 0, 0);

            letters[38].SetPixel(0, 1, 3);
            letters[38].SetPixel(1, 1, 3);
            letters[38].SetPixel(2, 1, 3);
            letters[38].SetPixel(3, 1, 10);

            letters[38].SetPixel(0, 2, 3);
            letters[38].SetPixel(1, 2, 10);
            letters[38].SetPixel(2, 2, 10);
            letters[38].SetPixel(3, 2, 10);

            letters[38].SetPixel(0, 3, 3);
            letters[38].SetPixel(1, 3, 3);
            letters[38].SetPixel(2, 3, 3);
            letters[38].SetPixel(3, 3, 0);

            letters[38].SetPixel(0, 4, 3);
            letters[38].SetPixel(1, 4, 10);
            letters[38].SetPixel(2, 4, 10);
            letters[38].SetPixel(3, 4, 10);

            letters[38].SetPixel(0, 5, 3);
            letters[38].SetPixel(1, 5, 10);
            letters[38].SetPixel(2, 5, 0);
            letters[38].SetPixel(3, 5, 0);

            letters[38].SetPixel(0, 6, 3);
            letters[38].SetPixel(1, 6, 10);
            letters[38].SetPixel(2, 6, 0);
            letters[38].SetPixel(3, 6, 0);

            letters[38].SetPixel(0, 7, 0);
            letters[38].SetPixel(1, 7, 10);
            letters[38].SetPixel(2, 7, 0);
            letters[38].SetPixel(3, 7, 0);
            #endregion
            #region G
            letters[39].SetPixel(0, 0, 0);
            letters[39].SetPixel(1, 0, 3);
            letters[39].SetPixel(2, 0, 3);
            letters[39].SetPixel(3, 0, 0);

            letters[39].SetPixel(0, 1, 3);
            letters[39].SetPixel(1, 1, 3);
            letters[39].SetPixel(2, 1, 3);
            letters[39].SetPixel(3, 1, 10);

            letters[39].SetPixel(0, 2, 3);
            letters[39].SetPixel(1, 2, 10);
            letters[39].SetPixel(2, 2, 10);
            letters[39].SetPixel(3, 2, 10);

            letters[39].SetPixel(0, 3, 3);
            letters[39].SetPixel(1, 3, 10);
            letters[39].SetPixel(2, 3, 3);
            letters[39].SetPixel(3, 3, 0);

            letters[39].SetPixel(0, 4, 3);
            letters[39].SetPixel(1, 4, 10);
            letters[39].SetPixel(2, 4, 3);
            letters[39].SetPixel(3, 4, 10);

            letters[39].SetPixel(0, 5, 3);
            letters[39].SetPixel(1, 5, 3);
            letters[39].SetPixel(2, 5, 3);
            letters[39].SetPixel(3, 5, 10);

            letters[39].SetPixel(0, 6, 0);
            letters[39].SetPixel(1, 6, 3);
            letters[39].SetPixel(2, 6, 3);
            letters[39].SetPixel(3, 6, 10);

            letters[39].SetPixel(0, 7, 0);
            letters[39].SetPixel(1, 7, 0);
            letters[39].SetPixel(2, 7, 10);
            letters[39].SetPixel(3, 7, 10);
            #endregion
            #region H
            letters[40].SetPixel(0, 0, 3);
            letters[40].SetPixel(1, 0, 0);
            letters[40].SetPixel(2, 0, 3);
            letters[40].SetPixel(3, 0, 0);

            letters[40].SetPixel(0, 1, 3);
            letters[40].SetPixel(1, 1, 10);
            letters[40].SetPixel(2, 1, 3);
            letters[40].SetPixel(3, 1, 10);

            letters[40].SetPixel(0, 2, 3);
            letters[40].SetPixel(1, 2, 10);
            letters[40].SetPixel(2, 2, 3);
            letters[40].SetPixel(3, 2, 10);

            letters[40].SetPixel(0, 3, 3);
            letters[40].SetPixel(1, 3, 3);
            letters[40].SetPixel(2, 3, 3);
            letters[40].SetPixel(3, 3, 10);

            letters[40].SetPixel(0, 4, 3);
            letters[40].SetPixel(1, 4, 10);
            letters[40].SetPixel(2, 4, 3);
            letters[40].SetPixel(3, 4, 10);

            letters[40].SetPixel(0, 5, 3);
            letters[40].SetPixel(1, 5, 10);
            letters[40].SetPixel(2, 5, 3);
            letters[40].SetPixel(3, 5, 10);

            letters[40].SetPixel(0, 6, 3);
            letters[40].SetPixel(1, 6, 10);
            letters[40].SetPixel(2, 6, 3);
            letters[40].SetPixel(3, 6, 10);

            letters[40].SetPixel(0, 7, 0);
            letters[40].SetPixel(1, 7, 10);
            letters[40].SetPixel(2, 7, 0);
            letters[40].SetPixel(3, 7, 10);
            #endregion
            #region I
            letters[41].SetPixel(0, 0, 3);
            letters[41].SetPixel(1, 0, 0);
       

            letters[41].SetPixel(0, 1, 3);
            letters[41].SetPixel(1, 1, 10);
          

            letters[41].SetPixel(0, 2, 3);
            letters[41].SetPixel(1, 2, 10);
        
            letters[41].SetPixel(0, 3, 3);
            letters[41].SetPixel(1, 3, 10);
          

            letters[41].SetPixel(0, 4, 3);
            letters[41].SetPixel(1, 4, 10);
          

            letters[41].SetPixel(0, 5, 3);
            letters[41].SetPixel(1, 5, 10);
         

            letters[41].SetPixel(0, 6, 3);
            letters[41].SetPixel(1, 6, 10);
         

            letters[41].SetPixel(0, 7, 0);
            letters[41].SetPixel(1, 7, 10);
          
            #endregion
            #region J
            letters[42].SetPixel(0, 0, 0);
            letters[42].SetPixel(1, 0, 0);
            letters[42].SetPixel(2, 0, 3);
            letters[42].SetPixel(3, 0, 0);

            letters[42].SetPixel(0, 1, 0);
            letters[42].SetPixel(1, 1, 0);
            letters[42].SetPixel(2, 1, 3);
            letters[42].SetPixel(3, 1, 10);

            letters[42].SetPixel(0, 2, 0);
            letters[42].SetPixel(1, 2, 0);
            letters[42].SetPixel(2, 2, 3);
            letters[42].SetPixel(3, 2, 10);

            letters[42].SetPixel(0, 3, 0);
            letters[42].SetPixel(1, 3, 0);
            letters[42].SetPixel(2, 3, 3);
            letters[42].SetPixel(3, 3, 10);

            letters[42].SetPixel(0, 4, 0);
            letters[42].SetPixel(1, 4, 0);
            letters[42].SetPixel(2, 4, 3);
            letters[42].SetPixel(3, 4, 10);

            letters[42].SetPixel(0, 5, 3);
            letters[42].SetPixel(1, 5, 3);
            letters[42].SetPixel(2, 5, 3);
            letters[42].SetPixel(3, 5, 10);

            letters[42].SetPixel(0, 6, 3);
            letters[42].SetPixel(1, 6, 3);
            letters[42].SetPixel(2, 6, 10);
            letters[42].SetPixel(3, 6, 10);

            letters[42].SetPixel(0, 7, 0);
            letters[42].SetPixel(1, 7, 10);
            letters[42].SetPixel(2, 7, 10);
            letters[42].SetPixel(3, 7, 0);
            #endregion
            #region K
            letters[43].SetPixel(0, 0, 3);
            letters[43].SetPixel(1, 0, 0);
            letters[43].SetPixel(2, 0, 3);
            letters[43].SetPixel(3, 0, 0);

            letters[43].SetPixel(0, 1, 3);
            letters[43].SetPixel(1, 1, 10);
            letters[43].SetPixel(2, 1, 3);
            letters[43].SetPixel(3, 1, 10);

            letters[43].SetPixel(0, 2, 3);
            letters[43].SetPixel(1, 2, 10);
            letters[43].SetPixel(2, 2, 3);
            letters[43].SetPixel(3, 2, 10);

            letters[43].SetPixel(0, 3, 3);
            letters[43].SetPixel(1, 3, 3);
            letters[43].SetPixel(2, 3, 0);
            letters[43].SetPixel(3, 3, 10);

            letters[43].SetPixel(0, 4, 3);
            letters[43].SetPixel(1, 4, 10);
            letters[43].SetPixel(2, 4, 3);
            letters[43].SetPixel(3, 4, 0);

            letters[43].SetPixel(0, 5, 3);
            letters[43].SetPixel(1, 5, 10);
            letters[43].SetPixel(2, 5, 3);
            letters[43].SetPixel(3, 5, 10);

            letters[43].SetPixel(0, 6, 3);
            letters[43].SetPixel(1, 6, 10);
            letters[43].SetPixel(2, 6, 3);
            letters[43].SetPixel(3, 6, 10);

            letters[43].SetPixel(0, 7, 0);
            letters[43].SetPixel(1, 7, 10);
            letters[43].SetPixel(2, 7, 0);
            letters[43].SetPixel(3, 7, 10);
            #endregion
            #region L
            letters[44].SetPixel(0, 0, 3);
            letters[44].SetPixel(1, 0, 0);
            letters[44].SetPixel(2, 0, 0);
            letters[44].SetPixel(3, 0, 0);

            letters[44].SetPixel(0, 1, 3);
            letters[44].SetPixel(1, 1, 10);
            letters[44].SetPixel(2, 1,0);
            letters[44].SetPixel(3, 1, 0);

            letters[44].SetPixel(0, 2, 3);
            letters[44].SetPixel(1, 2, 10);
            letters[44].SetPixel(2, 2, 0);
            letters[44].SetPixel(3, 2, 0);

            letters[44].SetPixel(0, 3, 3);
            letters[44].SetPixel(1, 3, 10);
            letters[44].SetPixel(2, 3, 0);
            letters[44].SetPixel(3, 3, 0);

            letters[44].SetPixel(0, 4, 3);
            letters[44].SetPixel(1, 4, 10);
            letters[44].SetPixel(2, 4, 0);
            letters[44].SetPixel(3, 4, 0);

            letters[44].SetPixel(0, 5, 3);
            letters[44].SetPixel(1, 5, 3);
            letters[44].SetPixel(2, 5, 3);
            letters[44].SetPixel(3, 5, 0);

            letters[44].SetPixel(0, 6, 3);
            letters[44].SetPixel(1, 6, 3);
            letters[44].SetPixel(2, 6, 3);
            letters[44].SetPixel(3, 6, 10);

            letters[44].SetPixel(0, 7, 0);
            letters[44].SetPixel(1, 7, 10);
            letters[44].SetPixel(2, 7, 10);
            letters[44].SetPixel(3, 7, 10);
            #endregion
            #region M
            letters[45].SetPixel(0, 0, 3);
            letters[45].SetPixel(1, 0, 3);
            letters[45].SetPixel(2, 0, 0);
            letters[45].SetPixel(3, 0, 3);
            letters[45].SetPixel(4, 0, 0);
            letters[45].SetPixel(5, 0, 0);

            letters[45].SetPixel(0, 1, 3);
            letters[45].SetPixel(1, 1, 3);
            letters[45].SetPixel(2, 1, 3);
            letters[45].SetPixel(3, 1, 3);
            letters[45].SetPixel(4, 1, 3);
            letters[45].SetPixel(5, 1, 0);

            letters[45].SetPixel(0, 2, 3);
            letters[45].SetPixel(1, 2, 10);
            letters[45].SetPixel(2, 2, 3);
            letters[45].SetPixel(3, 2, 10);
            letters[45].SetPixel(4, 2, 3);
            letters[45].SetPixel(5, 2, 10);

            letters[45].SetPixel(0, 3, 3);
            letters[45].SetPixel(1, 3, 10);
            letters[45].SetPixel(2, 3, 3);
            letters[45].SetPixel(3, 3, 10);
            letters[45].SetPixel(4, 3, 3);
            letters[45].SetPixel(5, 3, 10);

            letters[45].SetPixel(0, 4, 3);
            letters[45].SetPixel(1, 4, 10);
            letters[45].SetPixel(2, 4, 3);
            letters[45].SetPixel(3, 4, 10);
            letters[45].SetPixel(4, 4, 3);
            letters[45].SetPixel(5, 4, 10);

            letters[45].SetPixel(0, 5, 3);
            letters[45].SetPixel(1, 5, 10);
            letters[45].SetPixel(2, 5, 3);
            letters[45].SetPixel(3, 5, 10);
            letters[45].SetPixel(4, 5, 3);
            letters[45].SetPixel(5, 5, 10);

            letters[45].SetPixel(0, 6, 3);
            letters[45].SetPixel(1, 6, 10);
            letters[45].SetPixel(2, 6, 3);
            letters[45].SetPixel(3, 6, 10);
            letters[45].SetPixel(4, 6, 3);
            letters[45].SetPixel(5, 6, 10);

            letters[45].SetPixel(0, 7, 0);
            letters[45].SetPixel(1, 7, 10);
            letters[45].SetPixel(2, 7, 0);
            letters[45].SetPixel(3, 7, 10);
            letters[45].SetPixel(4, 7, 0);
            letters[45].SetPixel(5, 7, 10);
            #endregion
            #region N
            letters[46].SetPixel(0, 0, 3);
            letters[46].SetPixel(1, 0, 3);
            letters[46].SetPixel(2, 0, 0);
            letters[46].SetPixel(3, 0, 0);

            letters[46].SetPixel(0, 1, 3);
            letters[46].SetPixel(1, 1, 3);
            letters[46].SetPixel(2, 1, 3);
            letters[46].SetPixel(3, 1, 0);

            letters[46].SetPixel(0, 2, 3);
            letters[46].SetPixel(1, 2, 10);
            letters[46].SetPixel(2, 2, 3);
            letters[46].SetPixel(3, 2, 10);

            letters[46].SetPixel(0, 3, 3);
            letters[46].SetPixel(1, 3, 10);
            letters[46].SetPixel(2, 3, 3);
            letters[46].SetPixel(3, 3, 10);

            letters[46].SetPixel(0, 4, 3);
            letters[46].SetPixel(1, 4, 10);
            letters[46].SetPixel(2, 4, 3);
            letters[46].SetPixel(3, 4, 10);

            letters[46].SetPixel(0, 5, 3);
            letters[46].SetPixel(1, 5, 10);
            letters[46].SetPixel(2, 5, 3);
            letters[46].SetPixel(3, 5, 10);

            letters[46].SetPixel(0, 6, 3);
            letters[46].SetPixel(1, 6, 10);
            letters[46].SetPixel(2, 6, 3);
            letters[46].SetPixel(3, 6, 10);

            letters[46].SetPixel(0, 7, 0);
            letters[46].SetPixel(1, 7, 10);
            letters[46].SetPixel(2, 7, 0);
            letters[46].SetPixel(3, 7, 10);
            #endregion      
            #region O
            letters[47].SetPixel(0, 0, 0);
            letters[47].SetPixel(1, 0, 3);
            letters[47].SetPixel(2, 0, 0);
            letters[47].SetPixel(3, 0, 0);

            letters[47].SetPixel(0, 1, 3);
            letters[47].SetPixel(1, 1, 3);
            letters[47].SetPixel(2, 1, 3);
            letters[47].SetPixel(3, 1, 0);

            letters[47].SetPixel(0, 2, 3);
            letters[47].SetPixel(1, 2, 10);
            letters[47].SetPixel(2, 2, 3);
            letters[47].SetPixel(3, 2, 10);

            letters[47].SetPixel(0, 3, 3);
            letters[47].SetPixel(1, 3, 10);
            letters[47].SetPixel(2, 3, 3);
            letters[47].SetPixel(3, 3, 10);

            letters[47].SetPixel(0, 4, 3);
            letters[47].SetPixel(1, 4, 10);
            letters[47].SetPixel(2, 4, 3);
            letters[47].SetPixel(3, 4, 10);

            letters[47].SetPixel(0, 5, 3);
            letters[47].SetPixel(1, 5, 3);
            letters[47].SetPixel(2, 5, 3);
            letters[47].SetPixel(3, 5, 10);

            letters[47].SetPixel(0, 6, 0);
            letters[47].SetPixel(1, 6, 3);
            letters[47].SetPixel(2, 6, 10);
            letters[47].SetPixel(3, 6, 10);

            letters[47].SetPixel(0, 7, 0);
            letters[47].SetPixel(1, 7, 0);
            letters[47].SetPixel(2, 7, 10);
            letters[47].SetPixel(3, 7, 0);
            #endregion
            #region P
            letters[48].SetPixel(0, 0, 3);
            letters[48].SetPixel(1, 0, 3);
            letters[48].SetPixel(2, 0, 0);
            letters[48].SetPixel(3, 0, 0);

            letters[48].SetPixel(0, 1, 3);
            letters[48].SetPixel(1, 1, 3);
            letters[48].SetPixel(2, 1, 3);
            letters[48].SetPixel(3, 1, 0);

            letters[48].SetPixel(0, 2, 3);
            letters[48].SetPixel(1, 2, 10);
            letters[48].SetPixel(2, 2, 3);
            letters[48].SetPixel(3, 2, 10);

            letters[48].SetPixel(0, 3, 3);
            letters[48].SetPixel(1, 3, 3);
            letters[48].SetPixel(2, 3, 3);
            letters[48].SetPixel(3, 3, 10);

            letters[48].SetPixel(0, 4, 3);
            letters[48].SetPixel(1, 4, 3);
            letters[48].SetPixel(2, 4, 10);
            letters[48].SetPixel(3, 4, 10);

            letters[48].SetPixel(0, 5, 3);
            letters[48].SetPixel(1, 5, 10);
            letters[48].SetPixel(2, 5, 10);
            letters[48].SetPixel(3, 5, 0);

            letters[48].SetPixel(0, 6, 3);
            letters[48].SetPixel(1, 6, 10);
            letters[48].SetPixel(2, 6, 0);
            letters[48].SetPixel(3, 6, 0);

            letters[48].SetPixel(0, 7, 0);
            letters[48].SetPixel(1, 7, 10);
            letters[48].SetPixel(2, 7, 0);
            letters[48].SetPixel(3, 7, 0);
            #endregion
            #region Q
            letters[49].SetPixel(0, 0, 0);
            letters[49].SetPixel(1, 0, 3);
            letters[49].SetPixel(2, 0, 0);
            letters[49].SetPixel(3, 0, 0);

            letters[49].SetPixel(0, 1, 3);
            letters[49].SetPixel(1, 1, 3);
            letters[49].SetPixel(2, 1, 3);
            letters[49].SetPixel(3, 1, 0);

            letters[49].SetPixel(0, 2, 3);
            letters[49].SetPixel(1, 2, 10);
            letters[49].SetPixel(2, 2, 3);
            letters[49].SetPixel(3, 2, 10);

            letters[49].SetPixel(0, 3, 3);
            letters[49].SetPixel(1, 3, 10);
            letters[49].SetPixel(2, 3, 3);
            letters[49].SetPixel(3, 3, 10);

            letters[49].SetPixel(0, 4, 3);
            letters[49].SetPixel(1, 4, 10);
            letters[49].SetPixel(2, 4, 3);
            letters[49].SetPixel(3, 4, 10);

            letters[49].SetPixel(0, 5, 3);
            letters[49].SetPixel(1, 5, 3);
            letters[49].SetPixel(2, 5, 3);
            letters[49].SetPixel(3, 5, 10);

            letters[49].SetPixel(0, 6, 0);
            letters[49].SetPixel(1, 6, 3);
            letters[49].SetPixel(2, 6, 3);
            letters[49].SetPixel(3, 6, 10);

            letters[49].SetPixel(0, 7, 0);
            letters[49].SetPixel(1, 7, 0);
            letters[49].SetPixel(2, 7, 10);
            letters[49].SetPixel(3, 7, 0);
            #endregion
            #region R
            letters[50].SetPixel(0, 0, 3);
            letters[50].SetPixel(1, 0, 3);
            letters[50].SetPixel(2, 0, 0);
            letters[50].SetPixel(3, 0, 0);

            letters[50].SetPixel(0, 1, 3);
            letters[50].SetPixel(1, 1, 3);
            letters[50].SetPixel(2, 1, 3);
            letters[50].SetPixel(3, 1, 0);

            letters[50].SetPixel(0, 2, 3);
            letters[50].SetPixel(1, 2, 10);
            letters[50].SetPixel(2, 2, 3);
            letters[50].SetPixel(3, 2, 10);

            letters[50].SetPixel(0, 3, 3);
            letters[50].SetPixel(1, 3, 3);
            letters[50].SetPixel(2, 3, 0);
            letters[50].SetPixel(3, 3, 10);

            letters[50].SetPixel(0, 4, 3);
            letters[50].SetPixel(1, 4, 3);
            letters[50].SetPixel(2, 4, 3);
            letters[50].SetPixel(3, 4, 0);

            letters[50].SetPixel(0, 5, 3);
            letters[50].SetPixel(1, 5, 10);
            letters[50].SetPixel(2, 5, 3);
            letters[50].SetPixel(3, 5, 10);

            letters[50].SetPixel(0, 6, 3);
            letters[50].SetPixel(1, 6, 10);
            letters[50].SetPixel(2, 6, 3);
            letters[50].SetPixel(3, 6, 10);

            letters[50].SetPixel(0, 7, 0);
            letters[50].SetPixel(1, 7, 10);
            letters[50].SetPixel(2, 7, 0);
            letters[50].SetPixel(3, 7, 10);
            #endregion
            #region S
            letters[51].SetPixel(0, 0, 0);
            letters[51].SetPixel(1, 0, 3);
            letters[51].SetPixel(2, 0, 3);
            letters[51].SetPixel(3, 0, 0);

            letters[51].SetPixel(0, 1, 3);
            letters[51].SetPixel(1, 1, 3);
            letters[51].SetPixel(2, 1, 3);
            letters[51].SetPixel(3, 1, 10);

            letters[51].SetPixel(0, 2, 3);
            letters[51].SetPixel(1, 2, 10);
            letters[51].SetPixel(2, 2, 10);
            letters[51].SetPixel(3, 2, 10);

            letters[51].SetPixel(0, 3, 0);
            letters[51].SetPixel(1, 3, 3);
            letters[51].SetPixel(2, 3, 0);
            letters[51].SetPixel(3, 3, 0);

            letters[51].SetPixel(0, 4, 0);
            letters[51].SetPixel(1, 4, 0);
            letters[51].SetPixel(2, 4, 3);
            letters[51].SetPixel(3, 4, 0);

            letters[51].SetPixel(0, 5, 3);
            letters[51].SetPixel(1, 5, 3);
            letters[51].SetPixel(2, 5, 3);
            letters[51].SetPixel(3, 5, 10);

            letters[51].SetPixel(0, 6, 3);
            letters[51].SetPixel(1, 6, 3);
            letters[51].SetPixel(2, 6, 10);
            letters[51].SetPixel(3, 6, 10);

            letters[51].SetPixel(0, 7, 0);
            letters[51].SetPixel(1, 7, 10);
            letters[51].SetPixel(2, 7, 10);
            letters[51].SetPixel(3, 7, 0);
            #endregion
            #region T
            letters[52].SetPixel(0, 0, 3);
            letters[52].SetPixel(1, 0, 3);
            letters[52].SetPixel(2, 0, 3);
            letters[52].SetPixel(3, 0, 0);

            letters[52].SetPixel(0, 1, 3);
            letters[52].SetPixel(1, 1, 3);
            letters[52].SetPixel(2, 1, 3);
            letters[52].SetPixel(3, 1, 10);

            letters[52].SetPixel(0, 2, 0);
            letters[52].SetPixel(1, 2, 3);
            letters[52].SetPixel(2, 2, 10);
            letters[52].SetPixel(3, 2, 10);

            letters[52].SetPixel(0, 3, 0);
            letters[52].SetPixel(1, 3, 3);
            letters[52].SetPixel(2, 3, 10);
            letters[52].SetPixel(3, 3, 0);

            letters[52].SetPixel(0, 4, 0);
            letters[52].SetPixel(1, 4, 3);
            letters[52].SetPixel(2, 4, 10);
            letters[52].SetPixel(3, 4, 0);

            letters[52].SetPixel(0, 5, 0);
            letters[52].SetPixel(1, 5, 3);
            letters[52].SetPixel(2, 5, 10);
            letters[52].SetPixel(3, 5, 0);

            letters[52].SetPixel(0, 6, 0);
            letters[52].SetPixel(1, 6, 3);
            letters[52].SetPixel(2, 6, 10);
            letters[52].SetPixel(3, 6, 0);

            letters[52].SetPixel(0, 7, 0);
            letters[52].SetPixel(1, 7, 0);
            letters[52].SetPixel(2, 7, 10);
            letters[52].SetPixel(3, 7, 0);
            #endregion
            #region U
            letters[53].SetPixel(0, 0, 3);
            letters[53].SetPixel(1, 0, 0);
            letters[53].SetPixel(2, 0, 3);
            letters[53].SetPixel(3, 0, 0);

            letters[53].SetPixel(0, 1, 3);
            letters[53].SetPixel(1, 1, 10);
            letters[53].SetPixel(2, 1, 3);
            letters[53].SetPixel(3, 1, 10);

            letters[53].SetPixel(0, 2, 3);
            letters[53].SetPixel(1, 2, 10);
            letters[53].SetPixel(2, 2, 3);
            letters[53].SetPixel(3, 2, 10);

            letters[53].SetPixel(0, 3, 3);
            letters[53].SetPixel(1, 3, 10);
            letters[53].SetPixel(2, 3, 3);
            letters[53].SetPixel(3, 3, 10);

            letters[53].SetPixel(0, 4, 3);
            letters[53].SetPixel(1, 4, 10);
            letters[53].SetPixel(2, 4, 3);
            letters[53].SetPixel(3, 4, 10);

            letters[53].SetPixel(0, 5, 3);
            letters[53].SetPixel(1, 5, 3);
            letters[53].SetPixel(2, 5, 3);
            letters[53].SetPixel(3, 5, 10);

            letters[53].SetPixel(0, 6, 3);
            letters[53].SetPixel(1, 6, 3);
            letters[53].SetPixel(2, 6, 10);
            letters[53].SetPixel(3, 6, 10);

            letters[53].SetPixel(0, 7, 0);
            letters[53].SetPixel(1, 7, 10);
            letters[53].SetPixel(2, 7, 10);
            letters[53].SetPixel(3, 7, 0);
            #endregion
            #region V
            letters[54].SetPixel(0, 0, 3);
            letters[54].SetPixel(1, 0, 0);
            letters[54].SetPixel(2, 0, 3);
            letters[54].SetPixel(3, 0, 0);

            letters[54].SetPixel(0, 1, 3);
            letters[54].SetPixel(1, 1, 10);
            letters[54].SetPixel(2, 1, 3);
            letters[54].SetPixel(3, 1, 10);

            letters[54].SetPixel(0, 2, 3);
            letters[54].SetPixel(1, 2, 10);
            letters[54].SetPixel(2, 2, 3);
            letters[54].SetPixel(3, 2, 10);

            letters[54].SetPixel(0, 3, 3);
            letters[54].SetPixel(1, 3, 10);
            letters[54].SetPixel(2, 3, 3);
            letters[54].SetPixel(3, 3, 10);

            letters[54].SetPixel(0, 4, 3);
            letters[54].SetPixel(1, 4, 10);
            letters[54].SetPixel(2, 4, 3);
            letters[54].SetPixel(3, 4, 10);

            letters[54].SetPixel(0, 5,0);
            letters[54].SetPixel(1, 5, 3);
            letters[54].SetPixel(2, 5, 10);
            letters[54].SetPixel(3, 5, 10);

            letters[54].SetPixel(0, 6, 0);
            letters[54].SetPixel(1, 6, 3);
            letters[54].SetPixel(2, 6, 10);
            letters[54].SetPixel(3, 6, 0);

            letters[54].SetPixel(0, 7, 0);
            letters[54].SetPixel(1, 7, 0);
            letters[54].SetPixel(2, 7, 10);
            letters[54].SetPixel(3, 7, 0);
            #endregion
            #region W
            letters[55].SetPixel(0, 0, 3);
            letters[55].SetPixel(1, 0, 0);
            letters[55].SetPixel(2, 0, 3);
            letters[55].SetPixel(3, 0, 0);
            letters[55].SetPixel(4, 0, 3);
            letters[55].SetPixel(5, 0, 0);

            letters[55].SetPixel(0, 1, 3);
            letters[55].SetPixel(1, 1, 10);
            letters[55].SetPixel(2, 1, 3);
            letters[55].SetPixel(3, 1, 10);
            letters[55].SetPixel(4, 1, 3);
            letters[55].SetPixel(5, 1, 0);

            letters[55].SetPixel(0, 2, 3);
            letters[55].SetPixel(1, 2, 10);
            letters[55].SetPixel(2, 2, 3);
            letters[55].SetPixel(3, 2, 10);
            letters[55].SetPixel(4, 2, 3);
            letters[55].SetPixel(5, 2, 10);

            letters[55].SetPixel(0, 3, 3);
            letters[55].SetPixel(1, 3, 10);
            letters[55].SetPixel(2, 3, 3);
            letters[55].SetPixel(3, 3, 10);
            letters[55].SetPixel(4, 3, 3);
            letters[55].SetPixel(5, 3, 10);

            letters[55].SetPixel(0, 4, 3);
            letters[55].SetPixel(1, 4, 10);
            letters[55].SetPixel(2, 4, 3);
            letters[55].SetPixel(3, 4, 10);
            letters[55].SetPixel(4, 4, 3);
            letters[55].SetPixel(5, 4, 10);

            letters[55].SetPixel(0, 5, 3);
            letters[55].SetPixel(1, 5, 3);
            letters[55].SetPixel(2, 5, 3);
            letters[55].SetPixel(3, 5, 3);
            letters[55].SetPixel(4, 5, 3);
            letters[55].SetPixel(5, 5, 10);

            letters[55].SetPixel(0, 6, 3);
            letters[55].SetPixel(1, 6, 3);
            letters[55].SetPixel(2, 6, 10);
            letters[55].SetPixel(3, 6, 3);
            letters[55].SetPixel(4, 6, 10);
            letters[55].SetPixel(5, 6, 10);

            letters[55].SetPixel(0, 7, 0);
            letters[55].SetPixel(1, 7, 10);
            letters[55].SetPixel(2, 7, 10);
            letters[55].SetPixel(3, 7, 0);
            letters[55].SetPixel(4, 7, 10);
            letters[55].SetPixel(5, 7, 0);
            #endregion
            #region X
            letters[56].SetPixel(0, 0, 3);
            letters[56].SetPixel(1, 0, 0);
            letters[56].SetPixel(2, 0, 3);
            letters[56].SetPixel(3, 0, 0);

            letters[56].SetPixel(0, 1, 3);
            letters[56].SetPixel(1, 1, 10);
            letters[56].SetPixel(2, 1, 3);
            letters[56].SetPixel(3, 1, 10);

            letters[56].SetPixel(0, 2, 3);
            letters[56].SetPixel(1, 2, 10);
            letters[56].SetPixel(2, 2, 3);
            letters[56].SetPixel(3, 2, 10);

            letters[56].SetPixel(0, 3, 0);
            letters[56].SetPixel(1, 3, 3);
            letters[56].SetPixel(2, 3, 0);
            letters[56].SetPixel(3, 3, 10);

            letters[56].SetPixel(0, 4, 3);
            letters[56].SetPixel(1, 4, 0);
            letters[56].SetPixel(2, 4, 3);
            letters[56].SetPixel(3, 4, 0);

            letters[56].SetPixel(0, 5, 3);
            letters[56].SetPixel(1, 5, 10);
            letters[56].SetPixel(2, 5, 3);
            letters[56].SetPixel(3, 5, 10);

            letters[56].SetPixel(0, 6, 3);
            letters[56].SetPixel(1, 6, 10);
            letters[56].SetPixel(2, 6, 3);
            letters[56].SetPixel(3, 6, 10);

            letters[56].SetPixel(0, 7, 0);
            letters[56].SetPixel(1, 7, 10);
            letters[56].SetPixel(2, 7, 0);
            letters[56].SetPixel(3, 7, 10);
            #endregion
            #region Y
            letters[57].SetPixel(0, 0, 3);
            letters[57].SetPixel(1, 0, 0);
            letters[57].SetPixel(2, 0, 3);
            letters[57].SetPixel(3, 0, 0);

            letters[57].SetPixel(0, 1, 3);
            letters[57].SetPixel(1, 1, 10);
            letters[57].SetPixel(2, 1, 3);
            letters[57].SetPixel(3, 1, 10);

            letters[57].SetPixel(0, 2, 3);
            letters[57].SetPixel(1, 2, 10);
            letters[57].SetPixel(2, 2, 3);
            letters[57].SetPixel(3, 2, 10);

            letters[57].SetPixel(0, 3, 3);
            letters[57].SetPixel(1, 3, 3);
            letters[57].SetPixel(2, 3, 3);
            letters[57].SetPixel(3, 3, 10);

            letters[57].SetPixel(0, 4, 0);
            letters[57].SetPixel(1, 4, 3);
            letters[57].SetPixel(2, 4, 10);
            letters[57].SetPixel(3, 4, 10);

            letters[57].SetPixel(0, 5, 0);
            letters[57].SetPixel(1, 5, 3);
            letters[57].SetPixel(2, 5, 10);
            letters[57].SetPixel(3, 5, 0);

            letters[57].SetPixel(0, 6, 0);
            letters[57].SetPixel(1, 6, 3);
            letters[57].SetPixel(2, 6, 10);
            letters[57].SetPixel(3, 6, 0);

            letters[57].SetPixel(0, 7, 0);
            letters[57].SetPixel(1, 7, 0);
            letters[57].SetPixel(2, 7, 10);
            letters[57].SetPixel(3, 7, 0);
            #endregion
            #region Z
            letters[58].SetPixel(0, 0, 3);
            letters[58].SetPixel(1, 0, 3);
            letters[58].SetPixel(2, 0, 3);
            letters[58].SetPixel(3, 0, 0);

            letters[58].SetPixel(0, 1, 3);
            letters[58].SetPixel(1, 1, 3);
            letters[58].SetPixel(2, 1, 3);
            letters[58].SetPixel(3, 1, 10);

            letters[58].SetPixel(0, 2, 0);
            letters[58].SetPixel(1, 2, 10);
            letters[58].SetPixel(2, 2, 3);
            letters[58].SetPixel(3, 2, 10);

            letters[58].SetPixel(0, 3, 0);
            letters[58].SetPixel(1, 3, 3);
            letters[58].SetPixel(2, 3, 0);
            letters[58].SetPixel(3, 3, 10);

            letters[58].SetPixel(0, 4, 3);
            letters[58].SetPixel(1, 4, 0);
            letters[58].SetPixel(2, 4, 10);
            letters[58].SetPixel(3, 4, 0);

            letters[58].SetPixel(0, 5, 3);
            letters[58].SetPixel(1, 5, 3);
            letters[58].SetPixel(2, 5, 3);
            letters[58].SetPixel(3, 5, 0);

            letters[58].SetPixel(0, 6, 3);
            letters[58].SetPixel(1, 6, 3);
            letters[58].SetPixel(2, 6, 3);
            letters[58].SetPixel(3, 6, 10);

            letters[58].SetPixel(0, 7, 0);
            letters[58].SetPixel(1, 7, 10);
            letters[58].SetPixel(2, 7, 10);
            letters[58].SetPixel(3, 7, 10);
            #endregion
            #region Space
letters[0].SetPixel(0, 0, 0);
letters[0].SetPixel(1, 0, 0);
letters[0].SetPixel(2, 0, 0);
letters[0].SetPixel(3, 0, 0);

            letters[0].SetPixel(0, 1, 0);
            letters[0].SetPixel(1, 1, 0);
            letters[0].SetPixel(2, 1, 0);
            letters[0].SetPixel(3, 1, 0);

            letters[0].SetPixel(0, 2, 0);
            letters[0].SetPixel(1, 2, 0);
            letters[0].SetPixel(2, 2, 0);
            letters[0].SetPixel(3, 2, 0);

            letters[0].SetPixel(0, 3, 0);
            letters[0].SetPixel(1, 3, 0);
            letters[0].SetPixel(2, 3, 0);
            letters[0].SetPixel(3, 3, 0);

            letters[0].SetPixel(0, 4, 0);
            letters[0].SetPixel(1, 4, 0);
            letters[0].SetPixel(2, 4, 0);
            letters[0].SetPixel(3, 4, 0);

            letters[0].SetPixel(0, 5, 0);
            letters[0].SetPixel(1, 5, 0);
            letters[0].SetPixel(2, 5, 0);
            letters[0].SetPixel(3, 5, 0);

            letters[0].SetPixel(0, 6, 0);
            letters[0].SetPixel(1, 6, 0);
            letters[0].SetPixel(2, 6, 0);
            letters[0].SetPixel(3, 6, 0);

            letters[0].SetPixel(0, 7, 0);
            letters[0].SetPixel(1, 7, 0);
            letters[0].SetPixel(2, 7, 0);
            letters[0].SetPixel(3, 7, 0);
            #endregion
            #region !
            letters[1].SetPixel(0, 0, 3);
            letters[1].SetPixel(1, 0, 0);
        
            letters[1].SetPixel(0, 1, 3);
            letters[1].SetPixel(1, 1, 10);
         
            letters[1].SetPixel(0, 2, 3);
            letters[1].SetPixel(1, 2, 10);
        
            letters[1].SetPixel(0, 3, 3);
            letters[1].SetPixel(1, 3, 10);
         

            letters[1].SetPixel(0, 4, 3);
            letters[1].SetPixel(1, 4, 10);
         

            letters[1].SetPixel(0, 5, 0);
            letters[1].SetPixel(1, 5, 10);
          
            letters[1].SetPixel(0, 6, 3);
            letters[1].SetPixel(1, 6, 0);
      
            letters[1].SetPixel(0, 7, 0);
            letters[1].SetPixel(1, 7, 10);
          
            #endregion
            #region Period
            letters[14].SetPixel(0, 0, 0);
            letters[14].SetPixel(1, 0, 0);
            //  letters[14].SetPixel(2, 0, 0);

            letters[14].SetPixel(0, 1, 0);
            letters[14].SetPixel(1, 1, 0);
            //   letters[14].SetPixel(2, 1, 0);

            letters[14].SetPixel(0, 2, 0);
            letters[14].SetPixel(1, 2, 0);
            //  letters[14].SetPixel(2, 2, 0);

            letters[14].SetPixel(0, 3, 0);
            letters[14].SetPixel(1, 3, 0);
            // letters[14].SetPixel(2, 3, 0);

            letters[14].SetPixel(0, 4, 0);
            letters[14].SetPixel(1, 4, 0);
            //  letters[14].SetPixel(2, 4, 0);

            letters[14].SetPixel(0, 5, 0);
            letters[14].SetPixel(1, 5, 0);
            //letters[14].SetPixel(2, 5, 0);

            letters[14].SetPixel(0, 6, 3);
            letters[14].SetPixel(1, 6, 0);
            //  letters[14].SetPixel(2, 6, 0);

            letters[14].SetPixel(0, 7, 0);
            letters[14].SetPixel(1, 7, 10);
            // letters[14].SetPixel(2, 7, 10);
            #endregion
            #region Colon
            letters[26].SetPixel(0, 0, 0);
            letters[26].SetPixel(1, 0, 0);
           
            letters[26].SetPixel(0, 1, 0);
            letters[26].SetPixel(1, 1, 0);
            

            letters[26].SetPixel(0, 2, 3);
            letters[26].SetPixel(1, 2, 0);
        

            letters[26].SetPixel(0, 3, 0);
            letters[26].SetPixel(1, 3, 10);
          

            letters[26].SetPixel(0, 4, 0);
            letters[26].SetPixel(1, 4, 0);
          
            letters[26].SetPixel(0, 5, 3);
            letters[26].SetPixel(1, 5, 0);
          

            letters[26].SetPixel(0, 6, 0);
            letters[26].SetPixel(1, 6, 10);


            letters[26].SetPixel(0, 7, 0);
            letters[26].SetPixel(1, 7, 0);
            #endregion
            #region 0
            letters[16].SetPixel(0, 0, 0);
            letters[16].SetPixel(1, 0, 3);
            letters[16].SetPixel(2, 0, 3);
            letters[16].SetPixel(3, 0, 0);
            letters[16].SetPixel(4, 0, 0);
          

            letters[16].SetPixel(0, 1, 3);
            letters[16].SetPixel(1, 1, 0);
            letters[16].SetPixel(2, 1, 0);
            letters[16].SetPixel(3, 1, 3);
            letters[16].SetPixel(4, 1, 0);
      

            letters[16].SetPixel(0, 2, 3);
            letters[16].SetPixel(1, 2, 0);
            letters[16].SetPixel(2, 2, 3);
            letters[16].SetPixel(3, 2, 3);
            letters[16].SetPixel(4, 2, 10);
           

            letters[16].SetPixel(0, 3, 3);
            letters[16].SetPixel(1, 3, 3);
            letters[16].SetPixel(2, 3, 3);
            letters[16].SetPixel(3, 3, 3);
            letters[16].SetPixel(4, 3, 10);
            

            letters[16].SetPixel(0, 4, 3);
            letters[16].SetPixel(1, 4, 3);
            letters[16].SetPixel(2, 4, 10);
            letters[16].SetPixel(3, 4, 3);
            letters[16].SetPixel(4, 4, 10);
           

            letters[16].SetPixel(0, 5, 3);
            letters[16].SetPixel(1, 5, 0);
            letters[16].SetPixel(2, 5, 10);
            letters[16].SetPixel(3, 5, 3);
            letters[16].SetPixel(4, 5, 10);
          

            letters[16].SetPixel(0, 6, 0);
            letters[16].SetPixel(1, 6, 3);
            letters[16].SetPixel(2, 6, 3);
            letters[16].SetPixel(3, 6, 10);
            letters[16].SetPixel(4, 6, 10);
        

            letters[16].SetPixel(0, 7, 0);
            letters[16].SetPixel(1, 7, 0);
            letters[16].SetPixel(2, 7, 10);
            letters[16].SetPixel(3, 7, 10);
            letters[16].SetPixel(4, 7, 0);        
            #endregion
            #region 1
            letters[17].SetPixel(0, 0, 0);
            letters[17].SetPixel(1, 0, 3);
            letters[17].SetPixel(2, 0, 0);
            letters[17].SetPixel(3, 0, 0);

            letters[17].SetPixel(0, 1, 3);
            letters[17].SetPixel(1, 1, 3);
            letters[17].SetPixel(2, 1, 10);
            letters[17].SetPixel(3, 1, 0);

            letters[17].SetPixel(0, 2, 0);
            letters[17].SetPixel(1, 2, 3);
            letters[17].SetPixel(2, 2, 10);
            letters[17].SetPixel(3, 2, 0);

            letters[17].SetPixel(0, 3, 0);
            letters[17].SetPixel(1, 3, 3);
            letters[17].SetPixel(2, 3, 10);
            letters[17].SetPixel(3, 3, 0);

            letters[17].SetPixel(0, 4, 0);
            letters[17].SetPixel(1, 4, 3);
            letters[17].SetPixel(2, 4, 10);
            letters[17].SetPixel(3, 4, 0);

            letters[17].SetPixel(0, 5, 0);
            letters[17].SetPixel(1, 5, 3);
            letters[17].SetPixel(2, 5, 10);
            letters[17].SetPixel(3, 5, 0);

            letters[17].SetPixel(0, 6, 3);
            letters[17].SetPixel(1, 6, 3);
            letters[17].SetPixel(2, 6, 3);
            letters[17].SetPixel(3, 6, 0);

            letters[17].SetPixel(0, 7, 0);
            letters[17].SetPixel(1, 7, 10);
            letters[17].SetPixel(2, 7, 10);
            letters[17].SetPixel(3, 7, 10);
            #endregion
            #region 2
            letters[18].SetPixel(0, 0, 0);
            letters[18].SetPixel(1, 0, 3);
            letters[18].SetPixel(2, 0, 3);
            letters[18].SetPixel(3, 0, 0);
            letters[18].SetPixel(4, 0, 0);

            letters[18].SetPixel(0, 1, 3);
            letters[18].SetPixel(1, 1, 10);
            letters[18].SetPixel(2, 1, 10);
            letters[18].SetPixel(3, 1, 3);
            letters[18].SetPixel(4, 1, 0);

            letters[18].SetPixel(0, 2, 0);
            letters[18].SetPixel(1, 2, 0);
            letters[18].SetPixel(2, 2, 0);
            letters[18].SetPixel(3, 2, 3);
            letters[18].SetPixel(4, 2, 0);

            letters[18].SetPixel(0, 3, 0);
            letters[18].SetPixel(1, 3, 0);
            letters[18].SetPixel(2, 3, 3);
            letters[18].SetPixel(3, 3, 10);
            letters[18].SetPixel(4, 3, 0);

            letters[18].SetPixel(0, 4, 0);
            letters[18].SetPixel(1, 4, 3);
            letters[18].SetPixel(2, 4, 10);
            letters[18].SetPixel(3, 4, 0);
            letters[18].SetPixel(4, 4, 0);

            letters[18].SetPixel(0, 5, 3);
            letters[18].SetPixel(1, 5, 10);
            letters[18].SetPixel(2, 5, 0);
            letters[18].SetPixel(3, 5, 0);
            letters[18].SetPixel(4, 5, 0);


            letters[18].SetPixel(0, 6, 3);
            letters[18].SetPixel(1, 6, 3);
            letters[18].SetPixel(2, 6, 3);
            letters[18].SetPixel(3, 6, 3);
            letters[18].SetPixel(4, 6, 0);


            letters[18].SetPixel(0, 7, 0);
            letters[18].SetPixel(1, 7, 10);
            letters[18].SetPixel(2, 7, 10);
            letters[18].SetPixel(3, 7, 10);
            letters[18].SetPixel(4, 7, 10);
            #endregion
            #region 3
            letters[19].SetPixel(0, 0, 0);
            letters[19].SetPixel(1, 0, 3);
            letters[19].SetPixel(2, 0, 3);
            letters[19].SetPixel(3, 0, 0);
            letters[19].SetPixel(4, 0, 0);


            letters[19].SetPixel(0, 1, 3);
            letters[19].SetPixel(1, 1, 10);
            letters[19].SetPixel(2, 1, 10);
            letters[19].SetPixel(3, 1, 3);
            letters[19].SetPixel(4, 1, 0);


            letters[19].SetPixel(0, 2, 0);
            letters[19].SetPixel(1, 2, 0);
            letters[19].SetPixel(2, 2, 0);
            letters[19].SetPixel(3, 2, 3);
            letters[19].SetPixel(4, 2, 10);


            letters[19].SetPixel(0, 3, 0);
            letters[19].SetPixel(1, 3, 0);
            letters[19].SetPixel(2, 3, 3);
            letters[19].SetPixel(3, 3, 0);
            letters[19].SetPixel(4, 3, 10);


            letters[19].SetPixel(0, 4, 0);
            letters[19].SetPixel(1, 4, 0);
            letters[19].SetPixel(2, 4, 0);
            letters[19].SetPixel(3, 4, 3);
            letters[19].SetPixel(4, 4, 0);


            letters[19].SetPixel(0, 5, 3);
            letters[19].SetPixel(1, 5, 10);
            letters[19].SetPixel(2, 5, 10);
            letters[19].SetPixel(3, 5, 3);
            letters[19].SetPixel(4, 5, 10);


            letters[19].SetPixel(0, 6, 0);
            letters[19].SetPixel(1, 6, 3);
            letters[19].SetPixel(2, 6, 3);
            letters[19].SetPixel(3, 6, 10);
            letters[19].SetPixel(4, 6, 10);


            letters[19].SetPixel(0, 7, 0);
            letters[19].SetPixel(1, 7, 0);
            letters[19].SetPixel(2, 7, 10);
            letters[19].SetPixel(3, 7, 10);
            letters[19].SetPixel(4, 7, 0);
            #endregion
            #region 4
            letters[20].SetPixel(0, 0, 3);
            letters[20].SetPixel(1, 0, 0);
            letters[20].SetPixel(2, 0, 3);
            letters[20].SetPixel(3, 0, 0);

            letters[20].SetPixel(0, 1, 3);
            letters[20].SetPixel(1, 1, 10);
            letters[20].SetPixel(2, 1, 3);
            letters[20].SetPixel(3, 1, 10);

            letters[20].SetPixel(0, 2, 3);
            letters[20].SetPixel(1, 2, 10);
            letters[20].SetPixel(2, 2, 3);
            letters[20].SetPixel(3, 2, 10);

            letters[20].SetPixel(0, 3, 0);
            letters[20].SetPixel(1, 3, 3);
            letters[20].SetPixel(2, 3, 3);
            letters[20].SetPixel(3, 3, 10);

            letters[20].SetPixel(0, 4, 0);
            letters[20].SetPixel(1, 4, 0);
            letters[20].SetPixel(2, 4, 3);
            letters[20].SetPixel(3, 4, 10);

            letters[20].SetPixel(0, 5, 0);
            letters[20].SetPixel(1, 5, 0);
            letters[20].SetPixel(2, 5, 3);
            letters[20].SetPixel(3, 5, 10);

            letters[20].SetPixel(0, 6, 0);
            letters[20].SetPixel(1, 6, 0);
            letters[20].SetPixel(2, 6, 3);
            letters[20].SetPixel(3, 6, 10);

            letters[20].SetPixel(0, 7, 0);
            letters[20].SetPixel(1, 7, 0);
            letters[20].SetPixel(2, 7, 0);
            letters[20].SetPixel(3, 7, 10);
            #endregion
            #region 5
            letters[21].SetPixel(0, 0, 0);
            letters[21].SetPixel(1, 0, 3);
            letters[21].SetPixel(2, 0, 3);
            letters[21].SetPixel(3, 0, 3);
            letters[21].SetPixel(4, 0, 0);

            letters[21].SetPixel(0, 1, 3);
            letters[21].SetPixel(1, 1, 10);
            letters[21].SetPixel(2, 1, 10);
            letters[21].SetPixel(3, 1, 10);
            letters[21].SetPixel(4, 1, 0);

            letters[21].SetPixel(0, 2, 3);
            letters[21].SetPixel(1, 2, 10);
            letters[21].SetPixel(2, 2, 0);
            letters[21].SetPixel(3, 2, 0);
            letters[21].SetPixel(4, 2, 0);

            letters[21].SetPixel(0, 3, 3);
            letters[21].SetPixel(1, 3, 3);
            letters[21].SetPixel(2, 3, 3);
            letters[21].SetPixel(3, 3, 0);
            letters[21].SetPixel(4, 3, 0);

            letters[21].SetPixel(0, 4, 0);
            letters[21].SetPixel(1, 4, 0);
            letters[21].SetPixel(2, 4, 0);
            letters[21].SetPixel(3, 4, 3);
            letters[21].SetPixel(4, 4, 0);

            letters[21].SetPixel(0, 5, 0);
            letters[21].SetPixel(1, 5, 0);
            letters[21].SetPixel(2, 5, 0);
            letters[21].SetPixel(3, 5, 3);
            letters[21].SetPixel(4, 5, 10);

            letters[21].SetPixel(0, 6, 3);
            letters[21].SetPixel(1, 6, 3);
            letters[21].SetPixel(2, 6, 3);
            letters[21].SetPixel(3, 6, 10);
            letters[21].SetPixel(4, 6, 10);

            letters[21].SetPixel(0, 7, 0);
            letters[21].SetPixel(1, 7, 10);
            letters[21].SetPixel(2, 7, 10);
            letters[21].SetPixel(3, 7, 10);
            letters[21].SetPixel(4, 7, 0);
            #endregion
            #region 6
            letters[22].SetPixel(0, 0, 0);
            letters[22].SetPixel(1, 0, 3);
            letters[22].SetPixel(2, 0, 3);
            letters[22].SetPixel(3, 0, 3);
            letters[22].SetPixel(4, 0, 0);

            letters[22].SetPixel(0, 1, 3);
            letters[22].SetPixel(1, 1, 10);
            letters[22].SetPixel(2, 1, 10);
            letters[22].SetPixel(3, 1, 10);
            letters[22].SetPixel(4, 1, 0);

            letters[22].SetPixel(0, 2, 3);
            letters[22].SetPixel(1, 2, 10);
            letters[22].SetPixel(2, 2, 0);
            letters[22].SetPixel(3, 2, 0);
            letters[22].SetPixel(4, 2, 0);

            letters[22].SetPixel(0, 3, 3);
            letters[22].SetPixel(1, 3, 3);
            letters[22].SetPixel(2, 3, 3);
            letters[22].SetPixel(3, 3, 0);
            letters[22].SetPixel(4, 3, 0);

            letters[22].SetPixel(0, 4, 3);
            letters[22].SetPixel(1, 4, 10);
            letters[22].SetPixel(2, 4, 10);
            letters[22].SetPixel(3, 4, 3);
            letters[22].SetPixel(4, 4, 10);

            letters[22].SetPixel(0, 5, 3);
            letters[22].SetPixel(1, 5, 10);
            letters[22].SetPixel(2, 5, 10);
            letters[22].SetPixel(3, 5, 3);
            letters[22].SetPixel(4, 5, 10);

            letters[22].SetPixel(0, 6, 0);
            letters[22].SetPixel(1, 6, 3);
            letters[22].SetPixel(2, 6, 3);
            letters[22].SetPixel(3, 6, 10);
            letters[22].SetPixel(4, 6, 10);

            letters[22].SetPixel(0, 7, 0);
            letters[22].SetPixel(1, 7, 0);
            letters[22].SetPixel(2, 7, 10);
            letters[22].SetPixel(3, 7, 10);
            letters[22].SetPixel(4, 7, 0);
            #endregion
            #region 7
            letters[23].SetPixel(0, 0, 3);
            letters[23].SetPixel(1, 0, 3);
            letters[23].SetPixel(2, 0, 3);
            letters[23].SetPixel(3, 0, 3);
            letters[23].SetPixel(4, 0, 0);

            letters[23].SetPixel(0, 1, 0);
            letters[23].SetPixel(1, 1, 10);
            letters[23].SetPixel(2, 1, 10);
            letters[23].SetPixel(3, 1, 3);
            letters[23].SetPixel(4, 1, 10);

            letters[23].SetPixel(0, 2, 0);
            letters[23].SetPixel(1, 2, 0);
            letters[23].SetPixel(2, 2, 0);
            letters[23].SetPixel(3, 2, 3);
            letters[23].SetPixel(4, 2, 10);

            letters[23].SetPixel(0, 3, 0);
            letters[23].SetPixel(1, 3, 0);
            letters[23].SetPixel(2, 3, 3);
            letters[23].SetPixel(3, 3, 10);
            letters[23].SetPixel(4, 3, 10);

            letters[23].SetPixel(0, 4,0);
            letters[23].SetPixel(1, 4, 0);
            letters[23].SetPixel(2, 4, 3);
            letters[23].SetPixel(3, 4, 10);
            letters[23].SetPixel(4, 4, 0);

            letters[23].SetPixel(0, 5,0);
            letters[23].SetPixel(1, 5, 3);
            letters[23].SetPixel(2, 5, 10);
            letters[23].SetPixel(3, 5, 10);
            letters[23].SetPixel(4, 5, 0);

            letters[23].SetPixel(0, 6, 0);
            letters[23].SetPixel(1, 6, 3);
            letters[23].SetPixel(2, 6, 10);
            letters[23].SetPixel(3, 6, 0);
            letters[23].SetPixel(4, 6, 0);

            letters[23].SetPixel(0, 7, 0);
            letters[23].SetPixel(1, 7, 0);
            letters[23].SetPixel(2, 7, 10);
            letters[23].SetPixel(3, 7, 0);
            letters[23].SetPixel(4, 7, 0);
            #endregion
            #region 8
            letters[24].SetPixel(0, 0, 0);
            letters[24].SetPixel(1, 0, 3);
            letters[24].SetPixel(2, 0, 3);
            letters[24].SetPixel(3, 0, 0);
            letters[24].SetPixel(4, 0, 0);

            letters[24].SetPixel(0, 1, 3);
            letters[24].SetPixel(1, 1, 10);
            letters[24].SetPixel(2, 1, 10);
            letters[24].SetPixel(3, 1, 3);
            letters[24].SetPixel(4, 1, 0);

            letters[24].SetPixel(0, 2, 3);
            letters[24].SetPixel(1, 2, 10);
            letters[24].SetPixel(2, 2, 10);
            letters[24].SetPixel(3, 2, 3);
            letters[24].SetPixel(4, 2, 10);

            letters[24].SetPixel(0, 3, 0);
            letters[24].SetPixel(1, 3, 3);
            letters[24].SetPixel(2, 3, 3);
            letters[24].SetPixel(3, 3, 0);
            letters[24].SetPixel(4, 3, 10);

            letters[24].SetPixel(0, 4, 3);
            letters[24].SetPixel(1, 4, 10);
            letters[24].SetPixel(2, 4, 10);
            letters[24].SetPixel(3, 4, 3);
            letters[24].SetPixel(4, 4, 0);

            letters[24].SetPixel(0, 5, 3);
            letters[24].SetPixel(1, 5, 10);
            letters[24].SetPixel(2, 5, 10);
            letters[24].SetPixel(3, 5, 3);
            letters[24].SetPixel(4, 5, 10);

            letters[24].SetPixel(0, 6, 0);
            letters[24].SetPixel(1, 6, 3);
            letters[24].SetPixel(2, 6, 3);
            letters[24].SetPixel(3, 6, 10);
            letters[24].SetPixel(4, 6, 10);

            letters[24].SetPixel(0, 7, 0);
            letters[24].SetPixel(1, 7, 0);
            letters[24].SetPixel(2, 7, 10);
            letters[24].SetPixel(3, 7, 10);
            letters[24].SetPixel(4, 7, 0);
            #endregion
            #region 9
            letters[25].SetPixel(0, 0, 0);
            letters[25].SetPixel(1, 0, 3);
            letters[25].SetPixel(2, 0, 3);
            letters[25].SetPixel(3, 0, 0);
            letters[25].SetPixel(4, 0, 0);

            letters[25].SetPixel(0, 1, 3);
            letters[25].SetPixel(1, 1, 10);
            letters[25].SetPixel(2, 1, 10);
            letters[25].SetPixel(3, 1, 3);
            letters[25].SetPixel(4, 1, 10);

            letters[25].SetPixel(0, 2, 3);
            letters[25].SetPixel(1, 2, 10);
            letters[25].SetPixel(2, 2, 10);
            letters[25].SetPixel(3, 2, 3);
            letters[25].SetPixel(4, 2, 10);

            letters[25].SetPixel(0, 3, 0);
            letters[25].SetPixel(1, 3, 3);
            letters[25].SetPixel(2, 3, 3);
            letters[25].SetPixel(3, 3, 3);
            letters[25].SetPixel(4, 3, 10);

            letters[25].SetPixel(0, 4, 0);
            letters[25].SetPixel(1, 4, 0);
            letters[25].SetPixel(2, 4, 3);
            letters[25].SetPixel(3, 4, 10);
            letters[25].SetPixel(4, 4, 0);

            letters[25].SetPixel(0, 5, 0);
            letters[25].SetPixel(1, 5, 3);
            letters[25].SetPixel(2, 5, 10);
            letters[25].SetPixel(3, 5, 0);
            letters[25].SetPixel(4, 5, 0);

            letters[25].SetPixel(0, 6, 0);
            letters[25].SetPixel(1, 6, 3);
            letters[25].SetPixel(2, 6, 10);
            letters[25].SetPixel(3, 6, 0);
            letters[25].SetPixel(4, 6, 0);

            letters[25].SetPixel(0, 7, 0);
            letters[25].SetPixel(1, 7, 0);
            letters[25].SetPixel(2, 7, 10);
            letters[25].SetPixel(3, 7, 0);
            letters[25].SetPixel(4, 7, 0);
            #endregion
            #endregion
            #region Small Font
            #region Height and Width Definitions
            //punctuation
            small_font[0] = new fontTile(4, 5);
            small_font[1] = new fontTile(2, 5);
            small_font[14] = new fontTile(2, 5);
            small_font[26] = new fontTile(1, 5);
            //numbers
            small_font[16] = new fontTile(3, 5);
            small_font[17] = new fontTile(3, 5);
            small_font[18] = new fontTile(3, 5);
            small_font[19] = new fontTile(3, 5);
            small_font[20] = new fontTile(3, 5);
            small_font[21] = new fontTile(3, 5);
            small_font[22] = new fontTile(3, 5);
            small_font[23] = new fontTile(3, 5);
            small_font[24] = new fontTile(3, 5);
            small_font[25] = new fontTile(3, 5); 
            //letters
            small_font[33] = new fontTile(3, 5);
            small_font[34] = new fontTile(3, 5);
            small_font[35] = new fontTile(3, 5);
            small_font[36] = new fontTile(3, 5);
            small_font[37] = new fontTile(3, 5);
            small_font[38] = new fontTile(3, 5);
            small_font[39] = new fontTile(3, 5);
            small_font[40] = new fontTile(3, 5);
            small_font[41] = new fontTile(3, 5);
            small_font[42] = new fontTile(3, 5);
            small_font[43] = new fontTile(3, 5);
            small_font[44] = new fontTile(3, 5);
            small_font[45] = new fontTile(3, 5);
            small_font[46] = new fontTile(3, 5);
            small_font[47] = new fontTile(3, 5);
            small_font[48] = new fontTile(3, 5);
            small_font[49] = new fontTile(3, 5);
            small_font[50] = new fontTile(3, 5);
            small_font[51] = new fontTile(3, 5);
            small_font[52] = new fontTile(3, 5);
            small_font[53] = new fontTile(3, 5);
            small_font[54] = new fontTile(3, 5);
            small_font[55] = new fontTile(3, 5);
            small_font[56] = new fontTile(3, 5);
            small_font[57] = new fontTile(3, 5);
            small_font[58] = new fontTile(3, 5);
            #endregion

            #region A
            small_font[33].SetPixel(0, 0, 3);
            small_font[33].SetPixel(1, 0, 3);
            small_font[33].SetPixel(2, 0, 3);

            small_font[33].SetPixel(0, 1, 3);
            small_font[33].SetPixel(1, 1, 0);
            small_font[33].SetPixel(2, 1, 3);
       
            small_font[33].SetPixel(0, 2, 3);
            small_font[33].SetPixel(1, 2, 3);
            small_font[33].SetPixel(2, 2, 3);
           
            small_font[33].SetPixel(0, 3, 3);
            small_font[33].SetPixel(1, 3, 0);
            small_font[33].SetPixel(2, 3, 3);
          
            small_font[33].SetPixel(0, 4, 3);
            small_font[33].SetPixel(1, 4, 0);
            small_font[33].SetPixel(2, 4, 3);
            #endregion
            #region B
            small_font[34].SetPixel(0, 0, 3);
            small_font[34].SetPixel(1, 0, 3);
            small_font[34].SetPixel(2, 0, 3);
          

            small_font[34].SetPixel(0, 1, 3);
            small_font[34].SetPixel(1, 1, 0);
            small_font[34].SetPixel(2, 1, 3);
          

            small_font[34].SetPixel(0, 2, 3);
            small_font[34].SetPixel(1, 2, 3);
            small_font[34].SetPixel(2, 2, 3);
           

            small_font[34].SetPixel(0, 3, 3);
            small_font[34].SetPixel(1, 3, 0);
            small_font[34].SetPixel(2, 3, 3);
            

            small_font[34].SetPixel(0, 4, 3);
            small_font[34].SetPixel(1, 4, 3);
            small_font[34].SetPixel(2, 4, 3);
            #endregion
            #region C
            small_font[35].SetPixel(0, 0, 3);
            small_font[35].SetPixel(1, 0, 3);
            small_font[35].SetPixel(2, 0, 3);
          
            small_font[35].SetPixel(0, 1, 3);
            small_font[35].SetPixel(1, 1, 0);
            small_font[35].SetPixel(2, 1, 0);
          
            small_font[35].SetPixel(0, 2, 3);
            small_font[35].SetPixel(1, 2, 0);
            small_font[35].SetPixel(2, 2, 0);
          
            small_font[35].SetPixel(0, 3, 3);
            small_font[35].SetPixel(1, 3, 0);
            small_font[35].SetPixel(2, 3, 0);
     
            small_font[35].SetPixel(0, 4, 3);
            small_font[35].SetPixel(1, 4, 3);
            small_font[35].SetPixel(2, 4, 3);
            #endregion
            #region D
            small_font[36].SetPixel(0, 0, 3);
            small_font[36].SetPixel(1, 0, 3);
            small_font[36].SetPixel(2, 0, 0);        

            small_font[36].SetPixel(0, 1, 3);
            small_font[36].SetPixel(1, 1, 0);
            small_font[36].SetPixel(2, 1, 3);
          
            small_font[36].SetPixel(0, 2, 3);
            small_font[36].SetPixel(1, 2, 0);
            small_font[36].SetPixel(2, 2, 3);
            
            small_font[36].SetPixel(0, 3, 3);
            small_font[36].SetPixel(1, 3, 0);
            small_font[36].SetPixel(2, 3, 3);
          
            small_font[36].SetPixel(0, 4, 3);
            small_font[36].SetPixel(1, 4, 3);
            small_font[36].SetPixel(2, 4, 0);     
            #endregion
            #region E
            small_font[37].SetPixel(0, 0, 3);
            small_font[37].SetPixel(1, 0, 3);
            small_font[37].SetPixel(2, 0, 3);
            

            small_font[37].SetPixel(0, 1, 3);
            small_font[37].SetPixel(1, 1, 0);
            small_font[37].SetPixel(2, 1, 0);
           

            small_font[37].SetPixel(0, 2, 3);
            small_font[37].SetPixel(1, 2, 3);
            small_font[37].SetPixel(2, 2, 0);
            

            small_font[37].SetPixel(0, 3, 3);
            small_font[37].SetPixel(1, 3, 0);
            small_font[37].SetPixel(2, 3, 0);
           

            small_font[37].SetPixel(0, 4, 3);
            small_font[37].SetPixel(1, 4, 3);
            small_font[37].SetPixel(2, 4, 3);
            #endregion
            #region F
            small_font[38].SetPixel(0, 0, 3);
            small_font[38].SetPixel(1, 0, 3);
            small_font[38].SetPixel(2, 0, 3);
          

            small_font[38].SetPixel(0, 1, 3);
            small_font[38].SetPixel(1, 1, 3);
            small_font[38].SetPixel(2, 1, 3);
      

            small_font[38].SetPixel(0, 2, 3);
            small_font[38].SetPixel(1, 2, 10);
            small_font[38].SetPixel(2, 2, 10);
           

            small_font[38].SetPixel(0, 3, 3);
            small_font[38].SetPixel(1, 3, 3);
            small_font[38].SetPixel(2, 3, 3);
         

            small_font[38].SetPixel(0, 4, 3);
            small_font[38].SetPixel(1, 4, 10);
            small_font[38].SetPixel(2, 4, 10);
          
            #endregion
            #region G
            small_font[39].SetPixel(0, 0, 0);
            small_font[39].SetPixel(1, 0, 3);
            small_font[39].SetPixel(2, 0, 3);
           
            small_font[39].SetPixel(0, 1, 3);
            small_font[39].SetPixel(1, 1, 3);
            small_font[39].SetPixel(2, 1, 3);
            

            small_font[39].SetPixel(0, 2, 3);
            small_font[39].SetPixel(1, 2, 10);
            small_font[39].SetPixel(2, 2, 10);
           

            small_font[39].SetPixel(0, 3, 3);
            small_font[39].SetPixel(1, 3, 10);
            small_font[39].SetPixel(2, 3, 3);
           

            small_font[39].SetPixel(0, 4, 3);
            small_font[39].SetPixel(1, 4, 10);
            small_font[39].SetPixel(2, 4, 3);
            #endregion
            #region H
            small_font[40].SetPixel(0, 0, 3);
            small_font[40].SetPixel(1, 0, 0);
            small_font[40].SetPixel(2, 0, 3);
          

            small_font[40].SetPixel(0, 1, 3);
            small_font[40].SetPixel(1, 1, 0);
            small_font[40].SetPixel(2, 1, 3);
           

            small_font[40].SetPixel(0, 2, 3);
            small_font[40].SetPixel(1, 2, 3);
            small_font[40].SetPixel(2, 2, 3);
           

            small_font[40].SetPixel(0, 3, 3);
            small_font[40].SetPixel(1, 3, 0);
            small_font[40].SetPixel(2, 3, 3);
        

            small_font[40].SetPixel(0, 4, 3);
            small_font[40].SetPixel(1, 4, 0);
            small_font[40].SetPixel(2, 4, 3);
            #endregion
            #region I
            small_font[41].SetPixel(0, 0, 3);
            small_font[41].SetPixel(1, 0, 0);
            small_font[41].SetPixel(1, 0, 0);

            small_font[41].SetPixel(0, 1, 3);
            small_font[41].SetPixel(1, 1, 10);
            small_font[41].SetPixel(1, 0, 0);

            small_font[41].SetPixel(0, 2, 3);
            small_font[41].SetPixel(1, 2, 10);
            small_font[41].SetPixel(1, 0, 0);

            small_font[41].SetPixel(0, 3, 3);
            small_font[41].SetPixel(1, 3, 10);
            small_font[41].SetPixel(1, 0, 0);

            small_font[41].SetPixel(0, 4, 3);
            small_font[41].SetPixel(1, 4, 10);
            small_font[41].SetPixel(1, 0, 0);
            #endregion
            #region J
            small_font[42].SetPixel(0, 0, 0);
            small_font[42].SetPixel(1, 0, 0);
            small_font[42].SetPixel(2, 0, 3);
          

            small_font[42].SetPixel(0, 1, 0);
            small_font[42].SetPixel(1, 1, 0);
            small_font[42].SetPixel(2, 1, 3);
          

            small_font[42].SetPixel(0, 2, 0);
            small_font[42].SetPixel(1, 2, 0);
            small_font[42].SetPixel(2, 2, 3);
           

            small_font[42].SetPixel(0, 3, 0);
            small_font[42].SetPixel(1, 3, 0);
            small_font[42].SetPixel(2, 3, 3);
           

            small_font[42].SetPixel(0, 4, 0);
            small_font[42].SetPixel(1, 4, 0);
            small_font[42].SetPixel(2, 4, 3);
 #endregion
            #region K
            small_font[43].SetPixel(0, 0, 3);
            small_font[43].SetPixel(1, 0, 0);
            small_font[43].SetPixel(2, 0, 3);
     

            small_font[43].SetPixel(0, 1, 3);
            small_font[43].SetPixel(1, 1, 0);
            small_font[43].SetPixel(2, 1, 3);
         

            small_font[43].SetPixel(0, 2, 3);
            small_font[43].SetPixel(1, 2, 3);
            small_font[43].SetPixel(2, 2, 0);


            small_font[43].SetPixel(0, 3, 3);
            small_font[43].SetPixel(1, 3, 0);
            small_font[43].SetPixel(2, 3, 3);
     

            small_font[43].SetPixel(0, 4, 3);
            small_font[43].SetPixel(1, 4, 0);
            small_font[43].SetPixel(2, 4, 3);
            #endregion
            #region L
            small_font[44].SetPixel(0, 0, 3);
            small_font[44].SetPixel(1, 0, 0);
            small_font[44].SetPixel(2, 0, 0);
          

            small_font[44].SetPixel(0, 1, 3);
            small_font[44].SetPixel(1, 1, 0);
            small_font[44].SetPixel(2, 1, 0);
       

            small_font[44].SetPixel(0, 2, 3);
            small_font[44].SetPixel(1, 2, 0);
            small_font[44].SetPixel(2, 2, 0);
           
            small_font[44].SetPixel(0, 3, 3);
            small_font[44].SetPixel(1, 3, 0);
            small_font[44].SetPixel(2, 3, 0);
         

            small_font[44].SetPixel(0, 4, 3);
            small_font[44].SetPixel(1, 4, 3);
            small_font[44].SetPixel(2, 4, 3);      
            #endregion
            #region M
            small_font[45].SetPixel(0, 0, 3);
            small_font[45].SetPixel(1, 0, 3);
            small_font[45].SetPixel(2, 0, 0);
          

            small_font[45].SetPixel(0, 1, 3);
            small_font[45].SetPixel(1, 1, 3);
            small_font[45].SetPixel(2, 1, 3);
          

            small_font[45].SetPixel(0, 2, 3);
            small_font[45].SetPixel(1, 2, 10);
            small_font[45].SetPixel(2, 2, 3);
          

            small_font[45].SetPixel(0, 3, 3);
            small_font[45].SetPixel(1, 3, 10);
            small_font[45].SetPixel(2, 3, 3);
          

            small_font[45].SetPixel(0, 4, 3);
            small_font[45].SetPixel(1, 4, 10);
            small_font[45].SetPixel(2, 4, 3);
           

            #endregion
            #region N
            small_font[46].SetPixel(0, 0, 3);
            small_font[46].SetPixel(1, 0, 3);
            small_font[46].SetPixel(2, 0, 3);
        
            small_font[46].SetPixel(0, 1, 3);
            small_font[46].SetPixel(1, 1, 0);
            small_font[46].SetPixel(2, 1, 3);
            

            small_font[46].SetPixel(0, 2, 3);
            small_font[46].SetPixel(1, 2, 0);
            small_font[46].SetPixel(2, 2, 3);
           

            small_font[46].SetPixel(0, 3, 3);
            small_font[46].SetPixel(1, 3, 0);
            small_font[46].SetPixel(2, 3, 3);
           
            small_font[46].SetPixel(0, 4, 3);
            small_font[46].SetPixel(1, 4, 0);
            small_font[46].SetPixel(2, 4, 3);  
            #endregion
            #region O
            small_font[47].SetPixel(0, 0, 3);
            small_font[47].SetPixel(1, 0, 3);
            small_font[47].SetPixel(2, 0, 3);
           
            small_font[47].SetPixel(0, 1, 3);
            small_font[47].SetPixel(1, 1, 0);
            small_font[47].SetPixel(2, 1, 3);
            

            small_font[47].SetPixel(0, 2, 3);
            small_font[47].SetPixel(1, 2, 0);
            small_font[47].SetPixel(2, 2, 3);
          
            small_font[47].SetPixel(0, 3, 3);
            small_font[47].SetPixel(1, 3, 0);
            small_font[47].SetPixel(2, 3, 3);
       

            small_font[47].SetPixel(0, 4, 3);
            small_font[47].SetPixel(1, 4, 3);
            small_font[47].SetPixel(2, 4, 3);         
            #endregion
            #region P
            small_font[48].SetPixel(0, 0, 3);
            small_font[48].SetPixel(1, 0, 3);
            small_font[48].SetPixel(2, 0, 3);
         
            small_font[48].SetPixel(0, 1, 3);
            small_font[48].SetPixel(1, 1, 0);
            small_font[48].SetPixel(2, 1, 3);
          

            small_font[48].SetPixel(0, 2, 3);
            small_font[48].SetPixel(1, 2, 3);
            small_font[48].SetPixel(2, 2, 3);
           

            small_font[48].SetPixel(0, 3, 3);
            small_font[48].SetPixel(1, 3, 0);
            small_font[48].SetPixel(2, 3, 0);
           

            small_font[48].SetPixel(0, 4, 3);
            small_font[48].SetPixel(1, 4, 0);
            small_font[48].SetPixel(2, 4, 0);
            #endregion
            #region Q
            small_font[49].SetPixel(0, 0, 0);
            small_font[49].SetPixel(1, 0, 3);
            small_font[49].SetPixel(2, 0, 0);
         
            small_font[49].SetPixel(0, 1, 3);
            small_font[49].SetPixel(1, 1, 3);
            small_font[49].SetPixel(2, 1, 3);
          

            small_font[49].SetPixel(0, 2, 3);
            small_font[49].SetPixel(1, 2, 10);
            small_font[49].SetPixel(2, 2, 3);
          

            small_font[49].SetPixel(0, 3, 3);
            small_font[49].SetPixel(1, 3, 10);
            small_font[49].SetPixel(2, 3, 3);
          

            small_font[49].SetPixel(0, 4, 3);
            small_font[49].SetPixel(1, 4, 10);
            small_font[49].SetPixel(2, 4, 3);


          
            #endregion
            #region R
            small_font[50].SetPixel(0, 0, 3);
            small_font[50].SetPixel(1, 0, 3);
            small_font[50].SetPixel(2, 0, 0);
          

            small_font[50].SetPixel(0, 1, 3);
            small_font[50].SetPixel(1, 1, 0);
            small_font[50].SetPixel(2, 1, 3);
         
            small_font[50].SetPixel(0, 2, 3);
            small_font[50].SetPixel(1, 2, 3);
            small_font[50].SetPixel(2, 2, 0);
           

            small_font[50].SetPixel(0, 3, 3);
            small_font[50].SetPixel(1, 3, 0);
            small_font[50].SetPixel(2, 3, 3);
           

            small_font[50].SetPixel(0, 4, 3);
            small_font[50].SetPixel(1, 4, 0);
            small_font[50].SetPixel(2, 4, 3);
            

            #endregion
            #region S
            small_font[51].SetPixel(0, 0, 3);
            small_font[51].SetPixel(1, 0, 3);
            small_font[51].SetPixel(2, 0, 3);
           

            small_font[51].SetPixel(0, 1, 3);
            small_font[51].SetPixel(1, 1, 0);
            small_font[51].SetPixel(2, 1, 0);
         

            small_font[51].SetPixel(0, 2, 3);
            small_font[51].SetPixel(1, 2, 3);
            small_font[51].SetPixel(2, 2, 3);
          

            small_font[51].SetPixel(0, 3, 0);
            small_font[51].SetPixel(1, 3, 0);
            small_font[51].SetPixel(2, 3, 3);
           

            small_font[51].SetPixel(0, 4, 3);
            small_font[51].SetPixel(1, 4, 3);
            small_font[51].SetPixel(2, 4, 3);
            #endregion
            #region T
            small_font[52].SetPixel(0, 0, 3);
            small_font[52].SetPixel(1, 0, 3);
            small_font[52].SetPixel(2, 0, 3);
         
            small_font[52].SetPixel(0, 1, 0);
            small_font[52].SetPixel(1, 1, 3);
            small_font[52].SetPixel(2, 1, 0);
            

            small_font[52].SetPixel(0, 2, 0);
            small_font[52].SetPixel(1, 2, 3);
            small_font[52].SetPixel(2, 2, 0);
         

            small_font[52].SetPixel(0, 3, 0);
            small_font[52].SetPixel(1, 3, 3);
            small_font[52].SetPixel(2, 3, 0);
            

            small_font[52].SetPixel(0, 4, 0);
            small_font[52].SetPixel(1, 4, 3);
            small_font[52].SetPixel(2, 4, 0);
        
         
            #endregion
            #region U
            small_font[53].SetPixel(0, 0, 3);
            small_font[53].SetPixel(1, 0, 0);
            small_font[53].SetPixel(2, 0, 3);
          

            small_font[53].SetPixel(0, 1, 3);
            small_font[53].SetPixel(1, 1, 0);
            small_font[53].SetPixel(2, 1, 3);
           

            small_font[53].SetPixel(0, 2, 3);
            small_font[53].SetPixel(1, 2, 0);
            small_font[53].SetPixel(2, 2, 3);
            

            small_font[53].SetPixel(0, 3, 3);
            small_font[53].SetPixel(1, 3, 0);
            small_font[53].SetPixel(2, 3, 3);
          

            small_font[53].SetPixel(0, 4, 3);
            small_font[53].SetPixel(1, 4, 3);
            small_font[53].SetPixel(2, 4, 3);
            

          
            #endregion
            #region V
            small_font[54].SetPixel(0, 0, 3);
            small_font[54].SetPixel(1, 0, 0);
            small_font[54].SetPixel(2, 0, 3);
           

            small_font[54].SetPixel(0, 1, 3);
            small_font[54].SetPixel(1, 1, 10);
            small_font[54].SetPixel(2, 1, 3);
          

            small_font[54].SetPixel(0, 2, 3);
            small_font[54].SetPixel(1, 2, 10);
            small_font[54].SetPixel(2, 2, 3);
           

            small_font[54].SetPixel(0, 3, 3);
            small_font[54].SetPixel(1, 3, 10);
            small_font[54].SetPixel(2, 3, 3);
           

            small_font[54].SetPixel(0, 4, 3);
            small_font[54].SetPixel(1, 4, 10);
            small_font[54].SetPixel(2, 4, 3);
           

            #endregion
            #region W
            small_font[55].SetPixel(0, 0, 3);
            small_font[55].SetPixel(1, 0, 0);
            small_font[55].SetPixel(2, 0, 3);
          
            small_font[55].SetPixel(0, 1, 3);
            small_font[55].SetPixel(1, 1, 0);
            small_font[55].SetPixel(2, 1, 3);
            

            small_font[55].SetPixel(0, 2, 3);
            small_font[55].SetPixel(1, 2, 0);
            small_font[55].SetPixel(2, 2, 3);
          

            small_font[55].SetPixel(0, 3, 3);
            small_font[55].SetPixel(1, 3, 3);
            small_font[55].SetPixel(2, 3, 3);
           

            small_font[55].SetPixel(0, 4, 3);
            small_font[55].SetPixel(1, 4, 0);
            small_font[55].SetPixel(2, 4, 3);

            #endregion
            #region X
            small_font[56].SetPixel(0, 0, 3);
            small_font[56].SetPixel(1, 0, 0);
            small_font[56].SetPixel(2, 0, 3);
         
            small_font[56].SetPixel(0, 1, 3);
            small_font[56].SetPixel(1, 1, 10);
            small_font[56].SetPixel(2, 1, 3);
         

            small_font[56].SetPixel(0, 2, 3);
            small_font[56].SetPixel(1, 2, 10);
            small_font[56].SetPixel(2, 2, 3);
           
            small_font[56].SetPixel(0, 3, 0);
            small_font[56].SetPixel(1, 3, 3);
            small_font[56].SetPixel(2, 3, 0);
          
            small_font[56].SetPixel(0, 4, 3);
            small_font[56].SetPixel(1, 4, 0);
            small_font[56].SetPixel(2, 4, 3);         
            #endregion
            #region Y
            small_font[57].SetPixel(0, 0, 3);
            small_font[57].SetPixel(1, 0, 0);
            small_font[57].SetPixel(2, 0, 3);
           
            small_font[57].SetPixel(0, 1, 3);
            small_font[57].SetPixel(1, 1, 10);
            small_font[57].SetPixel(2, 1, 3);
           

            small_font[57].SetPixel(0, 2, 3);
            small_font[57].SetPixel(1, 2, 10);
            small_font[57].SetPixel(2, 2, 3);
           

            small_font[57].SetPixel(0, 3, 3);
            small_font[57].SetPixel(1, 3, 3);
            small_font[57].SetPixel(2, 3, 3);
           

            small_font[57].SetPixel(0, 4, 0);
            small_font[57].SetPixel(1, 4, 3);
            small_font[57].SetPixel(2, 4, 10);
          

           
            #endregion
            #region Z
            small_font[58].SetPixel(0, 0, 3);
            small_font[58].SetPixel(1, 0, 3);
            small_font[58].SetPixel(2, 0, 3);
          

            small_font[58].SetPixel(0, 1, 3);
            small_font[58].SetPixel(1, 1, 3);
            small_font[58].SetPixel(2, 1, 3);
        

            small_font[58].SetPixel(0, 2, 0);
            small_font[58].SetPixel(1, 2, 10);
            small_font[58].SetPixel(2, 2, 3);
           

            small_font[58].SetPixel(0, 3, 0);
            small_font[58].SetPixel(1, 3, 3);
            small_font[58].SetPixel(2, 3, 0);
           

            small_font[58].SetPixel(0, 4, 3);
            small_font[58].SetPixel(1, 4, 0);
            small_font[58].SetPixel(2, 4, 10);
   
            #endregion
            #region 0
            small_font[16].SetPixel(0, 0, 3);
            small_font[16].SetPixel(1, 0, 3);
            small_font[16].SetPixel(2, 0, 3);
          

            small_font[16].SetPixel(0, 1, 3);
            small_font[16].SetPixel(1, 1, 0);
            small_font[16].SetPixel(2, 1, 3);
            

            small_font[16].SetPixel(0, 2, 3);
            small_font[16].SetPixel(1, 2, 0);
            small_font[16].SetPixel(2, 2, 3);
           

            small_font[16].SetPixel(0, 3, 3);
            small_font[16].SetPixel(1, 3, 0);
            small_font[16].SetPixel(2, 3, 3);
         

            small_font[16].SetPixel(0, 4, 3);
            small_font[16].SetPixel(1, 4, 3);
            small_font[16].SetPixel(2, 4, 3);
            #endregion
            #region 1
            small_font[17].SetPixel(0, 0, 0);
            small_font[17].SetPixel(1, 0, 0);
            small_font[17].SetPixel(2, 0, 3);
            

            small_font[17].SetPixel(0, 1, 0);
            small_font[17].SetPixel(1, 1, 0);
            small_font[17].SetPixel(2, 1, 3);
            

            small_font[17].SetPixel(0, 2, 0);
            small_font[17].SetPixel(1, 2, 0);
            small_font[17].SetPixel(2, 2, 3);
          

            small_font[17].SetPixel(0, 3, 0);
            small_font[17].SetPixel(1, 3, 0);
            small_font[17].SetPixel(2, 3, 3);
           

            small_font[17].SetPixel(0, 4, 0);
            small_font[17].SetPixel(1, 4, 0);
            small_font[17].SetPixel(2, 4, 3);
            #endregion
            #region 2
            small_font[18].SetPixel(0, 0, 3);
            small_font[18].SetPixel(1, 0, 3);
            small_font[18].SetPixel(2, 0, 3);
            
            small_font[18].SetPixel(0, 1, 0);
            small_font[18].SetPixel(1, 1, 0);
            small_font[18].SetPixel(2, 1, 3);

            small_font[18].SetPixel(0, 2, 3);
            small_font[18].SetPixel(1, 2, 3);
            small_font[18].SetPixel(2, 2, 3);
          
            small_font[18].SetPixel(0, 3, 3);
            small_font[18].SetPixel(1, 3, 0);
            small_font[18].SetPixel(2, 3, 0);
      
            small_font[18].SetPixel(0, 4, 3);
            small_font[18].SetPixel(1, 4, 3);
            small_font[18].SetPixel(2, 4, 3);
            #endregion
            #region 3
            small_font[19].SetPixel(0, 0, 3);
            small_font[19].SetPixel(1, 0, 3);
            small_font[19].SetPixel(2, 0, 3);

            small_font[19].SetPixel(0, 1, 0);
            small_font[19].SetPixel(1, 1, 0);
            small_font[19].SetPixel(2, 1, 3);
           
            small_font[19].SetPixel(0, 2, 0);
            small_font[19].SetPixel(1, 2, 3);
            small_font[19].SetPixel(2, 2, 3);
            
            small_font[19].SetPixel(0, 3, 0);
            small_font[19].SetPixel(1, 3, 0);
            small_font[19].SetPixel(2, 3, 3);
           
            small_font[19].SetPixel(0, 4, 3);
            small_font[19].SetPixel(1, 4, 3);
            small_font[19].SetPixel(2, 4, 3);
            
            #endregion
            #region 4
            small_font[20].SetPixel(0, 0, 3);
            small_font[20].SetPixel(1, 0, 0);
            small_font[20].SetPixel(2, 0, 3);
          
            small_font[20].SetPixel(0, 1, 3);
            small_font[20].SetPixel(1, 1, 0);
            small_font[20].SetPixel(2, 1, 3);
           
            small_font[20].SetPixel(0, 2, 3);
            small_font[20].SetPixel(1, 2, 3);
            small_font[20].SetPixel(2, 2, 3);
           
            small_font[20].SetPixel(0, 3, 0);
            small_font[20].SetPixel(1, 3, 0);
            small_font[20].SetPixel(2, 3, 3);
           
            small_font[20].SetPixel(0, 4, 0);
            small_font[20].SetPixel(1, 4, 0);
            small_font[20].SetPixel(2, 4, 3);
            #endregion
            #region 5
            small_font[21].SetPixel(0, 0, 3);
            small_font[21].SetPixel(1, 0, 3);
            small_font[21].SetPixel(2, 0, 3);
           
            small_font[21].SetPixel(0, 1, 3);
            small_font[21].SetPixel(1, 1, 0);
            small_font[21].SetPixel(2, 1, 0);
          
            small_font[21].SetPixel(0, 2, 3);
            small_font[21].SetPixel(1, 2, 3);
            small_font[21].SetPixel(2, 2, 3);
           
            small_font[21].SetPixel(0, 3, 0);
            small_font[21].SetPixel(1, 3, 0);
            small_font[21].SetPixel(2, 3, 3);
          
            small_font[21].SetPixel(0, 4, 3);
            small_font[21].SetPixel(1, 4, 3);
            small_font[21].SetPixel(2, 4, 3);
           
            #endregion
            #region 6
            small_font[22].SetPixel(0, 0, 3);
            small_font[22].SetPixel(1, 0, 3);
            small_font[22].SetPixel(2, 0, 3);
          
            small_font[22].SetPixel(0, 1, 3);
            small_font[22].SetPixel(1, 1, 0);
            small_font[22].SetPixel(2, 1, 0);
          
            small_font[22].SetPixel(0, 2, 3);
            small_font[22].SetPixel(1, 2, 3);
            small_font[22].SetPixel(2, 2, 3);
           

            small_font[22].SetPixel(0, 3, 3);
            small_font[22].SetPixel(1, 3, 0);
            small_font[22].SetPixel(2, 3, 3);
            

            small_font[22].SetPixel(0, 4, 3);
            small_font[22].SetPixel(1, 4, 3);
            small_font[22].SetPixel(2, 4, 3);
            #endregion
            #region 7
            small_font[23].SetPixel(0, 0, 3);
            small_font[23].SetPixel(1, 0, 3);
            small_font[23].SetPixel(2, 0, 3);
            
            small_font[23].SetPixel(0, 1, 0);
            small_font[23].SetPixel(1, 1, 0);
            small_font[23].SetPixel(2, 1, 3);
           
            small_font[23].SetPixel(0, 2, 0);
            small_font[23].SetPixel(1, 2, 0);
            small_font[23].SetPixel(2, 2, 3);
           
            small_font[23].SetPixel(0, 3, 0);
            small_font[23].SetPixel(1, 3, 0);
            small_font[23].SetPixel(2, 3, 3);
          
            small_font[23].SetPixel(0, 4, 0);
            small_font[23].SetPixel(1, 4, 0);
            small_font[23].SetPixel(2, 4, 3);
            #endregion
            #region 8
            small_font[24].SetPixel(0, 0, 3);
            small_font[24].SetPixel(1, 0, 3);
            small_font[24].SetPixel(2, 0, 3);
           
            small_font[24].SetPixel(0, 1, 3);
            small_font[24].SetPixel(1, 1, 0);
            small_font[24].SetPixel(2, 1, 3);
            
            small_font[24].SetPixel(0, 2, 3);
            small_font[24].SetPixel(1, 2, 3);
            small_font[24].SetPixel(2, 2, 3);
         
            small_font[24].SetPixel(0, 3, 3);
            small_font[24].SetPixel(1, 3, 0);
            small_font[24].SetPixel(2, 3, 3);
           
            small_font[24].SetPixel(0, 4, 3);
            small_font[24].SetPixel(1, 4, 3);
            small_font[24].SetPixel(2, 4, 3);
            #endregion
            #region 9
            small_font[25].SetPixel(0, 0, 3);
            small_font[25].SetPixel(1, 0, 3);
            small_font[25].SetPixel(2, 0, 3);
           
            small_font[25].SetPixel(0, 1, 3);
            small_font[25].SetPixel(1, 1, 0);
            small_font[25].SetPixel(2, 1, 3);
           
            small_font[25].SetPixel(0, 2, 3);
            small_font[25].SetPixel(1, 2, 3);
            small_font[25].SetPixel(2, 2, 3);
           
            small_font[25].SetPixel(0, 3, 0);
            small_font[25].SetPixel(1, 3, 0);
            small_font[25].SetPixel(2, 3, 3);
           
            small_font[25].SetPixel(0, 4, 3);
            small_font[25].SetPixel(1, 4, 3);
            small_font[25].SetPixel(2, 4, 3);
            #endregion
            #region Colon
            small_font[26].SetPixel(0, 0, 0);
            small_font[26].SetPixel(0, 1, 3);
           small_font[26].SetPixel(0, 2, 0);
           small_font[26].SetPixel(0, 3, 3);
          small_font[26].SetPixel(0, 4, 0);
               #endregion
            #endregion
        }
        private void InitializePalette()
        {
            //create general palette
           // colorpalette[0] = System.Drawing.Color.FromArgb(255, 128, 0, 128);  //color 
            colorpalette[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);   //transparent
            colorpalette[1] = System.Drawing.Color.FromArgb(64, 64, 240);
            colorpalette[2] = System.Drawing.Color.FromArgb(40, 40, 224);
            colorpalette[3] = System.Drawing.Color.FromArgb(0, 208, 0);
            colorpalette[4] = System.Drawing.Color.FromArgb(248, 0, 00);
            colorpalette[5] = System.Drawing.Color.FromArgb(0, 248, 00);
            colorpalette[6] = System.Drawing.Color.FromArgb(255, 255, 0);
            colorpalette[7] = System.Drawing.Color.FromArgb(248, 248, 248);
            colorpalette[8] = System.Drawing.Color.FromArgb(0, 192, 00);
            colorpalette[9] = System.Drawing.Color.FromArgb(216, 200, 00);
            colorpalette[10] = System.Drawing.Color.FromArgb(0, 0, 00);
            colorpalette[11] = System.Drawing.Color.FromArgb(0, 144, 00);
            colorpalette[12] = System.Drawing.Color.FromArgb(176, 156, 00);
            colorpalette[13] = System.Drawing.Color.FromArgb(0, 104, 00);
            colorpalette[14] = System.Drawing.Color.FromArgb(192, 192, 192);
            colorpalette[15] = System.Drawing.Color.FromArgb(152, 152, 152);

        }
        private byte[] getTileFromArray(int[,] backgroundArray, int startx, int starty)
        {
            byte[] tile = new byte[64];
            int counter = 0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tile[counter] = (byte)backgroundArray[startx + j, starty + i];
                    counter++;
                }
            }

            return tile;
        }
        private Bitmap tile2bitmap(byte[] tile_data, Color[] palette, int tile_size_X, int tile_size_Y)
        {
            //tile_size_X,tile_size_Y are tile sizes generated from size of the pictureBox

            int counter = 0;
            int pixelX = tile_size_X / 8;
            int pixelY = tile_size_Y / 8;
            int palette_color = 0;

            Bitmap tilebitmap = new Bitmap(tile_size_X, tile_size_Y);

            //adjust pixel size based on bitmap size. 
            for (int y = 0; y < 8; y++) //8 here is the original tile height
            {
                for (int x = 0; x < 8; x++) //8 here is the original tile width
                {
                    for (int q = 0; q < pixelX; q++)
                    {
                        if (tile_data[counter] == 3)
                            //palette_color = fontColor;
                            palette_color = (int)theFontColorOptions;
                        else
                            palette_color = tile_data[counter];

                        tilebitmap.SetPixel((x * (pixelX)) + q, (y * pixelY), palette[palette_color]);
                        for (int r = 0; r < pixelY; r++)
                        {
                            tilebitmap.SetPixel((x * (pixelX)) + q, (y * pixelY) + r, palette[palette_color]);
                        }
                    }
                    counter++;
                }
            }
            return tilebitmap;
        }
        public void setOffsetX(int x)
        {
            offsetx = x;
            redrawFlag = true;
            this.Invalidate();
        }
        public void setOffsetY(int y)
        {
            offsety = y;
            redrawFlag = true;
            this.Invalidate();
        }

        //sets the font color from the palette (0-15)
        public void setFontColorbyIndex(int color)
        {
            FontColorOptions test = (FontColorOptions)color;
            theFontColorOptions = test;
            redrawFlag = true;
            this.Invalidate();
        }
        
        //returns the size of the linear array 
        public int getLinearArraySize()
        {
            //32 bytes per tile
            return 32 * tile_height * tile_width;
        }
        
        //returns the linear, flattened array (size based on image size)
        public byte[] getLinearArray()
        {
            byte[] flat_4bpp_array = new byte[getLinearArraySize()];
            byte[] tile = new byte[32];

            int locationx = 0;
            int locationy = 0;
            int counter = 0;
            int offset = 0;

            for (int z = 0; z < tile_height; z++)
            {
                for (int j = 0; j < tile_width; j++)
                {
                    tile = get4bpp(0, locationx, locationy);

                    for (int i = 0; i < 8; i++)
                    {
                        flat_4bpp_array[offset] = tile[(counter)];
                        flat_4bpp_array[offset + 1] = tile[(counter + 1)];
                        flat_4bpp_array[offset + 16] = tile[(counter + 2)];
                        flat_4bpp_array[offset + 17] = tile[(counter + 3)];

                        offset = offset + 2;
                        counter = counter + 4;
                    }
                    locationx = locationx + 8;
                    counter = 0;
                    offset = offset + 16;
                }
                locationx = 0;
                locationy = locationy + 8;
            }

            return flat_4bpp_array;
        }
        
        //returns byte[32] array
        private byte[] get4bpp(int address, int startx, int starty)
        {
            int offset = address;
            byte[] layers = new byte[4];
            byte[] temp = new byte[4];
            byte[] pixels = new byte[8];
            int counter = 0;

            byte[] tile = new byte[32];

            for (int y = 0; y < 8; y++)
            {
                for (int i = 0; i < 8; i++)
                {
                    pixels[i] = (byte)savedbackArray[startx + i, starty + y];

                    temp[0] = (byte)((pixels[i] >> 0) & 1);
                    temp[1] = (byte)((pixels[i] >> 1) & 1);
                    temp[2] = (byte)((pixels[i] >> 2) & 1);
                    temp[3] = (byte)((pixels[i] >> 3) & 1);

                    layers[0] = (byte)(layers[0] << 1);
                    layers[0] = (byte)(layers[0] | temp[0]);
                    layers[1] = (byte)(layers[1] << 1);
                    layers[1] = (byte)(layers[1] | temp[1]);
                    layers[2] = (byte)(layers[2] << 1);
                    layers[2] = (byte)(layers[2] | temp[2]);
                    layers[3] = (byte)(layers[3] << 1);
                    layers[3] = (byte)(layers[3] | temp[3]);

                    tile[counter] = layers[0];
                    tile[counter + 1] = layers[1];
                    tile[counter + 2] = layers[2];
                    tile[counter + 3] = layers[3];
                }

                counter = counter + 4;
            }

            return tile;

        }
    }

    public class fontTile
    {
        public int Width;
        public int Height;
        private int[,] pixels;

        public fontTile(int x, int y)
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
