using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Mantenimiento
{
    public class AtributoTA : IEntity
    {
        public const int TipoBoolean = 1;
        public const int TipoInteger = 2;
        public const int TipoLong = 3;
        public const int TipoDouble = 4;
        public const int TipoDate = 5;
        public const int TipoString = 6;
        public const int TipoFloat = 7;
        public const int TipoGeometry = 8;
        public const int TipoXml = 9;
        public const int TipoXsd = 10;
        public const int TipoCheck = 11;
        public const int TipoNumeracion = 12;
        public const int TipoClob = 13;

        public long Id_Atributo { get; set; }
        public long Id_Componente { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public long Orden { get; set; }
        public string Campo { get; set; }
        public string Funcion { get; set; }
        public int Id_Tipo_Dato { get; set; }
        public long Precision { get; set; }
        public long? Escala { get; set; }
        public long Es_Clave { get; set; }
        public bool Es_Visible { get; set; }
        public bool Es_Obligatorio { get; set; }
        public string Tabla { get; set; }
        public string  Esquema { get; set; }
        public string Campo_Relac { get; set; }
        public string Descriptor { get; set; }
        public bool Es_Geometry { get; set; }
        public bool Es_Valor_Fijo { get; set; }
        public bool Es_Editable { get; set; }
        public bool Es_Filtro { get; set; }
        public string Enumeracion { get; set; }
        public string Expresion_Regular { get; set; }

        public string Valor { get; set; }
        public List<AtributoRelacionado> Opciones { get; set; }

    }
  

}

    
