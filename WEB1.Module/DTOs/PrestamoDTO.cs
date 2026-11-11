namespace WEB1.Module.DTOs
{
    public class PrestamoDTO
    {
        public int IdPrestamo { get; set; }
        public int IdCliente { get; set; }
        public string? NombreCliente { get; set; }
        public int? IdUsuarioRegistro { get; set; }
        public decimal Monto { get; set; }
        public decimal PorcentajeInteres { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string ModalidadCobro { get; set; } = "Mensual";
        public string Estado { get; set; } = "Activo";
        public DateTime FechaRegistro { get; set; }
    }

    public class PrestamoCreacionDTO
    {
        public int IdCliente { get; set; }
        public int? IdUsuarioRegistro { get; set; }
        public decimal Monto { get; set; }
        public decimal PorcentajeInteres { get; set; } = 10;
        public string ModalidadCobro { get; set; } = "Mensual";
    }
}
