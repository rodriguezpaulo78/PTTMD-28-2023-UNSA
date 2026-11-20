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
    /// Clase de capa de datos (CD) encargada del acceso a información de las evaluaciones de proyectos.
    /// </summary>
    public class EvaluacionCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase EvaluacionCD. Inicializa la conexión.
        /// </summary>
        public EvaluacionCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta una nueva evaluación de proyecto en la base de datos.
        /// </summary>
        /// <param name="toEvaluacion">Objeto Evaluacion a registrar.</param>
        /// <returns>Entero con el ID generado de la evaluación.</returns>
        public int mxInsertar(Evaluacion toEvaluacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodEva", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodPro", toEvaluacion.pnCodPro);
                tnParametros.Add("@pcNomVersion", toEvaluacion.pcNomVersion);
                tnParametros.Add("@pdFecEval", toEvaluacion.pdFecEval);
                tnParametros.Add("@pcResponsable", toEvaluacion.pcResponsable);
                tnParametros.Add("@pcObservaciones", toEvaluacion.pcObservaciones);

                tdConexion.Execute("PROC_I_Evaluacion", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodEva");
            }
        }

        /// <summary>
        /// Actualiza una evaluación existente en la base de datos.
        /// </summary>
        /// <param name="toEvaluacion">Objeto Evaluacion con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Evaluacion toEvaluacion)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Evaluacion", new
                {
                    pnCodEva = toEvaluacion.pnCodEva,
                    pnCodPro = toEvaluacion.pnCodPro,
                    pcNomVersion = toEvaluacion.pcNomVersion,
                    pdFecEval = toEvaluacion.pdFecEval,
                    pcResponsable = toEvaluacion.pcResponsable,
                    pcObservaciones = toEvaluacion.pcObservaciones
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina una evaluación de la base de datos por su identificador.
        /// </summary>
        /// <param name="pnCodEva">Identificador de la evaluación.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodEva)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Evaluacion", new { pnCodEva }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de evaluaciones registradas.
        /// </summary>
        /// <returns>Lista de objetos Evaluacion.</returns>
        public IEnumerable<Evaluacion> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Evaluacion>("PROC_S_TraerTodasEvaluaciones", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene una evaluación específica por su identificador.
        /// </summary>
        /// <param name="pnCodEva">Identificador de la evaluación.</param>
        /// <returns>Objeto Evaluacion correspondiente.</returns>
        public Evaluacion mxTraerPorID(int pnCodEva)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Evaluacion>("PROC_S_TraerEvaluacionXID", new { pnCodEva }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
