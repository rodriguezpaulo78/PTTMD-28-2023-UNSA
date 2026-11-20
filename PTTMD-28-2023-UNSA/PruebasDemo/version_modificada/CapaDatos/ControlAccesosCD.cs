using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace PTTMD_28_2023_UNSA.CapaDatos
{
    /// <summary>
    /// Clase de capa de datos (CD) encargada del acceso a la información de usuarios.
    /// </summary>
    public class ControlAccesosCD
    {
        // Instancia de conexión a base de datos
        public ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ControlAccesosCD. Inicializa la conexión.
        /// </summary>
        public ControlAccesosCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="toUsuario">Objeto Usuario a registrar.</param>
        /// <returns>Entero con el ID generado del usuario.</returns>
        public int mxInsertar(Usuario toUsuario)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodUsu", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pcNomUsu", toUsuario.pcNomUsu);
                tnParametros.Add("@pcCorreo", toUsuario.pcCorreo);
                tnParametros.Add("@pcClave", toUsuario.pcClave);
                tnParametros.Add("@pcRol", toUsuario.pcRol);
                tnParametros.Add("@pdFecReg", toUsuario.pdFecReg);

                tdConexion.Execute("PROC_I_Usuario", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodUsu");
            }
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="toUsuario">Objeto Usuario con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Usuario toUsuario)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Usuario", new
                {
                    pnCodUsu = toUsuario.pnCodUsu,
                    pcNomUsu = toUsuario.pcNomUsu,
                    pcCorreo = toUsuario.pcCorreo,
                    pcClave = toUsuario.pcClave,
                    pcRol = toUsuario.pcRol
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina un usuario de la base de datos por su identificador.
        /// </summary>
        /// <param name="pnCodUsu">Identificador del usuario a eliminar.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodUsu)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Usuario", new { pnCodUsu }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de usuarios registrados.
        /// </summary>
        /// <returns>Lista de objetos Usuario.</returns>
        public IEnumerable<Usuario> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Usuario>("PROC_S_TraerTodosUsuarios", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador.
        /// </summary>
        /// <param name="pnCodUsu">Identificador del usuario.</param>
        /// <returns>Objeto Usuario correspondiente.</returns>
        public Usuario mxTraerPorID(int pnCodUsu)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Usuario>("PROC_S_TraerUsuarioXID", new { pnCodUsu }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
