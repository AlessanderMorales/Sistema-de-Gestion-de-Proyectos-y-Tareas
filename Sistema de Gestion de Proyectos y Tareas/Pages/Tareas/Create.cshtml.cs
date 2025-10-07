using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly TareaRepositoryCreator _repositoryCreator;

        [BindProperty]
        public Tarea Tarea { get; set; } = new();

        public CreateModel(TareaRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            IDB<Tarea> repo = _repositoryCreator.CreateRepository();
            repo.AddAsync(Tarea);

            return RedirectToPage("./Index");
        }
    }
}