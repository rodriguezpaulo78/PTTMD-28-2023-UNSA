using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using PTTMD_28_2023_UNSA.CapaControlador;

namespace PTTMD_28_2023_UNSA
{
    public partial class CPInterfazReconocimiento : Form
    {
        public CPInterfazReconocimiento()
        {
            InitializeComponent();
            this.mxInitVar();
        }

        private void mxInitVar()
        {
            // Configurar las columnas del ListView
            lsvNivel1.View = View.Details;
            lsvNivel1.Columns.Add("Nombre del archivo", 200);
            lsvNivel1.Columns.Add("Tamaño (bytes)", 100);
            lsvNivel1.Columns.Add("Tipo de archivo", 100);
            lsvNivel1.Columns.Add("Fecha de creación", 150);
            lsvNivel1.Columns.Add("Ruta completa", 300);  // Nueva columna para la ruta
        }

        private void btnExplorar_Click(object sender, EventArgs e)
        {
            using (var dialogo = new FolderBrowserDialog())
            {
                if (dialogo.ShowDialog() == DialogResult.OK)
                {
                    txcRuta.Text = dialogo.SelectedPath;
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            this.mxLimpiarControles(this);
        }

        private void mxLimpiarControles(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox) ((TextBox)c).Clear();
                else if (c is ComboBox) ((ComboBox)c).SelectedIndex = -1;
                else if (c is CheckBox) ((CheckBox)c).Checked = false;
                else if (c is RadioButton) ((RadioButton)c).Checked = false;
                else if (c is DateTimePicker) ((DateTimePicker)c).Value = DateTime.Now;
                else if (c.HasChildren) mxLimpiarControles(c); // Recursivo para contenedores
            }
        }
        public void CargarArbolCodigo(string rutaProyecto, TreeView treeView)
        {
            treeView.Nodes.Clear();

            var archivos = Directory.GetFiles(rutaProyecto, "*.cs", SearchOption.AllDirectories);

            foreach (var archivo in archivos)
            {
                var codigo = File.ReadAllText(archivo);
                var arbolSintaxis = CSharpSyntaxTree.ParseText(codigo);
                var raiz = arbolSintaxis.GetRoot() as CompilationUnitSyntax;

                // Nodo raíz por archivo
                TreeNode nodoArchivo = new TreeNode(Path.GetFileName(archivo));

                var clases = raiz.DescendantNodes().OfType<ClassDeclarationSyntax>();
                foreach (var clase in clases)
                {
                    TreeNode nodoClase = new TreeNode("Clase: " + clase.Identifier.Text);

                    var metodos = clase.Members.OfType<MethodDeclarationSyntax>();
                    foreach (var metodo in metodos)
                    {
                        string firma = metodo.Identifier.Text + "(" + string.Join(", ", metodo.ParameterList.Parameters.Select(p => p.Type + " " + p.Identifier.Text)) + ")";
                        TreeNode nodoMetodo = new TreeNode("Método: " + firma);
                        nodoClase.Nodes.Add(nodoMetodo);
                    }

                    nodoArchivo.Nodes.Add(nodoClase);
                }

                treeView.Nodes.Add(nodoArchivo);
            }

            treeView.ExpandAll();
        }

        private void btnAnalizar_Click(object sender, EventArgs e)
        {
            CNCodigoFuente cNCodigoFuente = new CNCodigoFuente();

            CargarArbolCodigo(this.txcRuta.Text, trvArbol);
            this.mxRealizarAnalisisPrimerNivel();
            cNCodigoFuente.mxContarMetodos();
        }

        private void mxRealizarAnalisisPrimerNivel()
        {
            // Obtener la ruta seleccionada
            string rutaCarpeta = this.txcRuta.Text;

            // Limpiar el ListView antes de cargar nuevos archivos
            lsvNivel1.Items.Clear();

            // Obtener los archivos en la carpeta
            try
            {
                // Obtenemos los archivos en la ruta seleccionada
                string[] archivos = Directory.GetFiles(rutaCarpeta, "*.*", SearchOption.AllDirectories);


                foreach (string archivo in archivos)
                {
                    // Crear un nuevo ListViewItem para cada archivo
                    FileInfo fileInfo = new FileInfo(archivo);

                    // Crear un arreglo de columnas con las propiedades del archivo
                    string[] fila = {
                        fileInfo.Name,                 // Nombre del archivo
                        fileInfo.Length.ToString(),    // Tamaño en bytes
                        fileInfo.Extension,            // Tipo de archivo (extensión)
                        fileInfo.CreationTime.ToString(), // Fecha de creación
                        archivo                        // Ruta completa del archivo
                    };

                    // Agregar la fila al ListView
                    ListViewItem item = new ListViewItem(fila);
                    lsvNivel1.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los archivos: " + ex.Message);
            }
            
        }

        private void btnExportarNivel1_Click(object sender, EventArgs e)
        {
            // Crear un cuadro de diálogo para elegir la ubicación del archivo
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Archivo CSV (*.csv)|*.csv";
            saveFileDialog.DefaultExt = ".csv"; // Establecer la extensión por defecto

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Obtener la ruta del archivo elegido
                    string rutaArchivo = saveFileDialog.FileName;

                    // Crear el archivo y abrirlo para escritura con codificación UTF-8
                    using (StreamWriter writer = new StreamWriter(rutaArchivo, false, Encoding.UTF8))
                    {
                        // Escribir los encabezados de las columnas
                        writer.WriteLine("Nombre del archivo,Tamaño (bytes),Tipo de archivo,Fecha de creación,Ruta completa");

                        // Recorrer cada item del ListView y escribirlo en el archivo
                        foreach (ListViewItem item in lsvNivel1.Items)
                        {
                            // Crear una lista para almacenar cada valor con comillas dobles
                            List<string> valores = new List<string>();

                            // Recorrer las columnas de la fila y asegurarse de que estén entre comillas
                            foreach (ListViewItem.ListViewSubItem subItem in item.SubItems)
                            {
                                // Reemplazar comas y saltos de línea para evitar problemas en el CSV
                                string valor = subItem.Text.Replace("\"", "\"\""); // Escapar las comillas dobles
                                if (valor.Contains(",") || valor.Contains("\n") || valor.Contains("\r"))
                                {
                                    valor = $"\"{valor}\""; // Rodear con comillas dobles
                                }
                                valores.Add(valor);
                            }

                            // Escribir la línea separada por comas
                            writer.WriteLine(string.Join(",", valores));
                        }
                    }

                    MessageBox.Show("Los datos se han exportado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar el archivo: " + ex.Message);
                }
            }
        }
    }
}
