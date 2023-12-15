using GeoSit.Data.BusinessEntities.Documentos;
using System;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaDocumento : IEntity
    {
        public long ParcelaDocumentoID { get; set; }
        public long ParcelaID { get; set; }
        public long DocumentoID { get; set; }
        public long UsuarioAltaID { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModificacionID { get; set; }
        public DateTime FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }

        /*Navegacion*/
        public Parcela Parcela { get; set; }
        public Documento Documento { get; set; }
    }
}
