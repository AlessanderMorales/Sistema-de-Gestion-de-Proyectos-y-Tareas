using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly IRepositoryFactory _factory;
        [BindProperty]
        public Tarea Tarea { get; set; } = new();

        public CreateModel(IRepositoryFactory factory) => _factory = factory;

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var repo = _factory.CreateTareaRepository();
            await repo.AddAsync(Tarea);
            return RedirectToPage("./Index");
        }
    }
}