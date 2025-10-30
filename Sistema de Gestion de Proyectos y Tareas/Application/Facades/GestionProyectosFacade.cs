using System;
using System.Collections.Generic;
using System.Linq;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceComentario.Application.Service;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Facades
{
    public class GestionProyectosFacade
    {
     private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
private readonly UsuarioService _usuarioService;
        private readonly ComentarioService _comentarioService;

        public GestionProyectosFacade(
            ProyectoService proyectoService,
        TareaService tareaService,
     UsuarioService usuarioService,
  ComentarioService comentarioService)
        {
 _proyectoService = proyectoService;
      _tareaService = tareaService;
            _usuarioService = usuarioService;
            _comentarioService = comentarioService;
        }

        #region Operaciones de Proyectos

        public void EliminarProyectoCompleto(int idProyecto)
        {
      var tareas = _tareaService.ObtenerTodasLasTareas()
      .Where(t => t.IdProyecto == idProyecto)
                .ToList();

            foreach (var tarea in tareas)
     {
                var comentarios = _comentarioService.GetAll()
            .Where(c => c.IdTarea == tarea.Id);
          
         foreach (var comentario in comentarios)
      {
        _comentarioService.Delete(comentario.Id);
         }
        }

   _proyectoService.EliminarProyectoPorId(idProyecto);
      }

        public ProyectoDetalladoViewModel ObtenerProyectoDetallado(int idProyecto)
        {
      var proyecto = _proyectoService.ObtenerProyectoConTareas(idProyecto);
          if (proyecto == null)
      throw new InvalidOperationException($"Proyecto con ID {idProyecto} no encontrado");

        var usuariosIds = proyecto.Tareas?
     .Where(t => t.IdUsuarioAsignado.HasValue)
             .Select(t => t.IdUsuarioAsignado.Value)
    .Distinct()
                .ToList() ?? new List<int>();

          var usuarios = usuariosIds
         .Select(id => _usuarioService.ObtenerUsuarioPorId(id))
   .Where(u => u != null)
     .ToList();

            return new ProyectoDetalladoViewModel
            {
      Proyecto = proyecto,
    TotalTareas = proyecto.Tareas?.Count ?? 0,
      TareasCompletadas = proyecto.Tareas?.Count(t => t.Status == "Completada") ?? 0,
 TareasEnProgreso = proyecto.Tareas?.Count(t => t.Status == "EnProgreso") ?? 0,
 TareasPendientes = proyecto.Tareas?.Count(t => t.Status == "SinIniciar") ?? 0,
           UsuariosAsignados = usuarios
        };
        }

        #endregion

        #region Operaciones de Tareas

 public Tarea CrearTareaCompleta(Tarea tarea, List<int>? idsUsuarios = null)
        {
            var proyecto = _proyectoService.ObtenerProyectoPorId(tarea.IdProyecto);
 if (proyecto == null)
     throw new InvalidOperationException($"Proyecto con ID {tarea.IdProyecto} no encontrado");

  _tareaService.CrearNuevaTarea(tarea);


            if (idsUsuarios != null && idsUsuarios.Any())
            {
_tareaService.AsignarMultiplesUsuarios(tarea.Id, idsUsuarios);
        }

         return tarea;
        }

     public void ReasignarTarea(int idTarea, List<int> nuevosUsuariosIds)
        {
            var tarea = _tareaService.ObtenerTareaPorId(idTarea);
  if (tarea == null)
   throw new InvalidOperationException($"Tarea con ID {idTarea} no encontrada");

   _tareaService.AsignarMultiplesUsuarios(idTarea, nuevosUsuariosIds);
      }

        public void CambiarEstadoTarea(int idTarea, string nuevoEstado)
        {
  var tarea = _tareaService.ObtenerTareaPorId(idTarea);
     if (tarea == null)
       throw new InvalidOperationException($"Tarea con ID {idTarea} no encontrada");

            ValidarTransicionEstado(tarea.Status, nuevoEstado);

 tarea.Status = nuevoEstado;
          _tareaService.ActualizarTarea(tarea);
        }

        private void ValidarTransicionEstado(string estadoActual, string nuevoEstado)
      {
            if (estadoActual == "Completada" && nuevoEstado == "SinIniciar")
 {
          throw new InvalidOperationException("No se puede regresar una tarea completada a 'Sin Iniciar'");
 }
        }

        #endregion

        #region Operaciones de Usuarios
        public DashboardUsuarioViewModel ObtenerDashboardUsuario(int idUsuario)
        {
   var usuario = _usuarioService.ObtenerUsuarioPorId(idUsuario);
      if (usuario == null)
           throw new InvalidOperationException($"Usuario con ID {idUsuario} no encontrado");

        var proyectos = _proyectoService.ObtenerProyectosPorUsuarioAsignado(idUsuario).ToList();
       var tareas = _tareaService.ObtenerTareasPorUsuarioAsignado(idUsuario).ToList();

            return new DashboardUsuarioViewModel
      {
      Usuario = usuario,
            Proyectos = proyectos,
      TotalProyectos = proyectos.Count,
    Tareas = tareas,
           TotalTareas = tareas.Count,
     TareasCompletadas = tareas.Count(t => t.Status == "Completada"),
              TareasEnProgreso = tareas.Count(t => t.Status == "EnProgreso"),
                TareasPendientes = tareas.Count(t => t.Status == "SinIniciar"),
 ProyectosActivos = proyectos.Count(p => p.Estado == 1)
            };
        }

        public EstadisticasGeneralesViewModel ObtenerEstadisticasGenerales()
        {
  var proyectos = _proyectoService.ObtenerTodosLosProyectos().ToList();
          var tareas = _tareaService.ObtenerTodasLasTareas().ToList();
            var usuarios = _usuarioService.ObtenerTodosLosUsuarios()
              .Where(u => u.Rol != "SuperAdmin")
    .ToList();

   return new EstadisticasGeneralesViewModel
   {
     TotalProyectos = proyectos.Count,
             ProyectosActivos = proyectos.Count(p => p.Estado == 1),
        TotalTareas = tareas.Count,
             TareasCompletadas = tareas.Count(t => t.Status == "Completada"),
    TareasEnProgreso = tareas.Count(t => t.Status == "EnProgreso"),
             TareasPendientes = tareas.Count(t => t.Status == "SinIniciar"),
      TotalUsuarios = usuarios.Count,
      Empleados = usuarios.Count(u => u.Rol == "Empleado"),
   JefesDeProyecto = usuarios.Count(u => u.Rol == "JefeDeProyecto")
            };
        }

        #endregion
    }

#region ViewModels

    public class ProyectoDetalladoViewModel
    {
        public Proyecto Proyecto { get; set; }
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
public int TareasEnProgreso { get; set; }
        public int TareasPendientes { get; set; }
        public List<Usuario> UsuariosAsignados { get; set; } = new List<Usuario>();
    }

    public class DashboardUsuarioViewModel
    {
        public Usuario Usuario { get; set; }
     public List<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
 public int TotalProyectos { get; set; }
        public List<Tarea> Tareas { get; set; } = new List<Tarea>();
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasEnProgreso { get; set; }
        public int TareasPendientes { get; set; }
   public int ProyectosActivos { get; set; }
    }

    public class EstadisticasGeneralesViewModel
  {
      public int TotalProyectos { get; set; }
 public int ProyectosActivos { get; set; }
        public int TotalTareas { get; set; }
        public int TareasCompletadas { get; set; }
        public int TareasEnProgreso { get; set; }
        public int TareasPendientes { get; set; }
        public int TotalUsuarios { get; set; }
        public int Empleados { get; set; }
        public int JefesDeProyecto { get; set; }
    }

    #endregion
}
