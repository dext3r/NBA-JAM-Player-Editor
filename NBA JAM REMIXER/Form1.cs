using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;



namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        System.Drawing.Color[] nametag_pallete = new System.Drawing.Color[16]; // used as general palette - could move to Constants class
        fontTile[] letters = new fontTile[60];
        int[,] backArray = new int[48, 16];  //array for nametag data
        int[,] backArray2 = new int[48, 16];  //array for nametag data
        List<playerData> nbajam_players = new List<playerData>();


        byte[] fileBuffer;

     
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            int Color = new UInt16();

            
            // this file loading stuff shouldnt really be in the Click method ... ok for now though ...

           /// string balls = "C:\\Users\\dext3r\\Desktop\\NBA Jam - Tournament Edition (U) [!].smc";
            
             //   Console.WriteLine("\n" + dlg.FileName);
                int count; 
                int start_address = new int();
                int red, green, blue;
                byte[] portrait = new byte[Constants.portraitBitmapSize];
                byte[] palette = new byte[Constants.portraitPaletteSize];
               // byte[] buffer;
                byte[] layer = new byte[5];
                byte[] pixelbuilder = new byte[5];
                byte[] pixel_row = new byte[8];
                byte[] teh_pixels = new byte[64];
                System.Drawing.Bitmap palette_image = new System.Drawing.Bitmap(10, 10);
                System.Drawing.Color[] palette_swatch = new System.Drawing.Color[32];
                Bitmap paletteBitmap = new Bitmap(pictureBox2.Width,pictureBox2.Height);
                Bitmap portraitBitmap = new Bitmap(48,56);
                Bitmap[,] tile = new Bitmap[6,7];
                Bitmap[] tiles = new Bitmap[42];

                for (int i = 0; i < 42; i++)
                {
                        tiles[i] = new Bitmap(8, 8);
                }
                //transfer single tile to location in global bitmap somehow

             /*   FileStream fileStream = new FileStream(balls, FileMode.Open, FileAccess.Read);
                try
                {
                    int length = (int)fileStream.Length;  // get file length
                    buffer = new byte[length];            // create buffer
                                              // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                        sum += count;  // sum is a buffer offset for next reading
                }
                finally
                {
                    fileStream.Close();
                }
               */
                start_address = Convert.ToInt32(textBox1.Text);
                count = 0;
            //    Console.WriteLine("PORTRAIT DATA:");
                for (int i = 0; i < Constants.portraitBitmapSize; i++)
                {
                    portrait[i] = fileBuffer[start_address];
               //     Console.Write(portrait[i].ToString("X2"));
                    start_address++;
                    count++;
                }
            //    Console.WriteLine("Count: " + count.ToString());
            //    Console.WriteLine();
                count = 0;
            //    Console.WriteLine("PALETTE DATA:");
                for (int j = 0; j < Constants.portraitPaletteSize; j++)
                {
                    palette[j] = fileBuffer[start_address];
             //       Console.Write(palette[j].ToString("X2"));
                    start_address++;
                    count++;
                }
             //   Console.WriteLine("Count: " + count.ToString());
              //  Console.WriteLine();
              
                int offset_x = 0;
                int offset_y = 0; 
                int counto = 0;
                int pixelcounter = 0;
                //getting colors here
                for (int i = 0; i < Constants.portraitPaletteSize; i = i + 2)
                {  
                   
                    Color = palette[i + 1];
                    Color = Color << 8;
                    Color = Color + palette[i];
            ///        Console.WriteLine("Color = " + Color.ToString());

                    red = (Color % 32) * 8;
                    green = ((Color / 32) % 32) * 8;
                    blue = ((Color / 1024) % 32) * 8;

          ///          Console.WriteLine("Red: " + red.ToString());
          ///          Console.WriteLine("Green: " + green.ToString());
          ///          Console.WriteLine("Blue: " + blue.ToString());

                    palette_swatch[counto] = System.Drawing.Color.FromArgb(red, green, blue);
                    counto++;
                }
       //palette drawing hack
                for (int f = 0; f < 16; f++)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {                 
                            paletteBitmap.SetPixel((offset_x+ j), (offset_y + i), palette_swatch[f]);
                         }
                     }

                    offset_x = offset_x + 8;
                 }

                offset_x = 0;
                offset_y = offset_y + 14;
