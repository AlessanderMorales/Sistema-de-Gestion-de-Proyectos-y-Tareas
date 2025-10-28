using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Tareas
{
    [Authorize(Policy = "OnlyJefe")]
    public class CreateModel : PageModel
    {
 private readonly TareaService _tareaService;
     private readonly ProyectoService _proyectoService;

        [BindProperty]
     public Tarea Tarea { get; set; } = new();
   
        public SelectList ProyectosDisponibles { get; set; }

        [TempData]
   public string? MensajeExito { get; set; }

        [TempData]
        public string? MensajeError { get; set; }

        public CreateModel(TareaService tareaService, ProyectoService proyectoService)
        {
     _tareaService = tareaService;
      _proyectoService = proyectoService;
        }

        public void OnGet()
        {
       var proyectos = _proyectoService.ObtenerTodosLosProyectos();
  ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");
     }

        public async Task<IActionResult> OnPostAsync()
        {
       if (!ModelState.IsValid)
    {
                var proyectos = _proyectoService.ObtenerTodosLosProyectos();
  ProyectosDisponibles = new SelectList(proyectos, "Id", "Nombre");
       return Page();
   }

   try
         {
  // Forzar estado inicial a SinIniciar
     Tarea.Status = "SinIniciar";
         
             _tareaService.CrearNuevaTarea(Tarea);
                TempData["SuccessMessage"] = "Tarea creada exitosamente.";
   return RedirectToPage("./Index");
       }
 catch (Exception ex)
            {
      TempData["ErrorMessage"] = $"Error al crear la tarea: {ex.Message}";
                return RedirectToPage("./Index");
     }
        }
    }
}
