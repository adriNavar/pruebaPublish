using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeE1E2 : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeE1E2()
        {
            InitializeComponent();

           
        }

        private void DetailReport1_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var Mejorasotrascar = ((((DetailReportBand)sender).DataSource as UnidadTributaria[])[0]).DeclaracionJ.Mejora.SelectMany(a => a.OtrasCar);
            var otrascaracteristicas = Mejorasotrascar.ElementAt(e.CurrentRow).OtraCar.Descripcion;
            var mejorasotrascar = Mejorasotrascar.ElementAt(e.CurrentRow).Valor;
            XRLabel Label = ((XRControl)sender).FindControl("lblOtrasCar", false) as XRLabel;
            XRLabel Label1 = ((XRControl)sender).FindControl("lblValor", false) as XRLabel;
            Label.Text = string.Empty;
            Label1.Text = string.Empty;
            if (mejorasotrascar != null)
            {
                Label.Text = $"{otrascaracteristicas}:";
                Label1.Text = $"{mejorasotrascar}";
            }
            else
            {
                Label.Text= null;
            }
        }
    }
}
