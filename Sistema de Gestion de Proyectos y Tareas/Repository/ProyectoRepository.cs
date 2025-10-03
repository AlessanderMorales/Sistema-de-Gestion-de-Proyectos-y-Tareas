namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper; 
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    using System;
    using System.Data;
    
    public class ProyectoRepository : IDB<Proyecto>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ProyectoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Proyecto>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin 
                                 FROM Proyecto WHERE estado = 1 ORDER BY nombre";
            return await connection.QueryAsync<Proyecto>(query);
        }

        public async Task<Proyecto?> GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin, estado, fecha_registro AS FechaRegistro
                                 FROM Proyecto WHERE id_proyecto = @Id AND estado = 1";
            return await connection.QueryFirstOrDefaultAsync<Proyecto>(query, new { Id = id });
        }

        public async Task AddAsync(Proyecto entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin, estado, fecha_registro) 
                                 VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin, @Estado, @FechaRegistro);";
            await connection.ExecuteAsync(query, entity);
        }

        public async Task UpdateAsync(Proyecto entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"UPDATE Proyecto SET nombre = @Nombre, descripcion = @Descripcion, fecha_inicio = @FechaInicio, fecha_fin = @FechaFin, estado = @Estado
                                 WHERE id_proyecto = @Id";
            await connection.ExecuteAsync(query, entity);
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"UPDATE Proyecto SET estado = 0 WHERE id_proyecto = @Id";
            await connection.ExecuteAsync(query, new { Id = id });
        }
    }
}


