namespace WEB1.Module.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador";
        public bool EstaHabilitado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class UsuarioCreacionDTO
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador";
    }

    public class UsuarioEdicionDTO
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador";
        public bool EstaHabilitado { get; set; }
        public string? NuevaContrasena { get; set; }
    }

    public class UsuarioFormModel
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string CorreoElectronico { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operador";
        public string Contrasena { get; set; } = string.Empty;
        public bool EstaHabilitado { get; set; } = true;
    }
}
