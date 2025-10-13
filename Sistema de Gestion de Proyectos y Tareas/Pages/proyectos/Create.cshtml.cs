
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
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