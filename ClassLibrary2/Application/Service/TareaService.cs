using System.Collections.Generic;
using System.Linq;
using ServiceCommon.Domain.Port;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceTarea.Domain.Entities;

namespace ServiceTarea.Application.Service
{
    public class TareaService
    {
        private readonly MySqlRepositoryFactory<Tarea> _tareaFactory;
        private readonly IComentarioManager _comentarioManager;

        public TareaService(MySqlRepositoryFactory<Tarea> tareaFactory, IComentarioManager comentarioManager)
        {
            _tareaFactory = tareaFactory;
            _comentarioManager = comentarioManager;
        }

        public IEnumerable<Tarea> ObtenerTodasLasTareas()
        {
            var repo = _tareaFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public IEnumerable<Tarea> ObtenerTareasPorUsuarioAsignado(int idUsuario)
        {
            var repo = _tareaFactory.CreateRepository();

            if (repo is Infrastructure.Persistence.Repositories.TareaRepository tareaRepo)
            {
                return tareaRepo.GetByAssignedUserId(idUsuario);
            }

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

            // 🔹 Desactivar comentarios asociados a esta tarea
            _comentarioManager.DesactivarPorTareaId(tarea.Id);
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