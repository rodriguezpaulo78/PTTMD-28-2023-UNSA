using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTMD_28_2023_UNSA.CapaDatos.Modelos
{    
    // Entidad HistorialAcceso - registros de login/logout y acciones
    public class HistorialAcceso
    {
        // PK
        public int pnId { get; set; }

        // FK a Usuarios.pnCodUsu
        public int pnCodUsu { get; set; }

        // Acción realizada (Login, Logout, CambioClave, etc.)
        public string pcAccion { get; set; }

        // Fecha y hora del evento
        public DateTime pdFecAccion { get; set; }
    }
}
