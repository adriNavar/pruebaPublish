using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Personas;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class METramite : IEntity, IBajaLogica
    {
        public int IdTramite { get; set; }
        public string Numero { get; set; }
        public int IdPrioridad { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaLibro { get; set; }
        public DateTime? FechaVenc { get; set; }
        public long IdJurisdiccion { get; set; }
        public long? IdLocalidad { get; set; }
        public int IdTipoTramite { get; set; }
        public int IdObjetoTramite { get; set; }
        public string Motivo { get; set; }
        public int IdEstado { get; set; }
        public long? IdIniciador { get; set; }
        public long? IdUnidadTributaria { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }


        public MEPrioridadTramite Prioridad { get; set; }
        public Objeto Jurisdiccion { get; set; }
        public Objeto Localidad { get; set; }
        public METipoTramite Tipo { get; set; }
        public MEObjetoTramite Objeto { get; set; }
        public MEEstadoTramite Estado { get; set; }
        public Persona Iniciador { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; }

        public ICollection<MEMovimiento> Movimientos { get; set; }
        public ICollection<METramiteRequisito> TramiteRequisitos { get; set; }
        public ICollection<METramiteDocumento> TramiteDocumentos { get; set; }
        public ICollection<MEDesglose> Desgloses { get; set; }
        public ICollection<METramiteEntrada> TramiteEntradas { get; set; }



    }
}
