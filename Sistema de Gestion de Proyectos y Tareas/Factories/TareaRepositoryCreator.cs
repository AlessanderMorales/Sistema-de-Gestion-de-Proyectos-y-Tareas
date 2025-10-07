using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    // Hereda de la misma clase base, pero con el modelo Tarea
    public class TareaRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Tarea>(connectionFactory)
    {
        public override IDB<Tarea> CreateRepository()
        {
            // Retorna la instancia del TareaRepository que ya creamos antes
            return new TareaRepository(this._connectionSingleton);
        }
    }
}