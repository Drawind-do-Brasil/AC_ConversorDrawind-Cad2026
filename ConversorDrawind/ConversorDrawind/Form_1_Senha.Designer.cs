namespace ConversorDrawind
{
    partial class Form_1_Senha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_1_Senha));
            this.label1 = new System.Windows.Forms.Label();
            this.tBNumberSerie = new System.Windows.Forms.TextBox();
            this.ativar = new System.Windows.Forms.Button();
            this.cancelar = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Número de Serie:";
            // 
            // tBNumberSerie
            // 
            this.tBNumberSerie.Location = new System.Drawing.Point(139, 125);
            this.tBNumberSerie.Name = "tBNumberSerie";
            this.tBNumberSerie.Size = new System.Drawing.Size(356, 20);
            this.tBNumberSerie.TabIndex = 1;
            // 
            // ativar
            // 
            this.ativar.Location = new System.Drawing.Point(420, 151);
            this.ativar.Name = "ativar";
            this.ativar.Size = new System.Drawing.Size(75, 23);
            this.ativar.TabIndex = 0;
            this.ativar.Text = "Ativar";
            this.ativar.UseVisualStyleBackColor = true;
            this.ativar.Click += new System.EventHandler(this.ativar_Click);
            // 
            // cancelar
            // 
            this.cancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelar.Location = new System.Drawing.Point(339, 151);
            this.cancelar.Name = "cancelar";
            this.cancelar.Size = new System.Drawing.Size(75, 23);
            this.cancelar.TabIndex = 2;
            this.cancelar.Text = "Cancelar";
            this.cancelar.UseVisualStyleBackColor = true;
            this.cancelar.Click += new System.EventHandler(this.cancelar_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::ConversorDrawind.Properties.Resources.DRAWIND_PEQUENA;
            this.pictureBox1.Location = new System.Drawing.Point(157, 29);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(193, 61);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Form_1_Senha
            // 
            this.AcceptButton = this.ativar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.cancelar;
            this.ClientSize = new System.Drawing.Size(507, 186);
            this.Controls.Add(this.cancelar);
            this.Controls.Add(this.ativar);
            this.Controls.Add(this.tBNumberSerie);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form_1_Senha";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ativação";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Senha_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBNumberSerie;
        private System.Windows.Forms.Button ativar;
        private System.Windows.Forms.Button cancelar;
    }
}