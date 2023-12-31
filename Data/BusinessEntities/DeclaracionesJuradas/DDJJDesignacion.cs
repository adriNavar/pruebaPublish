﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.DeclaracionesJuradas
{
    public class DDJJDesignacion
    {
        public long IdDesignacion { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public long IdTipoDesignador { get; set; }
        public int? IdCalle { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public int? IdBarrio { get; set; }
        public string Barrio { get; set; }
        public int? IdLocalidad { get; set; }
        public string Localidad { get; set; }
        public int? IdDepartamento { get; set; }
        public string Departamento { get; set; }
        public int? IdParaje { get; set; }
        public string Paraje { get; set; }
        public int? IdSeccion { get; set; }
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

        public DDJJ DeclaracionJurada { get; set; }

    }
}
