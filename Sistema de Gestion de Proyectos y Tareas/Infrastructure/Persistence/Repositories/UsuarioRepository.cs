
using Dapper;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Repositories
{
    public class UsuarioRepository : IDB<Usuario>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Usuario> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.Query<Usuario>(
                @"SELECT id_usuario AS Id, primer_nombre AS PrimerNombre, segundo_nombre AS SegundoNombre, 
                         apellidos, contraseña, email, rol AS Rol, estado AS Estado 
                  FROM Usuario WHERE estado = 1 ORDER BY apellidos");
        }

        public Usuario GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QueryFirstOrDefault<Usuario>(
                @"SELECT id_usuario AS Id, primer_nombre AS PrimerNombre, segundo_nombre AS SegundoNombre, 
                         apellidos, contraseña, email, rol AS Rol, estado AS Estado 
                  FROM Usuario WHERE id_usuario = @Id AND estado = 1;",
                new { Id = id });
        }

        public void AddAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"INSERT INTO Usuario (primer_nombre, segundo_nombre, apellidos, contraseña, email, rol, estado) 
                  VALUES (@PrimerNombre, @SegundoNombre, @Apellidos, @Contraseña, @Email, @Rol, @Estado);",
                entity);
        }

        public void UpdateAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario 
                  SET primer_nombre = @PrimerNombre, segundo_nombre = @SegundoNombre, apellidos = @Apellidos, 
                      contraseña = @Contraseña, email = @Email, rol = @Rol
                  WHERE id_usuario = @Id;",
                entity);
        }

        public void DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario SET estado = 0 WHERE id_usuario = @Id;",
                new { Id = id });
        }

        public void DeleteAsync(Usuario entity)
        {
            DeleteAsync(entity.Id);
        }

        public void DeactivateByProjectId(int idProyecto)
        {
        }
    }
}