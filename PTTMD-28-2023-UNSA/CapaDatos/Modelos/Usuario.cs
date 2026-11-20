using System;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    // Entidad Usuario que representa la tabla de usuarios en la base de datos.
    public class Usuario
    {
        // Código de usuario (PK)
        public int pnCodUsu { get; set; }

        // Nombre completo del usuario
        public string pcNomUsu { get; set; }

        // Correo electrónico o nombre de inicio de sesión
        public string pcCorreo { get; set; }

        // Clave encriptada o hash almacenado
        public string pcClave { get; set; }

        // Rol o tipo de usuario (Admin, Evaluador, etc.)
        public string pcRol { get; set; }

        // Fecha de registro del usuario
        public DateTime pdFecReg { get; set; }
    }
}
