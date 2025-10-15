
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        public IndexModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public void OnGet()
        {
            Proyectos = _proyectoService.ObtenerTodosLosProyectos();
        }

        public IActionResult OnPost(int id)
        {
            var proyecto = _proyectoService.ObtenerProyectoPorId(id);
            if (proyecto != null)
            {
                _proyectoService.EliminarProyecto(proyecto);
            }
            _proyectoService.EliminarProyectoPorId(id);
            return RedirectToPage("./Index");
        }
    }
}
