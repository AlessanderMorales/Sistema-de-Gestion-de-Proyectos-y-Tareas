using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
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
            var user = User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var role = user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                if (role.ToLowerInvariant() == "admin" || role.ToLowerInvariant() == "jefe de proyecto" || role.ToLowerInvariant() == "supervisor")
                {
                    Tareas = _tareaService.ObtenerTodasLasTareas();
                }
                else
                {
                    // show only tasks assigned to this user
                    var idClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var userId))
                    {
                        Tareas = _tareaService.ObtenerTareasPorUsuario(userId);
                    }
                    else
                    {
                        Tareas = new List<Tarea>();
                    }
                }
            }
            else
            {
                // not authenticated -> empty
                Tareas = new List<Tarea>();
            }
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