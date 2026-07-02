namespace ConversorDrawind
{
    partial class Form_8_GenericNewColor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_8_GenericNewColor));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tBRed = new System.Windows.Forms.TextBox();
            this.tBGreen = new System.Windows.Forms.TextBox();
            this.tbBlue = new System.Windows.Forms.TextBox();
            this.tbColor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.bContinuar = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Red:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Green:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Blue:";
            // 
            // tBRed
            // 
            this.tBRed.Location = new System.Drawing.Point(49, 20);
            this.tBRed.Name = "tBRed";
            this.tBRed.Size = new System.Drawing.Size(100, 20);
            this.tBRed.TabIndex = 1;
            this.tBRed.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tBRed_MouseClick);
            this.tBRed.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tBRed_KeyPress);
            // 
            // tBGreen
            // 
            this.tBGreen.Location = new System.Drawing.Point(49, 46);
            this.tBGreen.Name = "tBGreen";
            this.tBGreen.Size = new System.Drawing.Size(100, 20);
            this.tBGreen.TabIndex = 2;
            this.tBGreen.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tBGreen_MouseClick);
            this.tBGreen.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tBGreen_KeyPress);
            // 
            // tbBlue
            // 
            this.tbBlue.Location = new System.Drawing.Point(49, 73);
            this.tbBlue.Name = "tbBlue";
            this.tbBlue.Size = new System.Drawing.Size(100, 20);
            this.tbBlue.TabIndex = 3;
            this.tbBlue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbBlue_MouseClick);
            this.tbBlue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbBlue_KeyPress);
            // 
            // tbColor
            // 
            this.tbColor.Location = new System.Drawing.Point(49, 99);
            this.tbColor.Name = "tbColor";
            this.tbColor.Size = new System.Drawing.Size(200, 20);
            this.tbColor.TabIndex = 4;
            this.tbColor.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tbColor_MouseClick);
            this.tbColor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbColor_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 102);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Color:";
            // 
            // bContinuar
            // 
            this.bContinuar.Location = new System.Drawing.Point(11, 136);
            this.bContinuar.Name = "bContinuar";
            this.bContinuar.Size = new System.Drawing.Size(116, 23);
            this.bContinuar.TabIndex = 0;
            this.bContinuar.Text = "Continuar";
            this.bContinuar.UseVisualStyleBackColor = true;
            this.bContinuar.Click += new System.EventHandler(this.bContinuar_Click);
            // 
            // bCancelar
            // 
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Location = new System.Drawing.Point(133, 136);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(116, 23);
            this.bCancelar.TabIndex = 5;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // Form_8_GenericNewColor
            // 
            this.AcceptButton = this.bContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(263, 171);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bContinuar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbColor);
            this.Controls.Add(this.tbBlue);
            this.Controls.Add(this.tBGreen);
            this.Controls.Add(this.tBRed);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_8_GenericNewColor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cor RGB";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConfColor_FormClosed);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConfColor_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tBRed;
        private System.Windows.Forms.TextBox tBGreen;
        private System.Windows.Forms.TextBox tbBlue;
        private System.Windows.Forms.TextBox tbColor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button bContinuar;
        private System.Windows.Forms.Button bCancelar;
    }
}