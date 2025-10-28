# ?? Validaci�n ESTRICTA de Caracteres en Nombres y Apellidos

## ? Pol�tica de Validaci�n

**SOLO se permiten caracteres espec�ficos para nombres propios:**
- Letras (A-Z, a-z) con acentos (�, �, �, �, �, �, etc.)
- Espacios simples entre palabras
- Guiones (`-`) para nombres compuestos (Jean-Pierre, Mar�a-Jos�)
- Ap�strofes (`'`) para nombres internacionales (O'Connor, D'Angelo)

**TODO LO DEM�S EST� BLOQUEADO:**
- ? N�meros (0-9)
- ? Caracteres especiales ($%#&@!*()+=[]{}...etc.)

---

## ?? Caracteres Permitidos vs Bloqueados

### ? �NICAMENTE PERMITIDOS:
```
LETRAS:     A-Z, a-z
ACENTOS:    � � � � � � � � � � � �
ESPACIOS:   " " (un solo espacio entre palabras)
GUIONES:    "-" (para nombres compuestos)
AP�STROFES: "'" (para nombres internacionales)
```

### ? TOTALMENTE BLOQUEADOS:
```
N�MEROS:    0 1 2 3 4 5 6 7 8 9
ESPECIALES: $ % # & @ ! * ( ) + = [ ] { } ; : " < > , . ? / \ | ` ~ ^
```

---

## ?? Ejemplos Pr�cticos

### ? **PERMITIDOS (V�lidos):**

| Entrada | Raz�n |
|---------|-------|
| `Juan Carlos` | ? Solo letras y espacio |
| `Mar�a Jos�` | ? Solo letras con acentos y espacio |
| `Mar�a-Jos�` | ? Letras con guion (nombre compuesto) |
| `Jean-Pierre` | ? Letras con guion (nombre compuesto) |
| `O'Connor` | ? Letras con ap�strofe (nombre irland�s) |
| `D'Angelo` | ? Letras con ap�strofe (nombre italiano) |
| `Saint-Pierre` | ? Letras con guion (apellido compuesto) |
| `Anne-Marie O'Brien` | ? Combinaci�n v�lida con guion y ap�strofe |
| `Jos� Luis` | ? Solo letras con acentos |
| `Garc�a L�pez` | ? Solo letras con acentos |

### ? **BLOQUEADOS (Inv�lidos):**

| Entrada | Raz�n del Rechazo | Mensaje de Error |
|---------|-------------------|------------------|
| `54%$%` | Contiene n�meros (54) y caracteres especiales (%$) | "Los nombres no pueden contener n�meros..." |
| `Juan123` | Contiene n�meros (123) | "Los nombres no pueden contener n�meros..." |
| `Mar�a7` | Contiene n�mero (7) | "Los nombres no pueden contener n�meros..." |
| `Pedro2024` | Contiene n�meros (2024) | "Los nombres no pueden contener n�meros..." |
| `Juan$` | Contiene car�cter especial ($) | "Los nombres no pueden contener caracteres especiales..." |
| `Mar�a%` | Contiene car�cter especial (%) | "Los nombres no pueden contener caracteres especiales..." |
| `Ana@Gmail` | Contiene car�cter especial (@) | "Los nombres no pueden contener caracteres especiales..." |
| `Pedro#Test` | Contiene car�cter especial (#) | "Los nombres no pueden contener caracteres especiales..." |
| `Jos�!` | Contiene car�cter especial (!) | "Los nombres no pueden contener caracteres especiales..." |
| `Carlos.Jos�` | Contiene punto (.) | "Los nombres no pueden contener caracteres especiales..." |
| `Ana&Luis` | Contiene ampersand (&) | "Los nombres no pueden contener caracteres especiales..." |
| `Juan+Carlos` | Contiene s�mbolo m�s (+) | "Los nombres no pueden contener caracteres especiales..." |

---

## ??? Capas de Validaci�n (Orden de Ejecuci�n)

### 1?? **Validaci�n de N�meros** (Primera capa)
```csharp
string contieneNumerosPattern = @"\d";

if (Regex.IsMatch(Nombres, contieneNumerosPattern))
{
    yield return new ValidationResult(
        "Los nombres no pueden contener n�meros. Solo se permiten letras, espacios, guiones y ap�strofes.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `54%$%`, `Juan123`, `Mar�a7`, `Pedro2024`, etc.

---

### 2?? **Validaci�n de Caracteres Especiales** (Segunda capa)
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

**Bloquea:** `Juan$`, `Mar�a%`, `Pedro#`, `Ana@`, `Jos�!`, etc.

---

### 3?? **Validaci�n de Formato Estricto** (Tercera capa)
```csharp
// ? SOLO permite: letras (con acentos), espacios, guiones y ap�strofes
string nombrePattern = @"^(?! )[A-Za-z������������'-]+(?: [A-Za-z������������'-]+)*$";

if (!Regex.IsMatch(Nombres.Trim(), nombrePattern))
{
    yield return new ValidationResult(
   "Los nombres solo pueden contener letras, espacios, guiones y ap�strofes.",
      new[] { nameof(Nombres) });
}
```

**Garantiza que SOLO** se acepten los caracteres permitidos.

---

### 4?? **Validaci�n de Espacios M�ltiples**
```csharp
string sinEspaciosMultiples = @" {2,}";

if (Regex.IsMatch(Nombres, sinEspaciosMultiples))
{
    yield return new ValidationResult(
        "Los nombres no pueden tener m�ltiples espacios consecutivos.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `Juan  Carlos` (dos espacios), `Mar�a   Jos�` (tres espacios)

---

### 5?? **Validaci�n de Espacios al Inicio/Final**
```csharp
if (Nombres != Nombres.Trim())
{
    yield return new ValidationResult(
        "Los nombres no deben empezar ni terminar con espacios.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** ` Juan`, `Mar�a `, ` Carlos `

---

### 6?? **Validaci�n contra SQL Injection y XSS** (�ltima capa)
```csharp
if (ContainsInjection(Nombres))
{
    yield return new ValidationResult(
   "No se permiten intentos expl�citos de inyecci�n SQL o contenido HTML/JS peligroso.",
        new[] { nameof(Nombres) });
}
```

**Bloquea:** `<script>`, `'; DROP TABLE--`, `UNION SELECT`, etc.

---

## ?? Pruebas de Validaci�n

### Caso de Prueba 1: Entrada `54%$%`

```
PASO 1: �Contiene n�meros?
  ? S� (54) ?
  
ERROR: "Los nombres no pueden contener n�meros. Solo se permiten letras, espacios, guiones y ap�strofes."

RESULTADO: ? RECHAZADO
```

### Caso de Prueba 2: Entrada `Juan$Carlos`

```
PASO 1: �Contiene n�meros?
  ? NO ?
  
PASO 2: �Contiene caracteres especiales?
  ? S� ($) ?
  
ERROR: "Los nombres no pueden contener caracteres especiales como $, %, #, &, @, etc."

RESULTADO: ? RECHAZADO
```

### Caso de Prueba 3: Entrada `Jean-Pierre`

```
PASO 1: �Contiene n�meros?
  ? NO ?
  
PASO 2: �Contiene caracteres especiales prohibidos?
  ? NO (el guion - est� permitido) ?
  
PASO 3: �Cumple el patr�n?
  ? S� (solo letras y guion permitido) ?
  
PASO 4: �Espacios m�ltiples?
  ? NO ?
  
PASO 5: �Espacios al inicio/final?
  ? NO ?
  
PASO 6: �SQL Injection/XSS?
  ? NO ?

RESULTADO: ? ACEPTADO
```

### Caso de Prueba 4: Entrada `O'Connor`

```
PASO 1: �Contiene n�meros?
  ? NO ?
  
PASO 2: �Contiene caracteres especiales prohibidos?
  ? NO (el ap�strofe ' est� permitido) ?
  
PASO 3: �Cumple el patr�n?
  ? S� (solo letras y ap�strofe permitido) ?
  
PASO 4-6: Otras validaciones
  ? PASAN ?

RESULTADO: ? ACEPTADO
```

---

## ?? Resumen de Reglas

| Tipo de Car�cter | �Permitido? | Ejemplos |
|------------------|-------------|----------|
| **Letras (A-Z, a-z)** | ? S� | Juan, Mar�a, Jos� |
| **Letras con acentos** | ? S� | �, �, �, �, �, � |
| **Espacios simples** | ? S� | "Juan Carlos" |
| **Guiones (-)** | ? S� | "Jean-Pierre", "Mar�a-Jos�" |
| **Ap�strofes (')** | ? S� | "O'Connor", "D'Angelo" |
| **N�meros (0-9)** | ? NO | 123, 456, 789 |
| **S�mbolos ($%#&@!)** | ? NO | $, %, #, &, @, ! |
| **Puntuaci�n (.,;:)** | ? NO | . , ; : |
| **Par�ntesis (())** | ? NO | ( ) |
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
Fran�ois         ?
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

### ? **Espa�oles:**
```
Mar�a-Jos�       ?
Juan-Carlos      ?
Jos�-Luis        ?
Garc�a   ?
```

---

## ?? Impacto en la Aplicaci�n

### D�nde se Aplican estas Validaciones:

1. **? Crear Usuario** (`/Usuarios/Create`)
   - Cualquier entrada con n�meros o caracteres especiales ser� rechazada

2. **? Editar Usuario** (`/Usuarios/Edit`)
   - Las mismas reglas se aplican al editar

3. **? Validaci�n del Modelo**
   - Se ejecuta autom�ticamente en el servidor con `ModelState.IsValid`

4. **? API/Servicios**
   - `UsuarioService.CrearNuevoUsuario()`
   - `UsuarioService.ActualizarUsuario()`

---

## ?? Mensajes de Error Claros

| Problema Detectado | Mensaje al Usuario |
|--------------------|-------------------|
| Contiene n�meros | "Los nombres no pueden contener n�meros. Solo se permiten letras, espacios, guiones y ap�strofes." |
| Contiene s�mbolos | "Los nombres no pueden contener caracteres especiales como $, %, #, &, @, etc. Solo se permiten letras, espacios, guiones y ap�strofes." |
| Formato incorrecto | "Los nombres solo pueden contener letras, espacios, guiones y ap�strofes." |
| Espacios m�ltiples | "Los nombres no pueden tener m�ltiples espacios consecutivos." |
| SQL Injection | "No se permiten intentos expl�citos de inyecci�n SQL o contenido HTML/JS peligroso." |

---

## ? Estado Final

**Pol�tica Implementada:** ? **SOLO letras, guiones y ap�strofes permitidos**

**Validaci�n Actual:** ? **CORRECTA Y COMPLETA**

**Archivos Afectados:**
- `ServiceUsuario/Domain/Entities/Usuario.cs` ? Validado

**Seguridad:** ?? **M�XIMA**
- N�meros bloqueados
- Caracteres especiales bloqueados
- SQL Injection bloqueado
- XSS bloqueado

**Fecha:** 2025  
**Estado:** ? Implementado y Funcionando
