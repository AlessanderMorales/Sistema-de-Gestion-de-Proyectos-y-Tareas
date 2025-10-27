using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceCommon.Application.Services;
using System;
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
        public Usuario Usuario { get; set; } = new();
        
        [TempData]
        public string? MensajeExito { get; set; }
        
        [TempData]
        public string? MensajeError { get; set; }

        public CreateModel(UsuarioService usuarioService, EmailService emailService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remover la validación de contraseña del ModelState ya que se generará automáticamente
            ModelState.Remove("Usuario.Contraseña");
            ModelState.Remove("Usuario.NombreUsuario");
            
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
                // La contraseña y nombre de usuario se generan automáticamente en el servicio
                string contraseñaGenerada = _usuarioService.CrearNuevoUsuario(Usuario);

                // Obtener el usuario recién creado para acceder a su nombre de usuario generado
                var usuarioCreado = _usuarioService.ObtenerTodosLosUsuarios()
                    .FirstOrDefault(u => u.Email.Equals(Usuario.Email, StringComparison.OrdinalIgnoreCase));

                if (usuarioCreado != null)
                {
                    // Enviar email con las credenciales
                    string nombreCompleto = !string.IsNullOrEmpty(usuarioCreado.SegundoApellido)
                        ? $"{usuarioCreado.Nombres} {usuarioCreado.PrimerApellido} {usuarioCreado.SegundoApellido}"
                        : $"{usuarioCreado.Nombres} {usuarioCreado.PrimerApellido}";

                    bool emailEnviado = await _emailService.EnviarEmailContraseña(
                        usuarioCreado.Email, 
                        nombreCompleto,
                        usuarioCreado.NombreUsuario,  // Ahora se envía el nombre de usuario
                        contraseñaGenerada
                    );

                    if (emailEnviado)
                    {
                        MensajeExito = $"Usuario creado exitosamente. Se ha enviado un correo a {usuarioCreado.Email} con las credenciales de acceso.";
                    }
                    else
                    {
                        MensajeError = $"Usuario creado, pero hubo un error al enviar el correo. Usuario: {usuarioCreado.NombreUsuario} | Contraseña: {contraseñaGenerada}";
                    }
                }
                else
                {
                    MensajeError = "Usuario creado pero no se pudo recuperar la información para enviar el email.";
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al crear el usuario: {ex.Message}";
            }

            return RedirectToPage("./Index");
        }
    }
}