//2nd row of palette drawing
                for (int f = 16; f < 32; f++)
                {
                    for (int i = 0; i < 14; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            paletteBitmap.SetPixel((offset_x + j), (offset_y + i), palette_swatch[f]);
                        }
                    }

                    offset_x = offset_x + 8;
                }

                pictureBox2.Image = paletteBitmap;

        
                
                int initial_offset = 0;
                int secondary_offset = initial_offset + 32;

                //this gets 42 tiles

                for (int total_tiles = 0; total_tiles < 42; total_tiles++)
                {
                    //the secondary_offset is 32 away from the initial_offset
                    //but only increments by 1 every loop pass
                    secondary_offset = initial_offset + 32;

                    //debug
                   // Console.WriteLine("Tile #" + total_tiles.ToString());
                   
                    //this gets a tiles worth of data
                    for (int i = 0; i < 8; i++)
                    { 
                        //debug
                        //Console.WriteLine(initial_offset.ToString() + "," + (initial_offset + 1).ToString() + "," + (initial_offset + 16).ToString() + "," + (initial_offset + 17).ToString() + "," + secondary_offset.ToString());
                       
                        layer[0] = portrait[initial_offset];
                        layer[1] = portrait[initial_offset + 1];
                        layer[2] = portrait[initial_offset + 16];
                        layer[3] = portrait[initial_offset + 17];
                        layer[4] = portrait[secondary_offset];

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
                            tiles[total_tiles].SetPixel(a, i, palette_swatch[teh_pixels[pixelcounter]]);
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
                }

            
            Bitmap temp = portraitBitmap;
            Bitmap bmap = (Bitmap)temp.Clone();
            Graphics gr = Graphics.FromImage(bmap);
            Rectangle rect = new Rectangle(0, 0, 8, 8);
            //tiles[7].RotateFlip(RotateFlipType.RotateNoneFlipX);
            int tilecounter=0;

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    rect.X = j*8;
                    rect.Y = i * 8;
                  //  Console.WriteLine(j.ToString() + "," + i.ToString() + "[" + tilecounter.ToString() + "]");
                    tiles[tilecounter].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    gr.DrawImage(tiles[tilecounter], rect);
                    tilecounter++;
                }
              
            }

            portraitBitmap = (Bitmap)bmap.Clone();

           
           // pictureBox1.Image = tiles[15];
            pictureBox1.Image = portraitBitmap;
             
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "No file open.";
            //textBox1.Text = "2232681";
            textBox1.Text = "2234425"; // ??
         //   textBox2.Text = " 1363877"; //pippen
            textBox2.Text = "1398188";
            initializePalette();
            initializeFont();
            initializePlayers();
          
         
           
        }

        private void initializePlayers()
        {
            //ID,name,picture,nametag,stats,secret?
            nbajam_players.Add(new playerData(0, "BJ Armstrong", 2241401, 1398188, 1398188, false));
            
        }

        private void initializeFont()
        {
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

            
            //letters are defined in the following regions:

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
        }

        private void initializePalette()
        {
            //create general palette
            nametag_pallete[0] = System.Drawing.Color.FromArgb(0, 0, 0, 0);   //transparent
            nametag_pallete[1] = System.Drawing.Color.FromArgb(64, 64, 240);
            nametag_pallete[2] = System.Drawing.Color.FromArgb(40, 40, 224);
            nametag_pallete[3] = System.Drawing.Color.FromArgb(0, 208, 0);
            nametag_pallete[4] = System.Drawing.Color.FromArgb(248, 0, 00);
            nametag_pallete[5] = System.Drawing.Color.FromArgb(0, 248, 00);
            nametag_pallete[6] = System.Drawing.Color.FromArgb(0, 0, 192);
            nametag_pallete[7] = System.Drawing.Color.FromArgb(248, 248, 248);
            nametag_pallete[8] = System.Drawing.Color.FromArgb(0, 192, 00);
            nametag_pallete[9] = System.Drawing.Color.FromArgb(216, 200, 00);
            nametag_pallete[10] = System.Drawing.Color.FromArgb(0, 0, 00);
            nametag_pallete[11] = System.Drawing.Color.FromArgb(0, 144, 00);
            nametag_pallete[12] = System.Drawing.Color.FromArgb(176, 156, 00);
            nametag_pallete[13] = System.Drawing.Color.FromArgb(0, 104, 00);
            nametag_pallete[14] = System.Drawing.Color.FromArgb(192, 192, 192);
            nametag_pallete[15] = System.Drawing.Color.FromArgb(152, 152, 152);
            
        }

        private void test()
        {
            Console.WriteLine("Test");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox1.Text);
            temp = temp + 1744;
            textBox1.Text = temp.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox1.Text);
            temp = temp - 1744;
            textBox1.Text = temp.ToString();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Image rawr = pictureBox1.Image;
                rawr.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = rawr;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //string balls = "C:\\Users\\dext3r\\Desktop\\NBA Jam - Tournament Edition (U) [!].smc";
           // string balls = "C:\\Users\\dext3r\\Desktop\\nbajam.smc.smc";
            int start_address = 0;
            int count = 0;
            //byte[] buffer;
            byte[] nametag = new byte[Constants.nametagSize];
            byte[] tile_buffer = new byte[32];
            byte[] tile_pixels = new byte[64];
            byte[] temp = new byte[4];

            int nametag_corrected_width = (pictureBox3.Width / Constants.num_nametag_tilesX);
            int nametag_corrected_height = (pictureBox3.Height / Constants.num_nametag_tilesY);
            int nametag_total_tiles = (Constants.num_nametag_tilesX * Constants.num_nametag_tilesY);

            Bitmap nametagBitmap = new Bitmap(nametag_corrected_width * Constants.num_nametag_tilesX, nametag_corrected_width * Constants.num_nametag_tilesY);
            Bitmap[] tiles = new Bitmap[nametag_total_tiles]; //create array of tiles based on known number of required tiles


            
            for (int i = 0; i < 12; i++)
            {
                //each tile has to be scaled based on the parent picturebox size...
                tiles[i] = new Bitmap(nametag_corrected_width,nametag_corrected_height);
            }


            if (checkBox2.Checked)
            {

                nametag_pallete[0] = System.Drawing.Color.FromArgb(255, 128,0, 128);  //color         
            }
            else
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(0, 128, 0, 128);  //transparent    
            }
         /*   FileStream fileStream = new FileStream(balls, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            */
            start_address = Convert.ToInt32(textBox2.Text);

            count = 0;
       //     Console.WriteLine("NAMETAG DATA:");
            for (int i = 0; i < Constants.nametagSize; i++)
            {
                nametag[i] = fileBuffer[start_address];
              //  Console.Write(nametag[i].ToString("X2"));
                start_address++;
                count++;
            }
    
            count = 0;

            byte[] tiletest = new byte[64];
            int countur=0;

            for (int s = 0; s < nametag_total_tiles; s++)
            {
                tiletest = getTile(countur, nametag);
                tiles[s] = tile2bitmap(tiletest, nametag_pallete,nametag_corrected_width,nametag_corrected_height);
                countur = countur + 32;
            }

            countur = 0;

            Rectangle rect = new Rectangle(0, 0, nametag_corrected_width ,nametag_corrected_height);
            Bitmap temps = nametagBitmap;
            Bitmap bmap = (Bitmap)temps.Clone();
            Graphics gr = Graphics.FromImage(bmap);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    rect.X = j * nametag_corrected_width;
                    rect.Y = i * nametag_corrected_height;
                    //Console.WriteLine(j.ToString() + "," + i.ToString() + "[" + tilecounter.ToString() + "]");
                    tiles[countur].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    
                    gr.DrawImage(tiles[countur], rect);
                
                    countur++;
                }
            }

            nametagBitmap = (Bitmap)bmap.Clone();
        
            pictureBox3.Image = nametagBitmap;
      
               
        }

        private byte[] getTile(int address, byte[] buffer)
        {
            byte[] tile= new byte[64];
            byte[] temp = new byte[4];
            int offset = address;
            int counter = 0;
            
            //flatten the layers here
            for(int i=0;i<8;i++)
            {
                for (int a = 0; a < 8; a++)
                {
                    //these shifts allow us to just get the bit we are interested in
                    temp[0] = (byte)((buffer[offset] >> a) & 1);
                    temp[1] = (byte)((buffer[offset + 1] >> a) & 1);
                    temp[2] = (byte)((buffer[offset + 16] >> a) & 1);
                    temp[3] = (byte)((buffer[offset + 17] >> a) & 1);     
                    //this OR operation 'sandwiches' the layers into one color palette value
                    tile[counter] = (byte)((temp[0]) | (temp[1] << 1 | (temp[2] << 2) | (temp[3] << 3)));
                    counter++;
                }
                
                offset = offset + 2;
            }

            return tile;
        }

        private byte[] get4bpp(int address,int startx,int starty)
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
                    pixels[i] = (byte)backArray2[startx+i, starty+y];

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


        private Bitmap tile2bitmap(byte[] tile_data, Color[] palette, int tile_size_X, int tile_size_Y)
        {
            //tile_size_X,tile_size_Y are tile sizes generated from size of the pictureBox

            int counter =0 ;
            int pixelX = tile_size_X / 8;
            int pixelY = tile_size_Y / 8;

            Bitmap tilebitmap = new Bitmap(tile_size_X, tile_size_Y); 
           
            //adjust pixel size based on bitmap size. 
            for(int y=0;y<8;y++) //8 here is the original tile height
            {
                for(int x =0;x<8;x++) //8 here is the original tile width
                {
                    for (int q = 0; q < pixelX; q++)
                    {
                        tilebitmap.SetPixel((x * (pixelX)) + q, (y * pixelY), palette[tile_data[counter]]);
                        for(int r =0;r<pixelY;r++)
                        {
                            tilebitmap.SetPixel((x * (pixelX))+q, (y * pixelY) + r, palette[tile_data[counter]]);
                        }
                    }
                  counter++;
                }
            }
            return tilebitmap;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox2.Text);
           temp = temp + 384;
           // temp = temp + 1;
            textBox2.Text = temp.ToString();
            button4_Click(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
         int temp = Convert.ToInt32(textBox2.Text);
        temp = temp - 384;
        // temp = temp - 1;
            textBox2.Text = temp.ToString();
            button4_Click(sender, e);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
           // string balls = "C:\\Users\\dext3r\\Desktop\\NBA Jam - Tournament Edition (U) [!].smc";
           // byte[] buffer;
            byte[] stats = new byte[26];
            int count = 0;
            int start_address;

            int playerValue;
            int bodySize;
            int speedRating;
            int threeptRating;
            int dunkRating;
            int blockRating;
            int stealRating;
            int AILevel;
            int passRating;
            int powerRating;
            int clutchRating;
            int skinPointer;
            int headPointer;

            int[] speed_lookup = new int[11];
            int[] threes_lookup = new int[11];

            speed_lookup[0] = 0x13F8;
            speed_lookup[1] = 0x1455;
            speed_lookup[2] = 0x14A7;
            speed_lookup[3] = 0x14FA;
            speed_lookup[4] = 0x154C;
            speed_lookup[5] = 0x159F;
            speed_lookup[6] = 0x15F1;
            speed_lookup[7] = 0x1644;
            speed_lookup[8] = 0x1711;
            speed_lookup[9] = 0x1777;
            speed_lookup[10] =0x17DE;

            threes_lookup[0] = 0x02BC;
            threes_lookup[1] = 0x02EE;
            threes_lookup[2] = 0x0320;
            threes_lookup[3] = 0x035C;
            threes_lookup[4] = 0x03A2;
            threes_lookup[5] = 0x03CA;
            threes_lookup[6] = 0x0410;
            threes_lookup[7] = 0x044C;
            threes_lookup[8] = 0x0460;
            threes_lookup[9] = 0x0492;
            threes_lookup[10] =0x04E2;

          /*  FileStream fileStream = new FileStream(balls, FileMode.Open, FileAccess.Read);

            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            */
            start_address = Convert.ToInt32(textBox3.Text);

            count = 0;
         //   Console.WriteLine("STAT DATA:");
            for (int i = 0; i < Constants.playerStats_size; i++)
            {
                stats[i] = fileBuffer[start_address];
           //     Console.Write(stats[i].ToString("X2")+ " ");
                start_address++;
                count++;
            }
       //     Console.WriteLine();
            playerValue = (stats[1]  << 8) | stats[0];
            bodySize = (stats[3] << 8) | stats[2];
            speedRating = (stats[5] << 8) | stats[4];
            threeptRating = (stats[7] << 8) | stats [6];
            dunkRating = (stats[9] << 8) | stats[8];
            blockRating = (stats[11] << 8) | stats[10];
            stealRating = (stats[13] << 8) | stats[12];
            AILevel = (stats[15] << 8) | stats[14];
            passRating = (stats[17] << 8) | stats[16];
            powerRating = (stats[19] << 8) | stats[18];
            clutchRating = (stats[21] << 8) | stats[20];
            skinPointer = (stats[23] << 8) | stats[22];
            headPointer = (stats[25] << 8) | stats[24];

  
           /* Console.WriteLine("Player Value: " + playerValue.ToString("X4"));
            Console.WriteLine("Body Size: " + bodySize.ToString("X4"));
            Console.WriteLine("Speed Rating: " + speedRating.ToString("X4"));
            Console.WriteLine("3PT Rating: " + threeptRating.ToString("X4"));
            Console.WriteLine("Dunk Rating: " + dunkRating.ToString("X4")); 
            Console.WriteLine("Block Rating: " + blockRating.ToString("X4"));
            Console.WriteLine("Steal Rating: " + stealRating.ToString("X4"));
            Console.WriteLine("AI Level: " + AILevel.ToString("X4"));
            Console.WriteLine("Pass Rating: " + passRating.ToString("X4"));
            Console.WriteLine("Power Rating: " + powerRating.ToString("X4"));
            Console.WriteLine("Clutch Rating: " + clutchRating.ToString("X4"));
            Console.WriteLine("Skin Pointer: " + skinPointer.ToString("X4"));
            Console.WriteLine("Head Pointer: " + headPointer.ToString("X4"));
            */

            //speed and 3 pt funky lookup
            for (int i = 0; i < 11; i++)
            {
                if(speed_lookup[i] == speedRating)
                    speedRating = i;
            }


            for (int j = 0; j < 11; j++)
            {
                if(threes_lookup[j] == threeptRating)
                    threeptRating = j;
            }
            //3s
            label10.Text = threeptRating.ToString();
            //speed
            label9.Text = speedRating.ToString();
            //dunk
            label11.Text = dunkRating.ToString();
            //pass
            label12.Text = passRating.ToString();
            //power
            label13.Text = powerRating.ToString();
            //steal
            label14.Text = stealRating.ToString();
            //block
            label15.Text = blockRating.ToString();
            //clutch
            label16.Text = clutchRating.ToString();

        } //end of function

        private void button8_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox3.Text);
            temp = temp + 26;
            textBox3.Text = temp.ToString();
            button9_Click(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox3.Text);
            temp = temp - 26;
            textBox3.Text = temp.ToString();
            button9_Click(sender, e);
        }

        private byte[] getTileFromArray(int[,] backgroundArray,int startx,int starty)
        {
            byte[] tile = new byte[64];
            int counter =0;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    tile[counter] = (byte)backgroundArray[startx+j,starty+i];
                    counter++;
                }
            }

            return tile;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            //int[,] backArray = new int[48, 16];
            
            byte[][] tiletest = new byte[12][];
            string balls;
            int locationx=0;
            int locationy=0;
            balls = textBox4.Text;
            int z;
            int f=0;
            int[] textname;
            int text_size = 0;

            int nametag_corrected_width = (pictureBox4.Width / Constants.num_nametag_tilesX);
            int nametag_corrected_height = (pictureBox4.Height / Constants.num_nametag_tilesY);
            int nametag_total_tiles = (Constants.num_nametag_tilesX * Constants.num_nametag_tilesY);

            Bitmap nametagBitmap = new Bitmap(nametag_corrected_width * Constants.num_nametag_tilesX, nametag_corrected_width * Constants.num_nametag_tilesY);
            Bitmap[] tiles = new Bitmap[nametag_total_tiles]; //create array of tiles based on known number of required tiles

            textname = new int[textBox4.Text.Length];

      //      Console.WriteLine("Text:");

            //counts the characters and creats aarrays 
            //dont care yet
            foreach (char c in balls)
            {
                z = (int)(c - 32); //65 = A
                //this offsets the array for searching
         //       Console.WriteLine(Convert.ToInt16(z).ToString());
                textname[f] = z;
                f++;
            }


            for (int i = 0; i < nametag_total_tiles ; i++)
            {
                //each tile has to be scaled based on the parent picturebox size...
                tiles[i] = new Bitmap(nametag_corrected_width, nametag_corrected_height);
                tiletest[i] = new byte[64];
            }

            //copy letter to background array
            //get total size of img
            foreach (int fu in textname)
            {
                text_size = text_size + letters[fu].Width;
            }
            text_size = (48 - (text_size)) / 2;
          
            locationx = text_size;
            locationy = 7 ;

        //  locationx = 0;
        //  locationy = 0;

            foreach (int fu in textname)
            {
                for(int y = 0; y<letters[fu].Height;y++)
                {
                    for(int x =0;x<letters[fu].Width;x++)
                    {
                        if((locationx+x  < 48))
                         backArray[locationx+x,locationy+y] = letters[fu].getPixel(x,y);
                    }
                }
                locationx = locationx + letters[fu].Width;
            }

           

            int rodcounter = 0;

            for (int q = 0; q < 16; q = q + 8)
            {
                for (int v = 0; v < 48; v = v + 8)
                {
                    tiletest[rodcounter] = getTileFromArray(backArray, v, q);
                    rodcounter++;
                }
            }

            if (checkBox2.Checked)
            {

                nametag_pallete[0] = System.Drawing.Color.FromArgb(255, 128, 0, 128);  //color         
            }
            else
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(0, 128, 0, 128);  //transparent    
            }
            for (int s = 0; s < nametag_total_tiles; s++)
            {
               
                   // tiletest[s] = getTileFromArray(backArray, 0, 0); 
                   tiles[s] = tile2bitmap(tiletest[s], nametag_pallete, nametag_corrected_width, nametag_corrected_height);
     
            }
            //48*16 = 6 tiles * 2 tiles
           //grab a tile from the back array
            int countur = 0;

            Rectangle rect = new Rectangle(0, 0, nametag_corrected_width, nametag_corrected_height);
            Bitmap temps = nametagBitmap;
            Bitmap bmap = (Bitmap)temps.Clone();
            Graphics gr = Graphics.FromImage(bmap);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    rect.X = j * nametag_corrected_width;
                    rect.Y = i * nametag_corrected_height;
                    //Console.WriteLine(j.ToString() + "," + i.ToString() + "[" + tilecounter.ToString() + "]");
                   // tiles[countur].RotateFlip(RotateFlipType.RotateNoneFlipX);

                    gr.DrawImage(tiles[countur], rect);

                    countur++;
                }
            }

            nametagBitmap = (Bitmap)bmap.Clone();

            pictureBox4.Image = nametagBitmap;
            nbajamTextBox1.Image = nametagBitmap;

            for (int q = 0; q < 16; q++)
            {
                for (int v = 0; v < 48; v++)
                {
                    backArray2[v, q] = backArray[v, q]; //just copies this to a global variable to use in the save prodcedure. stupid as hell. 
                    backArray[v,q] = 0; //clears background array. 
                    
                }
            }
            
            button11.Enabled = true;

        
        }

        private void button11_Click(object sender, EventArgs e)
        {
            //save dat shit
            byte[] nametag_4bpp = new byte[Constants.nametagSize]; 
            byte[] tile = new byte[32];
            int counter=0;
            int offset=0;
         //   string balls = "C:\\Users\\dext3r\\Desktop\\dicks.sfc";
         //   string balls2= "C:\\Users\\dext3r\\Desktop\\pokey.sfc";
            int locationx=0;
            int locationy=0;
            int count = 0;
            byte[] buffer;
            int nameOffset = Convert.ToInt32(textBox2.Text);
      //      FileStream fileStream = new FileStream(balls, FileMode.Open, FileAccess.Read);
      //      FileStream fileStream2 = new FileStream(balls2, FileMode.OpenOrCreate, FileAccess.Write);
        /*    try
            {

            
                
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                // actual number of bytes read
                   int sum = 0;                          // total number of bytes read
                //
                // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                      sum += count;  // sum is a buffer offset for next reading
            }
            finally 
            {
                fileStream.Close();
            }
          */


            #region Debug: Prints Array --- WHICH ARRAY??? MORE DESCRIPTIVE
            for (int q = 0; q < 16; q++)
            {
                for (int v = 0; v < 48; v++)
                {
                    Console.Write(backArray2[v, q].ToString("X"));

                }
                    Console.WriteLine();
            }
            #endregion


            for (int z = 0; z< 2; z++)
            {
                for (int j = 0; j < 6; j++)
                {
                    tile = get4bpp(0, locationx, locationy);

                    for (int i = 0; i < 8; i++)
                    {
                        nametag_4bpp[offset] = tile[(counter)];
                        nametag_4bpp[offset + 1] = tile[(counter + 1)];
                        nametag_4bpp[offset + 16] = tile[(counter + 2)];
                        nametag_4bpp[offset + 17] = tile[(counter + 3)];

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


         /*   tile = get4bpp(0, 0,8);
            counter = 0;
            offset = offset + 16;
            for (int i = 0; i < 8; i++)
            {
                nametag_4bpp[offset] = tile[(counter)];
                nametag_4bpp[offset + 1] = tile[(counter + 1)];
                nametag_4bpp[offset + 16] = tile[(counter + 2)];
                nametag_4bpp[offset + 17] = tile[(counter + 3)];

                offset = offset + 2;
                counter = counter + 4;
            }
             */
            //0,8,16,24,32,40



            //Copy the nametag buffer to the file buffer
            for (int i = 0; i < Constants.nametagSize; i++)
            {
                fileBuffer[nameOffset] = nametag_4bpp[i];
               nameOffset++;
            }

//            fileStream2.Seek(0, 0);
//            fileStream2.Write(fileBuffer, 0, fileBuffer.Length);
//            fileStream2.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            button10_Click(sender, e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "SNES ROM files (*.sfc;*.smc)|*.sfc;*.smc";
            int count;

            DialogResult result = dlg.ShowDialog();
           
            if (result == DialogResult.OK)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open, FileAccess.Read);
                try
                {
                    int length = (int)fileStream.Length;  // get file length
                    fileBuffer = new byte[length];            // create buffer
                    // actual number of bytes read
                    int sum = 0;                          // total number of bytes read

                    // read until Read method returns 0 (end of the stream has been reached)
                    while ((count = fileStream.Read(fileBuffer, sum, length - sum)) > 0)
                        sum += count;  // sum is a buffer offset for next reading
                }
                finally
                {  
                    toolStripStatusLabel1.Text = dlg.FileName;
                    fileStream.Close();
                }
            }


        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //create SaveFileDialog object...
            SaveFileDialog fileSaveDialog = new SaveFileDialog();
            fileSaveDialog.Filter = "SNES ROM files (*.sfc;*.smc)|*.sfc;*.smc";
            fileSaveDialog.Title = "Save to a new ROM file...";
            fileSaveDialog.ShowDialog();

            //create a filestream object to write to
            FileStream outputFile = new FileStream(fileSaveDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write);

            outputFile.Seek(0, 0);
            outputFile.Write(fileBuffer, 0, fileBuffer.Length);
            toolStripStatusLabel1.Text = "Wrote file: " + fileSaveDialog.FileName.ToString();
            //TODO: should add some kind of error handling here ...
            outputFile.Close();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {

        }

        private void nbajamTextBox1_Click(object sender, EventArgs e)
        {

        }


        
    }

    static class Constants
    {
        public const int portraitBitmapSize = 1680;
        public const int portraitPaletteSize = 64;

        public const int num_nametag_tilesX = 6;
        public const int num_nametag_tilesY = 2;
        public const int nametagSize = 384; // could probably shorten this buffer in the future

        public const int playerStats_size = 26; // 0x1A bytes
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


    public class playerData
    {
        private int ID;
        private String internalName;
        private int portraitLocation;
        private int nametagLocation;
        private int statsLocation;
        private bool secretPlayer;

        public playerData(int addID, String addName, int addPortrait, int addNametag, int addStats,bool addSecret)
        {
            ID = addID;
            internalName = addName;
            portraitLocation = addPortrait;
            nametagLocation = addNametag;
            statsLocation = addStats;
            secretPlayer = addSecret;
        }

        public String getName()
        {
            return internalName;
        }

        public int getPortrait()
        {
            return portraitLocation;
        }

        public int getNametag()
        {
            return nametagLocation;
        }

        public int getStats()
        {
            return statsLocation;
        }

        public bool isSecretPlayer()
        {
            return secretPlayer;
        }


    }
}
