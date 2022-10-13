using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExpovgenGUI
{
    public partial class CreatingVideoForm : Form
    {
        //TODO: Implement this form

        public enum GenerationType { VideoGen, AudioOnlyGen }

        readonly string[] titles = { "Extração de palavras-chave", "Pesquisa de imagens", "Geração da narração", "Alinhamento forçado", "Renderização (pode levar um tempo)" };
        List<PictureBox> pbxs = new();
        List<Label> lbls = new();
        List<ProgressBar> pbrs = new();

        public CreatingVideoForm(string[] Document, GenerationType contentType)
        {
            InitializeComponent();
            
            if (contentType == GenerationType.VideoGen)
            {
                foreach (string title in titles)
                {
                    PictureBox pbx = new PictureBox()
                    {
                        Image = Properties.Resources.circle_FILL0_wght400_GRAD0_opsz48,
                        SizeMode = PictureBoxSizeMode.AutoSize
                    };
                    //TODO: --- Stopped here ---
                    throw new NotImplementedException();
                }
            }
            else if (contentType == GenerationType.AudioOnlyGen)
            {
                //TODO: Implement audio-only generation
                throw new NotImplementedException();
            }
        }
    }
}
