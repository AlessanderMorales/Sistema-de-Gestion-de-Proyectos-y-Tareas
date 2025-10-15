namespace Sistema_de_Gestion_de_Proyectos_y_Tareas.Helpers
{
    public static class InputSanitizer
    {
        public static string? NormalizeSpaces(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;
            // Trim start/end and collapse multiple spaces to single
            var trimmed = input.Trim();
            while (trimmed.Contains("  "))
            {
                trimmed = trimmed.Replace("  ", " ");
            }
            return trimmed;
        }
    }
}
