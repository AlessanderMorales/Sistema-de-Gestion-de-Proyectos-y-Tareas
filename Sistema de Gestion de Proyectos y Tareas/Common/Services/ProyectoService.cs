using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Common.Services
{
    public class ProyectoService
    {
        private readonly MySqlRepositoryFactory<Proyecto> _proyectoFactory;
        private readonly MySqlRepositoryFactory<Tarea> _tareaFactory;
        public ProyectoService(MySqlRepositoryFactory<Proyecto> proyectoFactory, MySqlRepositoryFactory<Tarea> tareaFactory)
        {
            _proyectoFactory = proyectoFactory;
            _tareaFactory = tareaFactory;
        }
        public IEnumerable<Proyecto> ObtenerTodosLosProyectos()
        {
            var repo = _proyectoFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public IEnumerable<Proyecto> ObtenerProyectosPorUsuarioAsignado(int idUsuario)
        {
            var repo = _proyectoFactory.CreateRepository();
            if (repo is ProyectoRepository pr)
            {
                return pr.GetProjectsByAssignedUserId(idUsuario);
            }
            return Enumerable.Empty<Proyecto>();
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

        public Proyecto ObtenerProyectoConTareas(int idProyecto)
        {
            var repoBase = _proyectoFactory.CreateRepository();
            var repoConcreto = repoBase as ProyectoRepository;
            if (repoConcreto != null)
            {
                return repoConcreto.GetByIdConTareas(idProyecto);
            }
            return null;
        }

        public void EliminarProyectoPorId(int id)
        {
            var tareaRepo = _tareaFactory.CreateRepository();
            tareaRepo.DeactivateByProjectId(id);
            var proyectoRepo = _proyectoFactory.CreateRepository();
            proyectoRepo.DeleteAsync(id);
        }


    }
}