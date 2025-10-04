using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _factory;
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

        public IndexModel(IRepositoryFactory factory) => _factory = factory;

        public async Task OnGetAsync()
        {
            var repo = _factory.CreateProyectoRepository();
            Proyectos = await repo.GetAllAsync();
        }
    }
}