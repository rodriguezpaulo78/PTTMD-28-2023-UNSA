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
    /// Clase de capa de datos (CD) encargada del acceso a información de anomalías estructurales detectadas.
    /// </summary>
    public class AnomaliaCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase AnomaliaCD. Inicializa la conexión.
        /// </summary>
        public AnomaliaCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta una nueva anomalía detectada en una comparación de versiones.
        /// </summary>
        /// <param name="toAnomalia">Objeto Anomalia a registrar.</param>
        /// <returns>Entero con el ID generado de la anomalía.</returns>
        public int mxInsertar(Anomalia toAnomalia)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodAno", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pnCodCom", toAnomalia.pnCodCom);
                tnParametros.Add("@pcElemento", toAnomalia.pcElemento);
                tnParametros.Add("@pcTipoAnomalia", toAnomalia.pcTipoAnomalia);
                tnParametros.Add("@pcDescripcion", toAnomalia.pcDescripcion);
                tnParametros.Add("@pcSeveridad", toAnomalia.pcSeveridad);
                tnParametros.Add("@pdFecDet", toAnomalia.pdFecDet);

                tdConexion.Execute("PROC_I_Anomalia", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodAno");
            }
        }

        /// <summary>
        /// Actualiza la información de una anomalía existente.
        /// </summary>
        /// <param name="toAnomalia">Objeto Anomalia con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Anomalia toAnomalia)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Anomalia", new
                {
                    pnCodAno = toAnomalia.pnCodAno,
                    pnCodCom = toAnomalia.pnCodCom,
                    pcElemento = toAnomalia.pcElemento,
                    pcTipoAnomalia = toAnomalia.pcTipoAnomalia,
                    pcDescripcion = toAnomalia.pcDescripcion,
                    pcSeveridad = toAnomalia.pcSeveridad
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina una anomalía de la base de datos.
        /// </summary>
        /// <param name="pnCodAno">Identificador de la anomalía.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodAno)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Anomalia", new { pnCodAno }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de anomalías detectadas en una comparación específica.
        /// </summary>
        /// <param name="pnCodCom">Identificador de la comparación.</param>
        /// <returns>Lista de objetos Anomalia correspondientes.</returns>
        public IEnumerable<Anomalia> mxTraerPorComparacion(int pnCodCom)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Anomalia>("PROC_S_TraerAnomaliasPorComparacion", new { pnCodCom }, commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene una anomalía específica por su identificador.
        /// </summary>
        /// <param name="pnCodAno">Identificador de la anomalía.</param>
        /// <returns>Objeto Anomalia correspondiente.</returns>
        public Anomalia mxTraerPorID(int pnCodAno)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Anomalia>("PROC_S_TraerAnomaliaXID", new { pnCodAno }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
