using System.Collections.Generic;
using System.Linq;
using ServiceCommon.Domain.Port;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceTarea.Domain.Entities;
using ServiceTarea.Infrastructure.Persistence.Repositories;
using Dapper;

namespace ServiceTarea.Application.Service
{
    public class TareaService
    {
        private readonly MySqlRepositoryFactory<Tarea> _tareaFactory;
        private readonly IComentarioManager _comentarioManager;
        private readonly MySqlConnectionSingleton _connectionSingleton;

        public TareaService(MySqlRepositoryFactory<Tarea> tareaFactory, IComentarioManager comentarioManager, MySqlConnectionSingleton connectionSingleton)
        {
            _tareaFactory = tareaFactory;
            _comentarioManager = comentarioManager;
            _connectionSingleton = connectionSingleton;
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

            _comentarioManager.DesactivarPorTareaId(tarea.Id);
        }

        public bool AsignarTareaAUsuario(int idTarea, int idUsuario)
        {
            var repo = _tareaFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(idTarea);
       
         if (tarea == null)
       return false;

            tarea.IdUsuarioAsignado = idUsuario;
       repo.UpdateAsync(tarea);

          var tareaUsuarioRepo = new TareaUsuarioRepository(_connectionSingleton);
            tareaUsuarioRepo.AsignarUsuario(idTarea, idUsuario);

   bool usuarioAsignadoAlProyecto = VerificarUsuarioEnProyecto(tarea.IdProyecto, idUsuario);

            if (!usuarioAsignadoAlProyecto)
      {
           AsignarUsuarioAProyecto(tarea.IdProyecto, idUsuario);
      return true;
  }

        return false;
  }

  public void AsignarMultiplesUsuarios(int idTarea, List<int> idsUsuarios)
        {
          var repo = _tareaFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(idTarea);

         if (tarea == null)
  return;

  var tareaUsuarioRepo = new TareaUsuarioRepository(_connectionSingleton);
            tareaUsuarioRepo.ReemplazarUsuarios(idTarea, idsUsuarios);

 foreach (var idUsuario in idsUsuarios)
      {
        bool usuarioEnProyecto = VerificarUsuarioEnProyecto(tarea.IdProyecto, idUsuario);
                
 if (!usuarioEnProyecto)
             {
    AsignarUsuarioAProyecto(tarea.IdProyecto, idUsuario);
          }
            }
        }

 public IEnumerable<int> ObtenerIdsUsuariosAsignados(int idTarea)
        {
    var tareaUsuarioRepo = new TareaUsuarioRepository(_connectionSingleton);
    return tareaUsuarioRepo.GetUsuariosIdsByTareaId(idTarea);
        }

        private bool VerificarUsuarioEnProyecto(int idProyecto, int idUsuario)
        {
    using var connection = _connectionSingleton.CreateConnection();

            string query = @"
  SELECT COUNT(*) 
           FROM Proyecto_Usuario 
  WHERE id_proyecto = @IdProyecto 
      AND id_usuario = @IdUsuario 
          AND estado = 1;";

            var count = connection.QueryFirstOrDefault<int>(query, new { IdProyecto = idProyecto, IdUsuario = idUsuario });
 return count > 0;
        }

        private void AsignarUsuarioAProyecto(int idProyecto, int idUsuario)
{
   using var connection = _connectionSingleton.CreateConnection();
        
            connection.Execute(
       "CALL sp_asignar_usuario_proyecto(@IdProyecto, @IdUsuario)",
     new { IdProyecto = idProyecto, IdUsuario = idUsuario });
        }
    }
}