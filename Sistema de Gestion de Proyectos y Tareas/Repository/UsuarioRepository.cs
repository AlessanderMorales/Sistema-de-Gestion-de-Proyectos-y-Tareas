namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

    public class UsuarioRepository : IDB<Usuario>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"SELECT id_usuario AS Id, primer_nombre as PrimerNombre, segundo_nombre as SegundoNombre, apellidos, rol 
                         FROM Usuario WHERE estado = 1 ORDER BY apellidos";
            return await connection.QueryAsync<Usuario>(query);
        }

        public async Task AddAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            string query = @"INSERT INTO Usuario (primer_nombre, segundo_nombre, apellidos, contraseña, rol) 
                         VALUES (@PrimerNombre, @SegundoNombre, @Apellidos, @Contraseña, @Rol);";
            await connection.ExecuteAsync(query, entity);
        }

        public Task<Usuario?> GetByIdAsync(int id) => throw new NotImplementedException();
        public Task UpdateAsync(Usuario entity) => throw new NotImplementedException();
        public Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}
