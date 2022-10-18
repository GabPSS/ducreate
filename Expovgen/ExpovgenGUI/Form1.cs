using Expovgen;
using System.Diagnostics;

namespace ExpovgenGUI
{
    public partial class Form1 : Form
    {
        ExpovgenGenerationSettings settings;
        public Form1()
        {
            InitializeComponent();
            settings = new ExpovgenGenerationSettings();
            propertyGrid1.SelectedObject = settings;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings.VideoDimensions = (Convert.ToInt32(numericUpDown1.Value), Convert.ToInt32(numericUpDown2.Value));
            List<string>? check = settings.CheckValid();
            if (check is not null)
            {
                string errorout = "";
                foreach (string errorstr in check)
                {
                    errorout += " -> " + errorstr + "\n";
                }
                MessageBox.Show("Configurações inválidas! Favor resolver os seguintes conflitos:\n\n" + errorout + "\nNenhuma alteração foi realizada", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new CreatingVideoForm(textBox1.Lines,settings).Show();
            //TODO: (IDEA) Quick video generation wizard.
            /* Idea description:
             *   It's a wizard where you load in a txt file, select
             *   video generation options, and the video is generated
             *   without having to necessarily create a project and all
             *   of that
             */
        }
    }
}