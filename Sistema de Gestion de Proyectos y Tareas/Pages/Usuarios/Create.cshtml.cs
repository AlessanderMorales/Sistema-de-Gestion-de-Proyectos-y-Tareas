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
            // Remover la validaci�n de contrase�a del ModelState ya que se generar� autom�ticamente
            ModelState.Remove("Usuario.Contrase�a");
            
            if (_usuarioService.EmailYaExiste(Usuario.Email))
            {
                ModelState.AddModelError("Usuario.Email", "Este correo electr�nico ya est� registrado.");
            }
            
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // La contrase�a se genera autom�ticamente en el servicio
                string contrase�aGenerada = _usuarioService.CrearNuevoUsuario(Usuario);

                // Enviar email con las credenciales
                string nombreCompleto = $"{Usuario.PrimerNombre} {Usuario.Apellidos}";
                bool emailEnviado = await _emailService.EnviarEmailContrase�a(
                    Usuario.Email, 
                    nombreCompleto, 
                    contrase�aGenerada
                );

                if (emailEnviado)
                {
                    MensajeExito = $"Usuario creado exitosamente. Se ha enviado un correo a {Usuario.Email} con las credenciales de acceso.";
                }
                else
                {
                    MensajeError = $"Usuario creado, pero hubo un error al enviar el correo. Contrase�a generada: {contrase�aGenerada}";
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