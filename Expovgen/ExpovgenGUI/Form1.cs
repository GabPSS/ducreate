using Expovgen;

namespace ExpovgenGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Expovgen.Expovgen expovgen;
        (int width, int height) videoDimensions = (1366, 768);
        private void button1_Click(object sender, EventArgs e)
        {
            //Create new video generator
            expovgen = new Expovgen.Expovgen()
            {
                VideoDimensions = videoDimensions
            };

            //Create files and directories
            Directory.CreateDirectory("res");
            File.WriteAllLines("res\\text.txt", textBox1.Lines);
            
            //Create events
            expovgen.Logger.TextWritten += Logger_TextWritten;

            expovgen.Etapa1Complete += Expovgen_Etapa1Complete;
            expovgen.Etapa1Failed += Expovgen_Etapa1Failed;

            expovgen.Etapa2Complete += Expovgen_Etapa2Complete;
            expovgen.Etapa2Incomplete += Expovgen_Etapa2Incomplete;

            //Begin first sttep
            expovgen.Etapa1();
        }
        private void Logger_TextWritten(object source, ExpovgenLogs.TextWrittenEventArgs e)
        {
            label1.Text = e.WrittenText;
            label1.Update();
            File.AppendAllLines("res\\logs.txt",new string[] { e.WrittenText });
        }

        #region Etapa 1 Implementation

        private void Expovgen_Etapa1Complete(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            expovgen.Etapa2(e.Keywords);
        }

        private void Expovgen_Etapa1Failed(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            if (MessageBox.Show(e.OverrideIntentionallyRequested ? "Etapa concluída, iniciando override" : "Etapa 1 falhou! Usar override?", 
                e.OverrideIntentionallyRequested ? "Sucesso" : "Erro", 
                e.OverrideIntentionallyRequested ? MessageBoxButtons.OK : MessageBoxButtons.OKCancel, 
                e.OverrideIntentionallyRequested ? MessageBoxIcon.Information : MessageBoxIcon.Error
                ) == DialogResult.OK)
            {
                Etapa1OverrideForm overrideForm = new(e.Keywords);
                if (overrideForm.ShowDialog() == DialogResult.OK)
                {
                    expovgen.OverrideEtapa1(overrideForm.UserKeywords);
                }
            }
        }

        #endregion
    
        #region Etapa 2 Implementation
        private void Expovgen_Etapa2Incomplete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            MessageBox.Show("Etapa 2 não concluída até o final! Favor verifique as imagens!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            Etapa2OverrideForm overrideForm = new Etapa2OverrideForm(e.RequestQueries, e.Images, videoDimensions);
            overrideForm.ShowDialog();
        }

        private void Expovgen_Etapa2Complete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            
        }
        #endregion

    }
}