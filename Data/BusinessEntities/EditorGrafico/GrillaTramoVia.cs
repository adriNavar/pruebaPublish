namespace GeoSit.Data.BusinessEntities.EditorGrafico.DTO
{
    public class GrillaTramoVia
    {
        public long TramoViaId { get; set; }
        public string Calle { get; set; }
        public string Altura { get; set; }
        public string Paridad { get; set; }
        public string Localidad { get; set; }
        public long LocalidadId { get; set; }
        public long TipoViaId { get; set; }
        public string TipoVia { get; set; }
    }
}