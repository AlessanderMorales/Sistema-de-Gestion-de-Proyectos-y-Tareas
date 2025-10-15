using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using BCrypt.Net;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly UsuarioService _usuarioService;

        public LoginModel(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Usuario y contraseña son requeridos.");
                return Page();
            }

            var usuario = _usuarioService.ObtenerUsuarioPorUsername(username);
            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return Page();
            }

            // Verify password hash
            var valid = BCrypt.Net.BCrypt.Verify(password, usuario.Contraseña);
            if (!valid)
            {
                ModelState.AddModelError(string.Empty, "Usuario o contraseña inválidos.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                // Do NOT persist cookie by default. Closing browser will end session.
                IsPersistent = false
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return LocalRedirect("/");
        }
    }
}
