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
    public partial class MainForm : Form
    {
        bool Changed = false;
        ExpovgenGenerationSettings Settings { get; set; }
        public MainForm(ExpovgenGenerationSettings settings)
        {
            InitializeComponent();
            Settings = settings;
        }

        private void arquivoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Changed = true;
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Salvar()
        {
            Changed = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Changed)
            {
                DialogResult dr = MessageBox.Show("Você tem alterações não salvas, deseja salvar?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dr)
                {
                    case DialogResult.Yes:
                        Salvar();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            New();
        }

        private void New()
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {

        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Salvar();
        }

        private void Export()
        {
            ExportForm dialog = new ExportForm(Settings);
            DialogResult dr = dialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Settings = dialog.Settings;
                CreatingVideoForm renderer = new(textBox1.Lines, dialog.Settings);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Export();
        }
    }

}
