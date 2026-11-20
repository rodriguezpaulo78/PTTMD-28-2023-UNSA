using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PTTMD_28_2023_UNSA.CapaControlador;
using PTTMD_28_2023_UNSA.CapaDatos;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.Reportes;

namespace PTTMD_28_2023_UNSA
{
    /// <summary>
    /// Registro de cierre de proyecto: análisis estructural de UNA versión (Doc 01, sección 4.3).
    /// Nivel 1 = archivos (físico), Nivel 2 = clases (jerárquico), Nivel 3 = firmas hash (lógico).
    /// </summary>
    public partial class CPInterfazReconocimiento : Form
    {
        // Resultado del último análisis ejecutado
        private Evaluacion poEvaluacion;

        public CPInterfazReconocimiento()
        {
            InitializeComponent();
            this.mxInitVar();
        }

        private void mxInitVar()
        {
            this.Text = "Registro de Cierre de Proyecto";
            txcResponsable.Text = Environment.UserName;

            // Nivel 1: archivos
            lsvNivel1.View = View.Details;
            lsvNivel1.Columns.Add("Archivo", 220);
            lsvNivel1.Columns.Add("Tamaño (bytes)", 90);
            lsvNivel1.Columns.Add("Líneas", 60);
            lsvNivel1.Columns.Add("Clases", 60);
            lsvNivel1.Columns.Add("Última modificación", 130);
            lsvNivel1.Columns.Add("Hash físico (SHA-256)", 300);

            // Nivel 2: clases
            lsvNivel2.View = View.Details;
            lsvNivel2.Columns.Add("Clase", 150);
            lsvNivel2.Columns.Add("Métodos", 70);
            lsvNivel2.Columns.Add("Atributos", 70);
            lsvNivel2.Columns.Add("Propiedades", 80);
            lsvNivel2.Columns.Add("Líneas", 60);
            lsvNivel2.Columns.Add("Herencia", 120);
            lsvNivel2.Columns.Add("Archivo", 200);

            // Nivel 3: firma de cada clase y de cada método
            lsvNivel3.View = View.Details;
            lsvNivel3.Columns.Add("Clase", 130);
            lsvNivel3.Columns.Add("Método", 200);
            lsvNivel3.Columns.Add("Líneas", 55);
            lsvNivel3.Columns.Add("Complejidad", 80);
            lsvNivel3.Columns.Add("Hash lógico (SHA-256)", 300);
        }

        private void btnExplorar_Click(object sender, EventArgs e)
        {
            using (var dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = "Seleccione la carpeta raíz del proyecto";
                if (dialogo.ShowDialog(this) == DialogResult.OK) txcRuta.Text = dialogo.SelectedPath;
            }
        }

