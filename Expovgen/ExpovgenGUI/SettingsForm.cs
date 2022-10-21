using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            //if (Settings.GenerationType == GenerationType.AudioOnlyGen)
            //{
            //    podcastOpt.Checked = true;
            //}
            /*else */
            
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (Settings.Etapa1Behaviors == Etapa1Behaviors.ForceOneByParagraph)
            {
                apresentacaoOpt.Checked = true;
            }
            else
            {
                videoaulaOpt.Checked = true;
            }

            kw_enableOpt.Checked = Settings.Etapa1Behaviors == Etapa1Behaviors.Auto || Settings.Etapa1Behaviors == Etapa1Behaviors.AutoManual;
            kw_enableOpt_CheckedChanged(this, new EventArgs());
            kw_reviseOpt.Checked = Settings.Etapa1Behaviors == Etapa1Behaviors.AutoManual;
            kw_ShowOnImagesOpt.Checked = Settings.ShowKeywordOnImage;

            img_enableOpt.Checked = Settings.Etapa2Behaviors == Etapa2Behaviors.Auto || Settings.Etapa2Behaviors == Etapa2Behaviors.AutoManual;
            img_enableOpt_CheckedChanged(this, new EventArgs());
            img_reviseOpt.Checked = Settings.Etapa2Behaviors == Etapa2Behaviors.AutoManual;
            imgProviderCombo.SelectedIndex = Settings.ImgFetchService == Expovgen.ImgFetch.Services.google ? 0 : 1;
            imgs_ccOpt.Checked = Settings.UseCCLicense;

            autoNarrationEnableOpt.Checked = Settings.Etapa3Behaviors == Etapa3Behaviors.Auto;

            windowStyleCombo.SelectedIndex = Settings.WindowStyle == WindowStyle.Simple ? 0 : 1;
        }

        private void SaveSettings()
        {
            //Handling Etapa 1
            if (apresentacaoOpt.Checked)
            {
                Settings.Etapa1Behaviors = Etapa1Behaviors.ForceOneByParagraph;
            }
            else if (kw_enableOpt.Checked && !kw_reviseOpt.Checked)
            {
                Settings.Etapa1Behaviors = Etapa1Behaviors.Auto;
            }
            else if (kw_enableOpt.Checked && kw_enableOpt.Checked)
            {
                Settings.Etapa1Behaviors = Etapa1Behaviors.AutoManual;
            }
            else
            {
                Settings.Etapa1Behaviors = Etapa1Behaviors.ForceManual;
            }

            Settings.ShowKeywordOnImage = kw_ShowOnImagesOpt.Checked;

            //Handling Etapa 2
            if (img_enableOpt.Checked && !img_reviseOpt.Checked)
            {
                Settings.Etapa2Behaviors = Etapa2Behaviors.Auto;
            }
            else if (img_enableOpt.Checked && img_reviseOpt.Checked)
            {
                Settings.Etapa2Behaviors = Etapa2Behaviors.AutoManual;
            }
            else
            {
                Settings.Etapa2Behaviors = Etapa2Behaviors.ForceManual;
            }

            Settings.UseCCLicense = imgs_ccOpt.Checked;
            Settings.ImgFetchService = imgProviderCombo.SelectedIndex == 0 ? Expovgen.ImgFetch.Services.google : Expovgen.ImgFetch.Services.pixabay;

            //Handling Etapa 3
            Settings.Etapa3Behaviors = autoNarrationEnableOpt.Checked ? Etapa3Behaviors.Auto : Etapa3Behaviors.ForceManual;

            //Handling Other opts
            Settings.WindowStyle = windowStyleCombo.SelectedIndex == 0 ? WindowStyle.Simple : WindowStyle.Detailed;
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
                keywordsGbx.Visible = true;
                imagesGbx.Visible = true;
            }
        }

        private void apresentacaoOpt_CheckedChanged(object sender, EventArgs e)
        {
            if (apresentacaoOpt.Checked)
            {
                label2.Text = "No modo apresentação, vídeos são gerados de forma que cada parágrafo do roteiro corresponde, no vídeo final, a uma imagem a ser enviada manualmente. Assim, é possível ter melhor controle acerca de quando cada imagem é mostrada, ideal para apresentações de slides";
                Settings.GenerationType = GenerationType.VideoGen;
                Settings.Etapa1Behaviors = Etapa1Behaviors.ForceOneByParagraph;
                LoadSettings();
                keywordsGbx.Visible = false;
                imagesGbx.Visible = false;
            }
        }

        private void podcastOpt_CheckedChanged(object sender, EventArgs e)
        {
            //if (podcastOpt.Checked)
            {
                label2.Text = "No modo podcast, apenas áudios são gerados pelo programa, através do serviço de Conversão de Texto para Voz (TTS)";
                Settings.GenerationType = GenerationType.AudioOnlyGen;
                LoadSettings();
            }
        }

        private void imgProviderCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Visible = imgProviderCombo.SelectedIndex == 1;
            pictureBox1.Visible = imgProviderCombo.SelectedIndex == 1;
            label5.Visible = imgProviderCombo.SelectedIndex == 1;
            linkLabel1.Visible = imgProviderCombo.SelectedIndex == 1;
            imgs_ccOpt.Enabled = imgProviderCombo.SelectedIndex == 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://pixabay.com/",
                UseShellExecute = true
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "https://pixabay.com/service/license/",
                UseShellExecute = true
            });
        }

        private void kw_enableOpt_CheckedChanged(object sender, EventArgs e)
        {
            kw_reviseOpt.Enabled = kw_enableOpt.Checked;
        }

        private void img_enableOpt_CheckedChanged(object sender, EventArgs e)
        {
            imgProviderCombo.Enabled = img_enableOpt.Checked;
            img_reviseOpt.Enabled = img_enableOpt.Checked;
            pictureBox1.Enabled = img_enableOpt.Checked;
            linkLabel1.Enabled = img_enableOpt.Checked;
            label7.Enabled = img_enableOpt.Checked;
            label5.Enabled = img_enableOpt.Checked;
            imgs_ccOpt.Enabled = imgProviderCombo.SelectedIndex == 1 ? false : img_enableOpt.Checked;
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
