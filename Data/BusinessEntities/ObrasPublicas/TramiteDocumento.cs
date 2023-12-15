using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class TramiteDocumento : IEntity
    {
        public long Id_Tramite_Documento { get; set; }
        public long Id_Tramite { get; set; }
        public long Id_Documento { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }
        public Tramite Tramite { get; set; }
        public Documento Documento { get; set; }

    }
}
