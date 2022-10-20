using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Expovgen;

namespace ExpovgenGUI
{
    public partial class ExportForm : Form
    {
        public ExpovgenGenerationSettings Settings;
        public ExportForm(ExpovgenGenerationSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            DoCheck(textBox3);
            DoCheck(textBox4);
            LoadSettings();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            volValueLabel.Text = musFundoVolTrackbar.Value + "%";
            Settings.BackgroundVolume = Convert.ToDouble(musFundoVolTrackbar.Value) / 100;
        }

        private void LoadSettings()
        {
            larguraNum.Value = Settings.VideoDimensions.width;
            alturaNum.Value = Settings.VideoDimensions.height;
            fonteNum.Value = Convert.ToInt32(Settings.FontSize);
            if (Settings.BackgroundMusicPath is not null)
            {
                txtMusFundoPath.Text = Settings.BackgroundMusicPath;
                musFundoLimparBtn.Enabled = true;
            }
            musFundoVolTrackbar.Value = Convert.ToInt32(Settings.BackgroundVolume * 100);
            trackBar1_Scroll(this, new EventArgs());
            filePathToSave.Text = Settings.ExportPath == null ? "(Nenhum caminho selecionado)" : Settings.ExportPath;
            if (Settings.ExportPath is null)
            {
                button4.Enabled = false;
            }

        }

        private bool SaveSettings()
        {
            Settings.VideoDimensions = (Convert.ToInt32(larguraNum.Value), Convert.ToInt32(alturaNum.Value));
            Settings.FontSize = Convert.ToDouble(fonteNum.Value);
            Settings.BackgroundMusicPath = txtMusFundoPath.Text == "(Nada selecionado)" ? null : txtMusFundoPath.Text;
            Settings.BackgroundVolume = Convert.ToDouble(musFundoVolTrackbar.Value) / 100;
            Settings.ExportPath = filePathToSave.Text == "(Nenhum caminho selecionado)" ? null : filePathToSave.Text;
            if (Settings.ExportPath == null)
                return false;
            else
                return true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (SaveSettings())
            {
                Close();
            }
            else
            {
                MessageBox.Show("Favor selecione um caminho para salvar o conteúdo", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);   
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            DoCheck(textBox3);
        }

        private void DoCheck(Control ctrl)
        {
            if (ctrl.Text == "")
            {
                ctrl.Update();
                ctrl.CreateGraphics().DrawString("Deixe em branco para \nnão incluir este card \nno vídeo final", new Font("Segoe UI", 8), Brushes.Gray, 2, 2);
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            DoCheck(textBox3);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            DoCheck(textBox4);
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            DoCheck(textBox4);
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {

        }

        private void ExportForm_Paint(object sender, PaintEventArgs e)
        {
            DoCheck(textBox4);
            DoCheck(textBox3);
        }

        bool mostrarConfig = false;

        private void button1_Click(object sender, EventArgs e)
        {
            if (mostrarConfig)
            {
                OcultarConfig();
            }
            else
            {
                MostrarConfig();
            }
        }

        private void OcultarConfig()
        {

        }

        private void MostrarConfig()
        {

        }
    }
}
