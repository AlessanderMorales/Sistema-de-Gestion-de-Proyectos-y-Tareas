# Configuraci�n del Sistema de Generaci�n Autom�tica de Contrase�as y Env�o por Email

## Cambios Implementados

### 1. Generaci�n Autom�tica de Contrase�as
- **Algoritmo**: Las contrase�as se generan autom�ticamente usando las 3 primeras letras del primer nombre y las 2 primeras letras del apellido.
- **Formato**: Primera letra en may�scula + resto en min�sculas + car�cter especial "!"
- **Ejemplo**: 
  - Usuario: Juan P�rez ? Contrase�a: `Juape!`
  - Usuario: Mar�a Gonz�lez ? Contrase�a: `Margo!`
  
- **Prevenci�n de Duplicados**: Si ya existe una contrase�a igual, se agregan n�meros incrementales:
  - Primera coincidencia: `Juape1!`
  - Segunda coincidencia: `Juape2!`
  - Y as� sucesivamente...

### 2. Env�o de Email con SMTP
Se implement� un servicio de email que env�a autom�ticamente las credenciales al usuario cuando se crea su cuenta.

## Configuraci�n Requerida

### Paso 1: Configurar SMTP en appsettings.json

Abre el archivo `appsettings.json` y actualiza la secci�n `SmtpSettings` con tus credenciales:

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Sistema de Gesti�n de Proyectos"
  }
}
```

### Paso 2: Configurar Gmail (si usas Gmail)

Para usar Gmail como servidor SMTP, necesitas:

1. **Habilitar la verificaci�n en dos pasos** en tu cuenta de Gmail
2. **Generar una contrase�a de aplicaci�n**:
   - Ve a: https://myaccount.google.com/security
   - En "Verificaci�n en dos pasos", busca "Contrase�as de aplicaciones"
   - Genera una nueva contrase�a de aplicaci�n
   - Usa esa contrase�a en el campo `Password` del `appsettings.json`

### Paso 3: Otros Proveedores de Email

Si no usas Gmail, puedes configurar otros proveedores:

#### Outlook/Hotmail
```json
"SmtpSettings": {
  "Host": "smtp-mail.outlook.com",
  "Port": 587,
  "Username": "tu-email@outlook.com",
  "Password": "tu-contrase�a",
  "FromEmail": "tu-email@outlook.com",
  "FromName": "Sistema de Gesti�n de Proyectos"
}
```

#### Office 365
```json
"SmtpSettings": {
  "Host": "smtp.office365.com",
  "Port": 587,
  "Username": "tu-email@tudominio.com",
  "Password": "tu-contrase�a",
  "FromEmail": "tu-email@tudominio.com",
  "FromName": "Sistema de Gesti�n de Proyectos"
}
```

## Uso del Sistema

### Crear un Nuevo Usuario

1. El administrador accede a la p�gina de **Crear Usuario**
2. **Ya NO es necesario ingresar una contrase�a manualmente**
3. Completa los siguientes campos:
   - Primer Nombre
   - Segundo Nombre (opcional)
   - Apellidos
   - Email
   - Rol

4. Al hacer clic en "Crear Usuario":
   - Se genera autom�ticamente una contrase�a segura
   - La contrase�a se hashea y se guarda en la base de datos
   - Se env�a un email al usuario con sus credenciales

### Formato del Email

El usuario recibir� un email con el siguiente contenido:

- **Asunto**: "Bienvenido - Credenciales de Acceso"
- **Contenido**:
  - Nombre completo del usuario
  - Email (usuario)
  - Contrase�a temporal
  - Recomendaci�n de seguridad

## Seguridad

### Contrase�as Hasheadas
Todas las contrase�as se hashean usando **PBKDF2** con:
- 100,000 iteraciones
- SHA-256
- Salt aleatorio de 16 bytes

### Requisitos de Contrase�a
Las contrase�as generadas autom�ticamente cumplen con los siguientes requisitos:
- ? Entre 8 y 15 caracteres
- ? Al menos una letra may�scula
- ? Al menos una letra min�scula
- ? Al menos un car�cter especial (!)
- ? Se agregan n�meros si hay duplicados

## Mensajes del Sistema

Despu�s de crear un usuario, ver�s uno de estos mensajes en la p�gina de �ndice:

### ? �xito
```
Usuario creado exitosamente. Se ha enviado un correo a usuario@example.com con las credenciales de acceso.
```

### ?? Advertencia
```
Usuario creado, pero hubo un error al enviar el correo. Contrase�a generada: Juape!
```

En caso de error al enviar el email, la contrase�a se mostrar� en el mensaje para que puedas compartirla manualmente con el usuario.

## Soluci�n de Problemas

### El email no se env�a

1. **Verifica las credenciales SMTP** en `appsettings.json`
2. **Gmail**: Aseg�rate de usar una contrase�a de aplicaci�n, no tu contrase�a normal
3. **Firewall**: Verifica que el puerto 587 est� abierto
4. **Revisa los logs** en la consola para ver el mensaje de error espec�fico

### La contrase�a generada no cumple los requisitos

El sistema agrega autom�ticamente un "!" al final para cumplir con el requisito de car�cter especial.

### Usuarios con nombres muy cortos

Si el nombre tiene menos de 3 letras o el apellido menos de 2, se rellenan con 'x':
- Usuario: Li Wu ? Contrase�a: `Lixwu!`

## Archivos Modificados

1. ? `Application/Services/EmailService.cs` (Nuevo)
2. ? `Application/Services/UsuarioService.cs`
3. ? `Pages/Usuarios/Create.cshtml.cs`
4. ? `Pages/Usuarios/Create.cshtml`
5. ? `Pages/Usuarios/Index.cshtml`
6. ? `Domain/Entities/Usuario.cs`
7. ? `Program.cs`
8. ? `appsettings.json`

## Pr�ximos Pasos Recomendados

1. **Implementar funcionalidad de "Cambiar Contrase�a"** para que los usuarios puedan cambiar su contrase�a temporal
2. **Agregar opci�n de "Olvid� mi contrase�a"** que env�e un enlace de recuperaci�n
3. **Registrar los intentos de env�o de email** en una base de datos para auditor�a
4. **Agregar plantillas de email personalizables** seg�n el tipo de notificaci�n

---

**Nota Importante**: Recuerda nunca subir el archivo `appsettings.json` con credenciales reales a un repositorio p�blico. Usa variables de entorno o Azure Key Vault para producci�n.
