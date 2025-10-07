using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Usuario> _repositoryFactory;
        [BindProperty]
        public Usuario Usuario { get; set; } = new();

        public CreateModel(MySqlRepositoryFactory<Usuario> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var repo = _repositoryFactory.CreateRepository();
            repo.AddAsync(Usuario);
            return RedirectToPage("./Index");
        }
    }
}