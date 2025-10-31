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
using ServiceProyecto.Application.Service.Reportes;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Middleware;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("es-ES");
cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
cultureInfo.DateTimeFormat.LongDatePattern = "dd/MM/yyyy";
cultureInfo.DateTimeFormat.DateSeparator = "/";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);
    options.SupportedCultures = new[] { cultureInfo };
    options.SupportedUICultures = new[] { cultureInfo };
    options.ApplyCurrentCultureToResponseHeaders = true;
});

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
    ctx.User.IsInRole(Roles.SuperAdmin)));

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
builder.Services.AddScoped<ReporteService>();
builder.Services.AddScoped<Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Facades.GestionProyectosFacade>();
builder.Services.AddScoped<ServiceProyecto.Application.Service.Reportes.IPdfReporteBuilder, ServiceProyecto.Application.Service.Reportes.PdfReporteBuilder>();
builder.Services.AddScoped<ServiceProyecto.Application.Service.Reportes.IExcelReporteBuilder, ServiceProyecto.Application.Service.Reportes.ExcelReporteBuilder>();
builder.Services.AddScoped<ServiceProyecto.Application.Service.Reportes.ReporteService>();

builder.Services.AddRazorPages(options =>
{
  options.Conventions.AuthorizeFolder("/Usuarios", "SoloAdmin");
    options.Conventions.AuthorizeFolder("/Proyectos", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Tareas", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Comentarios", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Empleados", "OnlyJefe"); 
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

app.UseRequestLocalization();

app.UseRouting();

app.UseAuthentication();

app.UseValidateUserExists();

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