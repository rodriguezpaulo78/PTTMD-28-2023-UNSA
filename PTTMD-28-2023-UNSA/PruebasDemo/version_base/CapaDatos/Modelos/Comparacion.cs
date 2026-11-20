using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Comparaciones (compara dos evaluaciones)
        public class Comparacion
        {
            // PK
            public int pnCodCom { get; set; }

            // FK a Evaluaciones.pnCodEva (anterior)
            public int pnCodEvaAnterior { get; set; }

            // FK a Evaluaciones.pnCodEva (actual)
            public int pnCodEvaActual { get; set; }

            // Resultado general (texto breve)
            public string pcResultadoGeneral { get; set; }

            // Fecha de comparación
            public DateTime pdFecCom { get; set; }
        }
}
