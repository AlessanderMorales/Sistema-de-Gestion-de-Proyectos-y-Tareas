namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository
{
    using Dapper;
    using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
    using System.Data;
    using System.Collections.Generic; // Necesario para IEnumerable

    // Asumo que IDB<T> es una interfaz que define los métodos CRUD básicos
    // (GetAllAsync, GetByIdAsync, AddAsync, UpdateAsync, DeleteAsync).
    public class UsuarioRepository : IDB<Usuario>
    {
        private readonly IDbConnectionSingleton _connectionFactory;

        // Constructor para inyectar la dependencia de la fábrica de conexiones
        public UsuarioRepository(IDbConnectionSingleton connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public IEnumerable<Usuario> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
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
            return connection.QueryFirstOrDefault<Usuario>(
                @"SELECT id_usuario AS Id, primer_nombre AS PrimerNombre, segundo_nombre AS SegundoNombre, 
                         apellidos, contraseña, rol AS Rol, estado AS Estado 
                  FROM Usuario 
                  WHERE id_usuario = @Id AND estado = 1",
                new { Id = id });
        }

        public void AddAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Incluye 'rol' y 'estado' en la inserción.
            // Los valores se toman de las propiedades correspondientes de 'entity'.
            connection.Execute(
                @"INSERT INTO Usuario (primer_nombre, segundo_nombre, apellidos, contraseña, rol, estado) 
                  VALUES (@PrimerNombre, @SegundoNombre, @Apellidos, @Contraseña, @Rol, @Estado);",
                entity);
        }

        public void UpdateAsync(Usuario entity)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Actualiza todas las propiedades modificables, incluyendo 'rol'.
            // El 'estado' no se actualiza aquí, ya que se maneja por DeleteAsync.
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

        // Método principal para la eliminación lógica por ID
        public void DeleteAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            // Esta es la consulta clave: cambia el 'estado' a 0.
            connection.Execute(
                @"UPDATE Usuario SET estado = 0 WHERE id_usuario = @Id;",
                new { Id = id });
        }

        // Sobrecarga para eliminar lógicamente pasando un objeto Usuario
        public void DeleteAsync(Usuario entity)
        {
            // Simplemente llama al método por ID, extrayendo el Id de la entidad.
            // Asegúrate de que 'entity.Id' tenga un valor válido.
            DeleteAsync(entity.Id);
        }
    }
}