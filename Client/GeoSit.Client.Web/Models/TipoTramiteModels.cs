using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class TipoTramiteModels
    {
        public TipoTramiteModels()
        {
            DatosTipoTramite = new TipoTramiteModel();
            //EsAutonumerico = new bool();
            //BoolImprimeCabecera = new bool();
            //BoolImprimeUnidades = new bool();
            //BoolImprimeInformeFinal = new bool();
            //NombreInformeFinal = string();
        }
        public TipoTramiteModel DatosTipoTramite { get; set; }
        public bool EsAutonumerico { get; set; }
        public bool BoolImprimeCabecera { get; set; }
        public bool BoolImprimeUnidades { get; set; }
        public bool BoolImprimeInformeFinal { get; set; }
        public string NombreInformeFinal { get; set; }
        public string TextoInformeFinal { get; set; }
        public bool BoolConfiguracionImpresiónPorDefecto { get; set; }
        public bool BoolImprimeInformeSeccion { get; set; }
        public string NombreInformeSeccion { get; set; }
        public string TextoInformeSeccion { get; set; }
    }

    public class TipoTramiteModel
    {     
        public long Id_Tipo_Tramite { get; set; }
        public string Nombre { get; set; }
        public bool Autonumerico { get; set; }
        public Nullable<long> Numerador { get; set; }
        public bool Imprime_Cab { get; set; } //ImprimeCab //Nullable<int>

        public bool Imprime_UTS { get; set; }//ImprimeUts //Nullable<int>
        public bool Imprime_Doc { get; set; }//ImprimeDoc //Nullable<int>

        public bool Imprime_Final { get; set; }//ImprimeFinal //Nullable<int>

        public DateTime Fecha_Alta { get; set; }//FechaAlta

        public Nullable<DateTime> Fecha_Modif { get; set; }//FechaModif

        public Nullable<long> Id_Usu_Modif { get; set; }//idUsuModif

        public Nullable<long> Id_Usu_Baja { get; set; }//idUsuBaja

        public Nullable<DateTime> Fecha_Baja { get; set; }//FechaBaja

        public Nullable<long> Id_Usu_Alta { get; set; }//idUsuAlta

        public bool Imprime_Per { get; set; }//ImprimePer //Nullable<int>

        public string Plantilla_Final { get; set; }//PlantillaFinal


    }
}