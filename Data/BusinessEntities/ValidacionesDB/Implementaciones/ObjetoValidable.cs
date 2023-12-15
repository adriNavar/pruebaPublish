using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using System;

namespace GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones
{
    public class ObjetoValidable
    {
        public TipoObjetoValidable TipoObjeto { get; set; }
        public FuncionValidable Funcion { get; set; }
        public GrupoValidable Grupo { get; set; }
        public int? IdTramite { get; set; }
        public long? IdObjeto { get; set; }
        public string Tipo { get; set; }
        public string SubTipo { get; set; }
        public string Codigo { get; set; }
        public string Valor { get; set; }
        public string WKT { get; set; }
        public string Codigo1 { get; set; }
        public string Codigo2 { get; set; }
        public string Codigo3 { get; set; }
        public string Codigo4 { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
