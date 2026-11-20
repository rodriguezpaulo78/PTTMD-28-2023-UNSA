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

            // ---- Datos del análisis estructural (se llenan en memoria; no son columnas de la tabla) ----

            // Carpeta raíz de la versión analizada
            public string pcRuta { get; set; }

            // Tiempo que tomó el análisis, en milisegundos
            public long pnMilisegundos { get; set; }

            // Archivos de código fuente de la versión
            public List<Archivo> paArchivos { get; set; }

            // Número total de archivos
            public int pnTotalArchivos { get; set; }

            // Número total de clases
            public int pnTotalClases { get; set; }

            // Número total de métodos
            public int pnTotalMetodos { get; set; }

            // Total de líneas de código
            public int pnTotalLineas { get; set; }

            // Complejidad ciclomática promedio por método
            public double pnComplejidadPromedio { get; set; }

            // Firma estructural de la versión: SHA-256 de los hash lógicos de todos sus archivos
            public string pcFirmaEstructural { get; set; }

            public Evaluacion()
            {
                paArchivos = new List<Archivo>();
            }
        }
}
