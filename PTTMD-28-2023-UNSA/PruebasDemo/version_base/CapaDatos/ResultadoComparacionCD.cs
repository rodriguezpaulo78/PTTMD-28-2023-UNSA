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
    /// Clase de capa de datos (CD) encargada del acceso a información de resultados de comparación estructural.
    /// </summary>
    public class ResultadoComparacionCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ResultadoComparacionCD. Inicializa la conexión.
        /// </summary>
        public ResultadoComparacionCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta un nuevo resultado de comparación entre versiones de un proyecto.
        /// </summary>
        /// <param name="toResultadoComparacion">Objeto ResultadoComparacion a registrar.</param>
        /// <returns>Entero con el ID generado del resultado de comparación.</returns>
        public int mxInsertar(ResultadoComparacion toResultadoComparacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodResCom", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodCom", toResultadoComparacion.pnCodCom);
                tnParametros.Add("@pcElemento", toResultadoComparacion.pcElemento);
                tnParametros.Add("@pcTipoCambio", toResultadoComparacion.pcTipoCambio);
                tnParametros.Add("@pcDescripcionCambio", toResultadoComparacion.pcDescripcionCambio);
                tnParametros.Add("@pcNivelImpacto", toResultadoComparacion.pcNivelImpacto);

                tdConexion.Execute("PROC_I_ResultadoComparacion", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodResCom");
            }
        }

        /// <summary>
        /// Actualiza un resultado de comparación existente.
        /// </summary>
        /// <param name="toResultadoComparacion">Objeto ResultadoComparacion con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(ResultadoComparacion toResultadoComparacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_ResultadoComparacion", new
                {
                    pnCodResCom = toResultadoComparacion.pnCodResCom,
                    pnCodCom = toResultadoComparacion.pnCodCom,
                    pcElemento = toResultadoComparacion.pcElemento,
                    pcTipoCambio = toResultadoComparacion.pcTipoCambio,
                    pcDescripcionCambio = toResultadoComparacion.pcDescripcionCambio,
                    pcNivelImpacto = toResultadoComparacion.pcNivelImpacto
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina un resultado de comparación por su identificador.
        /// </summary>
        /// <param name="pnCodResCom">Identificador del resultado de comparación.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodResCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_ResultadoComparacion", new { pnCodResCom }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene todos los resultados de comparación asociados a una comparación específica.
        /// </summary>
        /// <param name="pnCodCom">Identificador de la comparación.</param>
        /// <returns>Lista de resultados de comparación.</returns>
        public IEnumerable<ResultadoComparacion> mxTraerPorComparacion(int pnCodCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<ResultadoComparacion>("PROC_S_TraerResultadosPorComparacion", new { pnCodCom }, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene un resultado de comparación específico por su identificador.
        /// </summary>
        /// <param name="pnCodResCom">Identificador del resultado de comparación.</param>
        /// <returns>Objeto ResultadoComparacion correspondiente.</returns>
        public ResultadoComparacion mxTraerPorID(int pnCodResCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<ResultadoComparacion>("PROC_S_TraerResultadoComparacionXID", new { pnCodResCom }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
