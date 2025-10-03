namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

    public class TareaRepository : IDB<Tarea>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public TareaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Tarea>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT id_tarea AS Id, titulo, descripcion, prioridad 
                         FROM Tareas WHERE estado = 1 ORDER BY prioridad";
            return await connection.QueryAsync<Tarea>(query);
        }

        public async Task AddAsync(Tarea entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"INSERT INTO Tareas (titulo, descripcion, prioridad) 
                         VALUES (@Titulo, @Descripcion, @Prioridad);";
            await connection.ExecuteAsync(query, entity);
        }

        public Task<Tarea?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task UpdateAsync(Tarea entity) => throw new NotImplementedException();
        public Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}
