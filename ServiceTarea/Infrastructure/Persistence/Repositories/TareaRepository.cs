using Dapper;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories
{

    public class TareaRepository : IDB<Tarea>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public TareaRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }



        public IEnumerable<Tarea> GetAllAsync()
        {
            using var connection = _connectionSignleton.CreateConnection();
            string query = @"
        SELECT 
            -- Seleccionamos explícitamente las columnas de Tarea
            t.id_tarea AS Id, 
            t.titulo, 
            t.descripcion, 
            t.prioridad, 
            t.estado,
            t.id_proyecto,
            t.id_usuario_asignado AS IdUsuarioAsignado,
            t.status,

            -- Seleccionamos explícitamente las columnas de Proyecto
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

            var tareas = connection.Query<Tarea, Proyecto, Tarea>(
                query,
                (tarea, proyecto) =>
                {
                    tarea.Proyecto = proyecto;
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
            t.id_proyecto,
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

            var tareas = connection.Query<Tarea, Proyecto, Tarea>(
                query,
                (tarea, proyecto) =>
                {
                    tarea.Proyecto = proyecto;
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
                              VALUES (@Titulo, @Descripcion, @Prioridad, @id_proyecto, @IdUsuarioAsignado, @Status);";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public Tarea GetByIdAsync(int id)
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado, id_proyecto, id_usuario_asignado AS IdUsuarioAsignado, status
                              FROM Tareas
                              WHERE id_tarea = @Id AND estado = 1;";

            var parameters = new { Id = id };
            return _connectionSignleton.QueryFirstOrDefault<Tarea>(query, parameters);
        }

        public void UpdateAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                              SET titulo = @Titulo,
                              descripcion = @Descripcion,
                              prioridad = @Prioridad,
                              id_proyecto = @id_proyecto,
                              id_usuario_asignado = @IdUsuarioAsignado,
                              status = @Status
                              WHERE id_tarea = @Id;";

            _connectionSignleton.ExcuteCommand(query, entity);
        }


        public void DeleteAsync(int id)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_tarea = @Id;";
            using var connection = _connectionSignleton.CreateConnection();
            connection.Execute(query, new { Id = id });
        }

        public void DeleteAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                             SET estado = 0
                             WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public void DeactivateByProjectId(int idProyecto)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_proyecto = @IdProyecto;";

            using var connection = _connectionSignleton.CreateConnection();
            connection.Execute(query, new { IdProyecto = idProyecto });
        }


    }
}