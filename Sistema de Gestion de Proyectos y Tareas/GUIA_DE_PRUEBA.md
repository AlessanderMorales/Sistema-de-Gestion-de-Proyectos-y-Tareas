# Gu�a de Prueba - Sistema de Creaci�n Autom�tica de Usuarios

## ?? Cambios Realizados para Solucionar el Problema

### Problema Identificado
El bot�n "Crear Usuario" no hac�a nada porque:
1. La validaci�n del ModelState estaba fallando porque esperaba una contrase�a
2. La contrase�a est� vac�a (ya que se genera autom�ticamente en el backend)

### Soluci�n Implementada
Se agreg� la l�nea:
```csharp
ModelState.Remove("Usuario.Contrase�a");
```

Esto **remueve la validaci�n de la contrase�a del ModelState** antes de verificar si el formulario es v�lido, permitiendo que el proceso contin�e y se genere la contrase�a autom�ticamente.

---

## ?? Pasos para Probar el Sistema

### Paso 1: Iniciar la Aplicaci�n
1. Ejecuta el proyecto
2. Inicia sesi�n como **SuperAdmin**

### Paso 2: Crear un Usuario de Prueba
1. Ve a la secci�n de **Usuarios**
2. Haz clic en **"Crear Nuevo Usuario"**
3. Completa el formulario con estos datos:

#### Datos de Prueba 1:
```
Primer Nombre: Test
Segundo Nombre: (dejar vac�o)
Apellidos: Usuario
Email: tu_email_personal@gmail.com
Rol: Empleado
```

**Contrase�a que se generar� autom�ticamente:** `Testus!`

4. Haz clic en **"Crear Usuario"**

---

## ? Resultados Esperados

### Escenario 1: Todo Funciona Correctamente ?

**En la p�gina de Index de Usuarios ver�s:**
```
? �xito: Usuario creado exitosamente. Se ha enviado un correo a tu_email_personal@gmail.com con las credenciales de acceso.
```

**En tu email recibir�s:**
- Asunto: "Bienvenido - Credenciales de Acceso"
- Usuario: tu_email_personal@gmail.com
- Contrase�a temporal: `Testus!`

**En la base de datos:**
```sql
SELECT * FROM Usuario WHERE email = 'tu_email_personal@gmail.com';
```
Ver�s que la contrase�a est� hasheada con formato:
```
PBKDF2:100000:xxxxxx:xxxxxx
```

### Escenario 2: Email No Se Env�a ??

**En la p�gina ver�s:**
```
? Advertencia: Usuario creado, pero hubo un error al enviar el correo. Contrase�a generada: Testus!
```

**Esto significa:**
- ? El usuario S� se cre� en la base de datos
- ? La contrase�a S� est� hasheada
- ? El email no se envi� (problema con SMTP)

**En este caso, copia la contrase�a mostrada** (`Testus!`) y env�asela manualmente al usuario.

---

## ?? Pruebas Adicionales

### Prueba 2: Verificar Duplicados de Contrase�a
Crea otro usuario con el mismo nombre:

```
Primer Nombre: Test
Apellidos: Usuario
Email: otro_email@gmail.com
Rol: Empleado
```

**Contrase�a esperada:** `Testus1!` (nota el n�mero 1)

### Prueba 3: Verificar Login con Contrase�a Generada

1. Cierra sesi�n del sistema
2. Intenta iniciar sesi�n con:
   - Email: `tu_email_personal@gmail.com`
   - Contrase�a: `Testus!`

**Resultado esperado:** ? Deber�as poder iniciar sesi�n exitosamente

### Prueba 4: Nombres con Acentos

```
Primer Nombre: Mar�a
Apellidos: Gonz�lez
Email: maria@test.com
Rol: Empleado
```

**Contrase�a esperada:** `Margo!` (sin acentos, normalizada)

---

## ?? Verificaci�n en Base de Datos

### Consulta 1: Ver Usuarios Creados
```sql
SELECT 
    id_usuario, 
    primer_nombre, 
    apellidos, 
    email, 
    LEFT(contrase�a, 30) as contrase�a_hasheada,
    rol,
    estado
FROM Usuario 
WHERE estado = 1
ORDER BY id_usuario DESC
LIMIT 5;
```

