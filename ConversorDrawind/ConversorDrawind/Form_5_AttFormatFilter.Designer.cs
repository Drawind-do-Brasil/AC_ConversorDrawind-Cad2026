namespace ConversorDrawind
{
    partial class Form_5_AttFormatFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_5_AttFormatFilter));
            this.ControlFilterGBTipo = new System.Windows.Forms.GroupBox();
            this.ControlFilterCBLayer = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBCor = new System.Windows.Forms.GroupBox();
            this.ControlFilterBCodigoCor = new System.Windows.Forms.Button();
            this.ControlFilterCBCor = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBTexto = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ControlFilterLTextoAltura = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ControlFilterLTextoConteudo = new System.Windows.Forms.TextBox();
            this.ControlFilterBCancelar = new System.Windows.Forms.Button();
            this.ControlFilterBContinuar = new System.Windows.Forms.Button();
            this.ControlFilterGBTipo.SuspendLayout();
            this.ControlFilterGBCor.SuspendLayout();
            this.ControlFilterGBTexto.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlFilterGBTipo
            // 
            this.ControlFilterGBTipo.Controls.Add(this.ControlFilterCBLayer);
            this.ControlFilterGBTipo.Location = new System.Drawing.Point(12, 12);
            this.ControlFilterGBTipo.Name = "ControlFilterGBTipo";
            this.ControlFilterGBTipo.Size = new System.Drawing.Size(260, 76);
            this.ControlFilterGBTipo.TabIndex = 1;
            this.ControlFilterGBTipo.TabStop = false;
            this.ControlFilterGBTipo.Text = "Layer";
            // 
            // ControlFilterCBLayer
            // 
            this.ControlFilterCBLayer.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ControlFilterCBLayer.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBLayer.FormattingEnabled = true;
            this.ControlFilterCBLayer.Location = new System.Drawing.Point(6, 19);
            this.ControlFilterCBLayer.Name = "ControlFilterCBLayer";
            this.ControlFilterCBLayer.Size = new System.Drawing.Size(247, 21);
            this.ControlFilterCBLayer.TabIndex = 0;
            // 
            // ControlFilterGBCor
            // 
            this.ControlFilterGBCor.Controls.Add(this.ControlFilterBCodigoCor);
            this.ControlFilterGBCor.Controls.Add(this.ControlFilterCBCor);
            this.ControlFilterGBCor.Location = new System.Drawing.Point(12, 94);
            this.ControlFilterGBCor.Name = "ControlFilterGBCor";
            this.ControlFilterGBCor.Size = new System.Drawing.Size(260, 76);
            this.ControlFilterGBCor.TabIndex = 2;
            this.ControlFilterGBCor.TabStop = false;
            this.ControlFilterGBCor.Text = "Cor";
            // 
            // ControlFilterBCodigoCor
            // 
            this.ControlFilterBCodigoCor.Location = new System.Drawing.Point(6, 46);
            this.ControlFilterBCodigoCor.Name = "ControlFilterBCodigoCor";
            this.ControlFilterBCodigoCor.Size = new System.Drawing.Size(246, 23);
            this.ControlFilterBCodigoCor.TabIndex = 1;
            this.ControlFilterBCodigoCor.Text = "Outros tipos de cor";
            this.ControlFilterBCodigoCor.UseVisualStyleBackColor = true;
            this.ControlFilterBCodigoCor.Click += new System.EventHandler(this.ControlFilterBCodigoCor_Click_1);
            // 
            // ControlFilterCBCor
            // 
            this.ControlFilterCBCor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ControlFilterCBCor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBCor.FormattingEnabled = true;
            this.ControlFilterCBCor.Location = new System.Drawing.Point(6, 19);
            this.ControlFilterCBCor.Name = "ControlFilterCBCor";
            this.ControlFilterCBCor.Size = new System.Drawing.Size(247, 21);
            this.ControlFilterCBCor.TabIndex = 0;
            // 
            // ControlFilterGBTexto
            // 
            this.ControlFilterGBTexto.Controls.Add(this.label2);
            this.ControlFilterGBTexto.Controls.Add(this.ControlFilterLTextoAltura);
            this.ControlFilterGBTexto.Controls.Add(this.label1);
            this.ControlFilterGBTexto.Controls.Add(this.ControlFilterLTextoConteudo);
            this.ControlFilterGBTexto.Location = new System.Drawing.Point(12, 176);
            this.ControlFilterGBTexto.Name = "ControlFilterGBTexto";
            this.ControlFilterGBTexto.Size = new System.Drawing.Size(260, 76);
            this.ControlFilterGBTexto.TabIndex = 3;
            this.ControlFilterGBTexto.TabStop = false;
            this.ControlFilterGBTexto.Text = "Texto";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Altura:";
            // 
            // ControlFilterLTextoAltura
            // 
            this.ControlFilterLTextoAltura.Location = new System.Drawing.Point(68, 45);
            this.ControlFilterLTextoAltura.Name = "ControlFilterLTextoAltura";
            this.ControlFilterLTextoAltura.Size = new System.Drawing.Size(185, 20);
            this.ControlFilterLTextoAltura.TabIndex = 1;
            this.ControlFilterLTextoAltura.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlFilterLTextoAltura_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Conteudo:";
            // 
            // ControlFilterLTextoConteudo
            // 
            this.ControlFilterLTextoConteudo.Location = new System.Drawing.Point(68, 19);
            this.ControlFilterLTextoConteudo.Name = "ControlFilterLTextoConteudo";
            this.ControlFilterLTextoConteudo.Size = new System.Drawing.Size(185, 20);
            this.ControlFilterLTextoConteudo.TabIndex = 0;
            // 
            // ControlFilterBCancelar
            // 
            this.ControlFilterBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ControlFilterBCancelar.Location = new System.Drawing.Point(156, 271);
            this.ControlFilterBCancelar.Name = "ControlFilterBCancelar";
            this.ControlFilterBCancelar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBCancelar.TabIndex = 4;
            this.ControlFilterBCancelar.Text = "Cancelar";
            this.ControlFilterBCancelar.UseVisualStyleBackColor = true;
            this.ControlFilterBCancelar.Click += new System.EventHandler(this.ControlFilterBCancelar_Click);
            // 
            // ControlFilterBContinuar
            // 
            this.ControlFilterBContinuar.Location = new System.Drawing.Point(13, 272);
            this.ControlFilterBContinuar.Name = "ControlFilterBContinuar";
            this.ControlFilterBContinuar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBContinuar.TabIndex = 0;
            this.ControlFilterBContinuar.Text = "Continuar";
            this.ControlFilterBContinuar.UseVisualStyleBackColor = true;
            this.ControlFilterBContinuar.Click += new System.EventHandler(this.ControlFilterBContinuar_Click);
            // 
            // Form_5_AttFormatFilter
            // 
            this.AcceptButton = this.ControlFilterBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ControlFilterBCancelar;
            this.ClientSize = new System.Drawing.Size(284, 306);
            this.Controls.Add(this.ControlFilterBCancelar);
            this.Controls.Add(this.ControlFilterGBTexto);
            this.Controls.Add(this.ControlFilterBContinuar);
            this.Controls.Add(this.ControlFilterGBCor);
            this.Controls.Add(this.ControlFilterGBTipo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_5_AttFormatFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filtro";
            this.ControlFilterGBTipo.ResumeLayout(false);
            this.ControlFilterGBCor.ResumeLayout(false);
            this.ControlFilterGBTexto.ResumeLayout(false);
            this.ControlFilterGBTexto.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ControlFilterGBTipo;
        private System.Windows.Forms.ComboBox ControlFilterCBLayer;
        private System.Windows.Forms.GroupBox ControlFilterGBCor;
        private System.Windows.Forms.Button ControlFilterBCodigoCor;
        private System.Windows.Forms.ComboBox ControlFilterCBCor;
        private System.Windows.Forms.GroupBox ControlFilterGBTexto;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ControlFilterLTextoAltura;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ControlFilterLTextoConteudo;
        private System.Windows.Forms.Button ControlFilterBCancelar;
        private System.Windows.Forms.Button ControlFilterBContinuar;
    }
}