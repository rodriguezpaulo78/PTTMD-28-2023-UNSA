using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using PTTMD_28_2023_UNSA.CapaDatos.Modelos;
using PTTMD_28_2023_UNSA.CapaDatos;
using PTTMD_28_2023_UNSA.Models;

namespace PTTMD_28_2023_UNSA.CD
{
    /// <summary>
    /// Clase de capa de datos (CD) encargada del acceso a información de comparaciones entre evaluaciones.
    /// </summary>
    public class ComparacionCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ComparacionCD. Inicializa la conexión.
        /// </summary>
        public ComparacionCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta una nueva comparación entre dos evaluaciones.
        /// </summary>
        /// <param name="toComparacion">Objeto Comparacion a registrar.</param>
        /// <returns>Entero con el ID generado de la comparación.</returns>
        public int mxInsertar(Comparacion toComparacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodCom", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodEvaAnterior", toComparacion.pnCodEvaAnterior);
                tnParametros.Add("@pnCodEvaActual", toComparacion.pnCodEvaActual);
                tnParametros.Add("@pcResultadoGeneral", toComparacion.pcResultadoGeneral);
                tnParametros.Add("@pdFecCom", toComparacion.pdFecCom);

                tdConexion.Execute("PROC_I_Comparacion", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodCom");
            }
        }

        /// <summary>
        /// Actualiza los datos de una comparación existente.
        /// </summary>
        /// <param name="toComparacion">Objeto Comparacion con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Comparacion toComparacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Comparacion", new
                {
                    pnCodCom = toComparacion.pnCodCom,
                    pnCodEvaAnterior = toComparacion.pnCodEvaAnterior,
                    pnCodEvaActual = toComparacion.pnCodEvaActual,
                    pcResultadoGeneral = toComparacion.pcResultadoGeneral
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina una comparación de la base de datos.
        /// </summary>
        /// <param name="pnCodCom">Identificador de la comparación.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Comparacion", new { pnCodCom }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de comparaciones registradas.
        /// </summary>
        /// <returns>Lista de objetos Comparacion.</returns>
        public IEnumerable<Comparacion> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Comparacion>("PROC_S_TraerTodasComparaciones", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene una comparación específica por su identificador.
        /// </summary>
        /// <param name="pnCodCom">Identificador de la comparación.</param>
        /// <returns>Objeto Comparacion correspondiente.</returns>
        public Comparacion mxTraerPorID(int pnCodCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Comparacion>("PROC_S_TraerComparacionXID", new { pnCodCom }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
