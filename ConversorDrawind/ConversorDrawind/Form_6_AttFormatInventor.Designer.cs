namespace ConversorDrawind
{
    partial class Form_6_AttFormatInventor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_6_AttFormatInventor));
            this.DataAssociacao = new System.Windows.Forms.DataGridView();
            this.Blocos_Originais = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Blocos_Inventor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Width_Factor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AssociarPTags = new System.Windows.Forms.Button();
            this.AssociarPOrdem = new System.Windows.Forms.Button();
            this.bSave = new System.Windows.Forms.Button();
            this.Cancelar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DataAssociacao)).BeginInit();
            this.SuspendLayout();
            // 
            // DataAssociacao
            // 
            this.DataAssociacao.AllowUserToAddRows = false;
            this.DataAssociacao.AllowUserToDeleteRows = false;
            this.DataAssociacao.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataAssociacao.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.DataAssociacao.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataAssociacao.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Blocos_Originais,
            this.Blocos_Inventor,
            this.Width_Factor});
            this.DataAssociacao.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DataAssociacao.Location = new System.Drawing.Point(12, 55);
            this.DataAssociacao.Name = "DataAssociacao";
            this.DataAssociacao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DataAssociacao.Size = new System.Drawing.Size(763, 389);
            this.DataAssociacao.TabIndex = 3;
            this.DataAssociacao.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataAssociacao_CellDoubleClick);
            this.DataAssociacao.SelectionChanged += new System.EventHandler(this.DataAssociacao_SelectionChanged);
            this.DataAssociacao.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DataAssociacao_MouseClick);
            // 
            // Blocos_Originais
            // 
            this.Blocos_Originais.FillWeight = 300F;
            this.Blocos_Originais.HeaderText = "Blocos Originais";
            this.Blocos_Originais.MinimumWidth = 300;
            this.Blocos_Originais.Name = "Blocos_Originais";
            this.Blocos_Originais.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Blocos_Inventor
            // 
            this.Blocos_Inventor.FillWeight = 300F;
            this.Blocos_Inventor.HeaderText = "Blocos CAD";
            this.Blocos_Inventor.MinimumWidth = 300;
            this.Blocos_Inventor.Name = "Blocos_Inventor";
            this.Blocos_Inventor.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Width_Factor
            // 
            this.Width_Factor.HeaderText = "Width Factor";
            this.Width_Factor.Name = "Width_Factor";
            // 
            // AssociarPTags
            // 
            this.AssociarPTags.Location = new System.Drawing.Point(12, 12);
            this.AssociarPTags.Name = "AssociarPTags";
            this.AssociarPTags.Size = new System.Drawing.Size(137, 23);
            this.AssociarPTags.TabIndex = 4;
            this.AssociarPTags.Text = "Associar por tags";
            this.AssociarPTags.UseVisualStyleBackColor = true;
            this.AssociarPTags.Click += new System.EventHandler(this.AssociarPTags_Click);
            // 
            // AssociarPOrdem
            // 
            this.AssociarPOrdem.Location = new System.Drawing.Point(155, 12);
            this.AssociarPOrdem.Name = "AssociarPOrdem";
            this.AssociarPOrdem.Size = new System.Drawing.Size(137, 23);
            this.AssociarPOrdem.TabIndex = 5;
            this.AssociarPOrdem.Text = "Associar por ordem";
            this.AssociarPOrdem.UseVisualStyleBackColor = true;
            this.AssociarPOrdem.Click += new System.EventHandler(this.AssociarPOrdem_Click);
            // 
            // bSave
            // 
            this.bSave.Location = new System.Drawing.Point(700, 450);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 6;
            this.bSave.Text = "Salvar";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.bSave_Click);
            // 
            // Cancelar
            // 
            this.Cancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancelar.Location = new System.Drawing.Point(619, 450);
            this.Cancelar.Name = "Cancelar";
            this.Cancelar.Size = new System.Drawing.Size(75, 23);
            this.Cancelar.TabIndex = 7;
            this.Cancelar.Text = "Cancelar";
            this.Cancelar.UseVisualStyleBackColor = true;
            this.Cancelar.Click += new System.EventHandler(this.Cancelar_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(298, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(137, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Mover Tags Para Baixo";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(441, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "Mover Tags Para Cima";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form_6_AttFormatInventor
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancelar;
            this.ClientSize = new System.Drawing.Size(787, 483);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Cancelar);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.AssociarPOrdem);
            this.Controls.Add(this.AssociarPTags);
            this.Controls.Add(this.DataAssociacao);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_6_AttFormatInventor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuração";
            ((System.ComponentModel.ISupportInitialize)(this.DataAssociacao)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DataAssociacao;
        private System.Windows.Forms.Button AssociarPTags;
        private System.Windows.Forms.Button AssociarPOrdem;
        private System.Windows.Forms.Button bSave;
        private System.Windows.Forms.Button Cancelar;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Blocos_Originais;
        private System.Windows.Forms.DataGridViewTextBoxColumn Blocos_Inventor;
        private System.Windows.Forms.DataGridViewTextBoxColumn Width_Factor;
    }
}