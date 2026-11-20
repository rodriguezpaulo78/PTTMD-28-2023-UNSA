using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Archivos (subidas asociadas a un proyecto/version)
        public class Archivo
        {
            // PK
            public int pnCodArc { get; set; }

            // FK a Proyectos.pnCodPro
            public int pnCodPro { get; set; }

            // Ruta física o relativa del archivo/zip
            public string pcRuta { get; set; }

            // Nombre del archivo
            public string pcNombre { get; set; }

            // Fecha de subida
            public DateTime pdFecSubida { get; set; }

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Tamaño en bytes
            public long pnTamano { get; set; }

            // Fecha de última modificación en disco
            public DateTime pdFecModificacion { get; set; }

            // Líneas de código no vacías
            public int pnLineas { get; set; }

            // SHA-256 del contenido en disco (nivel físico)
            public string pcHashFisico { get; set; }

            // SHA-256 del código sin espacios ni comentarios (nivel lógico)
            public string pcHashLogico { get; set; }

            // Clases declaradas en el archivo
            public List<Clase> paClases { get; set; }

            // Líneas del archivo normalizadas; se usan para detectar archivos movidos o renombrados
            public List<string> paLineasNormalizadas { get; set; }

            public Archivo()
            {
                paClases = new List<Clase>();
                paLineasNormalizadas = new List<string>();
            }
        }
}