        private void btnAbrirZip_Click(object sender, EventArgs e)
        {
            using (var dialogo = new OpenFileDialog())
            {
                dialogo.Title = "Seleccione el proyecto comprimido";
                dialogo.Filter = "Proyecto comprimido (*.zip)|*.zip";
                if (dialogo.ShowDialog(this) == DialogResult.OK) txcRuta.Text = dialogo.FileName;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txcRuta.Clear();
            lsvNivel1.Items.Clear();
            lsvNivel2.Items.Clear();
            lsvNivel3.Items.Clear();
            trvArbol.Nodes.Clear();
            poEvaluacion = null;
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            bool esZip = File.Exists(txcRuta.Text) && Path.GetExtension(txcRuta.Text).Equals(".zip", StringComparison.OrdinalIgnoreCase);
            if (!Directory.Exists(txcRuta.Text) && !esZip)
            {
                MessageBox.Show(this, "Seleccione la carpeta del proyecto o un archivo .zip.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Cursor = Cursors.WaitCursor;
            try
            {
                poEvaluacion = new CNAnalisisEstructural().mxAnalizar(txcRuta.Text);
                this.mxMostrarNiveles();
                this.mxCargarArbol(poEvaluacion, trvArbol);
                MessageBox.Show(this,
                    "Archivos: " + poEvaluacion.pnTotalArchivos + "\r\nClases: " + poEvaluacion.pnTotalClases +
                    "\r\nMétodos: " + poEvaluacion.pnTotalMetodos + "\r\nLíneas de código: " + poEvaluacion.pnTotalLineas +
                    "\r\n\r\nFirma estructural:\r\n" + poEvaluacion.pcFirmaEstructural,
                    "Análisis completado", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo analizar el proyecto:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void mxMostrarNiveles()
        {
            lsvNivel1.Items.Clear();
            lsvNivel2.Items.Clear();
            lsvNivel3.Items.Clear();

            foreach (var archivo in poEvaluacion.paArchivos)
            {
                lsvNivel1.Items.Add(new ListViewItem(new[] {
                    archivo.pcRuta, archivo.pnTamano.ToString(), archivo.pnLineas.ToString(), archivo.paClases.Count.ToString(),
                    archivo.pdFecModificacion.ToString("yyyy-MM-dd HH:mm"), archivo.pcHashFisico }));

                foreach (var clase in archivo.paClases)
                {
                    lsvNivel2.Items.Add(new ListViewItem(new[] {
                        clase.pcNomCla, clase.paMetodos.Count.ToString(), clase.pnAtributos.ToString(), clase.pnPropiedades.ToString(),
                        clase.pnLineas.ToString(), clase.pcClaseBase, archivo.pcRuta }));

                    lsvNivel3.Items.Add(new ListViewItem(new[] { clase.pcNomCla, "(clase completa)", clase.pnLineas.ToString(), "", clase.pcHash }));
                    foreach (var metodo in clase.paMetodos)
                        lsvNivel3.Items.Add(new ListViewItem(new[] {
                            clase.pcNomCla, metodo.pcFirma, metodo.pnCantLineas.ToString(), metodo.pnComplejidad.ToString(), metodo.pcHash }));
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

        private void btnExportarNivel1_Click(object sender, EventArgs e)
        {
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Filter = "Archivo CSV (*.csv)|*.csv";
                dialogo.DefaultExt = ".csv";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(string.Join(";", lsvNivel1.Columns.Cast<ColumnHeader>().Select(c => c.Text)));
                    foreach (ListViewItem item in lsvNivel1.Items)
                        sb.AppendLine(string.Join(";", item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select(s => s.Text)));
                    // UTF-8 con BOM para que Excel muestre bien las tildes
                    File.WriteAllText(dialogo.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show(this, "Los datos se han exportado correctamente.", "Exportación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Error al exportar el archivo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (poEvaluacion == null)
            {
                MessageBox.Show(this, "Primero realice el análisis del proyecto.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El registro en base de datos se completará cuando se defina el esquema definitivo.
            try
            {
                var repo = new ProyectoMetodos();
                var nuevoProyecto = new CDProyecto
                {
                    Nombre = new DirectoryInfo(poEvaluacion.pcRuta).Name,
                    NumeroArchivos = poEvaluacion.pnTotalArchivos,
                    NumeroClases = poEvaluacion.pnTotalClases,
                    LineasCodigo = poEvaluacion.pnTotalLineas,
                    NumeroMetodos = poEvaluacion.pnTotalMetodos,
                    Ruta = poEvaluacion.pcRuta
                };
                repo.Insertar(nuevoProyecto);
                MessageBox.Show(this, "Proyecto registrado con éxito.", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudo registrar en la base de datos:\r\n" + ex.Message, "Base de datos no disponible", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Genera el documento de parámetros de cierre (HTML) y guarda junto a él la línea base (.lineabase.xml),
        /// que luego se carga en la pantalla de validación para verificar versiones posteriores.
        /// </summary>
        private void btnDocumentoCierre_Click(object sender, EventArgs e)
        {
            if (poEvaluacion == null)
            {
                MessageBox.Show(this, "Primero realice el análisis del proyecto.", "Faltan datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string lcProyecto = Path.GetFileNameWithoutExtension(poEvaluacion.pcRuta.TrimEnd('\\', '/'));
            using (var dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Guardar documento de parámetros de cierre";
                dialogo.Filter = "Documento HTML (*.html)|*.html";
                dialogo.FileName = "DocumentoCierre_" + lcProyecto + "_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".html";
                if (dialogo.ShowDialog(this) != DialogResult.OK) return;

                try
                {
                    poEvaluacion.pcResponsable = txcResponsable.Text.Trim();
                    poEvaluacion.pcNomVersion = lcProyecto;

                    // 1. Línea base junto al documento
                    string lcLineaBase = Path.Combine(Path.GetDirectoryName(dialogo.FileName),
                                                      Path.GetFileNameWithoutExtension(dialogo.FileName) + LineaBaseCD.pcExtension);
                    string lcHash = LineaBaseCD.mxGuardar(poEvaluacion, lcLineaBase);

                    // 2. Documento de parámetros de cierre
                    var datos = new CNDocumentoCierre.DatosEmision
                    {
                        pcProyecto = lcProyecto,
                        pcResponsable = txcResponsable.Text.Trim(),
                        pcCargo = txcCargo.Text.Trim(),
                        pcHashLineaBase = lcHash,
                        pcArchivoLineaBase = Path.GetFileName(lcLineaBase)
                    };
                    CNDocumentoCierre.mxGuardar(CNDocumentoCierre.mxGenerarHtml(poEvaluacion, datos, null), dialogo.FileName);

                    var respuesta = MessageBox.Show(this,
                        "Se generaron:\r\n- " + Path.GetFileName(dialogo.FileName) + "\r\n- " + Path.GetFileName(lcLineaBase) +
                        "\r\n\r\nCódigo de cierre: " + CNDocumentoCierre.mxCodigoCierre(lcProyecto, poEvaluacion) +
                        "\r\n\r\nGuarde la línea base: es la que se carga para validar el cierre.\r\n¿Desea abrir el documento?",
                        "Documento de cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (respuesta == DialogResult.Yes) Process.Start(dialogo.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "No se pudo generar el documento de cierre:\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
