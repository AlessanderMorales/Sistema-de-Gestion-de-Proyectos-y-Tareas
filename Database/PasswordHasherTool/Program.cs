using System;
using System.Security.Cryptography;

Console.WriteLine("=================================================================");
Console.WriteLine("   GENERADOR DE HASHES PBKDF2 PARA BASE DE DATOS");
Console.WriteLine("   Sistema de Gestión de Proyectos y Tareas");
Console.WriteLine("=================================================================\n");

// Generar hashes para todas las contraseñas
var passwords = new[]
{
    new { User = "admin", Email = "admin@sgpt.com", Password = "Admin123!", Role = "SuperAdmin" },
    new { User = "jefeproy", Email = "jefeProy@sgpt.com", Password = "Jefe123!", Role = "JefeDeProyecto" },
    new { User = "empleado1", Email = "empleado1@sgpt.com", Password = "Empleado1!", Role = "Empleado" },
    new { User = "empleado2", Email = "empleado2@sgpt.com", Password = "Empleado2!", Role = "Empleado" },
    new { User = "empleado3", Email = "empleado3@sgpt.com", Password = "Empleado3!", Role = "Empleado" }
};

Console.WriteLine("CONTRASEÑAS HASHEADAS:");
Console.WriteLine("-------------------------------------------------------------------\n");

foreach (var user in passwords)
{
    string hash = HashPassword(user.Password);
    
    Console.WriteLine($"-- {user.Role}: {user.User}");
    Console.WriteLine($"-- Email: {user.Email} | Contraseña: {user.Password}");
    Console.WriteLine($"UPDATE Usuario ");
    Console.WriteLine($"SET contraseña = '{hash}'");
    Console.WriteLine($"WHERE nombre_usuario = '{user.User}';");
    Console.WriteLine();
}

Console.WriteLine("\n=================================================================");
Console.WriteLine("   COPIE Y EJECUTE ESTOS UPDATE EN SU BASE DE DATOS MySQL");
Console.WriteLine("=================================================================");

static string HashPassword(string password)
{
    using var rng = RandomNumberGenerator.Create();
    byte[] salt = new byte[16];
    rng.GetBytes(salt);

    using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
    byte[] hash = pbkdf2.GetBytes(32);

    return $"PBKDF2:100000:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
}
