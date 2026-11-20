using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad ResultadosMetricas (valores cuantitativos por evaluación/metric)
        public class ResultadoMetrica
        {
            // PK
            public int pnCodRes { get; set; }

            // FK a Evaluaciones.pnCodEva
            public int pnCodEva { get; set; }

            // FK a Metricas.pnCodMetri
            public int pnCodMetri { get; set; }

            // FK opcional a Clases.pnCodCla
            public int? pnCodCla { get; set; }

            // FK opcional a Metodos.pnCodMet
            public int? pnCodMet { get; set; }

            // Valor numérico de la métrica
            public decimal pnValor { get; set; }
        }
}
