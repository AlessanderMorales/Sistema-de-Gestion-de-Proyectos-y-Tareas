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
using System;
using iText.Kernel.Geom;

namespace ServiceProyecto.Application.Service.Reportes
{
  public class PdfReporteBuilder
    {
        private readonly ProyectoService _proyectoService;
        private readonly TareaService _tareaService;
        private readonly UsuarioService _usuarioService;

        private readonly PdfFont _fontRegular;
        private readonly PdfFont _fontBold;

        public PdfReporteBuilder(ProyectoService proyectoService, TareaService tareaService, UsuarioService usuarioService)
        {
            _proyectoService = proyectoService;
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

            // ? NUEVO: Agregar gráficos de torta al inicio
       AgregarGraficosEstadisticas(document, proyectos);

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

            document.Close();
            var generated = stream.ToArray();
            return AddFootersToPdfBytes(generated, usuarioNombre);
        }

        /// <summary>
     /// ? NUEVO: Agregar gráficos estadísticos de torta
   /// </summary>
        private void AgregarGraficosEstadisticas(Document document, IEnumerable<Proyecto> proyectos)
        {
   // Obtener todas las tareas
         var todasLasTareas = proyectos
    .SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>())
       .ToList();

          if (!todasLasTareas.Any())
   {
  document.Add(new Paragraph("No hay tareas para generar estadísticas.")
    .SetFont(_fontRegular)
         .SetFontSize(10)
   .SetFontColor(ColorConstants.GRAY)
    .SetTextAlignment(TextAlignment.CENTER)
       .SetMarginBottom(20));
        return;
  }

 // Calcular estadísticas
var tareasCompletadas = todasLasTareas.Count(t => t.Status == "Completada");
var tareasEnProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
       var tareasSinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");

    var tareasAlta = todasLasTareas.Count(t => t.Prioridad == "Alta");
     var tareasMedia = todasLasTareas.Count(t => t.Prioridad == "Media");
        var tareasBaja = todasLasTareas.Count(t => t.Prioridad == "Baja");

  // Título de estadísticas
    document.Add(new Paragraph("Estadísticas Generales")
     .SetFont(_fontBold)
    .SetFontSize(14)
     .SetFontColor(ColorConstants.DARK_GRAY)
     .SetMarginTop(10)
     .SetMarginBottom(15));

         // Crear tabla para los dos gráficos lado a lado
     var tablaGraficos = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
    .UseAllAvailableWidth()
       .SetBorder(Border.NO_BORDER);

    // Celda para gráfico de Estados
   var celdaEstados = new Cell()
    .SetBorder(Border.NO_BORDER)
     .SetPadding(10);
   celdaEstados.Add(CrearGraficoTortaEstados(tareasCompletadas, tareasEnProgreso, tareasSinIniciar));

   // Celda para gráfico de Prioridades
    var celdaPrioridades = new Cell()
     .SetBorder(Border.NO_BORDER)
     .SetPadding(10);
      celdaPrioridades.Add(CrearGraficoTortaPrioridades(tareasAlta, tareasMedia, tareasBaja));

 tablaGraficos.AddCell(celdaEstados);
   tablaGraficos.AddCell(celdaPrioridades);

      document.Add(tablaGraficos);
    document.Add(new Paragraph().SetMarginBottom(20)); // Espacio después de gráficos
}

