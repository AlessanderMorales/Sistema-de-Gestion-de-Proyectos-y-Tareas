﻿using Dapper;
using MySql.Data.MySqlClient; 
using System.Collections.Generic;
using System.Data; 
using Microsoft.Extensions.Configuration;
using System.Linq;
using System;

namespace ServiceCommon.Infrastructure.Persistence.Data
{
    public class MySqlConnectionSingleton : IDbConnectionFactory
    {
        private readonly string _connectionString;
   
        public MySqlConnectionSingleton()
        {
            throw new InvalidOperationException(
        "Use el constructor con IConfiguration. " +
        "La cadena de conexion debe configurarse en appsettings.json");
        }

        public MySqlConnectionSingleton(IConfiguration configuration)
        {
            var cs = configuration?.GetConnectionString("MySqlConnection");
            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException("La cadena de conexion 'MySqlConnection' no esta configurada en appsettings.json");
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