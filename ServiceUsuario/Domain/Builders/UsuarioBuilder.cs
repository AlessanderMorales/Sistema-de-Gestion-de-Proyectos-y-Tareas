using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ServiceUsuario.Domain.Entities;

namespace ServiceUsuario.Domain.Builders
{
    /// <summary>
    /// Builder Pattern para construcción fluida de objetos Usuario
    /// Permite construir usuarios paso a paso con validaciones y configuraciones personalizadas
    /// </summary>
  public class UsuarioBuilder
    {
    private readonly Usuario _usuario;
   private string _contraseñaPlana;
private bool _generarContraseñaAutomatica;
        private bool _hashearContraseña = true;

        public UsuarioBuilder()
        {
  _usuario = new Usuario
            {
  Estado = 1 // Por defecto activo
   };
    }

        /// <summary>
    /// Establece los nombres del usuario
        /// </summary>
      public UsuarioBuilder ConNombres(string nombres)
  {
      if (string.IsNullOrWhiteSpace(nombres))
                throw new ArgumentException("Los nombres no pueden estar vacíos", nameof(nombres));
            
       _usuario.Nombres = nombres.Trim();
   return this;
 }

        /// <summary>
        /// Establece los apellidos del usuario
     /// </summary>
    public UsuarioBuilder ConApellidos(string primerApellido, string? segundoApellido = null)
        {
     if (string.IsNullOrWhiteSpace(primerApellido))
  throw new ArgumentException("El primer apellido no puede estar vacío", nameof(primerApellido));
            
   _usuario.PrimerApellido = primerApellido.Trim();
            _usuario.SegundoApellido = segundoApellido?.Trim();
      return this;
  }

        /// <summary>
        /// Establece el email y genera automáticamente el nombre de usuario
  /// </summary>
        public UsuarioBuilder ConEmail(string email)
   {
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("El email no puede estar vacío", nameof(email));
            
     _usuario.Email = email.Trim();
    _usuario.NombreUsuario = GenerarNombreUsuario(email);
        return this;
        }

        /// <summary>
        /// Establece un nombre de usuario personalizado (sobrescribe el generado automáticamente)
        /// </summary>
        public UsuarioBuilder ConNombreUsuario(string nombreUsuario)
        {
  if (string.IsNullOrWhiteSpace(nombreUsuario))
             throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(nombreUsuario));
            
            _usuario.NombreUsuario = nombreUsuario.Trim();
       return this;
    }

     /// <summary>
        /// Establece el rol del usuario
/// </summary>
    public UsuarioBuilder ConRol(string rol)
        {
   if (string.IsNullOrWhiteSpace(rol))
                throw new ArgumentException("El rol no puede estar vacío", nameof(rol));
 
            _usuario.Rol = rol.Trim();
            return this;
   }

 /// <summary>
        /// Marca para generar una contraseña automática basada en el nombre y apellido
        /// </summary>
    public UsuarioBuilder ConContraseñaAutomatica()
        {
   _generarContraseñaAutomatica = true;
     return this;
        }

        /// <summary>
    /// Establece una contraseña específica
        /// </summary>
        public UsuarioBuilder ConContraseña(string contraseña)
  {
    if (string.IsNullOrWhiteSpace(contraseña))
      throw new ArgumentException("La contraseña no puede estar vacía", nameof(contraseña));
    
            _contraseñaPlana = contraseña;
            _generarContraseñaAutomatica = false;
            return this;
        }

 /// <summary>
     /// Establece una contraseña ya hasheada (útil para migraciones o importaciones)
     /// </summary>
        public UsuarioBuilder ConContraseñaHasheada(string contraseñaHash)
        {
       if (string.IsNullOrWhiteSpace(contraseñaHash))
       throw new ArgumentException("El hash de contraseña no puede estar vacío", nameof(contraseñaHash));
 
    _usuario.Contraseña = contraseñaHash;
            _hashearContraseña = false;
            _generarContraseñaAutomatica = false;
        return this;
        }

