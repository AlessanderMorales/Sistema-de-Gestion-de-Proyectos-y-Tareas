using ServiceComentario.Application.Service;
using ServiceComentario.Domain.Entities;
using ServiceComentario.Infrastructure.Persistence.Factories;
using ServiceCommon.Application.Services;
using ServiceCommon.Domain.Common;
using ServiceCommon.Domain.Port;
using ServiceCommon.Infrastructure.Persistence.Data;
using ServiceProyecto.Application.Service;
using ServiceProyecto.Domain.Entities;
using ServiceProyecto.Infrastructure.Persistence.Factories;
using ServiceTarea.Application.Service;
using ServiceTarea.Domain.Entities;
using ServiceTarea.Infrastructure.Persistence.Factories;
using ServiceUsuario.Application.Service;
using ServiceUsuario.Domain.Entities;
using ServiceUsuario.Infrastructure.Persistence.Factories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "Sgpt.AuthCookie";
        options.LoginPath = "/Login/Login";
     options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
   options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmin", policy =>
        policy.RequireRole(Roles.SuperAdmin));

    options.AddPolicy("OnlyJefeOrEmpleado", policy =>
      policy.RequireAssertion(ctx => 
     ctx.User.IsInRole(Roles.JefeDeProyecto) || 
            ctx.User.IsInRole(Roles.Empleado) ||
    ctx.User.IsInRole(Roles.SuperAdmin))); // ✅ Agregar SuperAdmin

    options.AddPolicy("OnlyJefe", policy =>
        policy.RequireRole(Roles.JefeDeProyecto));

    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
 .Build();
});

builder.Services.AddSingleton<MySqlConnectionSingleton>();
builder.Services.AddScoped<MySqlRepositoryFactory<Proyecto>, ProyectoryRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Usuario>, UsuarioRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Tarea>, TareaRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Comentario>, ComentarioRepositoryCreator>();
builder.Services.AddScoped<IComentarioManager, ComentarioService>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ComentarioService>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Usuarios", "SoloAdmin");
    options.Conventions.AuthorizeFolder("/Proyectos", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Tareas", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Comentarios", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Empleados", "OnlyJefe"); // ✅ NUEVO
    options.Conventions.AuthorizeFolder("/Configuracion");
    options.Conventions.AuthorizePage("/Index", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizePage("/Proyectos/Create", "OnlyJefe");
    options.Conventions.AuthorizePage("/Tareas/Create", "OnlyJefe");
    options.Conventions.AuthorizePage("/Tareas/Asignar", "OnlyJefe");
    options.Conventions.AllowAnonymousToPage("/Login/Login");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
    options.Conventions.AllowAnonymousToPage("/Error");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Logout");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
  await next();
    if (context.Response.StatusCode == 403)
    {
        context.Response.Redirect("/AccessDenied");
    }
});

app.MapRazorPages();

app.Run();