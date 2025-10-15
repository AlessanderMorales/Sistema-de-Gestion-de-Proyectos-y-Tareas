using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Helpers;
using System.Security.Cryptography;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services
{
    public class UsuarioService
    {
        private readonly MySqlRepositoryFactory<Usuario> _usuarioFactory;

        public UsuarioService(MySqlRepositoryFactory<Usuario> usuarioFactory)
        {
            _usuarioFactory = usuarioFactory;
        }


        public IEnumerable<Usuario> ObtenerTodosLosUsuarios()
        {
            var repo = _usuarioFactory.CreateRepository();
            return repo.GetAllAsync();
        }

        public Usuario ObtenerUsuarioPorId(int id)
        {
            var repo = _usuarioFactory.CreateRepository();
            return repo.GetByIdAsync(id);
        }

        public Usuario ObtenerUsuarioPorUsername(string username)
        {
            var repo = _usuarioFactory.CreateRepository() as dynamic;
            return repo.GetByUsername(username);
        }

        // Returns the generated plain password if one was created, otherwise null
        public string? CrearNuevoUsuario(Usuario usuario)
        {
            // Normalize inputs
            usuario.PrimerNombre = InputSanitizer.NormalizeSpaces(usuario.PrimerNombre) ?? usuario.PrimerNombre;
            usuario.SegundoNombre = InputSanitizer.NormalizeSpaces(usuario.SegundoNombre);
            usuario.Apellidos = InputSanitizer.NormalizeSpaces(usuario.Apellidos);

            var repo = _usuarioFactory.CreateRepository() as dynamic;
            // check username unique
            var existing = repo.GetByUsername(usuario.Username);
            if (existing != null)
            {
                throw new InvalidOperationException("El nombre de usuario ya existe.");
            }

            string? generatedPassword = null;

            // If password not provided, generate a secure default password that satisfies validation
            if (string.IsNullOrWhiteSpace(usuario.Contraseña))
            {
                generatedPassword = GenerateSecurePassword(10);
                usuario.Contraseña = generatedPassword;
            }

            // Hash password before storing
            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);

            repo.AddAsync(usuario);

            return generatedPassword;
        }

        private static string GenerateSecurePassword(int length = 10)
        {
            const string lowers = "abcdefghijklmnopqrstuvwxyz";
            const string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";
            const string specials = "!@#$%&*()_-+=<>?";
            string all = lowers + uppers + digits + specials;

            var rand = RandomNumberGenerator.Create();
            char[] password = new char[length];

            // Ensure at least one of each required type
            password[0] = lowers[GetInt(rand, lowers.Length)];
            password[1] = uppers[GetInt(rand, uppers.Length)];
            password[2] = digits[GetInt(rand, digits.Length)];
            password[3] = specials[GetInt(rand, specials.Length)];

            for (int i = 4; i < length; i++)
            {
                password[i] = all[GetInt(rand, all.Length)];
            }

            // Shuffle
            return new string(password.OrderBy(x => GetInt(rand, 100000)).ToArray());
        }

        private static int GetInt(RandomNumberGenerator rng, int maxExclusive)
        {
            var bytes = new byte[4];
            rng.GetBytes(bytes);
            int value = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return value % maxExclusive;
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            usuario.PrimerNombre = InputSanitizer.NormalizeSpaces(usuario.PrimerNombre) ?? usuario.PrimerNombre;
            usuario.SegundoNombre = InputSanitizer.NormalizeSpaces(usuario.SegundoNombre);
            usuario.Apellidos = InputSanitizer.NormalizeSpaces(usuario.Apellidos);

            // Hash password if it's not already a bcrypt hash (basic heuristic)
            if (!string.IsNullOrEmpty(usuario.Contraseña) && !usuario.Contraseña.StartsWith("$2"))
            {
                usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);
            }

            var repo = _usuarioFactory.CreateRepository();
            // Do not change username here; to change username you must use a specific method
            repo.UpdateAsync(usuario);
        }

        public void EliminarUsuario(int id)
        {
            var repo = _usuarioFactory.CreateRepository();
            repo.DeleteAsync(id);
        }
    }
}