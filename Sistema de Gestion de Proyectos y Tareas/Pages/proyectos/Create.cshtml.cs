using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class CreateModel : PageModel
    {
        private readonly ProyectoryRepositoryCreator _repositoryCreator;
        [BindProperty]
        public Proyecto Proyecto { get; set; } = new();

        public CreateModel(ProyectoryRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            IDB<Proyecto> repo = _repositoryCreator.CreateRepository();
            repo.AddAsync(Proyecto);
            return RedirectToPage("./Index");
        }
    }
}