using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using ServiceUsuario.Application.Service;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Middleware
{
    public class ValidateUserExistsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidateUserExistsMiddleware> _logger;

        public ValidateUserExistsMiddleware(RequestDelegate next, ILogger<ValidateUserExistsMiddleware> logger)
   {
            _next = next;
   _logger = logger;
        }

     public async Task InvokeAsync(HttpContext context, UsuarioService usuarioService)
        {
    if (context.User?.Identity?.IsAuthenticated == true)
            {
    var path = context.Request.Path.Value?.ToLower() ?? "";
          if (path.StartsWith("/login") || 
  path.StartsWith("/logout") || 
           path.StartsWith("/accessdenied") ||
         path.StartsWith("/error") ||
                path.StartsWith("/privacy") ||
    path.Contains(".css") ||
    path.Contains(".js") ||
 path.Contains(".png") ||
        path.Contains(".jpg") ||
      path.Contains("/lib/"))
   {
        await _next(context);
   return;
      }
    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
            if (int.TryParse(userIdClaim, out var userId))
      {
        try
          {
        var usuario = usuarioService.ObtenerUsuarioPorId(userId);

       if (usuario == null)
        {
               _logger.LogWarning($"Usuario con ID {userId} ya no existe en la base de datos. Cerrando sesión automáticamente.");

    await context.SignOutAsync("MyCookieAuth");

        context.Response.Cookies.Append("SessionExpiredMessage", 
"Tu cuenta de usuario ya no existe. Por favor, contacta al administrador.",
    new CookieOptions 
           { 
                 HttpOnly = false, 
       Secure = true,
       SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(1)
   });

   context.Response.Redirect("/Login/Login");
     return;
          }
   }
         catch (Exception ex)
   {
   _logger.LogError(ex, $"Error al validar existencia del usuario {userId}");
              }
                }
            }
         await _next(context);
    }
    }

    public static class ValidateUserExistsMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateUserExists(this IApplicationBuilder builder)
        {
    return builder.UseMiddleware<ValidateUserExistsMiddleware>();
        }
    }
}
