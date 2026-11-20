using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    // Clase encargada de obtener la conexión a la base de datos.
    // Lee la cadena de conexión desde App.config (nombre: ConexionLocal).
    public class ConexionBD
    {
        public static string tcCadenaConexion;

        // Constructor estático para inicializar la cadena de conexión.
        static ConexionBD()
        {
            try
            {
                var toCadena = ConfigurationManager.ConnectionStrings["ConexionLocal"];
                if (toCadena == null)
                    throw new Exception("No se encontró la cadena de conexión 'ConexionLocal' en App.config.");

                tcCadenaConexion = toCadena.ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al inicializar la conexión a la base de datos: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Devuelve una nueva conexión SQL sin abrir.
        /// </summary>
        /// <returns>Instancia de SqlConnection</returns>
        public static SqlConnection fxObtenerConexion()
        {
            if (string.IsNullOrWhiteSpace(tcCadenaConexion))
                throw new InvalidOperationException("La cadena de conexión no está configurada.");

            return new SqlConnection(tcCadenaConexion);
        }
    }
}
