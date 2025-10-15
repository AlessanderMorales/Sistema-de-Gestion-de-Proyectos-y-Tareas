using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories
{
    public class ComentarioRepositoryCreator(MySqlConnectionSingleton connectionFactory)
        : MySqlRepositoryFactory<Comentario>(connectionFactory)
    {
        public override IDB<Comentario> CreateRepository()
        {
            return new ComentarioRepository(connectionFactory);
        }
    }
}
