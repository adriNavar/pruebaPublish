using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEDatosEspecificos
    {
        public Guid Guid { get; set; }
        public IEnumerable<Guid> ParentGuids { get; set; }
        public long? IdTramiteEntrada { get; set; }
        public IEnumerable<int> ParentIdTramiteEntradas { get; set; }
        public TipoDatoEspecifico TipoObjeto { get; set; }
        public List<Propiedad> Propiedades { get; set; }
    }

    public class TipoDatoEspecifico
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class Propiedad
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Visible { get; set; }
        public string Label { get; set; }
    }
}
