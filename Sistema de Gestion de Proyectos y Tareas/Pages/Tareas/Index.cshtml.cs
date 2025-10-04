using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly IRepositoryFactory _factory;
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(IRepositoryFactory factory) => _factory = factory;

        public async Task OnGetAsync()
        {
            var repo = _factory.CreateTareaRepository();
            Tareas = await repo.GetAllAsync();
        }
    }
}