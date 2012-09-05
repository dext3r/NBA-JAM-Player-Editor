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
//NOT MY CODE \/ \/ \/ \/ \/ \/ 
using SimplePaletteQuantizer.Quantizers;
using SimplePaletteQuantizer.Quantizers.HSB;
using SimplePaletteQuantizer.Quantizers.Median;
using SimplePaletteQuantizer.Quantizers.Octree;
using SimplePaletteQuantizer.Quantizers.Popularity;
using SimplePaletteQuantizer.Quantizers.Uniform;
//NOT MY CODE /\ /\ /\ /\ /\ /\
//Awesome project from here: http://www.codeproject.com/KB/recipes/SimplePaletteQuantizer.aspx

// 9/4/2012:    Started commenting chunks of code functionality and cleaning shit up
//              TODO: Finish player editor - Edit stats, portrait - leave roster editor for next version
//              improve palette quantization and transparent color scheme in nbajamPictureBox

// 8/1/2011:    Start keeping notes because you don't remember shit
//              Palette Quantizer seems to be OK, needs work. Loading procedure: Load ROM. Click Load Image, it writes it to the ROM.
//              Will attempt the roster editor. Proposed idea is to use listview with custom listviewitem. under investigation.
//              Added custom User Control - playerPairing - needs functionality code implemented
//              Something might be fucked up with the project - might need to make again. - seems ok? dunno what this note is about - 9/4/2012
//              Need to implement internal sqlite database


namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        System.Drawing.Color[] nametag_pallete = new System.Drawing.Color[16]; // used as general palette - could move to Constants class

        //Defines 60 unique characters for a font
        fontTile[] letters = new fontTile[60];
        
        //no clue what this is for
        int[,] backArray = new int[48, 16];  //array for nametag data
        int[,] backArray2 = new int[48, 16];  //array for nametag data
        List<playerData> nbajam_players = new List<playerData>();
        int timerthing = 0;
        int animator = 0;

        //no clue wha this is
        public enum FontColorOptions { Pallete_0, Pallete_1, Pallete_2, Pallete_3, Pallete_4, Pallete_5, Pallete_6, Pallete_7, Pallete_8, Pallete_9, Pallete_10, Pallete_11, Pallete_12, Pallete_13, Pallete_14, Pallete_15 };

        byte[] fileBuffer;

        /// <summary>
        /// Palette Quantizing stuff
        /// </summary>
        /// this stuff has been deprecated into nbajampicturebox i think
        private Image sourceImage;
        private FileInfo sourceFileInfo;
        private IColorQuantizer the_quantizer = new PaletteQuantizer();
        private Color[] optimized_palette = new Color[32];
        private byte[,] new_back_array = new byte[48, 56];

        public Form1()
        {
            InitializeComponent();
        }

        private void loadPortrait(int location)
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
            Bitmap paletteBitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Bitmap portraitBitmap = new Bitmap(48, 56);
            Bitmap[,] tile = new Bitmap[6, 7];
            Bitmap[] tiles = new Bitmap[42];
            byte[] full_data = new byte[Constants.portraitBitmapSize + Constants.portraitPaletteSize];
            int balls = 0;

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
            start_address = location;
            count = 0;
            balls = 0;
            //    Console.WriteLine("PORTRAIT DATA:");
            for (int i = 0; i < Constants.portraitBitmapSize; i++)
            {
                portrait[i] = fileBuffer[start_address];
                full_data[balls] = fileBuffer[start_address];
                //     Console.Write(portrait[i].ToString("X2"));
                start_address++;
                count++;
                balls++;
            }
            //    Console.WriteLine("Count: " + count.ToString());
            //    Console.WriteLine();
            count = 0;
            //    Console.WriteLine("PALETTE DATA:");
            for (int j = 0; j < Constants.portraitPaletteSize; j++)
            {
                palette[j] = fileBuffer[start_address];
                full_data[balls] = fileBuffer[start_address];
                //       Console.Write(palette[j].ToString("X2"));
                start_address++;
                count++;
                balls++;
            }
            //   Console.WriteLine("Count: " + count.ToString());
            //  Console.WriteLine();


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

            //This "Palette Hack" draws the pallet at the specified memory address to an image box
            //This code sucks and should be eventually removed. 
            #region Palette Hack
            //palette drawing hack
            int offset_x = 0, offset_y = 0;
            for (int f = 0; f < 16; f++)
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

            #endregion
            #region Old Code - To Delete
            /*
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

            */
            #endregion
            nbajamPictureBox1.DataSize = 1744;
            nbajamPictureBox1.PaletteSize = 32;
            nbajamPictureBox1.isPortrait = true;
            nbajamPictureBox1.loadImageData(full_data);

            nbajamPictureBox2.DataSize = 1744;
            nbajamPictureBox2.PaletteSize = 32;
            nbajamPictureBox2.isPortrait = true;
            nbajamPictureBox2.loadImageData(full_data);

            nbajamPictureBox3.DataSize = 1744;
            nbajamPictureBox3.PaletteSize = 32;
            nbajamPictureBox3.isPortrait = true;
            nbajamPictureBox3.loadImageData(full_data);

            //playerPairing1.setName1("LENNON");
            // playerPairing1.setName2("MCCARTNEY");
            //playerPairing1.setPortrait1(full_data);
            //playerPairing1.setPortrait2(full_data);

            //layerPairing2.isExpandedRoster = true;
            // playerPairing2.setName1("LENNON");
            //playerPairing2.setName2("MCCARTNEY");
            // playerPairing2.setPortrait1(full_data);
            //playerPairing2.setPortrait2(full_data);

            //nbajamPictureBox1.Invalidate();
            //    nbajamPictureBox1.Image = portraitBitmap; 
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
            Bitmap paletteBitmap = new Bitmap(pictureBox2.Width, pictureBox2.Height);
            Bitmap portraitBitmap = new Bitmap(48, 56);
            Bitmap[,] tile = new Bitmap[6, 7];
            Bitmap[] tiles = new Bitmap[42];
            byte[] full_data = new byte[Constants.portraitBitmapSize + Constants.portraitPaletteSize];
            int balls = 0;

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
            balls = 0;
            //    Console.WriteLine("PORTRAIT DATA:");
            for (int i = 0; i < Constants.portraitBitmapSize; i++)
            {
                portrait[i] = fileBuffer[start_address];
                full_data[balls] = fileBuffer[start_address];
                //     Console.Write(portrait[i].ToString("X2"));
                start_address++;
                count++;
                balls++;
            }
            //    Console.WriteLine("Count: " + count.ToString());
            //    Console.WriteLine();
            count = 0;
            //    Console.WriteLine("PALETTE DATA:");
            for (int j = 0; j < Constants.portraitPaletteSize; j++)
            {
                palette[j] = fileBuffer[start_address];
                full_data[balls] = fileBuffer[start_address];
                //       Console.Write(palette[j].ToString("X2"));
                start_address++;
                count++;
                balls++;
            }
            //   Console.WriteLine("Count: " + count.ToString());
            //  Console.WriteLine();


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

            //This "Palette Hack" draws the pallet at the specified memory address to an image box
            //This code sucks and should be eventually removed. 
            #region Palette Hack
            //palette drawing hack
            int offset_x = 0, offset_y = 0;
            for (int f = 0; f < 16; f++)
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

            #endregion
            #region Old Code - To Delete
            /*
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

            */
            #endregion
            nbajamPictureBox1.DataSize = 1744;
            nbajamPictureBox1.PaletteSize = 32;
            nbajamPictureBox1.isPortrait = true;
            nbajamPictureBox1.loadImageData(full_data);

            nbajamPictureBox2.DataSize = 1744;
            nbajamPictureBox2.PaletteSize = 32;
            nbajamPictureBox2.isPortrait = true;
            nbajamPictureBox2.loadImageData(full_data);

            nbajamPictureBox3.DataSize = 1744;
            nbajamPictureBox3.PaletteSize = 32;
            nbajamPictureBox3.isPortrait = true;
            nbajamPictureBox3.loadImageData(full_data);

            //playerPairing1.setName1("LENNON");
           // playerPairing1.setName2("MCCARTNEY");
            //playerPairing1.setPortrait1(full_data);
            //playerPairing1.setPortrait2(full_data);

            //layerPairing2.isExpandedRoster = true;
           // playerPairing2.setName1("LENNON");
            //playerPairing2.setName2("MCCARTNEY");
           // playerPairing2.setPortrait1(full_data);
            //playerPairing2.setPortrait2(full_data);

            //nbajamPictureBox1.Invalidate();
            //    nbajamPictureBox1.Image = portraitBitmap;         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Tell user no file selected.
            toolStripStatusLabel1.Text = "No file open.";

            //Some junk to help debugging
            //textBox1.Text = "2232681";
            textBox1.Text = "2234425"; // ??
            //   textBox2.Text = " 1363877"; //pippen
            textBox2.Text = "1398188";


            //initialzePalette defines a program-wide pallete - see function for details
            initializePalette();
            //initializeFont defines the program-wide font (character widths and pixelmap info)
         //   initializeFont();
  
           initializePlayers();

           loadListboxWithPlayers();
           
         
            //panel1.Visible = false;
            //panel2.Visible = true;
            //panel1.SendToBack();
           // panel2.BringToFront();
        }

        private void loadListboxWithPlayers()
        {
            foreach (playerData player in nbajam_players)
            {
                comboBox1.Items.Add(player.getName());
            }
        }

        private void initializePlayers()
        {
            //ID,name,picture,nametag,stats,secret?
            //nbajam_players.Add(new playerData(0, "BJ Armstrong", 2241401, 1398188, 1398188, false));
            nbajam_players.Add(new playerData(0, "A C Green", 429472, 621328, 2081371, false));
            nbajam_players.Add(new playerData(1, "Aaron McKie", 2473296, 749130, 2081527, false));
            nbajam_players.Add(new playerData(2, "Adam Horovitz (Adrock)", 2846800, 1405484, 2081553, false));
            nbajam_players.Add(new playerData(3, "Adam Yauch (MCA)", 2848544, 1405868, 2081579, false));
            nbajam_players.Add(new playerData(4, "Alex de Lucia (Facime)", 2468064, 747978, 2081605, false));
            nbajam_players.Add(new playerData(5, "Alonzo Mourning", 0, 748362, 2079187, false));
            nbajam_players.Add(new playerData(6, "Anfernee Hardaway", 2269712, 1404332, 2081085, false));
            nbajam_players.Add(new playerData(7, "Anthony Mason", 2266224, 1403564, 2080929, false));
            nbajam_players.Add(new playerData(8, "Anthony Peeler", 537433, 1403948, 2080305, false));
            nbajam_players.Add(new playerData(9, "Antonio Davis", 2280176, 736074, 2080175, false));
            nbajam_players.Add(new playerData(10, "Armon Gilliam", 2482016, 619024, 2080877, false));
            nbajam_players.Add(new playerData(11, "Asif Chaudhri (Chow-Chow)", 2461088, 746442, 2080903, false));
            nbajam_players.Add(new playerData(12, "B.J. Armstrong", 2241401, 746826, 2079291, false));
            nbajam_players.Add(new playerData(13, "Benny", 2447504, 743754, 2082359, false));
            nbajam_players.Add(new playerData(14, "Bill Clinton", 2454480, 745290, 2082463, false));
            nbajam_players.Add(new playerData(15, "Bill Curley", 455632, 392295, 2079837, false));
            nbajam_players.Add(new playerData(16, "Billy Owens", 2287152, 737610, 2082125, false));
            nbajam_players.Add(new playerData(17, "Bobby Hurley", 547897, 737994, 2081553, false));
            nbajam_players.Add(new playerData(18, "Brad Daugherty", 0, 738378, 2079395, false));
            nbajam_players.Add(new playerData(19, "Brett Gow (Brutah)", 2462832, 746826, 2079421, false));
            nbajam_players.Add(new playerData(20, "Brooks Thompson", 2679451, 2489024, 2082203, false));
            nbajam_players.Add(new playerData(21, "Calbert Cheaney", 0, 2489408, 2082021, false));
            nbajam_players.Add(new playerData(22, "Carol Blazekowski (Blaze)", 2445760, 743370, 2082047, false));
            nbajam_players.Add(new playerData(23, "Charles Oakley", 2267968, 743754, 2080981, false));
            nbajam_players.Add(new playerData(24, "Chris Kirby", 2435296, 741066, 2082281, false));
            nbajam_players.Add(new playerData(25, "Chris Mullin", 3081936, 741450, 2079889, false));
            nbajam_players.Add(new playerData(26, "Chris Webber", 533945, 1373093, 2079915, false));
            nbajam_players.Add(new playerData(27, "Christian Laettner", 0, 1360037, 2080669, false));
            nbajam_players.Add(new playerData(28, "Chuck Person", 0, 1360421, 2081761, false));
            nbajam_players.Add(new playerData(29, "Clarence Weatherspoon", 526969, 1371557, 2081163, false));
            nbajam_players.Add(new playerData(30, "Clifford Robinson", 540921, 1392044, 2081475, false));
            nbajam_players.Add(new playerData(31, "Clifford Rozier ", 439936, 651572, 0, false));
            nbajam_players.Add(new playerData(32, "Clyde Drexler", 3094144, 1355429, 2081423, false));
            nbajam_players.Add(new playerData(33, "Crunch", 2450992, 744522, 2082411, false));
            nbajam_players.Add(new playerData(34, "Dan Feinstein (Weasel)", 2464576, 747210, 2082437, false));
            nbajam_players.Add(new playerData(35, "Dan Majerle", 3090656, 1354661, 2081293, false));
            nbajam_players.Add(new playerData(36, "Dana Barros", 425984, 620560, 2081241, false));
            nbajam_players.Add(new playerData(37, "Danny Manning", 3083680, 1352357, 2081397, false));
            nbajam_players.Add(new playerData(38, "David Benoit", 2236169, 1352741, 2081943, false));
            nbajam_players.Add(new playerData(39, "David Robinson", 0, 1360805, 2081657, false));
            nbajam_players.Add(new playerData(40, "Dee Brown", 0, 1367717, 2079031, false));
            nbajam_players.Add(new playerData(41, "Dell Curry", 2248377, 1399724, 2079213, false));
            nbajam_players.Add(new playerData(42, "Dennis Rodman", 2232681, 1396268, 2081683, false));
            nbajam_players.Add(new playerData(43, "Dennis Scott", 2485504, 619792, 2081137, false));
            nbajam_players.Add(new playerData(44, "Derek Harper", 3104608, 1357733, 2081007, false));
            nbajam_players.Add(new playerData(45, "Derrick Coleman", 0, 1369253, 2080773, false));
            nbajam_players.Add(new playerData(46, "Derrick McKey", 2278432, 735690, 2080149, false));
            nbajam_players.Add(new playerData(47, "Detlef Schrempf", 3099376, 1356581, 2081865, false));
            nbajam_players.Add(new playerData(48, "Dikembe Mutombo", 3108096, 1358501, 2079603, false));
            nbajam_players.Add(new playerData(49, "Dino Radja", 553129, 1358885, 2079083, false));
            nbajam_players.Add(new playerData(50, "Dominique Wilkins", 0, 1362341, 2079005, false));
            nbajam_players.Add(new playerData(51, "Donyell Marshall", 445168, 652724, 2080747, false));
            nbajam_players.Add(new playerData(52, "Doug West", 2480272, 618640, 2080721, false));
            nbajam_players.Add(new playerData(53, "Dwight Myers (Heavy D)", 450400, 653876, 2080747, false));
            nbajam_players.Add(new playerData(54, "Eddie Jones", 2672475, 392679, 2080409, false));
            nbajam_players.Add(new playerData(55, "Eric Kuby (Kabuki)", 2471552, 748746, 2080435, false));
            nbajam_players.Add(new playerData(56, "Eric Mobley", 2674219, 2456224, 2080643, false));
            nbajam_players.Add(new playerData(57, "Eric Montross", 432960, 622096, 2079109, false));
            nbajam_players.Add(new playerData(58, "Eric Murdock", 2478528, 618256, 2080617, false));
            nbajam_players.Add(new playerData(59, "Eric Samulski (Air Dog)", 2466320, 747594, 2080643, false));
            nbajam_players.Add(new playerData(60, "Frank Thomas", 2841568, 1375520, 2083087, false));
            nbajam_players.Add(new playerData(61, "Gary Payton", 542665, 1375904, 2081813, false));
            nbajam_players.Add(new playerData(62, "Gerald Wilkins", 539177, 1391660, 2079447, false));
            nbajam_players.Add(new playerData(63, "Glen Rice", 2260992, 1402412, 2080435, false));
            nbajam_players.Add(new playerData(64, "Glenn Robinson", 443424, 652340, 2082229, false));
            nbajam_players.Add(new playerData(65, "Grant Hill", 438192, 651188, 2079811, false));
            nbajam_players.Add(new playerData(66, "Greg Fischbach (Stud Muffin)", 452144, 654260, 2079837, false));
            nbajam_players.Add(new playerData(67, "Hakeem Olajuwon", 0, 1359269, 2079967, false));
            nbajam_players.Add(new playerData(68, "Harold Miner", 0, 1368869, 2080487, false));
            nbajam_players.Add(new playerData(69, "Hersey Hawkins", 2239657, 1397804, 2079161, false));
            nbajam_players.Add(new playerData(70, "Hillary Clinton", 448656, 653492, 2083217, false));
            nbajam_players.Add(new playerData(71, "Horace Grant", 0, 1364261, 2081059, false));
            nbajam_players.Add(new playerData(72, "Hugo", 2449248, 744138, 2082385, false));
            nbajam_players.Add(new playerData(73, "Isaiah Rider", 2230937, 744522, 2080695, false));
            nbajam_players.Add(new playerData(74, "Jalen Rose", 436448, 650804, 2079707, false));
            nbajam_players.Add(new playerData(75, "Jamal Mashburn", 551385, 1394348, 2079499, false));
            nbajam_players.Add(new playerData(76, "James Worthy", 3087168, 1353893, 2080357, false));
            nbajam_players.Add(new playerData(77, "Jamie Rivett", 2430064, 739914, 2082827, false));
            nbajam_players.Add(new playerData(78, "Jason Falcus", 2438784, 741834, 2082515, false));
            nbajam_players.Add(new playerData(79, "Jason Kidd", 434704, 650452, 2079551, false));
            nbajam_players.Add(new playerData(80, "Jay Moon", 2469808, 748362, 2082775, false));
            nbajam_players.Add(new playerData(81, "Jeff Hornacek", 528713, 1371941, 2081969, false));
            nbajam_players.Add(new playerData(82, "Jeff Malone", 2487248, 620176, 2081215, false));
            nbajam_players.Add(new playerData(83, "Jeff Townes (Jazzy Jeff)", 2682939, 2489760, 2081241, false));
            nbajam_players.Add(new playerData(84, "Jim Jackson", 3106352, 1358117, 2079473, false));
            nbajam_players.Add(new playerData(85, "Joe Dumars", 2246633, 1358501, 2079733, false));
            nbajam_players.Add(new playerData(86, "John Carlton", 2288896, 737994, 2082853, false));
            nbajam_players.Add(new playerData(87, "John Starks", 0, 1370405, 2080955, false));
            nbajam_players.Add(new playerData(88, "John Stockton", 0, 1370789, 2081917, false));
            nbajam_players.Add(new playerData(89, "John Williams", 546153, 1393196, 2079421, false));
            nbajam_players.Add(new playerData(90, "Juwan Howard ", 2475040, 749514, 0, false));
            nbajam_players.Add(new playerData(91, "Karl Malone", 0, 1361573, 2081891, false));
            nbajam_players.Add(new playerData(92, "Kendall Gill", 544409, 1392812, 2081839, false));
            nbajam_players.Add(new playerData(93, "Kenny Anderson", 0, 1393196, 2080799, false));
            nbajam_players.Add(new playerData(94, "Kevin Edwards", 0, 1367333, 2080851, false));
            nbajam_players.Add(new playerData(95, "Kevin Johnson", 3092400, 1355045, 2081319, false));
            nbajam_players.Add(new playerData(96, "Kevin Willis", 2237913, 1397420, 2078979, false));
            nbajam_players.Add(new playerData(97, "Khalid Reeves", 441680, 651956, 2080539, false));
            nbajam_players.Add(new playerData(98, "LaPhonso Ellis", 3109840, 652340, 2080565, false));
            nbajam_players.Add(new playerData(99, "Larry Bird", 2444016, 742986, 2082255, false));
            nbajam_players.Add(new playerData(100, "Larry Johnson", 0, 1363109, 2079135, false));
            nbajam_players.Add(new playerData(101, "Latrell Sprewell", 2273200, 734538, 2079941, false));
            nbajam_players.Add(new playerData(102, "Lionel Simmons", 2244889, 1398956, 2081631, false));
            nbajam_players.Add(new playerData(103, "Loy Vaught", 2234425, 1396652, 2080201, false));
            nbajam_players.Add(new playerData(104, "Luc Longley", 535689, 1390892, 2079343, false));
            nbajam_players.Add(new playerData(105, "Mahmoud Abdul-Rauf", 2264480, 1403180, 2079655, false));
            nbajam_players.Add(new playerData(106, "Malik Sealy", 2250121, 1400108, 2080253, false));
            nbajam_players.Add(new playerData(107, "Mark Price", 0, 1364645, 2079369, false));
            nbajam_players.Add(new playerData(108, "Mark Turmell", 2426576, 739146, 2082983, false));
            nbajam_players.Add(new playerData(109, "Mike Diamond (Mike D)", 2845056, 1405100, 2083009, false));
            nbajam_players.Add(new playerData(110, "Mike Muskett", 2440528, 742218, 2083035, false));
            nbajam_players.Add(new playerData(111, "Mitch Richmond", 3102864, 742602, 2081579, false));
            nbajam_players.Add(new playerData(112, "Monty Williams", 2677707, 2456960, 2082151, false));
            nbajam_players.Add(new playerData(113, "Mookie Blaylock", 2258841, 1402028, 2078927, false));
            nbajam_players.Add(new playerData(114, "Moosekat", 453888, 391911, 2078953, false));
            nbajam_players.Add(new playerData(115, "Muggsy Bogues", 0, 1365413, 2079239, false));
            nbajam_players.Add(new playerData(116, "Neil Hill", 2442272, 742602, 2079265, false));
            nbajam_players.Add(new playerData(117, "Nick Anderson", 0, 1370789, 2081111, false));
            nbajam_players.Add(new playerData(118, "Nick Van Exel", 2283664, 736842, 2080383, false));
            nbajam_players.Add(new playerData(119, "Oliver Miller", 3085424, 1353509, 2079785, false));
            nbajam_players.Add(new playerData(120, "Otis Thorpe", 2274944, 734922, 2080045, false));
            nbajam_players.Add(new playerData(121, "Patrick Ewing", 0, 1370021, 2080903, false));
            nbajam_players.Add(new playerData(122, "Pooh Richardson", 2281920, 736458, 2080279, false));
            nbajam_players.Add(new playerData(123, "Prince Charles", 2457600, 745674, 2082489, false));
            nbajam_players.Add(new playerData(124, "Randall Cunningham", 2843312, 1404716, 2083113, false));
            nbajam_players.Add(new playerData(125, "Reggie Miller", 0, 1366181, 2080097, false));
            nbajam_players.Add(new playerData(126, "Rex Chapman", 2476784, 749898, 2082073, false));
            nbajam_players.Add(new playerData(127, "Rik Smits", 2251865, 750282, 2080123, false));
            nbajam_players.Add(new playerData(128, "Robert Horry", 554873, 1395116, 2080019, false));
            nbajam_players.Add(new playerData(129, "Robert Pack", 532201, 1395500, 2079681, false));
            nbajam_players.Add(new playerData(130, "Rod Strickland", 427728, 620944, 2081501, false));
            nbajam_players.Add(new playerData(131, "Rony Seikaly", 0, 1368485, 2080461, false));
            nbajam_players.Add(new playerData(132, "Sal DiVita", 2290640, 738378, 2080487, false));
            nbajam_players.Add(new playerData(133, "Sam Cassell", 2276688, 735306, 2080071, false));
            nbajam_players.Add(new playerData(134, "Scott Scheno (Kid Silk)", 2433552, 740682, 2080097, false));
            nbajam_players.Add(new playerData(135, "Scott Skiles", 525225, 741066, 2082047, false));
            nbajam_players.Add(new playerData(136, "Scottie Pippen", 0, 1363877, 2079265, false));
            nbajam_players.Add(new playerData(137, "Sean Elliott", 0, 1361189, 2081735, false));
            nbajam_players.Add(new playerData(138, "Sean Rooks", 0, 1366949, 2079525, false));
            nbajam_players.Add(new playerData(139, "Shaquille O'Neal ", 2483760, 619408, 2081033, false));
            nbajam_players.Add(new playerData(140, "Sharone Wright", 446912, 653108, 2081267, false));
            nbajam_players.Add(new playerData(141, "Shawn Bradley", 0, 1365797, 2081189, false));
            nbajam_players.Add(new playerData(142, "Shawn Kemp", 3097632, 1356197, 2081787, false));
            nbajam_players.Add(new playerData(143, "Shawn Liptak", 2424832, 738762, 2081813, false));
            nbajam_players.Add(new playerData(144, "Spud Webb", 549641, 1393964, 2081605, false));
            nbajam_players.Add(new playerData(145, "Stacey Augmon", 0, 1362725, 2078953, false));
            nbajam_players.Add(new playerData(146, "Stanley Roberts", 0, 1363109, 2080227, false));
            nbajam_players.Add(new playerData(147, "Steve \"Snake\" Palmer", 2437040, 741450, 2080253, false));
            nbajam_players.Add(new playerData(148, "Steve Smith", 2229193, 1395500, 2080513, false));
            nbajam_players.Add(new playerData(149, "Steven Tyler", 2459344, 746058, 2080539, false));
            nbajam_players.Add(new playerData(150, "Suns Gorilla", 2452736, 744906, 2082437, false));
            nbajam_players.Add(new playerData(151, "Terry Cummings", 2262736, 745290, 2081709, false));
            nbajam_players.Add(new playerData(152, "Terry Davis", 2285408, 737226, 2082099, false));
            nbajam_players.Add(new playerData(153, "Terry Mills", 2271456, 734154, 2079759, false));
            nbajam_players.Add(new playerData(154, "Terry Porter", 3095888, 734538, 2081449, false));
            nbajam_players.Add(new playerData(155, "Tim Hardaway", 3080192, 1352357, 2079863, false));
            nbajam_players.Add(new playerData(156, "Todd Day", 2253609, 1401260, 2080591, false));
            nbajam_players.Add(new playerData(157, "Tom Gugliotta", 530457, 1372325, 2081995, false));
            nbajam_players.Add(new playerData(158, "Toni Kukoc", 2243145, 1398572, 2079317, false));
            nbajam_players.Add(new playerData(159, "Tony Dumas", 431216, 621712, 2079577, false));
            nbajam_players.Add(new playerData(160, "Tony Goskie", 2428320, 739530, 2082801, false));
            nbajam_players.Add(new playerData(161, "Vernon Maxwell", 0, 1359653, 2079993, false));
            nbajam_players.Add(new playerData(162, "Vin Baker", 2253609, 1400876, 2080565, false));
            nbajam_players.Add(new playerData(163, "Vlade Divac", 3088912, 1401260, 2080331, false));
            nbajam_players.Add(new playerData(164, "Wayman Tisdale", 3101120, 1356965, 2081345, false));
            nbajam_players.Add(new playerData(165, "Wes Little (Scooter Pie)", 2431808, 740298, 2081371, false));
            nbajam_players.Add(new playerData(166, "Wesley Person", 2681195, 2489408, 2082177, false));
            nbajam_players.Add(new playerData(167, "Will Smith (Fresh Prince)", 2684683, 1375136, 2082203, false));
            nbajam_players.Add(new playerData(168, "Xavier McDaniel", 2257097, 1375520, 2079057, false));
            nbajam_players.Add(new playerData(169, "Yinka Dare", 2675963, 2456576, 2080825, false));


        }

        private void initializeFont()
        {
            //This defines the height and width for each character
            //The array numbers are currently a mystery.
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

            //Character graphics are defined in the following regions:
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
            letters[33].SetPixel(0, 0, 0);
            letters[33].SetPixel(1, 0, 3);
            letters[33].SetPixel(2, 0, 0);
            letters[33].SetPixel(3, 0, 0);

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
            letters[44].SetPixel(2, 1, 0);
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

            letters[54].SetPixel(0, 5, 0);
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

        //is this really needed? wtf is this
        private void test()
        { 
            Console.WriteLine("Test");
        }

        //cycle thru player portraits
        private void button2_Click(object sender, EventArgs e)
        {
            
            int temp = Convert.ToInt32(textBox1.Text);
            temp = temp + 1744;
            textBox1.Text = temp.ToString();
        }

        //cycle thru player portraits
        private void button3_Click(object sender, EventArgs e)
        {
            int temp = Convert.ToInt32(textBox1.Text);
            temp = temp - 1744;
            textBox1.Text = temp.ToString();
        }

        //some dumb thing to test flipping pictures
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                Image rawr = pictureBox1.Image;
                rawr.RotateFlip(RotateFlipType.RotateNoneFlipX);
                pictureBox1.Image = rawr;
            }
        }
        
        //load the nametag graphic from the ROM
        //this function should be rewritten to take an offset location and return an Image. 
        //might have been coded elsewhere already. 
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
                tiles[i] = new Bitmap(nametag_corrected_width, nametag_corrected_height);
            }

            //this is just for drawing the nametag in the UI - either way the background will be (128,0,128) in the game
            //ie alpha values ignored
            if (checkBox2.Checked)
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(255, 128, 0, 128);  //color         
            }
            else
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(0, 128, 0, 128);  //transparent    
            }
     
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
            int countur = 0;

            for (int s = 0; s < nametag_total_tiles; s++)
            {
                tiletest = getTile(countur, nametag);
                tiles[s] = tile2bitmap(tiletest, nametag_pallete, nametag_corrected_width, nametag_corrected_height);
                countur = countur + 32;
            }

            countur = 0;

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
                    tiles[countur].RotateFlip(RotateFlipType.RotateNoneFlipX);

                    gr.DrawImage(tiles[countur], rect);

                    countur++;
                }
            }

            nametagBitmap = (Bitmap)bmap.Clone();

            pictureBox3.Image = nametagBitmap;


        }

        private void loadNametag(int location)
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
                tiles[i] = new Bitmap(nametag_corrected_width, nametag_corrected_height);
            }

            //this is just for drawing the nametag in the UI - either way the background will be (128,0,128) in the game
            //ie alpha values ignored
            if (checkBox2.Checked)
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(255, 128, 0, 128);  //color         
            }
            else
            {
                nametag_pallete[0] = System.Drawing.Color.FromArgb(0, 128, 0, 128);  //transparent    
            }

            start_address = location;

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
            int countur = 0;

            for (int s = 0; s < nametag_total_tiles; s++)
            {
                tiletest = getTile(countur, nametag);
                tiles[s] = tile2bitmap(tiletest, nametag_pallete, nametag_corrected_width, nametag_corrected_height);
                countur = countur + 32;
            }

            countur = 0;

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
            byte[] tile = new byte[64];
            byte[] temp = new byte[4];
            int offset = address;
            int counter = 0;

            //flatten the layers here
            for (int i = 0; i < 8; i++)
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
                    pixels[i] = (byte)backArray2[startx + i, starty + y];

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

            int counter = 0;
            int pixelX = tile_size_X / 8;
            int pixelY = tile_size_Y / 8;

            Bitmap tilebitmap = new Bitmap(tile_size_X, tile_size_Y);

            //adjust pixel size based on bitmap size. 
            for (int y = 0; y < 8; y++) //8 here is the original tile height
            {
                for (int x = 0; x < 8; x++) //8 here is the original tile width
                {
                    for (int q = 0; q < pixelX; q++)
                    {
                        tilebitmap.SetPixel((x * (pixelX)) + q, (y * pixelY), palette[tile_data[counter]]);
                        for (int r = 0; r < pixelY; r++)
                        {
                            tilebitmap.SetPixel((x * (pixelX)) + q, (y * pixelY) + r, palette[tile_data[counter]]);
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

        private void loadStats(int location)
        {
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

            //These are static values assigned to numerical values 0-10 (Only 0-9 shown in game; 10 is like "Mad Quick")
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
            speed_lookup[10] = 0x17DE;

            //These are static values assigned to numerical values 0-10 (Only 0-9 shown in game; 10 is like "Gimme the Hot Sauce")
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
            threes_lookup[10] = 0x04E2;

            start_address = location;
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
            playerValue = (stats[1] << 8) | stats[0];
            bodySize = (stats[3] << 8) | stats[2];
            speedRating = (stats[5] << 8) | stats[4];
            threeptRating = (stats[7] << 8) | stats[6];
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
                if (speed_lookup[i] == speedRating)
                    speedRating = i;
            }


            for (int j = 0; j < 11; j++)
            {
                if (threes_lookup[j] == threeptRating)
                    threeptRating = j;
            }
            //3s
            // label10.Text = threeptRating.ToString();

            threeptsUpDown.Value = threeptRating;
            stats_3ptsNum.Text = threeptRating.ToString();
            //speed
            speedUpDown.Value = speedRating;
            stats_speedNum.Text = speedRating.ToString();
            //dunk
            // label11.Text = dunkRating.ToString();
            dunkUpDown.Value = dunkRating;
            stats_dunkNum.Text = dunkRating.ToString();
            //pass
            //  label12.Text = passRating.ToString();
            passUpDown.Value = passRating;
            stats_passNum.Text = passRating.ToString();
            //power
            powerUpDown.Value = powerRating;
            stats_powerNum.Text = powerRating.ToString();
            //steal
            //  label14.Text = stealRating.ToString();
            stealUpDown.Value = stealRating;
            stats_stealNum.Text = stealRating.ToString();
            //block
            //  label15.Text = blockRating.ToString();
            blockUpDown.Value = blockRating;
            stats_blockNum.Text = blockRating.ToString();
            //clutch
            //  label16.Text = clutchRating.ToString();
            clutchUpDown.Value = clutchRating;
            stats_clutchNum.Text = clutchRating.ToString();
        }


        private void button9_Click(object sender, EventArgs e)
        {
            //player stat graphics display start at 0x930C8 ROM
            // 12b0c9 RAM
            //4ce79f - ROM where player names are stored IN ASCII

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

            //These are static values assigned to numerical values 0-10 (Only 0-9 shown in game; 10 is like "Mad Quick")
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
            speed_lookup[10] = 0x17DE;

            //These are static values assigned to numerical values 0-10 (Only 0-9 shown in game; 10 is like "Gimme the Hot Sauce")
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
            threes_lookup[10] = 0x04E2;

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
            playerValue = (stats[1] << 8) | stats[0];
            bodySize = (stats[3] << 8) | stats[2];
            speedRating = (stats[5] << 8) | stats[4];
            threeptRating = (stats[7] << 8) | stats[6];
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
                if (speed_lookup[i] == speedRating)
                    speedRating = i;
            }


            for (int j = 0; j < 11; j++)
            {
                if (threes_lookup[j] == threeptRating)
                    threeptRating = j;
            }
            //3s
            // label10.Text = threeptRating.ToString();
            
            threeptsUpDown.Value = threeptRating;
            stats_3ptsNum.Text = threeptRating.ToString();
            //speed
            speedUpDown.Value = speedRating;
            stats_speedNum.Text = speedRating.ToString();
            //dunk
            // label11.Text = dunkRating.ToString();
            dunkUpDown.Value = dunkRating;
            stats_dunkNum.Text = dunkRating.ToString();
            //pass
            //  label12.Text = passRating.ToString();
            passUpDown.Value = passRating;
            stats_passNum.Text = passRating.ToString();
            //power
            powerUpDown.Value = powerRating;
            stats_powerNum.Text = powerRating.ToString();
            //steal
            //  label14.Text = stealRating.ToString();
            stealUpDown.Value = stealRating;
            stats_stealNum.Text = stealRating.ToString();
            //block
            //  label15.Text = blockRating.ToString();
            blockUpDown.Value = blockRating;
            stats_blockNum.Text = blockRating.ToString();
            //clutch
            //  label16.Text = clutchRating.ToString();
            clutchUpDown.Value = clutchRating;
            stats_clutchNum.Text = clutchRating.ToString();

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

        private void button10_Click(object sender, EventArgs e)
        {
            //int[,] backArray = new int[48, 16];

            //Declaration of jagged array.
            //This creates 12 tiletests, each tiletest is an array of 64 bytes.
            byte[][] tiletest = new byte[12][];

            string balls;
            int locationx = 0;
            int locationy = 0;
            balls = textBox4.Text;
            int z;
            int f = 0;
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


            for (int i = 0; i < nametag_total_tiles; i++)
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
            locationy = 7;

            //  locationx = 0;
            //  locationy = 0;

            foreach (int fu in textname)
            {
                for (int y = 0; y < letters[fu].Height; y++)
                {
                    for (int x = 0; x < letters[fu].Width; x++)
                    {
                        if ((locationx + x < 48))
                            backArray[locationx + x, locationy + y] = letters[fu].getPixel(x, y);
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
            //  nbajamTextBox1.Image = nametagBitmap;

            for (int q = 0; q < 16; q++)
            {
                for (int v = 0; v < 48; v++)
                {
                    backArray2[v, q] = backArray[v, q]; //just copies this to a global variable to use in the save prodcedure. stupid as hell. 
                    backArray[v, q] = 0; //clears background array. 

                }
            }

            button11.Enabled = true;


        }

        private void button11_Click(object sender, EventArgs e)
        {
            //save dat shit
            byte[] nametag_4bpp = new byte[Constants.nametagSize];
            byte[] tile = new byte[32];
            int counter = 0;
            int offset = 0;
            //   string balls = "C:\\Users\\dext3r\\Desktop\\dicks.sfc";
            //   string balls2= "C:\\Users\\dext3r\\Desktop\\pokey.sfc";
            int locationx = 0;
            int locationy = 0;
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


            for (int z = 0; z < 2; z++)
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
            //button10_Click(sender, e);
            nbajamTextBox20.Text = textBox4.Text;
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

        private void button12_Click(object sender, EventArgs e)
        {
            //pictureBox4.BackColor = System.Drawing.Color.FromArgb(255, 128, 0, 128); 
            nbajamTextBox1.Text = "Spaget!";
            nbajamTextBox1.setFontColorbyIndex(14);

            timer1.Start();


            //bajamTextBox1.BackColor = System.Drawing.Color.FromArgb(255, 128, 0, 0); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            nbajamTextBox1.TextJustify = nbajamTextBox.nbajamTextBox.TextJustifyOptions.Manual;
            nbajamTextBox1.setFontColorbyIndex(timerthing);
            nbajamTextBox1.setOffsetX(animator);
            timerthing++;
            animator += 1;
            if (animator == 8 * nbajamTextBox1.TilesWide)
                animator = 0;
            if (timerthing == 15)
                timerthing = 0;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            timer1.Stop();

            nbajamTextBox1.TextJustify = nbajamTextBox.nbajamTextBox.TextJustifyOptions.Center;
            nbajamTextBox1.Text = "FUCK YOU!";
            nbajamTextBox1.setFontColorbyIndex(3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            MessageBox.Show(nbajamTextBox20.getLinearArraySize().ToString());
            byte[] localByteArray = new byte[nbajamTextBox20.getLinearArraySize()];

            localByteArray = nbajamTextBox20.getLinearArray();
            int nameOffset = Convert.ToInt32(textBox2.Text);


            for (int i = 0; i < Constants.nametagSize; i++)
            {
                fileBuffer[nameOffset] = localByteArray[i];
                nameOffset++;
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            switch (listBox1.SelectedIndex)
            {
                case 0:
                    nbajamTextBox1.Text = "chicago bulls";
                    nbajamTextBox1.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_7;
                    nbajamTextBox1.BackColor = Color.FromKnownColor(KnownColor.Maroon);
                    pictureBox5.BackColor = Color.FromKnownColor(KnownColor.Black);
                    break;
                case 1:
                    nbajamTextBox1.Text = "dallas mavericks";
                    nbajamTextBox1.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_14;
                    nbajamTextBox1.BackColor = Color.FromKnownColor(KnownColor.MediumBlue);
                    pictureBox5.BackColor = Color.FromKnownColor(KnownColor.ForestGreen);

                    break;
                case 2:
                    nbajamTextBox1.Text = "seattle supersonics";
                    nbajamTextBox1.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_8;
                    nbajamTextBox1.BackColor = Color.FromKnownColor(KnownColor.DarkGreen);
                    pictureBox5.BackColor = Color.FromKnownColor(KnownColor.Gold);
                    break;
            }*/


        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /* Don't do any of this shit yet
             * switch (tabControl1.SelectedIndex)
            {
                case 0:

                    panel1.Visible = false;
                    panel2.Visible = true;

                    panel2.BringToFront();
                    panel1.SendToBack();
                    //  panel1.Visible = false;

                    break;
                case 1:
                    panel1.BringToFront();
                    panel2.SendToBack();
                    panel1.Visible = true;
                    panel2.Visible = false;
                    break;
                case 2:
                    MessageBox.Show("Not Implemented Yet!");
                    break;
            }*/
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            int pal_index = 1;
            byte[] new_color_pal = new byte[64]; //32 color @ 2 bytes/col
            int q = 0;

            if (dialogOpenFile.ShowDialog() == DialogResult.OK)
            {
                sourceFileInfo = new FileInfo(dialogOpenFile.FileName);
                sourceImage = Image.FromFile(dialogOpenFile.FileName);

                //prepare the quantizer
                the_quantizer.Clear();

                Image targetImage = GetQuantizedImage(sourceImage);
                Bitmap gayness = (Bitmap)targetImage;
                pictureBox6.Image = targetImage;

                List<Color> yourColorList = the_quantizer.GetPalette(31);

                foreach (Color color in yourColorList)
                {
                    optimized_palette[pal_index] = color;
                    pal_index++;
                    Console.WriteLine("Win Color: " + color.ToString());
                    Console.WriteLine(toSNESColor(color).ToString("X4"));
                }


                //swap pal 5 and 0, cuz we be hackin all night long
                //     Color temp = optimized_palette[0];
                //    optimized_palette[0] = optimized_palette[5];
                //    optimized_palette[5] = temp;


                foreach (Color color in optimized_palette)
                {


                    new_color_pal[q] = (byte)(toSNESColor(color));
                    new_color_pal[q + 1] = (byte)(toSNESColor(color) >> 8);
                    Console.WriteLine("Snes: " + new_color_pal[q].ToString("X2") + new_color_pal[q + 1].ToString("X2"));
                    q = q + 2;

                }


                //load array with palette values
                for (int y = 0; y < 56; y++)
                {
                    for (int x = 0; x < 48; x++)
                    {
                        new_back_array[x, y] = (byte)colorIndexLookup(gayness.GetPixel(x, y));
                    }
                }


                for (int y_tiles2 = 0; y_tiles2 < 56; y_tiles2++)
                {
                    for (int x_tiles2 = 0; x_tiles2 < 48; x_tiles2++)
                    {
                        Console.Write("{0:X2}", new_back_array[x_tiles2, y_tiles2]);
                    }
                    Console.WriteLine();
                }

                byte[] localByteArray = new byte[1680];

                localByteArray = getLinearArray();
                int nameOffset = Convert.ToInt32(textBox1.Text);


                for (int i = 0; i < 1680; i++)
                {
                    fileBuffer[nameOffset] = localByteArray[i];
                    nameOffset++;
                }

                for (int ugh = 0; ugh < 64; ugh++)
                {
                    fileBuffer[nameOffset] = new_color_pal[ugh];
                    nameOffset++;
                }
            }
        }

        public byte[] getLinearArray()
        {
            byte[] flat_5bpp_array = new byte[1680];
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


        private int colorIndexLookup(Color input)
        {
            int the_one = 0;

            for (int x = 0; x < 32; x++)
            {
                if (input == optimized_palette[x])
                    the_one = x;
            }

            return the_one;
        }


        private UInt16 toSNESColor(Color inputColor)
        {
            UInt16 output = 0;
            int r, g, b;

            r = (int)inputColor.R / 8;
            g = (int)inputColor.G / 8;
            b = (int)inputColor.B / 8;

            output = (UInt16)(b * 1024 + g * 32 + r);

            return output;
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
                Int32 colorCount = 32; //GetColorCount();
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

        private void pictureBox6_Click(object sender, EventArgs e)
        {

        }


        //Button 16? - "fuck shit up"
        //Seems to:
        /*
         * 1. load an image into an nbajamPictureBox (which auto quantizes the palette down to 31 colors + transparent)
         * 2. create an array to hold the picture data and pallete info
         * 3. call the methods from nbajamPictureBox to load the linear array of portrait data and pallete info
         * 4. load the info at the location specified by textbox1.text (portrait location offset)
  
         */
        private void button16_Click(object sender, EventArgs e)
        {
            if (dialogOpenFile.ShowDialog() == DialogResult.OK)
            {
    
                nbajamPictureBox1.loadNewImage(Image.FromFile(dialogOpenFile.FileName));
               

                byte[] localByteArray = new byte[1680];
                byte[] localPalette = new byte[64];

                localByteArray = nbajamPictureBox1.get5bppLinearArray();
          //      localByteArray = nbajamPictureBox3.get5bppLinearArray();

                localPalette = nbajamPictureBox1.getPortraitPalette();
             //   localPalette = nbajamPictureBox3.getPortraitPalette();
                

                int nameOffset = Convert.ToInt32(textBox1.Text);

                for (int i = 0; i < 1680; i++)
                {
                    fileBuffer[nameOffset] = localByteArray[i];
                    nameOffset++;
                }

                for (int ugh = 0; ugh < 64; ugh++)
                {
                    fileBuffer[nameOffset] = localPalette[ugh];
                    nameOffset++;
                }
            }
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This is an editor for the SNES version of NBA JAM: Tournament Edition\n\nBig thanks to Mattrizzle for all the game info.\n\nI am not associated with the NBA or EA or whoever owns the NBA JAM trademark nowadays. Please don't sue. ");
        }

        private void nbajamPictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void nbajamPictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            MessageBox.Show("X:" + e.X.ToString() + ",Y:" + e.Y.ToString());
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

       
        private void stats_speedNum_textChanged(object sender, nbajamTextBox.TextChangedEventArgs e)
        {
            Int16 stat_value = Convert.ToInt16(e.NewValue);

            if (sender is nbajamTextBox.nbajamTextBox)
            {
                nbajamTextBox.nbajamTextBox dummy = (nbajamTextBox.nbajamTextBox)sender;
                if (stat_value < 3) //0,1,2
                {
                    //Console.WriteLine("Made it to the event test: " + e.NewValue);
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_4;
                }
                else if (stat_value > 2 && stat_value < 8)
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_7;
                else
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_3;
            }
        }

        private void stats_powerNum_textChanged(object sender, nbajamTextBox.TextChangedEventArgs e)
        {
            Int16 stat_value = Convert.ToInt16(e.NewValue);

            if (sender is nbajamTextBox.nbajamTextBox)
            {
                nbajamTextBox.nbajamTextBox dummy = (nbajamTextBox.nbajamTextBox)sender;
                if (stat_value < 3) //0,1,2
                {
                    //Console.WriteLine("Made it to the event test: " + e.NewValue);
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_4;
                }
                else if (stat_value > 2 && stat_value < 8)
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_7;
                else
                    dummy.FontColor = nbajamTextBox.nbajamTextBox.FontColorOptions.Pallete_3;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            playerData jammers = nbajam_players.ElementAt(comboBox1.SelectedIndex);
            loadStats(jammers.getStats());
            loadPortrait(jammers.getPortrait());
            loadNametag(jammers.getNametag());

        }

      
        
    }

    /*
    * Constants is a simple class to hold various game specific constants - like the array size of the player information.
    */
        static class Constants
        {

            public const int portraitBitmapSize = 1680; // Number of bytes needed to hold a player portrait
            public const int portraitPaletteSize = 64;  // Number of bytes needed to hold the portrait pallete info

            public const int num_nametag_tilesX = 6;    // Width (in 8x8 tiles) of the player "nametag" - 6*8 = 48 pixels wide
            public const int num_nametag_tilesY = 2;    // Height (in 8x8 tiles) of the player "nametag" - 2*8 = 16 pixels tall
            public const int nametagSize = 384;         // Number of bytes required to hold the "nametag" graphic information.

            public const int playerStats_size = 26;     // Number of bytes required to hold the player stat information (0x1A bytes)
        }

        /*
         * fontTile is a simple class that lets you define a {Width} and {Height} for an font character 'tile' and
         * also stores an {x,y} pixel map for the character's graphics.
         * 
         * Note: colors are not defined in the fontTitle itself - the values stored in the pixel map are arbitrary as far as
         * the fontTile is concerned. It is up to the renderer to determine what the {value} means (ie what color should be displayed).
         */
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

        /* 
         *  player Data seems to hold a bunch of internal player info - not really implemented yet
         */
        public class playerData
        {
            private int ID;
            private String internalName;
            private int portraitLocation;
            private int nametagLocation;
            private int statsLocation;
            private bool secretPlayer;

            public playerData(int addID, String addName, int addPortrait, int addNametag, int addStats, bool addSecret)
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

