using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Client.Web.Models
{
    public class DomicilioModels
    {
        public DomicilioModels()
        {
            DatosDomicilio = new DomicilioModel();
        }
        public DomicilioModel DatosDomicilio { get; set; }
    }

    public class DomicilioModel
    {
        public long DomicilioId { get; set; }
        public string ViaNombre { get; set; }
        public string numero_puerta { get; set; }
        public string piso { get; set; }
        public string unidad { get; set; }
        public string barrio { get; set; }
        public string localidad { get; set; }
        public Nullable<long> IdLocalidad { get; set; }
        public string municipio { get; set; }
        public string provincia { get; set; }
        public Nullable<long> ProvinciaId { get; set; }
        public string pais { get; set; }
        public string ubicacion { get; set; }
        public string codigo_postal { get; set; }
        public long UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModifId { get; set; }
        public DateTime FechaModif { get; set; }
        public Nullable<long> UsuarioBajaId { get; set; }
        public Nullable<DateTime> FechaBaja { get; set; }
        public Nullable<long> ViaId { get; set; }
        public long TipoDomicilioId { get; set; }  
        public TipoDomicilio TipoDomicilio { get; set; }
    }
}