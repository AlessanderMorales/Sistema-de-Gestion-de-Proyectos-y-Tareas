using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Common;

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
                var usuario = _usuarioService.ValidarUsuario(Input.Email, Input.Password);

                if (usuario != null)
                {
                    // Normalize role to the constants in Common/Roles
                    string normalizedRole = Roles.Empleado;
                    if (!string.IsNullOrEmpty(usuario.Rol))
                    {
                        var r = usuario.Rol.Trim().ToLowerInvariant();
                        if (r.Contains("super")) normalizedRole = Roles.SuperAdmin;
                        else if (r.Contains("jefe") || r.Contains("jefe de proyecto") || r.Contains("jefedeproyecto")) normalizedRole = Roles.JefeDeProyecto;
                        else normalizedRole = Roles.Empleado;
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), // user id
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("FullName", $"{usuario.PrimerNombre} {usuario.Apellidos}"),
                        new Claim(ClaimTypes.Role, normalizedRole)
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