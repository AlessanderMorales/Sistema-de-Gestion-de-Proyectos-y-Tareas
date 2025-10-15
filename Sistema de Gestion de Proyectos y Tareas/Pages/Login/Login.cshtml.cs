

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public LoginModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El email es obligatorio.")]
            [EmailAddress]
            public string Email { get; set; }

            [Required(ErrorMessage = "La contraseña es obligatoria.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // --- LÓGICA DE VALIDACIÓN ---
                // ¡Necesitaremos añadir un método en UsuarioService para esto!
                var usuario = _usuarioService.ValidarUsuario(Input.Email, Input.Password);

                if (usuario != null)
                {
                    // --- CREACIÓN DE LA COOKIE DE AUTENTICACIÓN ---

                    // 1. Crear la lista de "claims". Los claims son datos sobre el usuario.
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Email), // Guardamos el email del usuario
                        new Claim("FullName", $"{usuario.PrimerNombre} {usuario.Apellidos}"), // Guardamos su nombre completo
                        new Claim(ClaimTypes.Role, usuario.Rol) // ¡MUY IMPORTANTE! Guardamos su rol.
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    return Page();
                }
            }
            return Page();
        }
    }
}