using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
    {
        // Entidad ResultadosComparacion (detalles de diferencia encontrados)
        public class ResultadoComparacion
        {
            // PK
            public int pnCodResCom { get; set; }

            // FK a Comparaciones.pnCodCom
            public int pnCodCom { get; set; }

            // Elemento afectado (clase, método, archivo)
            public string pcElemento { get; set; }

            // Tipo de cambio (Añadido, Eliminado, Modificado)
            public string pcTipoCambio { get; set; }

            // Descripción detallada
            public string pcDescripcion { get; set; }
        }
}
