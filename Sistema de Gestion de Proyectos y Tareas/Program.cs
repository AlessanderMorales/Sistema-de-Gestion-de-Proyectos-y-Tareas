using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Entities;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Data;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Infrastructure.Persistence.Factories;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Application.Services;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Domain.Common;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "Sgpt.AuthCookie";
        options.LoginPath = "/Login/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });


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

builder.Services.AddScoped<ProyectoService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<ComentarioService>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Usuarios", "SoloAdmin");
    options.Conventions.AuthorizeFolder("/proyectos", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Tareas", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizeFolder("/Comentarios", "OnlyJefeOrEmpleado");
    options.Conventions.AuthorizePage("/Index", "OnlyJefeOrEmpleado");

    options.Conventions.AuthorizePage("/proyectos/Create", "OnlyJefe");
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