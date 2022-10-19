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
    public partial class SettingsForm : Form
    {
        public ExpovgenGenerationSettings Settings { get; set; }
        public SettingsForm(ExpovgenGenerationSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            SettingsModified = false;
            if (Settings.GenerationType == GenerationType.AudioOnlyGen)
            {
                podcastOpt.Checked = true;
            }
            else if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph)
            {
                apresentacaoOpt.Checked = true;
            }
            else
            {
                videoaulaOpt.Checked = true;
            }
            UpdateControls();
        }

        private bool SettingsModified
        {
            get
            {
                return SettingsModified;
            }
            set
            {
                applyBtn.Enabled = value;
            }
        }
        //TODO: Apply settings, Save settings, return new settings object
        //TODO: Give all controls helpprovider strings

        private void UpdateControls()
        {
            //Load project type
            if (Settings.GenerationType == GenerationType.AudioOnlyGen)
            {
                //podcastOpt.Checked = true;
                keywordsGbx.Visible = false;
                imagesGbx.Visible = false;
                if (tabControl1.TabPages.Contains(tabPage4))
                {
                    tabControl1.TabPages.Remove(tabPage4);
                }
            }
            else if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph)
            {
                //apresentacaoOpt.Checked = true;
                keywordsGbx.Visible = false;
                imagesGbx.Visible = false;
                if (!tabControl1.TabPages.Contains(tabPage4))
                {
                    tabControl1.TabPages.Add(tabPage4);
                }
            }
            else
            {
                //videoaulaOpt.Checked = true;
                if (!tabControl1.TabPages.Contains(tabPage4))
                {
                    tabControl1.TabPages.Add(tabPage4);
                }
            }


            //TODO: Implement this method
            /* Tasks necessary:
             *   - Hiding groupboxes and tabpages when project type is audio only
             *   - Updating controls with respective values
             * 
             */
        }

        private void RemoveInput()
        {
            //TODO: Implement this method
        }

        private void AddInput()
        {
            //TODO: Implement this method along with RemoveInput
        }

        private void videoaulaOpt_CheckedChanged(object sender, EventArgs e)
        {
            if (videoaulaOpt.Checked)
            {
                label2.Text = "No modo videoaula, vídeos são gerados através da busca de imagens por palavra-chave, assim compondo uma apresentação mais dinâmica. A geração de conteúdo pode ser automática, sem interrupção do início ao fim";
                Settings.GenerationType = GenerationType.VideoGen;
                if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph)
                {
                    Settings.Etapa1Behaviors = Etapa1Behaviors.Auto;
                }
                SettingsModified = true;
                UpdateControls();
            }
        }

        private void apresentacaoOpt_CheckedChanged(object sender, EventArgs e)
        {
            if (apresentacaoOpt.Checked)
            {
                label2.Text = "No modo apresentação, vídeos são gerados de forma que cada parágrafo do roteiro corresponde, no vídeo final, a uma imagem a ser enviada manualmente. Assim, é possível ter melhor controle acerca de quando cada imagem é mostrada, ideal para apresentações de slides";
                Settings.GenerationType = GenerationType.VideoGen;
                Settings.Etapa1Behaviors = Etapa1Behaviors.ForceOneByParagraph;
                SettingsModified = true;
                UpdateControls();
            }
        }

        private void podcastOpt_CheckedChanged(object sender, EventArgs e)
        {
            if (podcastOpt.Checked)
            {
                label2.Text = "No modo podcast, apenas áudios são gerados pelo programa, através do serviço de Conversão de Texto para Voz (TTS)";
                Settings.GenerationType = GenerationType.AudioOnlyGen;
                SettingsModified = true;
                UpdateControls();
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            volValueLabel.Text = trackBar1.Value + "%";
            Settings.BackgroundVolume = Convert.ToDouble(trackBar1.Value) / 100;
        }
    }
}
