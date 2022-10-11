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
    public partial class Etapa1OverrideForm : Form
    {
        public string[] UserKeywords { get { return textInput.Lines; } }

        public Etapa1OverrideForm(string[]? keywords = null)
        {
            InitializeComponent();

            if (keywords != null)
            {
                textInput.Lines = keywords;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
