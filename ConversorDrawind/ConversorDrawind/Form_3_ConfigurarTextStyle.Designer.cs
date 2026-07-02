namespace ConversorDrawind
{
    partial class Form_3_ConfigurarTextStyle
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_3_ConfigurarTextStyle));
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.Nome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fonte = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Oblique = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Negrito = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Tamanho = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WidthFactor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Angulo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bExcluirLinha = new System.Windows.Forms.Button();
            this.bincluirLinha = new System.Windows.Forms.Button();
            this.ControlNewLayerBCancelar = new System.Windows.Forms.Button();
            this.ControlNewLayerBContinuar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nome,
            this.Fonte,
            this.Oblique,
            this.Negrito,
            this.Tamanho,
            this.WidthFactor,
            this.Angulo});
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(12, 63);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(776, 333);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            // 
            // Nome
            // 
            this.Nome.HeaderText = "Nome";
            this.Nome.Name = "Nome";
            this.Nome.Width = 150;
            // 
            // Fonte
            // 
            this.Fonte.HeaderText = "Fonte";
            this.Fonte.Name = "Fonte";
            // 
            // Oblique
            // 
            this.Oblique.HeaderText = "Oblique";
            this.Oblique.Name = "Oblique";
            // 
            // Negrito
            // 
            this.Negrito.HeaderText = "Negrito";
            this.Negrito.Name = "Negrito";
            // 
            // Tamanho
            // 
            this.Tamanho.HeaderText = "Tamanho";
            this.Tamanho.Name = "Tamanho";
            // 
            // WidthFactor
            // 
            this.WidthFactor.HeaderText = "Width Factor";
            this.WidthFactor.Name = "WidthFactor";
            // 
            // Angulo
            // 
            this.Angulo.HeaderText = "Angulo";
            this.Angulo.Name = "Angulo";
            // 
            // bExcluirLinha
            // 
            this.bExcluirLinha.Location = new System.Drawing.Point(186, 27);
            this.bExcluirLinha.Name = "bExcluirLinha";
            this.bExcluirLinha.Size = new System.Drawing.Size(168, 30);
            this.bExcluirLinha.TabIndex = 7;
            this.bExcluirLinha.Text = "Excluir linha";
            this.bExcluirLinha.UseVisualStyleBackColor = true;
            this.bExcluirLinha.Click += new System.EventHandler(this.bExcluirLinha_Click);
            // 
            // bincluirLinha
            // 
            this.bincluirLinha.Location = new System.Drawing.Point(12, 27);
            this.bincluirLinha.Name = "bincluirLinha";
            this.bincluirLinha.Size = new System.Drawing.Size(168, 30);
            this.bincluirLinha.TabIndex = 6;
            this.bincluirLinha.Text = "Incluir linha";
            this.bincluirLinha.UseVisualStyleBackColor = true;
            this.bincluirLinha.Click += new System.EventHandler(this.bincluirLinha_Click);
            // 
            // ControlNewLayerBCancelar
            // 
            this.ControlNewLayerBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ControlNewLayerBCancelar.Location = new System.Drawing.Point(672, 415);
            this.ControlNewLayerBCancelar.Name = "ControlNewLayerBCancelar";
            this.ControlNewLayerBCancelar.Size = new System.Drawing.Size(116, 23);
            this.ControlNewLayerBCancelar.TabIndex = 9;
            this.ControlNewLayerBCancelar.Text = "Cancelar";
            this.ControlNewLayerBCancelar.UseVisualStyleBackColor = true;
            this.ControlNewLayerBCancelar.Click += new System.EventHandler(this.ControlNewLayerBCancelar_Click);
            // 
            // ControlNewLayerBContinuar
            // 
            this.ControlNewLayerBContinuar.Location = new System.Drawing.Point(550, 415);
            this.ControlNewLayerBContinuar.Name = "ControlNewLayerBContinuar";
            this.ControlNewLayerBContinuar.Size = new System.Drawing.Size(116, 23);
            this.ControlNewLayerBContinuar.TabIndex = 8;
            this.ControlNewLayerBContinuar.Text = "Salvar";
            this.ControlNewLayerBContinuar.UseVisualStyleBackColor = true;
            this.ControlNewLayerBContinuar.Click += new System.EventHandler(this.ControlNewLayerBContinuar_Click);
            // 
            // Form_3_ConfigurarTextStyle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ControlNewLayerBCancelar);
            this.Controls.Add(this.ControlNewLayerBContinuar);
            this.Controls.Add(this.bExcluirLinha);
            this.Controls.Add(this.bincluirLinha);
            this.Controls.Add(this.dataGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_3_ConfigurarTextStyle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configura estilos de texto";
            this.Load += new System.EventHandler(this.Form_3_ConfigurarTextStyle_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button bExcluirLinha;
        private System.Windows.Forms.Button bincluirLinha;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nome;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fonte;
        private System.Windows.Forms.DataGridViewTextBoxColumn Oblique;
        private System.Windows.Forms.DataGridViewTextBoxColumn Negrito;
        private System.Windows.Forms.DataGridViewTextBoxColumn Tamanho;
        private System.Windows.Forms.DataGridViewTextBoxColumn WidthFactor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Angulo;
        private System.Windows.Forms.Button ControlNewLayerBCancelar;
        private System.Windows.Forms.Button ControlNewLayerBContinuar;
    }
}