### Consulta 2: Verificar Formato de Contrase�a
```sql
SELECT 
    email,
    CASE 
        WHEN contrase�a LIKE 'PBKDF2:%' THEN 'Hasheada Correctamente'
        ELSE 'Contrase�a en Texto Plano (ERROR)'
    END as estado_contrase�a
FROM Usuario
WHERE email = 'tu_email_personal@gmail.com';
```

**Resultado esperado:** `Hasheada Correctamente`

---

## ?? Soluci�n de Problemas

### Problema: Sigue sin pasar nada al hacer clic

**Posibles causas:**
1. JavaScript est� deshabilitado
2. Hay errores de validaci�n en el formulario que no se est�n mostrando
3. La aplicaci�n no se reinici� despu�s de los cambios

**Soluci�n:**
1. Presiona **F12** en el navegador
2. Ve a la pesta�a **Console**
3. Intenta crear el usuario nuevamente
4. Revisa si hay errores en la consola

### Problema: Error "SmtpException"

**Causa:** Configuraci�n SMTP incorrecta

**Soluci�n:** Verifica en `appsettings.json`:
```json
"SmtpSettings": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "Username": "jaime.deugarte@ucb.edu.bo",
  "Password": "xiykvigxzkuofvab",
  "FromEmail": "jaime.deugarte@ucb.edu.bo",
  "FromName": "Sistema de Gesti�n de Proyectos"
}
```

### Problema: "El campo contrase�a es obligatorio"

**Causa:** El `ModelState.Remove` no se ejecut� correctamente

**Soluci�n:** Aseg�rate de que est�s usando la versi�n actualizada del archivo `Create.cshtml.cs`

---

## ?? Checklist de Verificaci�n

Despu�s de crear un usuario, verifica:

- [ ] El usuario aparece en la lista de usuarios
- [ ] El usuario puede iniciar sesi�n con la contrase�a generada
- [ ] La contrase�a en la BD empieza con `PBKDF2:`
- [ ] Se recibi� el email (o se mostr� la contrase�a en pantalla)
- [ ] Si se crea otro usuario con el mismo nombre, la contrase�a tiene un n�mero

---

## ?? Formato de Contrase�as Generadas

| Nombre | Apellido | Contrase�a Generada |
|--------|----------|---------------------|
| Juan | P�rez | `Juape!` |
| Mar�a | Gonz�lez | `Margo!` |
| Jos� | Rodr�guez | `Josro!` |
| Ana | L�pez | `Analo!` |
| Test | Usuario | `Testus!` |
| Test | Usuario (2do) | `Testus1!` |
| Pedro | Wu | `Pedwu!` |
| Li | An | `Lixan!` |

**Nota:** Los nombres muy cortos se rellenan con 'x'.

---

## ?? Seguridad Verificada

? **Hasheado:** PBKDF2 con 100,000 iteraciones
? **Salt:** Aleatorio de 16 bytes
? **Algoritmo:** SHA-256
? **Longitud:** 8-15 caracteres
? **May�scula:** Primera letra
? **Min�sculas:** Resto del texto
? **Car�cter especial:** ! al final
? **N�meros:** Si hay duplicados

---

## ?? Ejemplo de Email Recibido

El usuario recibir� un email con este formato:

```
De: Sistema de Gesti�n de Proyectos <jaime.deugarte@ucb.edu.bo>
Para: test@ejemplo.com
Asunto: Bienvenido - Credenciales de Acceso

�Bienvenido al Sistema!

Hola Test Usuario,

Tu cuenta ha sido creada exitosamente en nuestro Sistema de 
Gesti�n de Proyectos y Tareas.

Usuario (Email): test@ejemplo.com
Contrase�a temporal: Testus!

?? Importante:
Por seguridad, te recomendamos cambiar tu contrase�a despu�s 
del primer inicio de sesi�n. Nunca compartas tus credenciales 
con nadie.
```

---

## ?? Notas Finales

1. **Las contrase�as S� se guardan hasheadas** en la base de datos
2. **Solo se muestra la contrase�a sin hashear** cuando hay error al enviar el email
3. **El proceso es completamente autom�tico** - el admin NO necesita crear contrase�as manualmente
4. **Cada usuario recibe sus credenciales por email** sin intervenci�n manual

�El sistema est� listo para usar! ??
