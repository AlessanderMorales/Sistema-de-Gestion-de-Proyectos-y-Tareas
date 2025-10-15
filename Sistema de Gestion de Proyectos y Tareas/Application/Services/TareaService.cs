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

        public IEnumerable<Tarea> ObtenerTareasPorUsuario(int userId)
        {
            var repo = _tareaFactory.CreateRepository() as dynamic;
            try
            {
                return repo.GetAllByUserId(userId);
            }
            catch
            {
                // fallback: return all if repo doesn't implement specific method
                return repo.GetAllAsync();
            }
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
    }
}