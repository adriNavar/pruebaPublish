using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class TipoProfesionModels
    {
        public TipoProfesionModels()
        {
            TiposProfesiones = new TiposProfesionesModel();
        }
        public TiposProfesionesModel TiposProfesiones { get; set; }
        public string Mensaje { get; set; }
    }

    public class TiposProfesionesModel
    {
        public long TipoProfesionId { get; set; }
        public string Descripcion { get; set; }
    }
}