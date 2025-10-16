using System.Collections.Generic;
using System.Linq;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services
{
    public class TareaService
    {
        private readonly MySqlRepositoryFactory<Tarea> _tareaFactory;

        public TareaService(MySqlRepositoryFactory<Tarea> tareaFactory)
        {
            _tareaFactory = tareaFactory;
        }

        public IEnumerable<Tarea> ObtenerTodasLasTareas()
        {
            var repo = _tareaFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public IEnumerable<Tarea> ObtenerTareasPorUsuarioAsignado(int idUsuario)
        {
            var repo = _tareaFactory.CreateRepository();

            // Si el repositorio concreto proporciona un método optimizado por consulta, úsalo.
            if (repo is Infrastructure.Persistence.Repositories.TareaRepository tareaRepo)
            {
                return tareaRepo.GetByAssignedUserId(idUsuario);
            }

            // Recurso de respaldo: filtrar en memoria
            return repo.GetAllAsync()
                       .Where(t => t.IdUsuarioAsignado.HasValue && t.IdUsuarioAsignado.Value == idUsuario);
        }

        public Tarea ObtenerTareaPorId(int id)
        {
            var repo = _tareaFactory.CreateRepository();
            return repo.GetByIdAsync(id);
        }

        public void CrearNuevaTarea(Tarea tarea)
        {
            var repo = _tareaFactory.CreateRepository();
            repo.AddAsync(tarea);
        }

        public void ActualizarTarea(Tarea tarea)
        {
            var repo = _tareaFactory.CreateRepository();
            repo.UpdateAsync(tarea);
        }

        public void EliminarTarea(Tarea tarea)
        {
            var repo = _tareaFactory.CreateRepository();
            repo.DeleteAsync(tarea);
        }

        public void AsignarTareaAUsuario(int idTarea, int idUsuario)
        {
            var repo = _tareaFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(idTarea);
            if (tarea != null)
            {
                tarea.IdUsuarioAsignado = idUsuario;
                repo.UpdateAsync(tarea);
            }
        }
    }
}