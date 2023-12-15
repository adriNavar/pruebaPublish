
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class UsuariosHist : IEntity
    {
        public long Id_Usuario_Hist { get; set; }
        public long Id_Usuario { get; set; }
        public string Login { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Mail { get; set; }
        public string Sector { get; set; }
        public string Id_Tipo_Doc { get; set; }
        public string Nro_Doc { get; set; }
        public string Domicilio { get; set; }
        public bool Habilitado { get; set; }
        public bool Cambio_Pass { get; set; }
        public long Usuario_Operacion { get; set; }
        public DateTime Fecha_Operacion { get; set; }
        
    }
    


}

    
