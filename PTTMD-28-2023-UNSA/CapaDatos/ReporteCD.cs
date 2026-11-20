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
    /// Clase de capa de datos (CD) encargada del acceso a información de reportes técnicos generados.
    /// </summary>
    public class ReporteCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase ReporteCD. Inicializa la conexión.
        /// </summary>
        public ReporteCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta un nuevo reporte técnico generado por el sistema.
        /// </summary>
        /// <param name="toReporte">Objeto Reporte a registrar.</param>
        /// <returns>Entero con el ID generado del reporte.</returns>
        public int mxInsertar(Reporte toReporte)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodRep", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodEva", toReporte.pnCodEva);
                tnParametros.Add("@pnCodCom", toReporte.pnCodCom);
                tnParametros.Add("@pcTipoReporte", toReporte.pcTipoReporte);
                tnParametros.Add("@pcRutaArchivo", toReporte.pcRutaArchivo);
                tnParametros.Add("@pcDescripcion", toReporte.pcDescripcion);
                tnParametros.Add("@pdFecGen", toReporte.pdFecGen);

                tdConexion.Execute("PROC_I_Reporte", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodRep");
            }
        }

        /// <summary>
        /// Actualiza la información de un reporte existente.
        /// </summary>
        /// <param name="toReporte">Objeto Reporte con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Reporte toReporte)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Reporte", new
                {
                    pnCodRep = toReporte.pnCodRep,
                    pnCodEva = toReporte.pnCodEva,
                    pnCodCom = toReporte.pnCodCom,
                    pcTipoReporte = toReporte.pcTipoReporte,
                    pcRutaArchivo = toReporte.pcRutaArchivo,
                    pcDescripcion = toReporte.pcDescripcion
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina un reporte técnico de la base de datos.
        /// </summary>
        /// <param name="pnCodRep">Identificador del reporte.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodRep)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Reporte", new { pnCodRep }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de reportes generados.
        /// </summary>
        /// <returns>Lista de objetos Reporte.</returns>
        public IEnumerable<Reporte> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Reporte>("PROC_S_TraerTodosReportes", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene todos los reportes vinculados a una evaluación específica.
        /// </summary>
        /// <param name="pnCodEva">Identificador de la evaluación.</param>
        /// <returns>Lista de reportes asociados a la evaluación.</returns>
        public IEnumerable<Reporte> mxTraerPorEvaluacion(int pnCodEva)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Reporte>("PROC_S_TraerReportesPorEvaluacion", new { pnCodEva }, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene un reporte específico por su identificador.
        /// </summary>
        /// <param name="pnCodRep">Identificador del reporte.</param>
        /// <returns>Objeto Reporte correspondiente.</returns>
        public Reporte mxTraerPorID(int pnCodRep)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Reporte>("PROC_S_TraerReporteXID", new { pnCodRep }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
