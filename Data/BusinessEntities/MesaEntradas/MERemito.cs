using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MERemito : IBajaLogica
    {
        public int IdRemito { get; set; }
        public int IdSectorOrigen { get; set; }
        public int IdSectorDestino { get; set; }
        public string Numero { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long? UsuarioRecep { get; set; }
        public DateTime? FechaRecep { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }

        public Sector SectorOrigen { get; set; }
        public Sector SectorDestino { get; set; }

        public Usuarios Receptor { get; set; }

        public ICollection<MEMovimiento> Movimientos { get; set; }
    }
}
