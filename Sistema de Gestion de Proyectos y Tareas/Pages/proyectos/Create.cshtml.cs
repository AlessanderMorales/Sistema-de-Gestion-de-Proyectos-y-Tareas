using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using System.Globalization;

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
       // Las fechas se inicializarán vacías para que el usuario las ingrese manualmente
       }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
     return Page();
  }

        try
          {
      // ? Aplicar trim automático a campos de texto
            if (!string.IsNullOrEmpty(Proyecto.Nombre))
        {
    Proyecto.Nombre = TrimExtraSpaces(Proyecto.Nombre);
      }
 
           if (!string.IsNullOrEmpty(Proyecto.Descripcion))
          {
     Proyecto.Descripcion = TrimExtraSpaces(Proyecto.Descripcion);
             }

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

     /// <summary>
    /// Elimina espacios al inicio, al final y múltiples espacios consecutivos en el medio
     /// </summary>
   private string TrimExtraSpaces(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
         
            // Eliminar espacios al inicio y al final
      input = input.Trim();
  
  // Reemplazar múltiples espacios consecutivos por uno solo
          return System.Text.RegularExpressions.Regex.Replace(input, @"\s+", " ");
     }
    }
}