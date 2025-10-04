namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    

    public class TareaRepository : IDB<Tarea>
    {
        private readonly IDbConnectionSingleton _connectionFactory;
        public TareaRepository(IDbConnectionSingleton connectionFactory) => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<Tarea>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Tarea>(
                @"SELECT id_tarea AS Id, titulo, descripcion, prioridad 
              FROM Tareas WHERE estado = 1 ORDER BY prioridad");
        }

        public async Task AddAsync(Tarea entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            await connection.ExecuteAsync(
                @"INSERT INTO Tareas (titulo, descripcion, prioridad) 
              VALUES (@Titulo, @Descripcion, @Prioridad);", entity);
        }

        public Task<Tarea?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task UpdateAsync(Tarea entity) => throw new NotImplementedException();
        public Task DeleteAsync(int id) => throw new NotImplementedException();

        IEnumerable<Tarea> IDB<Tarea>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Tarea IDB<Tarea>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        void IDB<Tarea>.AddAsync(Tarea entity)
        {
            throw new NotImplementedException();
        }

        void IDB<Tarea>.UpdateAsync(Tarea entity)
        {
            throw new NotImplementedException();
        }

        void IDB<Tarea>.DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(Tarea entity)
        {
            throw new NotImplementedException();
        }
    }
}
