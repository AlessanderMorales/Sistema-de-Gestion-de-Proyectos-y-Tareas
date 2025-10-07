using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    // Usa un parámetro de ruta para el ID: Tareas/Mostrar/5
    public class MostrarModel : PageModel
    {
        private readonly TareaRepositoryCreator _repositoryCreator;

        public MostrarModel(TareaRepositoryCreator repositoryCreator)
        {
            _repositoryCreator = repositoryCreator;
        }

        // Propiedad para la tarea individual
        public Tarea Tarea { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repo = _repositoryCreator.CreateRepository();
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