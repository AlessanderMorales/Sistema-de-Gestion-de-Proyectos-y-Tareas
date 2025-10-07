using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly UsuarioRepositoryCreator _repositoryCreator;
        public IEnumerable<Usuario> Usuarios { get; private set; } = new List<Usuario>();

        public IndexModel(UsuarioRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        public IActionResult OnGet()
        {
            var repo = _repositoryCreator.CreateRepository();
            Usuarios = repo.GetAllAsync(); // Sin await, método síncrono
            return Page();
        }

        public IActionResult OnPostDelete(int? id)
        {
            if (id.HasValue)
            {
                var repo = _repositoryCreator.CreateRepository();
                repo.DeleteAsync(id.Value); // Usar .Value para convertir int? a int
            }
            return RedirectToPage("Index");
        }
    }
}