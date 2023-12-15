using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class AnalisisPredefinidoRepository : IAnalisisPredefinidoRepository
    {

        private readonly GeoSITMContext _context;

        public AnalisisPredefinidoRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Distritos> GetDistritos()
        {
            return _context.Distritos;
        }

        public void InsertReclamos(CargasTecnicas reclamoTecnico)
        {
            _context.CargasTecnicas.Add(reclamoTecnico);
        }

        public AnalisisTecnicos GetAnalisisTecnicoById(long idAnalisisTecnico)
        {
            return _context.AnalisisTecnicos.Find(idAnalisisTecnico);
        }

    }
}
