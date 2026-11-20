using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.Utilidades;

namespace PTTMD_28_2023_UNSA.CapaControlador
{
    /// <summary>
    /// Submodelo 1 (Doc 01, sección 4.3): análisis estructural de UNA versión.
    /// Recorre la carpeta, construye la jerarquía archivo -> clase -> método,
    /// calcula las métricas y genera los hash de cada nivel.
    /// </summary>
    public class CNAnalisisEstructural
    {
        /// <summary>Carpetas que no contienen código escrito por el equipo.</summary>
        public static readonly string[] paCarpetasExcluidas = { "bin", "obj", ".vs", ".git", "packages", "node_modules" };

        /// <summary>Si es verdadero, se omiten los archivos generados por herramientas (*.Designer.cs, *.g.cs).</summary>
        public bool plExcluirGenerados { get; set; }

        /// <summary>
        /// Archivos que no son código C# pero forman parte del proyecto (configuración, proyecto, recursos).
        /// Se incluyen en la exploración física: un cambio en ellos (por ejemplo, la cadena de conexión) también es una intervención.
        /// </summary>
        public static readonly string[] paExtensionesNoCodigo = { ".config", ".csproj", ".sln", ".resx", ".settings", ".json" };

        /// <summary>Si es verdadero, la exploración física incluye los archivos de paExtensionesNoCodigo.</summary>
        public bool plIncluirNoCodigo { get; set; }

        public CNAnalisisEstructural() { plExcluirGenerados = true; plIncluirNoCodigo = true; }

