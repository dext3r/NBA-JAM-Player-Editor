using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace playerPairing
{
    public partial class playerPairing : UserControl
    {
        private bool rosterFlag = false;
        
        public bool isExpandedRoster
        {
            get
            {
                return rosterFlag;
            }
            set
            {
                rosterFlag = value;
            }
        }

        //constructor
        public playerPairing()
        {
            InitializeComponent();

        }

       
        public void setName1(String name)
        {
            nbajamTextBox1.Text = name;
        }

        public void setName2(String name)
        {
            nbajamTextBox2.Text = name;
        }

        public void setPortrait1(byte[] data)
        {
            nbajamPictureBox1.DataSize = 1744;
            nbajamPictureBox1.PaletteSize = 32;
            nbajamPictureBox1.isPortrait = true;
            nbajamPictureBox1.loadImageData(data);
        }

        public void setPortrait2(byte[] data)
        {
            nbajamPictureBox2.DataSize = 1744;
            nbajamPictureBox2.PaletteSize = 32;
            nbajamPictureBox2.isPortrait = true;
            nbajamPictureBox2.isFlipped = true;
            nbajamPictureBox2.loadImageData(data);

        }

        private void nbajamTextBox9_Click(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (rosterFlag)
            {
                txtROSTER.Visible = true;
                txtEXP.Visible = true;
            }
        }
      
    }
}
