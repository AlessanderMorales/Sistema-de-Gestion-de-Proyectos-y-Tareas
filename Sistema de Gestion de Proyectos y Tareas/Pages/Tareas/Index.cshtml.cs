using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Tarea> _repositoryFactory;
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(MySqlRepositoryFactory<Tarea> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public void OnGet()
        {
            IDB<Tarea> repo = _repositoryFactory.CreateRepository();
            Tareas = repo.GetAllAsync();
        }

        public IActionResult OnPost(int id)
        {
            var repo = _repositoryFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(id);

            if (tarea != null)
            {
                repo.DeleteAsync(tarea);
            }
            return RedirectToPage("./Index");
        }
    }
}