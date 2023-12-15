using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class EspacioPublicoTemporal : ITemporalTramite
    {
        public long EspacioPublicoID { get; set; }
        public long ParcelaID { get; set; }
        public int IdTramite { get; set; }
        public decimal Superficie { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        
    }
}
