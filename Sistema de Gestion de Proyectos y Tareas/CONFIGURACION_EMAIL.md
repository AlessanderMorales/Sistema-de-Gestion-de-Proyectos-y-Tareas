# Configuración del Sistema de Generación Automática de Contraseñas y Envío por Email

## Cambios Implementados

### 1. Generación Automática de Contraseñas
- **Algoritmo**: Las contraseñas se generan automáticamente usando las 3 primeras letras del primer nombre y las 2 primeras letras del apellido.
- **Formato**: Primera letra en mayúscula + resto en minúsculas + carácter especial "!"
- **Ejemplo**: 
  - Usuario: Juan Pérez ? Contraseña: `Juape!`
  - Usuario: María González ? Contraseña: `Margo!`
  
- **Prevención de Duplicados**: Si ya existe una contraseña igual, se agregan números incrementales:
  - Primera coincidencia: `Juape1!`
  - Segunda coincidencia: `Juape2!`
  - Y así sucesivamente...

### 2. Envío de Email con SMTP
Se implementó un servicio de email que envía automáticamente las credenciales al usuario cuando se crea su cuenta.

## Configuración Requerida

### Paso 1: Configurar SMTP en appsettings.json

Abre el archivo `appsettings.json` y actualiza la sección `SmtpSettings` con tus credenciales:

```json
{
  "SmtpSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Username": "tu-email@gmail.com",
    "Password": "tu-app-password",
    "FromEmail": "tu-email@gmail.com",
    "FromName": "Sistema de Gestión de Proyectos"
  }
}
```

### Paso 2: Configurar Gmail (si usas Gmail)

Para usar Gmail como servidor SMTP, necesitas:

1. **Habilitar la verificación en dos pasos** en tu cuenta de Gmail
2. **Generar una contraseña de aplicación**:
   - Ve a: https://myaccount.google.com/security
   - En "Verificación en dos pasos", busca "Contraseñas de aplicaciones"
   - Genera una nueva contraseña de aplicación
   - Usa esa contraseña en el campo `Password` del `appsettings.json`

### Paso 3: Otros Proveedores de Email

Si no usas Gmail, puedes configurar otros proveedores:

#### Outlook/Hotmail
```json
"SmtpSettings": {
  "Host": "smtp-mail.outlook.com",
  "Port": 587,
  "Username": "tu-email@outlook.com",
  "Password": "tu-contraseña",
  "FromEmail": "tu-email@outlook.com",
  "FromName": "Sistema de Gestión de Proyectos"
}
```

#### Office 365
```json
"SmtpSettings": {
  "Host": "smtp.office365.com",
  "Port": 587,
  "Username": "tu-email@tudominio.com",
  "Password": "tu-contraseña",
  "FromEmail": "tu-email@tudominio.com",
  "FromName": "Sistema de Gestión de Proyectos"
}
```

## Uso del Sistema

### Crear un Nuevo Usuario

1. El administrador accede a la página de **Crear Usuario**
2. **Ya NO es necesario ingresar una contraseña manualmente**
3. Completa los siguientes campos:
   - Primer Nombre
   - Segundo Nombre (opcional)
   - Apellidos
   - Email
   - Rol

4. Al hacer clic en "Crear Usuario":
   - Se genera automáticamente una contraseña segura
   - La contraseña se hashea y se guarda en la base de datos
   - Se envía un email al usuario con sus credenciales

### Formato del Email

El usuario recibirá un email con el siguiente contenido:

- **Asunto**: "Bienvenido - Credenciales de Acceso"
- **Contenido**:
  - Nombre completo del usuario
  - Email (usuario)
  - Contraseña temporal
  - Recomendación de seguridad

## Seguridad

### Contraseñas Hasheadas
Todas las contraseñas se hashean usando **PBKDF2** con:
- 100,000 iteraciones
- SHA-256
- Salt aleatorio de 16 bytes

### Requisitos de Contraseña
Las contraseñas generadas automáticamente cumplen con los siguientes requisitos:
- ? Entre 8 y 15 caracteres
- ? Al menos una letra mayúscula
- ? Al menos una letra minúscula
- ? Al menos un carácter especial (!)
- ? Se agregan números si hay duplicados

## Mensajes del Sistema

Después de crear un usuario, verás uno de estos mensajes en la página de índice:

### ? Éxito
```
Usuario creado exitosamente. Se ha enviado un correo a usuario@example.com con las credenciales de acceso.
```

### ?? Advertencia
```
Usuario creado, pero hubo un error al enviar el correo. Contraseña generada: Juape!
```

En caso de error al enviar el email, la contraseña se mostrará en el mensaje para que puedas compartirla manualmente con el usuario.

## Solución de Problemas

### El email no se envía

1. **Verifica las credenciales SMTP** en `appsettings.json`
2. **Gmail**: Asegúrate de usar una contraseña de aplicación, no tu contraseña normal
3. **Firewall**: Verifica que el puerto 587 esté abierto
4. **Revisa los logs** en la consola para ver el mensaje de error específico

### La contraseña generada no cumple los requisitos

El sistema agrega automáticamente un "!" al final para cumplir con el requisito de carácter especial.

### Usuarios con nombres muy cortos

Si el nombre tiene menos de 3 letras o el apellido menos de 2, se rellenan con 'x':
- Usuario: Li Wu ? Contraseña: `Lixwu!`

## Archivos Modificados

1. ? `Application/Services/EmailService.cs` (Nuevo)
2. ? `Application/Services/UsuarioService.cs`
3. ? `Pages/Usuarios/Create.cshtml.cs`
4. ? `Pages/Usuarios/Create.cshtml`
5. ? `Pages/Usuarios/Index.cshtml`
6. ? `Domain/Entities/Usuario.cs`
7. ? `Program.cs`
8. ? `appsettings.json`

## Próximos Pasos Recomendados

1. **Implementar funcionalidad de "Cambiar Contraseña"** para que los usuarios puedan cambiar su contraseña temporal
2. **Agregar opción de "Olvidé mi contraseña"** que envíe un enlace de recuperación
3. **Registrar los intentos de envío de email** en una base de datos para auditoría
4. **Agregar plantillas de email personalizables** según el tipo de notificación

---

**Nota Importante**: Recuerda nunca subir el archivo `appsettings.json` con credenciales reales a un repositorio público. Usa variables de entorno o Azure Key Vault para producción.
