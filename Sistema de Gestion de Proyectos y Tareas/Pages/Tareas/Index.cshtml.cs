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
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(TareaRepositoryCreator tareaRepositoryCreator)
        {
            _tareaRepositoryCreator = tareaRepositoryCreator;
        }

        public void OnGet()
        {
            IDB<Tarea> repo = _tareaRepositoryCreator.CreateRepository();
            Tareas = repo.GetAllAsync();
        }

        public IActionResult OnPost(int id)
        {
            var repo = _tareaRepositoryCreator.CreateRepository();
            var tarea = repo.GetByIdAsync(id);

            if (tarea != null)
            {
                repo.DeleteAsync(tarea);
            }
            return RedirectToPage("./Index");
        }
    }
}