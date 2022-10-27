namespace ExpovgenGUI
{
    partial class CreatingVideoForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreatingVideoForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.detailsViewTLP = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.simpleViewTLP = new System.Windows.Forms.TableLayoutPanel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.unifiedProgressBar = new System.Windows.Forms.ProgressBar();
            this.showMoreButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.detailsViewTLP.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.simpleViewTLP.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 19);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(409, 20);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(409, 20);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.MinimumSize = new System.Drawing.Size(415, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(415, 42);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Processo de geração";
            // 
            // detailsViewTLP
            // 
            this.detailsViewTLP.AutoSize = true;
            this.detailsViewTLP.ColumnCount = 1;
            this.detailsViewTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsViewTLP.Controls.Add(this.groupBox1, 0, 0);
            this.detailsViewTLP.Controls.Add(this.groupBox2, 0, 1);
            this.detailsViewTLP.Location = new System.Drawing.Point(196, 9);
            this.detailsViewTLP.Margin = new System.Windows.Forms.Padding(0);
            this.detailsViewTLP.Name = "detailsViewTLP";
            this.detailsViewTLP.Padding = new System.Windows.Forms.Padding(10);
            this.detailsViewTLP.RowCount = 3;
            this.detailsViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.detailsViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.detailsViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.detailsViewTLP.Size = new System.Drawing.Size(441, 287);
            this.detailsViewTLP.TabIndex = 2;
            this.detailsViewTLP.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBox1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(13, 61);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(415, 184);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Informações";
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 15;
            this.listBox1.Location = new System.Drawing.Point(3, 19);
            this.listBox1.MinimumSize = new System.Drawing.Size(350, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(409, 162);
            this.listBox1.TabIndex = 0;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelBtn.Location = new System.Drawing.Point(401, 105);
            this.cancelBtn.Margin = new System.Windows.Forms.Padding(3, 10, 10, 3);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(74, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "&Cancelar";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.asCancel_Click);
            // 
            // simpleViewTLP
            // 
            this.simpleViewTLP.AutoSize = true;
            this.simpleViewTLP.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.simpleViewTLP.ColumnCount = 2;
            this.simpleViewTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.simpleViewTLP.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.simpleViewTLP.Controls.Add(this.statusLabel, 0, 0);
            this.simpleViewTLP.Controls.Add(this.cancelBtn, 1, 2);
            this.simpleViewTLP.Controls.Add(this.unifiedProgressBar, 0, 1);
            this.simpleViewTLP.Controls.Add(this.showMoreButton, 0, 2);
            this.simpleViewTLP.Dock = System.Windows.Forms.DockStyle.Fill;
            this.simpleViewTLP.Location = new System.Drawing.Point(0, 0);
            this.simpleViewTLP.Name = "simpleViewTLP";
            this.simpleViewTLP.Padding = new System.Windows.Forms.Padding(10);
            this.simpleViewTLP.RowCount = 3;
            this.simpleViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.simpleViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.simpleViewTLP.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.simpleViewTLP.Size = new System.Drawing.Size(486, 144);
            this.simpleViewTLP.TabIndex = 3;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.simpleViewTLP.SetColumnSpan(this.statusLabel, 2);
            this.statusLabel.Location = new System.Drawing.Point(20, 20);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(10);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(176, 15);
            this.statusLabel.TabIndex = 0;
            this.statusLabel.Text = "Iniciando processo de geração...";
            // 
            // unifiedProgressBar
            // 
            this.unifiedProgressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.simpleViewTLP.SetColumnSpan(this.unifiedProgressBar, 2);
            this.unifiedProgressBar.Location = new System.Drawing.Point(20, 55);
            this.unifiedProgressBar.Margin = new System.Windows.Forms.Padding(10);
            this.unifiedProgressBar.MarqueeAnimationSpeed = 10;
            this.unifiedProgressBar.Name = "unifiedProgressBar";
            this.unifiedProgressBar.Size = new System.Drawing.Size(455, 30);
            this.unifiedProgressBar.TabIndex = 1;
            // 
            // showMoreButton
            // 
            this.showMoreButton.Location = new System.Drawing.Point(20, 105);
            this.showMoreButton.Margin = new System.Windows.Forms.Padding(10);
            this.showMoreButton.Name = "showMoreButton";
            this.showMoreButton.Size = new System.Drawing.Size(112, 23);
            this.showMoreButton.TabIndex = 2;
            this.showMoreButton.Text = "&Mostrar detalhes";
            this.showMoreButton.UseVisualStyleBackColor = true;
            this.showMoreButton.Click += new System.EventHandler(this.showMoreButton_Click);
            // 
            // CreatingVideoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(486, 144);
            this.ControlBox = false;
            this.Controls.Add(this.simpleViewTLP);
            this.Controls.Add(this.detailsViewTLP);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreatingVideoForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gerando...";
            this.Load += new System.EventHandler(this.CreatingVideoForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.detailsViewTLP.ResumeLayout(false);
            this.detailsViewTLP.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.simpleViewTLP.ResumeLayout(false);
            this.simpleViewTLP.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private GroupBox groupBox1;
        private TableLayoutPanel detailsViewTLP;
        private Button cancelBtn;
        private GroupBox groupBox2;
        private ListBox listBox1;
        private TableLayoutPanel simpleViewTLP;
        private Label statusLabel;
        private ProgressBar unifiedProgressBar;
        private Button showMoreButton;
    }
}