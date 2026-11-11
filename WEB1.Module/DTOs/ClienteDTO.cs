namespace WEB1.Module.DTOs
{
    public class ClienteDTO
    {
        public int IdCliente { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string? Correo { get; set; }
        public string EstadoCivil { get; set; } = "Soltero(a)";
        public DateOnly? FechaNacimiento { get; set; }
        public string? Direccion { get; set; }
        public bool EstaHabilitado { get; set; } = true;
        public DateTime FechaRegistro { get; set; }
    }
}
