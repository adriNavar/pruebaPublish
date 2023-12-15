using System.Collections.Generic;
using System;

namespace GeoSit.Client.Web.Models
{
    public class MensuraModels
    {
        public MensuraModels()
        {
            DatosMensura = new MensuraModel();
            Mensaje = "";
            TextoBusqueda = "";
        }
        public MensuraModel DatosMensura { get; set; }
        public string Mensaje { get; set; }
        public string TextoBusqueda { get; set; }
    }

    public class MensuraModel
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


        public TipoMensuraModel TipoMensura { get; set; }
        public EstadoMensuraModel EstadoMensura { get; set; }

        public string Tipo { get; set; }
        public string Estado { get; set; }

        public ICollection<ParcelaMensuraModels> ParcelasMensuras { get; set; }
        public ICollection<MensuraRelacionadaModels> MensurasRelacionadasOrigen { get; set; }
        public ICollection<MensuraRelacionadaModels> MensurasRelacionadasDestino { get; set; }
        public ICollection<MensuraDocumentoModels> MensurasDocumentos { get; set; }

        //public ICollection<UnidadTributaria> UnidadesTributarias { get; set; }

    }

    public class EstadoMensuraModel
    {
        public long IdEstadoMensura { get; set; }
        public string Descripcion { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }


    }

    public class TipoMensuraModel
    {
        public long IdTipoMensura { get; set; }
        public string Descripcion { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }


    }

}