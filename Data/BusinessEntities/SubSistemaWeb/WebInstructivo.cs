namespace GeoSit.Data.BusinessEntities.SubSistemaWeb
{
    public class WebInstructivo : IEntity
    {
        public long IdInstructivo { get; set; }
        public string Seccion { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }
        public long? IdFuncion { get; set; }
    }
}