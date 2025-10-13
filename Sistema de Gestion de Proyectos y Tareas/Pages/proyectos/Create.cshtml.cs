using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Ports.Repositories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Proyecto> _repositoryFactory;
        [BindProperty]
        public Proyecto Proyecto { get; set; } = new();

        public CreateModel(MySqlRepositoryFactory<Proyecto> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            IDB<Proyecto> repo = _repositoryFactory.CreateRepository();
            repo.AddAsync(Proyecto);
            return RedirectToPage("./Index");
        }
    }
}