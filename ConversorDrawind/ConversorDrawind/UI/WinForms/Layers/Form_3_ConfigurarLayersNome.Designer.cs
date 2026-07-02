namespace ConversorDrawind
{
    partial class Form_3_ConfigurarLayersNome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_3_ConfigurarLayersNome));
            this.NLCLNome = new System.Windows.Forms.TextBox();
            this.NLCBContinuar = new System.Windows.Forms.Button();
            this.NLCBCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NLCLNome
            // 
            this.NLCLNome.Location = new System.Drawing.Point(12, 12);
            this.NLCLNome.Name = "NLCLNome";
            this.NLCLNome.Size = new System.Drawing.Size(260, 20);
            this.NLCLNome.TabIndex = 1;
            // 
            // NLCBContinuar
            // 
            this.NLCBContinuar.Location = new System.Drawing.Point(12, 43);
            this.NLCBContinuar.Name = "NLCBContinuar";
            this.NLCBContinuar.Size = new System.Drawing.Size(116, 23);
            this.NLCBContinuar.TabIndex = 0;
            this.NLCBContinuar.Text = "Continuar";
            this.NLCBContinuar.UseVisualStyleBackColor = true;
            this.NLCBContinuar.Click += new System.EventHandler(this.NLCBContinuar_Click);
            // 
            // NLCBCancelar
            // 
            this.NLCBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NLCBCancelar.Location = new System.Drawing.Point(156, 43);
            this.NLCBCancelar.Name = "NLCBCancelar";
            this.NLCBCancelar.Size = new System.Drawing.Size(116, 23);
            this.NLCBCancelar.TabIndex = 2;
            this.NLCBCancelar.Text = "Cancelar";
            this.NLCBCancelar.UseVisualStyleBackColor = true;
            this.NLCBCancelar.Click += new System.EventHandler(this.NLCBCancelar_Click);
            // 
            // Form_3_ConfigurarLayersNome
            // 
            this.AcceptButton = this.NLCBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.NLCBCancelar;
            this.ClientSize = new System.Drawing.Size(284, 72);
            this.Controls.Add(this.NLCBCancelar);
            this.Controls.Add(this.NLCBContinuar);
            this.Controls.Add(this.NLCLNome);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_3_ConfigurarLayersNome";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nome";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox NLCLNome;
        private System.Windows.Forms.Button NLCBContinuar;
        private System.Windows.Forms.Button NLCBCancelar;
    }
}