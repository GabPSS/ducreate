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
    public partial class CreatingVideoForm : Form
    {
        //TODO: Implement this form

        
        private string[] Document { get; set; }
        readonly string[] titles = { "Extração de palavras-chave", "Pesquisa de imagens", "Geração da narração", "Alinhamento forçado", "Renderização (pode levar um tempo)" };
        List<PictureBox> pbxs = new();
        List<Label> lbls = new();
        List<ProgressBar> pbrs = new();
        ExpovgenGenerationSettings Settings { get; set; }

        public CreatingVideoForm(string[] document, ExpovgenGenerationSettings settings)
        {
            InitializeComponent();
            Document = document;
            Settings = settings;
            if (settings.GenerationType == GenerationType.VideoGen)
            {
                for (int i = 0; i < titles.Length; i++)
                {
                    PictureBox pbx = new()
                    {
                        Image = Properties.Resources.circle_FILL0_wght400_GRAD0_opsz48,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(48, 48)
                    };

                    Label lbl = new()
                    {
                        Text = titles[i],
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        AutoSize = true
                    };

                    ProgressBar pbr = new()
                    {
                        Anchor = AnchorStyles.Left | AnchorStyles.Right,
                        MarqueeAnimationSpeed = 50
                    };

                    pbxs.Add(pbx);
                    lbls.Add(lbl);
                    pbrs.Add(pbr);
                    
                    tableLayoutPanel1.Controls.Add(pbx);
                    tableLayoutPanel1.Controls.Add(lbl);
                    tableLayoutPanel1.Controls.Add(pbr);

                    tableLayoutPanel1.SetCellPosition(pbx, new TableLayoutPanelCellPosition(0, i));
                    tableLayoutPanel1.SetCellPosition(lbl, new TableLayoutPanelCellPosition(1, i));
                    tableLayoutPanel1.SetCellPosition(pbr, new TableLayoutPanelCellPosition(2, i));

                    if (i < (titles.Length - 1))
                        tableLayoutPanel1.RowCount++;
                }
            }
            else if (settings.GenerationType == GenerationType.AudioOnlyGen)
            {
                //TODO: Implement audio-only generation
                throw new NotImplementedException();
            }
        }


        #region Video generation

        Expovgen.Expovgen expovgen;

        private void StartProcess()
        {
            //Create new video generator
            //TODO: Make these settings user-editable
            expovgen = new Expovgen.Expovgen(Settings);

            //Create files and directories
            Directory.CreateDirectory("res");
            File.WriteAllLines("res\\text.txt", Document);

            //Create events
            expovgen.Logger.TextWritten += Logger_TextWritten;

            expovgen.Etapa1Complete += Expovgen_Etapa1Complete;
            expovgen.Etapa1Failed += Expovgen_Etapa1Failed;

            expovgen.Etapa2Complete += Expovgen_Etapa2Complete;
            expovgen.Etapa2Incomplete += Expovgen_Etapa2Incomplete;

            expovgen.Etapa3Completed += Expovgen_Etapa3Completed;
            expovgen.Etapa3Failed += Expovgen_Etapa3Failed;

            expovgen.Etapa4Completed += Expovgen_Etapa4Completed;
            expovgen.Etapa4Failed += Expovgen_Etapa4Failed;

            expovgen.Etapa5Completed += Expovgen_Etapa5Completed;
            expovgen.Etapa5Failed += Expovgen_Etapa5Failed;

            //Begin first sttep
            StartStep(0);
            expovgen.Etapa1();
        }

        int currentStep = 0;

        /// <summary>
        /// Provides an interface for handling logs written by Expovgen logger
        /// </summary>
        private void Logger_TextWritten(object source, ExpovgenLogs.TextWrittenEventArgs e)
        {
            listBox1.Invoke(new Action(() =>
            {
                if (currentStep == 4)
                {
                    try
                    {
                        string[] progfind = e.WrittenText.Substring(3).Split('%');
                        int val = Convert.ToInt32(progfind[0]);
                        pbrs[4].Value = val;
                    } catch { }
                }
                listBox1.Items.Add(e.WrittenText);
            }));
            File.AppendAllLines("res\\logs.txt", new string[] { e.WrittenText });
        }

        #region Form-specific/visual methods

        private void CompleteStep(int stepNum)
        {
            Invoke(new Action(() =>
            {
                pbxs[stepNum].Image = Properties.Resources.check_circle_FILL0_wght400_GRAD0_opsz48;
                pbrs[stepNum].Value = 100;
                pbrs[stepNum].Style = ProgressBarStyle.Blocks;
            }));
        }

        private void StartStep(int stepNum)
        {
            Invoke(new Action(() =>
            {
                pbxs[stepNum].Image = Properties.Resources.pending_FILL0_wght400_GRAD0_opsz48;
                pbrs[stepNum].Value = 20;
                pbrs[stepNum].Style = ProgressBarStyle.Marquee;
                currentStep = stepNum;
            }));
        }

        private void Cancel()
        {
            shouldCancel = true;
            Expovgen.Expovgen.Dispose();
            expovgen.Logger.TextWritten -= Logger_TextWritten;

            expovgen.Etapa1Complete -= Expovgen_Etapa1Complete;
            expovgen.Etapa1Failed -= Expovgen_Etapa1Failed;

            expovgen.Etapa2Complete -= Expovgen_Etapa2Complete;
            expovgen.Etapa2Incomplete -= Expovgen_Etapa2Incomplete;

            expovgen.Etapa3Completed -= Expovgen_Etapa3Completed;
            expovgen.Etapa3Failed -= Expovgen_Etapa3Failed;

            expovgen.Etapa4Completed -= Expovgen_Etapa4Completed;
            expovgen.Etapa4Failed -= Expovgen_Etapa4Failed;
            
            expovgen.Etapa5Completed -= Expovgen_Etapa5Completed;
            expovgen.Etapa5Failed -= Expovgen_Etapa5Failed;

            expovgen.Logger.WriteLine("====== OPERAÇÃO CANCELADA PELO USUÁRIO ======");
            expovgen.Logger.WriteLine("Aguardando finalização dos processos");

            Expovgen.Expovgen.Dispose();
            Close();
        }

        #endregion

        #region Etapa 1 Implementation

        private void Expovgen_Etapa1Complete(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            CompleteStep(0);
            StartStep(1);
            if (shouldCancel)
            {
                return;
            }
            expovgen.Etapa2(e.Keywords);
        }

        private void Expovgen_Etapa1Failed(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            if (shouldCancel)
            {
                return;
            }
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
            if (shouldCancel)
            {
                return;
            }
            MessageBox.Show(
                e.OverrideRequestedIntentionally ? "Etapa 2 concluída, verifique as imagens" : "Etapa 2 falhou! Verifique as imagens",
                e.OverrideRequestedIntentionally ? "Informação" : "Erro",
                MessageBoxButtons.OK,
                e.OverrideRequestedIntentionally ? MessageBoxIcon.Information : MessageBoxIcon.Error
                );


            Etapa2OverrideForm overrideForm = new Etapa2OverrideForm(e.RequestQueries, e.Images, Settings.videoDimensions);
            DialogResult result = DialogResult.None;
            this.Invoke(new Action(() =>
            {
                result = overrideForm.ShowDialog();
            }));

            if (result == DialogResult.OK)
            {
                CompleteStep(1);
                StartStep(2);
                if (shouldCancel)
                {
                    return;
                }
                expovgen.Etapa3();
            }
        }

        private void Expovgen_Etapa2Complete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            CompleteStep(1);
            StartStep(2);
            if (shouldCancel)
            {
                return;
            }
            expovgen.Etapa3();
        }
        #endregion

        #region Etapa 3 Implementation

        private void Expovgen_Etapa3Completed(object sender, Expovgen.Expovgen.Etapa3EventArgs e)
        {
            if (shouldCancel)
            {
                return;
            }
            CompleteStep(2);
            StartStep(3);
            expovgen.Etapa4();
        }

        private void Expovgen_Etapa3Failed(object sender, Expovgen.Expovgen.Etapa3EventArgs e)
        {
            if (shouldCancel)
            {
                return;
            }
            //TODO: Melhorar o override
            DialogResult q = MessageBox.Show("A etapa de geração de áudio falhou. Verifique sua conexão com a internet.\n\nDeseja enviar um arquivo de narração local?\n\nAperte CONTINUAR para enviar um arquivo local, TENTAR NOVAMENTE para repetir a etapa, e CANCELAR para cancelar o processo de geração de vídeo", "Erro", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error);
            switch (q)
            {
                case DialogResult.TryAgain:
                    expovgen.Etapa3();
                    break;
                case DialogResult.Continue:
                    OpenFileDialog dialog = new OpenFileDialog() { Title = "Selecione um arquivo de narração", Filter = "Arquivos MP3|*.mp3" };
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(dialog.FileName, "res\\speech.mp3");
                    }
                    CompleteStep(2);
                    StartStep(3);
                    expovgen.Etapa4();
                    break;
                case DialogResult.Cancel:
                    Invoke(new Action(() =>
                    {
                        Dispose();
                    }));
                    break;
            }
        }

        #endregion

        #region Etapa 4 Implementation

        private void Expovgen_Etapa4Failed(object sender, EventArgs e)
        {
            MessageBox.Show("FATAL: O processo de geração de vídeo falhou, devido a um erro na etapa de alinhamento forçado. Caso o erro persistir, tente reinstalar o programa", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Invoke(new Action(() =>
            {
                Dispose();
            }));
        }

        private void Expovgen_Etapa4Completed(object sender, EventArgs e)
        {
            if (shouldCancel)
            {
                return;
            }
            CompleteStep(3);
            StartStep(4);
            expovgen.Etapa5();
        }

        #endregion

        #region Etapa 5 Implementation

        private void Expovgen_Etapa5Failed(object sender, EventArgs e)
        {
            MessageBox.Show("FATAL: O processo de geração de vídeo falhou, devido a um erro na etapa de renderização. Caso o erro persistir, tente reinstalar o programa", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Invoke(new Action(() =>
            {
                Dispose();
            }));
        }

        private void Expovgen_Etapa5Completed(object sender, EventArgs e)
        {
            CompleteStep(4);
            MessageBox.Show("Vídeo gerado com sucesso!","Sucesso",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        #endregion

        #endregion

        Thread vidGenThread;
        bool shouldCancel = false;

        private void CreatingVideoForm_Load(object sender, EventArgs e)
        {
            vidGenThread = new Thread(StartProcess);
            vidGenThread.Name = "Expovgen Video Generator";
            vidGenThread.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza de que deseja cancelar?", "Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Cancel();
            }
        }
    }
}
