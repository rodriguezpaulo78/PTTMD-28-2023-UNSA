using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace PTTMD_28_2023_UNSA.CapaControlador
{
    public class CNCodigoFuente
    {
        public void mxRealizarAnalisisPrimerNivel(string tcRuta, ref ListView toLstView)
        {
            // Obtener la ruta seleccionada
            string rutaCarpeta = tcRuta;

            

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
                    toLstView.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los archivos: " + ex.Message);
            }

        }

        public void mxContarMetodos()
        {
            string path = "Ruta/Al/Archivo.cs"; // <- poné el path a tu archivo C#
            string codigo = File.ReadAllText(path);

            var arbol = CSharpSyntaxTree.ParseText(codigo);
            var raiz = arbol.GetRoot();

            //Propiedades
            var propiedades = raiz.DescendantNodes().OfType<PropertyDeclarationSyntax>();
            Console.WriteLine($"Propiedades: {propiedades.Count()}");

            //Metodos
            var metodos = raiz.DescendantNodes().OfType<MethodDeclarationSyntax>();
            Console.WriteLine($"Cantidad de métodos: {metodos.Count()}");

            foreach (var metodo in metodos)
            {
                Console.WriteLine($"Método: {metodo.Identifier.Text}()");
            }

            //Inicio Fin de cada metodo
            foreach (var metodo in metodos)
            {
                var span = metodo.GetLocation().GetLineSpan();
                int lineaInicio = span.StartLinePosition.Line + 1; // +1 porque las líneas empiezan en 0
                int lineaFin = span.EndLinePosition.Line + 1;

                Console.WriteLine($"Método: {metodo.Identifier.Text}()");
                Console.WriteLine($" - Comienza en la línea: {lineaInicio}");
                Console.WriteLine($" - Termina en la línea: {lineaFin}");
            }
        }

        public void mxContarLOCClase()
        {
            string path = "Ruta/Al/Archivo.cs"; // Reemplazá por tu ruta
            string codigo = File.ReadAllText(path);

            var arbol = CSharpSyntaxTree.ParseText(codigo);
            var raiz = arbol.GetRoot();

            // Total líneas del archivo
            int totalLineas = codigo.Split('\n').Length;
            Console.WriteLine($"Líneas totales del archivo: {totalLineas}");
         
        }

        public void mxContarASCII()
        {
            string ruta = "Ruta/Al/Archivo.cs"; // Cambiá esto por tu archivo .cs
            string codigo = File.ReadAllText(ruta);

            int sumaAscii = 0;

            foreach (char c in codigo)
            {
                sumaAscii += (int)c;
            }

            Console.WriteLine($"Suma total de los valores ASCII del archivo: {sumaAscii}");
        }
    }
}
