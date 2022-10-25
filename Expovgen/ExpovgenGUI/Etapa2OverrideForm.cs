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
        private Image NoImage;
        private (int width, int height) VideoDimensions { get; set; }
        public bool OverlayKeywordOverImage { get; set; } = false;

        public Etapa2OverrideForm(string[] Keywords, List<Image?> Images, (int width, int height) videoDimensions)
        {
            InitializeComponent();
            VideoDimensions = videoDimensions;
            imageList1.ImageSize = new Size(120, 120 * VideoDimensions.height / VideoDimensions.width);

            NoImage = Expovgen.ImgFetch.ImgFetch2.ResizeImage(Ducreate.Properties.Resources.photoless, imageList1.ImageSize.Width, imageList1.ImageSize.Height, Brushes.LightGray);
            listView1.MultiSelect = false;
            listView1.Activation = ItemActivation.TwoClick;
            listView1.ItemActivate += ReplaceImage;
            FormClosing += SaveAllImages;
            for (int i = 0; i < Keywords.Length; i++)
            {
                AddItem(Keywords[i], Images[i]);
            }
        }

        private void SaveAllImages(object? sender, FormClosingEventArgs e)
        {
            if (cancelClose)
            {
                List<int> indices = new();
                for (int i = 0; i < imageList1.Images.Count; i++)
                {
                    Image? image = ((ImageItem)listView1.Items[i]).ResultingImage;
                    if (image is null)
                    {
                        indices.Add(i + 1);
                    }
                }

                if (indices.Count > 0)
                {
                    string outputString = "";
                    for (int i = 0; i < indices.Count; i++)
                    {
                        outputString += indices[i] + "ª";
                        if (indices.Count >= 2 && i == indices.Count - 2)
                        {
                            outputString += " e a ";
                        }
                        else if (i < indices.Count - 1)
                        {
                            outputString += ", a ";
                        }
                    }
                    MessageBox.Show("Favor adicionar a " + outputString + " imagem", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                else
                {
                    List<Image> resultimages = new();
                    foreach (ImageItem item in listView1.Items)
                    {
                        Image newItem = Expovgen.ImgFetch.ImgFetch2.ResizeImage(item.ResultingImage, VideoDimensions.width, VideoDimensions.height, Brushes.Black, OverlayKeywordOverImage, item.Text);
                        resultimages.Add(newItem);
                    }
                    Expovgen.Expovgen.SaveImgfetchPictures(resultimages);
                    DialogResult = DialogResult.OK;
                }
            } else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        /// Method called when an image is activated and is ready to be replaced
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReplaceImage(object? sender, EventArgs e)
        {
            ImageItem item = listView1.SelectedItems[0] as ImageItem;
            int index = listView1.Items.IndexOf(item);

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "Arquivos de imagem suportados|*.jpg;*.png;*.bmp;*.gif|All files|*.*",
                Title="Select a replacement image",
                Multiselect=false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image attempt = Image.FromFile(openFileDialog.FileName);
                    Image resized = Expovgen.ImgFetch.ImgFetch2.ResizeImage(attempt, imageList1.ImageSize.Width, imageList1.ImageSize.Height, Brushes.Black, OverlayKeywordOverImage, listView1.Items[index].Text);
                    item.ResultingImage = attempt;
                    imageList1.Images[index] = resized;
                }
                catch
                {
                    MessageBox.Show("Erro ao importar o arquivo! Verifique se o caminho é acessível ou se o formato do arquivo corresponde a um formato de imagem válido");
                }
            }
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
                displayedImage = NoImage;
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
        bool cancelClose = true;
        private void button2_Click(object sender, EventArgs e)
        {
            cancelClose = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
