namespace ConversorDrawind
{
    partial class Form_4_LayersLayerBase
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_4_LayersLayerBase));
            this.LayerBaseLayers = new System.Windows.Forms.ComboBox();
            this.LayerBaseProcurar = new System.Windows.Forms.Button();
            this.LayerBaseContinuar = new System.Windows.Forms.Button();
            this.LayerBaseCancelar = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // LayerBaseLayers
            // 
            this.LayerBaseLayers.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.LayerBaseLayers.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.LayerBaseLayers.FormattingEnabled = true;
            this.LayerBaseLayers.Location = new System.Drawing.Point(12, 12);
            this.LayerBaseLayers.Name = "LayerBaseLayers";
            this.LayerBaseLayers.Size = new System.Drawing.Size(177, 21);
            this.LayerBaseLayers.TabIndex = 1;
            // 
            // LayerBaseProcurar
            // 
            this.LayerBaseProcurar.Location = new System.Drawing.Point(195, 10);
            this.LayerBaseProcurar.Name = "LayerBaseProcurar";
            this.LayerBaseProcurar.Size = new System.Drawing.Size(61, 23);
            this.LayerBaseProcurar.TabIndex = 2;
            this.LayerBaseProcurar.Text = "...";
            this.LayerBaseProcurar.UseVisualStyleBackColor = true;
            this.LayerBaseProcurar.Click += new System.EventHandler(this.LayerBaseProcurar_Click);
            // 
            // LayerBaseContinuar
            // 
            this.LayerBaseContinuar.Location = new System.Drawing.Point(12, 39);
            this.LayerBaseContinuar.Name = "LayerBaseContinuar";
            this.LayerBaseContinuar.Size = new System.Drawing.Size(116, 23);
            this.LayerBaseContinuar.TabIndex = 0;
            this.LayerBaseContinuar.Text = "Continuar";
            this.LayerBaseContinuar.UseVisualStyleBackColor = true;
            this.LayerBaseContinuar.Click += new System.EventHandler(this.LayerBaseContinuar_Click);
            // 
            // LayerBaseCancelar
            // 
            this.LayerBaseCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.LayerBaseCancelar.Location = new System.Drawing.Point(140, 39);
            this.LayerBaseCancelar.Name = "LayerBaseCancelar";
            this.LayerBaseCancelar.Size = new System.Drawing.Size(116, 23);
            this.LayerBaseCancelar.TabIndex = 3;
            this.LayerBaseCancelar.Text = "Cancelar";
            this.LayerBaseCancelar.UseVisualStyleBackColor = true;
            this.LayerBaseCancelar.Click += new System.EventHandler(this.LayerBaseCancelar_Click);
            // 
            // Form_4_LayersLayerBase
            // 
            this.AcceptButton = this.LayerBaseContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.LayerBaseCancelar;
            this.ClientSize = new System.Drawing.Size(268, 72);
            this.Controls.Add(this.LayerBaseCancelar);
            this.Controls.Add(this.LayerBaseContinuar);
            this.Controls.Add(this.LayerBaseProcurar);
            this.Controls.Add(this.LayerBaseLayers);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_4_LayersLayerBase";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Layer Base";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControlLayerBase_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox LayerBaseLayers;
        private System.Windows.Forms.Button LayerBaseProcurar;
        private System.Windows.Forms.Button LayerBaseContinuar;
        private System.Windows.Forms.Button LayerBaseCancelar;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}