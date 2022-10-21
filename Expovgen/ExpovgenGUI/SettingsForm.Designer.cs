namespace ExpovgenGUI
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.projtypeGbx = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.apresentacaoOpt = new System.Windows.Forms.RadioButton();
            this.videoaulaOpt = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.keywordsGbx = new System.Windows.Forms.GroupBox();
            this.kw_ShowOnImagesOpt = new System.Windows.Forms.CheckBox();
            this.kw_reviseOpt = new System.Windows.Forms.CheckBox();
            this.kw_enableOpt = new System.Windows.Forms.CheckBox();
            this.imagesGbx = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.imgs_ccOpt = new System.Windows.Forms.CheckBox();
            this.imgProviderCombo = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.img_reviseOpt = new System.Windows.Forms.CheckBox();
            this.img_enableOpt = new System.Windows.Forms.CheckBox();
            this.speechGbx = new System.Windows.Forms.GroupBox();
            this.autoNarrationEnableOpt = new System.Windows.Forms.CheckBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.contentwindowGbx = new System.Windows.Forms.GroupBox();
            this.windowStyleCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pythonGbx = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.OKBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.projtypeGbx.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.keywordsGbx.SuspendLayout();
            this.imagesGbx.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.speechGbx.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.contentwindowGbx.SuspendLayout();
            this.pythonGbx.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(383, 426);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.flowLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(375, 398);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Projeto";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.projtypeGbx);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(369, 392);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // projtypeGbx
            // 
            this.projtypeGbx.Controls.Add(this.label2);
            this.projtypeGbx.Controls.Add(this.apresentacaoOpt);
            this.projtypeGbx.Controls.Add(this.videoaulaOpt);
            this.projtypeGbx.Controls.Add(this.label1);
            this.helpProvider1.SetHelpKeyword(this.projtypeGbx, "Tipo de projeto");
            this.helpProvider1.SetHelpString(this.projtypeGbx, resources.GetString("projtypeGbx.HelpString"));
            this.projtypeGbx.Location = new System.Drawing.Point(10, 10);
            this.projtypeGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.projtypeGbx.Name = "projtypeGbx";
            this.helpProvider1.SetShowHelp(this.projtypeGbx, true);
            this.projtypeGbx.Size = new System.Drawing.Size(349, 183);
            this.projtypeGbx.TabIndex = 0;
            this.projtypeGbx.TabStop = false;
            this.projtypeGbx.Text = "Tipo de projeto";
            // 
            // label2
            // 
            this.helpProvider1.SetHelpKeyword(this.label2, "Tipo de projeto");
            this.helpProvider1.SetHelpString(this.label2, resources.GetString("label2.HelpString"));
            this.label2.Location = new System.Drawing.Point(20, 94);
            this.label2.Name = "label2";
            this.helpProvider1.SetShowHelp(this.label2, true);
            this.label2.Size = new System.Drawing.Size(311, 94);
            this.label2.TabIndex = 2;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // apresentacaoOpt
            // 
            this.apresentacaoOpt.AutoSize = true;
            this.apresentacaoOpt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.helpProvider1.SetHelpKeyword(this.apresentacaoOpt, "Tipo de projeto");
            this.helpProvider1.SetHelpString(this.apresentacaoOpt, resources.GetString("apresentacaoOpt.HelpString"));
            this.apresentacaoOpt.Location = new System.Drawing.Point(105, 60);
            this.apresentacaoOpt.Name = "apresentacaoOpt";
            this.helpProvider1.SetShowHelp(this.apresentacaoOpt, true);
            this.apresentacaoOpt.Size = new System.Drawing.Size(101, 19);
            this.apresentacaoOpt.TabIndex = 1;
            this.apresentacaoOpt.TabStop = true;
            this.apresentacaoOpt.Text = "Apresentação";
            this.apresentacaoOpt.UseVisualStyleBackColor = true;
            this.apresentacaoOpt.CheckedChanged += new System.EventHandler(this.apresentacaoOpt_CheckedChanged);
            // 
            // videoaulaOpt
            // 
            this.videoaulaOpt.AutoSize = true;
            this.videoaulaOpt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.helpProvider1.SetHelpKeyword(this.videoaulaOpt, "Tipo de projeto");
            this.helpProvider1.SetHelpString(this.videoaulaOpt, resources.GetString("videoaulaOpt.HelpString"));
            this.videoaulaOpt.Location = new System.Drawing.Point(20, 60);
            this.videoaulaOpt.Name = "videoaulaOpt";
            this.helpProvider1.SetShowHelp(this.videoaulaOpt, true);
            this.videoaulaOpt.Size = new System.Drawing.Size(79, 19);
            this.videoaulaOpt.TabIndex = 1;
            this.videoaulaOpt.TabStop = true;
            this.videoaulaOpt.Text = "Videoaula";
            this.videoaulaOpt.UseVisualStyleBackColor = true;
            this.videoaulaOpt.CheckedChanged += new System.EventHandler(this.videoaulaOpt_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.helpProvider1.SetHelpKeyword(this.label1, "Tipo de projeto");
            this.helpProvider1.SetHelpString(this.label1, resources.GetString("label1.HelpString"));
            this.label1.Location = new System.Drawing.Point(17, 34);
            this.label1.Name = "label1";
            this.helpProvider1.SetShowHelp(this.label1, true);
            this.label1.Size = new System.Drawing.Size(152, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selecione o tipo do projeto:";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.flowLayoutPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(375, 398);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Serviços";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.keywordsGbx);
            this.flowLayoutPanel3.Controls.Add(this.imagesGbx);
            this.flowLayoutPanel3.Controls.Add(this.speechGbx);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(369, 392);
            this.flowLayoutPanel3.TabIndex = 0;
            // 
            // keywordsGbx
            // 
            this.keywordsGbx.Controls.Add(this.kw_ShowOnImagesOpt);
            this.keywordsGbx.Controls.Add(this.kw_reviseOpt);
            this.keywordsGbx.Controls.Add(this.kw_enableOpt);
            this.keywordsGbx.Location = new System.Drawing.Point(10, 10);
            this.keywordsGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.keywordsGbx.Name = "keywordsGbx";
            this.keywordsGbx.Size = new System.Drawing.Size(349, 112);
            this.keywordsGbx.TabIndex = 2;
            this.keywordsGbx.TabStop = false;
            this.keywordsGbx.Text = "Palavras-chave";
            // 
            // kw_ShowOnImagesOpt
            // 
            this.kw_ShowOnImagesOpt.AutoSize = true;
            this.kw_ShowOnImagesOpt.Location = new System.Drawing.Point(15, 78);
            this.kw_ShowOnImagesOpt.Name = "kw_ShowOnImagesOpt";
            this.kw_ShowOnImagesOpt.Size = new System.Drawing.Size(210, 19);
            this.kw_ShowOnImagesOpt.TabIndex = 2;
            this.kw_ShowOnImagesOpt.Text = "Incluir palavras-chave nas imagens";
            this.kw_ShowOnImagesOpt.UseVisualStyleBackColor = true;
            // 
            // kw_reviseOpt
            // 
            this.kw_reviseOpt.AutoSize = true;
            this.helpProvider1.SetHelpString(this.kw_reviseOpt, resources.GetString("kw_reviseOpt.HelpString"));
            this.kw_reviseOpt.Location = new System.Drawing.Point(15, 53);
            this.kw_reviseOpt.Name = "kw_reviseOpt";
            this.helpProvider1.SetShowHelp(this.kw_reviseOpt, true);
            this.kw_reviseOpt.Size = new System.Drawing.Size(264, 19);
            this.kw_reviseOpt.TabIndex = 1;
            this.kw_reviseOpt.Text = "Revisar palavras-chave antes de gerar o vídeo";
            this.kw_reviseOpt.UseVisualStyleBackColor = true;
            // 
            // kw_enableOpt
            // 
            this.kw_enableOpt.AutoSize = true;
            this.helpProvider1.SetHelpString(this.kw_enableOpt, "Habilita o componente LangAPI, que analisa o roteiro e extrai as principais palav" +
        "ras-chave nele. Estas são, então, usadas no fornescimento de imagens para o víde" +
        "o.");
            this.kw_enableOpt.Location = new System.Drawing.Point(15, 28);
            this.kw_enableOpt.Name = "kw_enableOpt";
            this.helpProvider1.SetShowHelp(this.kw_enableOpt, true);
            this.kw_enableOpt.Size = new System.Drawing.Size(280, 19);
            this.kw_enableOpt.TabIndex = 0;
            this.kw_enableOpt.Text = "Habilitar extração de palavras-chave automática";
            this.kw_enableOpt.UseVisualStyleBackColor = true;
            this.kw_enableOpt.CheckedChanged += new System.EventHandler(this.kw_enableOpt_CheckedChanged);
            // 
            // imagesGbx
            // 
            this.imagesGbx.Controls.Add(this.label7);
            this.imagesGbx.Controls.Add(this.label5);
            this.imagesGbx.Controls.Add(this.linkLabel1);
            this.imagesGbx.Controls.Add(this.pictureBox1);
            this.imagesGbx.Controls.Add(this.imgs_ccOpt);
            this.imagesGbx.Controls.Add(this.imgProviderCombo);
            this.imagesGbx.Controls.Add(this.label6);
            this.imagesGbx.Controls.Add(this.img_reviseOpt);
            this.imagesGbx.Controls.Add(this.img_enableOpt);
            this.helpProvider1.SetHelpString(this.imagesGbx, "Define as configurações de imagens no vídeo final");
            this.imagesGbx.Location = new System.Drawing.Point(10, 132);
            this.imagesGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.imagesGbx.Name = "imagesGbx";
            this.helpProvider1.SetShowHelp(this.imagesGbx, true);
            this.imagesGbx.Size = new System.Drawing.Size(349, 177);
            this.imagesGbx.TabIndex = 1;
            this.imagesGbx.TabStop = false;
            this.imagesGbx.Text = "Imagens";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(288, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 15);
            this.label7.TabIndex = 9;
            this.label7.Text = "Fonte:";
            this.label7.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(275, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 46);
            this.label5.TabIndex = 8;
            this.label5.Text = "Clique para visitar o Pixabay";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label5.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(12, 137);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(283, 38);
            this.linkLabel1.TabIndex = 7;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Imagens provenientes do Pixabay são licensiadas pela Licença Pixabay. Clique para" +
    " conferir os termos";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ExpovgenGUI.Properties.Resources.pixabay_logo_square;
            this.pictureBox1.Location = new System.Drawing.Point(277, 42);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(62, 61);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // imgs_ccOpt
            // 
            this.imgs_ccOpt.AutoSize = true;
            this.imgs_ccOpt.Location = new System.Drawing.Point(15, 118);
            this.imgs_ccOpt.Name = "imgs_ccOpt";
            this.imgs_ccOpt.Size = new System.Drawing.Size(156, 19);
            this.imgs_ccOpt.TabIndex = 4;
            this.imgs_ccOpt.Text = "Usar apenas imagens CC";
            this.imgs_ccOpt.UseVisualStyleBackColor = true;
            // 
            // imgProviderCombo
            // 
            this.imgProviderCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imgProviderCombo.FormattingEnabled = true;
            this.helpProvider1.SetHelpString(this.imgProviderCombo, "Define o serviço de busca de imagens pela internet");
            this.imgProviderCombo.Items.AddRange(new object[] {
            "Google CSE",
            "Pixabay"});
            this.imgProviderCombo.Location = new System.Drawing.Point(143, 84);
            this.imgProviderCombo.Name = "imgProviderCombo";
            this.helpProvider1.SetShowHelp(this.imgProviderCombo, true);
            this.imgProviderCombo.Size = new System.Drawing.Size(102, 23);
            this.imgProviderCombo.TabIndex = 3;
            this.imgProviderCombo.SelectedIndexChanged += new System.EventHandler(this.imgProviderCombo_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.helpProvider1.SetHelpString(this.label6, "Define o serviço de busca de imagens pela internet");
            this.label6.Location = new System.Drawing.Point(15, 87);
            this.label6.Name = "label6";
            this.helpProvider1.SetShowHelp(this.label6, true);
            this.label6.Size = new System.Drawing.Size(122, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Provedor de imagens:";
            // 
            // img_reviseOpt
            // 
            this.img_reviseOpt.AutoSize = true;
            this.helpProvider1.SetHelpString(this.img_reviseOpt, resources.GetString("img_reviseOpt.HelpString"));
            this.img_reviseOpt.Location = new System.Drawing.Point(15, 56);
            this.img_reviseOpt.Name = "img_reviseOpt";
            this.helpProvider1.SetShowHelp(this.img_reviseOpt, true);
            this.img_reviseOpt.Size = new System.Drawing.Size(230, 19);
            this.img_reviseOpt.TabIndex = 1;
            this.img_reviseOpt.Text = "Revisar imagens antes de gerar o vídeo";
            this.img_reviseOpt.UseVisualStyleBackColor = true;
            // 
            // img_enableOpt
            // 
            this.img_enableOpt.AutoSize = true;
            this.helpProvider1.SetHelpString(this.img_enableOpt, "Habilita o componente ImgFetch, que pesquisará e baixará imagens da internet auto" +
        "maticamente usando provedores de conteúdo");
            this.img_enableOpt.Location = new System.Drawing.Point(15, 31);
            this.img_enableOpt.Name = "img_enableOpt";
            this.helpProvider1.SetShowHelp(this.img_enableOpt, true);
            this.img_enableOpt.Size = new System.Drawing.Size(244, 19);
            this.img_enableOpt.TabIndex = 0;
            this.img_enableOpt.Text = "Habilitar pesquisa de imagens na internet";
            this.img_enableOpt.UseVisualStyleBackColor = true;
            this.img_enableOpt.CheckedChanged += new System.EventHandler(this.img_enableOpt_CheckedChanged);
            // 
            // speechGbx
            // 
            this.speechGbx.Controls.Add(this.autoNarrationEnableOpt);
            this.speechGbx.Location = new System.Drawing.Point(10, 319);
            this.speechGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.speechGbx.Name = "speechGbx";
            this.speechGbx.Size = new System.Drawing.Size(349, 67);
            this.speechGbx.TabIndex = 3;
            this.speechGbx.TabStop = false;
            this.speechGbx.Text = "Narração";
            // 
            // autoNarrationEnableOpt
            // 
            this.autoNarrationEnableOpt.AutoSize = true;
            this.helpProvider1.SetHelpString(this.autoNarrationEnableOpt, "Quando habilitada, esta opção habilita o componente Audioworks_TTS, que converte " +
        "o roteiro em uma narração fada por uma voz artificial");
            this.autoNarrationEnableOpt.Location = new System.Drawing.Point(15, 31);
            this.autoNarrationEnableOpt.Name = "autoNarrationEnableOpt";
            this.helpProvider1.SetShowHelp(this.autoNarrationEnableOpt, true);
            this.autoNarrationEnableOpt.Size = new System.Drawing.Size(221, 19);
            this.autoNarrationEnableOpt.TabIndex = 0;
            this.autoNarrationEnableOpt.Text = "Habilitar conversão de texto para voz";
            this.autoNarrationEnableOpt.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.flowLayoutPanel2);
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(375, 398);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Avançado";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.contentwindowGbx);
            this.flowLayoutPanel2.Controls.Add(this.pythonGbx);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(369, 392);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // contentwindowGbx
            // 
            this.contentwindowGbx.Controls.Add(this.windowStyleCombo);
            this.contentwindowGbx.Controls.Add(this.label3);
            this.contentwindowGbx.Location = new System.Drawing.Point(10, 10);
            this.contentwindowGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.contentwindowGbx.Name = "contentwindowGbx";
            this.contentwindowGbx.Size = new System.Drawing.Size(349, 68);
            this.contentwindowGbx.TabIndex = 2;
            this.contentwindowGbx.TabStop = false;
            this.contentwindowGbx.Text = "Janela de geração de conteúdo";
            // 
            // windowStyleCombo
            // 
            this.windowStyleCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.windowStyleCombo.FormattingEnabled = true;
            this.helpProvider1.SetHelpKeyword(this.windowStyleCombo, "Estilo da janela de geração de conteúdo");
            this.helpProvider1.SetHelpString(this.windowStyleCombo, resources.GetString("windowStyleCombo.HelpString"));
            this.windowStyleCombo.Items.AddRange(new object[] {
            "Resumido",
            "Detalhado"});
            this.windowStyleCombo.Location = new System.Drawing.Point(67, 27);
            this.windowStyleCombo.Name = "windowStyleCombo";
            this.helpProvider1.SetShowHelp(this.windowStyleCombo, true);
            this.windowStyleCombo.Size = new System.Drawing.Size(117, 23);
            this.windowStyleCombo.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.helpProvider1.SetHelpKeyword(this.label3, "Estilo da janela de geração de conteúdo");
            this.helpProvider1.SetHelpString(this.label3, resources.GetString("label3.HelpString"));
            this.label3.Location = new System.Drawing.Point(23, 30);
            this.label3.Name = "label3";
            this.helpProvider1.SetShowHelp(this.label3, true);
            this.label3.Size = new System.Drawing.Size(38, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Estilo:";
            // 
            // pythonGbx
            // 
            this.pythonGbx.Controls.Add(this.button1);
            this.pythonGbx.Controls.Add(this.textBox1);
            this.pythonGbx.Controls.Add(this.label14);
            this.pythonGbx.Controls.Add(this.label4);
            this.pythonGbx.Enabled = false;
            this.pythonGbx.Location = new System.Drawing.Point(10, 88);
            this.pythonGbx.Margin = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.pythonGbx.Name = "pythonGbx";
            this.pythonGbx.Size = new System.Drawing.Size(349, 144);
            this.pythonGbx.TabIndex = 3;
            this.pythonGbx.TabStop = false;
            this.pythonGbx.Text = "Interatividade com Python";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(268, 104);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "&Buscar";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(14, 104);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(244, 23);
            this.textBox1.TabIndex = 1;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label14.Location = new System.Drawing.Point(23, 28);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(308, 47);
            this.label14.TabIndex = 3;
            this.label14.Text = "ATENÇÃO: Alterar esta opção pode comprometer o funcionamento do programa. Altere " +
    "somente se necessário ou indicado pela documentação";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(218, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "Caminho do interpretador (python.exe):";
            // 
            // OKBtn
            // 
            this.OKBtn.Location = new System.Drawing.Point(235, 444);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 2;
            this.OKBtn.Text = "&OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            this.OKBtn.Click += new System.EventHandler(this.OKBtn_Click);
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(316, 444);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 3;
            this.CancelBtn.Text = "&Cancelar";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(407, 479);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OKBtn);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Propriedades";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.projtypeGbx.ResumeLayout(false);
            this.projtypeGbx.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.keywordsGbx.ResumeLayout(false);
            this.keywordsGbx.PerformLayout();
            this.imagesGbx.ResumeLayout(false);
            this.imagesGbx.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.speechGbx.ResumeLayout(false);
            this.speechGbx.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.contentwindowGbx.ResumeLayout(false);
            this.contentwindowGbx.PerformLayout();
            this.pythonGbx.ResumeLayout(false);
            this.pythonGbx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private TabControl tabControl1;
        private Button OKBtn;
        private Button CancelBtn;
        private TabPage tabPage1;
        private FlowLayoutPanel flowLayoutPanel1;
        private GroupBox projtypeGbx;
        private Label label2;
        private RadioButton apresentacaoOpt;
        private RadioButton videoaulaOpt;
        private Label label1;
        private TabPage tabPage2;
        private HelpProvider helpProvider1;
        private TabPage tabPage3;
        private FlowLayoutPanel flowLayoutPanel2;
        private GroupBox contentwindowGbx;
        private ComboBox windowStyleCombo;
        private Label label3;
        private GroupBox pythonGbx;
        private Button button1;
        private TextBox textBox1;
        private Label label4;
        private FlowLayoutPanel flowLayoutPanel3;
        private GroupBox imagesGbx;
        private CheckBox img_reviseOpt;
        private CheckBox img_enableOpt;
        private GroupBox keywordsGbx;
        private CheckBox kw_reviseOpt;
        private CheckBox kw_enableOpt;
        private ComboBox imgProviderCombo;
        private Label label6;
        private GroupBox speechGbx;
        private CheckBox autoNarrationEnableOpt;
        private Label label14;
        private CheckBox imgs_ccOpt;
        private CheckBox kw_ShowOnImagesOpt;
        private Label label7;
        private Label label5;
        private LinkLabel linkLabel1;
        private PictureBox pictureBox1;
    }
}