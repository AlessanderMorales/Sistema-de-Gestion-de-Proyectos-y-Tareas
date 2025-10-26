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

// ✅ Autenticación por cookies
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "Sgpt.AuthCookie";
        options.LoginPath = "/Login/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// ✅ Políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdmin", policy =>
        policy.RequireRole(Roles.SuperAdmin));

    options.AddPolicy("OnlyJefeOrEmpleado", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInRole(Roles.JefeDeProyecto) || ctx.User.IsInRole(Roles.Empleado)));

    options.AddPolicy("OnlyJefe", policy =>
        policy.RequireRole(Roles.JefeDeProyecto));
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
    options.Conventions.AuthorizePage("/Index", "OnlyJefeOrEmpleado");

    options.Conventions.AuthorizePage("/Proyectos/Create", "OnlyJefe");
    options.Conventions.AuthorizePage("/Tareas/Create", "OnlyJefe");
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

app.MapRazorPages();

app.Run();