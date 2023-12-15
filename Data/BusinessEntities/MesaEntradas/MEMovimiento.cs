using GeoSit.Data.BusinessEntities.Seguridad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEMovimiento
    {
        public int IdMovimiento { get; set; }
        public int IdTramite { get; set; }
        public int IdTipoMovimiento { get; set; }
        public int IdSectorOrigen { get; set; }
        public int IdSectorDestino { get; set; }
        public int? IdRemito { get; set; }
        public string Observacion { get; set; }
        public int IdEstado { get; set; }

        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long? UsuarioRecep { get; set; }
        public DateTime? FechaRecep { get; set; }

        public METramite Tramite { get; set; }
        public METipoMovimiento TipoMovimiento { get; set; }
        public Sector SectorOrigen { get; set; }
        public Sector SectorDestino { get; set; }
        public MERemito Remito { get; set; }
        public MEEstadoTramite Estado { get; set; }
        public Usuarios Usuario_Alta { get; set; }

    }
}
