using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        private readonly ProyectoryRepositoryCreator _proyectoryRepositoryCreator;
        public IndexModel(ProyectoryRepositoryCreator proyectoryRepositoryCreator  ) {
            _proyectoryRepositoryCreator = proyectoryRepositoryCreator;
        }

        public void OnGet()
        {
            Proyectos = _proyectoryRepositoryCreator.CreateRepository().GetAllAsync();
        }

        public IActionResult OnPost(int id)
        {
            var repo = _proyectoryRepositoryCreator.CreateRepository();
            var proyecto = repo.GetAllAsync().FirstOrDefault(p => p.Id == id);
            if (proyecto != null)
            {
                repo.DeleteAsync(proyecto);
            }
            return RedirectToPage("./Index");
        }
    }
}