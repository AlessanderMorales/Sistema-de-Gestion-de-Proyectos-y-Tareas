using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    public class ProyectoryRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Proyecto>(connectionFactory)
    {
        public override IDB<Proyecto> CreateRepository()
        {
            return new ProyectoRepository(this._connectionSingleton);
        }
    }
}
