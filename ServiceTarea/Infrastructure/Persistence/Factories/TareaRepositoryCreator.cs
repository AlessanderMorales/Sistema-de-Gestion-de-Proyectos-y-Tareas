
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using ServiceCommon.Infrastructure.Persistence.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories
{
    public class TareaRepositoryCreator : MySqlRepositoryFactory<Tarea>
    {
        public TareaRepositoryCreator(MySqlConnectionSingleton connectionFactory) : base(connectionFactory) { }
        public override IDB<Tarea> CreateRepository()
        {
            return new TareaRepository(this._connectionSingleton);
        }
    }
}