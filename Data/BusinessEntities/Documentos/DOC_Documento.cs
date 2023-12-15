using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Data;
using System.Dynamic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoSit.Data.BusinessEntities.Documentos
{
    public class DOC_Documento : IEntity
    {
        public long Id_Documento { get; set; }
        [ForeignKey("mDOC_Tipo_Documento")]
        public long Id_Tipo_Documento { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
        public string Observaciones { get; set; }
        public string Nombre_Archivo { get; set; }
        public string Extension_Archivo { get; set; }
        //public string Contenido { get; set; }
        public Nullable<long> Id_Usu_Alta { get; set; }
        public Nullable<DateTime> Fecha_Alta { get; set; }
        public Nullable<long> Id_Usu_Modif { get; set; }
        public Nullable<DateTime> Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }

        public DOC_Tipo_Documento mDOC_Tipo_Documento { get; set; }
    }
}
