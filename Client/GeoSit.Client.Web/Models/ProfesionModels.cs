using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class ProfesionModels
    {
        public ProfesionModels()
        {
            DatosProfesion = new ProfesionModel();
        }
        public ProfesionModel DatosProfesion {get; set;}
    }

    public class ProfesionModel
    {
        public long PersonaId { get; set; }
        public long TipoProfesionId { get; set; }
        public string Matricula { get; set; }
    }


}