using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class TramoViaModels
    {
        public TramoViaModels()
        {
            DatosTramoVia = new TramoViaModel();
        }
        public TramoViaModel DatosTramoVia { get; set; }
    }

    public class TramoViaModel
    {
        public long TramoViaId { get; set; }
        public long ViaId { get; set; }
        public Nullable<long> AlturaDesde { get; set; }
        public Nullable<long> AlturaHasta { get; set; }
        public string Sufijo { get; set; }
        public string Paridad { get; set; }
        public string Cpa { get; set; }
        public string Geometry { get; set; }
        public byte[] Atributos { get; set; }
        public Nullable<long> UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public Nullable<long> UsuarioModifId { get; set; }
        public Nullable<DateTime> FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public long TramoId { get; set; }
        public long TipoViaId { get; set; }

    }
}