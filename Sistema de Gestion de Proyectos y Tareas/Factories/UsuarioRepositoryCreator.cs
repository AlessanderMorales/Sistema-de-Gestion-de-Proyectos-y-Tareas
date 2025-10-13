using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    public class UsuarioRepositoryCreator(MySqlConnectionSingleton connectionFactory) : MySqlRepositoryFactory<Usuario>(connectionFactory)
    {
        public override IDB<Usuario> CreateRepository()
        {
            return new UsuarioRepository(this._connectionSingleton);
        }
    }
}