using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Reportes
        public class Reporte
        {
            // PK
            public int pnCodRep { get; set; }

            // FK opcional a Evaluaciones
            public int? pnCodEva { get; set; }

            // FK opcional a Comparaciones
            public int? pnCodCom { get; set; }

            // Ruta del archivo (PDF)
            public string pcRutaArchivo { get; set; }

            // Tipo/plantilla del reporte
            public string pcTipoReporte { get; set; }

            // Descripción adicional
            public string pcDescripcion { get; set; }

            // Fecha de generación
            public DateTime pdFecGen { get; set; }
        }
}
