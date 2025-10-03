using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    public class IndexModel : PageModel
    {
        private readonly IDB<Tarea> _tareaRepository;
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(IDB<Tarea> tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        public async Task OnGetAsync()
        {
            Tareas = await _tareaRepository.GetAllAsync();
        }
    }
}