using System.Configuration;
using System.Data;
using System.Data.SqlClient;

public static class DbConnectionFactory
{
    public static IDbConnection GetConnection()
    {
        string connectionString = ConfigurationManager.ConnectionStrings["ConexionDB"].ConnectionString;
        return new SqlConnection(connectionString);
    }
}
