using System;
using System.Data;
using System.Text;
using System.IO;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObrasPublicas;

namespace GeoSit.Client.Web.Models.ObrasPublicas
{
    public class TramiteSaveModel
    {
        public int Tipo { get; set; }
        public string Identificador { get; set; }
        public string FechaInicio { get; set; }
        public long NumeroTramite { get; set; }
        public bool ImprimeUTS { get; set; }
        public bool ImprimePersonas { get; set; }
        public bool ImprimeDocumentos { get; set; }
        public string InformeFinal { get; set; }
        public string Estado { get; set; }
        public bool ImprimeInformeFinal { get; set; }
    }
    public class TramiteModel
    {
        public TramiteModel()
        {
            TramiteEstados = new List<TramiteEstado>();

            TramiteEstados.Add(new TramiteEstado(1, "Iniciado"));
            TramiteEstados.Add(new TramiteEstado(2, "En Proceso"));
            TramiteEstados.Add(new TramiteEstado(3, "Pendiente de Cierre"));
            TramiteEstados.Add(new TramiteEstado(4, "Finalizado"));
            TramiteEstados.Add(new TramiteEstado(5, "Anulado"));

            TramiteUnidadTributaria = new List<TramiteUnidadTributaria>();
        }
        public virtual List<TramiteEstado> TramiteEstados { get; set; }
        public List<TramiteUnidadTributaria> TramiteUnidadTributaria { get; set; }
    }

    public class TramiteEstado
    {
        public TramiteEstado(int pId_Estado, string pDescripcion)
        {
            Id_Estado = pId_Estado;
            Descripcion = pDescripcion;
        }

        public int Id_Estado { get; set; }
        public string Descripcion { get; set; }
    }
}
