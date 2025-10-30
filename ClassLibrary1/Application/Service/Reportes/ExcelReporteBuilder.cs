using ClosedXML.Excel;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ExcelReporteBuilder
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        public ExcelReporteBuilder(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _tarea_service_placeholder: _ = tareaService; // evita advertencias si no se usa directamente
            _tareaService = tareaService;
            _usuarioService = usuarioService;
        }

        public byte[] GenerarReporteGeneralProyectosExcel(string usuarioNombre = "Sistema")
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos()?.ToList() ?? new List<Proyecto>();

            var proyectosConTareas = new List<Proyecto>();
            foreach (var p in proyectos)
            {
                var pConTareas = _proyectoService.ObtenerProyectoConTareas(p.Id) ?? p;
                proyectosConTareas.Add(pConTareas);
            }

            return GenerarReporteGeneralProyectosExcel(proyectosConTareas, usuarioNombre);
        }

        public byte[] GenerarReporteGeneralProyectosExcel(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Proyectos");

            var headers = new[]
            {
                "Proyecto",
                "Descripción",
                "Fecha Inicio",
                "Fecha Fin",
                "Estado Proyecto",
                "Tarea Título",
                "Prioridad",
                "Estado Tarea",
                "Usuarios Asignados"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
            }
            ws.Row(1).Style.Font.Bold = true;
            ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.SheetView.FreezeRows(1);

            int row = 2;
            foreach (var proyecto in proyectos)
            {
                if (proyecto.Tareas == null || !proyecto.Tareas.Any())
                {
                    var pConTareas = _proyectoService.ObtenerProyectoConTareas(proyecto.Id);
                    if (pConTareas?.Tareas != null && pConTareas.Tareas.Any())
                    {
                        proyecto.Tareas = pConTareas.Tareas;
                    }
                    else
                    {
                        var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                        proyecto.Tareas = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();
                    }
                }

                if (proyecto.Tareas == null || !proyecto.Tareas.Any())
                {
                    ws.Cell(row, 1).Value = proyecto.Nombre ?? "-";
                    ws.Cell(row, 2).Value = proyecto.Descripcion ?? "-";
                    ws.Cell(row, 3).Value = proyecto.FechaInicio.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    ws.Cell(row, 4).Value = proyecto.FechaFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    ws.Cell(row, 5).Value = proyecto.Estado == 1 ? "Activo" : "Inactivo";
                    for (int c = 6; c <= 9; c++) ws.Cell(row, c).Value = string.Empty;
                    row++;
                }
                else
                {
                    foreach (var tarea in proyecto.Tareas)
                    {
                        ws.Cell(row, 1).Value = proyecto.Nombre ?? "-";
                        ws.Cell(row, 2).Value = proyecto.Descripcion ?? "-";
                        ws.Cell(row, 3).Value = proyecto.FechaInicio.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cell(row, 4).Value = proyecto.FechaFin.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ws.Cell(row, 5).Value = proyecto.Estado == 1 ? "Activo" : "Inactivo";

                        ws.Cell(row, 6).Value = tarea.Titulo ?? "-";
                        ws.Cell(row, 7).Value = tarea.Prioridad ?? "-";
                        ws.Cell(row, 8).Value = MapStatusToFriendly(tarea.Status, tarea.Estado);

                        var usuariosNombres = new List<string>();
                        if (tarea.IdUsuarioAsignado.HasValue)
                        {
                            var usuario = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                            if (usuario != null)
                                usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                        }

                        var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                        foreach (var uid in otrosIds)
                        {
                            if (tarea.IdUsuarioAsignado.HasValue && uid == tarea.IdUsuarioAsignado.Value) continue;
                            var usuario = _usuarioService.ObtenerUsuarioPorId(uid);
                            if (usuario != null)
                                usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                        }

                        var usuariosStr = usuariosNombres.Any() ? string.Join(", ", usuariosNombres.Distinct()) : "*Sin Empleados Asignados*";
                        ws.Cell(row, 9).Value = usuariosStr;

                        row++;
                    }
                }
            }

            ws.ColumnsUsed().AdjustToContents();

            workbook.Properties.Author = usuarioNombre;
            workbook.Properties.Title = "Reporte General Proyectos";
            workbook.Properties.Created = DateTime.Now;

            ws.Cell(row + 1, 1).Value = $"Reporte generado por: {usuarioNombre}";
            ws.Cell(row + 2, 1).Value = $"Fecha: {DateTime.Now.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)}";

            AgregarHojaEstadisticas(workbook, proyectos);

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        private void AgregarHojaEstadisticas(XLWorkbook workbook, IEnumerable<Proyecto> proyectos)
        {
            var wsStats = workbook.Worksheets.Add("Estadísticas");

            var todasLasTareas = proyectos
                .SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>())
                .ToList();

            if (!todasLasTareas.Any())
            {
                wsStats.Cell(1, 1).Value = "No hay tareas para generar estadísticas.";
                return;
            }

            wsStats.Cell(1, 1).Value = "ESTADÍSTICAS GENERALES DE TAREAS";
            wsStats.Cell(1, 1).Style.Font.Bold = true;
            wsStats.Cell(1, 1).Style.Font.FontSize = 16;
            wsStats.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.DarkBlue;
            wsStats.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            wsStats.Range(1, 1, 1, 6).Merge();
            wsStats.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            wsStats.Cell(3, 1).Value = "Distribución por Estado";
            wsStats.Cell(3, 1).Style.Font.Bold = true;
            wsStats.Cell(3, 1).Style.Font.FontSize = 12;
            wsStats.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            wsStats.Range(3, 1, 3, 2).Merge();

 wsStats.Cell(4, 1).Value = "Estado";
wsStats.Cell(4, 2).Value = "Cantidad";
       wsStats.Cell(4, 3).Value = "Porcentaje";
   wsStats.Range(4, 1, 4, 3).Style.Font.Bold = true;
  wsStats.Range(4, 1, 4, 3).Style.Fill.BackgroundColor = XLColor.LightGray;

     var tareasCompletadas = todasLasTareas.Count(t => t.Status == "Completada");
     var tareasEnProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
      var tareasSinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");
    int totalTareas = todasLasTareas.Count;

    wsStats.Cell(5, 1).Value = "Completadas";
      wsStats.Cell(5, 2).Value = tareasCompletadas;
 wsStats.Cell(5, 3).Value = $"{(tareasCompletadas * 100.0 / totalTareas):F1}%";
    wsStats.Range(5, 1, 5, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(200, 230, 201);
        wsStats.Cell(5, 1).Style.Font.Bold = true;

        wsStats.Cell(6, 1).Value = "? En Progreso";
   wsStats.Cell(6, 2).Value = tareasEnProgreso;
        wsStats.Cell(6, 3).Value = $"{(tareasEnProgreso * 100.0 / totalTareas):F1}%";
wsStats.Range(6, 1, 6, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 244, 179);
        wsStats.Cell(6, 1).Style.Font.Bold = true;

    wsStats.Cell(7, 1).Value = "Sin Iniciar";
     wsStats.Cell(7, 2).Value = tareasSinIniciar;
       wsStats.Cell(7, 3).Value = $"{(tareasSinIniciar * 100.0 / totalTareas):F1}%";
    wsStats.Range(7, 1, 7, 3).Style.Fill.BackgroundColor = XLColor.FromArgb(224, 224, 224);
wsStats.Cell(7, 1).Style.Font.Bold = true;

   wsStats.Cell(8, 1).Value = "TOTAL";
    wsStats.Cell(8, 2).Value = totalTareas;
   wsStats.Cell(8, 3).Value = "100%";
  wsStats.Range(8, 1, 8, 3).Style.Font.Bold = true;
     wsStats.Range(8, 1, 8, 3).Style.Fill.BackgroundColor = XLColor.Gray;
   wsStats.Range(8, 1, 8, 3).Style.Font.FontColor = XLColor.White;

            wsStats.Cell(3, 5).Value = "Distribución por Prioridad";
             wsStats.Cell(3, 5).Style.Font.Bold = true;
            wsStats.Cell(3, 5).Style.Font.FontSize = 12;
             wsStats.Cell(3, 5).Style.Fill.BackgroundColor = XLColor.LightGray;
                wsStats.Range(3, 5, 3, 7).Merge();

   wsStats.Cell(4, 5).Value = "Prioridad";
   wsStats.Cell(4, 6).Value = "Cantidad";
       wsStats.Cell(4, 7).Value = "Porcentaje";
    wsStats.Range(4, 5, 4, 7).Style.Font.Bold = true;
   wsStats.Range(4, 5, 4, 7).Style.Fill.BackgroundColor = XLColor.LightGray;

 var tareasAlta = todasLasTareas.Count(t => t.Prioridad == "Alta");
  var tareasMedia = todasLasTareas.Count(t => t.Prioridad == "Media");
   var tareasBaja = todasLasTareas.Count(t => t.Prioridad == "Baja");

  wsStats.Cell(5, 5).Value = "Alta";
     wsStats.Cell(5, 6).Value = tareasAlta;
    wsStats.Cell(5, 7).Value = $"{(tareasAlta * 100.0 / totalTareas):F1}%";
 wsStats.Range(5, 5, 5, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 205, 210);
   wsStats.Cell(5, 5).Style.Font.Bold = true;

     wsStats.Cell(6, 5).Value = "Media";
wsStats.Cell(6, 6).Value = tareasMedia;
    wsStats.Cell(6, 7).Value = $"{(tareasMedia * 100.0 / totalTareas):F1}%";
       wsStats.Range(6, 5, 6, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 224, 178);
  wsStats.Cell(6, 5).Style.Font.Bold = true;

  wsStats.Cell(7, 5).Value = "Baja";
    wsStats.Cell(7, 6).Value = tareasBaja;
    wsStats.Cell(7, 7).Value = $"{(tareasBaja * 100.0 / totalTareas):F1}%";
       wsStats.Range(7, 5, 7, 7).Style.Fill.BackgroundColor = XLColor.FromArgb(200, 230, 201);
      wsStats.Cell(7, 5).Style.Font.Bold = true;

       wsStats.Cell(8, 5).Value = "TOTAL";
   wsStats.Cell(8, 6).Value = totalTareas;
     wsStats.Cell(8, 7).Value = "100%";
 wsStats.Range(8, 5, 8, 7).Style.Font.Bold = true;
      wsStats.Range(8, 5, 8, 7).Style.Fill.BackgroundColor = XLColor.Gray;
       wsStats.Range(8, 5, 8, 7).Style.Font.FontColor = XLColor.White;

  wsStats.Cell(11, 1).Value = "TOTAL";
       wsStats.Cell(11, 1).Style.Font.Bold = true;
  wsStats.Cell(11, 1).Style.Font.FontSize = 14;
  wsStats.Cell(11, 1).Style.Fill.BackgroundColor = XLColor.DarkBlue;
   wsStats.Cell(11, 1).Style.Font.FontColor = XLColor.White;
  wsStats.Range(11, 1, 11, 7).Merge();
     wsStats.Cell(11, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

   wsStats.Cell(13, 1).Value = "Total de Proyectos:";
   wsStats.Cell(13, 2).Value = proyectos.Count();
  wsStats.Cell(13, 1).Style.Font.Bold = true;

      wsStats.Cell(14, 1).Value = "Total de Tareas:";
    wsStats.Cell(14, 2).Value = totalTareas;
wsStats.Cell(14, 1).Style.Font.Bold = true;

wsStats.Cell(15, 1).Value = "% Completado:";
       wsStats.Cell(15, 2).Value = $"{(tareasCompletadas * 100.0 / totalTareas):F1}%";
     wsStats.Cell(15, 1).Style.Font.Bold = true;
   wsStats.Cell(15, 2).Style.Fill.BackgroundColor = XLColor.LightGreen;

       wsStats.Cell(16, 1).Value = "% En Progreso:";
     wsStats.Cell(16, 2).Value = $"{(tareasEnProgreso * 100.0 / totalTareas):F1}%";
   wsStats.Cell(16, 1).Style.Font.Bold = true;
        wsStats.Cell(16, 2).Style.Fill.BackgroundColor = XLColor.LightYellow;

  wsStats.Cell(17, 1).Value = "% Pendiente:";
     wsStats.Cell(17, 2).Value = $"{(tareasSinIniciar * 100.0 / totalTareas):F1}%";
   wsStats.Cell(17, 1).Style.Font.Bold = true;
     wsStats.Cell(17, 2).Style.Fill.BackgroundColor = XLColor.LightGray;

   wsStats.Range(3, 1, 8, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
        wsStats.Range(3, 5, 8, 7).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
    wsStats.Range(13, 1, 17, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

       wsStats.ColumnsUsed().AdjustToContents();
    }

        private string MapStatusToFriendly(string status, int estado)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                var s = status.Trim().ToLowerInvariant();
                if ((s.Contains("sin") && s.Contains("iniciar")) || s.Contains("sininiciar") || s == "sininiciar") return "Sin iniciar";
                if ((s.Contains("en") && s.Contains("progreso")) || s.Contains("enprogreso") || s.Contains("en progreso")) return "En progreso";
                if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done")) return "Completado";
                return char.ToUpper(status[0]) + status.Substring(1);
            }

            return estado == 1 ? "Activo" : "Inactivo";
        }
    }
}