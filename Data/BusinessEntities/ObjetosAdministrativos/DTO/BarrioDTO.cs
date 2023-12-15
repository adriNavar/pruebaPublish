namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos.DTO
{
    public class BarrioDTO
    {
        public long FeatId { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Descripcion { get; set; }
        public string Nomenclatura { get; set; }
        public string WKT { get; set; }
        public long? LocalidadId { get; set; }
        public long UsuarioId { get; set; }
    }
}
