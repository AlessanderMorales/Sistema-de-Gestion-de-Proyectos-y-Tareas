using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
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