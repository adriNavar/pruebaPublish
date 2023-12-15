using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Repositories
{
    public class TipoNomenclaturaRepository : ITipoNomenclaturaRepository
    {
        private readonly GeoSITMContext context;

        public TipoNomenclaturaRepository(GeoSITMContext context)
        {
            this.context = context;
        }
        public ICollection<TipoNomenclatura> GetTiposNomenclatura()
        {
            return context.TiposNomenclaturas.ToList();
        }
        public TipoNomenclatura GetTipoNomenclaturaById(long id)
        {
            return context.TiposNomenclaturas.FirstOrDefault(tn => tn.TipoNomenclaturaID == id);
        }
    }
}
