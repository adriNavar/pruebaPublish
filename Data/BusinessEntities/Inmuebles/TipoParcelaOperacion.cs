using System.Collections.Generic;

namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class TipoParcelaOperacion
    {
        public long Id { get; set; }
        public string Descripcion { get; set; }

        //Propiedad de Navegación
        public ICollection<ParcelaOperacion> ParcelaOperaciones { get; set; }
    }
}
