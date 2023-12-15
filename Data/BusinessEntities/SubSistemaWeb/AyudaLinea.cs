namespace GeoSit.Data.BusinessEntities.SubSistemaWeb
{
    public class AyudaLinea : IEntity
    {
        public int IdAyuda { get; set; }
        public string Seccion { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }
        public long? IdFuncion { get; set; }
        public int IdTipo { get; set; }
        public int Orden { get; set; }
    }
}