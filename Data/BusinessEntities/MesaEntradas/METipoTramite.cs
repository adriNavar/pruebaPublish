using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METipoTramite : IBajaLogica
    {
        public int IdTipoTramite { get; set; }
        public string Descripcion { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ICollection<MEPrioridadTipo> PrioridadesTipos { get; set; }
    }
}
