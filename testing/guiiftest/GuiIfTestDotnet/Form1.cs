using libimgfetch;

namespace GuiIfTestDotnet
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<Stream> fileStreams = new List<Stream>();
            Services service = (Services)comboBox1.SelectedIndex;
            string query = textBox1.Text;
            foreach (Control c in flowLayoutPanel1.Controls)
            {
                c.Dispose(); 
            }
            flowLayoutPanel1.Controls.Clear();

            Cursor = Cursors.WaitCursor;
            Thread imgfetch = new Thread(() => { 
                WriteLine("Obtendo imagens através da internet...");
                SetProgress(20,ProgressBarStyle.Marquee);
                ImgFetch fetcher = new ImgFetch();
                fileStreams = fetcher.RequestImageStreams(service,query);
                
                int i = 0;
                foreach (Stream file in fileStreams)
                {
                    this.Invoke(new Action(() =>
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
                            i++;
                        }
                        catch (Exception x)
                        {
                            Console.Error.WriteLine("Error while creating image from streams: " + x.Message);
                        }
                    }));
                }
                WriteLine(i + " imagens adicionadas.");
                this.Invoke(new Action(() =>
                {
                    Cursor = Cursors.Default;
                }));
                SetProgress(100, ProgressBarStyle.Continuous);
            });
            imgfetch.Name = "ImgFetch";
            imgfetch.Start();
        }

        void WriteLine(string text)
        {
            this.Invoke(new Action(() =>
            {
                statusStrip1.Items[0].Text = text;
            }));
        }

        void SetProgress(int value, ProgressBarStyle style)
        {
            this.Invoke(new Action(() =>
            {
                toolStripProgressBar1.Value = value;
                toolStripProgressBar1.Style = style;
            }));
        }
    }
}