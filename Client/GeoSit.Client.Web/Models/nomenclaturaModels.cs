using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GeoSit.Client.Web.Models
{
    public class NomenclaturaModels
    {
        public NomenclaturaModels()
        {
            DatosNomenclatura = new NomenclaturaModel();
        }
        public NomenclaturaModel DatosNomenclatura { get; set; }
    }

    public class NomenclaturaModel
    {
        public long NomenclaturaID { get; set; }
        public long ParcelaID { get; set; }
        public string Nombre { get; set; }
        public int TipoNomenclaturaID { get; set; }
        public int? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
    }

}