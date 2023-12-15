namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class SEEvento
    {
        public long Id_Evento { get; set; }
        public string Nombre { get; set; }
        public long Id_Funcion { get; set; }

        public Funciones Funcion { get; set; }
    }
}
