using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceCommon.Infrastructure.Persistence.Data;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Proyectos
{
    [Authorize(Policy = "OnlyJefe")]
    public class GestionarUsuariosModel : PageModel
    {
        private readonly ProyectoService _proyectoService;
        private readonly UsuarioService _usuarioService;
  private readonly MySqlConnectionSingleton _connectionSingleton;

 public GestionarUsuariosModel(
            ProyectoService proyectoService,
            UsuarioService usuarioService,
   MySqlConnectionSingleton connectionSingleton)
 {
        _proyectoService = proyectoService;
          _usuarioService = usuarioService;
            _connectionSingleton = connectionSingleton;
      }

 public Proyecto Proyecto { get; set; }
 public List<Usuario> UsuariosAsignados { get; set; } = new List<Usuario>();
        public List<Usuario> UsuariosDisponibles { get; set; } = new List<Usuario>();

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
   public string MensajeError { get; set; }

        public IActionResult OnGet(int id)
        {
    Proyecto = _proyectoService.ObtenerProyectoPorId(id);
            
            if (Proyecto == null)
            {
     TempData["ErrorMessage"] = "Proyecto no encontrado.";
             return RedirectToPage("./Index");
            }

            CargarUsuarios(id);
     return Page();
        }

        public IActionResult OnPostAsignar(int idProyecto, int idUsuario)
  {
          try
     {
           using var connection = _connectionSingleton.CreateConnection();
     connection.Execute("CALL sp_asignar_usuario_proyecto(@IdProyecto, @IdUsuario)",
        new { IdProyecto = idProyecto, IdUsuario = idUsuario });

    var usuario = _usuarioService.ObtenerUsuarioPorId(idUsuario);
         TempData["SuccessMessage"] = $"{usuario.Nombres} {usuario.PrimerApellido} asignado al proyecto exitosamente.";
  }
catch (Exception ex)
  {
           TempData["ErrorMessage"] = $"Error al asignar usuario: {ex.Message}";
      }

         return RedirectToPage(new { id = idProyecto });
        }

        public IActionResult OnPostDesasignar(int idProyecto, int idUsuario)
        {
        try
 {
                using var connection = _connectionSingleton.CreateConnection();
           connection.Execute("CALL sp_desasignar_usuario_proyecto(@IdProyecto, @IdUsuario)",
               new { IdProyecto = idProyecto, IdUsuario = idUsuario });

        var usuario = _usuarioService.ObtenerUsuarioPorId(idUsuario);
             TempData["SuccessMessage"] = $"{usuario.Nombres} {usuario.PrimerApellido} desasignado del proyecto. Sus tareas en este proyecto fueron removidas.";
   }
            catch (Exception ex)
    {
        TempData["ErrorMessage"] = $"Error al desasignar usuario: {ex.Message}";
            }

          return RedirectToPage(new { id = idProyecto });
        }

    private void CargarUsuarios(int idProyecto)
        {
            using var connection = _connectionSingleton.CreateConnection();

            // Obtener usuarios asignados
  var asignados = connection.Query<Usuario>(
        "CALL sp_obtener_usuarios_proyecto(@IdProyecto)",
                new { IdProyecto = idProyecto }).ToList();

            UsuariosAsignados = asignados;

            // Obtener todos los empleados y jefes (excepto SuperAdmin)
            var todosLosUsuarios = _usuarioService.ObtenerTodosLosUsuarios()
     .Where(u => u.Rol != "SuperAdmin")
    .ToList();

      // Filtrar disponibles (no asignados)
          var idsAsignados = asignados.Select(u => u.Id).ToHashSet();
   UsuariosDisponibles = todosLosUsuarios
         .Where(u => !idsAsignados.Contains(u.Id))
     .OrderBy(u => u.PrimerApellido)
      .ToList();
        }
    }
}
