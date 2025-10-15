using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService; 
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Tarea Tarea { get; set; } = new();
        public SelectList ProyectosDisponibles { get; set; }
        public SelectList UsuariosDisponibles { get; set; }
        public bool CanAssignUsers { get; set; }
        public bool CanCreate { get; set; }

        public CreateModel(TareaService tareaService, ProyectoService proyectoService, UsuarioService usuarioService)
        {
            _tareaService = tareaService;
            _proyectoService = proyectoService;
            _usuarioService = usuarioService;
        }
        public void OnGet()
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos();
            ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            CanAssignUsers = (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "supervisor" || role.ToLowerInvariant() == "jefe de proyecto");
            CanCreate = (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "supervisor" || role.ToLowerInvariant() == "jefe de proyecto");

            if (CanAssignUsers)
            {
                var users = _usuarioService.ObtenerTodosLosUsuarios();
                UsuariosDisponibles = new SelectList(users, "Id", "PrimerNombre");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            var canCreate = (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "supervisor" || role.ToLowerInvariant() == "jefe de proyecto");
            if (!canCreate)
            {
                return Forbid();
            }

            // sanitize spaces
            Tarea.Titulo = InputSanitizer.NormalizeSpaces(Tarea.Titulo) ?? Tarea.Titulo;
            Tarea.Descripcion = InputSanitizer.NormalizeSpaces(Tarea.Descripcion);
            Tarea.Prioridad = InputSanitizer.NormalizeSpaces(Tarea.Prioridad);

            if (!ModelState.IsValid)
            {
                var proyectos = _proyectoService.ObtenerTodosLosProyectos();
                ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");
                return Page();
            }

            // if not admin/supervisor/jefe, assign to current user by default
            if (!(role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "jefe de proyecto" || role.ToLowerInvariant() == "supervisor"))
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var userId))
                {
                    Tarea.AssignedUserId = userId;
                }
            }

            _tareaService.CrearNuevaTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}