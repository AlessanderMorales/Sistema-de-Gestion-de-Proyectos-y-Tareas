using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{

    public class TareaRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Tarea>(connectionFactory)
    {
        public override IDB<Tarea> CreateRepository()
        {
            return new TareaRepository(this._connectionSingleton);
        }
    }
}