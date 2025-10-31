using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceCommon.Application.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    [Authorize(Policy = "SoloAdmin")]
    public class CreateModel : PageModel
    {
        private readonly UsuarioService _usuarioService;
        private readonly EmailService _emailService;

        [BindProperty]
        public Usuario Usuario { get; set; } = new Usuario { Rol = "Empleado" };

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public CreateModel(UsuarioService usuarioService, EmailService emailService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
        }

        public void OnGet()
        {
            Usuario = new Usuario { Rol = "Empleado" };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove("Usuario.Contraseña");
            ModelState.Remove("Usuario.NombreUsuario");

            var validationContext = new ValidationContext(Usuario, serviceProvider: null, items: null);
            var validationResults = Usuario.Validate(validationContext).ToList();

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    ModelState.AddModelError($"Usuario.{memberName}", validationResult.ErrorMessage ?? "Error de validación");
                }
            }

            if (_usuarioService.EmailYaExiste(Usuario.Email))
            {
                ModelState.AddModelError("Usuario.Email", "Este correo electrónico ya está registrado.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // ? Aplicar trim automático a campos de texto
                if (!string.IsNullOrEmpty(Usuario.Nombres))
                {
                    Usuario.Nombres = TrimExtraSpaces(Usuario.Nombres);
                }

                if (!string.IsNullOrEmpty(Usuario.PrimerApellido))
                {
                    Usuario.PrimerApellido = TrimExtraSpaces(Usuario.PrimerApellido);
                }

                if (!string.IsNullOrEmpty(Usuario.SegundoApellido))
                {
                    Usuario.SegundoApellido = TrimExtraSpaces(Usuario.SegundoApellido);
                }

                if (!string.IsNullOrEmpty(Usuario.Email))
                {
                    Usuario.Email = Usuario.Email.Trim();
                }

                string nombreCompleto = !string.IsNullOrEmpty(Usuario.SegundoApellido)
                    ? $"{Usuario.Nombres} {Usuario.PrimerApellido} {Usuario.SegundoApellido}"
                    : $"{Usuario.Nombres} {Usuario.PrimerApellido}";

                string contrasenaGenerada = null;

                try
                {
                    contrasenaGenerada = _usuarioService.CrearNuevoUsuario(Usuario);

                    var usuarioCreado = _usuarioService.ObtenerTodosLosUsuarios()
                        .FirstOrDefault(u => u.Email.Equals(Usuario.Email, StringComparison.OrdinalIgnoreCase));

                    if (usuarioCreado == null)
                    {
                        MensajeError = "Error al crear el usuario. Intente nuevamente.";
                        return RedirectToPage("./Index");
                    }

                    bool emailEnviado = await _emailService.EnviarEmailContraseña(
                        usuarioCreado.Email,
                        nombreCompleto,
                        usuarioCreado.NombreUsuario,
                        contrasenaGenerada
                    );

                    if (!emailEnviado)
                    {
                        _usuarioService.EliminarUsuario(usuarioCreado.Id);

                        MensajeError = "ERROR: No se envió el correo con las credenciales. El usuario NO fue creado. Revisa la configuración del correo e intenta nuevamente.";
                        return RedirectToPage("./Index");
                    }

                    MensajeExito = $"Usuario creado exitosamente. Se envió un correo a {usuarioCreado.Email} con las credenciales de acceso.";
                }
                catch (Exception ex)
                {
                    MensajeError = $"Error al crear el usuario: {ex.Message}";
                    return RedirectToPage("./Index");
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error inesperado: {ex.Message}";
                return RedirectToPage("./Index");
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// Elimina espacios al inicio, al final y múltiples espacios consecutivos en el medio
        /// </summary>
        private string TrimExtraSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Eliminar espacios al inicio y al final
            input = input.Trim();

            // Reemplazar múltiples espacios consecutivos por uno solo
            return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
        }
    }
}