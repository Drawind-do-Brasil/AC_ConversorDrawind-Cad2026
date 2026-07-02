namespace ConversorDrawind
{
    partial class Form_4_LayersFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_4_LayersFilter));
            this.ControlFilterGBTipo = new System.Windows.Forms.GroupBox();
            this.ControlFilterBAddOntrasEntidades = new System.Windows.Forms.Button();
            this.ControlFilterCBTipo = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBCor = new System.Windows.Forms.GroupBox();
            this.ControlFilterBCodigoCor = new System.Windows.Forms.Button();
            this.ControlFilterCBCor = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBLinha = new System.Windows.Forms.GroupBox();
            this.ControlFilterBLinhaCarregar = new System.Windows.Forms.Button();
            this.ControlFilterCBLinha = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBTexto = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ControlFilterLTextoAltura = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ControlFilterLTextoConteudo = new System.Windows.Forms.TextBox();
            this.ControlFilterBContinuar = new System.Windows.Forms.Button();
            this.ControlFilterBCancelar = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ControlFilterCBOrientacao = new System.Windows.Forms.ComboBox();
            this.ControlFilterGBTipo.SuspendLayout();
            this.ControlFilterGBCor.SuspendLayout();
            this.ControlFilterGBLinha.SuspendLayout();
            this.ControlFilterGBTexto.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ControlFilterGBTipo
            // 
            this.ControlFilterGBTipo.Controls.Add(this.ControlFilterBAddOntrasEntidades);
            this.ControlFilterGBTipo.Controls.Add(this.ControlFilterCBTipo);
            this.ControlFilterGBTipo.Location = new System.Drawing.Point(12, 12);
            this.ControlFilterGBTipo.Name = "ControlFilterGBTipo";
            this.ControlFilterGBTipo.Size = new System.Drawing.Size(260, 76);
            this.ControlFilterGBTipo.TabIndex = 1;
            this.ControlFilterGBTipo.TabStop = false;
            this.ControlFilterGBTipo.Text = "Tipo de Objeto";
            // 
            // ControlFilterBAddOntrasEntidades
            // 
            this.ControlFilterBAddOntrasEntidades.Location = new System.Drawing.Point(6, 46);
            this.ControlFilterBAddOntrasEntidades.Name = "ControlFilterBAddOntrasEntidades";
            this.ControlFilterBAddOntrasEntidades.Size = new System.Drawing.Size(246, 23);
            this.ControlFilterBAddOntrasEntidades.TabIndex = 1;
            this.ControlFilterBAddOntrasEntidades.Text = "Adicionar outras entidades";
            this.ControlFilterBAddOntrasEntidades.UseVisualStyleBackColor = true;
            this.ControlFilterBAddOntrasEntidades.Click += new System.EventHandler(this.ControlFilterBAddOntrasEntidades_Click);
            // 
            // ControlFilterCBTipo
            // 
            this.ControlFilterCBTipo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ControlFilterCBTipo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBTipo.FormattingEnabled = true;
            this.ControlFilterCBTipo.Location = new System.Drawing.Point(6, 19);
            this.ControlFilterCBTipo.Name = "ControlFilterCBTipo";
            this.ControlFilterCBTipo.Size = new System.Drawing.Size(247, 21);
            this.ControlFilterCBTipo.TabIndex = 0;
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
            this.ControlFilterBCodigoCor.Click += new System.EventHandler(this.ControlFilterBCodigoCor_Click);
            // 
            // ControlFilterCBCor
            // 
            this.ControlFilterCBCor.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ControlFilterCBCor.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBCor.FormattingEnabled = true;
            this.ControlFilterCBCor.Location = new System.Drawing.Point(6, 19);
            this.ControlFilterCBCor.Name = "ControlFilterCBCor";
            this.ControlFilterCBCor.Size = new System.Drawing.Size(247, 21);
            this.ControlFilterCBCor.TabIndex = 0;
            // 
            // ControlFilterGBLinha
            // 
            this.ControlFilterGBLinha.Controls.Add(this.ControlFilterBLinhaCarregar);
            this.ControlFilterGBLinha.Controls.Add(this.ControlFilterCBLinha);
            this.ControlFilterGBLinha.Location = new System.Drawing.Point(12, 176);
            this.ControlFilterGBLinha.Name = "ControlFilterGBLinha";
            this.ControlFilterGBLinha.Size = new System.Drawing.Size(260, 77);
            this.ControlFilterGBLinha.TabIndex = 3;
            this.ControlFilterGBLinha.TabStop = false;
            this.ControlFilterGBLinha.Text = "Tipo de Linha";
            // 
            // ControlFilterBLinhaCarregar
            // 
            this.ControlFilterBLinhaCarregar.Location = new System.Drawing.Point(7, 46);
            this.ControlFilterBLinhaCarregar.Name = "ControlFilterBLinhaCarregar";
            this.ControlFilterBLinhaCarregar.Size = new System.Drawing.Size(246, 23);
            this.ControlFilterBLinhaCarregar.TabIndex = 1;
            this.ControlFilterBLinhaCarregar.Text = "Carregar outros tipos de linha";
            this.ControlFilterBLinhaCarregar.UseVisualStyleBackColor = true;
            this.ControlFilterBLinhaCarregar.Click += new System.EventHandler(this.ControlFilterBLinhaCarregar_Click);
            // 
            // ControlFilterCBLinha
            // 
            this.ControlFilterCBLinha.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ControlFilterCBLinha.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBLinha.FormattingEnabled = true;
            this.ControlFilterCBLinha.Location = new System.Drawing.Point(6, 19);
            this.ControlFilterCBLinha.Name = "ControlFilterCBLinha";
            this.ControlFilterCBLinha.Size = new System.Drawing.Size(247, 21);
            this.ControlFilterCBLinha.TabIndex = 0;
            // 
            // ControlFilterGBTexto
            // 
            this.ControlFilterGBTexto.Controls.Add(this.label2);
            this.ControlFilterGBTexto.Controls.Add(this.ControlFilterLTextoAltura);
            this.ControlFilterGBTexto.Controls.Add(this.label1);
            this.ControlFilterGBTexto.Controls.Add(this.ControlFilterLTextoConteudo);
            this.ControlFilterGBTexto.Location = new System.Drawing.Point(12, 259);
            this.ControlFilterGBTexto.Name = "ControlFilterGBTexto";
            this.ControlFilterGBTexto.Size = new System.Drawing.Size(260, 76);
            this.ControlFilterGBTexto.TabIndex = 4;
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
            // ControlFilterBContinuar
            // 
            this.ControlFilterBContinuar.Location = new System.Drawing.Point(13, 412);
            this.ControlFilterBContinuar.Name = "ControlFilterBContinuar";
            this.ControlFilterBContinuar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBContinuar.TabIndex = 0;
            this.ControlFilterBContinuar.Text = "Continuar";
            this.ControlFilterBContinuar.UseVisualStyleBackColor = true;
            this.ControlFilterBContinuar.Click += new System.EventHandler(this.ControlFilterBContinuar_Click);
            // 
            // ControlFilterBCancelar
            // 
            this.ControlFilterBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ControlFilterBCancelar.Location = new System.Drawing.Point(156, 411);
            this.ControlFilterBCancelar.Name = "ControlFilterBCancelar";
            this.ControlFilterBCancelar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBCancelar.TabIndex = 5;
            this.ControlFilterBCancelar.Text = "Cancelar";
            this.ControlFilterBCancelar.UseVisualStyleBackColor = true;
            this.ControlFilterBCancelar.Click += new System.EventHandler(this.ControlFilterBCancelar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.ControlFilterCBOrientacao);
            this.groupBox1.Location = new System.Drawing.Point(12, 335);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 57);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Orientação (Linha)";
            // 
            // ControlFilterCBOrientacao
            // 
            this.ControlFilterCBOrientacao.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.ControlFilterCBOrientacao.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.ControlFilterCBOrientacao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControlFilterCBOrientacao.FormattingEnabled = true;
            this.ControlFilterCBOrientacao.Items.AddRange(new object[] {
            "ALL",
            "HORIZONTAL",
            "VERTICAL"});
            this.ControlFilterCBOrientacao.Location = new System.Drawing.Point(9, 19);
            this.ControlFilterCBOrientacao.Name = "ControlFilterCBOrientacao";
            this.ControlFilterCBOrientacao.Size = new System.Drawing.Size(243, 21);
            this.ControlFilterCBOrientacao.TabIndex = 0;
            this.ControlFilterCBOrientacao.SelectedIndexChanged += new System.EventHandler(this.ControlFilterCBOrientacao_SelectedIndexChanged);
            this.ControlFilterCBOrientacao.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ControlFilterCBOrientacao_KeyPress);
            // 
            // Form_4_LayersFilter
            // 
            this.AcceptButton = this.ControlFilterBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ControlFilterBCancelar;
            this.ClientSize = new System.Drawing.Size(284, 446);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ControlFilterBCancelar);
            this.Controls.Add(this.ControlFilterBContinuar);
            this.Controls.Add(this.ControlFilterGBTexto);
            this.Controls.Add(this.ControlFilterGBLinha);
            this.Controls.Add(this.ControlFilterGBCor);
            this.Controls.Add(this.ControlFilterGBTipo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_4_LayersFilter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filtro";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlFilter_FormClosed);
            this.ControlFilterGBTipo.ResumeLayout(false);
            this.ControlFilterGBCor.ResumeLayout(false);
            this.ControlFilterGBLinha.ResumeLayout(false);
            this.ControlFilterGBTexto.ResumeLayout(false);
            this.ControlFilterGBTexto.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ControlFilterGBTipo;
        private System.Windows.Forms.ComboBox ControlFilterCBTipo;
        private System.Windows.Forms.GroupBox ControlFilterGBCor;
        private System.Windows.Forms.ComboBox ControlFilterCBCor;
        private System.Windows.Forms.GroupBox ControlFilterGBLinha;
        private System.Windows.Forms.Button ControlFilterBLinhaCarregar;
        private System.Windows.Forms.ComboBox ControlFilterCBLinha;
        private System.Windows.Forms.GroupBox ControlFilterGBTexto;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ControlFilterLTextoConteudo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ControlFilterLTextoAltura;
        private System.Windows.Forms.Button ControlFilterBContinuar;
        private System.Windows.Forms.Button ControlFilterBCancelar;
        private System.Windows.Forms.Button ControlFilterBCodigoCor;
        private System.Windows.Forms.Button ControlFilterBAddOntrasEntidades;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox ControlFilterCBOrientacao;
    }
}