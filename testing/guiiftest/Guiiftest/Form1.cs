using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using libimgfetch;

namespace Guiiftest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Stream> fileStreams = ImgFetch.GetImageStreams((Services)comboBox1.SelectedIndex, textBox1.Text);
            foreach (Stream file in fileStreams)
            {
                try
                {
                    Image img = Bitmap.FromStream(file);
                    PictureBox pbx = new PictureBox();
                    pbx.Width = 322;
                    pbx.Height = 181;
                    pbx.SizeMode = PictureBoxSizeMode.Zoom;
                    pbx.Image = img;
                    flowLayoutPanel1.Controls.Add(pbx);
                }
                catch (Exception x)
                {
                    Console.Error.WriteLine("Error while creating image from streams: " + x.Message);
                }
            }
        }
    }
}
