using MySql.Data.MySqlClient;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;
using System.Data;
namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    public abstract class RepositoryFactory<T>
    {
        public abstract IDB<T> CreateRepository();
    }
}
