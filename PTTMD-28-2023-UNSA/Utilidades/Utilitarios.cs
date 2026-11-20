using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PTTMD_28_2023_UNSA.Utilidades
{
    /// <summary>Funciones de apoyo: hash, rutas y similitud entre listas de líneas.</summary>
    public static class Utilitarios
    {
        public static string mxSha256(byte[] taDatos)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(taDatos);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public static string mxSha256(string tcTexto)
        {
            return mxSha256(Encoding.UTF8.GetBytes(tcTexto ?? string.Empty));
        }

        /// <summary>Ruta de un archivo relativa a la raíz, con '/' como separador.</summary>
        public static string mxRutaRelativa(string tcRaiz, string tcRuta)
        {
            string r = tcRaiz.TrimEnd('\\', '/');
            string rel = tcRuta.StartsWith(r, StringComparison.OrdinalIgnoreCase) ? tcRuta.Substring(r.Length) : tcRuta;
            return rel.TrimStart('\\', '/').Replace('\\', '/');
        }

        /// <summary>
        /// Longitud de la subsecuencia común más larga entre dos listas de líneas.
        /// Es la base para medir cuánto cambió un bloque de código.
        /// </summary>
        public static int mxSubsecuenciaComun(IList<string> taBase, IList<string> taNueva)
        {
            if (taBase.Count == 0 || taNueva.Count == 0) return 0;
            var anterior = new int[taNueva.Count + 1];
            var actual = new int[taNueva.Count + 1];
            for (int i = 1; i <= taBase.Count; i++)
            {
                for (int j = 1; j <= taNueva.Count; j++)
                {
                    if (taBase[i - 1] == taNueva[j - 1]) actual[j] = anterior[j - 1] + 1;
                    else actual[j] = Math.Max(anterior[j], actual[j - 1]);
                }
                var tmp = anterior; anterior = actual; actual = tmp;
                Array.Clear(actual, 0, actual.Length);
            }
            return anterior[taNueva.Count];
        }

        /// <summary>Similitud entre 0 y 1: líneas comunes (en orden) sobre el tamaño del bloque mayor.</summary>
        public static double mxSimilitud(IList<string> taBase, IList<string> taNueva)
        {
            int mayor = Math.Max(taBase.Count, taNueva.Count);
            if (mayor == 0) return 1.0;
            return (double)mxSubsecuenciaComun(taBase, taNueva) / mayor;
        }

        /// <summary>Porcentaje del bloque que cambió entre la versión base y la nueva (0 a 100).</summary>
        public static double mxPorcentajeCambiado(IList<string> taBase, IList<string> taNueva)
        {
            return Math.Round((1.0 - mxSimilitud(taBase, taNueva)) * 100.0, 1);
        }

        /// <summary>Variación porcentual de un valor numérico respecto de la versión base.</summary>
        public static double mxVariacionPorcentual(double tnValorBase, double tnValorNuevo)
        {
            if (tnValorBase == 0) return tnValorNuevo == 0 ? 0 : 100.0;
            return Math.Round(100.0 * (tnValorNuevo - tnValorBase) / tnValorBase, 1);
        }
    }
}
