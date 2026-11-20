using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.CapaDatos;

namespace PTTMD_28_2023_UNSA.CapaDatos

{
    /// <summary>
    /// Clase de capa de datos (CD) encargada del acceso a información de proyectos.
    /// </summary>
    public class ProyectoCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ProyectoCD. Inicializa la conexión.
        /// </summary>
        public ProyectoCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta un nuevo proyecto en la base de datos.
        /// </summary>
        /// <param name="toProyecto">Objeto Proyecto a registrar.</param>
        /// <returns>Entero con el ID generado del proyecto.</returns>
        public int mxInsertar(Proyecto toProyecto)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodPro", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pcNomPro", toProyecto.pcNomPro);
                tnParametros.Add("@pcDescripcion", toProyecto.pcDescripcion);
                tnParametros.Add("@pnCodUsu", toProyecto.pnCodUsu);
                tnParametros.Add("@pdFecReg", toProyecto.pdFecReg);

                tdConexion.Execute("PROC_I_Proyecto", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodPro");
            }
        }

        /// <summary>
        /// Actualiza los datos de un proyecto existente.
        /// </summary>
        /// <param name="toProyecto">Objeto Proyecto con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Proyecto toProyecto)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Proyecto", new
                {
                    pnCodPro = toProyecto.pnCodPro,
                    pcNomPro = toProyecto.pcNomPro,
                    pcDescripcion = toProyecto.pcDescripcion
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina un proyecto de la base de datos por su identificador.
        /// </summary>
        /// <param name="pnCodPro">Identificador del proyecto a eliminar.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodPro)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Proyecto", new { pnCodPro }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de proyectos registrados.
        /// </summary>
        /// <returns>Lista de objetos Proyecto.</returns>
        public IEnumerable<Proyecto> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Proyecto>("PROC_S_TraerTodosProyectos", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene un proyecto específico por su identificador.
        /// </summary>
        /// <param name="pnCodPro">Identificador del proyecto.</param>
        /// <returns>Objeto Proyecto correspondiente.</returns>
        public Proyecto mxTraerPorID(int pnCodPro)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Proyecto>("PROC_S_TraerProyectoXID", new { pnCodPro }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
