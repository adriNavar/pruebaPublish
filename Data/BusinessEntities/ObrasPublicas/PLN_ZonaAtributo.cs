using System;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class PLN_ZonaAtributo : IEntity
    {

        public long Id_Zona_Atributo { get; set; }
        public long Id_Atributo_Zona { get; set; }
        public string Valor { get; set; }
        public string U_Medida { get; set; }
        public long FeatId_Objeto { get; set; }
        public string Observaciones { get; set; }
        public long Id_Usu_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Id_Usu_Modif { get; set; }
        public DateTime Fecha_Modif { get; set; }
        public long? Id_Usu_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }

        public PLN_Atributo Atributo { get; set; }
        public Objeto ObjetoAdministrativo { get; set; }
    }
}
