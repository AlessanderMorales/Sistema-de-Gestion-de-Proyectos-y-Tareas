using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceProyecto.Application.Service;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Comentarios
{
    [Authorize]
    public class MostrarModel : PageModel
    {
        private readonly ComentarioService _comentarioService;
        private readonly TareaService _tareaService;
        private readonly ProyectoService _proyectoService;

        public Comentario Comentario { get; set; }

        public MostrarModel(ComentarioService comentarioService, TareaService tareaService, ProyectoService proyectoService)
        {
            _comentarioService = comentarioService;
            _tareaService = tareaService;
            _proyectoService = proyectoService;
        }

        public void OnGet(int id)
        {
            Comentario = _comentarioService.GetById(id);

            if (Comentario != null && Comentario.IdTarea > 0)
            {
                var tarea = _tareaService.ObtenerTareaPorId(Comentario.IdTarea);
                if (tarea != null)
                {
                    Comentario.Tarea = tarea;

                    var proyecto = _proyectoService.ObtenerProyectoPorId(tarea.IdProyecto);
                    if (proyecto != null)
                    {
                        Comentario.Tarea.ProyectoNombre = proyecto.Nombre;
                    }
                }
            }
        }
    }
}
