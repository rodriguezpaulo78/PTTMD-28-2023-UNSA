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
    /// Clase de capa de datos (CD) encargada del acceso a información de las métricas estructurales.
    /// </summary>
    public class MetricaCD
    {
        // Instancia de conexión a base de datos
        private readonly ConexionBD tcConexion;

        /// <summary>
        /// Constructor de la clase MetricaCD. Inicializa la conexión.
        /// </summary>
        public MetricaCD()
        {
            this.tcConexion = new ConexionBD();
        }

        /// <summary>
        /// Inserta una nueva métrica estructural en la base de datos.
        /// </summary>
        /// <param name="toMetrica">Objeto Metrica a registrar.</param>
        /// <returns>Entero con el ID generado de la métrica.</returns>
        public int mxInsertar(Metrica toMetrica)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnParametros = new DynamicParameters();
                tnParametros.Add("@pnCodMet", dbType: DbType.Int32, direction: ParameterDirection.Output);
                tnParametros.Add("@pcNomMet", toMetrica.pcNomMet);
                tnParametros.Add("@pcDescripcion", toMetrica.pcDescripcion);
                tnParametros.Add("@pcCategoria", toMetrica.pcCategoria);
                tnParametros.Add("@pcUmbral", toMetrica.pcUmbral);
                tnParametros.Add("@pcUnidad", toMetrica.pcUnidad);

                tdConexion.Execute("PROC_I_Metrica", tnParametros, commandType: CommandType.StoredProcedure);
                return tnParametros.Get<int>("@pnCodMet");
            }
        }

        /// <summary>
        /// Actualiza los datos de una métrica existente.
        /// </summary>
        /// <param name="toMetrica">Objeto Metrica con los datos actualizados.</param>
        /// <returns>Verdadero si la actualización fue exitosa.</returns>
        public bool mxActualizar(Metrica toMetrica)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_U_Metrica", new
                {
                    pnCodMet = toMetrica.pnCodMet,
                    pcNomMet = toMetrica.pcNomMet,
                    pcDescripcion = toMetrica.pcDescripcion,
                    pcCategoria = toMetrica.pcCategoria,
                    pcUmbral = toMetrica.pcUmbral,
                    pcUnidad = toMetrica.pcUnidad
                }, commandType: CommandType.StoredProcedure);

                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Elimina una métrica estructural de la base de datos.
        /// </summary>
        /// <param name="pnCodMet">Identificador de la métrica.</param>
        /// <returns>Verdadero si la eliminación fue exitosa.</returns>
        public bool mxEliminar(int pnCodMet)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                var tnFilasAfectadas = tdConexion.Execute("PROC_D_Metrica", new { pnCodMet }, commandType: CommandType.StoredProcedure);
                return tnFilasAfectadas > 0;
            }
        }

        /// <summary>
        /// Obtiene la lista completa de métricas registradas.
        /// </summary>
        /// <returns>Lista de objetos Metrica.</returns>
        public IEnumerable<Metrica> mxTraerTodos()
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.Query<Metrica>("PROC_S_TraerTodasMetricas", commandType: CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// Obtiene una métrica específica por su identificador.
        /// </summary>
        /// <param name="pnCodMet">Identificador de la métrica.</param>
        /// <returns>Objeto Metrica correspondiente.</returns>
        public Metrica mxTraerPorID(int pnCodMet)
        {
            using (var tdConexion = new SqlConnection(tcConexion.pcCadenaConexion))
            {
                return tdConexion.QuerySingleOrDefault<Metrica>("PROC_S_TraerMetricaXID", new { pnCodMet }, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
