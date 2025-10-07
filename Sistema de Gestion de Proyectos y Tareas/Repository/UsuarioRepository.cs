namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    using System.Data;
    using System.Collections.Generic;

    public class UsuarioRepository : IDB<Usuario>
    {
        private readonly IDbConnectionSingleton _connectionFactory;

        public UsuarioRepository(IDbConnectionSingleton connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Usuario> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            // Solo selecciona usuarios con estado = 1 para mostrar en la página
            return connection.Query<Usuario>(
                @"SELECT id_usuario AS Id, primer_nombre AS PrimerNombre, segundo_nombre AS SegundoNombre, 
                         apellidos, contraseña, rol AS Rol, estado AS Estado 
                  FROM Usuario 
                  WHERE estado = 1 
                  ORDER BY apellidos");
        }

        public Usuario GetByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Solo busca usuarios con estado = 1
            return connection.QueryFirstOrDefault<Usuario>(
                @"SELECT id_usuario AS Id, primer_nombre AS PrimerNombre, segundo_nombre AS SegundoNombre, 
                         apellidos, contraseña, rol AS Rol, estado AS Estado 
                  FROM Usuario 
                  WHERE id_usuario = @Id AND estado = 1;",
                new { Id = id });
        }

        public void AddAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Asegúrate de que los nuevos usuarios se inserten con estado = 1 por defecto
            connection.Execute(
                @"INSERT INTO Usuario (primer_nombre, segundo_nombre, apellidos, contraseña, rol, estado) 
                  VALUES (@PrimerNombre, @SegundoNombre, @Apellidos, @Contraseña, @Rol, @Estado);",
                entity);
        }

        public void UpdateAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario 
                  SET primer_nombre = @PrimerNombre, 
                      segundo_nombre = @SegundoNombre, 
                      apellidos = @Apellidos, 
                      contraseña = @Contraseña, 
                      rol = @Rol
                  WHERE id_usuario = @Id;",
                entity);
        }

        // --- MÉTODO CORREGIDO PARA ELIMINACIÓN LÓGICA (cambiar estado de 1 a 0) ---
        public void DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Execute(
                @"UPDATE Usuario SET estado = 0 WHERE id_usuario = @Id;",
                new { Id = id });
        }

        // Sobrecarga del método DeleteAsync que toma un objeto Usuario.
        // Llama al método por ID para realizar la eliminación lógica.
        public void DeleteAsync(Usuario entity)
        {
            DeleteAsync(entity.Id);
        }
    }
}