namespace PTTMD_28_2023_UNSA
{
    partial class Principal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txcRuta = new System.Windows.Forms.TextBox();
            this.lblRuta = new System.Windows.Forms.Label();
            this.btnExplorar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.trvArbol = new System.Windows.Forms.TreeView();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(28, 128);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(556, 292);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(548, 263);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(548, 263);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // txcRuta
            // 
            this.txcRuta.Location = new System.Drawing.Point(32, 34);
            this.txcRuta.Name = "txcRuta";
            this.txcRuta.ReadOnly = true;
            this.txcRuta.Size = new System.Drawing.Size(548, 22);
            this.txcRuta.TabIndex = 1;
            // 
            // lblRuta
            // 
            this.lblRuta.AutoSize = true;
            this.lblRuta.Location = new System.Drawing.Point(32, 13);
            this.lblRuta.Name = "lblRuta";
            this.lblRuta.Size = new System.Drawing.Size(117, 16);
            this.lblRuta.TabIndex = 2;
            this.lblRuta.Text = "Ruta del Proyecto:";
            // 
            // btnExplorar
            // 
            this.btnExplorar.Location = new System.Drawing.Point(602, 33);
            this.btnExplorar.Name = "btnExplorar";
            this.btnExplorar.Size = new System.Drawing.Size(97, 23);
            this.btnExplorar.TabIndex = 3;
            this.btnExplorar.Text = "Explorar";
            this.btnExplorar.UseVisualStyleBackColor = true;
            this.btnExplorar.Click += new System.EventHandler(this.btnExplorar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(707, 33);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(97, 23);
            this.btnLimpiar.TabIndex = 4;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnAnalizar
            // 
            this.btnAnalizar.Location = new System.Drawing.Point(602, 78);
            this.btnAnalizar.Name = "btnAnalizar";
            this.btnAnalizar.Size = new System.Drawing.Size(202, 23);
            this.btnAnalizar.TabIndex = 5;
            this.btnAnalizar.Text = "Realizar Analisis";
            this.btnAnalizar.UseVisualStyleBackColor = true;
            this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);
            // 
            // trvArbol
            // 
            this.trvArbol.Location = new System.Drawing.Point(587, 153);
            this.trvArbol.Name = "trvArbol";
            this.trvArbol.Size = new System.Drawing.Size(221, 263);
            this.trvArbol.TabIndex = 6;
            // 
            // Principal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 450);
            this.Controls.Add(this.trvArbol);
            this.Controls.Add(this.btnAnalizar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnExplorar);
            this.Controls.Add(this.lblRuta);
            this.Controls.Add(this.txcRuta);
            this.Controls.Add(this.tabControl1);
            this.Name = "Principal";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox txcRuta;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.Button btnExplorar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnAnalizar;
        private System.Windows.Forms.TreeView trvArbol;
    }
}

