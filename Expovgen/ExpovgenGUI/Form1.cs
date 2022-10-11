using Expovgen;

namespace ExpovgenGUI
{
    public partial class Form1 : Form
    {
        Expovgen.Expovgen expovgen;
        (int width, int height) videoDimensions = (1366, 768); //TODO: Make video dimensions user-editable
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Create new video generator
            expovgen = new Expovgen.Expovgen()
            {
                VideoDimensions = videoDimensions,
                Etapa1Behavior = Etapa1Behaviors.ForceOneByParagraph,
                Etapa2Behavior = Etapa2Behaviors.ForceManual,
                Etapa3Behavior = Etapa3Behaviors.ForceManual
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

            expovgen.Etapa3Completed += Expovgen_Etapa3Completed;
            expovgen.Etapa3Failed += Expovgen_Etapa3Failed;

            //Begin first sttep
            expovgen.Etapa1();
        }

        

        /// <summary>
        /// Provides an interface for handling logs written by Expovgen logger
        /// </summary>
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
                //Prepare override form
                Etapa1OverrideForm overrideForm = new(e.Keywords);
                if (overrideForm.ShowDialog() == DialogResult.OK)
                {
                    //Perform override
                    expovgen.OverrideEtapa1(overrideForm.UserKeywords);
                }
            }
        }

        #endregion
    
        #region Etapa 2 Implementation
        private void Expovgen_Etapa2Incomplete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            MessageBox.Show(
                e.OverrideRequestedIntentionally ? "Etapa 2 concluída, verifique as imagens" : "Etapa 2 falhou! Verifique as imagens",
                e.OverrideRequestedIntentionally ? "Informação" : "Erro",
                MessageBoxButtons.OK,
                e.OverrideRequestedIntentionally ? MessageBoxIcon.Information : MessageBoxIcon.Error
                );
            
            
            Etapa2OverrideForm overrideForm = new Etapa2OverrideForm(e.RequestQueries, e.Images, videoDimensions);
            if (overrideForm.ShowDialog() == DialogResult.OK)
            {
                expovgen.Etapa3();
            }
        }

        private void Expovgen_Etapa2Complete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            expovgen.Etapa3();
        }
        #endregion

        #region Etapa 3 Implementation

        private void Expovgen_Etapa3Completed(object sender, Expovgen.Expovgen.Etapa3EventArgs e)
        {
            expovgen.Etapa4();
            expovgen.Etapa5();
        }

        private void Expovgen_Etapa3Failed(object sender, Expovgen.Expovgen.Etapa3EventArgs e)
        {
            //TODO: Melhorar o override
            OpenFileDialog dialog = new OpenFileDialog() { Title = "Selecione um arquivo de narração", Filter = "Arquivos MP3|*.mp3" };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                File.Copy(dialog.FileName, "res\\speech.mp3");
            }
            expovgen.Etapa4();
            expovgen.Etapa5();
        }

        #endregion
    }
}