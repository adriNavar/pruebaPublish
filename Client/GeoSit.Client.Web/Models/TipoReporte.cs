namespace GeoSit.Client.Web.Models
{
    public class TipoReporte
    {
        public int IdReporte { get; set; }
        public string Nombre { get; set; }
        public bool UtilizaTareas { get; set; }
        public bool UtilizaUsuarios { get; set; }

        public TipoReporte()
        {
            UtilizaTareas = true;
            UtilizaUsuarios = true;
        }
    }
}