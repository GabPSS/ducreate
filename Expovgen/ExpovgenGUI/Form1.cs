using Expovgen;

namespace ExpovgenGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Expovgen.Expovgen expovgen;

        private void button1_Click(object sender, EventArgs e)
        {
            expovgen = new Expovgen.Expovgen();
            Directory.CreateDirectory("res");
            File.WriteAllLines("res\\text.txt", textBox1.Lines);
            expovgen.Logger.TextWritten += Logger_TextWritten;

            expovgen.Etapa1Complete += Expovgen_Etapa1Complete;
            expovgen.Etapa1Failed += Expovgen_Etapa1Failed;

            expovgen.Etapa2Complete += Expovgen_Etapa2Complete;
            expovgen.Etapa2Incomplete += Expovgen_Etapa2Incomplete;

            expovgen.Etapa1();
        }

        private void Expovgen_Etapa2Incomplete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            MessageBox.Show("Etapa 2 não concluída até o final! Favor verifique as imagens!","Erro");
            throw new NotImplementedException();
            //TODO: Connect and implement Etapa2OverrideForm
        }

        private void Expovgen_Etapa2Complete(object sender, Expovgen.Expovgen.Etapa2EventArgs e)
        {
            MessageBox.Show("Etapa 2 concluída!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Logger_TextWritten(object source, ExpovgenLogs.TextWrittenEventArgs e)
        {
            label1.Text = e.WrittenText;
            label1.Update();
            File.AppendAllLines("res\\logs.txt",new string[] { e.WrittenText });
        }

        private void Expovgen_Etapa1Failed(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            if (MessageBox.Show("Etapa 1 falhou! Usar override?", "Erro", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK)
            {
                expovgen.OverrideEtapa1(new string[] {"palavra"}); //TODO: Montar um override melhor
            }
        }

        private void Expovgen_Etapa1Complete(object source, Expovgen.Expovgen.Etapa1EventArgs e)
        {
            MessageBox.Show("Etapa 1 concluída! Aperte OK para iniciar a etapa 2", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            expovgen.Etapa2(e.Keywords);
        }
    }
}