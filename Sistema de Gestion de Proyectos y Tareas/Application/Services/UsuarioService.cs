using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        public string CrearNuevoUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();
            
            // Generar contraseña automática
            string contraseñaGenerada = GenerarContraseñaAutomatica(usuario.PrimerNombre, usuario.Apellidos);
            
            // Hashear la contraseña antes de guardarla
            usuario.Contraseña = HashPassword(contraseñaGenerada);

            repo.AddAsync(usuario);
            
            // Retornar la contraseña sin hashear para enviarla por email
            return contraseñaGenerada;
        }

        public void ActualizarUsuario(Usuario usuario)
        {
            var repo = _usuarioFactory.CreateRepository();
            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();
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

            if (!string.IsNullOrEmpty(usuario.Rol)) usuario.Rol = usuario.Rol.Trim();
            if (!string.IsNullOrEmpty(usuario.Contraseña) && usuario.Contraseña.StartsWith("PBKDF2:", StringComparison.Ordinal))
            {
                if (VerifyHashedPassword(usuario.Contraseña, password))
                {
                    return usuario;
                }
                return null;
            }

            if (usuario.Contraseña == password)
            {
                usuario.Contraseña = HashPassword(password);
                repo.UpdateAsync(usuario);
                return usuario;
            }

            return null;
        }

        private string GenerarContraseñaAutomatica(string primerNombre, string apellidos)
        {
            // Limpiar y normalizar los nombres (remover acentos y espacios extras)
            string nombreLimpio = LimpiarTexto(primerNombre);
            string apellidoLimpio = LimpiarTexto(apellidos);

            // Tomar las 3 primeras letras del nombre (o menos si es más corto)
            string partNombre = nombreLimpio.Length >= 3 
                ? nombreLimpio.Substring(0, 3) 
                : nombreLimpio.PadRight(3, 'x');

            // Tomar las 2 primeras letras del apellido (o menos si es más corto)
            string partApellido = apellidoLimpio.Length >= 2 
                ? apellidoLimpio.Substring(0, 2) 
                : apellidoLimpio.PadRight(2, 'x');

            // Crear la contraseña base: Primera letra mayúscula + resto minúsculas
            string contraseñaBase = char.ToUpper(partNombre[0]) + 
                                   partNombre.Substring(1).ToLower() + 
                                   partApellido.ToLower();

            // Verificar si la contraseña ya existe y agregar números si es necesario
            string contraseñaFinal = contraseñaBase;
            int contador = 1;
            
            var repo = _usuarioFactory.CreateRepository();
            var todosUsuarios = repo.GetAllAsync().ToList();

            // Buscar contraseñas similares para encontrar el siguiente número disponible
            while (ContraseñaExiste(contraseñaFinal, todosUsuarios))
            {
                contraseñaFinal = contraseñaBase + contador;
                contador++;
            }

            // Agregar un carácter especial al final para cumplir con los requisitos
            contraseñaFinal += "!";

            return contraseñaFinal;
        }

        private bool ContraseñaExiste(string contraseñaPlana, List<Usuario> usuarios)
        {
            // Verificar si algún usuario tiene esta contraseña
            foreach (var usuario in usuarios)
            {
                // Si la contraseña está hasheada, verificarla
                if (usuario.Contraseña.StartsWith("PBKDF2:", StringComparison.Ordinal))
                {
                    if (VerifyHashedPassword(usuario.Contraseña, contraseñaPlana))
                    {
                        return true;
                    }
                }
                // Si no está hasheada (legacy), comparar directamente
                else if (usuario.Contraseña == contraseñaPlana)
                {
                    return true;
                }
            }
            return false;
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Remover acentos y caracteres especiales
            string normalizado = texto.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            foreach (char c in normalizado)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

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