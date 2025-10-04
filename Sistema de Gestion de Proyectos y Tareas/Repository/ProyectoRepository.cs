namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    

    
    public class ProyectoRepository : IDB<Proyecto>
    {
        private readonly IDbConnectionSingleton _connectionFactory;

        public ProyectoRepository(IDbConnectionSingleton connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Proyecto>> GetAllAsync()
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin 
                             FROM Proyecto WHERE estado = 1 ORDER BY nombre";
                return await connection.QueryAsync<Proyecto>(query);
            }
        }

        public async Task AddAsync(Proyecto entity)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                // El parámetro se llama 'entity' en la interfaz, así que lo usamos aquí.
                string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin) 
                             VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin);";
                await connection.ExecuteAsync(query, entity);
            }
        }

        // He cambiado el parámetro a 'entity' para que coincida con la interfaz IDB
        public Task<Proyecto?> GetByIdAsync(int id) { throw new NotImplementedException(); }
        public Task UpdateAsync(Proyecto entity) { throw new NotImplementedException(); }
        public Task DeleteAsync(int id) { throw new NotImplementedException(); }
    }
}


