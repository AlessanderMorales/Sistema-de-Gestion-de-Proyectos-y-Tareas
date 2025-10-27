
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class MostrarModel : PageModel
    {
        private readonly TareaService _tareaService;

        public Tarea Tarea { get; set; } = default!;
        public MostrarModel(TareaService tareaService)
        {
            _tareaService = tareaService;
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
            return Page();
        }
    }
}