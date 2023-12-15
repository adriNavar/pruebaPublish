using Newtonsoft.Json;

namespace GeoSit.Data.BusinessEntities.MapasTematicos
{
    public class Atributo : IEntity
    {
        public long AtributoId { get; set; }
        public long ComponenteId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public short Orden { get; set; }
        public string Campo { get; set; }
        public string Funcion { get; set; }
        public int TipoDatoId { get; set; }
        public short Precision { get; set; }
        public short Escala { get; set; }
        public short EsClave { get; set; }
        public bool EsVisible { get; set; }
        public bool EsValorFijo { get; set; }
        public bool EsGeometry { get; set; }
        public bool EsLabel { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsEditable { get; set; }
        public long? ComponenteParentId { get; set; }
        public long? AtributoParentId { get; set; }
        public bool EsEditableMasivo { get; set; }
        public bool EsFeatId { get; set; }

        [JsonIgnore]
        public Componente Componente { get; set; }
    }
}
