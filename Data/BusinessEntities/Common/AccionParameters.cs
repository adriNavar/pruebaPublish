namespace GeoSit.Data.BusinessEntities.Common
{
    public class AccionParameters : IEntity
    {
        public string accion { get; set; }
        public int[] tramites { get; set; }
        public string sector { get; set; }
        public string usuario { get; set; }
        public string observ { get; set; }
    }
}
