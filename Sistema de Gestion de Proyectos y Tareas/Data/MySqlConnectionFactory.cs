using MySql.Data.MySqlClient;
using System.Data;

public class MySqlConnectionFactory : IDbConnectionSingleton
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConecction")!;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}