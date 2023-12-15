using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;


namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class UnidadTributaria : IEntity
    {
        public long UnidadTributariaId { get; set; }
        public long? ParcelaID { get; set; }
        public string CodigoProvincial { get; set; }
        public string CodigoMunicipal { get; set; }
        public string UnidadFuncional { get; set; }
        public decimal PorcentajeCopropiedad { get; set; }
        public long? JurisdiccionID { get; set; }
        public long? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public long? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public DateTime? FechaVigenciaDesde { get; set; }
        public DateTime? FechaVigenciaHasta { get; set; }
        public string Observaciones { get; set; }

        public decimal PorcientoCopropiedadTotal { get; set; }

        public int TipoUnidadTributariaID { get; set; }
        public string PlanoId { get; set; }
        public double? Superficie { get; set; }
        public string Piso { get; set; }
        public string Unidad { get; set; }
        public DateTime? Vigencia { get; set; }



        /* PROPIEDADES DE NAVEGACION */
        public TipoUnidadTributaria TipoUnidadTributaria { get; set; }
        public Parcela Parcela { get; set; }
        public ICollection<ValuacionPadronDetalle> Valuaciones { get; set; }
        public ICollection<UnidadTributariaDomicilio> UTDomicilios { get; set; }
        public ICollection<UnidadTributariaExpedienteObra> UnidadTributariaExpedienteObras { get; set; }
        public ICollection<ActaUnidadTributaria> ActaUnidadTributarias { get; set; }
        public ICollection<UnidadTributariaDocumento> UTDocumentos { get; set; }
        public ICollection<Dominio> Dominios { get; set; }
        public Designacion Designacion { get; set; }
        public Objeto Objeto { get; set; }
        public ICollection<VALValuacion> UTValuacionesHistoricas { get; set; }
        public DDJJ DeclaracionJ { get; set; }

        [NotMapped]
        public virtual VALValuacion UTValuaciones { get; set; }

    }
}
