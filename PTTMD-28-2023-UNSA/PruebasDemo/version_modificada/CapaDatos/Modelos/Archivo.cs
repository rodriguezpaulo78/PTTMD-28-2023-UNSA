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
        }
}
