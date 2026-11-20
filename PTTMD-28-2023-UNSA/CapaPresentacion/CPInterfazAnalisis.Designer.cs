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
            this.lblRutaBase = new System.Windows.Forms.Label();
            this.txcRutaBase = new System.Windows.Forms.TextBox();
            this.btnExplorarBase = new System.Windows.Forms.Button();
            this.lblRutaNueva = new System.Windows.Forms.Label();
            this.txcRutaNueva = new System.Windows.Forms.TextBox();
            this.btnExplorarNueva = new System.Windows.Forms.Button();
            this.btnAnalizar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.btnExportar = new System.Windows.Forms.Button();
            this.lblArbol = new System.Windows.Forms.Label();
            this.trvArbol = new System.Windows.Forms.TreeView();
            this.lblArbolNueva = new System.Windows.Forms.Label();
            this.trvArbolNueva = new System.Windows.Forms.TreeView();
            this.lblResumen = new System.Windows.Forms.Label();
            this.txcResumen = new System.Windows.Forms.TextBox();
            this.lblFiltro = new System.Windows.Forms.Label();
            this.cboFiltro = new System.Windows.Forms.ComboBox();
            this.dgvAnomalias = new System.Windows.Forms.DataGridView();
            this.colSeveridad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNivel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCambio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colArchivo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colElemento = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMetrica = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBase = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNueva = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVariacion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnomalias)).BeginInit();
            this.SuspendLayout();
            // 
            // lblRutaBase
            // 
            this.lblRutaBase.AutoSize = true;
            this.lblRutaBase.Location = new System.Drawing.Point(12, 16);
            this.lblRutaBase.Name = "lblRutaBase";
            this.lblRutaBase.Size = new System.Drawing.Size(231, 16);
            this.lblRutaBase.TabIndex = 0;
            this.lblRutaBase.Text = "Versión base (cierre registrado):";
            // 
            // txcRutaBase
            // 
            this.txcRutaBase.Location = new System.Drawing.Point(12, 37);
            this.txcRutaBase.Name = "txcRutaBase";
            this.txcRutaBase.Size = new System.Drawing.Size(548, 22);
            this.txcRutaBase.TabIndex = 1;
            // 
            // btnExplorarBase
            // 
            this.btnExplorarBase.Location = new System.Drawing.Point(582, 36);
            this.btnExplorarBase.Name = "btnExplorarBase";
            this.btnExplorarBase.Size = new System.Drawing.Size(97, 23);
            this.btnExplorarBase.TabIndex = 2;
            this.btnExplorarBase.Text = "Explorar";
            this.btnExplorarBase.UseVisualStyleBackColor = true;
            this.btnExplorarBase.Click += new System.EventHandler(this.btnExplorarBase_Click);
            // 
            // lblRutaNueva
            // 
            this.lblRutaNueva.AutoSize = true;
            this.lblRutaNueva.Location = new System.Drawing.Point(12, 72);
            this.lblRutaNueva.Name = "lblRutaNueva";
            this.lblRutaNueva.Size = new System.Drawing.Size(126, 16);
            this.lblRutaNueva.TabIndex = 3;
            this.lblRutaNueva.Text = "Versión a validar:";
            // 
            // txcRutaNueva
            // 
            this.txcRutaNueva.Location = new System.Drawing.Point(12, 93);
            this.txcRutaNueva.Name = "txcRutaNueva";
            this.txcRutaNueva.Size = new System.Drawing.Size(548, 22);
            this.txcRutaNueva.TabIndex = 4;
            // 
            // btnExplorarNueva
            // 
            this.btnExplorarNueva.Location = new System.Drawing.Point(582, 92);
            this.btnExplorarNueva.Name = "btnExplorarNueva";
            this.btnExplorarNueva.Size = new System.Drawing.Size(97, 23);
            this.btnExplorarNueva.TabIndex = 5;
            this.btnExplorarNueva.Text = "Explorar";
            this.btnExplorarNueva.UseVisualStyleBackColor = true;
            this.btnExplorarNueva.Click += new System.EventHandler(this.btnExplorarNueva_Click);
            // 
            // btnAnalizar
            // 
            this.btnAnalizar.Location = new System.Drawing.Point(697, 36);
            this.btnAnalizar.Name = "btnAnalizar";
            this.btnAnalizar.Size = new System.Drawing.Size(170, 79);
            this.btnAnalizar.TabIndex = 6;
            this.btnAnalizar.Text = "Realizar Análisis";
            this.btnAnalizar.UseVisualStyleBackColor = true;
            this.btnAnalizar.Click += new System.EventHandler(this.btnAnalizar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.Location = new System.Drawing.Point(882, 36);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(97, 23);
            this.btnLimpiar.TabIndex = 7;
            this.btnLimpiar.Text = "Limpiar";
            this.btnLimpiar.UseVisualStyleBackColor = true;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // btnExportar
            // 
            this.btnExportar.Enabled = false;
            this.btnExportar.Location = new System.Drawing.Point(882, 92);
            this.btnExportar.Name = "btnExportar";
            this.btnExportar.Size = new System.Drawing.Size(97, 23);
            this.btnExportar.TabIndex = 8;
            this.btnExportar.Text = "Exportar";
            this.btnExportar.UseVisualStyleBackColor = true;
            this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);
            // 
            // lblArbol
            // 
            this.lblArbol.AutoSize = true;
            this.lblArbol.Location = new System.Drawing.Point(15, 130);
            this.lblArbol.Name = "lblArbol";
            this.lblArbol.Size = new System.Drawing.Size(168, 16);
            this.lblArbol.TabIndex = 9;
            this.lblArbol.Text = "Árbol de la versión base";
            // 
            // trvArbol
            // 
            this.trvArbol.Location = new System.Drawing.Point(15, 152);
            this.trvArbol.Name = "trvArbol";
            this.trvArbol.Size = new System.Drawing.Size(255, 190);
            this.trvArbol.TabIndex = 10;
            // 
            // lblArbolNueva
            // 
            this.lblArbolNueva.AutoSize = true;
            this.lblArbolNueva.Location = new System.Drawing.Point(282, 130);
            this.lblArbolNueva.Name = "lblArbolNueva";
            this.lblArbolNueva.Size = new System.Drawing.Size(203, 16);
            this.lblArbolNueva.TabIndex = 11;
            this.lblArbolNueva.Text = "Árbol de la versión a validar";
            // 
            // trvArbolNueva
            // 
            this.trvArbolNueva.Location = new System.Drawing.Point(282, 152);
            this.trvArbolNueva.Name = "trvArbolNueva";
            this.trvArbolNueva.Size = new System.Drawing.Size(255, 190);
            this.trvArbolNueva.TabIndex = 12;
            // 
            // lblResumen
            // 
            this.lblResumen.AutoSize = true;
            this.lblResumen.Location = new System.Drawing.Point(550, 130);
            this.lblResumen.Name = "lblResumen";
            this.lblResumen.Size = new System.Drawing.Size(175, 16);
            this.lblResumen.TabIndex = 13;
            this.lblResumen.Text = "Resumen de la comparación";
            // 
            // txcResumen
            // 
            this.txcResumen.Multiline = true;
            this.txcResumen.ReadOnly = true;
            this.txcResumen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txcResumen.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.txcResumen.BackColor = System.Drawing.Color.White;
            this.txcResumen.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txcResumen.Location = new System.Drawing.Point(550, 152);
            this.txcResumen.Name = "txcResumen";
            this.txcResumen.Size = new System.Drawing.Size(535, 190);
            this.txcResumen.TabIndex = 14;
            // 
            // lblFiltro
            // 
            this.lblFiltro.AutoSize = true;
            this.lblFiltro.Location = new System.Drawing.Point(15, 356);
            this.lblFiltro.Name = "lblFiltro";
            this.lblFiltro.Size = new System.Drawing.Size(60, 16);
            this.lblFiltro.TabIndex = 15;
            this.lblFiltro.Text = "Mostrar:";
            // 
            // cboFiltro
            // 
            this.cboFiltro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFiltro.FormattingEnabled = true;
            this.cboFiltro.Location = new System.Drawing.Point(80, 352);
            this.cboFiltro.Name = "cboFiltro";
            this.cboFiltro.Size = new System.Drawing.Size(220, 24);
            this.cboFiltro.TabIndex = 16;
            this.cboFiltro.SelectedIndexChanged += new System.EventHandler(this.cboFiltro_SelectedIndexChanged);
            // 
            // dgvAnomalias
            // 
            this.dgvAnomalias.AllowUserToAddRows = false;
            this.dgvAnomalias.AllowUserToDeleteRows = false;
            this.dgvAnomalias.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAnomalias.BackgroundColor = System.Drawing.Color.White;
            this.dgvAnomalias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAnomalias.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSeveridad,
            this.colNivel,
            this.colCambio,
            this.colArchivo,
            this.colElemento,
            this.colMetrica,
            this.colBase,
            this.colNueva,
            this.colVariacion,
            this.colDescripcion});
            this.dgvAnomalias.Location = new System.Drawing.Point(15, 384);
            this.dgvAnomalias.Name = "dgvAnomalias";
            this.dgvAnomalias.ReadOnly = true;
            this.dgvAnomalias.RowHeadersVisible = false;
            this.dgvAnomalias.RowHeadersWidth = 51;
            this.dgvAnomalias.RowTemplate.Height = 24;
            this.dgvAnomalias.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvAnomalias.Size = new System.Drawing.Size(1070, 318);
            this.dgvAnomalias.TabIndex = 17;
            // 
            // colSeveridad
            // 
            this.colSeveridad.HeaderText = "Severidad";
            this.colSeveridad.MinimumWidth = 6;
            this.colSeveridad.Name = "colSeveridad";
            this.colSeveridad.ReadOnly = true;
            this.colSeveridad.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSeveridad.Width = 80;
            // 
            // colNivel
            // 
            this.colNivel.HeaderText = "Nivel";
            this.colNivel.MinimumWidth = 6;
            this.colNivel.Name = "colNivel";
            this.colNivel.ReadOnly = true;
            this.colNivel.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNivel.Width = 70;
            // 
            // colCambio
            // 
            this.colCambio.HeaderText = "Cambio";
            this.colCambio.MinimumWidth = 6;
            this.colCambio.Name = "colCambio";
            this.colCambio.ReadOnly = true;
            this.colCambio.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colCambio.Width = 90;
            // 
            // colArchivo
            // 
            this.colArchivo.HeaderText = "Archivo";
            this.colArchivo.MinimumWidth = 6;
            this.colArchivo.Name = "colArchivo";
            this.colArchivo.ReadOnly = true;
            this.colArchivo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colArchivo.Width = 210;
            // 
            // colElemento
            // 
            this.colElemento.HeaderText = "Elemento";
            this.colElemento.MinimumWidth = 6;
            this.colElemento.Name = "colElemento";
            this.colElemento.ReadOnly = true;
            this.colElemento.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colElemento.Width = 220;
            // 
            // colMetrica
            // 
            this.colMetrica.HeaderText = "Métrica";
            this.colMetrica.MinimumWidth = 6;
            this.colMetrica.Name = "colMetrica";
            this.colMetrica.ReadOnly = true;
            this.colMetrica.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colMetrica.Width = 150;
            // 
            // colBase
            // 
            this.colBase.HeaderText = "Base";
            this.colBase.MinimumWidth = 6;
            this.colBase.Name = "colBase";
            this.colBase.ReadOnly = true;
            this.colBase.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colBase.Width = 90;
            // 
            // colNueva
            // 
            this.colNueva.HeaderText = "Nueva";
            this.colNueva.MinimumWidth = 6;
            this.colNueva.Name = "colNueva";
            this.colNueva.ReadOnly = true;
            this.colNueva.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colNueva.Width = 90;
            // 
            // colVariacion
            // 
            this.colVariacion.HeaderText = "Variación";
            this.colVariacion.MinimumWidth = 6;
            this.colVariacion.Name = "colVariacion";
            this.colVariacion.ReadOnly = true;
            this.colVariacion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colVariacion.Width = 100;
            // 
            // colDescripcion
            // 
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.MinimumWidth = 6;
            this.colDescripcion.Name = "colDescripcion";
            this.colDescripcion.ReadOnly = true;
            this.colDescripcion.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colDescripcion.Width = 520;
            // 
            // CPInterfazAnalisis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 715);
            this.Controls.Add(this.dgvAnomalias);
            this.Controls.Add(this.cboFiltro);
            this.Controls.Add(this.lblFiltro);
            this.Controls.Add(this.txcResumen);
            this.Controls.Add(this.lblResumen);
            this.Controls.Add(this.trvArbolNueva);
            this.Controls.Add(this.lblArbolNueva);
            this.Controls.Add(this.trvArbol);
            this.Controls.Add(this.lblArbol);
            this.Controls.Add(this.btnExportar);
            this.Controls.Add(this.btnLimpiar);
            this.Controls.Add(this.btnAnalizar);
            this.Controls.Add(this.btnExplorarNueva);
            this.Controls.Add(this.txcRutaNueva);
            this.Controls.Add(this.lblRutaNueva);
            this.Controls.Add(this.btnExplorarBase);
            this.Controls.Add(this.txcRutaBase);
            this.Controls.Add(this.lblRutaBase);
            this.MinimumSize = new System.Drawing.Size(1118, 600);
            this.Name = "CPInterfazAnalisis";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Validación de Cierre de Proyecto - Comparación de versiones";
            ((System.ComponentModel.ISupportInitialize)(this.dgvAnomalias)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblRutaBase;
        private System.Windows.Forms.TextBox txcRutaBase;
        private System.Windows.Forms.Button btnExplorarBase;
        private System.Windows.Forms.Label lblRutaNueva;
        private System.Windows.Forms.TextBox txcRutaNueva;
        private System.Windows.Forms.Button btnExplorarNueva;
        private System.Windows.Forms.Button btnAnalizar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.Button btnExportar;
        private System.Windows.Forms.Label lblArbol;
        private System.Windows.Forms.TreeView trvArbol;
        private System.Windows.Forms.Label lblArbolNueva;
        private System.Windows.Forms.TreeView trvArbolNueva;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.TextBox txcResumen;
        private System.Windows.Forms.Label lblFiltro;
        private System.Windows.Forms.ComboBox cboFiltro;
        private System.Windows.Forms.DataGridView dgvAnomalias;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSeveridad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNivel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCambio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArchivo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colElemento;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMetrica;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBase;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNueva;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVariacion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
    }
}