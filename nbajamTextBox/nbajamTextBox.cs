using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace nbajamTextBox
{
    public partial class nbajamTextBox : PictureBox
    {
        //Properties
        //font definition?
        //bit arrays?
        //palette definition?
        private int tile_width = 1;                                                     // Width of the control in 8px*8px tiles
        private int tile_height = 1;                                                    // Height of the control in 8px*8px tiles
        private int scale_factor = 1;                                                   // Use the scale factor to maintain the control's physical size
        //private int fontColor = 1;                                                    // Color to use from the palette
        private String internal_text = "DEMO";                                          // The text to display on the control  
        private System.Drawing.Color[] colorpalette = new System.Drawing.Color[16];     // A general Color palette
        fontTile[] letters = new fontTile[60];                                          // Object to hold the font data/color
        private Bitmap displayBitmap;                                                   // What the user sees on the control
        private Bitmap[] tiles;                                                         // create array of tiles based on known number of required tiles
        private int[,] backArray;                                                       // An array to hold raw pixel data (Need to optimise - use something instead of int...) 
        private byte[][] tiletest;              //i have no idea??? 

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
                this.Size = new Size(8 * tile_width * scale_factor, 8 * tile_height * scale_factor);
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
                this.Size = new Size(8 * tile_width * scale_factor, 8 * tile_height * scale_factor);
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
                this.Size = new Size(8 * tile_width * scale_factor, 8 * tile_height * scale_factor);   
            }
        }
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
                internal_text = value;
            }
        }

        public nbajamTextBox()
        {
            InitializeComponent();
            InitializePalette();
            InitializeFont();

            int z;
            int f = 0;
            int[] textname= new int[internal_text.Length];

            byte[][] tiletest = new byte[tile_width*tile_height][];
            // Define pixel array based on number of pixels actually needed
            backArray = new int[8*tile_width, 8*tile_height];
            // Create new tiles based on amount needed
            tiles = new Bitmap[tile_width * tile_height];
            // Initialize each tile in the array to the proper size based on the scale factor
            for(int i=0;i<(tile_width * tile_height);i++)
            {
                tiles[i] = new Bitmap(scale_factor*tile_width, scale_factor*tile_height);
                tiletest[i] = new byte[64];
            }

            foreach (char c in internal_text)
            {
                z  = (int)(c - 32); //65 = A
                //this offsets the array for searching
                //       Console.WriteLine(Convert.ToInt16(z).ToString());
                textname[f] = z;
                f++;
            }
        }

        private void InitializeFont()
        {
            //Define height and width for each letter
            #region Height and Width Definitions
            letters[0] = new fontTile(4, 8);
            letters[1] = new fontTile(2, 8);
            letters[14] = new fontTile(2, 8);
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
            letters[54].SetPixel(2, 5, 0);
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
        }
        private void InitializePalette()
        {
            //create general palette
            colorpalette[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);   //transparent
            colorpalette[1] = System.Drawing.Color.FromArgb(64, 64, 240);
            colorpalette[2] = System.Drawing.Color.FromArgb(40, 40, 224);
            colorpalette[3] = System.Drawing.Color.FromArgb(0, 208, 0);
            colorpalette[4] = System.Drawing.Color.FromArgb(248, 0, 00);
            colorpalette[5] = System.Drawing.Color.FromArgb(0, 248, 00);
            colorpalette[6] = System.Drawing.Color.FromArgb(0, 0, 192);
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
