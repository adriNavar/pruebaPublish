using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Client.Web.Models
{
    public class EdicionObjetoEspecificoModels
    {
        public Operacion operacion { get; set; }
        public int tipoObjetoSelected { get; set; }
        public MEDatosEspecificos arbolObjetoSeleccionado { get; set; }
        public int? padre { get; set; }
    }

    public enum Operacion
    {
        Alta,
        Modificacion
    }
}