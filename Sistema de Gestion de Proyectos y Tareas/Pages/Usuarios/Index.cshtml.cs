using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class IndexModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Usuario> _repositoryFactory;
        public IEnumerable<Usuario> Usuarios { get; private set; } = new List<Usuario>();

        public IndexModel(MySqlRepositoryFactory<Usuario> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public IActionResult OnGet()
        {
            var repo = _repositoryFactory.CreateRepository();
            Usuarios = repo.GetAllAsync();
            return Page();
        }

        public IActionResult OnPostDelete(int? id)
        {
            if (id.HasValue)
            {
                var repo = _repositoryFactory.CreateRepository();
                repo.DeleteAsync(id.Value);
            }
            return RedirectToPage("Index");
        }
    }
}