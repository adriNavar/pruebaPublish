using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using System.Linq;

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformePropiedad : DevExpress.XtraReports.UI.XtraReport
    {
        public InformePropiedad()
        {
            InitializeComponent();
        }

        private void DetailReport_DataSourceRowChanged(object sender, DataSourceRowEventArgs e)
        {
            var ut = Report.GetCurrentRow() as UnidadTributaria;
            var designaciones = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).IdDesignacion;
            XRLabel Label = ((XRControl)sender).FindControl("lblDesignacion", true) as XRLabel;

            if (ut.Parcela.Designaciones.Any())
            {

                var mensaje = "";

                if (designaciones != 0)
                {
                    var Departamento = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Departamento;
                    if (Departamento != null)
                    {

                        mensaje += "Departamento: " + Departamento + "  ";

                    }
                    var Localidad = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Localidad;
                    if (Localidad != null)
                    {

                        mensaje += "Localidad: " + Localidad + "  ";

                    }
                    var Paraje = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Paraje;
                    if (Paraje != null)
                    {
                        mensaje += "Paraje: " + Paraje + "  ";

                    }
                    var Calle = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Calle;
                    if (Calle != null)
                    {
                        mensaje += "Calle: " + Calle + "  ";

                    }
                    var Numero = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Numero;
                    if (Numero != null)
                    {
                        mensaje += "Número: " + Numero + "  ";

                    }
                    var Seccion = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Seccion;
                    if (Seccion != null)
                    {
                        mensaje += "Sección: " + Seccion + "  ";

                    }
                    var Chacra = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Chacra;
                    if (Chacra != null)
                    {
                        mensaje += "Chacra: " + Chacra + "  ";

                    }
                    var Quinta = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Quinta;
                    if (Quinta != null)
                    {
                        mensaje += "Quinta: " + Quinta + "  ";

                    }
                    var Fraccion = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Fraccion;
                    if (Fraccion != null)
                    {
                        mensaje += "Fracción: " + Fraccion + "  ";

                    }
                    var Manzana = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Manzana;
                    if (Manzana != null)
                    {

                        mensaje += "Manzana: " + Manzana + "  ";

                    }
                    var Lote = ut.Parcela.Designaciones.ElementAt(e.CurrentRow).Lote;
                    if (Lote != null)
                    {
                        mensaje += "Parcela: " + Lote + "  ";

                    }
                }
                Label.Text = mensaje;
            }
        }
        /*public InformePropiedad(AtributosDocumento atributoDoc):this()
{
   numMensura.Text = $"{ atributoDoc.numero_plano }-{ atributoDoc.letra_plano }";
   vigenciaMensura.Text = atributoDoc.fecha_vigencia_actual.ToString("dd/MM/yyyy");
}*/
    }
}
