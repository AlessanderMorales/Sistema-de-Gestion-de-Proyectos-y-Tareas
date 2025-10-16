using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

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

        public void CrearNuevoUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            // Normalize role before insert
            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();

            // Hash password before storing
            usuario.Contraseña = HashPassword(usuario.Contraseña);

            repo.AddAsync(usuario);
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();

            // If password appears changed (not starting with PBKDF2:), hash it
            if (!string.IsNullOrEmpty(usuario.Contraseña) && !usuario.Contraseña.StartsWith("PBKDF2:", StringComparison.Ordinal))
            {
                usuario.Contraseña = HashPassword(usuario.Contraseña);
            }

            repo.UpdateAsync(usuario);
        }

        public void EliminarUsuario(int id)
        {
            var repo = _usuarioFactory.CreateRepository();
            repo.DeleteAsync(id);
        }

        public Usuario ValidarUsuario(string email, string password)
        {
            var repo = _usuarioFactory.CreateRepository();
            var usuario = repo.GetAllAsync().FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (usuario == null) return null;

            // Trim role
            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();

            // If stored password is hashed (starts with PBKDF2:), verify accordingly
            if (!string.IsNullOrEmpty(usuario.Contraseña) && usuario.Contraseña.StartsWith("PBKDF2:", StringComparison.Ordinal))
            {
                if (VerifyHashedPassword(usuario.Contraseña, password))
                {
                    return usuario;
                }
                return null;
            }

            // Legacy plaintext comparison - if matches, migrate to hashed value
            if (usuario.Contraseña == password)
            {
                usuario.Contraseña = HashPassword(password);
                repo.UpdateAsync(usuario);
                return usuario;
            }

            return null;
        }

        // PBKDF2 helpers
        private string HashPassword(string password)
        {
            // PBKDF2 with HMACSHA256
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            // store as PBKDF2:{iterations}:{saltBase64}:{hashBase64}
            return $"PBKDF2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        private bool VerifyHashedPassword(string storedHash, string providedPassword)
        {
            try
            {
                var parts = storedHash.Split(':');
                if (parts.Length != 4) return false;
                if (parts[0] != "PBKDF2") return false;
                int iterations = int.Parse(parts[1]);
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] hash = Convert.FromBase64String(parts[3]);

                using var pbkdf2 = new Rfc2898DeriveBytes(providedPassword, salt, iterations, HashAlgorithmName.SHA256);
                byte[] testHash = pbkdf2.GetBytes(hash.Length);

                return CryptographicOperations.FixedTimeEquals(testHash, hash);
            }
            catch
            {
                return false;
            }
        }


        public bool EmailYaExiste(string email)
        {
            var repo = _usuarioFactory.CreateRepository();
            var usuarioExistente = repo.GetAllAsync()
                                    .FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            return usuarioExistente != null;
        }
    }
}