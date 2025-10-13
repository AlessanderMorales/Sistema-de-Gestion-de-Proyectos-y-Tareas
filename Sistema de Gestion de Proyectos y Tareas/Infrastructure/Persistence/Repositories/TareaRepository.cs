namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
    using System.Collections.Generic;

    public class TareaRepository : IDB<Tarea>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public TareaRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }

        public IEnumerable<Tarea> GetAllAsync() 
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado
                              FROM Tareas -- ¡Usando el nombre de tabla corregido!
                              WHERE estado = 1 
                              ORDER BY id_tarea DESC";

            return _connectionSignleton.ExcuteCommandWithDataReturn<Tarea>(query);
        }

        public void AddAsync(Tarea entity)
        {
            string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad)
                              VALUES (@Titulo, @Descripcion, @Prioridad);";

            _connectionSignleton.ExcuteCommand(query, entity);
        }
        public Tarea GetByIdAsync(int id)
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado
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
                              prioridad = @Prioridad
                              WHERE id_tarea = @Id;";

            _connectionSignleton.ExcuteCommand(query, entity);
        }

        void IDB<Tarea>.DeleteAsync(int id)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand<dynamic>(query, new { Id = id });
        }

        public void DeleteAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                             SET estado = 0
                             WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand(query, entity);
        }
    }
}