namespace ASY
{
    partial class mainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаПараметровDevManToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаПараметровОтАСУToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.настройкаTcpСервераToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкаСоединенияСDevManToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelDevManStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this._labelRashod = new System.Windows.Forms.Label();
            this._labelTalblok = new System.Windows.Forms.Label();
            this._labelPodasa = new System.Windows.Forms.Label();
            this._labeObem = new System.Windows.Forms.Label();
            this._textBoxDiameterPorsna2 = new System.Windows.Forms.TextBox();
            this._textBoxDiameterPorsna1 = new System.Windows.Forms.TextBox();
            this._labelNagruzka = new System.Windows.Forms.Label();
            this._textBoxKlinia = new System.Windows.Forms.TextBox();
            this._textBoxMomentDvigRotor = new System.Windows.Forms.TextBox();
            this._textBoxSkorostRotor = new System.Windows.Forms.TextBox();
            this._textBoxSkorostDvig2 = new System.Windows.Forms.TextBox();
            this._textBoxSkorostDvig1 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.настройкиToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(430, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкаПараметровDevManToolStripMenuItem,
            this.настройкаПараметровОтАСУToolStripMenuItem,
            this.toolStripMenuItem1,
            this.настройкаTcpСервераToolStripMenuItem,
            this.настройкаСоединенияСDevManToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // настройкаПараметровDevManToolStripMenuItem
            // 
            this.настройкаПараметровDevManToolStripMenuItem.Name = "настройкаПараметровDevManToolStripMenuItem";
            this.настройкаПараметровDevManToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.настройкаПараметровDevManToolStripMenuItem.Text = "Настройка параметров devMan";
            this.настройкаПараметровDevManToolStripMenuItem.Click += new System.EventHandler(this.настройкаПараметровDevManToolStripMenuItem_Click);
            // 
            // настройкаПараметровОтАСУToolStripMenuItem
            // 
            this.настройкаПараметровОтАСУToolStripMenuItem.Name = "настройкаПараметровОтАСУToolStripMenuItem";
            this.настройкаПараметровОтАСУToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.настройкаПараметровОтАСУToolStripMenuItem.Text = "Настройка параметров от АСУ";
            this.настройкаПараметровОтАСУToolStripMenuItem.Click += new System.EventHandler(this.настройкаПараметровОтАСУToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(253, 6);
            // 
            // настройкаTcpСервераToolStripMenuItem
            // 
            this.настройкаTcpСервераToolStripMenuItem.Name = "настройкаTcpСервераToolStripMenuItem";
            this.настройкаTcpСервераToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.настройкаTcpСервераToolStripMenuItem.Text = "Настройка Tcp сервера";
            this.настройкаTcpСервераToolStripMenuItem.Click += new System.EventHandler(this.настройкаTcpСервераToolStripMenuItem_Click);
            // 
            // настройкаСоединенияСDevManToolStripMenuItem
            // 
            this.настройкаСоединенияСDevManToolStripMenuItem.Name = "настройкаСоединенияСDevManToolStripMenuItem";
            this.настройкаСоединенияСDevManToolStripMenuItem.Size = new System.Drawing.Size(256, 22);
            this.настройкаСоединенияСDevManToolStripMenuItem.Text = "Настройка соединения с devMan";
            this.настройкаСоединенияСDevManToolStripMenuItem.Click += new System.EventHandler(this.настройкаСоединенияСDevManToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelDevManStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 247);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(430, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabelDevManStatus
            // 
            this.toolStripStatusLabelDevManStatus.Name = "toolStripStatusLabelDevManStatus";
            this.toolStripStatusLabelDevManStatus.Size = new System.Drawing.Size(187, 17);
            this.toolStripStatusLabelDevManStatus.Text = "Не подключен с серверу данных";
            // 
            // _labelRashod
            // 
            this._labelRashod.BackColor = System.Drawing.Color.Salmon;
            this._labelRashod.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labelRashod.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labelRashod.Location = new System.Drawing.Point(301, 62);
            this._labelRashod.Name = "_labelRashod";
            this._labelRashod.Size = new System.Drawing.Size(100, 23);
            this._labelRashod.TabIndex = 36;
            this._labelRashod.Text = "Расход";
            this._labelRashod.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _labelTalblok
            // 
            this._labelTalblok.BackColor = System.Drawing.Color.Salmon;
            this._labelTalblok.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labelTalblok.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labelTalblok.Location = new System.Drawing.Point(301, 88);
            this._labelTalblok.Name = "_labelTalblok";
            this._labelTalblok.Size = new System.Drawing.Size(100, 23);
            this._labelTalblok.TabIndex = 35;
            this._labelTalblok.Text = "Тальблок";
            this._labelTalblok.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _labelPodasa
            // 
            this._labelPodasa.BackColor = System.Drawing.Color.Salmon;
            this._labelPodasa.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labelPodasa.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labelPodasa.Location = new System.Drawing.Point(301, 112);
            this._labelPodasa.Name = "_labelPodasa";
            this._labelPodasa.Size = new System.Drawing.Size(100, 23);
            this._labelPodasa.TabIndex = 34;
            this._labelPodasa.Text = "Подача";
            this._labelPodasa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _labeObem
            // 
            this._labeObem.BackColor = System.Drawing.Color.Salmon;
            this._labeObem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labeObem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labeObem.Location = new System.Drawing.Point(301, 138);
            this._labeObem.Name = "_labeObem";
            this._labeObem.Size = new System.Drawing.Size(100, 23);
            this._labeObem.TabIndex = 33;
            this._labeObem.Text = "Объем";
            this._labeObem.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _textBoxDiameterPorsna2
            // 
            this._textBoxDiameterPorsna2.Location = new System.Drawing.Point(185, 190);
            this._textBoxDiameterPorsna2.Name = "_textBoxDiameterPorsna2";
            this._textBoxDiameterPorsna2.ReadOnly = true;
            this._textBoxDiameterPorsna2.Size = new System.Drawing.Size(100, 20);
            this._textBoxDiameterPorsna2.TabIndex = 32;
            this._textBoxDiameterPorsna2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxDiameterPorsna1
            // 
            this._textBoxDiameterPorsna1.Location = new System.Drawing.Point(185, 164);
            this._textBoxDiameterPorsna1.Name = "_textBoxDiameterPorsna1";
            this._textBoxDiameterPorsna1.ReadOnly = true;
            this._textBoxDiameterPorsna1.Size = new System.Drawing.Size(100, 20);
            this._textBoxDiameterPorsna1.TabIndex = 31;
            this._textBoxDiameterPorsna1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _labelNagruzka
            // 
            this._labelNagruzka.BackColor = System.Drawing.Color.Salmon;
            this._labelNagruzka.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._labelNagruzka.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._labelNagruzka.Location = new System.Drawing.Point(301, 36);
            this._labelNagruzka.Name = "_labelNagruzka";
            this._labelNagruzka.Size = new System.Drawing.Size(100, 23);
            this._labelNagruzka.TabIndex = 23;
            this._labelNagruzka.Text = "Нагрузка";
            this._labelNagruzka.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _textBoxKlinia
            // 
            this._textBoxKlinia.Location = new System.Drawing.Point(185, 138);
            this._textBoxKlinia.Name = "_textBoxKlinia";
            this._textBoxKlinia.ReadOnly = true;
            this._textBoxKlinia.Size = new System.Drawing.Size(100, 20);
            this._textBoxKlinia.TabIndex = 30;
            this._textBoxKlinia.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxMomentDvigRotor
            // 
            this._textBoxMomentDvigRotor.Location = new System.Drawing.Point(185, 112);
            this._textBoxMomentDvigRotor.Name = "_textBoxMomentDvigRotor";
            this._textBoxMomentDvigRotor.ReadOnly = true;
            this._textBoxMomentDvigRotor.Size = new System.Drawing.Size(100, 20);
            this._textBoxMomentDvigRotor.TabIndex = 28;
            this._textBoxMomentDvigRotor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxSkorostRotor
            // 
            this._textBoxSkorostRotor.Location = new System.Drawing.Point(185, 88);
            this._textBoxSkorostRotor.Name = "_textBoxSkorostRotor";
            this._textBoxSkorostRotor.ReadOnly = true;
            this._textBoxSkorostRotor.Size = new System.Drawing.Size(100, 20);
            this._textBoxSkorostRotor.TabIndex = 25;
            this._textBoxSkorostRotor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxSkorostDvig2
            // 
            this._textBoxSkorostDvig2.Location = new System.Drawing.Point(185, 62);
            this._textBoxSkorostDvig2.Name = "_textBoxSkorostDvig2";
            this._textBoxSkorostDvig2.ReadOnly = true;
            this._textBoxSkorostDvig2.Size = new System.Drawing.Size(100, 20);
            this._textBoxSkorostDvig2.TabIndex = 22;
            this._textBoxSkorostDvig2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // _textBoxSkorostDvig1
            // 
            this._textBoxSkorostDvig1.Location = new System.Drawing.Point(185, 36);
            this._textBoxSkorostDvig1.Name = "_textBoxSkorostDvig1";
            this._textBoxSkorostDvig1.ReadOnly = true;
            this._textBoxSkorostDvig1.Size = new System.Drawing.Size(100, 20);
            this._textBoxSkorostDvig1.TabIndex = 20;
            this._textBoxSkorostDvig1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 193);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(142, 13);
            this.label7.TabIndex = 29;
            this.label7.Text = "Диаметр поршня насоса 2";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 13);
            this.label6.TabIndex = 27;
            this.label6.Text = "Диаметр поршня насоса 1";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 141);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 26;
            this.label5.Text = "Клинья подняты/опущены";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Момент двигателя ротора";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Скорость двигателя ротора";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "Скорость двигателя насоса 2";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(158, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Скорость двигателя насоса 1";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 219);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(157, 13);
            this.label8.TabIndex = 37;
            this.label8.Text = "Усилие в гидрораскрепителе";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(185, 216);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 38;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // mainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 269);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._labelRashod);
            this.Controls.Add(this._labelTalblok);
            this.Controls.Add(this._labelPodasa);
            this.Controls.Add(this._labeObem);
            this.Controls.Add(this._textBoxDiameterPorsna2);
            this.Controls.Add(this._textBoxDiameterPorsna1);
            this.Controls.Add(this._labelNagruzka);
            this.Controls.Add(this._textBoxKlinia);
            this.Controls.Add(this._textBoxMomentDvigRotor);
            this.Controls.Add(this._textBoxSkorostRotor);
            this.Controls.Add(this._textBoxSkorostDvig2);
            this.Controls.Add(this._textBoxSkorostDvig1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "mainForm";
            this.Text = "Модуль связи с АСУ Буровой";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelDevManStatus;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкаПараметровDevManToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкаПараметровОтАСУToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem настройкаTcpСервераToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкаСоединенияСDevManToolStripMenuItem;
        private System.Windows.Forms.Label _labelRashod;
        private System.Windows.Forms.Label _labelTalblok;
        private System.Windows.Forms.Label _labelPodasa;
        private System.Windows.Forms.Label _labeObem;
        private System.Windows.Forms.TextBox _textBoxDiameterPorsna2;
        private System.Windows.Forms.TextBox _textBoxDiameterPorsna1;
        private System.Windows.Forms.Label _labelNagruzka;
        private System.Windows.Forms.TextBox _textBoxKlinia;
        private System.Windows.Forms.TextBox _textBoxMomentDvigRotor;
        private System.Windows.Forms.TextBox _textBoxSkorostRotor;
        private System.Windows.Forms.TextBox _textBoxSkorostDvig2;
        private System.Windows.Forms.TextBox _textBoxSkorostDvig1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;
    }
}