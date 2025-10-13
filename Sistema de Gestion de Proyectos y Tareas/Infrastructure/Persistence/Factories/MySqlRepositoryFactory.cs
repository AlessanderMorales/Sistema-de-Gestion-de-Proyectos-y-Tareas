using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;


namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories
{
    public abstract class MySqlRepositoryFactory<T>
    {
        public readonly MySqlConnectionSingleton _connectionSingleton;

        public MySqlRepositoryFactory(MySqlConnectionSingleton connectionFactory)
        {
            _connectionSingleton = connectionFactory;
        }

        public abstract IDB<T> CreateRepository();


    }
}