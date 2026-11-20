using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using PTTMD_28_2023_UNSA.CapaControlador;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;

namespace PTTMD_28_2023_UNSA.Reportes
{
    /// <summary>
    /// Documento de parámetros de cierre (etapa 3 del procedimiento) e informe de verificación (etapas 5 y 7).
    /// Genera un documento HTML con las 9 secciones definidas en el informe del Hito 2.
    /// Se abre en cualquier navegador y se puede imprimir o guardar como PDF desde ahí,
    /// sin depender de librerías externas.
    /// </summary>
    public static class CNDocumentoCierre
    {
        private static readonly CultureInfo poCultura = CultureInfo.InvariantCulture;

        /// <summary>Datos de quien emite el documento.</summary>
        public class DatosEmision
        {
            public string pcProyecto { get; set; }
            public string pcResponsable { get; set; }
            public string pcCargo { get; set; }
            public string pcCorreo { get; set; }
            /// <summary>SHA-256 del archivo de línea base asociado (vacío si no se guardó).</summary>
            public string pcHashLineaBase { get; set; }
            /// <summary>Nombre del archivo de línea base asociado.</summary>
            public string pcArchivoLineaBase { get; set; }
        }

        /// <summary>
        /// Código único del cierre: PREFIJO-AAAA-CER-XXXXXXXX, donde XXXXXXXX son los primeros
        /// caracteres de la firma estructural. Dos cierres del mismo código tienen el mismo código.
        /// </summary>
        public static string mxCodigoCierre(string tcProyecto, Evaluacion toEvaluacion)
        {
            string primeraParte = (tcProyecto ?? "").Split(new[] { '-', ' ', '_', '.' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            string prefijo = new string(primeraParte.Where(char.IsLetterOrDigit).Take(8).ToArray()).ToUpperInvariant();
            if (prefijo.Length == 0) prefijo = "PRY";
            string firma = (toEvaluacion.pcFirmaEstructural ?? "").ToUpperInvariant();
            return prefijo + "-" + toEvaluacion.pdFecEval.ToString("yyyy", poCultura) + "-CER-" + (firma.Length >= 8 ? firma.Substring(0, 8) : firma);
        }

        /// <summary>
        /// Genera el documento. Si toComparacion es nulo, es un documento de cierre (primera línea base).
        /// Si no es nulo, es un informe de verificación: toEvaluacion es la versión verificada y
        /// las secciones 6 y 7 muestran las diferencias con el cierre registrado.
        /// </summary>
        public static string mxGenerarHtml(Evaluacion toEvaluacion, DatosEmision toDatos, Comparacion toComparacion)
        {
            if (toEvaluacion == null) throw new ArgumentNullException("toEvaluacion");
            if (toDatos == null) toDatos = new DatosEmision();

            bool esVerificacion = toComparacion != null;
            string titulo = esVerificacion ? "Informe de Verificación de Cierre" : "Documento de Parámetros de Cierre";
            string codigo = mxCodigoCierre(toDatos.pcProyecto, toEvaluacion);

            var archivosCodigo = toEvaluacion.paArchivos.Where(a => CNAnalisisEstructural.mxEsCodigo(a.pcNombre)).ToList();
            var archivosOtros = toEvaluacion.paArchivos.Where(a => !CNAnalisisEstructural.mxEsCodigo(a.pcNombre)).ToList();
            var metodos = toEvaluacion.paArchivos.SelectMany(a => a.paClases).SelectMany(c => c.paMetodos).ToList();
            DateTime? ultimaEdicion = toEvaluacion.paArchivos.Count == 0 ? (DateTime?)null : toEvaluacion.paArchivos.Max(a => a.pdFecModificacion);
            int profundidad = toEvaluacion.paArchivos.Count == 0 ? 0 : toEvaluacion.paArchivos.Max(a => a.pcRuta.Split('/').Length);
            int complejidadMaxima = metodos.Count == 0 ? 0 : metodos.Max(m => m.pnComplejidad);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html><html lang=\"es\"><head><meta charset=\"utf-8\">");
            sb.AppendLine("<title>" + H(titulo + " - " + codigo) + "</title>");
            sb.AppendLine("<style>" + mxEstilos() + "</style></head><body>");

            // 1. Encabezado
            sb.AppendLine("<header><h1>" + H(titulo) + "</h1>");
            sb.AppendLine("<h2>1. Encabezado</h2><table class=\"datos\">");
            mxFilaDato(sb, "Proyecto", toDatos.pcProyecto);
            mxFilaDato(sb, "Código único de cierre", codigo);
            mxFilaDato(sb, "Fecha del análisis", toEvaluacion.pdFecEval.ToString("dd/MM/yyyy HH:mm:ss", poCultura));
            mxFilaDato(sb, "Versión analizada", string.IsNullOrEmpty(toEvaluacion.pcNomVersion) ? toEvaluacion.pcRuta : toEvaluacion.pcNomVersion);
            mxFilaDato(sb, "Herramienta", "PTTMD-28-2023-UNSA — modelo de evaluación y reconocimiento de intervenciones");
            sb.AppendLine("</table></header>");

            // 2. Responsable
            sb.AppendLine("<h2>2. Responsable del análisis</h2><table class=\"datos\">");
            mxFilaDato(sb, "Nombre", toDatos.pcResponsable);
            mxFilaDato(sb, "Cargo", toDatos.pcCargo);
            mxFilaDato(sb, "Correo electrónico", toDatos.pcCorreo);
            sb.AppendLine("</table>");

            // 3. Resumen ejecutivo
            sb.AppendLine("<h2>3. Resumen ejecutivo</h2>");
            sb.AppendLine("<p>Se analizaron " + toEvaluacion.pnTotalArchivos + " archivos (" + archivosCodigo.Count + " de código C# y " + archivosOtros.Count +
                          " de configuración, proyecto o recursos), con " + toEvaluacion.pnTotalClases + " clases, " + toEvaluacion.pnTotalMetodos +
                          " métodos y " + toEvaluacion.pnTotalLineas + " líneas de código.</p>");
            if (!esVerificacion)
            {
                sb.AppendLine("<p>Este documento registra el estado del proyecto al momento del cierre. Su firma estructural (sección 9) " +
                              "es la referencia contra la que se verificarán las versiones posteriores.</p>");
            }
            else if (toComparacion.mxSonIdenticas())
            {
                sb.AppendLine("<p class=\"ok\"><b>Resultado de la verificación:</b> la versión verificada tiene la misma firma estructural que el cierre registrado. " +
                              "No se encontraron cambios en el código.</p>");
            }
            else
            {
                sb.AppendLine("<p class=\"alerta\"><b>Resultado de la verificación:</b> la versión verificada <b>no coincide</b> con el cierre registrado. Se encontraron " +
                              toComparacion.paAnomalias.Count + " hallazgos: " + toComparacion.mxContar(Severidad.Critica) + " críticos, " +
                              toComparacion.mxContar(Severidad.Moderada) + " moderados y " + toComparacion.mxContar(Severidad.Normal) + " normales.</p>");
                sb.AppendLine("<p>Los hallazgos señalan cambios estructurales que deben revisarse. La herramienta no determina la intención de un cambio: " +
                              "esa evaluación corresponde al responsable del proyecto.</p>");
            }

            // 4. Detalles del análisis
            sb.AppendLine("<h2>4. Detalles del análisis</h2><table class=\"datos\">");
            mxFilaDato(sb, "Ubicación analizada", toEvaluacion.pcRuta);
            mxFilaDato(sb, "Total de archivos analizados", toEvaluacion.pnTotalArchivos.ToString(poCultura));
            mxFilaDato(sb, "Archivos de código C#", archivosCodigo.Count.ToString(poCultura));
            mxFilaDato(sb, "Archivos de configuración, proyecto o recursos", archivosOtros.Count.ToString(poCultura));
            mxFilaDato(sb, "Fecha de última edición", ultimaEdicion.HasValue ? ultimaEdicion.Value.ToString("dd/MM/yyyy HH:mm:ss", poCultura) : "-");
            mxFilaDato(sb, "Tiempo de análisis", toEvaluacion.pnMilisegundos.ToString(poCultura) + " ms");
            sb.AppendLine("</table>");

            // 5. Métricas del código
            sb.AppendLine("<h2>5. Métricas del código</h2><table class=\"datos\">");
            mxFilaDato(sb, "Líneas de código (C#, sin líneas vacías)", toEvaluacion.pnTotalLineas.ToString(poCultura));
            mxFilaDato(sb, "Clases", toEvaluacion.pnTotalClases.ToString(poCultura));
            mxFilaDato(sb, "Métodos", toEvaluacion.pnTotalMetodos.ToString(poCultura));
            mxFilaDato(sb, "Complejidad ciclomática promedio por método", toEvaluacion.pnComplejidadPromedio.ToString("0.00", poCultura));
            mxFilaDato(sb, "Complejidad ciclomática máxima", complejidadMaxima.ToString(poCultura));
            mxFilaDato(sb, "Métodos con complejidad mayor que 10", metodos.Count(m => m.pnComplejidad > 10).ToString(poCultura));
            mxFilaDato(sb, "Profundidad estructural (niveles de carpetas)", profundidad.ToString(poCultura));
            sb.AppendLine("</table>");

            // 6. Resultados del análisis
            sb.AppendLine("<h2>6. Resultados del análisis</h2>");
            if (!esVerificacion)
            {
                sb.AppendLine("<p>No aplica: este es el registro del cierre y no se compara con una versión anterior.</p>");
            }
            else
            {
                sb.AppendLine("<p>Cambios respecto al cierre registrado (" + H(toComparacion.poEvaluacionBase.pcRuta) + ").</p>");
                sb.AppendLine("<table><tr><th>Nivel</th><th>Agregados</th><th>Eliminados</th><th>Modificados</th><th>Renombrados</th><th>Movidos</th></tr>");
                foreach (NivelElemento nivel in new[] { NivelElemento.Archivo, NivelElemento.Clase, NivelElemento.Metodo })
                {
                    var delNivel = toComparacion.paAnomalias.Where(h => h.pnNivel == nivel).ToList();
                    sb.AppendLine("<tr><td>" + nivel + "</td>" +
                                  mxCeldaConteo(delNivel, TipoCambio.Agregado) + mxCeldaConteo(delNivel, TipoCambio.Eliminado) +
                                  mxCeldaConteo(delNivel, TipoCambio.Modificado) + mxCeldaConteo(delNivel, TipoCambio.Renombrado) +
                                  mxCeldaConteo(delNivel, TipoCambio.Movido) + "</tr>");
                }
                sb.AppendLine("</table>");
                sb.AppendLine("<p class=\"nota\">Un mismo elemento puede contarse más de una vez si se evaluó en varias métricas.</p>");
            }

            // 7. Hallazgos críticos
            sb.AppendLine("<h2>7. Hallazgos críticos</h2>");
            if (!esVerificacion)
            {
                sb.AppendLine("<p>No aplica en el registro del cierre.</p>");
            }
            else
            {
                var relevantes = toComparacion.paAnomalias.Where(h => h.pnSeveridad != Severidad.Normal).ToList();
                if (relevantes.Count == 0)
                {
                    sb.AppendLine("<p>No hay hallazgos críticos ni moderados.</p>");
                }
                else
                {
                    sb.AppendLine("<table><tr><th>Severidad</th><th>Nivel</th><th>Cambio</th><th>Archivo</th><th>Elemento</th><th>Descripción</th></tr>");
                    foreach (var h in relevantes)
                        sb.AppendLine("<tr class=\"" + (h.pnSeveridad == Severidad.Critica ? "critica" : "moderada") + "\"><td>" +
                                      H(CNComparacionVersiones.mxTexto(h.pnSeveridad)) + "</td><td>" + h.pnNivel + "</td><td>" + h.pnCambio + "</td><td>" +
                                      H(h.pcArchivo) + "</td><td>" + H(h.pcElemento) + "</td><td>" + H(h.pcDescripcion) + "</td></tr>");
                    sb.AppendLine("</table>");
                }
            }

            // 8. Visualizaciones
            sb.AppendLine("<h2>8. Visualizaciones</h2>");
            sb.AppendLine("<h3>Archivos de código con más líneas</h3>");
            mxBarras(sb, archivosCodigo.OrderByDescending(a => a.pnLineas).Take(10).Select(a => new KeyValuePair<string, double>(a.pcRuta, a.pnLineas)).ToList(), "barra");
            sb.AppendLine("<h3>Distribución de la complejidad ciclomática de los métodos</h3>");
            mxBarras(sb, new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("1 a 5", metodos.Count(m => m.pnComplejidad <= 5)),
                new KeyValuePair<string, double>("6 a 10", metodos.Count(m => m.pnComplejidad > 5 && m.pnComplejidad <= 10)),
                new KeyValuePair<string, double>("11 a 15", metodos.Count(m => m.pnComplejidad > 10 && m.pnComplejidad <= 15)),
                new KeyValuePair<string, double>("más de 15", metodos.Count(m => m.pnComplejidad > 15)),
            }, "barra");
            if (esVerificacion && toComparacion.paAnomalias.Count > 0)
            {
                sb.AppendLine("<h3>Hallazgos por severidad</h3>");
                mxBarras(sb, new List<KeyValuePair<string, double>>
                {
                    new KeyValuePair<string, double>("Críticos", toComparacion.mxContar(Severidad.Critica)),
                    new KeyValuePair<string, double>("Moderados", toComparacion.mxContar(Severidad.Moderada)),
                    new KeyValuePair<string, double>("Normales", toComparacion.mxContar(Severidad.Normal)),
                }, "barra severidad");
            }

            // 9. Firma digital y código de verificación
            sb.AppendLine("<h2>9. Firma y código de verificación</h2><table class=\"datos\">");
            mxFilaDato(sb, "Firma estructural (SHA-256)", toEvaluacion.pcFirmaEstructural);
            mxFilaDato(sb, "Código único del cierre", codigo);
            if (!string.IsNullOrEmpty(toDatos.pcHashLineaBase))
            {
                mxFilaDato(sb, "Archivo de línea base", toDatos.pcArchivoLineaBase);
                mxFilaDato(sb, "SHA-256 del archivo de línea base", toDatos.pcHashLineaBase);
            }
            if (esVerificacion)
                mxFilaDato(sb, "Firma del cierre registrado", toComparacion.poEvaluacionBase.pcFirmaEstructural);
            sb.AppendLine("</table>");
            sb.AppendLine("<p class=\"nota\">La firma estructural resume el contenido lógico de todos los archivos analizados: cambia si se modifica " +
                          "el código o la configuración, pero no si solo cambian espacios o comentarios. Para verificar una versión posterior, " +
                          "cargue la línea base en «Validación de Cierre de Proyecto» y compare.</p>");

            // Anexo: inventario de archivos con su hash físico
            sb.AppendLine("<h2>Anexo. Inventario de archivos</h2>");
            sb.AppendLine("<table class=\"inventario\"><tr><th>Archivo</th><th>Bytes</th><th>Líneas</th><th>Clases</th><th>Hash físico (SHA-256)</th></tr>");
            foreach (var a in toEvaluacion.paArchivos)
                sb.AppendLine("<tr><td>" + H(a.pcRuta) + "</td><td class=\"num\">" + a.pnTamano.ToString(poCultura) + "</td><td class=\"num\">" +
                              a.pnLineas.ToString(poCultura) + "</td><td class=\"num\">" + a.paClases.Count.ToString(poCultura) + "</td><td class=\"hash\">" +
                              H(a.pcHashFisico) + "</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<footer>Generado el " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", poCultura) + " · " + H(codigo) + "</footer>");
            sb.AppendLine("</body></html>");
            return sb.ToString();
        }

        /// <summary>Guarda el documento en UTF-8.</summary>
        public static void mxGuardar(string tcHtml, string tcRuta)
        {
            File.WriteAllText(tcRuta, tcHtml, new UTF8Encoding(true));
        }

        // ---------------------------------------------------------------------

        private static string H(string tcTexto)
        {
            return WebUtility.HtmlEncode(tcTexto ?? "");
        }

        private static void mxFilaDato(StringBuilder sb, string tcEtiqueta, string tcValor)
        {
            sb.AppendLine("<tr><th>" + H(tcEtiqueta) + "</th><td>" + (string.IsNullOrEmpty(tcValor) ? "-" : H(tcValor)) + "</td></tr>");
        }

        private static string mxCeldaConteo(List<Anomalia> taHallazgos, TipoCambio tnCambio)
        {
            return "<td class=\"num\">" + taHallazgos.Count(h => h.pnCambio == tnCambio).ToString(poCultura) + "</td>";
        }

        private static void mxBarras(StringBuilder sb, List<KeyValuePair<string, double>> taDatos, string tcClase)
        {
            if (taDatos.Count == 0) { sb.AppendLine("<p>Sin datos.</p>"); return; }
            double maximo = Math.Max(1, taDatos.Max(d => d.Value));
            sb.AppendLine("<table class=\"grafico\">");
            foreach (var d in taDatos)
            {
                double ancho = Math.Round(100.0 * d.Value / maximo, 1);
                sb.AppendLine("<tr><td class=\"etiqueta\">" + H(d.Key) + "</td><td class=\"pista\"><div class=\"" + tcClase + "\" style=\"width:" +
                              ancho.ToString("0.#", poCultura) + "%\"></div></td><td class=\"num\">" + d.Value.ToString("0", poCultura) + "</td></tr>");
            }
            sb.AppendLine("</table>");
        }

        private static string mxEstilos()
        {
            return "body{font-family:Arial,Helvetica,sans-serif;font-size:13px;color:#222;max-width:960px;margin:24px auto;padding:0 16px}" +
                   "h1{font-size:22px;border-bottom:3px solid #1f4e79;padding-bottom:6px;color:#1f4e79}" +
                   "h2{font-size:16px;color:#1f4e79;margin-top:26px;border-bottom:1px solid #ccc;padding-bottom:3px}" +
                   "h3{font-size:13px;margin:14px 0 6px}" +
                   "table{border-collapse:collapse;width:100%;margin:6px 0}" +
                   "th,td{border:1px solid #ccc;padding:4px 6px;text-align:left;vertical-align:top}" +
                   "th{background:#eef3f8}" +
                   "table.datos th{width:38%}" +
                   "td.num{text-align:right;white-space:nowrap}" +
                   "td.hash{font-family:Consolas,monospace;font-size:11px;word-break:break-all}" +
                   "table.inventario{font-size:11px}" +
                   "table.grafico td{border:none;padding:2px 6px}" +
                   "td.etiqueta{width:34%;font-size:12px;word-break:break-all}" +
                   "td.pista{width:56%;background:#f4f4f4}" +
                   "div.barra{height:14px;background:#2e75b6}" +
                   "div.severidad{background:#c55a11}" +
                   "tr.critica td{background:#ffd6d6}" +
                   "tr.moderada td{background:#ffecc8}" +
                   "p.ok{background:#e2f0d9;padding:8px;border-left:4px solid #548235}" +
                   "p.alerta{background:#fde9e7;padding:8px;border-left:4px solid #c00000}" +
                   "p.nota{font-size:11px;color:#555}" +
                   "footer{margin-top:30px;font-size:11px;color:#777;border-top:1px solid #ccc;padding-top:6px}" +
                   "@media print{body{margin:0;max-width:none}h2{page-break-after:avoid}tr{page-break-inside:avoid}}";
        }
    }
}
