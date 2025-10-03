using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sistema_de_Gestion_de_Proyectos_y_Tareas.Models;

namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Pages.Usuarios
{
    public class CreateModel : PageModel
    {
        private readonly IDB<Usuario> _usuarioRepository;
        [BindProperty]
        public Usuario Usuario { get; set; } = new();

        public CreateModel(IDB<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
          

            if (!ModelState.IsValid) return Page();
            await _usuarioRepository.AddAsync(Usuario);
            return RedirectToPage("./Index");
        }
    }
}