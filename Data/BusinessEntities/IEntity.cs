using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace GeoSit.Data.BusinessEntities
{
    public class IEntity
    {
        [NotMapped]
        public long _Id_Usuario { get; set; }
        //[NotMapped]
        //public long _Id_Evento { get; set; }
        //[NotMapped]
        //public string _Datos_Adicionales { get; set; }
        //[NotMapped]
        //public DateTime _Fecha { get; set; }
        [NotMapped]
        public string _Ip { get; set; }
        [NotMapped]
        public string _Machine_Name { get; set; }
        //[NotMapped]
        //public string _Autorizado { get; set; }
        //[NotMapped]
        //public string _Objeto_Origen { get; set; }
        //[NotMapped]
        //public string _Objeto_Modif { get; set; }
        //[NotMapped]
        //public long _Id_Tipo_Operacion { get; set; }
        //[NotMapped]
        //public string _Objeto { get; set; }
        //[NotMapped]
        //public long _Cantidad { get; }
    }
}
