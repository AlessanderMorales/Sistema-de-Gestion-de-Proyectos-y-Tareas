using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.proyectos
{
    public class MostrarModel : PageModel
    {
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        private readonly ProyectoryRepositoryCreator _proyectoryRepositoryCreator;
        public MostrarModel(ProyectoryRepositoryCreator proyectoryRepositoryCreator)
        {
            _proyectoryRepositoryCreator = proyectoryRepositoryCreator;
        }

        public IActionResult OnGet()
        {
            Proyectos = _proyectoryRepositoryCreator.CreateRepository().GetAllAsync();
            return NotFound();
        }
    }
}
