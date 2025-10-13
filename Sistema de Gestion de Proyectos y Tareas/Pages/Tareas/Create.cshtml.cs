using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class CreateModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Tarea> _repositoryFactory;

        [BindProperty]
        public Tarea Tarea { get; set; } = new();

        public CreateModel(MySqlRepositoryFactory<Tarea> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            IDB<Tarea> repo = _repositoryFactory.CreateRepository();
            repo.AddAsync(Tarea);

            return RedirectToPage("./Index");
        }
    }
}