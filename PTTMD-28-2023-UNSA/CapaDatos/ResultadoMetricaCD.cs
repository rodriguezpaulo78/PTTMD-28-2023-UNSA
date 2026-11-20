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
    /// Clase de capa de datos (CD) encargada del acceso a los resultados de métricas por evaluación.
    /// </summary>
    public class ResultadoMetricaCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ResultadoMetricaCD. Inicializa la conexión.
        /// </summary>
        public ResultadoMetricaCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta un nuevo resultado de métrica asociado a una evaluación.
        /// </summary>
        /// <param name="toResultadoMetrica">Objeto ResultadoMetrica a registrar.</param>
        /// <returns>Entero con el ID generado del resultado de métrica.</returns>
        public int mxInsertar(ResultadoMetrica toResultadoMetrica)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodResMet", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodEva", toResultadoMetrica.pnCodEva);
                tnParametros.Add("@pnCodMet", toResultadoMetrica.pnCodMet);
                tnParametros.Add("@pnValor", toResultadoMetrica.pnValor);
                tnParametros.Add("@pcCategoria", toResultadoMetrica.pcCategoria);
                tnParametros.Add("@pcEvaluacion", toResultadoMetrica.pcEvaluacion);

                tdConexion.Execute("PROC_I_ResultadoMetrica", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodResMet");
            }
        }

        /// <summary>
        /// Actualiza un resultado de métrica existente.
        /// </summary>
        /// <param name="toResultadoMetrica">Objeto ResultadoMetrica con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(ResultadoMetrica toResultadoMetrica)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_ResultadoMetrica", new
                {
                    pnCodResMet = toResultadoMetrica.pnCodResMet,
                    pnCodEva = toResultadoMetrica.pnCodEva,
                    pnCodMet = toResultadoMetrica.pnCodMet,
                    pnValor = toResultadoMetrica.pnValor,
                    pcCategoria = toResultadoMetrica.pcCategoria,
                    pcEvaluacion = toResultadoMetrica.pcEvaluacion
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina un resultado de métrica por su identificador.
        /// </summary>
        /// <param name="pnCodResMet">Identificador del resultado de métrica.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodResMet)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_ResultadoMetrica", new { pnCodResMet }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene todos los resultados de métricas asociados a una evaluación específica.
        /// </summary>
        /// <param name="pnCodEva">Identificador de la evaluación.</param>
        /// <returns>Lista de resultados de métricas correspondientes a la evaluación.</returns>
        public IEnumerable<ResultadoMetrica> mxTraerPorEvaluacion(int pnCodEva)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<ResultadoMetrica>("PROC_S_TraerResultadosPorEvaluacion", new { pnCodEva }, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene un resultado de métrica específico por su identificador.
        /// </summary>
        /// <param name="pnCodResMet">Identificador del resultado de métrica.</param>
        /// <returns>Objeto ResultadoMetrica correspondiente.</returns>
        public ResultadoMetrica mxTraerPorID(int pnCodResMet)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<ResultadoMetrica>("PROC_S_TraerResultadoMetricaXID", new { pnCodResMet }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
