namespace GeoSit.Data.BusinessEntities.ObjetosAdministrativos
{
    public class TipoObjeto : IEntity
    {
        public long TipoObjetoId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long? IdTipoObjetoPadre { get; set; }
        public string Esquema { get; set; }
    }
}
