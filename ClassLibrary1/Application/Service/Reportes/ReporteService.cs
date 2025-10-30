using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceUsuario.Application.Service;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Geom;
using System;
using ClosedXML.Excel;
using System.Globalization;

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuario_service_placeholder_removed;
        private readonly UsuarioService _usuarioService;

        private readonly PdfFont _fontRegular;
        private readonly PdfFont _fontBold;

        public ReporteService(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
        _tarea_service_placeholder_removed: _ = tareaService;
            _tareaService = tareaService;
            _usuarioService = usuarioService;
            _fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        public byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre = "Sistema")
        {
            var proyecto = _proyectoService.ObtenerProyectoConTareas(idProyecto);

            if (proyecto == null) return null;

            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                proyecto.Tareas = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();
            }

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var titulo = new Paragraph($"REPORTE DE PROYECTO: {proyecto.Nombre}")
                .SetFont(_fontBold)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(6);
            document.Add(titulo);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(2).SetMarginBottom(8));

            document.Add(CrearSeccionInfoPrincipal(proyecto));

            var subtitulo = new Paragraph("Tareas Asignadas")
                .SetFont(_fontBold)
                .SetFontSize(13)
                .SetMarginTop(6)
                .SetMarginBottom(6)
                .SetFontColor(ColorConstants.DARK_GRAY);
            document.Add(subtitulo);

            document.Add(CrearTablaTareas(proyecto));

            document.Add(new Paragraph("Integrantes: (Listado consultado desde UsuarioService según tareas asignadas)")
                .SetFont(_fontRegular)
                .SetFontSize(9)
                .SetFontColor(ColorConstants.GRAY)
                .SetMarginTop(6));

            AddPieChartForSingleProject(pdf, document, proyecto);

            document.Close();

            var bytes = stream.ToArray();
            return AddFootersToPdfBytes(bytes, usuarioNombre);
        }

        public byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre = "Sistema")
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos()?.ToList() ?? new List<Proyecto>();

            var proyectosConTareas = new List<Proyecto>();
            foreach (var p in proyectos)
            {
                var pConTareas = _proyectoService.ObtenerProyectoConTareas(p.Id) ?? p;
                proyectosConTareas.Add(pConTareas);
            }

            return GenerarReporteGeneralProyectosPdf(proyectosConTareas, usuarioNombre);
        }

        public byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            var tituloGeneral = new Paragraph("Lista de proyectos")
                .SetFont(_fontBold)
                .SetFontSize(20)
                .SetFontColor(ColorConstants.BLACK)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(8);
            document.Add(tituloGeneral);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(2).SetMarginBottom(10));

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

                var projectHeader = new Paragraph(proyecto.Nombre)
                    .SetFont(_fontBold)
                    .SetFontSize(16)
                    .SetFontColor(ColorConstants.BLACK)
                    .SetMarginTop(6)
                    .SetMarginBottom(4);
                document.Add(projectHeader);

                document.Add(new LineSeparator(new SolidLine()).SetMarginBottom(6).SetMarginTop(2));

                document.Add(CrearSeccionInfoPrincipal(proyecto));

                var subtitulo = new Paragraph("Tareas Asignadas")
                    .SetFont(_fontBold)
                    .SetFontSize(12)
                    .SetMarginTop(4)
                    .SetMarginBottom(6)
                    .SetFontColor(ColorConstants.DARK_GRAY);
                document.Add(subtitulo);

                document.Add(CrearTablaTareas(proyecto));

                document.Add(new Paragraph().SetMarginBottom(10));
            }

            AddPieChartForProjects(pdf, document, proyectos);

            document.Close();

            var generated = stream.ToArray();
            return AddFootersToPdfBytes(generated, usuarioNombre);
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

        private Table CrearSeccionInfoPrincipal(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(10)
                .SetBorder(Border.NO_BORDER);

            table.AddCell(CrearCelda("Nombre:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.Nombre ?? "-", false, null, false, TextAlignment.LEFT));

            table.AddCell(CrearCelda("Descripción:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.Descripcion ?? "N/A", false, null, false, TextAlignment.LEFT));

            table.AddCell(CrearCelda("Inicia:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.FechaInicio.ToShortDateString(), false, null, false, TextAlignment.LEFT));

            table.AddCell(CrearCelda("Finaliza:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.FechaFin.ToShortDateString(), false, null, false, TextAlignment.LEFT));

            table.AddCell(CrearCelda("Estado:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.Estado == 1 ? "Activo" : "Inactivo", false, null, false, TextAlignment.LEFT));

            return table;
        }

        private Table CrearTablaTareas(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1, 1, 4 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetMarginBottom(8);

            table.AddHeaderCell(CrearCelda("Título", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.LEFT));
            table.AddHeaderCell(CrearCelda("Prioridad", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.CENTER));
            table.AddHeaderCell(CrearCelda("Estado", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.CENTER));
            table.AddHeaderCell(CrearCelda("Emplead@/s", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.LEFT));

            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                table.AddCell(new Cell(1, 4)
                    .Add(new Paragraph("No hay tareas asignadas a este proyecto."))
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetBorder(Border.NO_BORDER)
                    .SetPadding(8));
            }
            else
            {
                int idx = 0;
                foreach (var tarea in proyecto.Tareas)
                {
                    var rowBg = (idx % 2 == 1) ? (Color)new DeviceRgb(250, 250, 250) : null;

                    table.AddCell(CrearCelda(tarea.Titulo ?? "-", false, rowBg, false, TextAlignment.LEFT));
                    table.AddCell(CrearCelda(tarea.Prioridad ?? "N/A", false, rowBg, false, TextAlignment.CENTER));

                    var estadoTexto = MapStatusToFriendly(tarea.Status, tarea.Estado);
                    var estadoColor = GetStatusColor(estadoTexto);
                    table.AddCell(CrearCelda(estadoTexto, false, estadoColor ?? rowBg, false, TextAlignment.CENTER));

                    var usuariosNombres = new List<string>();
                    if (tarea.IdUsuarioAsignado.HasValue)
                    {
                        var usuario = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                        if (usuario != null)
                        {
                            usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                        }
                    }

                    var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                    foreach (var uid in otrosIds)
                    {
                        if (tarea.IdUsuarioAsignado.HasValue && uid == tarea.IdUsuarioAsignado.Value) continue;

                        var usuario = _usuarioService.ObtenerUsuarioPorId(uid);
                        if (usuario != null)
                        {
                            usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                        }
                    }

                    var usuariosStr = usuariosNombres.Any() ? string.Join(", ", usuariosNombres.Distinct()) : "*Sin Empleados Asignados*";
                    table.AddCell(CrearCelda(usuariosStr, false, rowBg, false, TextAlignment.LEFT));

                    idx++;
                }
            }

            return table;
        }

        private Color? GetStatusColor(string friendlyStatus)
        {
            if (string.IsNullOrWhiteSpace(friendlyStatus)) return null;
            var s = friendlyStatus.ToLowerInvariant();

            if (s.Contains("sin"))
            {
                return new DeviceRgb(235, 235, 235);
            }

            if (s.Contains("en progreso") || s.Contains("enprogreso") || s.Contains("progreso"))
            {
                return new DeviceRgb(255, 244, 179);
            }

            if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done"))
            {
                return new DeviceRgb(200, 230, 201);
            }

            return null;
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

        private Cell CrearCelda(string content, bool isHeader = false, Color? bgColor = null, bool visibleBorder = false, TextAlignment alignment = TextAlignment.LEFT)
        {
            var paragraph = new Paragraph(content ?? "")
                .SetFont(isHeader ? _fontBold : _fontRegular)
                .SetFontSize(isHeader ? 10 : 10)
                .SetFontColor(isHeader ? ColorConstants.DARK_GRAY : ColorConstants.BLACK)
                .SetTextAlignment(alignment)
                .SetMargin(0);

            var cell = new Cell().Add(paragraph)
                                 .SetPadding(6)
                                 .SetBorder(visibleBorder ? new SolidBorder(ColorConstants.BLACK, 0.5f) : Border.NO_BORDER);

            if (bgColor != null)
            {
                cell.SetBackgroundColor(bgColor);
            }

            return cell;
        }

        private byte[] AddFootersToPdfBytes(byte[] inputPdfBytes, string usuarioNombre)
        {
            using var inputStream = new MemoryStream(inputPdfBytes);
            using var reader = new PdfReader(inputStream);
            using var outputStream = new MemoryStream();
            using var writer = new PdfWriter(outputStream);
            using var pdfDoc = new PdfDocument(reader, writer);

            var footerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var footerFontSmall = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            var timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            int total = pdfDoc.GetNumberOfPages();
            for (int i = 1; i <= total; i++)
            {
                var page = pdfDoc.GetPage(i);
                var pageSize = page.GetPageSize();
                var canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);
                var layoutCanvas = new iText.Layout.Canvas(canvas, pageSize);

                float x = pageSize.GetRight() - 40;
                float y = pageSize.GetBottom() + 30;

                var pageNumPara = new Paragraph($"Página {i}")
                    .SetFont(footerFont)
                    .SetFontSize(9)
                    .SetFontColor(ColorConstants.GRAY);

                layoutCanvas.ShowTextAligned(pageNumPara, x, y, TextAlignment.RIGHT);

                var genPara = new Paragraph($"Reporte Generado Por: {usuarioNombre} - {timestamp}")
                    .SetFont(footerFontSmall)
                    .SetFontSize(8)
                    .SetFontColor(ColorConstants.GRAY);

                layoutCanvas.ShowTextAligned(genPara, x, y - 10, TextAlignment.RIGHT);

                layoutCanvas.Close();
            }

            pdfDoc.Close();
            return outputStream.ToArray();
        }

        private void AddPieChartForSingleProject(PdfDocument pdf, Document document, Proyecto proyecto)
        {
            int completed = proyecto.Tareas?.Count(t => MapStatusToFriendly(t.Status, t.Estado) == "Completado") ?? 0;
            int total = proyecto.Tareas?.Count() ?? 0;
            int remaining = Math.Max(0, total - completed);

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            var title = new Paragraph("Tareas Completadas")
                .SetFont(_fontBold)
                .SetFontSize(14)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(10);
            document.Add(title);

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 })).UseAllAvailableWidth().SetMarginTop(8).SetMarginLeft(50).SetMarginRight(50);
            table.AddHeaderCell(CrearCelda("Proyecto", true));
            table.AddHeaderCell(CrearCelda("Completadas", true));
            table.AddCell(CrearCelda(proyecto.Nombre ?? "-"));
            table.AddCell(CrearCelda(completed.ToString()));
            document.Add(table);

            var smallHeading = new Paragraph("Tareas")
                .SetFont(_fontBold)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.DARK_GRAY)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginTop(6);
            document.Add(smallHeading);

            var lastPage = pdf.GetLastPage();
            var pageSize = lastPage.GetPageSize();
            var canvas = new PdfCanvas(lastPage.NewContentStreamAfter(), lastPage.GetResources(), pdf);

            float cx = pageSize.GetWidth() / 2;
            float cy = (float)(pageSize.GetBottom() + 200);
            float radius = 80;

            var slices = new List<(double value, DeviceRgb color, string label)>();
            if (completed > 0) slices.Add((completed, new DeviceRgb(200, 230, 201), "Completadas"));
            if (remaining > 0) slices.Add((remaining, new DeviceRgb(255, 244, 179), "Restantes"));

            var layoutCanvasForChart = new iText.Layout.Canvas(canvas, pageSize);
            var headingPara = new Paragraph("Tareas").SetFont(_fontBold).SetFontSize(12).SetFontColor(ColorConstants.DARK_GRAY);
            layoutCanvasForChart.ShowTextAligned(headingPara, cx, cy + radius + 14, TextAlignment.CENTER);

            DrawPie(canvas, cx, cy, radius, slices);

            var legendX = cx - radius;
            var legendY = cy - radius - 30;
            foreach (var s in slices)
            {
                canvas.SaveState();
                canvas.SetFillColor(s.color);
                canvas.Rectangle(legendX, legendY, 10, 10);
                canvas.Fill();
                canvas.RestoreState();

                var p = new Paragraph(s.label).SetFont(_fontRegular).SetFontSize(9);
                layoutCanvasForChart.ShowTextAligned(p, legendX + 16, legendY + 2, TextAlignment.LEFT);
                legendY -= 14;
            }
            layoutCanvasForChart.Close();
        }

        private void AddPieChartForProjects(PdfDocument pdf, Document document, IEnumerable<Proyecto> proyectos)
        {
            var allCounts = proyectos.Select(p => new { Name = p.Nombre ?? "-", Completed = p.Tareas?.Count(t => MapStatusToFriendly(t.Status, t.Estado) == "Completado") ?? 0 }).ToList();
            var slicesData = allCounts.Where(x => x.Completed > 0).ToList();

            if (!allCounts.Any())
            {
                document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));
                var pageMsg = pdf.GetLastPage();
                var pageSizeMsg = pageMsg.GetPageSize();
                var msg = new Paragraph("Tareas Completadas\nNo hay proyectos para mostrar.")
                    .SetFont(_fontRegular).SetFontSize(12).SetTextAlignment(TextAlignment.CENTER);
                document.Add(msg);
                return;
            }

            document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

            var title = new Paragraph("Tareas Completadas").SetFont(_fontBold).SetFontSize(14).SetTextAlignment(TextAlignment.CENTER);
            document.Add(title);

            var table = new Table(UnitValue.CreatePercentArray(new float[] { 4, 1 })).UseAllAvailableWidth().SetMarginTop(8).SetMarginLeft(50).SetMarginRight(50);
            table.AddHeaderCell(CrearCelda("Proyecto", true));
            table.AddHeaderCell(CrearCelda("Completadas", true));
            foreach (var c in allCounts)
            {
                table.AddCell(CrearCelda(c.Name));
                table.AddCell(CrearCelda(c.Completed.ToString()));
            }
            document.Add(table);

            var lastPage = pdf.GetLastPage();
            var pageSize = lastPage.GetPageSize();
            var canvas = new PdfCanvas(lastPage.NewContentStreamAfter(), lastPage.GetResources(), pdf);

            float cx = pageSize.GetWidth() / 2;
            float cy = (float)(pageSize.GetBottom() + 220);
            float radius = 100;

            var palette = new List<DeviceRgb>
            {
                new DeviceRgb(141, 196, 63),
                new DeviceRgb(255, 192, 0),
                new DeviceRgb(0, 176, 240),
                new DeviceRgb(255, 102, 102),
                new DeviceRgb(153, 102, 255)
            };

            var slices = new List<(double value, DeviceRgb color, string label)>();
            int pi = 0;
            foreach (var c in slicesData)
            {
                var color = palette[pi % palette.Count];
                slices.Add((c.Completed, color, c.Name));
                pi++;
            }

            var layoutCanvasForChartAll = new iText.Layout.Canvas(canvas, pageSize);
            var headingAll = new Paragraph("Tareas").SetFont(_fontBold).SetFontSize(12).SetFontColor(ColorConstants.DARK_GRAY);
            layoutCanvasForChartAll.ShowTextAligned(headingAll, cx, cy + radius + 14, TextAlignment.CENTER);

            DrawPie(canvas, cx, cy, radius, slices);

            var legendX = cx - radius;
            var legendY = cy - radius - 30;
            for (int i = 0; i < slices.Count; i++)
            {
                var s = slices[i];
                canvas.SaveState();
                canvas.SetFillColor(s.color);
                canvas.Rectangle(legendX, legendY, 12, 12);
                canvas.Fill();
                canvas.RestoreState();

                var labelPara = new Paragraph(s.label).SetFont(_fontRegular).SetFontSize(9);
                layoutCanvasForChartAll.ShowTextAligned(labelPara, legendX + 18, legendY + 3, TextAlignment.LEFT);
                legendY -= 14;
            }
            layoutCanvasForChartAll.Close();
        }

        private void DrawPie(PdfCanvas canvas, float cx, float cy, float r, List<(double value, DeviceRgb color, string label)> slices)
        {
            double total = slices.Sum(s => s.value);
            if (total <= 0) return;

            double startAngle = 0;
            foreach (var s in slices)
            {
                double sweep = s.value / total * 360.0;
                canvas.SaveState();
                canvas.SetFillColor(s.color);

                double start = startAngle;
                double end = startAngle + sweep;

                double sx = cx + r * Math.Cos(start * Math.PI / 180.0);
                double sy = cy + r * Math.Sin(start * Math.PI / 180.0);
                double ex = cx + r * Math.Cos(end * Math.PI / 180.0);
                double ey = cy + r * Math.Sin(end * Math.PI / 180.0);

                canvas.MoveTo(cx, cy);
                canvas.LineTo((float)sx, (float)sy);
                canvas.Arc(cx - r, cy - r, cx + r, cy + r, (float)start, (float)sweep);
                canvas.LineTo(cx, cy);

                canvas.ClosePath();
                canvas.Fill();
                canvas.RestoreState();

                startAngle += sweep;
            }
        }
    }
}