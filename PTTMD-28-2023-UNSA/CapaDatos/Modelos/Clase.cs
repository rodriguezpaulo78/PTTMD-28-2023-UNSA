using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Clases (clases detectadas dentro de un archivo)
        public class Clase
        {
            // PK
            public int pnCodCla { get; set; }

            // FK a Archivos.pnCodArc
            public int pnCodArc { get; set; }

            // Nombre de la clase
            public string pcNomCla { get; set; }

            // Espacio de nombres (namespace)
            public string pcEspacioNombres { get; set; }

            // Fecha de análisis
            public DateTime pdFecAnalisis { get; set; }

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Espacio de nombres + clases contenedoras + nombre
            public string pcNombreCompleto { get; set; }

            // Tipo de declaración (class, struct, interface)
            public string pcTipo { get; set; }

            // Visibilidad de la clase
            public string pcAcceso { get; set; }

            // Clase base e interfaces declaradas
            public string pcClaseBase { get; set; }

            // Ruta relativa del archivo que contiene la clase
            public string pcRutaArchivo { get; set; }

            // Número de atributos (campos)
            public int pnAtributos { get; set; }

            // Número de propiedades
            public int pnPropiedades { get; set; }

            // Líneas que ocupa la clase
            public int pnLineas { get; set; }

            // SHA-256 de la clase sin espacios ni comentarios (nivel lógico)
            public string pcHash { get; set; }

            // Métodos y constructores de la clase
            public List<Metodo> paMetodos { get; set; }

            // Líneas de la clase normalizadas; se usan para detectar renombramientos
            public List<string> paLineasNormalizadas { get; set; }

            public Clase()
            {
                paMetodos = new List<Metodo>();
                paLineasNormalizadas = new List<string>();
            }
        }
}
