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
    public class DOC_Tipo_Documento: IEntity
    {
        public long Id_Tipo_Documento { get; set; }
        public string Descripcion { get; set; }
    }
}
