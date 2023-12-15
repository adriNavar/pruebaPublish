namespace GeoSit.Data.BusinessEntities.Actas
{
    public class ActaEstado : IEntity
    {
        public long EstadoActaId { get; set; }

        public long ActaId { get; set; }
        public Acta Acta { get; set; }
    }
}