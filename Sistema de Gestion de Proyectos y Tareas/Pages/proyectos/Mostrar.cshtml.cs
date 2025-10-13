
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.proyectos
{
    public class MostrarModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        public IEnumerable<Proyecto> Proyectos { get; private set; } = new List<Proyecto>();
        public MostrarModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }
        public IActionResult OnGet()
        {
            Proyectos = _proyectoService.ObtenerTodosLosProyectos();
            return Page();
        }
    }
}