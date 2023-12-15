using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeUnidadTributaria : XtraReport
    {
        public InformeUnidadTributaria()
        {
            InitializeComponent();
        }

        /*private void DetailReport2_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var parcela = Report.GetCurrentRow() as Parcela;
            var nombre = parcela.Domicilios.ElementAt(e.CurrentRow).ViaNombre;
            var numero = parcela.Domicilios.ElementAt(e.CurrentRow).numero_puerta;
            XRTableCell xrTableCellDireccion = ((XRControl)sender).FindControl("xrTableCellDireccion", true) as XRTableCell;
            xrTableCellDireccion.Text = nombre + " " + numero;
        }*/
    }
}
