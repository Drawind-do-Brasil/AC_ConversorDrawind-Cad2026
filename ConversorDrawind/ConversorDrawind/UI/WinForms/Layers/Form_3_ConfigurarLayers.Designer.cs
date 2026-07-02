namespace ConversorDrawind
{
    partial class Form_3_ConfigurarLayers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_3_ConfigurarLayers));
            this.bCarregar = new System.Windows.Forms.Button();
            this.bLimparTudo = new System.Windows.Forms.Button();
            this.bincluirLinha = new System.Windows.Forms.Button();
            this.bExcluirLinha = new System.Windows.Forms.Button();
            this.dGVNewLayers = new System.Windows.Forms.DataGridView();
            this.nomeDoLayerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.corDoLayerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.linhaDoLayerDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bCancelar = new System.Windows.Forms.Button();
            this.bContinuar = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dGVNewLayers)).BeginInit();
            this.SuspendLayout();
            // 
            // bCarregar
            // 
            this.bCarregar.Location = new System.Drawing.Point(12, 12);
            this.bCarregar.Name = "bCarregar";
            this.bCarregar.Size = new System.Drawing.Size(168, 30);
            this.bCarregar.TabIndex = 2;
            this.bCarregar.Text = "Carregar";
            this.bCarregar.UseVisualStyleBackColor = true;
            this.bCarregar.Click += new System.EventHandler(this.bCarregar_Click);
            // 
            // bLimparTudo
            // 
            this.bLimparTudo.Location = new System.Drawing.Point(186, 12);
            this.bLimparTudo.Name = "bLimparTudo";
            this.bLimparTudo.Size = new System.Drawing.Size(168, 30);
            this.bLimparTudo.TabIndex = 3;
            this.bLimparTudo.Text = "Limpar tudo";
            this.bLimparTudo.UseVisualStyleBackColor = true;
            this.bLimparTudo.Click += new System.EventHandler(this.bLimparTudo_Click);
            // 
            // bincluirLinha
            // 
            this.bincluirLinha.Location = new System.Drawing.Point(360, 12);
            this.bincluirLinha.Name = "bincluirLinha";
            this.bincluirLinha.Size = new System.Drawing.Size(168, 30);
            this.bincluirLinha.TabIndex = 4;
            this.bincluirLinha.Text = "Incluir linha";
            this.bincluirLinha.UseVisualStyleBackColor = true;
            this.bincluirLinha.Click += new System.EventHandler(this.bincluirLinha_Click);
            // 
            // bExcluirLinha
            // 
            this.bExcluirLinha.Location = new System.Drawing.Point(534, 12);
            this.bExcluirLinha.Name = "bExcluirLinha";
            this.bExcluirLinha.Size = new System.Drawing.Size(168, 30);
            this.bExcluirLinha.TabIndex = 5;
            this.bExcluirLinha.Text = "Excluir linha";
            this.bExcluirLinha.UseVisualStyleBackColor = true;
            this.bExcluirLinha.Click += new System.EventHandler(this.bExcluirLinha_Click);
            // 
            // dGVNewLayers
            // 
            this.dGVNewLayers.AllowUserToAddRows = false;
            this.dGVNewLayers.AllowUserToDeleteRows = false;
            this.dGVNewLayers.AllowUserToResizeColumns = false;
            this.dGVNewLayers.AllowUserToResizeRows = false;
            this.dGVNewLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVNewLayers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nomeDoLayerDataGridViewTextBoxColumn,
            this.corDoLayerDataGridViewTextBoxColumn,
            this.linhaDoLayerDataGridViewTextBoxColumn});
            this.dGVNewLayers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dGVNewLayers.Location = new System.Drawing.Point(12, 48);
            this.dGVNewLayers.MultiSelect = false;
            this.dGVNewLayers.Name = "dGVNewLayers";
            this.dGVNewLayers.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dGVNewLayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dGVNewLayers.Size = new System.Drawing.Size(690, 307);
            this.dGVNewLayers.TabIndex = 6;
            this.dGVNewLayers.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dGVNewLayers_CellDoubleClick);
            // 
            // nomeDoLayerDataGridViewTextBoxColumn
            // 
            this.nomeDoLayerDataGridViewTextBoxColumn.HeaderText = "Nome do Layer";
            this.nomeDoLayerDataGridViewTextBoxColumn.Name = "nomeDoLayerDataGridViewTextBoxColumn";
            this.nomeDoLayerDataGridViewTextBoxColumn.Width = 220;
            // 
            // corDoLayerDataGridViewTextBoxColumn
            // 
            this.corDoLayerDataGridViewTextBoxColumn.HeaderText = "Cor do Layer";
            this.corDoLayerDataGridViewTextBoxColumn.Name = "corDoLayerDataGridViewTextBoxColumn";
            this.corDoLayerDataGridViewTextBoxColumn.Width = 220;
            // 
            // linhaDoLayerDataGridViewTextBoxColumn
            // 
            this.linhaDoLayerDataGridViewTextBoxColumn.HeaderText = "Linha do Layer";
            this.linhaDoLayerDataGridViewTextBoxColumn.Name = "linhaDoLayerDataGridViewTextBoxColumn";
            this.linhaDoLayerDataGridViewTextBoxColumn.Width = 220;
            // 
            // bCancelar
            // 
            this.bCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancelar.Location = new System.Drawing.Point(534, 361);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(168, 30);
            this.bCancelar.TabIndex = 1;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // bContinuar
            // 
            this.bContinuar.Location = new System.Drawing.Point(360, 361);
            this.bContinuar.Name = "bContinuar";
            this.bContinuar.Size = new System.Drawing.Size(168, 30);
            this.bContinuar.TabIndex = 0;
            this.bContinuar.Text = "Continuar";
            this.bContinuar.UseVisualStyleBackColor = true;
            this.bContinuar.Click += new System.EventHandler(this.bContinuar_Click);
            // 
            // Form_3_ConfigurarLayers
            // 
            this.AcceptButton = this.bContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(714, 403);
            this.Controls.Add(this.bContinuar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.dGVNewLayers);
            this.Controls.Add(this.bExcluirLinha);
            this.Controls.Add(this.bincluirLinha);
            this.Controls.Add(this.bLimparTudo);
            this.Controls.Add(this.bCarregar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_3_ConfigurarLayers";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurar Novos Layers";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewLayerConfiguration_FormClosed);
            this.Load += new System.EventHandler(this.NewLayerConfiguration_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dGVNewLayers)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bCarregar;
        private System.Windows.Forms.Button bLimparTudo;
        private System.Windows.Forms.Button bincluirLinha;
        private System.Windows.Forms.Button bExcluirLinha;
        private System.Windows.Forms.DataGridView dGVNewLayers;
        private System.Windows.Forms.Button bCancelar;
        private System.Windows.Forms.Button bContinuar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.DataGridViewTextBoxColumn nomeDoLayerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn corDoLayerDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn linhaDoLayerDataGridViewTextBoxColumn;
    }
}