   /// <summary>
        /// Establece el estado del usuario (1 = Activo, 0 = Inactivo)
        /// </summary>
   public UsuarioBuilder ConEstado(int estado)
        {
            if (estado != 0 && estado != 1)
   throw new ArgumentException("El estado debe ser 0 (inactivo) o 1 (activo)", nameof(estado));
        
            _usuario.Estado = estado;
    return this;
        }

        /// <summary>
        /// Marca el usuario como inactivo
        /// </summary>
        public UsuarioBuilder Inactivo()
        {
  _usuario.Estado = 0;
     return this;
    }

        /// <summary>
        /// Construye y retorna el objeto Usuario con su contraseña plana
        /// </summary>
        /// <returns>Tupla con el Usuario construido y la contraseña en texto plano</returns>
     public (Usuario usuario, string contraseñaPlana) Construir()
        {
    // Validaciones finales
            ValidarConstruccion();

            // Generar contraseña si es necesario
            if (_generarContraseñaAutomatica)
   {
        _contraseñaPlana = GenerarContraseñaAutomatica(_usuario.Nombres, _usuario.PrimerApellido);
        }

         // Hashear contraseña si es necesario
            if (_hashearContraseña && !string.IsNullOrEmpty(_contraseñaPlana))
      {
        _usuario.Contraseña = HashPassword(_contraseñaPlana);
         }

            return (_usuario, _contraseñaPlana ?? string.Empty);
        }

  /// <summary>
   /// Construye y retorna solo el objeto Usuario (sin contraseña plana)
  /// </summary>
        public Usuario ConstruirSolo()
        {
      var (usuario, _) = Construir();
 return usuario;
        }

     #region Métodos Privados de Validación y Generación

        private void ValidarConstruccion()
        {
      if (string.IsNullOrWhiteSpace(_usuario.Nombres))
    throw new InvalidOperationException("El usuario debe tener nombres configurados. Use ConNombres().");
   
   if (string.IsNullOrWhiteSpace(_usuario.PrimerApellido))
   throw new InvalidOperationException("El usuario debe tener un primer apellido configurado. Use ConApellidos().");
         
  if (string.IsNullOrWhiteSpace(_usuario.Email))
    throw new InvalidOperationException("El usuario debe tener un email configurado. Use ConEmail().");
      
          if (string.IsNullOrWhiteSpace(_usuario.Rol))
      throw new InvalidOperationException("El usuario debe tener un rol asignado. Use ConRol().");

    if (string.IsNullOrWhiteSpace(_usuario.NombreUsuario))
   throw new InvalidOperationException("El usuario debe tener un nombre de usuario. Use ConEmail() o ConNombreUsuario().");

      // Validar que se haya configurado alguna contraseña
            if (_hashearContraseña && string.IsNullOrEmpty(_contraseñaPlana) && !_generarContraseñaAutomatica)
      throw new InvalidOperationException("Debe configurar una contraseña. Use ConContraseña() o ConContraseñaAutomatica().");
        }

        private string GenerarNombreUsuario(string email)
   {
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;

       var parteLocal = email.Split('@')[0];
            var nombreUsuario = Regex.Replace(parteLocal, @"[^a-zA-Z0-9._-]", "").ToLowerInvariant();

            return nombreUsuario;
        }

        private string GenerarContraseñaAutomatica(string nombres, string primerApellido)
        {
       string nombreLimpio = LimpiarTexto(nombres);
            string apellidoLimpio = LimpiarTexto(primerApellido);

     var primerNombre = nombreLimpio.Split(' ').FirstOrDefault() ?? nombreLimpio;
            string partNombre = primerNombre.Length >= 3 
            ? primerNombre.Substring(0, 3) 
    : primerNombre.PadRight(3, 'x');

            string partApellido = apellidoLimpio.Length >= 2 
 ? apellidoLimpio.Substring(0, 2) 
   : apellidoLimpio.PadRight(2, 'x');

    string contraseñaBase = char.ToUpper(partNombre[0]) + 
           partNombre.Substring(1).ToLower() + 
                partApellido.ToLower();

            return contraseñaBase + "!";
      }

        private string LimpiarTexto(string texto)
        {
  if (string.IsNullOrWhiteSpace(texto))
             return string.Empty;

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

   #endregion
    }
}
