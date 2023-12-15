using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using System;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJPersonaDomicilio
    {
        public long IdPersonaDomicilio { get; set; }
        public long IdDominioTitular { get; set; }
        public long IdDomicilio { get; set; }
        public long IdTipoDomicilio { get; set; }
        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public DDJJDominioTitular DominioTitular { get; set; }
        public Domicilio Domicilio { get; set; }

        public string Tipo { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Barrio { get; set; }
        public string Calle { get; set; }
        public long? IdCalle { get; set; }
        public string Altura { get; set; }
        public string Piso { get; set; }
        public string Departamento { get; set; }
        public string CodigoPostal { get; set; }
        public string Municipio { get; set; }
        public string Pais { get; set; }

    }
}
