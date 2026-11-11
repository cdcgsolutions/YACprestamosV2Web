namespace WEB1.Module.DTOs
{
    public class PagoDTO
    {
        public int IdPago { get; set; }
        public int IdPrestamo { get; set; }
        public decimal MontoAbonado { get; set; }
        public DateTime FechaPago { get; set; }
    }

    public class RegistrarAbonoDTO
    {
        public int IdPrestamo { get; set; }
        public decimal MontoAbonado { get; set; }
    }

    public class ResumenHistorialPagosDTO
    {
        public int IdPrestamo { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public decimal MontoTotalPrestamo { get; set; }
        public decimal SaldoPendiente { get; set; }
        public decimal TotalAbonado { get; set; }
        public List<PagoDTO> Pagos { get; set; } = new();
    }
}
