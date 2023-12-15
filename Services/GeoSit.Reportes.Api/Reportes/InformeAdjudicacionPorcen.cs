using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeAdjudicacionPorcen : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeAdjudicacionPorcen()
        {
            InitializeComponent();

           
        }


        /*public InformePropiedad(AtributosDocumento atributoDoc):this()
{
numMensura.Text = $"{ atributoDoc.numero_plano }-{ atributoDoc.letra_plano }";
vigenciaMensura.Text = atributoDoc.fecha_vigencia_actual.ToString("dd/MM/yyyy");
}*/
    }
}
