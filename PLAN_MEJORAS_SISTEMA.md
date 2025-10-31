# ?? Plan de Mejoras del Sistema

## Mejoras a Implementar

### ? 1. Buscador con Autocompletado
**Estado: EN PROGRESO**

- [x] Proyectos: Buscador implementado
- [ ] Tareas: Pendiente
- [ ] Comentarios: Pendiente

**Funcionalidad:**
- Búsqueda en tiempo real
- Filtra por: nombre, descripción, fechas
- Mensaje cuando no hay resultados

---

### 2. Ocultar IDs en Reportes PDF/Excel
**Estado: PENDIENTE**

**Cambios:**
- Eliminar columna "ID Proyecto"
- Eliminar columna "ID Tarea"

---

### 3. Corregir Acentos en Descripciones
**Estado: PENDIENTE**

**Solución:**
- Verificar encoding UTF-8

---

### 4. Asignación Automática de Empleados a Proyectos
**Estado: PENDIENTE**

Cuando un Jefe asigna una tarea a un empleado que NO está en el proyecto:
1. Asignar tarea al empleado
2. Automáticamente asignar empleado al proyecto

---

### 5. Mejorar Visualización de Comentarios
**Estado: PENDIENTE**

- Si envío ? Mostrar "Dirigido a"
- Si recibo ? Mostrar "Autor"

---

### 6. Corregir Caracteres Especiales
**Estado: PENDIENTE**

Problema: `? Éxito: ...` y `?? Cambio...`

Solución: Corregir encoding UTF-8

---

## Siguiente Paso

Implementar buscador en Tareas y continuar con las demás mejoras.
