namespace GeoSit.Data.BusinessEntities.ObrasParticulares
{
    public class UbicacionExpedienteObra
    {
        public long DomicilioInmuebleId { get; set; }

        public string NombreVia { get; set; }

        public string NumeroPuerta { get; set; }

        public string Barrio { get; set; }

        public string CodigoPostal { get; set; }

        public bool DomicilioPrimario { get; set; }

        public string DomicilioPrimarioSiNo { get; set; }
    }
}