        /// <summary>Verdadero si el archivo es código fuente C#.</summary>
        public static bool mxEsCodigo(string tcNombreArchivo)
        {
            return string.Equals(Path.GetExtension(tcNombreArchivo), ".cs", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Firma estructural de una versión: SHA-256 de la ruta y el hash lógico de cada archivo, en orden.
        /// Se usa al analizar y también para verificar que una línea base guardada no fue alterada.
        /// </summary>
        public static string mxCalcularFirma(Evaluacion toEvaluacion)
        {
            var sb = new StringBuilder();
            foreach (var a in toEvaluacion.paArchivos.OrderBy(x => x.pcRuta, StringComparer.OrdinalIgnoreCase))
                sb.Append(a.pcRuta.ToLowerInvariant()).Append('|').Append(a.pcHashLogico).Append('\n');
            return Utilitarios.mxSha256(sb.ToString());
        }

        /// <summary>
        /// Verdadero si la firma guardada en una evaluación coincide con la que se recalcula a partir de sus archivos.
        /// Si es falso, la línea base fue editada después de generarse.
        /// </summary>
        public static bool mxVerificarFirma(Evaluacion toEvaluacion)
        {
            if (toEvaluacion == null || string.IsNullOrEmpty(toEvaluacion.pcFirmaEstructural)) return false;
            return string.Equals(mxCalcularFirma(toEvaluacion), toEvaluacion.pcFirmaEstructural, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Analiza una versión del proyecto. Acepta una carpeta o un archivo .zip;
        /// el .zip se descomprime en una carpeta temporal que se borra al terminar.
        /// </summary>
        public Evaluacion mxAnalizar(string tcRutaProyecto)
        {
            if (!string.IsNullOrWhiteSpace(tcRutaProyecto) && File.Exists(tcRutaProyecto) &&
                string.Equals(Path.GetExtension(tcRutaProyecto), ".zip", StringComparison.OrdinalIgnoreCase))
                return mxAnalizarZip(tcRutaProyecto);

            return mxAnalizarCarpeta(tcRutaProyecto);
        }

        private Evaluacion mxAnalizarZip(string tcRutaZip)
        {
            string temporal = Path.Combine(Path.GetTempPath(), "PTTMD_" + Guid.NewGuid().ToString("N"));
            try
            {
                ZipFile.ExtractToDirectory(tcRutaZip, temporal);

                // Si el .zip contiene una sola carpeta raíz, esa carpeta es el proyecto
                // (así las rutas relativas coinciden con las de un análisis por carpeta).
                string raiz = temporal;
                if (Directory.GetFiles(temporal).Length == 0 && Directory.GetDirectories(temporal).Length == 1)
                    raiz = Directory.GetDirectories(temporal)[0];

                Evaluacion evaluacion = mxAnalizarCarpeta(raiz);
                evaluacion.pcRuta = tcRutaZip;
                return evaluacion;
            }
            finally
            {
                try { if (Directory.Exists(temporal)) Directory.Delete(temporal, true); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        private Evaluacion mxAnalizarCarpeta(string tcRutaProyecto)
        {
            if (string.IsNullOrWhiteSpace(tcRutaProyecto) || !Directory.Exists(tcRutaProyecto))
                throw new DirectoryNotFoundException("No existe la carpeta o el archivo .zip: " + tcRutaProyecto);

            var reloj = Stopwatch.StartNew();
            var proyecto = new Evaluacion { pcRuta = tcRutaProyecto, pdFecEval = DateTime.Now };

            // Fase 1: análisis físico (qué archivos hay)
            foreach (string ruta in mxListarArchivos(tcRutaProyecto))
            {
                // Fase 2 y 3: árbol jerárquico y análisis lógico de cada archivo
                proyecto.paArchivos.Add(mxAnalizarArchivo(tcRutaProyecto, ruta));
            }
            proyecto.paArchivos = proyecto.paArchivos.OrderBy(a => a.pcRuta, StringComparer.OrdinalIgnoreCase).ToList();

            // Fase 4: métricas globales del proyecto
            proyecto.pnTotalArchivos = proyecto.paArchivos.Count;
            proyecto.pnTotalClases = proyecto.paArchivos.Sum(a => a.paClases.Count);
            // Las líneas de código cuentan solo archivos C#; los de configuración se registran aparte
            proyecto.pnTotalLineas = proyecto.paArchivos.Where(a => mxEsCodigo(a.pcNombre)).Sum(a => a.pnLineas);
            var metodos = proyecto.paArchivos.SelectMany(a => a.paClases).SelectMany(c => c.paMetodos).ToList();
            proyecto.pnTotalMetodos = metodos.Count;
            proyecto.pnComplejidadPromedio = metodos.Count == 0 ? 0 : Math.Round(metodos.Average(m => (double)m.pnComplejidad), 2);

            // Fase 5: firma estructural de la versión
            proyecto.pcFirmaEstructural = mxCalcularFirma(proyecto);

            reloj.Stop();
            proyecto.pnMilisegundos = reloj.ElapsedMilliseconds;
            return proyecto;
        }

        private IEnumerable<string> mxListarArchivos(string tcRaiz)
        {
            foreach (string ruta in Directory.GetFiles(tcRaiz, "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(ruta);
                bool esCodigo = string.Equals(extension, ".cs", StringComparison.OrdinalIgnoreCase);
                bool esNoCodigo = plIncluirNoCodigo && paExtensionesNoCodigo.Contains(extension, StringComparer.OrdinalIgnoreCase);
                if (!esCodigo && !esNoCodigo) continue;

                string relativa = Utilitarios.mxRutaRelativa(tcRaiz, ruta);
                string[] partes = relativa.Split('/');
                bool excluida = false;
                for (int i = 0; i < partes.Length - 1; i++)
                    if (paCarpetasExcluidas.Contains(partes[i], StringComparer.OrdinalIgnoreCase)) { excluida = true; break; }
                if (excluida) continue;

                if (plExcluirGenerados && esCodigo)
                {
                    string nombre = partes[partes.Length - 1];
                    if (nombre.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) ||
                        nombre.EndsWith(".g.cs", StringComparison.OrdinalIgnoreCase) ||
                        nombre.EndsWith(".g.i.cs", StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                yield return ruta;
            }
        }

        private Archivo mxAnalizarArchivo(string tcRaiz, string tcRuta)
        {
            if (!mxEsCodigo(tcRuta)) return mxAnalizarArchivoNoCodigo(tcRaiz, tcRuta);

            byte[] bytes = File.ReadAllBytes(tcRuta);
            string codigo = File.ReadAllText(tcRuta);
            var info = new FileInfo(tcRuta);
            SyntaxNode raizSintaxis = CSharpSyntaxTree.ParseText(codigo).GetRoot();

            var archivo = new Archivo
            {
                pcNombre = info.Name,
                pcRuta = Utilitarios.mxRutaRelativa(tcRaiz, tcRuta),
                pnTamano = info.Length,
                pdFecModificacion = info.LastWriteTime,
                pnLineas = codigo.Split('\n').Count(l => !string.IsNullOrWhiteSpace(l)),
                pcHashFisico = Utilitarios.mxSha256(bytes),
                pcHashLogico = Utilitarios.mxSha256(mxTextoLogico(raizSintaxis)),
                paLineasNormalizadas = mxLineasLogicas(raizSintaxis)
            };

            foreach (var declaracion in raizSintaxis.DescendantNodes().OfType<TypeDeclarationSyntax>())
                archivo.paClases.Add(mxAnalizarClase(declaracion, archivo.pcRuta));

            return archivo;
        }

        /// <summary>
        /// Análisis físico de un archivo que no es código C#: tamaño, fecha, líneas y hash.
        /// El hash lógico ignora espacios al inicio y al final de cada línea y las líneas vacías,
        /// para distinguir un cambio de formato de un cambio de contenido.
        /// </summary>
        private Archivo mxAnalizarArchivoNoCodigo(string tcRaiz, string tcRuta)
        {
            byte[] bytes = File.ReadAllBytes(tcRuta);
            string contenido = File.ReadAllText(tcRuta);
            var info = new FileInfo(tcRuta);
            List<string> lineas = contenido.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
            return new Archivo
            {
                pcNombre = info.Name,
                pcRuta = Utilitarios.mxRutaRelativa(tcRaiz, tcRuta),
                pnTamano = info.Length,
                pdFecModificacion = info.LastWriteTime,
                pnLineas = lineas.Count,
                pcHashFisico = Utilitarios.mxSha256(bytes),
                pcHashLogico = Utilitarios.mxSha256(string.Join("\n", lineas)),
                paLineasNormalizadas = lineas
            };
        }

        private Clase mxAnalizarClase(TypeDeclarationSyntax toTipo, string tcRutaArchivo)
        {
            var span = toTipo.GetLocation().GetLineSpan();
            var clase = new Clase
            {
                pcEspacioNombres = mxEspacioNombres(toTipo),
                pdFecAnalisis = DateTime.Now,
                pcNomCla = toTipo.Identifier.Text,
                pcNombreCompleto = mxNombreCompleto(toTipo),
                pcTipo = toTipo.Keyword.Text,
                pcAcceso = mxAcceso(toTipo.Modifiers, toTipo.Parent is TypeDeclarationSyntax ? "private" : "internal"),
                pcClaseBase = toTipo.BaseList == null ? "" : string.Join(", ", toTipo.BaseList.Types.Select(t => t.Type.ToString())),
                pcRutaArchivo = tcRutaArchivo,
                pnAtributos = toTipo.Members.OfType<FieldDeclarationSyntax>().Sum(f => f.Declaration.Variables.Count),
                pnPropiedades = toTipo.Members.OfType<PropertyDeclarationSyntax>().Count(),
                pnLineas = span.EndLinePosition.Line - span.StartLinePosition.Line + 1,
                pcHash = Utilitarios.mxSha256(mxTextoLogico(toTipo)),
                paLineasNormalizadas = mxLineasLogicas(toTipo)
            };

            // Solo los miembros directos: los de una clase anidada pertenecen a esa clase.
            foreach (var m in toTipo.Members.OfType<BaseMethodDeclarationSyntax>())
                clase.paMetodos.Add(mxAnalizarMetodo(m, clase.pcNomCla));

            return clase;
        }

        private Metodo mxAnalizarMetodo(BaseMethodDeclarationSyntax toMetodo, string tcNombreClase)
        {
            string nombre;
            string retorno = "";
            var metodo = toMetodo as MethodDeclarationSyntax;
            if (metodo != null) { nombre = metodo.Identifier.Text; retorno = metodo.ReturnType.ToString(); }
            else if (toMetodo is ConstructorDeclarationSyntax) nombre = ".ctor";
            else if (toMetodo is DestructorDeclarationSyntax) nombre = ".dtor";
            else if (toMetodo is OperatorDeclarationSyntax) { nombre = "operator" + ((OperatorDeclarationSyntax)toMetodo).OperatorToken.Text; retorno = ((OperatorDeclarationSyntax)toMetodo).ReturnType.ToString(); }
            else if (toMetodo is ConversionOperatorDeclarationSyntax) { nombre = "conversion " + ((ConversionOperatorDeclarationSyntax)toMetodo).Type; retorno = ((ConversionOperatorDeclarationSyntax)toMetodo).Type.ToString(); }
            else nombre = toMetodo.Kind().ToString();

            var tiposParametros = toMetodo.ParameterList.Parameters.Select(p => p.Type == null ? "?" : p.Type.ToString()).ToList();
            SyntaxNode cuerpo = (SyntaxNode)toMetodo.Body ?? toMetodo.ExpressionBody;
            var span = toMetodo.GetLocation().GetLineSpan();

            return new Metodo
            {
                pcNomMet = nombre,
                pcFirma = nombre + "(" + string.Join(", ", tiposParametros) + ")",
                pcTipoRetorno = retorno,
                pcAcceso = mxAcceso(toMetodo.Modifiers, "private"),
                pnParametros = tiposParametros.Count,
                pnCantLineas = span.EndLinePosition.Line - span.StartLinePosition.Line + 1,
                pnComplejidad = mxComplejidadCiclomatica(cuerpo),
                pnLlamadas = cuerpo == null ? 0 : cuerpo.DescendantNodes().OfType<InvocationExpressionSyntax>().Count(),
                pnTryCatch = cuerpo == null ? 0 : cuerpo.DescendantNodes().OfType<TryStatementSyntax>().Count(),
                pnInstrucciones = cuerpo == null ? 0 : cuerpo.DescendantNodes().OfType<StatementSyntax>().Count(s => !(s is BlockSyntax)),
                pcHash = Utilitarios.mxSha256(cuerpo == null ? "" : mxTextoLogico(cuerpo)),
                paLineasCuerpo = cuerpo == null ? new List<string>() : mxLineasLogicas(cuerpo)
            };
        }

        /// <summary>
        /// Complejidad ciclomática de McCabe: 1 + número de puntos de decisión
        /// (if, bucles, case, catch, operador condicional, &amp;&amp;, || y ??).
        /// </summary>
        public static int mxComplejidadCiclomatica(SyntaxNode toCuerpo)
        {
            if (toCuerpo == null) return 1;
            int decisiones = 0;
            foreach (var n in toCuerpo.DescendantNodes())
            {
                switch (n.Kind())
                {
                    case SyntaxKind.IfStatement:
                    case SyntaxKind.ForStatement:
                    case SyntaxKind.ForEachStatement:
                    case SyntaxKind.ForEachVariableStatement:
                    case SyntaxKind.WhileStatement:
                    case SyntaxKind.DoStatement:
                    case SyntaxKind.CaseSwitchLabel:
                    case SyntaxKind.CasePatternSwitchLabel:
                    case SyntaxKind.SwitchExpressionArm:
                    case SyntaxKind.CatchClause:
                    case SyntaxKind.ConditionalExpression:
                    case SyntaxKind.LogicalAndExpression:
                    case SyntaxKind.LogicalOrExpression:
                    case SyntaxKind.CoalesceExpression:
                        decisiones++;
                        break;
                }
            }
            return 1 + decisiones;
        }

        /// <summary>Código sin espacios, saltos de línea ni comentarios: solo los símbolos del lenguaje.</summary>
        private static string mxTextoLogico(SyntaxNode toNodo)
        {
            var sb = new StringBuilder();
            foreach (var token in toNodo.DescendantTokens()) sb.Append(token.Text).Append(' ');
            return sb.ToString();
        }

        /// <summary>Igual que TextoLogico, pero conservando la división en líneas del código original.</summary>
        private static List<string> mxLineasLogicas(SyntaxNode toNodo)
        {
            var lineas = new List<string>();
            int lineaActual = -1;
            StringBuilder sb = null;
            foreach (var token in toNodo.DescendantTokens())
            {
                if (token.Text.Length == 0) continue;
                int linea = token.GetLocation().GetLineSpan().StartLinePosition.Line;
                if (linea != lineaActual)
                {
                    if (sb != null) lineas.Add(sb.ToString());
                    sb = new StringBuilder();
                    lineaActual = linea;
                }
                else sb.Append(' ');
                sb.Append(token.Text);
            }
            if (sb != null) lineas.Add(sb.ToString());
            return lineas;
        }

        private static string mxEspacioNombres(TypeDeclarationSyntax toTipo)
        {
            for (SyntaxNode p = toTipo.Parent; p != null; p = p.Parent)
            {
                var ns = p as BaseNamespaceDeclarationSyntax;
                if (ns != null) return ns.Name.ToString();
            }
            return "";
        }

        private static string mxNombreCompleto(TypeDeclarationSyntax toTipo)
        {
            var partes = new List<string> { toTipo.Identifier.Text };
            for (SyntaxNode p = toTipo.Parent; p != null; p = p.Parent)
            {
                var t = p as TypeDeclarationSyntax;
                if (t != null) { partes.Insert(0, t.Identifier.Text); continue; }
                var ns = p as BaseNamespaceDeclarationSyntax;
                if (ns != null) partes.Insert(0, ns.Name.ToString());
            }
            return string.Join(".", partes);
        }

        private static string mxAcceso(SyntaxTokenList toModificadores, string tcPorDefecto)
        {
            var acceso = toModificadores
                .Where(m => m.IsKind(SyntaxKind.PublicKeyword) || m.IsKind(SyntaxKind.PrivateKeyword) ||
                            m.IsKind(SyntaxKind.ProtectedKeyword) || m.IsKind(SyntaxKind.InternalKeyword))
                .Select(m => m.Text).ToList();
            return acceso.Count == 0 ? tcPorDefecto : string.Join(" ", acceso);
        }
    }
}
