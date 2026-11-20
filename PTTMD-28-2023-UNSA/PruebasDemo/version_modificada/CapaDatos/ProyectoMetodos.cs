using Dapper;
using PTTMD_28_2023_UNSA.CapaDatos;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

public class ProyectoMetodos
{
    public static string connectionString = ConfigurationManager.ConnectionStrings["ConexionDB"].ConnectionString;
    SqlConnection poConexion = new SqlConnection(connectionString);
    public int Insertar(CDProyecto proyecto)
    {
        IDbConnection db = poConexion;
        string sql = @"INSERT INTO Proyecto (Nombre, NumeroArchivos, NumeroClases, LineasCodigo, NumeroMetodos, Ruta)
                       VALUES (@Nombre, @NumeroArchivos, @NumeroClases, @LineasCodigo, @NumeroMetodos, @Ruta);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
        return db.QuerySingle<int>(sql, proyecto);
    }

    public CDProyecto ObtenerPorId(int id)
    {
        IDbConnection db = poConexion;
        return db.QueryFirstOrDefault<CDProyecto>("SELECT * FROM Proyecto WHERE Id = @id", new { id });
    }

    public IEnumerable<CDProyecto> ObtenerTodos()
    {
        IDbConnection db = poConexion;
        return db.Query<CDProyecto>("SELECT * FROM Proyecto");
    }

    public void Actualizar(CDProyecto proyecto)
    {
        IDbConnection db = poConexion;
        string sql = @"UPDATE Proyecto
                       SET Nombre = @Nombre,
                           NumeroArchivos = @NumeroArchivos,
                           NumeroClases = @NumeroClases,
                           LineasCodigo = @LineasCodigo,
                           NumeroMetodos = @NumeroMetodos,
                           Ruta = @Ruta

                       WHERE Id = @Id";
        db.Execute(sql, proyecto);
    }

    public void Eliminar(int id)
    {
        IDbConnection db = poConexion;
        db.Execute("DELETE FROM Proyecto WHERE Id = @id", new { id });
    }
}
