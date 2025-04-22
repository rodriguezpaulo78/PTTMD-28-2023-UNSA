namespace PTTMD_28_2023_UNSA.CapaPresentacion
{
    partial class CPInterfazAnalisis
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
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnExplorar = new System.Windows.Forms.Button();
            this.lblRuta = new System.Windows.Forms.Label();
            this.txcRuta = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblArbol = new System.Windows.Forms.Label();
            this.trvArbol = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(687, 36);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(97, 23);
            this.btnLimpiar.TabIndex = 8;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            // 
            // btnExplorar
            // 
            this.btnExplorar.Location = new System.Drawing.Point(582, 36);
            this.btnExplorar.Name = "btnExplorar";
            this.btnExplorar.Size = new System.Drawing.Size(97, 23);
            this.btnExplorar.TabIndex = 7;
            this.btnExplorar.Text = "Cargar";
            this.btnExplorar.UseVisualStyleBackColor = true;
            // 
            // lblRuta
            // 
            this.lblRuta.AutoSize = true;
            this.lblRuta.Location = new System.Drawing.Point(12, 16);
            this.lblRuta.Name = "lblRuta";
            this.lblRuta.Size = new System.Drawing.Size(117, 16);
            this.lblRuta.TabIndex = 6;
            this.lblRuta.Text = "Ruta del Proyecto:";
            // 
            // txcRuta
            // 
            this.txcRuta.Location = new System.Drawing.Point(12, 37);
            this.txcRuta.Name = "txcRuta";
            this.txcRuta.ReadOnly = true;
            this.txcRuta.Size = new System.Drawing.Size(548, 22);
            this.txcRuta.TabIndex = 5;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(582, 92);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Explorar";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Ruta del Proyecto:";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 93);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(548, 22);
            this.textBox1.TabIndex = 9;
            // 
            // lblArbol
            // 
            this.lblArbol.AutoSize = true;
            this.lblArbol.Location = new System.Drawing.Point(15, 138);
            this.lblArbol.Name = "lblArbol";
            this.lblArbol.Size = new System.Drawing.Size(118, 16);
            this.lblArbol.TabIndex = 14;
            this.lblArbol.Text = "Arbol del Proyecto";
            // 
            // trvArbol
            // 
            this.trvArbol.Location = new System.Drawing.Point(15, 163);
            this.trvArbol.Name = "trvArbol";
            this.trvArbol.Size = new System.Drawing.Size(221, 263);
            this.trvArbol.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(796, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 16);
            this.label2.TabIndex = 16;
            this.label2.Text = "Arbol del Proyecto";
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(796, 163);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(221, 263);
            this.treeView1.TabIndex = 15;
            // 
            // btnAnalizar
            // 
            this.btnAnalizar.Location = new System.Drawing.Point(687, 92);
            this.btnAnalizar.Name = "btnAnalizar";
            this.btnAnalizar.Size = new System.Drawing.Size(314, 23);
            this.btnAnalizar.TabIndex = 17;
            this.btnAnalizar.Text = "Realizar Analisis";
            this.btnAnalizar.UseVisualStyleBackColor = true;
            // 
            // CPInterfazAnalisis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1047, 450);
            this.Controls.Add(this.btnAnalizar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.lblArbol);
            this.Controls.Add(this.trvArbol);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnExplorar);
            this.Controls.Add(this.lblRuta);
            this.Controls.Add(this.txcRuta);
            this.Name = "CPInterfazAnalisis";
            this.Text = "CPInterfazAnalisis";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnExplorar;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.TextBox txcRuta;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblArbol;
        private System.Windows.Forms.TreeView trvArbol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button btnAnalizar;
    }
}