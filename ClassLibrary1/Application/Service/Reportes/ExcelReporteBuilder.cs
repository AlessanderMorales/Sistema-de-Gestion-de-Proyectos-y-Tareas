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
    public class ExcelReporteBuilder : IExcelReporteBuilder
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        public ExcelReporteBuilder(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
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

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
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