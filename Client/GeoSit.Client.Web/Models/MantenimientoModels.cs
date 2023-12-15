using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using GeoSit.Data.BusinessEntities.Mantenimiento;

namespace GeoSit.Client.Web.Models
{
    public class MantenimientoModel
    {
        public MantenimientoModel()
        {
            TablasAuxiliares = new TablasAuxiliaresModel();
            Mensaje = "";

        }
        public TablasAuxiliaresModel TablasAuxiliares { get; set; }
        public List<ComponenteTA> ListaComponente { get; set; }
        public List<AtributoTA> ListaAtributo { get; set; }
        public AtributoTA IdAtributo { get; set; }
        public string Mensaje { get; set; }
    }

    public class TablasAuxiliaresModel
    {
        public List<string> ValoresTablas { get; set; }
        public List<string> CamposTablas { get; set; }
        public string ComponentesId { get; set; }
        public string TablaID { get; set; }
        public List<List<string>> CamposTablasAgregar { get; set; }
        public List<List<string>> ValoresTablasAgregar { get; set; }
        public long Id_Usuario { get; set; }

    }
}