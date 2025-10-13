
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class EditModel : PageModel
    {
        private readonly TareaService _tareaService;

        [BindProperty]
        public Tarea Tarea { get; set; } = default!;
        public EditModel(TareaService tareaService)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _tareaService.ActualizarTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}