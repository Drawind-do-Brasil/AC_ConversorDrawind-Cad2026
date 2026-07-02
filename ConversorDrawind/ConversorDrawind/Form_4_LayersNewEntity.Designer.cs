namespace ConversorDrawind
{
    partial class Form_4_LayersNewEntity
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_4_LayersNewEntity));
            this.label1 = new System.Windows.Forms.Label();
            this.tBEntity = new System.Windows.Forms.TextBox();
            this.bContinuar = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Digite tipo de elemento (DXF Code):";
            // 
            // tBEntity
            // 
            this.tBEntity.Location = new System.Drawing.Point(12, 39);
            this.tBEntity.Name = "tBEntity";
            this.tBEntity.Size = new System.Drawing.Size(331, 20);
            this.tBEntity.TabIndex = 1;
            // 
            // bContinuar
            // 
            this.bContinuar.Location = new System.Drawing.Point(12, 71);
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
            this.bCancelar.Location = new System.Drawing.Point(227, 71);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(116, 23);
            this.bCancelar.TabIndex = 2;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.UseVisualStyleBackColor = true;
            this.bCancelar.Click += new System.EventHandler(this.bCancelar_Click);
            // 
            // Form_4_LayersNewEntity
            // 
            this.AcceptButton = this.bContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.bCancelar;
            this.ClientSize = new System.Drawing.Size(356, 106);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bContinuar);
            this.Controls.Add(this.tBEntity);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_4_LayersNewEntity";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nova entidade";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewEntity_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tBEntity;
        private System.Windows.Forms.Button bContinuar;
        private System.Windows.Forms.Button bCancelar;
    }
}