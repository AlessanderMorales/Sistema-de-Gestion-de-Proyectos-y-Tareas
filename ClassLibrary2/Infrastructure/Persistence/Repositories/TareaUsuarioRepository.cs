using Dapper;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceTarea.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServiceTarea.Infrastructure.Persistence.Repositories
{
    public class TareaUsuarioRepository
    {
        private readonly MySqlConnectionSingleton _connectionSingleton;

        public TareaUsuarioRepository(MySqlConnectionSingleton connectionSingleton)
        {
            _connectionSingleton = connectionSingleton;
        }

        /// <summary>
        /// Obtener todos los IDs de usuarios asignados a una tarea
        /// </summary>
        public IEnumerable<int> GetUsuariosIdsByTareaId(int idTarea)
        {
            string query = @"
    SELECT id_usuario
           FROM Tarea_Usuario
       WHERE id_tarea = @IdTarea AND estado = 1
      ORDER BY fecha_asignacion;";

            using var connection = _connectionSingleton.CreateConnection();
            return connection.Query<int>(query, new { IdTarea = idTarea }).ToList();
        }

        /// <summary>
        /// Obtener todas las tareas asignadas a un usuario
        /// </summary>
        public IEnumerable<int> GetTareasIdsByUsuarioId(int idUsuario)
        {
            string query = @"
    SELECT id_tarea
       FROM Tarea_Usuario
       WHERE id_usuario = @IdUsuario AND estado = 1;";

            using var connection = _connectionSingleton.CreateConnection();
            return connection.Query<int>(query, new { IdUsuario = idUsuario }).ToList();
        }

        /// <summary>
        /// Asignar un usuario a una tarea
        /// </summary>
        public void AsignarUsuario(int idTarea, int idUsuario)
        {
            string query = @"
        INSERT INTO Tarea_Usuario (id_tarea, id_usuario, estado)
           VALUES (@IdTarea, @IdUsuario, 1)
ON DUPLICATE KEY UPDATE estado = 1, fecha_asignacion = NOW();";

            _connectionSingleton.ExcuteCommand(query, new { IdTarea = idTarea, IdUsuario = idUsuario });
        }

        /// <summary>
        /// Remover un usuario de una tarea
        /// </summary>
        public void RemoverUsuario(int idTarea, int idUsuario)
        {
            string query = @"
 UPDATE Tarea_Usuario
         SET estado = 0
         WHERE id_tarea = @IdTarea AND id_usuario = @IdUsuario;";

            _connectionSingleton.ExcuteCommand(query, new { IdTarea = idTarea, IdUsuario = idUsuario });
        }

        /// <summary>
        /// Reemplazar todos los usuarios asignados a una tarea
        /// </summary>
        public void ReemplazarUsuarios(int idTarea, List<int> idsUsuarios)
        {
            using var connection = _connectionSingleton.CreateConnection();
            
  // Asegurarse de que la conexión esté abierta
    if (connection.State != System.Data.ConnectionState.Open)
    {
     connection.Open();
    }
    
    using var transaction = connection.BeginTransaction();

    try
    {
      // Desactivar todas las asignaciones actuales
 string deleteQuery = @"
            UPDATE Tarea_Usuario
  SET estado = 0
   WHERE id_tarea = @IdTarea;";
     
        connection.Execute(deleteQuery, new { IdTarea = idTarea }, transaction);

        // Insertar/reactivar las nuevas asignaciones
        if (idsUsuarios != null && idsUsuarios.Any())
        {
  string insertQuery = @"
     INSERT INTO Tarea_Usuario (id_tarea, id_usuario, estado)
 VALUES (@IdTarea, @IdUsuario, 1)
          ON DUPLICATE KEY UPDATE estado = 1, fecha_asignacion = NOW();";

            foreach (var idUsuario in idsUsuarios)
            {
     connection.Execute(insertQuery, new { IdTarea = idTarea, IdUsuario = idUsuario }, transaction);
    }
   }

        transaction.Commit();
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}

  /// <summary>
    /// Verificar si un usuario está asignado a una tarea
   /// </summary>
        public bool UsuarioEstaAsignado(int idTarea, int idUsuario)
     {
      string query = @"
        SELECT COUNT(*)
    FROM Tarea_Usuario
    WHERE id_tarea = @IdTarea AND id_usuario = @IdUsuario AND estado = 1;";

     using var connection = _connectionSingleton.CreateConnection();
 var count = connection.QueryFirstOrDefault<int>(query, new { IdTarea = idTarea, IdUsuario = idUsuario });
   return count > 0;
  }
    }
}
