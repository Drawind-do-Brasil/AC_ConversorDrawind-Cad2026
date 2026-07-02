namespace ConversorDrawind
{
    partial class Form_3_ConfigurarLayersLinha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_3_ConfigurarLayersLinha));
            this.NLCCBLinhas = new System.Windows.Forms.ComboBox();
            this.NLCBContinuar = new System.Windows.Forms.Button();
            this.NLCBCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NLCCBLinhas
            // 
            this.NLCCBLinhas.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.NLCCBLinhas.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.NLCCBLinhas.FormattingEnabled = true;
            this.NLCCBLinhas.Location = new System.Drawing.Point(12, 12);
            this.NLCCBLinhas.Name = "NLCCBLinhas";
            this.NLCCBLinhas.Size = new System.Drawing.Size(381, 21);
            this.NLCCBLinhas.TabIndex = 1;
            // 
            // NLCBContinuar
            // 
            this.NLCBContinuar.Location = new System.Drawing.Point(134, 39);
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
            this.NLCBCancelar.Location = new System.Drawing.Point(277, 39);
            this.NLCBCancelar.Name = "NLCBCancelar";
            this.NLCBCancelar.Size = new System.Drawing.Size(116, 23);
            this.NLCBCancelar.TabIndex = 2;
            this.NLCBCancelar.Text = "Cancelar";
            this.NLCBCancelar.UseVisualStyleBackColor = true;
            this.NLCBCancelar.Click += new System.EventHandler(this.NLCBCancelar_Click);
            // 
            // Form_3_ConfigurarLayersLinha
            // 
            this.AcceptButton = this.NLCBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.NLCBCancelar;
            this.ClientSize = new System.Drawing.Size(405, 72);
            this.Controls.Add(this.NLCBCancelar);
            this.Controls.Add(this.NLCBContinuar);
            this.Controls.Add(this.NLCCBLinhas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_3_ConfigurarLayersLinha";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Estilo de linha";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox NLCCBLinhas;
        private System.Windows.Forms.Button NLCBContinuar;
        private System.Windows.Forms.Button NLCBCancelar;
    }
}