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
        bool _Changed = false;
        bool Changed
        {
            get
            {
                return _Changed;
            }
            set
            {
                _Changed = value;
                if (value)
                {
                    Text = Text.Substring(0, 1) == "*" ? Text : "*" + Text;
                }
                else if (Text.Substring(0, 1) == "*")
                {
                    Text = Text[1..];
                }                    
            } 
        }
        string? _openFilePath;
        string? openFilePath { get { return _openFilePath; } set 
            {
                _openFilePath = value;
                Text = Application.ProductName + " v" + Application.ProductVersion + 
                    (value is not null ? " - [" + value + "]" : " - [Arquivo não salvo]");
            } }
        ExpovgenGenerationSettings Settings { get; set; }
        public MainForm(string? filePath)
        {
            InitializeComponent();
            if (filePath is not null)
            {
                if (!DoOpening(filePath))
                {
                    New();
                }
                //Se funcionou, yay!
            }
            else
            {
                Settings = new();
            }

            Text = Application.ProductName + " v" + Application.ProductVersion +
                    (openFilePath is not null ? " - [" + openFilePath + "]" : " - [Arquivo não salvo]");
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

        /// <summary>
        /// Saves current expovgen settings to a file
        /// </summary>
        /// <param name="toPath">Specify if the program should save to a specific file path. Otherwise the path to save will be <see cref="openFilePath"/></param>
        private void Salvar(string? toPath = null)
        {
            attempt:
            toPath = toPath is null ? openFilePath : toPath;
            if (toPath == null)
            {
                SalvarComo();
                return;
            }

            try
            {
                Settings.Document = textBox1.Lines;
                if (Settings.Save(toPath))
                {
                    Changed = false;
                    openFilePath = toPath;
                }
                else
                    throw new Exception();
            } 
            catch (Exception ex)
            {
                if (MessageBox.Show("Erro ao salvar o arquivo [" + toPath + "]. Verifique se o caminho é acessível e se nenhum outro programa está utilizando o arquivo.\n\n" + ex.Message + "\n\nClique em Tentar para tentar novamente", "Erro", MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                    goto attempt;
            }

        }

        private void SalvarComo()
        {
            SaveFileDialog saveDialog = new()
            {
                Title = "Salvar projeto como...",
                Filter = "Arquivos de projeto|*.exvpj",
                DefaultExt = "exvpj"
            };

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                Salvar(saveDialog.FileName);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!CheckAndSave())
                e.Cancel = true;
            
        }

        /// <summary>
        /// Check if changes were made, then ask to save them if they were
        /// </summary>
        /// <returns>True if process can continue or false if it should be cancelled</returns>
        private bool CheckAndSave()
        {
            if (Changed)
            {
                DialogResult dr = MessageBox.Show("Você tem alterações não salvas, deseja salvar?", "Aviso", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                switch (dr)
                {
                    case DialogResult.Yes:
                        Salvar();
                        return true;
                    case DialogResult.No:
                        //Só não salvar
                        return true;
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            New();
        }

        /// <summary>
        /// Check if file isn't saved, then resets everything and creates a new <see cref="ExpovgenGenerationSettings"/>
        /// </summary>
        private void New()
        {
            if (CheckAndSave())
            {
                textBox1.Lines = Array.Empty<string>();
                Settings = new();
                openFilePath = null;
                Changed = false;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {
            OpenFileDialog openDialog = new()
            {
                Title = "Selecione um arquivo",
                Filter = "Arquivos suportados|*.txt;*.exvpj|Arquivo de projeto|*.exvpj|Arquivo de texto|*.txt",
                ValidateNames = true
            };
            if (openFilePath is not null)
            {
                openDialog.InitialDirectory = openFilePath.Substring(0, (openFilePath.Length - openFilePath.Split("\\").Last().Length)-1);
            }

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                DoOpening(openDialog.FileName);
            }

        }

        private bool DoOpening(string fileName)
        {
            ExpovgenGenerationSettings? openAttempt = ExpovgenGenerationSettings.FromFile(fileName);
            if (openAttempt != null)
            {
                Settings = openAttempt;
                textBox1.Lines = Settings.Document;
                Changed = false;
                openFilePath = fileName;
                return true;
            }
            else
            {
                MessageBox.Show("O arquivo [" + fileName + "] não pôde ser aberto! Verifique se o caminho é acessível e não está aberto por outro programa");
                return false;
            }
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
                renderer.ShowDialog();
                Changed = true;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void salvarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Salvar();
        }

        private void salvarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SalvarComo();
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open();
        }

        private void novoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            New();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new(Settings);
            DialogResult dr = sf.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Settings = sf.Settings;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            undoBuffer = textBox1.Lines;
            textBox1.Undo();
        }

        private void desfazerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoBuffer = textBox1.Lines;
            textBox1.Undo();
        }

        string[] undoBuffer = Array.Empty<string>();

        private void refazerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (undoBuffer != Array.Empty<string>())
            {
                textBox1.Lines = undoBuffer;
            }
        }

        private void exportarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Export();
        }

        private void sobreOProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO: Make about screen with credits
            new SobreForm().ShowDialog();
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void cortarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Cut();
        }

        private void copiarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Copy();
        }

        private void colarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Paste();
        }

        private void abrirSiteDoProgramaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "https://gabpss.github.io/ducreate",
                UseShellExecute = true
            });
        }

        private void propriedadesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new(Settings);
            DialogResult dr = sf.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Settings = sf.Settings;
            }
        }
    }

}
