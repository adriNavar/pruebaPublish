using System;

namespace GeoSit.Client.Web.Models
{
    public class ParcelaGraficaModels
    {
        public ParcelaGraficaModels()
        {
            DatosParcelaGrafica = new ParcelaGraficaModel();
        }
        public ParcelaGraficaModel DatosParcelaGrafica { get; set; }

    }

    public class ParcelaGraficaModel
    {
        public long FeatID { get; set; }
        public long? ParcelaID { get; set; }
        public int? UsuarioAltaID { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? UsuarioModificacionID { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? UsuarioBajaID { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long ClassID { get; set; }
        public long RevisionNumber { get; set; }
    }
}