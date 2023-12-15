using System.Web.Mvc;

namespace GeoSit.Client.Web.Models.ExpedientesObras
{
    public class PersonaViewModel
    {
        public SelectList RolList { get; set; }

        public long IdRol { get; set; }

        public long IdPersona { get; set; }

        public string Persona { get; set; }

        public string DomicilioFisico { get; set; }
    }
}