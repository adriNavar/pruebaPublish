namespace GeoSit.Data.BusinessEntities.Via.DTO
{
    public class TramoViaDTO
    {
        public long TramoViaId { get; set; }
        public string NombreVia { get; set; }
        public long ViaId { get; set; }
        public long TipoViaId { get; set; }
        public long? AlturaDesde { get; set; }
        public long? AlturaHasta { get; set; }
        public string Paridad { get; set; }
        public string Cpa { get; set; }
        public double? Aforo { get; set; }
        public string WKT { get; set; }
        public long LocalidadId { get; set; }
        public long UsuarioId { get; set; }
    }
}
