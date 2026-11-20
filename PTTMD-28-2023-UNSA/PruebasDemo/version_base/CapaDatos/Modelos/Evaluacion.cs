using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{

        // Entidad Evaluaciones (una evaluación / versión de un proyecto)
        public class Evaluacion
        {
            // PK
            public int pnCodEva { get; set; }

            // FK a Proyectos.pnCodPro
            public int pnCodPro { get; set; }

            // Nombre de la versión / evaluación
            public string pcNomVersion { get; set; }

            // Fecha de la evaluación
            public DateTime pdFecEval { get; set; }

            // Responsable o usuario que ejecutó la evaluación (opcional)
            public string pcResponsable { get; set; }

            // Observaciones libres
            public string pcObservaciones { get; set; }
        }
}
