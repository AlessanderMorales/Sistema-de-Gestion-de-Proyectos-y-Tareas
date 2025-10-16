﻿using Dapper;
using MySql.Data.MySqlClient; 
using System.Collections.Generic;
using System.Data; 
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data
{
    public class MySqlConnectionSingleton : IDbConnectionFactory
    {
        private readonly string _connectionString;
        private static MySqlConnectionSingleton? _instance;
   
        public MySqlConnectionSingleton()
        {
            // Default fallback; prefer configuration-based constructor
            _connectionString = "Server=localhost;Port=3306;Database=gestion_proyectos;Uid=root;Pwd=your_password;";
        }

        public MySqlConnectionSingleton(IConfiguration configuration)
        {
            // Use corrected key MySqlConnection
            var cs = configuration?.GetConnectionString("MySqlConnection");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("La cadena de conexión 'MySqlConnection' no está configurada.");
            _connectionString = cs;
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

        public IEnumerable<Q> ExcuteCommandWithDataReturn<Q>(string query, object? parameters = null)
        {
            using (var connection = CreateConnection())
            {
                // Materialize results into a list while connection is open to avoid deferred execution after disposal
                var result = connection.Query<Q>(query, parameters);
                return result.ToList();
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
}