using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{
    // Entidad Roles - catálogo de roles del sistema
    public class Rol
    {
        // Código / nombre del rol (PK)
        public string pcRol { get; set; }

        // Descripción del rol
        public string pcDescripcion { get; set; }
    }  
}
