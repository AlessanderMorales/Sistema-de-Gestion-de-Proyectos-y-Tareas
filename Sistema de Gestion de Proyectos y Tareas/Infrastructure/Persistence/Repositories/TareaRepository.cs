using Dapper;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

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
            t.assigned_user_id AS AssignedUserId,

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

            try
            {
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
            catch (MySqlException ex) when (ex.Message != null && ex.Message.Contains("assigned_user_id"))
            {
                // fallback: database doesn't have assigned_user_id column — run query without it
                string fallback = @"
        SELECT 
            t.id_tarea AS Id, 
            t.titulo, 
            t.descripcion, 
            t.prioridad, 
            t.estado,
            t.id_proyecto,
            NULL AS AssignedUserId,

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
                    fallback,
                    (tarea, proyecto) =>
                    {
                        tarea.Proyecto = proyecto;
                        return tarea;
                    },
                    splitOn: "Id"
                );
                return tareas;
            }
        }

        public IEnumerable<Tarea> GetAllByUserId(int userId)
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
            t.assigned_user_id AS AssignedUserId,

            -- Seleccionamos explícitamente las columnas de Proyecto
            p.id_proyecto AS Id,
            p.nombre,
            p.descripcion,
            p.fecha_inicio AS FechaInicio,
            p.fecha_fin AS FechaFin,
            p.estado
        FROM Tareas t
        LEFT JOIN Proyecto p ON t.id_proyecto = p.id_proyecto
        WHERE t.estado = 1 AND t.assigned_user_id = @UserId
        ORDER BY t.id_tarea DESC";

            try
            {
                var tareas = connection.Query<Tarea, Proyecto, Tarea>(
                    query,
                    (tarea, proyecto) =>
                    {
                        tarea.Proyecto = proyecto;
                        return tarea;
                    },
                    new { UserId = userId },
                    splitOn: "Id"
                );
                return tareas;
            }
            catch (MySqlException ex) when (ex.Message != null && ex.Message.Contains("assigned_user_id"))
            {
                // Column missing, cannot filter by user — return empty list to be safe
                return Enumerable.Empty<Tarea>();
            }
        }


        public void AddAsync(Tarea entity)
        {

            string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad, id_proyecto, assigned_user_id)
                              VALUES (@Titulo, @Descripcion, @Prioridad, @id_proyecto, @AssignedUserId);";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        public Tarea GetByIdAsync(int id)
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado, id_proyecto, assigned_user_id AS AssignedUserId
                              FROM Tareas
                              WHERE id_tarea = @Id AND estado = 1;";

            var parameters = new { Id = id };
            try
            {
                return _connectionSignleton.QueryFirstOrDefault<Tarea>(query, parameters);
            }
            catch (MySqlException ex) when (ex.Message != null && ex.Message.Contains("assigned_user_id"))
            {
                // fallback without assigned_user_id
                string fallback = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado, id_proyecto, NULL AS AssignedUserId
                              FROM Tareas
                              WHERE id_tarea = @Id AND estado = 1;";
                return _connectionSignleton.QueryFirstOrDefault<Tarea>(fallback, parameters);
            }
        }

        public void UpdateAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                              SET titulo = @Titulo,
                              descripcion = @Descripcion,
                              prioridad = @Prioridad,
                              id_proyecto = @id_proyecto,
                              assigned_user_id = @AssignedUserId
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