using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly IDB<Tarea> _tareaRepository;
        [BindProperty]
        public Tarea Tarea { get; set; } = new();

        public CreateModel(IDB<Tarea> tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            await _tareaRepository.AddAsync(Tarea);
            return RedirectToPage("./Index");
        }
    }
}