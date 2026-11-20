using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Metricas (catálogo de métricas)
        public class Metrica
        {
            // PK
            public int pnCodMetri { get; set; }

            // Nombre de la métrica
            public string pcNomMetri { get; set; }

            // Descripción
            public string pcDescripcion { get; set; }

            // Categoría (tamaño, complejidad, acoplamiento, etc.)
            public string pcCategoria { get; set; }

            // Umbral o nota (puede guardarse como string)
            public string pcUmbral { get; set; }

            // Unidad opcional (ej. líneas)
            public string pcUnidad { get; set; }
        }
}
