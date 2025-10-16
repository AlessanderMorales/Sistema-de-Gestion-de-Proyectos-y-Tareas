using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class IndexModel : PageModel
    {

        private readonly ComentarioService _comentarioService;
        private readonly TareaService _tareaService;

        public IndexModel(ComentarioService comentarioService, TareaService tareaService)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
        }

        public IEnumerable<Comentario> Comentarios { get; set; } = new List<Comentario>();

        public void OnGet()
        {
            // If the user is an Empleado, show only comments related to tasks assigned to them
            if (User.IsInRole("Empleado"))
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var usuarioId))
                {
                    // Obtener tareas asignadas al usuario
                    var tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(usuarioId);
                    var comentariosFiltrados = new List<Comentario>();
                    foreach (var tarea in tareas)
                    {
                        // Obtener comentarios de cada tarea
                        // ComentarioService no tiene método para obtener por tarea en la interfaz mostrada,
                        // así que usamos GetAll y filtramos en memoria
                        var todos = _comentarioService.GetAll();
                        foreach (var c in todos)
                        {
                            if (c.IdTarea == tarea.Id && c.Estado == 1)
                                comentariosFiltrados.Add(c);
                        }
                    }

                    Comentarios = comentariosFiltrados;
                    return;
                }
            }

            // Otros roles ven todos los comentarios activos
            Comentarios = _comentarioService.GetAll();
        }

        public IActionResult OnPostDelete(int id)
        {
            _comentarioService.Delete(id);
            return RedirectToPage("/Comentarios/Index");
        }
    }
}
