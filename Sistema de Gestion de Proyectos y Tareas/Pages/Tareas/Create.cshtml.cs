
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly TareaService _tareaService;

        [BindProperty]
        public Tarea Tarea { get; set; } = new();
        public CreateModel(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _tareaService.CrearNuevaTarea(Tarea);

            return RedirectToPage("./Index");
        }
    }
}