using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Repository;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly TareaRepositoryCreator _tareaRepositoryCreator;

        // Lista de tareas para mostrar en la vista
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(TareaRepositoryCreator tareaRepositoryCreator)
        {
            _tareaRepositoryCreator = tareaRepositoryCreator;
        }

        public void OnGet()
        {
            // Cargar todas las tareas (se asume que GetAllAsync retorna IEnumerable<Tarea>)
            IDB<Tarea> repo = _tareaRepositoryCreator.CreateRepository();
            Tareas = repo.GetAllAsync();
        }

        public IActionResult OnPost(int id)
        {
            // Lógica para el borrado lógico (Estado = 0)
            var repo = _tareaRepositoryCreator.CreateRepository();
            var tarea = repo.GetByIdAsync(id); // Obtener la tarea por ID

            if (tarea != null)
            {
                repo.DeleteAsync(tarea);
            }
            return RedirectToPage("./Index");
        }
    }
}