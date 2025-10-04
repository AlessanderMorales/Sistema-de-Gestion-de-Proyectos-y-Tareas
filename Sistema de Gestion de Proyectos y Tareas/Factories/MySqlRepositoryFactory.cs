using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories
{
    public class MySqlRepositoryFactory : IRepositoryFactory
    {
        private readonly IDbConnectionSingleton _connectionFactory;

        public MySqlRepositoryFactory(IDbConnectionSingleton connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        
        public IDB<Proyecto> CreateProyectoRepository() => (IDB<Proyecto>)new ProyectoRepository(_connectionFactory);
        public IDB<Usuario> CreateUsuarioRepository() => new UsuarioRepository(_connectionFactory);
        public IDB<Tarea> CreateTareaRepository() => new TareaRepository(_connectionFactory);
    }
}