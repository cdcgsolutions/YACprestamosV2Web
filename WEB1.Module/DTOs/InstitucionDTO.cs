namespace WEB1.Module.DTOs
{
    public class InstitucionDTO
    {
        public int IdInstitucion { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public bool EstaHabilitado { get; set; } = true;
        public DateTime FechaRegistro { get; set; }
    }
}
