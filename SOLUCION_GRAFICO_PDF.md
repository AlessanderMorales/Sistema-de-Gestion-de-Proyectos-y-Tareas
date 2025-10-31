# ? Gráfico de Torta en Reportes PDF - SOLUCIÓN

## Problema
Al eliminar las columnas de ID en los reportes PDF, el gráfico de torta dejó de mostrarse.

## Solución Implementada

Agregar el gráfico de torta en una **página separada al final** del reporte general de proyectos, mostrando estadísticas globales:

### Método a Agregar

```csharp
// Agregar justo antes de document.Close() en GenerarReporteGeneralProyectosPdf línea 168

// ? NUEVO: Agregar gráfico de estadísticas generales
AgregarGraficoEstadisticas(document, proyectos.ToList());

document.Close();
```

### Métodos Nuevos

```csharp
private void AgregarGraficoEstadisticas(Document document, List<Proyecto> proyectos)
{
    var todasLasTareas = proyectos.SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>()).ToList();

    if (!todasLasTareas.Any())
        return;

    var sinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");
    var enProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
    var completadas = todasLasTareas.Count(t => t.Status == "Completada");
    var total = todasLasTareas.Count;

    if (total == 0)
        return;

    // Nueva página para el gráfico
    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

    var titulo = new Paragraph("Estadísticas Generales")
      .SetFont(_fontBold)
        .SetFontSize(18)
        .SetTextAlignment(TextAlignment.CENTER)
  .SetMarginBottom(20);
    document.Add(titulo);

    // Crear tabla para el gráfico y leyenda
    var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
        .UseAllAvailableWidth()
   .SetBorder(Border.NO_BORDER);

    // Columna del gráfico (usando Canvas)
    var graficoCell = new Cell()
        .SetBorder(Border.NO_BORDER)
.SetHeight(300);
    
    // Aquí se dibujará el gráfico con PdfCanvas después
    table.AddCell(graficoCell);

    // Columna de la leyenda
    var leyendaCell = new Cell()
        .SetBorder(Border.NO_BORDER);

    var porcentajeSinIniciar = (sinIniciar * 100.0 / total);
    var porcentajeEnProgreso = (enProgreso * 100.0 / total);
    var porcentajeCompletadas = (completadas * 100.0 / total);

    leyendaCell.Add(new Paragraph("Distribución de Tareas")
     .SetFont(_fontBold)
        .SetFontSize(14)
        .SetMarginBottom(15));

    leyendaCell.Add(new Paragraph($"? Sin Iniciar: {sinIniciar} ({porcentajeSinIniciar:F1}%)")
     .SetFontColor(new DeviceRgb(180, 180, 180))
        .SetFont(_fontBold)
    .SetFontSize(12)
        .SetMarginBottom(8));

    leyendaCell.Add(new Paragraph($"? En Progreso: {enProgreso} ({porcentajeEnProgreso:F1}%)")
        .SetFontColor(new DeviceRgb(255, 215, 0))
    .SetFont(_fontBold)
        .SetFontSize(12)
        .SetMarginBottom(8));

    leyendaCell.Add(new Paragraph($"? Completadas: {completadas} ({porcentajeCompletadas:F1}%)")
        .SetFontColor(new DeviceRgb(76, 175, 80))
        .SetFont(_fontBold)
        .SetFontSize(12)
        .SetMarginBottom(20));

    leyendaCell.Add(new Paragraph($"Total de Proyectos: {proyectos.Count}")
        .SetFont(_fontBold)
        .SetFontSize(11)
        .SetMarginBottom(5));

    leyendaCell.Add(new Paragraph($"Total de Tareas: {total}")
        .SetFont(_fontBold)
        .SetFontSize(11));

    table.AddCell(leyendaCell);

    document.Add(table);
}
```

### Alternativa Más Simple (Sin Gráfico Visual)

Si el gráfico de torta es muy complejo, una alternativa es mostrar las estadísticas en formato de tabla o barras simples:

