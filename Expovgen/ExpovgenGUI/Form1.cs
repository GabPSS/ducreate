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
            new CreatingVideoForm(textBox1.Lines,settings).Show();
        }
    }
}