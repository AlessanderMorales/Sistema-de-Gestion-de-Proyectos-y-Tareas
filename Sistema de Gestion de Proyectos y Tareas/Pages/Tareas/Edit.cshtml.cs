using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService;
        private readonly UsuarioService _usuarioService;

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;

        public SelectList ProyectosDisponibles { get; set; }
        public SelectList UsuariosDisponibles { get; set; }
        public bool CanAssignUsers { get; set; }

        public EditModel(TareaService tareaService, ProyectoService proyectoService, UsuarioService usuarioService)
        {
            _tareaService = tareaService;
            _proyectoService = proyectoService;
            _usuarioService = usuarioService;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = _tareaService.ObtenerTareaPorId(id.Value);

            if (tarea == null)
            {
                return NotFound();
            }
            Tarea = tarea;
            var proyectos = _proyectoService.ObtenerTodosLosProyectos();
            ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre", Tarea.id_proyecto);

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            CanAssignUsers = (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "supervisor" || role.ToLowerInvariant() == "jefe de proyecto");
            if (CanAssignUsers)
            {
                var users = _usuarioService.ObtenerTodosLosUsuarios();
                UsuariosDisponibles = new SelectList(users, "Id", "PrimerNombre", Tarea.AssignedUserId);
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var proyectos = _proyectoService.ObtenerTodosLosProyectos();
                ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre", Tarea.id_proyecto);
                return Page();
            }

            var role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            var canAssign = (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "supervisor" || role.ToLowerInvariant() == "jefe de proyecto");
            if (!canAssign)
            {
                // preserve existing assigned user if current user cannot change assignment
                var existing = _tareaService.ObtenerTareaPorId(Tarea.Id);
                if (existing != null)
                {
                    Tarea.AssignedUserId = existing.AssignedUserId;
                }
            }

            _tareaService.ActualizarTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}