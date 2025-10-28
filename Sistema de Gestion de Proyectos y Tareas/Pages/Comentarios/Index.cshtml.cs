using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ComentarioService _comentarioService;
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService;

        public IndexModel(ComentarioService comentarioService, TareaService tareaService, ProyectoService proyectoService)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
            _proyectoService = proyectoService;
        }

        public IEnumerable<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public int UsuarioActualId { get; set; } // ? NUEVO: Para identificar enviados vs recibidos

        public void OnGet()
        {
            // ? Obtener ID del usuario actual
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(idClaim, out var usuarioId))
            {
                UsuarioActualId = usuarioId;
            }

            if (User.IsInRole("Empleado"))
            {
                var tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(UsuarioActualId);
                var comentariosFiltrados = new List<Comentario>();
                
                foreach (var tarea in tareas)
                {
                    var todos = _comentarioService.GetAll();
                    foreach (var c in todos)
                    {
                        if (c.IdTarea == tarea.Id && c.Estado == 1)
                        {
                            c.Tarea = tarea;
                            
                            var proyecto = _proyectoService.ObtenerProyectoPorId(tarea.IdProyecto);
                            if (proyecto != null && c.Tarea != null)
                            {
                                c.Tarea.ProyectoNombre = proyecto.Nombre;
                            }
                            
                            comentariosFiltrados.Add(c);
                        }
                    }
                }

                Comentarios = comentariosFiltrados;
                return;
            }

            var todosLosComentarios = _comentarioService.GetAll().ToList();
            foreach (var comentario in todosLosComentarios)
            {
                var tarea = _tareaService.ObtenerTareaPorId(comentario.IdTarea);
                if (tarea != null)
                {
                    comentario.Tarea = tarea;
                    
                    var proyecto = _proyectoService.ObtenerProyectoPorId(tarea.IdProyecto);
                    if (proyecto != null)
                    {
                        comentario.Tarea.ProyectoNombre = proyecto.Nombre;
                    }
                }
            }

            Comentarios = todosLosComentarios;
        }

        public IActionResult OnPostDelete(int id)
        {
            _comentarioService.Delete(id);
            return RedirectToPage("/Comentarios/Index");
        }
    }
}
