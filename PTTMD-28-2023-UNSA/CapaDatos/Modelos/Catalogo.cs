using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Catalogos (valores genéricos)
        public class Catalogo
        {
            // PK
            public int pnCodCat { get; set; }

            // Tipo (por ejemplo: TipoMetrica, TipoAnomalia, Estado)
            public string pcTipo { get; set; }

            // Valor
            public string pcValor { get; set; }

            // Descripción
            public string pcDescripcion { get; set; }
        }
}
