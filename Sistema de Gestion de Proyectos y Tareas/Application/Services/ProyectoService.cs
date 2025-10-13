// Archivo: Application/Services/ProyectoService.cs

// --- USINGS NECESARIOS ---
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories; // Usamos la fábrica que ya tienes

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services
{
    public class ProyectoService
    {
        private readonly MySqlRepositoryFactory<Proyecto> _proyectoFactory;
        public ProyectoService(MySqlRepositoryFactory<Proyecto> proyectoFactory)
        {
            _proyectoFactory = proyectoFactory;
        }
        public IEnumerable<Proyecto> ObtenerTodosLosProyectos()
        {
            var repo = _proyectoFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public Proyecto ObtenerProyectoPorId(int id)
        {
            var repo = _proyectoFactory.CreateRepository();
            return repo.GetByIdAsync(id);
        }
        public void CrearNuevoProyecto(Proyecto proyecto)
        {
            var repo = _proyectoFactory.CreateRepository();
            repo.AddAsync(proyecto);
        }
        public void ActualizarProyecto(Proyecto proyecto)
        {
            var repo = _proyectoFactory.CreateRepository();
            repo.UpdateAsync(proyecto);
        }
        public void EliminarProyecto(Proyecto proyecto)
        {
            var repo = _proyectoFactory.CreateRepository();
            repo.DeleteAsync(proyecto);
        }
    }
}