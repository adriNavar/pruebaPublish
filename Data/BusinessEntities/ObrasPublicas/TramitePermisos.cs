using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    [Table("TRT_SECCION_FUNCION")]
    public class TramitePermisos : IEntity
    {
       [Key]
       public long ID_SECCION_FUNCION { get; set; }		
       public long ID_TIPO_SECCION {get;set;}		
       public long ID_FUNCION {get;set;}	
       public long ID_USU_ALTA {get;set;}	
       public DateTime FECHA_ALTA {get;set;}
       public long ID_USU_MODIF {get;set;}
       public DateTime FECHA_MODIF {get;set;}
       public long? ID_USU_BAJA {get;set;}
       public DateTime? FECHA_BAJA { get; set; }
       public long? ID_TIPO_TRAMITE { get; set; }
    }
}
