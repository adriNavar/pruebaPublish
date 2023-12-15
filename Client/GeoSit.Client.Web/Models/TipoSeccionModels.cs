//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System;
//using System.Web;

//namespace GeoSit.Client.Web.Models
//{
//    public class TipoSeccionModels
//    {
//        public TipoSeccionModels()
//        {
//            DatosTipoSeccion = new TipoSeccionModel();

//        }
//        public TipoSeccionModel DatosTipoSeccion { get; set; }
//    }

//    public class TipoSeccionModel
//    {
//        public long idTipoSeccion { get; set; }

//        public long idTipoTramite { get; set; }
//        public string Nombre { get; set; }

//        public Nullable<int> Imprime { get; set; }

//        public DateTime FechaAlta { get; set; }

//        public Nullable<DateTime> FechaModif { get; set; }

//        public Nullable<long> idUsuModif { get; set; }

//        public Nullable<long> idUsuBaja { get; set; }

//        public Nullable<DateTime> FechaBaja { get; set; }

//        public Nullable<long> idUsuAlta { get; set; }

//        public string Plantilla { get; set; }
//    }

//}