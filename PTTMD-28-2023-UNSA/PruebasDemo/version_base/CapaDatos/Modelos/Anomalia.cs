using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad AnomaliasDetectadas
        public class Anomalia
        {
            // PK
            public int pnCodAnom { get; set; }

            // FK a ResultadosComparacion.pnCodResCom
            public int pnCodResCom { get; set; }

            // Tipo de anomalía (ej. EliminacionSilenciosa, AumentoComplejidad)
            public string pcTipoAnom { get; set; }

            // Descripción / evidencia
            public string pcDescripcion { get; set; }

            // Fecha de detección
            public DateTime? pdFecDet { get; set; }

            // Severidad (opcional)
            public string pcSeveridad { get; set; }
        }
}
