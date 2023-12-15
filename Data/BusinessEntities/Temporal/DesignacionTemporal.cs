using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.Temporal
{
    public class DesignacionTemporal : ITemporalTramite
    {
        public long IdDesignacion { get; set; }
        public int IdTramite { get; set; }
        public short IdTipoDesignador { get; set; }
        public long IdParcela { get; set; }
        public long? IdCalle { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public long? IdBarrio { get; set; }
        public string Barrio { get; set; }
        public long? IdLocalidad { get; set; }
        public string Localidad { get; set; }
        public long? IdDepartamento { get; set; }
        public string Departamento { get; set; }
        public long? IdParaje { get; set; }
        public string Paraje { get; set; }
        public long? IdSeccion { get; set; }
        public string Seccion { get; set; }
        public string Chacra { get; set; }
        public string Quinta { get; set; }
        public string Fraccion { get; set; }
        public long? IdManzana { get; set; }
        public string Manzana { get; set; }
        public string Lote { get; set; }
        public string CodigoPostal { get; set; }

        public long IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public ParcelaTemporal Parcela { get; set; }
    }
}
