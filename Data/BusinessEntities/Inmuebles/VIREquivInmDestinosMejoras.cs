
namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class VIREquivInmDestinosMejoras : IEntity
    {
        public int Id { get; set; }
        public int? DCGIdDestinoMejora { get; set; }
        public int? DCGMejoraTipoDDJJ { get; set; }
        public string DCGDescripcion { get; set; }
        public string VIRUso { get; set; }
        public int? VIRIdTipo { get; set; }
        public string VIRTipoDescripcion { get; set; }
    }
}
