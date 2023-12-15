using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas.DTO
{
    public class MERequisitoCumplido
    {
        public int IdObjetoRequisito { get; set; }
        public string Descripcion { get; set; }
        public int Cumplido { get; set; }
        public int Obligatorio { get; set; }
    }
}
