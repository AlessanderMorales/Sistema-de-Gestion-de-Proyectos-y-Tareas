using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService;

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;

        public SelectList ProyectosDisponibles { get; set; }
        public EditModel(TareaService tareaService, ProyectoService proyectoService)
        {
            _tareaService = tareaService;
            _proyectoService = proyectoService;
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
            ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre", Tarea.IdProyecto);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var proyectos = _proyectoService.ObtenerTodosLosProyectos();
                ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre", Tarea.IdProyecto);
                return Page();
            }

            _tareaService.ActualizarTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}