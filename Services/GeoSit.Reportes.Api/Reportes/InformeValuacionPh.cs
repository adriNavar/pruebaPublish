using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Globalization;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeValuacionPh : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeValuacionPh()
        {
            InitializeComponent();

           
        }

        private void DetailReport4_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            
            var UnidadTributaria = ((((DetailReportBand)sender).DataSource as UnidadTributaria[])[0]).Parcela.UnidadesTributarias.ElementAt(e.CurrentRow);

            XRLabel lblValorMejoras2 = ((XRControl)sender).FindControl("lblValorMejoras2", true) as XRLabel;
            lblValorMejoras2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", UnidadTributaria.UTValuaciones.ValorMejoras);

            XRLabel lblValorTierra2 = ((XRControl)sender).FindControl("lblValorTierra2", true) as XRLabel;
            lblValorTierra2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", UnidadTributaria.UTValuaciones.ValorTierra);

            XRLabel lblValorTotal2 = ((XRControl)sender).FindControl("lblValorTotal2", true) as XRLabel;
            lblValorTotal2.Text = String.Format(new CultureInfo("es-AR"), "{0:C}", UnidadTributaria.UTValuaciones.ValorTotal);

        }
    }
}
