using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class TramitePermisos
    {
        public long ID_SECCION_FUNCION { get; set; }
        public long ID_TIPO_SECCION { get; set; }
        public long ID_FUNCION { get; set; }
        public long ID_USU_ALTA { get; set; }
        public DateTime FECHA_ALTA { get; set; }
        public long ID_USU_MODIF { get; set; }
        public DateTime FECHA_MODIF { get; set; }
        public long? ID_USU_BAJA { get; set; }
        public DateTime? FECHA_BAJA { get; set; }
        public long? ID_TIPO_TRAMITE { get; set; }
    }
}