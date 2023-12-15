
using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.Seguridad
{
    public class ParametrosGenerales
    {
        public long Id_Parametro { get; set; }
        public string Clave { get; set; }
        public string Valor { get; set; }
        public string Descripcion { get; set; }
        public string Agrupador { get; set; }

    }
    public class ParametrosGenerales_Valores : IEntity
    {
        public string LimitesAccesos { get; set; }
        public string LimitesClaves { get; set; }
        public string ConexionesDesde { get; set; }
        public string IntentosDesde { get; set; }
        public string InactividadDesde { get; set; }
        public string HabilitaMail { get; set; }
        public string Email { get; set; }
        public string VigenciaDesde { get; set; }
        public string CantidadDiasPassDesde { get; set; }
        public string CantidadAlmacenadaDesde { get; set; }
        public string LongitudDesde { get; set; }
        public string NivelLetras { get; set; }
        public string NivelNumeros { get; set; }
        public string NivelEspeciales { get; set; }
        public string NivelMayusculas { get; set; }
        public string NivelMinusculas { get; set; }
        public string MaximoNumeroPloteos { get; set; }
    }


}

    
