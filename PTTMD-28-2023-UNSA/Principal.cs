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

namespace PTTMD_28_2023_UNSA
{
    public partial class Principal : Form
    {
        public Principal()
        {
            InitializeComponent();
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
            CargarArbolCodigo(this.txcRuta.Text, trvArbol);
        }
    }
}
