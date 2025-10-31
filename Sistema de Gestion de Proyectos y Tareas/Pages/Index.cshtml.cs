using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Facades;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly GestionProyectosFacade _facade;

        public IndexModel(GestionProyectosFacade facade)
        {
            _facade = facade;
        }

        // Propiedades para mostrar estad�sticas
        public EstadisticasGeneralesViewModel Estadisticas { get; set; }
        public DashboardUsuarioViewModel DashboardUsuario { get; set; }

        public IActionResult OnGet()
        {
            // Redirigir SuperAdmin a su p�gina principal
            if (User.IsInRole("SuperAdmin"))
            {
                return RedirectToPage("/Usuarios/Index");
            }

            // Obtener estad�sticas usando el Facade
            if (User.IsInRole("JefeDeProyecto"))
            {
                Estadisticas = _facade.ObtenerEstadisticasGenerales();
            }

            // Obtener dashboard del usuario usando el Facade
            if (User.IsInRole("Empleado") || User.IsInRole("JefeDeProyecto"))
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var usuarioId))
                {
                    // ? CORRECCI�N: Manejar cuando el usuario no existe
                    DashboardUsuario = _facade.ObtenerDashboardUsuario(usuarioId);

                    if (DashboardUsuario == null)
                    {
                        // Usuario no encontrado - posiblemente fue eliminado pero la sesi�n sigue activa
                        TempData["ErrorMessage"] = "Tu cuenta de usuario no fue encontrada. Por favor, contacta al administrador o inicia sesi�n nuevamente.";
                        return RedirectToPage("/Logout");
                    }
                }
            }

            return Page();
        }
    }
}