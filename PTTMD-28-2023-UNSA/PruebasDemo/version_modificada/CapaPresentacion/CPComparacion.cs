using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PTTMD_28_2023_UNSA.Motor;

namespace PTTMD_28_2023_UNSA.CapaPresentacion
{
    /// <summary>
    /// Pantalla del prototipo: elegir dos versiones de un proyecto C#,
    /// compararlas con el motor y ver los hallazgos clasificados.
    /// Toda la lógica está en la carpeta Motor; esta pantalla solo la invoca y muestra el resultado.
    /// </summary>
    public class CPComparacion : Form
    {
        private readonly TextBox txtBase = new TextBox();
        private readonly TextBox txtNueva = new TextBox();
        private readonly Button btnBase = new Button();
        private readonly Button btnNueva = new Button();
        private readonly Button btnComparar = new Button();
        private readonly Button btnExportar = new Button();
        private readonly ComboBox cboFiltro = new ComboBox();
        private readonly TextBox txtResumen = new TextBox();
        private readonly DataGridView dgvHallazgos = new DataGridView();

        private ResultadoComparacion resultado;

        public CPComparacion()
        {
            Text = "PTTMD-28-2023-UNSA - Comparación estructural de versiones (prototipo)";
            Width = 1200;
            Height = 760;
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9F);
            ConstruirPantalla();
        }

        private void ConstruirPantalla()
        {
            var lblBase = new Label { Text = "Versión base:", Left = 12, Top = 16, Width = 90 };
            txtBase.SetBounds(105, 13, 880, 23);
            txtBase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnBase.Text = "Examinar...";
            btnBase.SetBounds(995, 12, 90, 25);
            btnBase.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBase.Click += (s, e) => ElegirCarpeta(txtBase);

            var lblNueva = new Label { Text = "Versión nueva:", Left = 12, Top = 47, Width = 90 };
            txtNueva.SetBounds(105, 44, 880, 23);
            txtNueva.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnNueva.Text = "Examinar...";
            btnNueva.SetBounds(995, 43, 90, 25);
            btnNueva.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNueva.Click += (s, e) => ElegirCarpeta(txtNueva);

            btnComparar.Text = "Comparar";
            btnComparar.SetBounds(1092, 12, 85, 56);
            btnComparar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnComparar.Click += (s, e) => Comparar();

            txtResumen.SetBounds(12, 78, 1165, 190);
            txtResumen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtResumen.Multiline = true;
            txtResumen.ReadOnly = true;
            txtResumen.ScrollBars = ScrollBars.Vertical;
            txtResumen.Font = new Font("Consolas", 9F);
            txtResumen.BackColor = Color.White;

            var lblFiltro = new Label { Text = "Mostrar:", Left = 12, Top = 281, Width = 55 };
            cboFiltro.SetBounds(70, 277, 200, 23);
            cboFiltro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFiltro.Items.AddRange(new object[] { "Todos los hallazgos", "Críticos y moderados", "Solo críticos" });
            cboFiltro.SelectedIndex = 0;
            cboFiltro.SelectedIndexChanged += (s, e) => MostrarHallazgos();

            btnExportar.Text = "Exportar CSV...";
            btnExportar.SetBounds(1067, 275, 110, 26);
            btnExportar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExportar.Enabled = false;
            btnExportar.Click += (s, e) => Exportar();

            dgvHallazgos.SetBounds(12, 308, 1165, 400);
            dgvHallazgos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvHallazgos.ReadOnly = true;
            dgvHallazgos.AllowUserToAddRows = false;
            dgvHallazgos.AllowUserToDeleteRows = false;
            dgvHallazgos.RowHeadersVisible = false;
            dgvHallazgos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHallazgos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvHallazgos.BackgroundColor = Color.White;
            AgregarColumna("Severidad", 75);
            AgregarColumna("Nivel", 65);
            AgregarColumna("Cambio", 85);
            AgregarColumna("Archivo", 210);
            AgregarColumna("Elemento", 220);
            AgregarColumna("Métrica", 150);
            AgregarColumna("Base", 90);
            AgregarColumna("Nueva", 90);
            AgregarColumna("Variación", 95);
            AgregarColumna("Descripción", 520);

            Controls.AddRange(new Control[] { lblBase, txtBase, btnBase, lblNueva, txtNueva, btnNueva, btnComparar,
                                              txtResumen, lblFiltro, cboFiltro, btnExportar, dgvHallazgos });
        }

        private void AgregarColumna(string titulo, int ancho)
        {
            dgvHallazgos.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = titulo, Width = ancho, SortMode = DataGridViewColumnSortMode.NotSortable });
        }

        private void ElegirCarpeta(TextBox destino)
        {
            using (var dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Seleccione la carpeta raíz de la versión del proyecto";
                if (Directory.Exists(destino.Text)) dialogo.SelectedPath = destino.Text;
                if (dialogo.ShowDialog(this) == DialogResult.OK) destino.Text = dialogo.SelectedPath;
            }
        }

        /// <summary>Lee las reglas desde reglas.cfg (junto al ejecutable). Si no existe, lo crea con los valores por defecto.</summary>
        private static ConjuntoReglas CargarReglas()
        {
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reglas.cfg");
            if (!File.Exists(ruta))
            {
                var porDefecto = ConjuntoReglas.PorDefecto();
                try { porDefecto.Guardar(ruta); } catch (IOException) { } catch (UnauthorizedAccessException) { }
                return porDefecto;
            }
            return ConjuntoReglas.Cargar(ruta);
        }

        private void Comparar()
        {
            if (!Directory.Exists(txtBase.Text) || !Directory.Exists(txtNueva.Text))
            {
                MessageBox.Show(this, "Seleccione las dos carpetas: la versión base y la versión nueva.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                var analizador = new AnalizadorEstructural();
                InfoProyecto vBase = analizador.Analizar(txtBase.Text);
                InfoProyecto vNueva = analizador.Analizar(txtNueva.Text);
                resultado = new ComparadorVersiones(CargarReglas()).Comparar(vBase, vNueva);

                txtResumen.Text = Exportador.Resumen(resultado).Replace("\r\n", "\n").Replace("\n", "\r\n");
                btnExportar.Enabled = true;
                MostrarHallazgos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo completar la comparación:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void MostrarHallazgos()
        {
            dgvHallazgos.Rows.Clear();
            if (resultado == null) return;

            IEnumerable<Hallazgo> lista = resultado.Hallazgos;
            if (cboFiltro.SelectedIndex == 1) lista = lista.Where(h => h.Severidad != Severidad.Normal);
            if (cboFiltro.SelectedIndex == 2) lista = lista.Where(h => h.Severidad == Severidad.Critica);

            foreach (var h in lista)
            {
                int i = dgvHallazgos.Rows.Add(ComparadorVersiones.Texto(h.Severidad), h.Nivel.ToString(), h.Cambio.ToString(),
                                              h.Archivo, h.Elemento, h.Metrica, h.ValorBase, h.ValorNuevo, h.Variacion, h.Descripcion);
                Color fondo = h.Severidad == Severidad.Critica ? Color.FromArgb(255, 214, 214)
                            : h.Severidad == Severidad.Moderada ? Color.FromArgb(255, 236, 200)
                            : Color.White;
                dgvHallazgos.Rows[i].DefaultCellStyle.BackColor = fondo;
            }
        }

        private void Exportar()
        {
            if (resultado == null) return;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo CSV (*.csv)|*.csv";
                dialogo.FileName = "hallazgos_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    Exportador.GuardarCsv(resultado, dialogo.FileName);
                    MessageBox.Show(this, "Hallazgos exportados.", "Exportación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo exportar:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
