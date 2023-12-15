using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace GeoSit.Client.Web.Models
{
    public class TipoMensuraModels
    {
        public TipoMensuraModels()
        {
            TiposMensuras = new TiposMensurasModel();
        }
        public TiposMensurasModel TiposMensuras { get; set; }
        public string Mensaje { get; set; }
    }

    public class TiposMensurasModel
    {
        public long IdTipoMensura { get; set; }
        public string Descripcion { get; set; }
        public long IdUsuarioAlta { get; set; }

        public DateTime FechaAlta { get; set; }
        public long IdUsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}