
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities; 
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;


namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.proyectos
{
    public class MostrarModel : PageModel
    {
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        private readonly MySqlRepositoryFactory<Proyecto> _repositoryFactory;
        public MostrarModel(MySqlRepositoryFactory<Proyecto> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public IActionResult OnGet()
        {
            Proyectos = _repositoryFactory.CreateRepository().GetAllAsync();
            return Page();
        }
    }
}