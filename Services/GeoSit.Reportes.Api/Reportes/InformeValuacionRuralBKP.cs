using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using System.Linq;//

namespace GeoSit.Reportes.Api.Reportes
{
    public partial class InformeValuacionRuralBKP : DevExpress.XtraReports.UI.XtraReport
    {
        public InformeValuacionRuralBKP()
        {
            InitializeComponent();

           
        }

       internal void SetCaracteristicas(XRControl contenedor, string label, DDJJSorCar Caracteristica)
        {
            XRLabel lblCaracteristicas = (XRLabel)contenedor.FindControl(label, true);

                var Relieve = Caracteristica.AptCar.SorCaracteristica.Descripcion; //orderby si lo quiero ordenar antes del select, aca y en 
                lblCaracteristicas.Text = string.Join(", ", Relieve);   

        }

        private void DetailReport1_DataSourceRowChanged_1(object sender, DataSourceRowEventArgs e)
        {
            //ar Caracteristicas = ((((DetailReportBand)sender).DataSource as DDJJSor[])[0]).SorCar.ElementAt(e.CurrentRow); // devuleve la valuacion historica que esta procesando, deberia pasar 3 veces por tener tres valuaciones historicas
            //SetCaracteristicas((XRControl)sender, "lblCaras", Caracteristicas);
        }



        /*public InformePropiedad(AtributosDocumento atributoDoc):this()
{
numMensura.Text = $"{ atributoDoc.numero_plano }-{ atributoDoc.letra_plano }";
vigenciaMensura.Text = atributoDoc.fecha_vigencia_actual.ToString("dd/MM/yyyy");
}*/
    }
}
