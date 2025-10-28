using Dapper;
using ServiceCommon.Domain.Port.Repositories;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceUsuario.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ServiceUsuario.Infrastructure.Persistence.Repositories
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
                @"SELECT id_usuario AS Id, nombres AS Nombres, primer_apellido AS PrimerApellido, 
                         segundo_apellido AS SegundoApellido, nombre_usuario AS NombreUsuario,
                         contraseña, email, rol AS Rol, estado AS Estado 
                  FROM Usuario WHERE estado = 1 ORDER BY primer_apellido");
        }

        public Usuario GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QueryFirstOrDefault<Usuario>(
                @"SELECT id_usuario AS Id, nombres AS Nombres, primer_apellido AS PrimerApellido, 
                         segundo_apellido AS SegundoApellido, nombre_usuario AS NombreUsuario,
                         contraseña, email, rol AS Rol, estado AS Estado 
                  FROM Usuario WHERE id_usuario = @Id AND estado = 1;",
                new { Id = id });
        }

        public Usuario GetByEmailOrUsername(string emailOrUsername)
        {
            using var connection = _connectionFactory.CreateConnection();
            return connection.QueryFirstOrDefault<Usuario>(
                @"SELECT id_usuario AS Id, nombres AS Nombres, primer_apellido AS PrimerApellido, 
                         segundo_apellido AS SegundoApellido, nombre_usuario AS NombreUsuario,
                         contraseña, email, rol AS Rol, estado AS Estado 
                  FROM Usuario 
                  WHERE (email = @EmailOrUsername OR nombre_usuario = @EmailOrUsername) 
                  AND estado = 1;",
                new { EmailOrUsername = emailOrUsername });
        }

        public void AddAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"INSERT INTO Usuario (nombres, primer_apellido, segundo_apellido, nombre_usuario, contraseña, email, rol, estado) 
                  VALUES (@Nombres, @PrimerApellido, @SegundoApellido, @NombreUsuario, @Contraseña, @Email, @Rol, @Estado);",
                entity);
        }

        public void UpdateAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario 
                  SET nombres = @Nombres, primer_apellido = @PrimerApellido, segundo_apellido = @SegundoApellido,
                      nombre_usuario = @NombreUsuario, contraseña = @Contraseña, email = @Email, rol = @Rol
                  WHERE id_usuario = @Id;",
                entity);
        }

        public void UpdatePassword(int id, string newPasswordHash)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario SET contraseña = @NewPasswordHash WHERE id_usuario = @Id;",
                new { Id = id, NewPasswordHash = newPasswordHash });
        }

        public void DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"DELETE FROM Usuario WHERE id_usuario = @Id;",
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