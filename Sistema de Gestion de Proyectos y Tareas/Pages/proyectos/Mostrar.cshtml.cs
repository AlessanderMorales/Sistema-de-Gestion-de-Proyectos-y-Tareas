using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Facades;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.proyectos
{
    [Authorize]
    public class MostrarModel : PageModel
    {
        private readonly GestionProyectosFacade _facade;
        public ProyectoDetalladoViewModel ProyectoDetallado { get; private set; }

        public MostrarModel(GestionProyectosFacade facade)
        {
            _facade = facade;
        }

        public IActionResult OnGet(int? id)
        {
            if (id == null) return NotFound();
            
            try
            {
                ProyectoDetallado = _facade.ObtenerProyectoDetallado(id.Value);
                return Page();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}