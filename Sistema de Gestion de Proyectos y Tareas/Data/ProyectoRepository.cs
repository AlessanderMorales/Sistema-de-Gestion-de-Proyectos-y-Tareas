// Data/ProyectoRepository.cs
using Dapper; 
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
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
            using (var connection = _connectionFactory.CreateConnection())
            {
                string query = @"SELECT id_proyecto AS Id, nombre, descripcion, fecha_inicio AS FechaInicio, fecha_fin AS FechaFin 
                             FROM Proyecto WHERE estado = 1 ORDER BY nombre";
                return await connection.QueryAsync<Proyecto>(query);
            }
        }

        public async Task AddAsync(Proyecto proyecto)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                string query = @"INSERT INTO Proyecto (nombre, descripcion, fecha_inicio, fecha_fin) 
                             VALUES (@Nombre, @Descripcion, @FechaInicio, @FechaFin);";
                await connection.ExecuteAsync(query, proyecto);
            }
        }



        public Task<Proyecto?> GetByIdAsync(int id) { throw new NotImplementedException(); }
        public Task UpdateAsync(Proyecto proyecto) { throw new NotImplementedException(); }
        public Task DeleteAsync(int id) { throw new NotImplementedException(); }
    }
