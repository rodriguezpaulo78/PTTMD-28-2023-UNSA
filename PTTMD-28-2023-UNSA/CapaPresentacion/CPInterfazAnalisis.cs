using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using PTTMD_28_2023_UNSA.CapaControlador;
using PTTMD_28_2023_UNSA.CapaDatos;
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

        // Datos de la línea base cargada (vacíos si la versión base se analizó desde una carpeta o un .zip)
        private string pcHashLineaBase = "";
        private bool plLineaBaseIntegra = true;

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

        // La versión base puede ser la línea base guardada al cerrar el proyecto (.lineabase.xml) o un .zip
        private void btnAbrirBase_Click(object sender, EventArgs e)
        {
            this.mxElegirArchivo(txcRutaBase, "Línea base del cierre (*.xml)|*.xml|Proyecto comprimido (*.zip)|*.zip");
        }

        private void btnAbrirNueva_Click(object sender, EventArgs e)
        {
            this.mxElegirArchivo(txcRutaNueva, "Proyecto comprimido (*.zip)|*.zip");
        }

        private void mxElegirArchivo(TextBox toDestino, string tcFiltro)
        {
            using (var dialogo = new OpenFileDialog())
            {
                dialogo.Filter = tcFiltro;
                if (dialogo.ShowDialog(this) == DialogResult.OK) toDestino.Text = dialogo.FileName;
            }
        }

        private static bool mxEsArchivo(string tcRuta, string tcExtension)
        {
            return File.Exists(tcRuta) && Path.GetExtension(tcRuta).Equals(tcExtension, StringComparison.OrdinalIgnoreCase);
        }

        private static bool mxRutaValida(string tcRuta, bool tlAceptaLineaBase)
        {
            return Directory.Exists(tcRuta) || mxEsArchivo(tcRuta, ".zip") || (tlAceptaLineaBase && mxEsArchivo(tcRuta, ".xml"));
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
            if (!mxRutaValida(txcRutaBase.Text, true) || !mxRutaValida(txcRutaNueva.Text, false))
            {
                MessageBox.Show(this, "Seleccione la versión base (carpeta, .zip o línea base .xml) y la versión a validar (carpeta o .zip).", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                // 1. Análisis estructural de cada versión
                var analisis = new CNAnalisisEstructural();
                Evaluacion evaluacionBase;
                if (mxEsArchivo(txcRutaBase.Text, ".xml"))
                {
                    // Etapa 5: verificación contra el cierre registrado
                    evaluacionBase = LineaBaseCD.mxCargar(txcRutaBase.Text);
                    pcHashLineaBase = LineaBaseCD.mxHashArchivo(txcRutaBase.Text);
                    plLineaBaseIntegra = CNAnalisisEstructural.mxVerificarFirma(evaluacionBase);
                    if (!plLineaBaseIntegra)
                        MessageBox.Show(this, "La línea base fue modificada después de generarse: su firma no coincide con su contenido.\r\n" +
                                        "Los resultados no son confiables.", "Línea base alterada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    evaluacionBase = analisis.mxAnalizar(txcRutaBase.Text);
                    pcHashLineaBase = "";
                    plLineaBaseIntegra = true;
                }
                Evaluacion evaluacionNueva = analisis.mxAnalizar(txcRutaNueva.Text);

                // 2. Comparación y clasificación con las reglas del modelo
                var comparador = new CNComparacionVersiones(CNReglasDeteccion.mxCargarDeAplicacion());
                poComparacion = comparador.mxComparar(evaluacionBase, evaluacionNueva);

                // 3. Presentación de resultados
                this.mxCargarArbol(evaluacionBase, trvArbol);
                this.mxCargarArbol(evaluacionNueva, trvArbolNueva);
                txcResumen.Text = (this.mxTextoVerificacion() + CNReporteResultados.mxResumen(poComparacion)).Replace("\r\n", "\n").Replace("\n", "\r\n");
                btnExportar.Enabled = true;
                btnInforme.Enabled = true;
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

        // Resultado de la verificación de cierre, al inicio del resumen
        private string mxTextoVerificacion()
        {
            if (poComparacion == null) return "";
            string lcTexto = poComparacion.mxSonIdenticas()
                ? "VERIFICACIÓN DE CIERRE: la versión coincide con el cierre registrado.\n"
                : "VERIFICACIÓN DE CIERRE: la versión NO coincide con el cierre registrado.\n";
            if (!string.IsNullOrEmpty(pcHashLineaBase))
                lcTexto += "Línea base: " + (plLineaBaseIntegra ? "íntegra" : "ALTERADA") + " (SHA-256 del archivo: " + pcHashLineaBase + ")\n";
            return lcTexto + "\n";
        }

        // Informe de verificación de cierre en HTML (etapas 5 y 7), con las mismas secciones del documento de cierre
        private void btnInforme_Click(object sender, EventArgs e)
        {
            if (poComparacion == null) return;
            string lcProyecto = string.IsNullOrEmpty(poComparacion.poEvaluacionBase.pcNomVersion)
                ? Path.GetFileNameWithoutExtension(poComparacion.poEvaluacionNueva.pcRuta.TrimEnd('\\', '/'))
                : poComparacion.poEvaluacionBase.pcNomVersion;
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Documento HTML (*.html)|*.html";
                dialogo.FileName = "InformeVerificacion_" + lcProyecto + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".html";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    var datos = new CNDocumentoCierre.DatosEmision
                    {
                        pcProyecto = lcProyecto,
                        pcResponsable = Environment.UserName,
                        pcHashLineaBase = pcHashLineaBase,
                        pcArchivoLineaBase = string.IsNullOrEmpty(pcHashLineaBase) ? "" : Path.GetFileName(txcRutaBase.Text)
                    };
                    CNDocumentoCierre.mxGuardar(CNDocumentoCierre.mxGenerarHtml(poComparacion.poEvaluacionNueva, datos, poComparacion), dialogo.FileName);
                    if (MessageBox.Show(this, "Informe generado. ¿Desea abrirlo?", "Informe", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(dialogo.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo generar el informe:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
            pcHashLineaBase = "";
            plLineaBaseIntegra = true;
            btnExportar.Enabled = false;
            btnInforme.Enabled = false;
        }
    }
}
