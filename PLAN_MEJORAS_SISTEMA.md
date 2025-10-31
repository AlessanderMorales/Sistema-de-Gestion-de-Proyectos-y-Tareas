# ?? Plan de Mejoras del Sistema

## Mejoras a Implementar

### ? 1. Buscador con Autocompletado
**Estado: EN PROGRESO**

- [x] Proyectos: Buscador implementado
- [ ] Tareas: Pendiente
- [ ] Comentarios: Pendiente

**Funcionalidad:**
- B�squeda en tiempo real
- Filtra por: nombre, descripci�n, fechas
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

**Soluci�n:**
- Verificar encoding UTF-8

---

### 4. Asignaci�n Autom�tica de Empleados a Proyectos
**Estado: PENDIENTE**

Cuando un Jefe asigna una tarea a un empleado que NO est� en el proyecto:
1. Asignar tarea al empleado
2. Autom�ticamente asignar empleado al proyecto

---

### 5. Mejorar Visualizaci�n de Comentarios
**Estado: PENDIENTE**

- Si env�o ? Mostrar "Dirigido a"
- Si recibo ? Mostrar "Autor"

---

### 6. Corregir Caracteres Especiales
**Estado: PENDIENTE**

Problema: `? �xito: ...` y `?? Cambio...`

Soluci�n: Corregir encoding UTF-8

---

## Siguiente Paso

Implementar buscador en Tareas y continuar con las dem�s mejoras.
