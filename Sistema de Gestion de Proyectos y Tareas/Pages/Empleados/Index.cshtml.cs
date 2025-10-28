using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceProyecto.Application.Service;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Empleados
{
    [Authorize(Policy = "OnlyJefe")]
    public class IndexModel : PageModel
    {
        private readonly UsuarioService _usuarioService;
        private readonly TareaService _tareaService;
      private readonly ProyectoService _proyectoService;

        public IndexModel(UsuarioService usuarioService, TareaService tareaService, ProyectoService proyectoService)
        {
          _usuarioService = usuarioService;
            _tareaService = tareaService;
     _proyectoService = proyectoService;
        }

  public class EmpleadoInfo
        {
            public Usuario Usuario { get; set; }
      public List<string> ProyectosAsignados { get; set; } = new List<string>();
            public List<string> TareasAsignadas { get; set; } = new List<string>();
      public int TotalProyectos { get; set; }
            public int TotalTareas { get; set; }
   }

  public List<EmpleadoInfo> Empleados { get; set; } = new List<EmpleadoInfo>();

        public void OnGet()
        {
  // Obtener todos los usuarios excepto SuperAdmin
       var todosLosUsuarios = _usuarioService.ObtenerTodosLosUsuarios()
       .Where(u => u.Rol != "SuperAdmin")
      .OrderBy(u => u.Rol)
.ThenBy(u => u.PrimerApellido)
      .ToList();

            var todosLosProyectos = _proyectoService.ObtenerTodosLosProyectos().ToList();
   var todasLasTareas = _tareaService.ObtenerTodasLasTareas().ToList();

            foreach (var usuario in todosLosUsuarios)
            {
         var empleadoInfo = new EmpleadoInfo
          {
        Usuario = usuario
                };

  // Obtener proyectos del usuario (si es empleado)
     if (usuario.Rol == "Empleado")
      {
        var proyectosDelUsuario = _proyectoService.ObtenerProyectosPorUsuarioAsignado(usuario.Id).ToList();
    empleadoInfo.ProyectosAsignados = proyectosDelUsuario.Select(p => p.Nombre).ToList();
          empleadoInfo.TotalProyectos = proyectosDelUsuario.Count;
              }

        // Obtener tareas asignadas del usuario
        var tareasDelUsuario = _tareaService.ObtenerTareasPorUsuarioAsignado(usuario.Id).ToList();
          empleadoInfo.TareasAsignadas = tareasDelUsuario.Select(t => t.Titulo).ToList();
 empleadoInfo.TotalTareas = tareasDelUsuario.Count;

            Empleados.Add(empleadoInfo);
            }
        }
    }
}
