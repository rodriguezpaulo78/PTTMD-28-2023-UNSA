using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PTTMD_28_2023_UNSA.CapaControlador;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;

namespace PTTMD_28_2023_UNSA.Reportes
{
    /// <summary>Salidas del motor: archivo CSV de hallazgos y resumen en texto.</summary>
    public static class CNReporteResultados
    {
        public static string mxTextoCambio(TipoCambio tnCambio)
        {
            return tnCambio.ToString();
        }

        public static void mxGuardarCsv(Comparacion toComparacion, string tcRuta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Severidad;Nivel;Cambio;Archivo;Elemento;Metrica;ValorBase;ValorNuevo;Variacion;Regla;Descripcion");
            foreach (var h in toComparacion.paAnomalias)
                sb.AppendLine(string.Join(";", new[] {
                    CNComparacionVersiones.mxTexto(h.pnSeveridad), h.pnNivel.ToString(), mxTextoCambio(h.pnCambio), h.pcArchivo, h.pcElemento,
                    h.pcMetrica, h.pcValorBase, h.pcValorNuevo, h.pcVariacion, h.pcRegla, h.pcDescripcion }.Select(mxCelda)));
            // UTF-8 con BOM para que Excel muestre bien las tildes
            File.WriteAllText(tcRuta, sb.ToString(), new UTF8Encoding(true));
        }

        private static string mxCelda(string tcValor)
        {
            tcValor = tcValor ?? "";
            if (tcValor.IndexOfAny(new[] { ';', '"', '\n', '\r' }) >= 0) return "\"" + tcValor.Replace("\"", "\"\"") + "\"";
            return tcValor;
        }

        /// <summary>Resumen de la comparación, con la estructura del documento de resultados (secciones 1 a 4).</summary>
        public static string mxResumen(Comparacion toComparacion)
        {
            var sb = new StringBuilder();
            var inv = CultureInfo.InvariantCulture;
            sb.AppendLine("RESULTADO DE LA COMPARACIÓN ESTRUCTURAL");
            sb.AppendLine("Fecha: " + toComparacion.pdFecCom.ToString("yyyy-MM-dd HH:mm"));
            sb.AppendLine();
            sb.AppendLine("Versión base : " + toComparacion.poEvaluacionBase.pcRuta);
            sb.AppendLine("  Firma      : " + toComparacion.poEvaluacionBase.pcFirmaEstructural);
            sb.AppendLine("Versión nueva: " + toComparacion.poEvaluacionNueva.pcRuta);
            sb.AppendLine("  Firma      : " + toComparacion.poEvaluacionNueva.pcFirmaEstructural);
            sb.AppendLine();
            sb.AppendLine("Métrica                    Base     Nueva");
            sb.AppendLine(mxFila("Archivos", toComparacion.poEvaluacionBase.pnTotalArchivos, toComparacion.poEvaluacionNueva.pnTotalArchivos));
            sb.AppendLine(mxFila("Clases", toComparacion.poEvaluacionBase.pnTotalClases, toComparacion.poEvaluacionNueva.pnTotalClases));
            sb.AppendLine(mxFila("Métodos", toComparacion.poEvaluacionBase.pnTotalMetodos, toComparacion.poEvaluacionNueva.pnTotalMetodos));
            sb.AppendLine(mxFila("Líneas de código", toComparacion.poEvaluacionBase.pnTotalLineas, toComparacion.poEvaluacionNueva.pnTotalLineas));
            sb.AppendLine("Complejidad promedio".PadRight(24) + toComparacion.poEvaluacionBase.pnComplejidadPromedio.ToString("0.00", inv).PadLeft(8) + toComparacion.poEvaluacionNueva.pnComplejidadPromedio.ToString("0.00", inv).PadLeft(10));
            sb.AppendLine();
            if (toComparacion.mxSonIdenticas())
            {
                sb.AppendLine("Las dos versiones tienen la misma firma estructural: no hay cambios en el código.");
                return sb.ToString();
            }
            sb.AppendLine("Anomalías detectadas: " + toComparacion.paAnomalias.Count +
                          "  (Críticos: " + toComparacion.mxContar(Severidad.Critica) +
                          ", Moderados: " + toComparacion.mxContar(Severidad.Moderada) +
                          ", Normales: " + toComparacion.mxContar(Severidad.Normal) + ")");
            foreach (var g in toComparacion.paAnomalias.GroupBy(h => h.pnNivel + " " + mxTextoCambio(h.pnCambio).ToLowerInvariant()).OrderBy(g => g.Key))
                sb.AppendLine("  " + g.Key + ": " + g.Count());
            sb.AppendLine();
            sb.AppendLine("Tiempo: análisis " + (toComparacion.poEvaluacionBase.pnMilisegundos + toComparacion.poEvaluacionNueva.pnMilisegundos) + " ms, comparación " + toComparacion.pnMilisegundos + " ms");
            return sb.ToString();
        }

        private static string mxFila(string tcNombre, int tnBase, int tnNueva)
        {
            return tcNombre.PadRight(24) + tnBase.ToString().PadLeft(8) + tnNueva.ToString().PadLeft(10);
        }
    }
}
