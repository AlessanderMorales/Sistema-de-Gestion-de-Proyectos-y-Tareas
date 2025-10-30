﻿using iText.IO.Font.Constants;
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

namespace ServiceProyecto.Application.Service.Reportes
{
    public class ReporteService
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuario_service_placeholder_removed; // placeholder to avoid accidental references
        private readonly UsuarioService _usuarioService;

        // Fuentes
        private readonly PdfFont _fontRegular;
        private readonly PdfFont _fontBold;

        public ReporteService(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
            _tarea_service_placeholder_removed: _ = tareaService; // no-op to avoid accidental older references
            _tareaService = tareaService;
            _usuarioService = usuarioService;
            _fontRegular = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            _fontBold = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
        }

        // -----------------------------
        // Método para 1 proyecto
        // -----------------------------
        public byte[] GenerarReporteProyectoPdf(int idProyecto, string usuarioNombre = "Sistema")
        {
            var proyecto = _proyectoService.ObtenerProyectoConTareas(idProyecto);

            if (proyecto == null) return null;

            // Asegurar que las tareas estén cargadas; si no, intentar obtenerlas desde TareaService
            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                var todas = _tareaService.ObtenerTodasLasTareas() ?? Enumerable.Empty<Tarea>();
                proyecto.Tareas = todas.Where(t => t.IdProyecto == proyecto.Id).ToList();
            }

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título del proyecto (single) - mantengo formato claro pero no demasiado distinto
            var titulo = new Paragraph($"REPORTE DE PROYECTO: {proyecto.Nombre}")
                .SetFont(_fontBold)
                .SetFontSize(18)
                .SetFontColor(ColorConstants.BLUE)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(6);
            document.Add(titulo);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(2).SetMarginBottom(8));

            // Info principal sin bordes (más elegante)
            document.Add(CrearSeccionInfoPrincipal(proyecto));

            // Tareas
            var subtitulo = new Paragraph("Tareas Asignadas")
                .SetFont(_fontBold)
                .SetFontSize(13)
                .SetMarginTop(6)
                .SetMarginBottom(6)
                .SetFontColor(ColorConstants.DARK_GRAY);
            document.Add(subtitulo);

            document.Add(CrearTablaTareas(proyecto));

            // Nota pequeña
            document.Add(new Paragraph("Integrantes: (Listado consultado desde UsuarioService según tareas asignadas)")
                .SetFont(_fontRegular)
                .SetFontSize(9)
                .SetFontColor(ColorConstants.GRAY)
                .SetMarginTop(6));

            // Close document to finalize pages
            document.Close();

            // Post-process bytes to add footers safely
            var bytes = stream.ToArray();
            return AddFootersToPdfBytes(bytes, usuarioNombre);
        }

        // -----------------------------
        // Lista de proyectos (general) - título actualizado y más elegante
        // -----------------------------
        public byte[] GenerarReporteGeneralProyectosPdf(string usuarioNombre = "Sistema")
        {
            var proyectos = _proyectoService.ObtenerTodosLosProyectos()?.ToList() ?? new List<Proyecto>();

            // Asegurarse de que cada proyecto tenga sus tareas cargadas
            var proyectosConTareas = new List<Proyecto>();
            foreach (var p in proyectos)
            {
                var pConTareas = _proyectoService.ObtenerProyectoConTareas(p.Id) ?? p;
                proyectosConTareas.Add(pConTareas);
            }

            return GenerarReporteGeneralProyectosPdf(proyectosConTareas, usuarioNombre);
        }

        // Mantengo el método existente que recibe proyectos (compatibilidad)
        public byte[] GenerarReporteGeneralProyectosPdf(IEnumerable<Proyecto> proyectos, string usuarioNombre = "Sistema")
        {
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var document = new Document(pdf);

            // Título general elegante y negro
            var tituloGeneral = new Paragraph("Lista de proyectos")
                .SetFont(_fontBold)
                .SetFontSize(20) // aumentar para que sobresalga
                .SetFontColor(ColorConstants.BLACK)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(8);
            document.Add(tituloGeneral);

            document.Add(new LineSeparator(new SolidLine()).SetMarginTop(2).SetMarginBottom(10));

            foreach (var proyecto in proyectos)
            {
                // Asegurar que las tareas estén cargadas
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

                // Project title as emphasized header
                var projectHeader = new Paragraph(proyecto.Nombre)
                    .SetFont(_fontBold)
                    .SetFontSize(16) // más grande para destacar
                    .SetFontColor(ColorConstants.BLACK) // elegante negro
                    .SetMarginTop(6)
                    .SetMarginBottom(4);
                document.Add(projectHeader);

                // Línea sutil para separar
                document.Add(new LineSeparator(new SolidLine()).SetMarginBottom(6).SetMarginTop(2));

                // Info (borderless table)
                document.Add(CrearSeccionInfoPrincipal(proyecto));

                // Tareas (borderless table with subtle row spacing)
                var subtitulo = new Paragraph("Tareas Asignadas")
                    .SetFont(_fontBold)
                    .SetFontSize(12)
                    .SetMarginTop(4)
                    .SetMarginBottom(6)
                    .SetFontColor(ColorConstants.DARK_GRAY);
                document.Add(subtitulo);

                document.Add(CrearTablaTareas(proyecto));

                // Invisible spacing between projects
                document.Add(new Paragraph().SetMarginBottom(10));
            }

            // Close document to finalize pages
            document.Close();

            // Post-process bytes to add footers safely
            var generated = stream.ToArray();
            return AddFootersToPdfBytes(generated, usuarioNombre);
        }

        // -----------------------------
        // Métodos internos
        // -----------------------------
        private Table CrearSeccionInfoPrincipal(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(10)
                .SetBorder(Border.NO_BORDER);

            // Label (negrita) / Value (normal) pairs
            table.AddCell(CrearCelda("ID Proyecto:", true, null, false, TextAlignment.RIGHT));
            table.AddCell(CrearCelda(proyecto.Id.ToString(), false, null, false, TextAlignment.LEFT));

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

        // Ahora incluye columna "Usuarios" con los usuarios asignados a cada tarea (tanto IdUsuarioAsignado como asignaciones en TareaUsuario)
        private Table CrearTablaTareas(Proyecto proyecto)
        {
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 4, 1, 1, 4 }))
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetMarginBottom(8);

            // Encabezados (sin borde, fondo sutil)
            table.AddHeaderCell(CrearCelda("ID", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.CENTER));
            table.AddHeaderCell(CrearCelda("Título", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.LEFT));
            table.AddHeaderCell(CrearCelda("Prioridad", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.CENTER));
            table.AddHeaderCell(CrearCelda("Estado", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.CENTER));
            table.AddHeaderCell(CrearCelda("Usuarios", true, ColorConstants.LIGHT_GRAY, false, TextAlignment.LEFT));

            if (proyecto.Tareas == null || !proyecto.Tareas.Any())
            {
                table.AddCell(new Cell(1, 5)
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

                    table.AddCell(CrearCelda(tarea.Id.ToString(), false, rowBg, false, TextAlignment.CENTER));
                    table.AddCell(CrearCelda(tarea.Titulo ?? "-", false, rowBg, false, TextAlignment.LEFT));
                    table.AddCell(CrearCelda(tarea.Prioridad ?? "N/A", false, rowBg, false, TextAlignment.CENTER));

                    var estadoTexto = MapStatusToFriendly(tarea.Status, tarea.Estado);
                    var estadoColor = GetStatusColor(estadoTexto);
                    table.AddCell(CrearCelda(estadoTexto, false, estadoColor ?? rowBg, false, TextAlignment.CENTER));

                    // Usuarios
                    var usuariosNombres = new List<string>();
                    if (tarea.IdUsuarioAsignado.HasValue)
                    {
                        var usuario = _usuarioService.ObtenerUsuarioPorId(tarea.IdUsuarioAsignado.Value);
                        if (usuario != null)
                        {
                            usuariosNombres.Add(($"{usuario.Nombres} {usuario.PrimerApellido}").Trim());
                        }
                    }

                    // Usuarios en tabla intermedia (TareaUsuario)
                    var otrosIds = _tareaService.ObtenerIdsUsuariosAsignados(tarea.Id) ?? Enumerable.Empty<int>();
                    foreach (var uid in otrosIds)
                    {
                        // Evitar duplicados con el IdUsuarioAsignado
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
                // Sin iniciar -> gris suave
                return new DeviceRgb(235, 235, 235);
            }

            if (s.Contains("en progreso") || s.Contains("enprogreso") || s.Contains("progreso"))
            {
                // En progreso -> amarillo/anaranjado suave
                return new DeviceRgb(255, 244, 179);
            }

            if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done"))
            {
                // Completado -> verde suave
                return new DeviceRgb(200, 230, 201);
            }

            // fallback
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
                // default: capitalize first letter
                return char.ToUpper(status[0]) + status.Substring(1);
            }

            // fallback by numeric Estado
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

        // Post-process PDF bytes and draw footer on each page
        private byte[] AddFootersToPdfBytes(byte[] inputPdfBytes, string usuarioNombre)
        {
            using var inputStream = new MemoryStream(inputPdfBytes);
            using var reader = new PdfReader(inputStream);
            using var outputStream = new MemoryStream();
            using var writer = new PdfWriter(outputStream);
            using var pdfDoc = new PdfDocument(reader, writer);

            // local fonts for footer stage
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
    }
}