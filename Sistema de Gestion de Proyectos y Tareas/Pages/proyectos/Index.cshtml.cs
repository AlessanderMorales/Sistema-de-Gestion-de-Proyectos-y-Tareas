using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    public class IndexModel : PageModel
    {
        private readonly IDB<Proyecto> _proyectoRepository;
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();

        public IndexModel(IDB<Proyecto> proyectoRepository)
        {
            _proyectoRepository = proyectoRepository;
        }

        public async Task OnGetAsync()
        {
            Proyectos = await _proyectoRepository.GetAllAsync();
        }
    }
}