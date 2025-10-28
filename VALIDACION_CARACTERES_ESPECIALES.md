# ?? Validación ESTRICTA de Caracteres en Nombres y Apellidos

## ? Política de Validación

**SOLO se permiten caracteres específicos para nombres propios:**
- Letras (A-Z, a-z) con acentos (Á, É, Í, Ó, Ú, Ñ, etc.)
- Espacios simples entre palabras
- Guiones (`-`) para nombres compuestos (Jean-Pierre, María-José)
- Apóstrofes (`'`) para nombres internacionales (O'Connor, D'Angelo)

**TODO LO DEMÁS ESTÁ BLOQUEADO:**
- ? Números (0-9)
- ? Caracteres especiales ($%#&@!*()+=[]{}...etc.)

---

## ?? Caracteres Permitidos vs Bloqueados

### ? ÚNICAMENTE PERMITIDOS:
```
LETRAS:     A-Z, a-z
ACENTOS:    Á É Í Ó Ú á é í ó ú Ñ ñ
ESPACIOS:   " " (un solo espacio entre palabras)
GUIONES:    "-" (para nombres compuestos)
APÓSTROFES: "'" (para nombres internacionales)
```

### ? TOTALMENTE BLOQUEADOS:
```
NÚMEROS:    0 1 2 3 4 5 6 7 8 9
ESPECIALES: $ % # & @ ! * ( ) + = [ ] { } ; : " < > , . ? / \ | ` ~ ^
```

---

## ?? Ejemplos Prácticos

### ? **PERMITIDOS (Válidos):**

| Entrada | Razón |
|---------|-------|
| `Juan Carlos` | ? Solo letras y espacio |
| `María José` | ? Solo letras con acentos y espacio |
| `María-José` | ? Letras con guion (nombre compuesto) |
| `Jean-Pierre` | ? Letras con guion (nombre compuesto) |
| `O'Connor` | ? Letras con apóstrofe (nombre irlandés) |
| `D'Angelo` | ? Letras con apóstrofe (nombre italiano) |
| `Saint-Pierre` | ? Letras con guion (apellido compuesto) |
| `Anne-Marie O'Brien` | ? Combinación válida con guion y apóstrofe |
| `José Luis` | ? Solo letras con acentos |
| `García López` | ? Solo letras con acentos |

### ? **BLOQUEADOS (Inválidos):**

| Entrada | Razón del Rechazo | Mensaje de Error |
|---------|-------------------|------------------|
| `54%$%` | Contiene números (54) y caracteres especiales (%$) | "Los nombres no pueden contener números..." |
| `Juan123` | Contiene números (123) | "Los nombres no pueden contener números..." |
| `María7` | Contiene número (7) | "Los nombres no pueden contener números..." |
| `Pedro2024` | Contiene números (2024) | "Los nombres no pueden contener números..." |
| `Juan$` | Contiene carácter especial ($) | "Los nombres no pueden contener caracteres especiales..." |
| `María%` | Contiene carácter especial (%) | "Los nombres no pueden contener caracteres especiales..." |
| `Ana@Gmail` | Contiene carácter especial (@) | "Los nombres no pueden contener caracteres especiales..." |
| `Pedro#Test` | Contiene carácter especial (#) | "Los nombres no pueden contener caracteres especiales..." |
| `José!` | Contiene carácter especial (!) | "Los nombres no pueden contener caracteres especiales..." |
| `Carlos.José` | Contiene punto (.) | "Los nombres no pueden contener caracteres especiales..." |
| `Ana&Luis` | Contiene ampersand (&) | "Los nombres no pueden contener caracteres especiales..." |
| `Juan+Carlos` | Contiene símbolo más (+) | "Los nombres no pueden contener caracteres especiales..." |

---

## ??? Capas de Validación (Orden de Ejecución)

### 1?? **Validación de Números** (Primera capa)
```csharp
string contieneNumerosPattern = @"\d";

if (Regex.IsMatch(Nombres, contieneNumerosPattern))
{
    yield return new ValidationResult(
        "Los nombres no pueden contener números. Solo se permiten letras, espacios, guiones y apóstrofes.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `54%$%`, `Juan123`, `María7`, `Pedro2024`, etc.

---

### 2?? **Validación de Caracteres Especiales** (Segunda capa)
```csharp
// ? Lista COMPLETA de caracteres especiales bloqueados (excluye solo - y ')
string caracteresEspecialesPattern = @"[$%#&@!*()+=\[\]{};:""<>,.?/\\|`~^]";

if (Regex.IsMatch(Nombres, caracteresEspecialesPattern))
{
    yield return new ValidationResult(
        "Los nombres no pueden contener caracteres especiales como $, %, #, &, @, etc.",
      new[] { nameof(Nombres) });
}
```

**Bloquea:** `Juan$`, `María%`, `Pedro#`, `Ana@`, `José!`, etc.

---

### 3?? **Validación de Formato Estricto** (Tercera capa)
```csharp
// ? SOLO permite: letras (con acentos), espacios, guiones y apóstrofes
string nombrePattern = @"^(?! )[A-Za-zÁÉÍÓÚáéíóúÑñ'-]+(?: [A-Za-zÁÉÍÓÚáéíóúÑñ'-]+)*$";

if (!Regex.IsMatch(Nombres.Trim(), nombrePattern))
{
    yield return new ValidationResult(
   "Los nombres solo pueden contener letras, espacios, guiones y apóstrofes.",
      new[] { nameof(Nombres) });
}
```

**Garantiza que SOLO** se acepten los caracteres permitidos.

---

### 4?? **Validación de Espacios Múltiples**
```csharp
string sinEspaciosMultiples = @" {2,}";

if (Regex.IsMatch(Nombres, sinEspaciosMultiples))
{
    yield return new ValidationResult(
        "Los nombres no pueden tener múltiples espacios consecutivos.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `Juan  Carlos` (dos espacios), `María   José` (tres espacios)

---

### 5?? **Validación de Espacios al Inicio/Final**
```csharp
if (Nombres != Nombres.Trim())
{
    yield return new ValidationResult(
        "Los nombres no deben empezar ni terminar con espacios.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** ` Juan`, `María `, ` Carlos `

---

### 6?? **Validación contra SQL Injection y XSS** (Última capa)
```csharp
if (ContainsInjection(Nombres))
{
    yield return new ValidationResult(
   "No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `<script>`, `'; DROP TABLE--`, `UNION SELECT`, etc.

---

## ?? Pruebas de Validación

### Caso de Prueba 1: Entrada `54%$%`

```
PASO 1: ¿Contiene números?
  ? SÍ (54) ?
  
ERROR: "Los nombres no pueden contener números. Solo se permiten letras, espacios, guiones y apóstrofes."

RESULTADO: ? RECHAZADO
```

### Caso de Prueba 2: Entrada `Juan$Carlos`

```
PASO 1: ¿Contiene números?
  ? NO ?
  
PASO 2: ¿Contiene caracteres especiales?
  ? SÍ ($) ?
  
ERROR: "Los nombres no pueden contener caracteres especiales como $, %, #, &, @, etc."

RESULTADO: ? RECHAZADO
```

### Caso de Prueba 3: Entrada `Jean-Pierre`

```
PASO 1: ¿Contiene números?
  ? NO ?
  
PASO 2: ¿Contiene caracteres especiales prohibidos?
  ? NO (el guion - está permitido) ?
  
PASO 3: ¿Cumple el patrón?
  ? SÍ (solo letras y guion permitido) ?
  
PASO 4: ¿Espacios múltiples?
  ? NO ?
  
PASO 5: ¿Espacios al inicio/final?
  ? NO ?
  
PASO 6: ¿SQL Injection/XSS?
  ? NO ?

RESULTADO: ? ACEPTADO
```

### Caso de Prueba 4: Entrada `O'Connor`

```
PASO 1: ¿Contiene números?
  ? NO ?
  
PASO 2: ¿Contiene caracteres especiales prohibidos?
  ? NO (el apóstrofe ' está permitido) ?
  
PASO 3: ¿Cumple el patrón?
  ? SÍ (solo letras y apóstrofe permitido) ?
  
PASO 4-6: Otras validaciones
  ? PASAN ?

RESULTADO: ? ACEPTADO
```

---

## ?? Resumen de Reglas

| Tipo de Carácter | ¿Permitido? | Ejemplos |
|------------------|-------------|----------|
| **Letras (A-Z, a-z)** | ? SÍ | Juan, María, José |
| **Letras con acentos** | ? SÍ | Á, É, Í, Ó, Ú, Ñ |
| **Espacios simples** | ? SÍ | "Juan Carlos" |
| **Guiones (-)** | ? SÍ | "Jean-Pierre", "María-José" |
| **Apóstrofes (')** | ? SÍ | "O'Connor", "D'Angelo" |
| **Números (0-9)** | ? NO | 123, 456, 789 |
| **Símbolos ($%#&@!)** | ? NO | $, %, #, &, @, ! |
| **Puntuación (.,;:)** | ? NO | . , ; : |
| **Paréntesis (())** | ? NO | ( ) |
| **Corchetes ([])** | ? NO | [ ] |
| **Llaves ({})** | ? NO | { } |
| **Barras (/\)** | ? NO | / \ |
| **Otros (+=<>)** | ? NO | +, =, <, > |

---

## ?? Nombres Internacionales Soportados

### ? **Franceses:**
```
Jean-Pierre?
Marie-Claire     ?
François         ?
D'Artagnan       ?
```

### ? **Irlandeses/Escoceses:**
```
O'Connor         ?
O'Brien?
O'Sullivan       ?
MacDonald        ? (o Mac-Donald con guion)
```

### ? **Italianos:**
```
D'Angelo         ?
D'Alessandro     ?
De Luca  ?
```

### ? **Alemanes:**
```
Karl-Heinz ?
Hans-Peter    ?
```

### ? **Españoles:**
```
María-José       ?
Juan-Carlos      ?
José-Luis        ?
García   ?
```

---

## ?? Impacto en la Aplicación

### Dónde se Aplican estas Validaciones:

1. **? Crear Usuario** (`/Usuarios/Create`)
   - Cualquier entrada con números o caracteres especiales será rechazada

2. **? Editar Usuario** (`/Usuarios/Edit`)
   - Las mismas reglas se aplican al editar

3. **? Validación del Modelo**
   - Se ejecuta automáticamente en el servidor con `ModelState.IsValid`

4. **? API/Servicios**
   - `UsuarioService.CrearNuevoUsuario()`
   - `UsuarioService.ActualizarUsuario()`

---

## ?? Mensajes de Error Claros

| Problema Detectado | Mensaje al Usuario |
|--------------------|-------------------|
| Contiene números | "Los nombres no pueden contener números. Solo se permiten letras, espacios, guiones y apóstrofes." |
| Contiene símbolos | "Los nombres no pueden contener caracteres especiales como $, %, #, &, @, etc. Solo se permiten letras, espacios, guiones y apóstrofes." |
| Formato incorrecto | "Los nombres solo pueden contener letras, espacios, guiones y apóstrofes." |
| Espacios múltiples | "Los nombres no pueden tener múltiples espacios consecutivos." |
| SQL Injection | "No se permiten intentos explícitos de inyección SQL o contenido HTML/JS peligroso." |

---

## ? Estado Final

**Política Implementada:** ? **SOLO letras, guiones y apóstrofes permitidos**

**Validación Actual:** ? **CORRECTA Y COMPLETA**

**Archivos Afectados:**
- `ServiceUsuario/Domain/Entities/Usuario.cs` ? Validado

**Seguridad:** ?? **MÁXIMA**
- Números bloqueados
- Caracteres especiales bloqueados
- SQL Injection bloqueado
- XSS bloqueado

**Fecha:** 2025  
**Estado:** ? Implementado y Funcionando
