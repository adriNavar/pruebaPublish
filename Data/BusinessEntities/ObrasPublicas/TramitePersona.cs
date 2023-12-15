using System;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.Personas;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class TramitePersona : IEntity
    {
        public long Id_Tramite_Persona { get; set; }
        public long Id_Tramite { get; set; }
        public long Id_Persona { get; set; }
        public long Id_Rol { get; set; }
        //public string Imprime { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }

        public Tramite Tramite { get; set; }
        public Persona Persona { get; set; }
        public TramiteRol Rol { get; set; }

    }
}
