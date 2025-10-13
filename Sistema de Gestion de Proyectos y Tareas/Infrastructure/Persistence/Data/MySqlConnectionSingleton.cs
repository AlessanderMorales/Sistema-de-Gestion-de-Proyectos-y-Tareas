
using Dapper;
using MySql.Data.MySqlClient; 
using System.Collections.Generic;
using System.Data; 
using Microsoft.Extensions.Configuration; 

public class MySqlConnectionSingleton : IDbConnectionSingleton
{
    private readonly string _connectionString;
    private static MySqlConnectionSingleton? _instance;
   
    public MySqlConnectionSingleton()
    {
        _connectionString = "Server=localhost;Database=proyectos_tareas_db;User=root;Password=your_password;";
    }
    public MySqlConnectionSingleton(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConecction")!;
    }

    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    public void ExcuteCommand<Q>(string query, Q entity)
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

    public Q QueryFirstOrDefault<Q>(string query, object? parameters = null) where Q : class
    {
        using (var connection = CreateConnection())
        {
            return connection.QueryFirstOrDefault<Q>(query, parameters);
        }
    }
}