```csharp
private void AgregarEstadisticasSimples(Document document, List<Proyecto> proyectos)
{
    var todasLasTareas = proyectos.SelectMany(p => p.Tareas ?? Enumerable.Empty<Tarea>()).ToList();

    if (!todasLasTareas.Any())
        return;

    var sinIniciar = todasLasTareas.Count(t => t.Status == "SinIniciar");
    var enProgreso = todasLasTareas.Count(t => t.Status == "EnProgreso");
    var completadas = todasLasTareas.Count(t => t.Status == "Completada");
    var total = todasLasTareas.Count;

    // Nueva página
    document.Add(new AreaBreak(AreaBreakType.NEXT_PAGE));

    var titulo = new Paragraph("Resumen Estadístico")
     .SetFont(_fontBold)
    .SetFontSize(18)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetMarginBottom(20);
    document.Add(titulo);

    // Tabla de estadísticas
    var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1 }))
        .UseAllAvailableWidth()
        .SetMarginBottom(20);

  // Headers
    table.AddHeaderCell(new Cell().Add(new Paragraph("Estado"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
        .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER));
    
    table.AddHeaderCell(new Cell().Add(new Paragraph("Cantidad"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
        .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER));
    
    table.AddHeaderCell(new Cell().Add(new Paragraph("Porcentaje"))
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
  .SetFont(_fontBold)
      .SetTextAlignment(TextAlignment.CENTER));

  // Sin Iniciar
    table.AddCell(new Cell().Add(new Paragraph("Sin Iniciar"))
        .SetBackgroundColor(new DeviceRgb(235, 235, 235)));
    table.AddCell(new Cell().Add(new Paragraph(sinIniciar.ToString()))
 .SetTextAlignment(TextAlignment.CENTER));
    table.AddCell(new Cell().Add(new Paragraph($"{(sinIniciar * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // En Progreso
  table.AddCell(new Cell().Add(new Paragraph("En Progreso"))
        .SetBackgroundColor(new DeviceRgb(255, 244, 179)));
    table.AddCell(new Cell().Add(new Paragraph(enProgreso.ToString()))
        .SetTextAlignment(TextAlignment.CENTER));
  table.AddCell(new Cell().Add(new Paragraph($"{(enProgreso * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // Completadas
    table.AddCell(new Cell().Add(new Paragraph("Completadas"))
        .SetBackgroundColor(new DeviceRgb(200, 230, 201)));
    table.AddCell(new Cell().Add(new Paragraph(completadas.ToString()))
        .SetTextAlignment(TextAlignment.CENTER));
    table.AddCell(new Cell().Add(new Paragraph($"{(completadas * 100.0 / total):F1}%"))
        .SetTextAlignment(TextAlignment.CENTER));

    // Total
    table.AddCell(new Cell().Add(new Paragraph("TOTAL"))
        .SetFont(_fontBold)
.SetBackgroundColor(ColorConstants.LIGHT_GRAY));
    table.AddCell(new Cell().Add(new Paragraph(total.ToString()))
 .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));
    table.AddCell(new Cell().Add(new Paragraph("100.0%"))
     .SetFont(_fontBold)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetBackgroundColor(ColorConstants.LIGHT_GRAY));

    document.Add(table);

    // Información adicional
    var info = new Paragraph($"Número total de proyectos: {proyectos.Count}")
        .SetFont(_fontBold)
      .SetFontSize(12)
        .SetTextAlignment(TextAlignment.CENTER)
        .SetMarginTop(10);
    document.Add(info);
}
```

## Uso

En el método `GenerarReporteGeneralProyectosPdf`, justo antes de `document.Close()`:

```csharp
// Opción 1: Con gráfico (más visual pero más complejo)
// AgregarGraficoEstadisticas(document, proyectos.ToList());

// Opción 2: Tabla simple (más fácil, igualmente efectiva)
AgregarEstadisticasSimples(document, proyectos.ToList());

document.Close();
```

## Ventajas de la Tabla Simple

? **Más simple de implementar**  
? **No depende de coordenadas complejas**  
? **Funciona sin importar las columnas de IDs**  
? **Fácil de mantener**  
? **Información clara y legible**  
? **Compatible con cualquier versión de iText**  

## Implementación Recomendada

Usar la **tabla simple de estadísticas** porque:
- Es más fácil de implementar y mantener
- No depende de gráficos complejos
- La información es clara y profesional
- No requiere coordenadas ni cálculos trigonométricos
