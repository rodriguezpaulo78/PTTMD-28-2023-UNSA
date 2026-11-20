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
        }
}
