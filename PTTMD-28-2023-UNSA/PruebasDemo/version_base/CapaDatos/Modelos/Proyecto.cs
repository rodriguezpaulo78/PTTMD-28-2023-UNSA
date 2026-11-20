using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
        // Entidad Proyectos
        public class Proyecto
        {
            // PK
            public int pnCodPro { get; set; }

            // Nombre del proyecto
            public string pcNomPro { get; set; }

            // Descripción
            public string pcDescripcion { get; set; }

            // Usuario propietario (fk)
            public int pnCodUsu { get; set; }

            // Fecha de registro
            public DateTime pdFecReg { get; set; }
        }
}