        /// <summary>
   /// ? NUEVO: Crear representación visual del gráfico de torta de Estados
     /// </summary>
        private Table CrearGraficoTortaEstados(int completadas, int enProgreso, int sinIniciar)
   {
       int total = completadas + enProgreso + sinIniciar;
    if (total == 0) total = 1; // Evitar división por cero

 // Tabla contenedora
      var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
        .UseAllAvailableWidth()
           .SetBorder(Border.NO_BORDER);

  // Título del gráfico
  tabla.AddCell(new Cell()
         .Add(new Paragraph("Distribución por Estado")
      .SetFont(_fontBold)
  .SetFontSize(12)
     .SetTextAlignment(TextAlignment.CENTER))
.SetBorder(Border.NO_BORDER)
    .SetBackgroundColor(new DeviceRgb(240, 240, 240))
   .SetPadding(8));

   // Crear barras visuales proporcionales
   var tablaBarras = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
  .UseAllAvailableWidth()
 .SetBorder(Border.NO_BORDER);

    // Barra Completadas (Verde)
    if (completadas > 0)
  {
var porcentaje = (completadas * 100.0 / total);
     tablaBarras.AddCell(new Cell()
       .Add(new Paragraph($"? Completadas: {completadas} ({porcentaje:F1}%)")
 .SetFont(_fontRegular)
   .SetFontSize(10))
       .SetBackgroundColor(new DeviceRgb(76, 175, 80))
         .SetBorder(Border.NO_BORDER)
         .SetPadding(8)
  .SetWidth(UnitValue.CreatePercentValue(100)));
       }

 // Barra En Progreso (Amarillo)
      if (enProgreso > 0)
   {
   var porcentaje = (enProgreso * 100.0 / total);
       tablaBarras.AddCell(new Cell()
     .Add(new Paragraph($"? En Progreso: {enProgreso} ({porcentaje:F1}%)")
   .SetFont(_fontRegular)
     .SetFontSize(10))
    .SetBackgroundColor(new DeviceRgb(255, 193, 7))
     .SetBorder(Border.NO_BORDER)
   .SetPadding(8)
 .SetWidth(UnitValue.CreatePercentValue(100)));
  }

 // Barra Sin Iniciar (Gris)
  if (sinIniciar > 0)
{
     var porcentaje = (sinIniciar * 100.0 / total);
  tablaBarras.AddCell(new Cell()
.Add(new Paragraph($"? Sin Iniciar: {sinIniciar} ({porcentaje:F1}%)")
        .SetFont(_fontRegular)
   .SetFontSize(10))
  .SetBackgroundColor(new DeviceRgb(158, 158, 158))
      .SetBorder(Border.NO_BORDER)
   .SetPadding(8)
.SetWidth(UnitValue.CreatePercentValue(100)));
   }

      tabla.AddCell(new Cell()
      .Add(tablaBarras)
.SetBorder(Border.NO_BORDER)
   .SetPadding(5));

       return tabla;
        }

  /// <summary>
   /// ? NUEVO: Crear representación visual del gráfico de torta de Prioridades
  /// </summary>
   private Table CrearGraficoTortaPrioridades(int alta, int media, int baja)
     {
      int total = alta + media + baja;
  if (total == 0) total = 1;

        var tabla = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
   .UseAllAvailableWidth()
     .SetBorder(Border.NO_BORDER);

        tabla.AddCell(new Cell()
    .Add(new Paragraph("Distribución por Prioridad")
.SetFont(_fontBold)
      .SetFontSize(12)
   .SetTextAlignment(TextAlignment.CENTER))
        .SetBorder(Border.NO_BORDER)
        .SetBackgroundColor(new DeviceRgb(240, 240, 240))
     .SetPadding(8));

var tablaBarras = new Table(UnitValue.CreatePercentArray(new float[] { 1 }))
     .UseAllAvailableWidth()
      .SetBorder(Border.NO_BORDER);

  if (alta > 0)
    {
    var porcentaje = (alta * 100.0 / total);
  tablaBarras.AddCell(new Cell()
       .Add(new Paragraph($"?? Alta: {alta} ({porcentaje:F1}%)")
      .SetFont(_fontRegular)
.SetFontSize(10))
        .SetBackgroundColor(new DeviceRgb(211, 47, 47))
  .SetFontColor(ColorConstants.WHITE)
.SetBorder(Border.NO_BORDER)
       .SetPadding(8));
   }

       if (media > 0)
  {
    var porcentaje = (media * 100.0 / total);
 tablaBarras.AddCell(new Cell()
       .Add(new Paragraph($"?? Media: {media} ({porcentaje:F1}%)")
   .SetFont(_fontRegular)
        .SetFontSize(10))
        .SetBackgroundColor(new DeviceRgb(255, 152, 0))
       .SetBorder(Border.NO_BORDER)
        .SetPadding(8));
  }

    if (baja > 0)
     {
  var porcentaje = (baja * 100.0 / total);
        tablaBarras.AddCell(new Cell()
        .Add(new Paragraph($"?? Baja: {baja} ({porcentaje:F1}%)")
.SetFont(_fontRegular)
  .SetFontSize(10))
         .SetBackgroundColor(new DeviceRgb(76, 175, 80))
        .SetBorder(Border.NO_BORDER)
.SetPadding(8));
        }

     tabla.AddCell(new Cell()
         .Add(tablaBarras)
 .SetBorder(Border.NO_BORDER)
     .SetPadding(5));

     return tabla;
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

            if (s.Contains("sin")) return new DeviceRgb(235, 235, 235);
            if (s.Contains("en progreso") || s.Contains("enprogreso") || s.Contains("progreso")) return new DeviceRgb(255, 244, 179);
            if (s.Contains("complet") || s.Contains("finaliz") || s.Contains("done")) return new DeviceRgb(200, 230, 201);

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

            if (bgColor != null) cell.SetBackgroundColor(bgColor);

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
    }
}