using System.Collections.Generic;

namespace GeoSit.Client.Web.Models.Search
{
    public class BuscadorGenericoConfig
    {
        public string Titulo { get; internal set; }
        public bool MultiSelect { get; internal set; }
        public string Tipos { get; internal set; }
        public IEnumerable<string> Campos { get; internal set; }
        public bool VerAgregar { get; internal set; }
        public object[][] SeleccionActual { get; internal set; }
        public bool IncluirTextoBusqueda { get; internal set; }
    }
    public class BuscadorGenericoSuggest
    {
        public long Id { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
    }
}