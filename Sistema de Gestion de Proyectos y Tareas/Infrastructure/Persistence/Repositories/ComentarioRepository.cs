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
