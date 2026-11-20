using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PTTMD_28_2023_UNSA.CapaControlador;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.Reportes;

namespace PTTMD_28_2023_UNSA.CapaPresentacion
{
    /// <summary>
    /// Validación de cierre de proyecto: compara la versión base con la versión a validar
    /// y muestra las anomalías clasificadas (Doc 01, sección 4.4).
    /// La pantalla solo invoca a los controladores y muestra el resultado.
    /// </summary>
    public partial class CPInterfazAnalisis : Form
    {
        // Resultado de la última comparación ejecutada
        private Comparacion poComparacion;

        public CPInterfazAnalisis()
        {
            InitializeComponent();
            this.mxInitVar();
        }

        private void mxInitVar()
        {
            cboFiltro.Items.AddRange(new object[] { "Todas las anomalías", "Críticas y moderadas", "Solo críticas" });
            cboFiltro.SelectedIndex = 0;
        }

        private void btnExplorarBase_Click(object sender, EventArgs e)
        {
            this.mxElegirCarpeta(txcRutaBase);
        }

        private void btnExplorarNueva_Click(object sender, EventArgs e)
        {
            this.mxElegirCarpeta(txcRutaNueva);
        }

        private void mxElegirCarpeta(TextBox toDestino)
        {
            using (var dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Seleccione la carpeta raíz de la versión del proyecto";
                if (Directory.Exists(toDestino.Text)) dialogo.SelectedPath = toDestino.Text;
                if (dialogo.ShowDialog(this) == DialogResult.OK) toDestino.Text = dialogo.SelectedPath;
            }
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txcRutaBase.Text) || !Directory.Exists(txcRutaNueva.Text))
            {
                MessageBox.Show(this, "Seleccione las dos carpetas: la versión base y la versión a validar.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                // 1. Análisis estructural de cada versión
                var analisis = new CNAnalisisEstructural();
                Evaluacion evaluacionBase = analisis.mxAnalizar(txcRutaBase.Text);
                Evaluacion evaluacionNueva = analisis.mxAnalizar(txcRutaNueva.Text);

                // 2. Comparación y clasificación con las reglas del modelo
                var comparador = new CNComparacionVersiones(CNReglasDeteccion.mxCargarDeAplicacion());
                poComparacion = comparador.mxComparar(evaluacionBase, evaluacionNueva);

                // 3. Presentación de resultados
                this.mxCargarArbol(evaluacionBase, trvArbol);
                this.mxCargarArbol(evaluacionNueva, trvArbolNueva);
                txcResumen.Text = CNReporteResultados.mxResumen(poComparacion).Replace("\r\n", "\n").Replace("\n", "\r\n");
                btnExportar.Enabled = true;
                this.mxMostrarAnomalias();
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

        // Muestra la jerarquía archivo -> clase -> método de una versión analizada
        private void mxCargarArbol(Evaluacion toEvaluacion, TreeView toArbol)
        {
            toArbol.BeginUpdate();
            toArbol.Nodes.Clear();
            foreach (var archivo in toEvaluacion.paArchivos)
            {
                TreeNode nodoArchivo = new TreeNode(archivo.pcRuta);
                foreach (var clase in archivo.paClases)
                {
                    TreeNode nodoClase = new TreeNode("Clase: " + clase.pcNomCla);
                    foreach (var metodo in clase.paMetodos)
                        nodoClase.Nodes.Add(new TreeNode("Método: " + metodo.pcFirma));
                    nodoArchivo.Nodes.Add(nodoClase);
                }
                toArbol.Nodes.Add(nodoArchivo);
            }
            toArbol.ExpandAll();
            if (toArbol.Nodes.Count > 0) toArbol.Nodes[0].EnsureVisible();
            toArbol.EndUpdate();
        }

        private void cboFiltro_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.mxMostrarAnomalias();
        }

        private void mxMostrarAnomalias()
        {
            dgvAnomalias.Rows.Clear();
            if (poComparacion == null) return;

            IEnumerable<Anomalia> lista = poComparacion.paAnomalias;
            if (cboFiltro.SelectedIndex == 1) lista = lista.Where(a => a.pnSeveridad != Severidad.Normal);
            if (cboFiltro.SelectedIndex == 2) lista = lista.Where(a => a.pnSeveridad == Severidad.Critica);

            foreach (var anomalia in lista)
            {
                int fila = dgvAnomalias.Rows.Add(anomalia.pcSeveridad, anomalia.pnNivel.ToString(), anomalia.pcTipoAnom,
                                                 anomalia.pcArchivo, anomalia.pcElemento, anomalia.pcMetrica,
                                                 anomalia.pcValorBase, anomalia.pcValorNuevo, anomalia.pcVariacion, anomalia.pcDescripcion);
                Color fondo = anomalia.pnSeveridad == Severidad.Critica ? Color.FromArgb(255, 214, 214)
                            : anomalia.pnSeveridad == Severidad.Moderada ? Color.FromArgb(255, 236, 200)
                            : Color.White;
                dgvAnomalias.Rows[fila].DefaultCellStyle.BackColor = fondo;
            }
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (poComparacion == null) return;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo CSV (*.csv)|*.csv";
                dialogo.FileName = "anomalias_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".csv";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    CNReporteResultados.mxGuardarCsv(poComparacion, dialogo.FileName);
                    MessageBox.Show(this, "Anomalías exportadas.", "Exportación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo exportar:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txcRutaBase.Clear();
            txcRutaNueva.Clear();
            txcResumen.Clear();
            trvArbol.Nodes.Clear();
            trvArbolNueva.Nodes.Clear();
            dgvAnomalias.Rows.Clear();
            poComparacion = null;
            btnExportar.Enabled = false;
        }
    }
}
