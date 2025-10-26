using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        [BindProperty]
        public Proyecto Proyecto { get; set; } = new();
        public CreateModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }
        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            _proyectoService.CrearNuevoProyecto(Proyecto);

            return RedirectToPage("./Index");
        }
    }
}