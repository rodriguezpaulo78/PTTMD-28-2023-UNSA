using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PTTMD_28_2023_UNSA.CapaControlador
{
    internal class CNCodigoFuente
    {
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
