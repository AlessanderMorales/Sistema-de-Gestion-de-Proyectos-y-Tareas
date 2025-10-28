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

        [TempData]
        public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }
      
        public CreateModel(ProyectoService proyectoService)
        {
            _proyectoService = proyectoService;
        }
     
        public void OnGet()
        {
            Proyecto = new Proyecto
            {
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _proyectoService.CrearNuevoProyecto(Proyecto);

                TempData["SuccessMessage"] = "Proyecto creado exitosamente.";
                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al crear el proyecto: {ex.Message}";
                return RedirectToPage("./Index");
            }
        }
    }
}