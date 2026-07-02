namespace ConversorDrawindDLL
{
    partial class ScaleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScaleForm));
            this.label1 = new System.Windows.Forms.Label();
            this.scaleB = new System.Windows.Forms.TextBox();
            this.OK = new System.Windows.Forms.Button();
            this.Cancela = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(150, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Informe a escala do desenho: ";
            // 
            // scaleB
            // 
            this.scaleB.Location = new System.Drawing.Point(188, 34);
            this.scaleB.Name = "scaleB";
            this.scaleB.Size = new System.Drawing.Size(100, 20);
            this.scaleB.TabIndex = 0;
            this.scaleB.Text = "10";
            this.scaleB.Enter += new System.EventHandler(this.scaleB_Enter);
            this.scaleB.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(213, 70);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Cancela
            // 
            this.Cancela.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancela.Location = new System.Drawing.Point(132, 70);
            this.Cancela.Name = "Cancela";
            this.Cancela.Size = new System.Drawing.Size(75, 23);
            this.Cancela.TabIndex = 2;
            this.Cancela.Text = "Cancelar";
            this.Cancela.UseVisualStyleBackColor = true;
            this.Cancela.Click += new System.EventHandler(this.Cancela_Click);
            // 
            // ScaleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.Cancela;
            this.ClientSize = new System.Drawing.Size(305, 105);
            this.Controls.Add(this.Cancela);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.scaleB);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ScaleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Escala";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox scaleB;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancela;
    }
}