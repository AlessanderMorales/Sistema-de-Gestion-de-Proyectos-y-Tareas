

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService; 

        [BindProperty]
        public Tarea Tarea { get; set; } = new();
        public SelectList ProyectosDisponibles { get; set; }
        public CreateModel(TareaService tareaService, ProyectoService proyectoService)
        {
            _tareaService = tareaService;
            _proyectoService = proyectoService;
        }
        public void OnGet()
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos();
            ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var proyectos = _proyectoService.ObtenerTodosLosProyectos();
                ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");
                return Page();
            }
            _tareaService.CrearNuevaTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}