using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly UsuarioRepositoryCreator _repositoryCreator;
        [BindProperty]
        public Usuario Usuario { get; set; } = new();

        public CreateModel(UsuarioRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var repo = _repositoryCreator.CreateRepository();
            repo.AddAsync(Usuario);
            return RedirectToPage("./Index");
        }
    }
}