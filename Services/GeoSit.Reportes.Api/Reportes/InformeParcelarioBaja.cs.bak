using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeParcelarioBaja : XtraReport
    {
        public InformeParcelarioBaja()
        {
            InitializeComponent();
        }

        private void Detail3_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }

        private void DetailReportDesignacion_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var parcela = Report.GetCurrentRow() as Parcela;
            var designaciones = parcela.Designaciones.ElementAt(e.CurrentRow).IdDesignacion;
            XRLabel Label = ((XRControl)sender).FindControl("lblDesignacion", true) as XRLabel;

            if (parcela.Designaciones.Any())
            {

                var mensaje = "";

                if (designaciones != 0)
                {
                    var Departamento = parcela.Designaciones.ElementAt(e.CurrentRow).Departamento;
                    if (Departamento != null)
                    {

                        mensaje += "Departamento: " + Departamento + "  ";

                    }
                    var Localidad = parcela.Designaciones.ElementAt(e.CurrentRow).Localidad;
                    if (Localidad != null)
                    {

                        mensaje += "Localidad: " + Localidad + "  ";

                    }
                    var Paraje = parcela.Designaciones.ElementAt(e.CurrentRow).Paraje;
                    if (Paraje != null)
                    {
                        mensaje += "Paraje: " + Paraje + "  ";

                    }
                    var Calle = parcela.Designaciones.ElementAt(e.CurrentRow).Calle;
                    if (Calle != null)
                    {
                        mensaje += "Calle: " + Calle + "  ";

                    }
                    var Numero = parcela.Designaciones.ElementAt(e.CurrentRow).Numero;
                    if (Numero != null)
                    {
                        mensaje += "Número: " + Numero + "  ";

                    }
                    var Seccion = parcela.Designaciones.ElementAt(e.CurrentRow).Seccion;
                    if (Seccion != null)
                    {
                        mensaje += "Sección: " + Seccion + "  ";

                    }
                    var Chacra = parcela.Designaciones.ElementAt(e.CurrentRow).Chacra;
                    if (Chacra != null)
                    {
                        mensaje += "Chacra: " + Chacra + "  ";

                    }
                    var Quinta = parcela.Designaciones.ElementAt(e.CurrentRow).Quinta;
                    if (Quinta != null)
                    {
                        mensaje += "Quinta: " + Quinta + "  ";

                    }
                    var Fraccion = parcela.Designaciones.ElementAt(e.CurrentRow).Fraccion;
                    if (Fraccion != null)
                    {
                        mensaje += "Fracción: " + Fraccion + "  ";

                    }
                    var Manzana = parcela.Designaciones.ElementAt(e.CurrentRow).Manzana;
                    if (Manzana != null)
                    {

                        mensaje += "Manzana: " + Manzana + "  ";

                    }
                    var Lote = parcela.Designaciones.ElementAt(e.CurrentRow).Lote;
                    if (Lote != null)
                    {
                        mensaje += "Parcela: " + Lote + "  ";

                    }
                }
                Label.Text = mensaje;
            }
        }

        private void DetailReport3_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var parcela = Report.GetCurrentRow() as Parcela;
            var ut = parcela.UnidadesTributarias.ElementAt(e.CurrentRow);
            CellDominio.Text = ut.Dominios.SingleOrDefault()?.Inscripcion??string.Empty;
        }
    }
}
