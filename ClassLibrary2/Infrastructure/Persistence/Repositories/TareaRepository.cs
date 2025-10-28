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
        private readonly MySqlConnectionSingleton _connectionSingleton;

        public TareaRepository(MySqlConnectionSingleton connectionSingleton)
        {
            _connectionSingleton = connectionSingleton;
        }

        public IEnumerable<Tarea> GetAllAsync()
        {
            using var connection = _connectionSingleton.CreateConnection();

            string query = @"
                SELECT 
                    t.id_tarea AS Id, 
                    t.titulo AS Titulo, 
                    t.descripcion AS Descripcion, 
                    t.prioridad AS Prioridad, 
                    t.estado AS Estado,
                    t.id_proyecto AS IdProyecto,
                    t.id_usuario_asignado AS IdUsuarioAsignado,
                    t.status AS Status,
                    p.nombre AS ProyectoNombre,
                    CONCAT(u.nombres, ' ', u.primer_apellido) AS UsuarioAsignadoNombre
                FROM Tareas t
                LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
                LEFT JOIN Usuario u ON t.id_usuario_asignado = u.id_usuario
                WHERE t.estado = 1
                ORDER BY t.id_tarea DESC;";

            var tareas = connection.Query<Tarea>(query).ToList();
            return tareas;
        }

        public IEnumerable<Tarea> GetByAssignedUserId(int idUsuario)
        {
            using var connection = _connectionSingleton.CreateConnection();

            string query = @"
                SELECT DISTINCT
                    t.id_tarea AS Id, 
                    t.titulo AS Titulo, 
                    t.descripcion AS Descripcion, 
                    t.prioridad AS Prioridad, 
                    t.estado AS Estado,
                    t.id_proyecto AS IdProyecto,
                    t.id_usuario_asignado AS IdUsuarioAsignado,
                    t.status AS Status,
                    p.nombre AS ProyectoNombre,
                    CONCAT(u.nombres, ' ', u.primer_apellido) AS UsuarioAsignadoNombre
                FROM Tareas t
                INNER JOIN Tarea_Usuario tu ON t.id_tarea = tu.id_tarea
                LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
                LEFT JOIN Usuario u ON t.id_usuario_asignado = u.id_usuario
                WHERE t.estado = 1 AND tu.id_usuario = @UsuarioId AND tu.estado = 1
                ORDER BY t.id_tarea DESC;";

            var tareas = connection.Query<Tarea>(query, new { UsuarioId = idUsuario }).ToList();
            return tareas;
        }

        public Tarea GetByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    t.id_tarea AS Id, 
                    t.titulo AS Titulo, 
                    t.descripcion AS Descripcion, 
                    t.prioridad AS Prioridad, 
                    t.estado AS Estado,
                    t.id_proyecto AS IdProyecto,
                    t.id_usuario_asignado AS IdUsuarioAsignado,
                    t.status AS Status,
                    p.nombre AS ProyectoNombre,
                    CONCAT(u.nombres, ' ', u.primer_apellido) AS UsuarioAsignadoNombre
                FROM Tareas t
                LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
                LEFT JOIN Usuario u ON t.id_usuario_asignado = u.id_usuario
                WHERE t.id_tarea = @Id AND t.estado = 1;";

            return _connectionSingleton.QueryFirstOrDefault<Tarea>(query, new { Id = id });
        }

        public void AddAsync(Tarea entity)
        {
            string query = @"
                INSERT INTO Tareas 
                    (titulo, descripcion, prioridad, id_proyecto, id_usuario_asignado, status, estado)
                VALUES 
                    (@Titulo, @Descripcion, @Prioridad, @IdProyecto, @IdUsuarioAsignado, @Status, 1);";

            _connectionSingleton.ExcuteCommand(query, entity);
        }

        public void UpdateAsync(Tarea entity)
        {
            string query = @"
                UPDATE Tareas
                SET titulo = @Titulo,
                    descripcion = @Descripcion,
                    prioridad = @Prioridad,
                    id_proyecto = @IdProyecto,
                    id_usuario_asignado = @IdUsuarioAsignado,
                    status = @Status
                WHERE id_tarea = @Id;";

            _connectionSingleton.ExcuteCommand(query, entity);
        }

        public void DeleteAsync(int id)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_tarea = @Id;";
            _connectionSingleton.ExcuteCommand(query, new { Id = id });
        }

        public void DeleteAsync(Tarea entity)
        {
            DeleteAsync(entity.Id);
        }

        public void DeactivateByProjectId(int idProyecto)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @IdProyecto;";
            _connectionSingleton.ExcuteCommand(query, new { IdProyecto = idProyecto });
        }
    }
}