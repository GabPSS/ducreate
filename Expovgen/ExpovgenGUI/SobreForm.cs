﻿using System;
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
    public partial class SobreForm : Form
    {
        public SobreForm()
        {
            InitializeComponent();
            string nome = Application.ProductName;
            label2.Text = "Sobre o " + nome;
            label3.Text = String.Format(label3.Text, nome);
        }
    }
}
