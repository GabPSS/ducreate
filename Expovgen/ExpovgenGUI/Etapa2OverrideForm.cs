using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpovgenGUI
{
    public partial class Etapa2OverrideForm : Form
    {
        public Etapa2OverrideForm(string[] Keywords, Image?[] Images)
        {
            InitializeComponent();
            //TODO: Add routine for adding items
            throw new NotImplementedException();
        }

        private class ImageItem : ListViewItem
        {
            public Image ResultingImage { get; set; }
        }

        private void AddItem(string text, Image? image)
        {
            Image? displayedImage = image;
            if (displayedImage == null)
            {
                displayedImage = Properties.Resources.photoless;
            }

            imageList1.Images.Add(displayedImage);

            ImageItem item = new()
            {
                Text = text,
                ResultingImage = image,
                ImageIndex = imageList1.Images.Count - 1
            };

            listView1.Items.Add(item);
        }
    }
}
