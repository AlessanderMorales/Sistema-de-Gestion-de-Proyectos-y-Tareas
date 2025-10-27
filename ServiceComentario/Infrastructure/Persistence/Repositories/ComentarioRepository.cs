using Dapper;
using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceComentario.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServiceComentario.Infrastructure.Persistence.Repositories
{
    public class ComentarioRepository : IDB<Comentario>
    {
        private readonly MySqlConnectionSingleton _connectionSingleton;

        public ComentarioRepository(MySqlConnectionSingleton connectionSingleton)
        {
            _connectionSingleton = connectionSingleton;
        }

        public IEnumerable<Comentario> GetAllAsync()
        {
            string query = @"SELECT c.id_comentario AS Id, c.contenido AS Contenido, c.fecha AS Fecha, 
                                    c.estado AS Estado, c.id_tarea AS IdTarea, c.id_usuario AS IdUsuario,
                                    t.id_tarea AS Tarea_Id, t.titulo AS Tarea_Titulo,
                                    u.id_usuario AS Usuario_Id, u.primer_nombre AS Usuario_PrimerNombre, u.apellidos AS Usuario_Apellidos
                             FROM Comentario c
                             LEFT JOIN Tareas t ON c.id_tarea = t.id_tarea
                             LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
                             WHERE c.estado = 1;";

            using var connection = _connectionSingleton.CreateConnection();
            var rows = connection.Query<dynamic>(query).ToList();

            var comentarios = rows.Select(r => new Comentario
            {
                Id = r.Id,
                Contenido = r.Contenido,
                Fecha = r.Fecha,
                Estado = r.Estado,
                IdTarea = r.IdTarea,
                IdUsuario = r.IdUsuario,
                Tarea = r.Tarea_Id != null ? new Tarea { Id = r.Tarea_Id, Titulo = r.Tarea_Titulo } : null,
                Usuario = r.Usuario_Id != null ? new Usuario { Id = r.Usuario_Id, PrimerNombre = r.Usuario_PrimerNombre, Apellidos = r.Usuario_Apellidos } : null
            }).ToList();

            return comentarios;
        }

        public Comentario GetByIdAsync(int id)
        {
            string query = @"SELECT c.id_comentario AS Id, c.contenido AS Contenido, c.fecha AS Fecha, 
                                    c.estado AS Estado, c.id_tarea AS IdTarea, c.id_usuario AS IdUsuario,
                                    t.id_tarea AS Tarea_Id, t.titulo AS Tarea_Titulo,
                                    u.id_usuario AS Usuario_Id, u.primer_nombre AS Usuario_PrimerNombre, u.apellidos AS Usuario_Apellidos
                             FROM Comentario c
                             LEFT JOIN Tareas t ON c.id_tarea = t.id_tarea
                             LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
                             WHERE c.id_comentario = @Id;";
            var parameters = new { Id = id };
            using var connection = _connectionSingleton.CreateConnection();
            var r = connection.QueryFirstOrDefault<dynamic>(query, parameters);
            if (r == null) return null;
            return new Comentario
            {
                Id = r.Id,
                Contenido = r.Contenido,
                Fecha = r.Fecha,
                Estado = r.Estado,
                IdTarea = r.IdTarea,
                IdUsuario = r.IdUsuario,
                Tarea = r.Tarea_Id != null ? new Tarea { Id = r.Tarea_Id, Titulo = r.Tarea_Titulo } : null,
                Usuario = r.Usuario_Id != null ? new Usuario { Id = r.Usuario_Id, PrimerNombre = r.Usuario_PrimerNombre, Apellidos = r.Usuario_Apellidos } : null
            };
        }

        public void AddAsync(Comentario entity)
        {
            string query = @"INSERT INTO Comentario (contenido, fecha, id_tarea, id_usuario, estado)
                     VALUES (@Contenido, @Fecha, @IdTarea, @IdUsuario, @Estado);";

            _connectionSingleton.ExcuteCommand(query, entity);
        }

        public void UpdateAsync(Comentario entity)
        {
            // Recuperamos el estado actual de la base de datos para no perderlo
            string getQuery = "SELECT estado FROM Comentario WHERE id_comentario = @Id;";
            var actual = _connectionSingleton.QueryFirstOrDefault<Comentario>(getQuery, new { entity.Id });

            int estadoActual = actual != null ? actual.Estado : 1;

            string query = @"UPDATE Comentario 
                     SET contenido = @Contenido, estado = @Estado
                     WHERE id_comentario = @Id;";
            _connectionSingleton.ExcuteCommand(query, new
            {
                entity.Contenido,
                Estado = estadoActual,
                entity.Id
            });
        }

        public void DeleteAsync(int id)
        {
            string query = @"UPDATE Comentario SET Estado = 0 WHERE id_comentario = @Id;";
            _connectionSingleton.ExcuteCommand<dynamic>(query, new { Id = id });
        }

        public void DeleteAsync(Comentario entity)
        {
            DeleteAsync(entity.Id);
        }

        public void DeactivateByProjectId(int idProyecto)
        {

        }

        public IEnumerable<Comentario> GetAllWhere(string whereClause, object parameters = null)
        {

            string query = $"SELECT * FROM Comentario WHERE {whereClause};";
            using var connection = _connectionSingleton.CreateConnection();
            return connection.Query<Comentario>(query, parameters);
        }
    }
}
