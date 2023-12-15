using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METramiteDocumento : IBajaLogica
    {
        public int IdTramiteDocumento { get; set; }
        public int IdTramite { get; set; }
        public long id_documento { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaAprobacion { get; set; }

        public Documento Documento { get; set; }
        public Usuarios Usuario_Alta { get; set; }
    }
}