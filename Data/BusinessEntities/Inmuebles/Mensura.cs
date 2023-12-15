using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class Mensura : IEntity
    {
        public long IdMensura { get; set; }
        public long IdTipoMensura { get; set; }
        public long IdEstadoMensura { get; set; }
        public string Numero { get; set; }
        public string Letra { get; set; }
        public DateTime? FechaPresentacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string Descripcion { get; set; }
        public string MensurasRelacionadasTexto { get; set; }
        public string Observaciones { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }


        public TipoMensura TipoMensura { get; set; }
        public EstadoMensura EstadoMensura { get; set; }

        //
        public String DescripcionTipoMensura { get; set; }

        public int Archivo { get; set; }

        public ICollection<ParcelaMensura> ParcelasMensuras { get; set; }
        public ICollection<MensuraDocumento> MensurasDocumentos { get; set; }
        public ICollection<MensuraRelacionada> MensurasRelacionadasOrigen { get; set; }
        public ICollection<MensuraRelacionada> MensurasRelacionadasDestino { get; set; }
    }
}
