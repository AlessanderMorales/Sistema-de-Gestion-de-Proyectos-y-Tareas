using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Redirigir SuperAdmin a su p�gina principal
            if (User.IsInRole("SuperAdmin"))
            {
                return RedirectToPage("/Usuarios/Index");
            }

            // Jefes y Empleados ven la p�gina de inicio normal
            return Page();
        }
    }
}