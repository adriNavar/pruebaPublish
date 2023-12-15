using System;
using GeoSit.Data.BusinessEntities.MesaEntradas;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaOperacion : IEntity
    {
        public long ParcelaOperacionID { get; set; }
        public long TipoOperacionID { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public long? ParcelaOrigenID { get; set; }
        public long ParcelaDestinoID { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public int? IdTramite { get; set; }

        /*Propiedades de Navegacion*/
        public TipoParcelaOperacion Tipo { get; set; }
        public Parcela ParcelaOrigen { get; set; } //ESTO ERA Parcela
        public Parcela ParcelaDestino { get; set; } 
        public METramite Tramite { get; set; }
    }
}
