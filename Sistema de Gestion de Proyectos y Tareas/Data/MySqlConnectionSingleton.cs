using Dapper;
using Microsoft.AspNetCore.Connections;
using MySql.Data.MySqlClient;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using System.Data;

public class MySqlConnectionSingleton
{
    private readonly string _connectionString;
    private static MySqlConnectionSingleton? _instance;
    public static MySqlConnectionSingleton Instance { get { 
         if(_instance == null) {
            _instance = new MySqlConnectionSingleton();
        }
         return _instance;
        } 
    }

    public void ExcuteCommand<Q>( string query, Q entity )
    {
        using (var connection = CreateConnection())
        {
            connection.Execute(query, entity);
        }
    }

    public IEnumerable<Q> ExcuteCommandWithDataReturn<Q>(string query)
    {
        using (var connection = CreateConnection())
        {
            return connection.Query<Q>(query);
        }

    }
    public MySqlConnectionSingleton()
    {
        // Default constructor for cases where IConfiguration is not available
        _connectionString = "Server=localhost;Database=proyectos_tareas_db;User=root;Password=your_password;";
    }
    public MySqlConnectionSingleton(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConecction")!;
    }

    MySqlConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}