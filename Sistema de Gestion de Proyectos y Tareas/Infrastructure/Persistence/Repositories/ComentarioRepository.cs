using Dapper;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories
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
            string query = @"SELECT id_comentario AS Id, contenido AS Contenido, fecha AS Fecha, 
                                    estado AS Estado, id_tarea AS IdTarea, id_usuario AS IdUsuario
                             FROM Comentario WHERE estado = 1;";
            return _connectionSingleton.ExcuteCommandWithDataReturn<Comentario>(query);
        }

        public Comentario GetByIdAsync(int id)
        {
            string query = @"SELECT id_comentario AS Id, contenido AS Contenido, fecha AS Fecha, 
                                    estado AS Estado, id_tarea AS IdTarea, id_usuario AS IdUsuario
                             FROM Comentario WHERE id_comentario = @Id;";
            var parameters = new { Id = id };
            return _connectionSingleton.QueryFirstOrDefault<Comentario>(query, parameters);
        }

        public void AddAsync(Comentario entity)
        {
            string query = @"INSERT INTO Comentario (contenido, id_tarea, id_usuario, estado)
                             VALUES (@Contenido, @IdTarea, @IdUsuario, @Estado);";
            _connectionSingleton.ExcuteCommand(query, entity);
        }

        public void UpdateAsync(Comentario entity)
        {
            string query = @"UPDATE Comentario 
                             SET contenido = @Contenido, estado = @Estado
                             WHERE id_comentario = @Id;";
            _connectionSingleton.ExcuteCommand(query, entity);
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
    }
}
