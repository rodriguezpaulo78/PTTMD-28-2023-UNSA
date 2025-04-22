namespace PTTMD_28_2023_UNSA
{
    partial class CPInterfazReconocimiento
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
            this.tabNivel = new System.Windows.Forms.TabControl();
            this.tabNivel1 = new System.Windows.Forms.TabPage();
            this.lsvNivel1 = new System.Windows.Forms.ListView();
            this.btnExportarNivel1 = new System.Windows.Forms.Button();
            this.tabNivel2 = new System.Windows.Forms.TabPage();
            this.tabNivel3 = new System.Windows.Forms.TabPage();
            this.txcRuta = new System.Windows.Forms.TextBox();
            this.lblRuta = new System.Windows.Forms.Label();
            this.btnExplorar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.trvArbol = new System.Windows.Forms.TreeView();
            this.lblArbol = new System.Windows.Forms.Label();
            this.btnRegistrar = new System.Windows.Forms.Button();
            this.tabNivel.SuspendLayout();
            this.tabNivel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabNivel
            // 
            this.tabNivel.Controls.Add(this.tabNivel1);
            this.tabNivel.Controls.Add(this.tabNivel2);
            this.tabNivel.Controls.Add(this.tabNivel3);
            this.tabNivel.Location = new System.Drawing.Point(24, 83);
            this.tabNivel.Name = "tabNivel";
            this.tabNivel.SelectedIndex = 0;
            this.tabNivel.Size = new System.Drawing.Size(556, 292);
            this.tabNivel.TabIndex = 0;
            this.tabNivel.Tag = "";
            // 
            // tabNivel1
            // 
            this.tabNivel1.Controls.Add(this.lsvNivel1);
            this.tabNivel1.Controls.Add(this.btnExportarNivel1);
            this.tabNivel1.Location = new System.Drawing.Point(4, 25);
            this.tabNivel1.Name = "tabNivel1";
            this.tabNivel1.Padding = new System.Windows.Forms.Padding(3);
            this.tabNivel1.Size = new System.Drawing.Size(548, 263);
            this.tabNivel1.TabIndex = 0;
            this.tabNivel1.Text = "Nivel 1";
            this.tabNivel1.UseVisualStyleBackColor = true;
            // 
            // lsvNivel1
            // 
            this.lsvNivel1.HideSelection = false;
            this.lsvNivel1.Location = new System.Drawing.Point(20, 16);
            this.lsvNivel1.Name = "lsvNivel1";
            this.lsvNivel1.Size = new System.Drawing.Size(510, 199);
            this.lsvNivel1.TabIndex = 1;
            this.lsvNivel1.UseCompatibleStateImageBehavior = false;
            // 
            // btnExportarNivel1
            // 
            this.btnExportarNivel1.Location = new System.Drawing.Point(20, 221);
            this.btnExportarNivel1.Name = "btnExportarNivel1";
            this.btnExportarNivel1.Size = new System.Drawing.Size(75, 23);
            this.btnExportarNivel1.TabIndex = 0;
            this.btnExportarNivel1.Text = "Exportar";
            this.btnExportarNivel1.UseVisualStyleBackColor = true;
            this.btnExportarNivel1.Click += new System.EventHandler(this.btnExportarNivel1_Click);
            // 
            // tabNivel2
            // 
            this.tabNivel2.Location = new System.Drawing.Point(4, 25);
            this.tabNivel2.Name = "tabNivel2";
            this.tabNivel2.Padding = new System.Windows.Forms.Padding(3);
            this.tabNivel2.Size = new System.Drawing.Size(548, 263);
            this.tabNivel2.TabIndex = 1;
            this.tabNivel2.Text = "Nivel 2";
            this.tabNivel2.UseVisualStyleBackColor = true;
            // 
            // tabNivel3
            // 
            this.tabNivel3.Location = new System.Drawing.Point(4, 25);
            this.tabNivel3.Name = "tabNivel3";
            this.tabNivel3.Padding = new System.Windows.Forms.Padding(3);
            this.tabNivel3.Size = new System.Drawing.Size(548, 263);
            this.tabNivel3.TabIndex = 2;
            this.tabNivel3.Text = "Nivel 3";
            this.tabNivel3.UseVisualStyleBackColor = true;
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
            this.btnAnalizar.Location = new System.Drawing.Point(24, 393);
            this.btnAnalizar.Name = "btnAnalizar";
            this.btnAnalizar.Size = new System.Drawing.Size(202, 23);
            this.btnAnalizar.TabIndex = 5;
            this.btnAnalizar.Text = "Realizar Analisis";
            this.btnAnalizar.UseVisualStyleBackColor = true;
            this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);
            // 
            // trvArbol
            // 
            this.trvArbol.Location = new System.Drawing.Point(586, 108);
            this.trvArbol.Name = "trvArbol";
            this.trvArbol.Size = new System.Drawing.Size(221, 263);
            this.trvArbol.TabIndex = 6;
            // 
            // lblArbol
            // 
            this.lblArbol.AutoSize = true;
            this.lblArbol.Location = new System.Drawing.Point(586, 83);
            this.lblArbol.Name = "lblArbol";
            this.lblArbol.Size = new System.Drawing.Size(118, 16);
            this.lblArbol.TabIndex = 7;
            this.lblArbol.Text = "Arbol del Proyecto";
            // 
            // btnRegistrar
            // 
            this.btnRegistrar.Location = new System.Drawing.Point(259, 393);
            this.btnRegistrar.Name = "btnRegistrar";
            this.btnRegistrar.Size = new System.Drawing.Size(202, 23);
            this.btnRegistrar.TabIndex = 8;
            this.btnRegistrar.Text = "Registrar Proyecto";
            this.btnRegistrar.UseVisualStyleBackColor = true;
            this.btnRegistrar.Click += new System.EventHandler(this.btnRegistrar_Click);
            // 
            // CPInterfazReconocimiento
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 450);
            this.Controls.Add(this.btnRegistrar);
            this.Controls.Add(this.lblArbol);
            this.Controls.Add(this.trvArbol);
            this.Controls.Add(this.btnAnalizar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnExplorar);
            this.Controls.Add(this.lblRuta);
            this.Controls.Add(this.txcRuta);
            this.Controls.Add(this.tabNivel);
            this.Name = "CPInterfazReconocimiento";
            this.Text = "Form1";
            this.tabNivel.ResumeLayout(false);
            this.tabNivel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabNivel;
        private System.Windows.Forms.TabPage tabNivel1;
        private System.Windows.Forms.TabPage tabNivel2;
        private System.Windows.Forms.TextBox txcRuta;
        private System.Windows.Forms.Label lblRuta;
        private System.Windows.Forms.Button btnExplorar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnAnalizar;
        private System.Windows.Forms.TreeView trvArbol;
        private System.Windows.Forms.Label lblArbol;
        private System.Windows.Forms.TabPage tabNivel3;
        private System.Windows.Forms.Button btnExportarNivel1;
        private System.Windows.Forms.ListView lsvNivel1;
        private System.Windows.Forms.Button btnRegistrar;
    }
}

