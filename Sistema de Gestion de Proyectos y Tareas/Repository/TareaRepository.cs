namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    using System.Collections.Generic; // Para IEnumerable

    public class TareaRepository : IDB<Tarea>
    {
        private readonly MySqlConnectionSingleton _connectionSignleton;

        public TareaRepository(MySqlConnectionSingleton mySqlConnectionSingleton)
        {
            _connectionSignleton = mySqlConnectionSingleton;
        }

        public IEnumerable<Tarea> GetAllAsync() // Aunque dice "Async", la implementación es síncrona aquí
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado
                              FROM Tareas -- ¡Usando el nombre de tabla corregido!
                              WHERE estado = 1 
                              ORDER BY id_tarea DESC";

            return _connectionSignleton.ExcuteCommandWithDataReturn<Tarea>(query);
        }

        public void AddAsync(Tarea entity) // Aunque dice "Async", la implementación es síncrona
        {
            string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad)
                              VALUES (@Titulo, @Descripcion, @Prioridad);";

            _connectionSignleton.ExcuteCommand<Tarea>(query, entity);
        }

        // Implementación corregida para GetByIdAsync (SÍNCRONA)
        public Tarea GetByIdAsync(int id)
        {
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad, estado
                              FROM Tareas
                              WHERE id_tarea = @Id AND estado = 1;"; // Tareas activas

            var parameters = new { Id = id }; // Objeto anónimo para los parámetros

            // ¡Usar el nuevo método QueryFirstOrDefault del singleton!
            return _connectionSignleton.QueryFirstOrDefault<Tarea>(query, parameters);
        }

        public void UpdateAsync(Tarea entity) // Aunque dice "Async", la implementación es síncrona
        {
            string query = @"UPDATE Tareas
                              SET titulo = @Titulo,
                              descripcion = @Descripcion,
                              prioridad = @Prioridad
                              WHERE id_tarea = @Id;";

            _connectionSignleton.ExcuteCommand<Tarea>(query, entity);
        }

        // Implementación explícita para IDB<Tarea>.DeleteAsync(int id)
        void IDB<Tarea>.DeleteAsync(int id)
        {
            string query = @"UPDATE Tareas SET estado = 0 WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand<dynamic>(query, new { Id = id }); // Pasa el ID como objeto anónimo
        }

        public void DeleteAsync(Tarea entity)
        {
            string query = @"UPDATE Tareas
                             SET estado = 0
                             WHERE id_tarea = @Id;";
            _connectionSignleton.ExcuteCommand<Tarea>(query, entity);
        }
    }
}