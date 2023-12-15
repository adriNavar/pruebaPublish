using GeoSit.Data.BusinessEntities.Interfaces;

namespace GeoSit.Data.BusinessEntities.Generico
{
    public class MenuItem : IEntity
    {
        public long MenuItemId { get; set; }
        public string Nombre { get; set; }
        public long? MenuItemIdPadre { get; set; }
        public long? Acceso { get; set; }
        public string Accion { get; set; }
        public string Icono { get; set; }
        public long? TipoAccion { get; set; }
        public long IdFuncion { get; set; }
    }
}
