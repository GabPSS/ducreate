using Expovgen;

namespace ExpovgenGUI
{
    public partial class CreatingVideoForm : Form
    {
        private string[] Document { get; set; }
        private readonly string[] titles = { "Extração de palavras-chave", "Pesquisa de imagens", "Geração da narração", "Alinhamento", "Renderização (pode levar um tempo)" };
        private readonly List<PictureBox> pbxs = new();
        private readonly List<Label> lbls = new();
        private readonly List<ProgressBar> pbrs = new();

        private ExpovgenGenerationSettings Settings { get; set; }

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
                        Image = Ducreate.Properties.Resources.circle_FILL0_wght400_GRAD0_opsz48,
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

        private Expovgen.Expovgen expovgen;
        private int currentStep = 0;

        #region Process control methods

        private void StartProcess()
        {
            //Create new video generator
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

        private void SwitchToDetailsView()
        {
            SuspendLayout();
            simpleViewTLP.Dock = DockStyle.None;
            simpleViewTLP.Dock = DockStyle.Fill;
            simpleViewTLP.Controls.Remove(cancelBtn);
            simpleViewTLP.Visible = false;
            AutoSizeMode = AutoSizeMode.GrowOnly;
            detailsViewTLP.Visible = true;
            detailsViewTLP.Dock = DockStyle.Fill;
            detailsViewTLP.Controls.Add(cancelBtn);
            cancelBtn.Anchor = AnchorStyles.None;
            cancelBtn.Margin = new Padding(3);
            ResumeLayout();
        }

        private void SwitchStep()
        {
            CompleteStep(currentStep);
            StartStep(currentStep + 1);
        }
        #region Switching Steps
        private void CompleteStep(int stepNum)
        {
            Invoke(new Action(() =>
            {
                pbxs[stepNum].Image = Ducreate.Properties.Resources.check_circle_FILL0_wght400_GRAD0_opsz48;
                pbrs[stepNum].Value = 100;
                pbrs[stepNum].Style = ProgressBarStyle.Blocks;
                unifiedProgressBar.Value = 100;
                unifiedProgressBar.Style = ProgressBarStyle.Blocks;
            }));
        }

        private void StartStep(int stepNum)
        {
            if (stepNum >= pbxs.Count)
            {
                shouldCancel = true;
                Cancel();
                return;
            }
            Invoke(new Action(() =>
            {
                pbxs[stepNum].Image = Ducreate.Properties.Resources.pending_FILL0_wght400_GRAD0_opsz48;
                pbrs[stepNum].Value = 20;
                pbrs[stepNum].Style = ProgressBarStyle.Marquee;
                unifiedProgressBar.Value = 20;
                unifiedProgressBar.Style = ProgressBarStyle.Marquee;
                statusLabel.Text = "Etapa " + (stepNum + 1) + "/" + lbls.Count + ": " + lbls[stepNum].Text;
                currentStep = stepNum;
            }));
        }
        #endregion

        private void ShowStepAsErrored(int stepNum)
        {
            Invoke(new Action(() =>
            {
                pbxs[stepNum].Image = Ducreate.Properties.Resources.error_circle_rounded_FILL0_wght400_GRAD0_opsz48;
            }));
        }

        /// <summary>
        /// Attempts to cancel the ongoing process
        /// </summary>
        private void Cancel()
        {
            Invoke(new Action(() =>
            {
                shouldCancel = true;
                //Expovgen.Expovgen.Dispose();
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

                //Expovgen.Expovgen.Dispose();
                Close();
            }));
        }

        #endregion

        #region Etapa 1 Implementation

        private void Expovgen_Etapa1Complete(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            SwitchStep();
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
            ShowStepAsErrored(0);
            if (!e.OverrideIntentionallyRequested)
            {
                if (MessageBox.Show("Erro ao acessar API de extração de palavras-chave automática. Deseja fornescer as palavras-chave manualmente?", "Erro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    Cancel();
                    return;
                }
            }

            Etapa1OverrideForm overrideForm = new(e.Keywords);
            DialogResult dr = overrideForm.ShowDialog();
            switch (dr)
            {
                case DialogResult.OK:
                    expovgen.OverrideEtapa1(overrideForm.UserKeywords);
                    break;
                case DialogResult.Cancel:
                    Cancel();
                    break;
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
            ShowStepAsErrored(1);

            if (!e.OverrideRequestedIntentionally)
            {
                MessageBox.Show("Algumas imagens não puderam ser baixadas. Clique OK para fornecer estas manualmente", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            Etapa2OverrideForm overrideForm = new(e.RequestQueries, e.Images, (Settings.VideoWidth, Settings.VideoHeight))
            {
                OverlayKeywordOverImage = Settings.ShowKeywordOnImage
            };
            DialogResult result = DialogResult.None;
            this.Invoke(new Action(() =>
            {
                result = overrideForm.ShowDialog();
            }));

            if (result == DialogResult.OK)
            {
                SwitchStep();
                if (shouldCancel)
                {
                    return;
                }
                expovgen.Etapa3();
            }
            else
            {
                Cancel();
            }
        }

        private void Expovgen_Etapa2Complete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            SwitchStep();
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
            SwitchStep();
            expovgen.Etapa4();
        }

        private void Expovgen_Etapa3Failed(object sender, Expovgen.Expovgen.Etapa3EventArgs e)
        {
            if (shouldCancel)
            {
                return;
            }
            ShowStepAsErrored(2);

            DialogResult q;
            
            if (!e.OverrideIntentional)
            {
                q = MessageBox.Show("A etapa de geração de áudio falhou. Verifique sua conexão com a internet.\n\nDeseja enviar um arquivo de narração local?\n\nAperte CONTINUAR para enviar um arquivo local, TENTAR NOVAMENTE para repetir a etapa, e CANCELAR para cancelar o processo de geração de vídeo", "Erro", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error);
            }
            else
            {
                q = DialogResult.Continue;
            }
            
            switch (q)
            {
                case DialogResult.TryAgain:
                    expovgen.Etapa3();
                    break;
                case DialogResult.Continue:
                    OpenFileDialog dialog = new() { Title = "Selecione um arquivo de narração", Filter = "Arquivos MP3|*.mp3" };
                    DialogResult dr = DialogResult.None;
                    Invoke(new Action(() =>
                    {
                        dr = dialog.ShowDialog();
                    }));

                    if (dr == DialogResult.OK)
                    {
                        File.Copy(dialog.FileName, "res\\speech.mp3");
                    }
                    SwitchStep();
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
            ShowStepAsErrored(3);
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
            SwitchStep();
            expovgen.Etapa5();
        }

        #endregion

        #region Etapa 5 Implementation

        private void Expovgen_Etapa5Failed(object sender, EventArgs e)
        {
            ShowStepAsErrored(4);
            MessageBox.Show("FATAL: O processo de geração de vídeo falhou, devido a um erro na etapa de renderização. Caso o erro persistir, tente reinstalar o programa", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Invoke(new Action(() =>
            {
                Dispose();
            }));
        }

        private void Expovgen_Etapa5Completed(object sender, EventArgs e)
        {
            CompleteStep(4);
            MessageBox.Show("Vídeo gerado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Invoke(new Action(() =>
            {
                cancelBtn.Text = "Fechar";
                cancelBtn.Click -= asCancel_Click;
                cancelBtn.Click += asExit_Click;
            }));
            string dir = Settings.ExportPath.Substring(0, (Settings.ExportPath.Length - Settings.ExportPath.Split("\\").Last().Length) - 1);
            System.Diagnostics.Process.Start("explorer", dir);
        }

        #endregion

        #endregion

        private Thread vidGenThread;
        private bool shouldCancel = false;

        private void CreatingVideoForm_Load(object sender, EventArgs e)
        {
            //Switch to details view if requested
            if (Settings.WindowStyle == WindowStyle.Detailed)
            {
                SwitchToDetailsView();
            }
            
            //Create video generator thread
            vidGenThread = new Thread(StartProcess)
            {
                Name = "Expovgen Video Generator"
            };
            vidGenThread.Start();
        }

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
                        string[] progfind = e.WrittenText.Substring(10).Trim().Split('%');
                        int val = Convert.ToInt32(progfind[0]);
                        pbrs[4].Style = ProgressBarStyle.Continuous;
                        pbrs[4].Value = val;
                        unifiedProgressBar.Style = ProgressBarStyle.Continuous;
                        unifiedProgressBar.Value = val;

                    }
                    catch { }
                }
                listBox1.Items.Add(e.WrittenText);
            }));
            try
            {
                File.AppendAllLines("res\\logs.txt", new string[] { e.WrittenText });
            } catch { }
        }

        #region Cancel/Exit button

        private void asCancel_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Tem certeza de que deseja cancelar?", "Confirme", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                Cancel();
            }
        }

        private void asExit_Click(object? sender, EventArgs e)
        {
            Close();
        }

        #endregion

        private void showMoreButton_Click(object sender, EventArgs e)
        {
            SwitchToDetailsView();
        }
    }
}
