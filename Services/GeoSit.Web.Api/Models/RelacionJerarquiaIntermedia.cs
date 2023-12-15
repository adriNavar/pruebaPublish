using MT = GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Web.Api.Models
{
    internal class RelacionJerarquiaIntermedia
    {
        internal MT.Atributo AtributoClaveJoin { get; set; }
        internal MT.Atributo AtributoClaveMain { get; set; }
        internal MT.Atributo AtributoFiltro { get; set; }
        internal MT.Componente ComponenteJoin { get; set; }
        internal object ValorFiltro { get; set; }
    }
}