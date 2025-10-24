using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

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
                // La contraseña se genera automáticamente en el servicio
                string contraseñaGenerada = _usuarioService.CrearNuevoUsuario(Usuario);

                // Enviar email con las credenciales
                string nombreCompleto = $"{Usuario.PrimerNombre} {Usuario.Apellidos}";
                bool emailEnviado = await _emailService.EnviarEmailContraseña(
                    Usuario.Email, 
                    nombreCompleto, 
                    contraseñaGenerada
                );

                if (emailEnviado)
                {
                    MensajeExito = $"Usuario creado exitosamente. Se ha enviado un correo a {Usuario.Email} con las credenciales de acceso.";
                }
                else
                {
                    MensajeError = $"Usuario creado, pero hubo un error al enviar el correo. Contraseña generada: {contraseñaGenerada}";
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