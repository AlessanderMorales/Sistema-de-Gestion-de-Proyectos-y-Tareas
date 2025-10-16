using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly TareaService _tareaService;
        public IEnumerable<Tarea> Tareas { get; private set; } = new List<Tarea>();

        public IndexModel(TareaService tareaService)
        {
            _tareaService = tareaService;
        }

        public void OnGet()
        {
            Tareas = _tareaService.ObtenerTodasLasTareas();
        }

        public IActionResult OnPost(int id)
        {

            var tarea = _tareaService.ObtenerTareaPorId(id);
            if (tarea != null)
            {
                _tareaService.EliminarTarea(tarea);
            }
            return RedirectToPage("./Index");
        }
    }
}