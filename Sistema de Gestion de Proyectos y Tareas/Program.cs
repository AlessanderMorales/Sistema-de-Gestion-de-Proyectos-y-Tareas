using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data; 
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Services;

using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddSingleton<MySqlConnectionSingleton>();
builder.Services.AddScoped<MySqlRepositoryFactory<Proyecto>, ProyectoryRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Usuario>, UsuarioRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Tarea>, TareaRepositoryCreator>();
builder.Services.AddScoped<MySqlRepositoryFactory<Comentario>, ComentarioRepositoryCreator>();
builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ComentarioService>();

// App runtime singleton to detect restarts
builder.Services.AddSingleton<AppRuntime>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var app = builder.Build();

// If runtime changes (app restart), sign out cookies by checking a runtime cookie
app.Use(async (context, next) =>
{
    var runtime = context.RequestServices.GetService<AppRuntime>();
    const string runtimeCookieName = "_app_runtime";
    var runtimeCookie = context.Request.Cookies[runtimeCookieName];
    if (runtimeCookie != runtime.InstanceId)
    {
        // set new runtime cookie (session-only)
        context.Response.Cookies.Append(runtimeCookieName, runtime.InstanceId, new CookieOptions { HttpOnly = true, IsEssential = true });
        // sign out current user if any
        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
    await next();
});

// run seeder
try
{
    DatabaseSeeder.Seed(app.Services);
}
catch
{
    // ignore seed errors at startup
}

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

// Redirect root to login when user is not authenticated
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/" || context.Request.Path == "")
    {
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.Redirect("/Account/Login");
            return;
        }
    }
    await next();
});

app.MapRazorPages();
app.Run();