namespace ConversorDrawind
{
    partial class Form_2_Processo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_2_Processo));
            this.ProgressBar1 = new System.Windows.Forms.ProgressBar();
            this.Button_Cancel = new System.Windows.Forms.Button();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Label_TempoRestante = new System.Windows.Forms.Label();
            this.Label_TempDecorrido = new System.Windows.Forms.Label();
            this.Label_Processo = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Location = new System.Drawing.Point(12, 191);
            this.ProgressBar1.MarqueeAnimationSpeed = 50;
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(520, 34);
            this.ProgressBar1.TabIndex = 0;
            // 
            // Button_Cancel
            // 
            this.Button_Cancel.Enabled = false;
            this.Button_Cancel.Location = new System.Drawing.Point(420, 277);
            this.Button_Cancel.Name = "Button_Cancel";
            this.Button_Cancel.Size = new System.Drawing.Size(112, 31);
            this.Button_Cancel.TabIndex = 1;
            this.Button_Cancel.Text = "Cancelar";
            this.Button_Cancel.UseVisualStyleBackColor = true;
            this.Button_Cancel.Click += new System.EventHandler(this.Button_Cancel_Click);
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.WorkerReportsProgress = true;
            this.BackgroundWorker1.WorkerSupportsCancellation = true;
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.BackgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.BackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConversorDrawind.Properties.Resources.DRAWIND_PEQUENA;
            this.pictureBox1.Location = new System.Drawing.Point(128, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(288, 115);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // Label_TempoRestante
            // 
            this.Label_TempoRestante.AutoSize = true;
            this.Label_TempoRestante.Location = new System.Drawing.Point(247, 228);
            this.Label_TempoRestante.Name = "Label_TempoRestante";
            this.Label_TempoRestante.Size = new System.Drawing.Size(16, 13);
            this.Label_TempoRestante.TabIndex = 3;
            this.Label_TempoRestante.Text = "...";
            // 
            // Label_TempDecorrido
            // 
            this.Label_TempDecorrido.AutoSize = true;
            this.Label_TempDecorrido.Location = new System.Drawing.Point(12, 228);
            this.Label_TempDecorrido.Name = "Label_TempDecorrido";
            this.Label_TempDecorrido.Size = new System.Drawing.Size(16, 13);
            this.Label_TempDecorrido.TabIndex = 4;
            this.Label_TempDecorrido.Text = "...";
            // 
            // Label_Processo
            // 
            this.Label_Processo.AutoSize = true;
            this.Label_Processo.Location = new System.Drawing.Point(12, 251);
            this.Label_Processo.Name = "Label_Processo";
            this.Label_Processo.Size = new System.Drawing.Size(138, 13);
            this.Label_Processo.TabIndex = 5;
            this.Label_Processo.Text = "Iniciando o Autocad 2026...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label1.Location = new System.Drawing.Point(156, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(233, 42);
            this.label1.TabIndex = 6;
            this.label1.Text = "Convertendo";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(9, 298);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = global::ConversorDrawind.Properties.Settings.Default.Versao;
            // 
            // Form_2_Processo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(544, 320);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Label_Processo);
            this.Controls.Add(this.Label_TempDecorrido);
            this.Controls.Add(this.Label_TempoRestante);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Button_Cancel);
            this.Controls.Add(this.ProgressBar1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_2_Processo";
            this.Opacity = 0.7D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Processando";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar ProgressBar1;
        private System.Windows.Forms.Button Button_Cancel;
        private System.ComponentModel.BackgroundWorker BackgroundWorker1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label Label_TempoRestante;
        private System.Windows.Forms.Label Label_TempDecorrido;
        private System.Windows.Forms.Label Label_Processo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}