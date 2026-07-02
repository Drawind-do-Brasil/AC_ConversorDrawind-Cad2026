namespace ConversorDrawind
{
    partial class Form_6_TagFormatInventor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_6_TagFormatInventor));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ControlFilterBCancelar = new System.Windows.Forms.Button();
            this.ControlFilterBContinuar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(15, 12);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(259, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBox1_DrawItem);
            this.comboBox1.SelectedValueChanged += new System.EventHandler(this.comboBox1_SelectedValueChanged);
            // 
            // ControlFilterBCancelar
            // 
            this.ControlFilterBCancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ControlFilterBCancelar.Location = new System.Drawing.Point(158, 57);
            this.ControlFilterBCancelar.Name = "ControlFilterBCancelar";
            this.ControlFilterBCancelar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBCancelar.TabIndex = 7;
            this.ControlFilterBCancelar.Text = "Cancelar";
            this.ControlFilterBCancelar.UseVisualStyleBackColor = true;
            this.ControlFilterBCancelar.Click += new System.EventHandler(this.ControlFilterBCancelar_Click);
            // 
            // ControlFilterBContinuar
            // 
            this.ControlFilterBContinuar.Location = new System.Drawing.Point(15, 58);
            this.ControlFilterBContinuar.Name = "ControlFilterBContinuar";
            this.ControlFilterBContinuar.Size = new System.Drawing.Size(116, 23);
            this.ControlFilterBContinuar.TabIndex = 6;
            this.ControlFilterBContinuar.Text = "Continuar";
            this.ControlFilterBContinuar.UseVisualStyleBackColor = true;
            this.ControlFilterBContinuar.Click += new System.EventHandler(this.ControlFilterBContinuar_Click);
            // 
            // Form_6_TagFormatInventor
            // 
            this.AcceptButton = this.ControlFilterBContinuar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ControlFilterBCancelar;
            this.ClientSize = new System.Drawing.Size(288, 93);
            this.Controls.Add(this.ControlFilterBCancelar);
            this.Controls.Add(this.ControlFilterBContinuar);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_6_TagFormatInventor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tags Formato Inventor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form_AssTag_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button ControlFilterBCancelar;
        private System.Windows.Forms.Button ControlFilterBContinuar;
    }
}