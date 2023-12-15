using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ITipoNomenclaturaRepository
    {
        ICollection<TipoNomenclatura> GetTiposNomenclatura();

        TipoNomenclatura GetTipoNomenclaturaById(long id);
    }
}
