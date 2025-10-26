using Dapper;
using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceTarea.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServiceTarea.Infrastructure.Persistence.Repositories
{
    public class TareaRepository : IDB<Tarea>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public TareaRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }

        // ✅ Clase interna local que representa solo lo que necesitamos del proyecto
        private class ProyectoLite
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string? Descripcion { get; set; }
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public int Estado { get; set; }
        }

        public IEnumerable<Tarea> GetAllAsync()
        {
            using var connection = _connectionSignleton.CreateConnection();

            string query = @"
                SELECT 
                    t.id_tarea AS Id, 
                    t.titulo, 
                    t.descripcion, 
                    t.prioridad, 
                    t.estado,
                    t.id_proyecto AS IdProyecto,
                    t.id_usuario_asignado AS IdUsuarioAsignado,
                    t.status,
                    p.id_proyecto AS Id,
                    p.nombre,
                    p.descripcion,
                    p.fecha_inicio AS FechaInicio,
                    p.fecha_fin AS FechaFin,
                    p.estado
                FROM Tareas t
                LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
                WHERE t.estado = 1 
                ORDER BY t.id_tarea DESC";

            // 🔹 Usamos ProyectoLite en lugar de Proyecto
            var tareas = connection.Query<Tarea, ProyectoLite, Tarea>(
                query,
                (tarea, proyecto) =>
                {
                    // Guardamos solo el ID del proyecto y opcionalmente nombre
                    tarea.IdProyecto = proyecto.Id;
                    return tarea;
                },
                splitOn: "Id"
            );

            return tareas;
        }

        public IEnumerable<Tarea> GetByAssignedUserId(int idUsuario)
        {
            using var connection = _connectionSignleton.CreateConnection();
            string query = @"
                SELECT 
                    t.id_tarea AS Id, 
                    t.titulo, 
                    t.descripcion, 
                    t.prioridad, 
                    t.estado,
                    t.id_proyecto AS IdProyecto,
                    t.id_usuario_asignado AS IdUsuarioAsignado,
                    t.status,
                    p.id_proyecto AS Id,
                    p.nombre,
                    p.descripcion,
                    p.fecha_inicio AS FechaInicio,
                    p.fecha_fin AS FechaFin,
                    p.estado
                FROM Tareas t
                LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
                WHERE t.estado = 1 AND t.id_usuario_asignado = @UsuarioId
                ORDER BY t.id_tarea DESC";

            var tareas = connection.Query<Tarea, ProyectoLite, Tarea>(
                query,
                (tarea, proyecto) =>
                {
                    tarea.IdProyecto = proyecto.Id;
                    return tarea;
                },
                new { UsuarioId = idUsuario },
                splitOn: "Id"
            );

            return tareas;
        }

        public void AddAsync(Tarea entity)
        {
            string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad, id_proyecto, id_usuario_asignado, status)
                             VALUES (@Titulo, @Descripcion, @Prioridad, @IdProyecto, @IdUsuarioAsignado, @Status);";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public Tarea GetByIdAsync(int id)
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado, id_proyecto AS IdProyecto, id_usuario_asignado AS IdUsuarioAsignado, status
                             FROM Tareas
                             WHERE id_tarea = @Id AND estado = 1;";

            return _connectionSignleton.QueryFirstOrDefault<Tarea>(query, new { Id = id });
        }

        public void UpdateAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                             SET titulo = @Titulo,
                                 descripcion = @Descripcion,
                                 prioridad = @Prioridad,
                                 id_proyecto = @IdProyecto,
                                 id_usuario_asignado = @IdUsuarioAsignado,
                                 status = @Status
                             WHERE id_tarea = @Id;";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public void DeleteAsync(int id)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand(query, new { Id = id });
        }

        public void DeleteAsync(Tarea entity)
        {
            DeleteAsync(entity.Id);
        }

        public void DeactivateByProjectId(int idProyecto)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @IdProyecto;";
            _connectionSignleton.ExcuteCommand(query, new { IdProyecto = idProyecto });
        }
    }
}
