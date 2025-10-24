using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Common;

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
            [Required(ErrorMessage = "El email o nombre de usuario es obligatorio.")]
            [Display(Name = "Email o Usuario")]
            public string EmailOrUsername { get; set; }

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
                var usuario = _usuarioService.ValidarUsuario(Input.EmailOrUsername, Input.Password);

                if (usuario != null)
                {
                    string normalizedRole = Roles.Empleado;
                    if (!string.IsNullOrEmpty(usuario.Rol))
                    {
                        var r = usuario.Rol.Trim().ToLowerInvariant();
                        if (r.Contains("super")) normalizedRole = Roles.SuperAdmin;
                        else if (r.Contains("jefe") || r.Contains("jefe de proyecto") || r.Contains("jefedeproyecto")) normalizedRole = Roles.JefeDeProyecto;
                        else normalizedRole = Roles.Empleado;
                    }

                    string nombreCompleto = !string.IsNullOrEmpty(usuario.SegundoApellido)
                        ? $"{usuario.Nombres} {usuario.PrimerApellido} {usuario.SegundoApellido}"
                        : $"{usuario.Nombres} {usuario.PrimerApellido}";

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new Claim(ClaimTypes.Name, usuario.Email),
                        new Claim("Username", usuario.NombreUsuario ?? usuario.Email),
                        new Claim("FullName", nombreCompleto),
                        new Claim(ClaimTypes.Role, normalizedRole)
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, "MyCookieAuth");
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email/usuario o contraseña incorrectos.");
                    return Page();
                }
            }
            return Page();
        }
    }
}