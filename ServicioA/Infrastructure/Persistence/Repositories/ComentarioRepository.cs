using Dapper;
using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceComentario.Domain.Entities;
using ServiceUsuario.Domain.Entities;
using ServiceTarea.Domain.Entities;
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
            string query = @"
                SELECT 
                    c.id_comentario AS Id, 
                    c.contenido AS Contenido, 
                    c.fecha AS Fecha, 
                    c.estado AS Estado, 
                    c.id_tarea AS IdTarea, 
                    c.id_usuario AS IdUsuario,
                    u.id_usuario AS Usuario_Id, 
                    u.nombres AS Usuario_Nombres, 
                    u.primer_apellido AS Usuario_PrimerApellido,
                    u.segundo_apellido AS Usuario_SegundoApellido,
                    u.email AS Usuario_Email,
                    u.rol AS Usuario_Rol,
                    t.id_tarea AS Tarea_Id,
                    t.titulo AS Tarea_Titulo,
                    t.id_proyecto AS Tarea_IdProyecto
                FROM Comentario c
                LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
                LEFT JOIN Tareas t ON c.id_tarea = t.id_tarea
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
                Usuario = r.Usuario_Id != null
                    ? new Usuario
                    {
                        Id = r.Usuario_Id,
                        Nombres = r.Usuario_Nombres,
                        PrimerApellido = r.Usuario_PrimerApellido,
                        SegundoApellido = r.Usuario_SegundoApellido,
                        Email = r.Usuario_Email,
                        Rol = r.Usuario_Rol
                    }
                    : null,
                Tarea = r.Tarea_Id != null
                    ? new Tarea
                    {
                        Id = r.Tarea_Id,
                        Titulo = r.Tarea_Titulo,
                        IdProyecto = r.Tarea_IdProyecto
                    }
                    : null
            }).ToList();

            return comentarios;
        }

        public Comentario GetByIdAsync(int id)
        {
            string query = @"
                SELECT 
                    c.id_comentario AS Id, 
                    c.contenido AS Contenido, 
                    c.fecha AS Fecha, 
                    c.estado AS Estado, 
                    c.id_tarea AS IdTarea, 
                    c.id_usuario AS IdUsuario,
                    u.id_usuario AS Usuario_Id, 
                    u.nombres AS Usuario_Nombres, 
                    u.primer_apellido AS Usuario_PrimerApellido,
                    u.segundo_apellido AS Usuario_SegundoApellido,
                    u.email AS Usuario_Email,
                    u.rol AS Usuario_Rol,
                    t.id_tarea AS Tarea_Id,
                    t.titulo AS Tarea_Titulo,
                    t.id_proyecto AS Tarea_IdProyecto
                FROM Comentario c
                LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
                LEFT JOIN Tareas t ON c.id_tarea = t.id_tarea
                WHERE c.id_comentario = @Id;";

            using var connection = _connectionSingleton.CreateConnection();
            var r = connection.QueryFirstOrDefault<dynamic>(query, new { Id = id });
            if (r == null) return null;

            return new Comentario
            {
                Id = r.Id,
                Contenido = r.Contenido,
                Fecha = r.Fecha,
                Estado = r.Estado,
                IdTarea = r.IdTarea,
                IdUsuario = r.IdUsuario,
                Usuario = r.Usuario_Id != null
                    ? new Usuario
                    {
                        Id = r.Usuario_Id,
                        Nombres = r.Usuario_Nombres,
                        PrimerApellido = r.Usuario_PrimerApellido,
                        SegundoApellido = r.Usuario_SegundoApellido,
                        Email = r.Usuario_Email,
                        Rol = r.Usuario_Rol
                    }
                    : null,
                Tarea = r.Tarea_Id != null
                    ? new Tarea
                    {
                        Id = r.Tarea_Id,
                        Titulo = r.Tarea_Titulo,
                        IdProyecto = r.Tarea_IdProyecto
                    }
                    : null
            };
        }

        public void AddAsync(Comentario entity)
        {
            string query = @"
                INSERT INTO Comentario (contenido, fecha, id_tarea, id_usuario, estado)
                VALUES (@Contenido, @Fecha, @IdTarea, @IdUsuario, @Estado);";

            _connectionSingleton.ExcuteCommand(query, entity);
        }

        public void UpdateAsync(Comentario entity)
        {
            string getQuery = "SELECT estado FROM Comentario WHERE id_comentario = @Id;";
            var actual = _connectionSingleton.QueryFirstOrDefault<Comentario>(getQuery, new { entity.Id });

            int estadoActual = actual != null ? actual.Estado : 1;

            string query = @"
                UPDATE Comentario 
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
            string query = @"UPDATE Comentario SET estado = 0 WHERE id_comentario = @Id;";
            _connectionSingleton.ExcuteCommand(query, new { Id = id });
        }

        public void DeleteAsync(Comentario entity)
        {
            DeleteAsync(entity.Id);
        }

        public void DeactivateByProjectId(int idProyecto)
        {

        }

        public IEnumerable<Comentario> GetByTareaId(int idTarea)
        {
            string query = @"
                SELECT 
                    c.id_comentario AS Id, 
                    c.contenido AS Contenido, 
                    c.fecha AS Fecha, 
                    c.estado AS Estado, 
                    c.id_tarea AS IdTarea, 
                    c.id_usuario AS IdUsuario,
                    u.id_usuario AS Usuario_Id, 
                    u.nombres AS Usuario_Nombres, 
                    u.primer_apellido AS Usuario_PrimerApellido,
                    u.segundo_apellido AS Usuario_SegundoApellido,
                    u.email AS Usuario_Email,
                    u.rol AS Usuario_Rol,
                    t.id_tarea AS Tarea_Id,
                    t.titulo AS Tarea_Titulo,
                    t.id_proyecto AS Tarea_IdProyecto
                FROM Comentario c
                LEFT JOIN Usuario u ON c.id_usuario = u.id_usuario
                LEFT JOIN Tareas t ON c.id_tarea = t.id_tarea
                WHERE c.id_tarea = @IdTarea AND c.estado = 1;";

            using var connection = _connectionSingleton.CreateConnection();
            var rows = connection.Query<dynamic>(query, new { IdTarea = idTarea }).ToList();

            var comentarios = rows.Select(r => new Comentario
            {
                Id = r.Id,
                Contenido = r.Contenido,
                Fecha = r.Fecha,
                Estado = r.Estado,
                IdTarea = r.IdTarea,
                IdUsuario = r.IdUsuario,
                Usuario = r.Usuario_Id != null
                    ? new Usuario
                    {
                        Id = r.Usuario_Id,
                        Nombres = r.Usuario_Nombres,
                        PrimerApellido = r.Usuario_PrimerApellido,
                        SegundoApellido = r.Usuario_SegundoApellido,
                        Email = r.Usuario_Email,
                        Rol = r.Usuario_Rol
                    }
                    : null,
                Tarea = r.Tarea_Id != null
                    ? new Tarea
                    {
                        Id = r.Tarea_Id,
                        Titulo = r.Tarea_Titulo,
                        IdProyecto = r.Tarea_IdProyecto
                    }
                    : null
            }).ToList();

            return comentarios;
        }
    }
}
