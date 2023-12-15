
using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Interfaces;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class PerfilesFunciones : IEntity
    {
        public long Id_Perfil_Funcion { get; set; }
        public long Id_Perfil { get; set; }
        public long Id_Funcion { get; set; }
        public long Usuario_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long? Usuario_Baja { get; set; }
        public DateTime? Fecha_Baja { get; set; }
        public Perfiles Perfiles { get; set; }
        public Funciones Funciones { get; set; }    
    }
}

    
