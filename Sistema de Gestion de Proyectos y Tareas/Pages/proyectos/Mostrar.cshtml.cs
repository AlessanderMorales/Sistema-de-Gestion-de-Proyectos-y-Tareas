
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services; 

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.proyectos
{
    public class MostrarModel : PageModel
    {
        private readonly ProyectoService _proyectoService; 
        public Proyecto Proyecto { get; private set; }

        public MostrarModel(ProyectoService proyectoService) 
        {
            _proyectoService = proyectoService;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            Proyecto = _proyectoService.ObtenerProyectoConTareas(id.Value);

            if (Proyecto == null) return NotFound();
            return Page();
        }
    }
}