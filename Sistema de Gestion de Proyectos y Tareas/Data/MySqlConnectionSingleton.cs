// MySqlConnectionSingleton.cs
using Dapper;
using MySql.Data.MySqlClient; // Asegúrate de tener MySql.Data.MySqlClient instalado via NuGet
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models; // Si tus modelos son usados aquí
using System.Collections.Generic;
using System.Data; // Necesario para IDbConnection
using Microsoft.Extensions.Configuration; // Necesario para IConfiguration

// ¡CORRECCIÓN AQUÍ! MySqlConnectionSingleton implementa IDbConnectionSingleton
public class MySqlConnectionSingleton : IDbConnectionSingleton
{
    private readonly string _connectionString;
    private static MySqlConnectionSingleton? _instance; // Este es un patrón Singleton "clásico"

    // Propiedad estática para acceder a la instancia única
    // NOTA: Si usas Inyección de Dependencias (DI) de .NET Core/5+, este patrón puede ser gestionado de forma diferente y más robusta por el contenedor de DI.
    // Si estás en una aplicación web o con DI, te recomiendo registrarlo en Startup.cs/Program.cs
    public static MySqlConnectionSingleton Instance
    {
        get
        {
            // En un entorno de aplicación que usa IConfiguration, el constructor sin parámetros
            // o el patrón 'Instance' estático puro pueden ser problemáticos si la configuración
            // de la cadena de conexión no se gestiona correctamente.
            // Para fines de este ejemplo y para que funcione con tu IConfiguration,
            // sugiero que la inicialización real se haga a través de DI si es posible.
            // Si necesitas un Singleton estático que use IConfiguration, tendrías que pasar
            // la configuración de alguna manera al inicializar _instance.
            if (_instance == null)
            {
                // Este throw es una medida de seguridad si 'Instance' se llama antes de ser configurado con IConfiguration.
                // En una app real, lo inyectarías via DI o pasarías IConfiguration aquí.
                throw new InvalidOperationException("MySqlConnectionSingleton debe ser inicializado antes de acceder a su instancia estática si se requiere IConfiguration.");
            }
            return _instance;
        }
    }

    // Constructor para uso directo sin IConfiguration (menos recomendado en aplicaciones modernas)
    public MySqlConnectionSingleton()
    {
        _connectionString = "Server=localhost;Database=proyectos_tareas_db;User=root;Password=your_password;";
        // Si este constructor es el que se usa y luego se accede via 'Instance',
        // 'Instance' se inicializará con esta cadena de conexión hardcodeada.
    }

    // Constructor preferido para inyección de dependencias con IConfiguration
    public MySqlConnectionSingleton(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MySqlConecction")!;
        // Si usas este constructor a través de DI, NO debes usar la propiedad estática 'Instance' de esta clase,
        // ya que el contenedor de DI gestionará la instancia única.
    }

    // ¡Implementación de la interfaz IDbConnectionSingleton!
    // Asegúrate de que este método es público y devuelve IDbConnection.
    public IDbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }

    // Métodos para ejecutar comandos de Dapper directamente (usados por ProyectoRepository)
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