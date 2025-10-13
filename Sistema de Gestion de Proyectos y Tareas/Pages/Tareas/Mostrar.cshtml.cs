using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class MostrarModel : PageModel
    {
        private readonly MySqlRepositoryFactory<Tarea> _repositoryFactory;

        public MostrarModel(MySqlRepositoryFactory<Tarea> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }
        public Tarea Tarea { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repo = _repositoryFactory.CreateRepository();
            var tarea = repo.GetByIdAsync(id.Value);

            if (tarea == null)
            {
                return NotFound();
            }

            Tarea = tarea;
            return Page();
        }
    }
}