using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GeoSit.Data.BusinessEntities.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.BusinessEntities.ObrasPublicas
{
    public class Tramite : IEntity
    {
        public long Id_Tramite { get; set; }
        public long Id_Tipo_Tramite { get; set; }
        public DateTime Fecha { get; set; }
        public long Nro_Tramite { get; set; }
        public string Cod_Tramite { get; set; }
        public bool Imprime_Cab { get; set; }
        public bool Imprime_UTS { get; set; }
        public bool Imprime_Per { get; set; }
        public bool Imprime_Doc { get; set; }
        public bool Imprime_Final { get; set; }
        public long Id_Usu_Alta { get; set; }
        public DateTime Fecha_Alta { get; set; }
        public long Id_Usu_Modif { get; set; }
        public DateTime Fecha_Modif { get; set; }
        public Nullable<long> Id_Usu_Baja { get; set; }
        public Nullable<DateTime> Fecha_Baja { get; set; }
        public string Estado { get; set; }
        public string Informe_Final { get; set; }
        public virtual TipoTramite TipoTramite { get; set; }
        public ICollection<TramitePersona> Personas { get; set; }
        public ICollection<TramiteSeccion> Secciones { get; set; }
    }
}
