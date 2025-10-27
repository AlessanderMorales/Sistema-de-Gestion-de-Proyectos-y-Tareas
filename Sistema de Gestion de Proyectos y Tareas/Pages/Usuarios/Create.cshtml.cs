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
            // Remover la validaci�n de contrase�a del ModelState ya que se generar� autom�ticamente
            ModelState.Remove("Usuario.Contrase�a");
            ModelState.Remove("Usuario.NombreUsuario");
            
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
                // La contrase�a y nombre de usuario se generan autom�ticamente en el servicio
                string contrase�aGenerada = _usuarioService.CrearNuevoUsuario(Usuario);

                // Obtener el usuario reci�n creado para acceder a su nombre de usuario generado
                var usuarioCreado = _usuarioService.ObtenerTodosLosUsuarios()
                    .FirstOrDefault(u => u.Email.Equals(Usuario.Email, StringComparison.OrdinalIgnoreCase));

                if (usuarioCreado != null)
                {
                    // Enviar email con las credenciales
                    string nombreCompleto = !string.IsNullOrEmpty(usuarioCreado.SegundoApellido)
                        ? $"{usuarioCreado.Nombres} {usuarioCreado.PrimerApellido} {usuarioCreado.SegundoApellido}"
                        : $"{usuarioCreado.Nombres} {usuarioCreado.PrimerApellido}";

                    bool emailEnviado = await _emailService.EnviarEmailContrase�a(
                        usuarioCreado.Email, 
                        nombreCompleto,
                        usuarioCreado.NombreUsuario,  // Ahora se env�a el nombre de usuario
                        contrase�aGenerada
                    );

                    if (emailEnviado)
                    {
                        MensajeExito = $"Usuario creado exitosamente. Se ha enviado un correo a {usuarioCreado.Email} con las credenciales de acceso.";
                    }
                    else
                    {
                        MensajeError = $"Usuario creado, pero hubo un error al enviar el correo. Usuario: {usuarioCreado.NombreUsuario} | Contrase�a: {contrase�aGenerada}";
                    }
                }
                else
                {
                    MensajeError = "Usuario creado pero no se pudo recuperar la informaci�n para enviar el email.";
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