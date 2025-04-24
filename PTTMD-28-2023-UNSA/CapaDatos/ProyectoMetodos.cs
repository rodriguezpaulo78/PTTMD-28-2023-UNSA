using Dapper;
using PTTMD_28_2023_UNSA.CapaDatos;
using System.Collections.Generic;
using System.Data;
using System.Linq;

public class ProyectoMetodos
{
    public int Insertar(CDProyecto proyecto)
    {
        IDbConnection db = DbConnectionFactory.GetConnection();
        string sql = @"INSERT INTO Proyecto (Nombre, NumeroArchivos, NumeroClases, LineasCodigo, NumeroMetodos, Ruta)
                       VALUES (@Nombre, @NumeroArchivos, @NumeroClases, @LineasCodigo, @NumeroMetodos, @Ruta);
                       SELECT CAST(SCOPE_IDENTITY() as int)";
        return db.QuerySingle<int>(sql, proyecto);
    }

    public CDProyecto ObtenerPorId(int id)
    {
        IDbConnection db = DbConnectionFactory.GetConnection();
        return db.QueryFirstOrDefault<CDProyecto>("SELECT * FROM Proyecto WHERE Id = @id", new { id });
    }

    public IEnumerable<CDProyecto> ObtenerTodos()
    {
        IDbConnection db = DbConnectionFactory.GetConnection();
        return db.Query<CDProyecto>("SELECT * FROM Proyecto");
    }

    public void Actualizar(CDProyecto proyecto)
    {
        IDbConnection db = DbConnectionFactory.GetConnection();
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
        IDbConnection db = DbConnectionFactory.GetConnection();
        db.Execute("DELETE FROM Proyecto WHERE Id = @id", new { id });
    }
}
