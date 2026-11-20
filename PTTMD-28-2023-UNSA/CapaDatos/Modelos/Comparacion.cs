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

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Versión base (evaluación anterior)
            public Evaluacion poEvaluacionBase { get; set; }

            // Versión nueva (evaluación actual)
            public Evaluacion poEvaluacionNueva { get; set; }

            // Tiempo que tomó la comparación, en milisegundos
            public long pnMilisegundos { get; set; }

            // Anomalías detectadas, ya clasificadas
            public List<Anomalia> paAnomalias { get; set; }

            public Comparacion()
            {
                paAnomalias = new List<Anomalia>();
            }

            // Cantidad de anomalías con la severidad indicada
            public int mxContar(Severidad tnSeveridad)
            {
                int cantidad = 0;
                foreach (var anomalia in paAnomalias) if (anomalia.pnSeveridad == tnSeveridad) cantidad++;
                return cantidad;
            }

            // Verdadero si las dos versiones tienen la misma firma estructural
            public bool mxSonIdenticas()
            {
                return poEvaluacionBase != null && poEvaluacionNueva != null &&
                       poEvaluacionBase.pcFirmaEstructural == poEvaluacionNueva.pcFirmaEstructural;
            }
        }
}
