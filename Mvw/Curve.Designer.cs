namespace MBusRead
{
    partial class CurveForm
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
            this.components = new System.ComponentModel.Container();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textMadaMaxNiuju = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.textBengMaxNiuju = new System.Windows.Forms.TextBox();
            this.textMadaNiuJuPL0 = new System.Windows.Forms.TextBox();
            this.label63 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.textBengNiuJuPL0 = new System.Windows.Forms.TextBox();
            this.BengTest = new System.Windows.Forms.Label();
            this.MadaTest = new System.Windows.Forms.Label();
            this.labelkp = new System.Windows.Forms.Label();
            this.pidKkk = new System.Windows.Forms.Label();
            this.pidPeriod = new System.Windows.Forms.Label();
            this.pidTarget = new System.Windows.Forms.Label();
            this.labelki = new System.Windows.Forms.Label();
            this.labelkd = new System.Windows.Forms.Label();
            this.checkPause = new System.Windows.Forms.CheckBox();
            this.checkSursor = new System.Windows.Forms.CheckBox();
            this.checkShuiping = new System.Windows.Forms.CheckBox();
            this.label1Inc = new System.Windows.Forms.Label();
            this.labelPidCnt = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.ForeColor = System.Drawing.Color.Red;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(792, 45);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(82, 22);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.Tag = "0";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox2.ForeColor = System.Drawing.Color.LimeGreen;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(792, 84);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(82, 22);
            this.comboBox2.TabIndex = 2;
            this.comboBox2.Tag = "1";
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox3.ForeColor = System.Drawing.Color.Fuchsia;
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(792, 123);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(82, 22);
            this.comboBox3.TabIndex = 2;
            this.comboBox3.Tag = "2";
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox4
            // 
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox4.ForeColor = System.Drawing.Color.RoyalBlue;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Location = new System.Drawing.Point(792, 162);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(82, 22);
            this.comboBox4.TabIndex = 2;
            this.comboBox4.Tag = "3";
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(878, 267);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(48, 16);
            this.checkBox1.TabIndex = 5;
            this.checkBox1.Text = "测试";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.button2.Location = new System.Drawing.Point(792, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 22);
            this.button2.TabIndex = 7;
            this.button2.Text = "保存曲线";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(1036, 267);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "选择文件夹";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Checked = true;
            this.chkAutoSave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoSave.Location = new System.Drawing.Point(792, 196);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(72, 16);
            this.chkAutoSave.TabIndex = 9;
            this.chkAutoSave.Text = "自动保存";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 14;
            this.listBox1.Location = new System.Drawing.Point(979, 5);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(161, 256);
            this.listBox1.TabIndex = 37;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // textMadaMaxNiuju
            // 
            this.textMadaMaxNiuju.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textMadaMaxNiuju.Location = new System.Drawing.Point(1069, 383);
            this.textMadaMaxNiuju.MaxLength = 5;
            this.textMadaMaxNiuju.Name = "textMadaMaxNiuju";
            this.textMadaMaxNiuju.Size = new System.Drawing.Size(64, 21);
            this.textMadaMaxNiuju.TabIndex = 89;
            this.textMadaMaxNiuju.Text = "32";
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(1013, 386);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(53, 12);
            this.label50.TabIndex = 91;
            this.label50.Text = "最大扭矩";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(1013, 354);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(53, 12);
            this.label64.TabIndex = 92;
            this.label64.Text = "最大扭矩";
            // 
            // textBengMaxNiuju
            // 
            this.textBengMaxNiuju.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBengMaxNiuju.Location = new System.Drawing.Point(1069, 349);
            this.textBengMaxNiuju.MaxLength = 5;
            this.textBengMaxNiuju.Name = "textBengMaxNiuju";
            this.textBengMaxNiuju.Size = new System.Drawing.Size(64, 21);
            this.textBengMaxNiuju.TabIndex = 90;
            this.textBengMaxNiuju.Text = "24";
            // 
            // textMadaNiuJuPL0
            // 
            this.textMadaNiuJuPL0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textMadaNiuJuPL0.Location = new System.Drawing.Point(942, 383);
            this.textMadaNiuJuPL0.MaxLength = 5;
            this.textMadaNiuJuPL0.Name = "textMadaNiuJuPL0";
            this.textMadaNiuJuPL0.Size = new System.Drawing.Size(64, 21);
            this.textMadaNiuJuPL0.TabIndex = 83;
            this.textMadaNiuJuPL0.Text = "32";
            // 
            // label63
            // 
            this.label63.AutoSize = true;
            this.label63.Location = new System.Drawing.Point(870, 386);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(71, 12);
            this.label63.TabIndex = 85;
            this.label63.Text = "马达 零位值";
            // 
            // label62
            // 
            this.label62.AutoSize = true;
            this.label62.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label62.Location = new System.Drawing.Point(869, 354);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(70, 14);
            this.label62.TabIndex = 86;
            this.label62.Text = "泵 零位值";
            // 
            // textBengNiuJuPL0
            // 
            this.textBengNiuJuPL0.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBengNiuJuPL0.Location = new System.Drawing.Point(942, 349);
            this.textBengNiuJuPL0.MaxLength = 5;
            this.textBengNiuJuPL0.Name = "textBengNiuJuPL0";
            this.textBengNiuJuPL0.Size = new System.Drawing.Size(64, 21);
            this.textBengNiuJuPL0.TabIndex = 84;
            this.textBengNiuJuPL0.Text = "24";
            // 
            // BengTest
            // 
            this.BengTest.AutoSize = true;
            this.BengTest.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BengTest.Location = new System.Drawing.Point(893, 291);
            this.BengTest.Name = "BengTest";
            this.BengTest.Size = new System.Drawing.Size(48, 16);
            this.BengTest.TabIndex = 93;
            this.BengTest.Text = "-----";
            // 
            // MadaTest
            // 
            this.MadaTest.AutoSize = true;
            this.MadaTest.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MadaTest.Location = new System.Drawing.Point(891, 323);
            this.MadaTest.Name = "MadaTest";
            this.MadaTest.Size = new System.Drawing.Size(48, 16);
            this.MadaTest.TabIndex = 93;
            this.MadaTest.Text = "-----";
            // 
            // labelkp
            // 
            this.labelkp.AutoSize = true;
            this.labelkp.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelkp.Location = new System.Drawing.Point(1083, 291);
            this.labelkp.Name = "labelkp";
            this.labelkp.Size = new System.Drawing.Size(48, 16);
            this.labelkp.TabIndex = 93;
            this.labelkp.Text = "-----";
            // 
            // pidKkk
            // 
            this.pidKkk.AutoSize = true;
            this.pidKkk.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidKkk.Location = new System.Drawing.Point(1010, 324);
            this.pidKkk.Name = "pidKkk";
            this.pidKkk.Size = new System.Drawing.Size(48, 16);
            this.pidKkk.TabIndex = 93;
            this.pidKkk.Text = "-----";
            // 
            // pidPeriod
            // 
            this.pidPeriod.AutoSize = true;
            this.pidPeriod.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidPeriod.Location = new System.Drawing.Point(1010, 308);
            this.pidPeriod.Name = "pidPeriod";
            this.pidPeriod.Size = new System.Drawing.Size(48, 16);
            this.pidPeriod.TabIndex = 93;
            this.pidPeriod.Text = "-----";
            // 
            // pidTarget
            // 
            this.pidTarget.AutoSize = true;
            this.pidTarget.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.pidTarget.Location = new System.Drawing.Point(1010, 291);
            this.pidTarget.Name = "pidTarget";
            this.pidTarget.Size = new System.Drawing.Size(48, 16);
            this.pidTarget.TabIndex = 93;
            this.pidTarget.Text = "-----";
            // 
            // labelki
            // 
            this.labelki.AutoSize = true;
            this.labelki.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelki.Location = new System.Drawing.Point(1083, 307);
            this.labelki.Name = "labelki";
            this.labelki.Size = new System.Drawing.Size(48, 16);
            this.labelki.TabIndex = 93;
            this.labelki.Text = "-----";
            // 
            // labelkd
            // 
            this.labelkd.AutoSize = true;
            this.labelkd.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelkd.Location = new System.Drawing.Point(1083, 323);
            this.labelkd.Name = "labelkd";
            this.labelkd.Size = new System.Drawing.Size(48, 16);
            this.labelkd.TabIndex = 93;
            this.labelkd.Text = "-----";
            // 
            // checkPause
            // 
            this.checkPause.AutoSize = true;
            this.checkPause.Location = new System.Drawing.Point(878, 198);
            this.checkPause.Name = "checkPause";
            this.checkPause.Size = new System.Drawing.Size(72, 16);
            this.checkPause.TabIndex = 94;
            this.checkPause.Text = "暂停刷新";
            this.checkPause.UseVisualStyleBackColor = true;
            // 
            // checkSursor
            // 
            this.checkSursor.AutoSize = true;
            this.checkSursor.Location = new System.Drawing.Point(878, 220);
            this.checkSursor.Name = "checkSursor";
            this.checkSursor.Size = new System.Drawing.Size(72, 16);
            this.checkSursor.TabIndex = 94;
            this.checkSursor.Text = "跟随显示";
            this.checkSursor.UseVisualStyleBackColor = true;
            // 
            // checkShuiping
            // 
            this.checkShuiping.AutoSize = true;
            this.checkShuiping.Location = new System.Drawing.Point(878, 244);
            this.checkShuiping.Name = "checkShuiping";
            this.checkShuiping.Size = new System.Drawing.Size(72, 16);
            this.checkShuiping.TabIndex = 94;
            this.checkShuiping.Text = "水平标线";
            this.checkShuiping.UseVisualStyleBackColor = true;
            // 
            // label1Inc
            // 
            this.label1Inc.AutoSize = true;
            this.label1Inc.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1Inc.Location = new System.Drawing.Point(956, 291);
            this.label1Inc.Name = "label1Inc";
            this.label1Inc.Size = new System.Drawing.Size(48, 16);
            this.label1Inc.TabIndex = 93;
            this.label1Inc.Text = "-----";
            // 
            // labelPidCnt
            // 
            this.labelPidCnt.AutoSize = true;
            this.labelPidCnt.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labelPidCnt.Location = new System.Drawing.Point(956, 323);
            this.labelPidCnt.Name = "labelPidCnt";
            this.labelPidCnt.Size = new System.Drawing.Size(48, 16);
            this.labelPidCnt.TabIndex = 93;
            this.labelPidCnt.Text = "-----";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(760, 200);
            this.pictureBox1.TabIndex = 95;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.Location = new System.Drawing.Point(4, 231);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(480, 170);
            this.pictureBox2.TabIndex = 97;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            // 
            // pictureBox3
            // 
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Location = new System.Drawing.Point(490, 231);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(340, 170);
            this.pictureBox3.TabIndex = 98;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox3_Paint);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(878, 5);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(95, 185);
            this.pictureBox4.TabIndex = 99;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox4_Paint);
            // 
            // CurveForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1154, 411);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkShuiping);
            this.Controls.Add(this.checkSursor);
            this.Controls.Add(this.checkPause);
            this.Controls.Add(this.pidTarget);
            this.Controls.Add(this.pidPeriod);
            this.Controls.Add(this.pidKkk);
            this.Controls.Add(this.labelkd);
            this.Controls.Add(this.labelki);
            this.Controls.Add(this.labelkp);
            this.Controls.Add(this.labelPidCnt);
            this.Controls.Add(this.label1Inc);
            this.Controls.Add(this.MadaTest);
            this.Controls.Add(this.BengTest);
            this.Controls.Add(this.textMadaMaxNiuju);
            this.Controls.Add(this.label50);
            this.Controls.Add(this.label64);
            this.Controls.Add(this.textBengMaxNiuju);
            this.Controls.Add(this.textMadaNiuJuPL0);
            this.Controls.Add(this.label63);
            this.Controls.Add(this.label62);
            this.Controls.Add(this.textBengNiuJuPL0);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.chkAutoSave);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "CurveForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "记录";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CurveForm_FormClosing);
            this.Load += new System.EventHandler(this.CurveForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.TextBox textBengMaxNiuju;
        public System.Windows.Forms.TextBox textMadaNiuJuPL0;
        public System.Windows.Forms.TextBox textMadaMaxNiuju;
        public System.Windows.Forms.TextBox textBengNiuJuPL0;
        public System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.Label label64;
        private System.Windows.Forms.Label label63;
        private System.Windows.Forms.Label label62;
        private System.Windows.Forms.Label BengTest;
        private System.Windows.Forms.Label MadaTest;
        private System.Windows.Forms.Label labelkp;
        private System.Windows.Forms.Label pidKkk;
        private System.Windows.Forms.Label pidPeriod;
        private System.Windows.Forms.Label pidTarget;
        private System.Windows.Forms.Label labelki;
        private System.Windows.Forms.Label labelkd;
        private System.Windows.Forms.CheckBox checkPause;
        private System.Windows.Forms.CheckBox checkSursor;
        private System.Windows.Forms.CheckBox checkShuiping;
        private System.Windows.Forms.Label label1Inc;
        private System.Windows.Forms.Label labelPidCnt;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
    }
}