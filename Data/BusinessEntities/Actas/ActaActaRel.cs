namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaActaRel : IEntity
    {
        public long ActaId { get; set; }
        public long ActaRelId { get; set; }
        public Acta Acta { get; set; }
    }
}