namespace ConversorDrawind
{
    partial class Form_4_NewTextSyle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_4_NewTextSyle));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Fonte = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Tamanho = new System.Windows.Forms.TextBox();
            this.ControlNewLayerBContinuar = new System.Windows.Forms.Button();
            this.ControlNewLayerBCancelar = new System.Windows.Forms.Button();
            this.Nome = new System.Windows.Forms.TextBox();
            this.WidthFactor = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Angulo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Oblique = new System.Windows.Forms.CheckBox();
            this.Negrito = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nome:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Fonte:";
            // 
            // Fonte
            // 
            this.Fonte.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.Fonte.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Fonte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Fonte.FormattingEnabled = true;
            this.Fonte.Location = new System.Drawing.Point(91, 37);
            this.Fonte.Name = "Fonte";
            this.Fonte.Size = new System.Drawing.Size(173, 21);
            this.Fonte.TabIndex = 2;
            this.Fonte.SelectedIndexChanged += new System.EventHandler(this.Fonte_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Tamanho:";
            // 
            // Tamanho
            // 
            this.Tamanho.Location = new System.Drawing.Point(91, 64);
            this.Tamanho.Name = "Tamanho";
            this.Tamanho.Size = new System.Drawing.Size(100, 20);
            this.Tamanho.TabIndex = 6;
            this.Tamanho.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Tamanho_KeyPress);
            this.Tamanho.Leave += new System.EventHandler(this.Tamanho_Leave);
            // 
            // ControlNewLayerBContinuar
            // 
            this.ControlNewLayerBContinuar.Location = new System.Drawing.Point(235, 113);
            this.ControlNewLayerBContinuar.Name = "ControlNewLayerBContinuar";
            this.ControlNewLayerBContinuar.Size = new System.Drawing.Size(116, 23);
            this.ControlNewLayerBContinuar.TabIndex = 0;
            this.ControlNewLayerBContinuar.Text = "Salvar";
            this.ControlNewLayerBContinuar.UseVisualStyleBackColor = true;
            this.ControlNewLayerBContinuar.Click += new System.EventHandler(this.ControlNewLayerBContinuar_Click);
            // 
            // ControlNewLayerBCancelar
            // 
            this.ControlNewLayerBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ControlNewLayerBCancelar.Location = new System.Drawing.Point(357, 113);
            this.ControlNewLayerBCancelar.Name = "ControlNewLayerBCancelar";
            this.ControlNewLayerBCancelar.Size = new System.Drawing.Size(116, 23);
            this.ControlNewLayerBCancelar.TabIndex = 7;
            this.ControlNewLayerBCancelar.Text = "Cancelar";
            this.ControlNewLayerBCancelar.UseVisualStyleBackColor = true;
            this.ControlNewLayerBCancelar.Click += new System.EventHandler(this.ControlNewLayerBCancelar_Click);
            // 
            // Nome
            // 
            this.Nome.Location = new System.Drawing.Point(91, 12);
            this.Nome.Name = "Nome";
            this.Nome.Size = new System.Drawing.Size(173, 20);
            this.Nome.TabIndex = 13;
            // 
            // WidthFactor
            // 
            this.WidthFactor.Location = new System.Drawing.Point(91, 90);
            this.WidthFactor.Name = "WidthFactor";
            this.WidthFactor.Size = new System.Drawing.Size(100, 20);
            this.WidthFactor.TabIndex = 14;
            this.WidthFactor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WidthFactor_KeyPress);
            this.WidthFactor.Leave += new System.EventHandler(this.WidthFactor_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Width Factor:";
            // 
            // Angulo
            // 
            this.Angulo.Location = new System.Drawing.Point(91, 116);
            this.Angulo.Name = "Angulo";
            this.Angulo.Size = new System.Drawing.Size(100, 20);
            this.Angulo.TabIndex = 16;
            this.Angulo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Angulo_KeyPress);
            this.Angulo.Leave += new System.EventHandler(this.Angulo_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Angulo:";
            // 
            // Oblique
            // 
            this.Oblique.AutoSize = true;
            this.Oblique.Location = new System.Drawing.Point(310, 14);
            this.Oblique.Name = "Oblique";
            this.Oblique.Size = new System.Drawing.Size(62, 17);
            this.Oblique.TabIndex = 18;
            this.Oblique.Text = "Oblique";
            this.Oblique.UseVisualStyleBackColor = true;
            // 
            // Negrito
            // 
            this.Negrito.AutoSize = true;
            this.Negrito.Location = new System.Drawing.Point(310, 37);
            this.Negrito.Name = "Negrito";
            this.Negrito.Size = new System.Drawing.Size(60, 17);
            this.Negrito.TabIndex = 19;
            this.Negrito.Text = "Negrito";
            this.Negrito.UseVisualStyleBackColor = true;
            // 
            // Form_4_NewTextSyle
            // 
            this.AcceptButton = this.ControlNewLayerBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.ControlNewLayerBCancelar;
            this.ClientSize = new System.Drawing.Size(485, 152);
            this.Controls.Add(this.Negrito);
            this.Controls.Add(this.Oblique);
            this.Controls.Add(this.Angulo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.WidthFactor);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Nome);
            this.Controls.Add(this.ControlNewLayerBCancelar);
            this.Controls.Add(this.ControlNewLayerBContinuar);
            this.Controls.Add(this.Tamanho);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Fonte);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_4_NewTextSyle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Novo Estilo de Texto";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlNewLayer_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox Fonte;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox Tamanho;
        private System.Windows.Forms.Button ControlNewLayerBContinuar;
        private System.Windows.Forms.Button ControlNewLayerBCancelar;
        private System.Windows.Forms.TextBox Nome;
        private System.Windows.Forms.TextBox WidthFactor;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Angulo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox Oblique;
        private System.Windows.Forms.CheckBox Negrito;
    }
}