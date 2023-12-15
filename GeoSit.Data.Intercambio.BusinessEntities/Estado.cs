
namespace GeoSit.Data.Intercambio.BusinessEntities
{
    public class Estado
    {
        public int IdEstado { get; set; }
        public string Nombre { get; set; }
        public int Orden { get; set; }
        public int? IdMunicipio { get; set; }
    }
}
