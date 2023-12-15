using GeoSit.Data.BusinessEntities.InterfaseRentas;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class InterfaseRentasLogRepository : IInterfaseRentasLogRepository
    {
        private readonly GeoSITMContext _context;

        public InterfaseRentasLogRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public InterfaseRentasLog GetById(long logId)
        {
            return _context.InterfaseRentaLogs.Find(logId);
        }

        public IEnumerable<InterfaseRentasLog> GetAll()
        {
            return _context.InterfaseRentaLogs.ToArray();
        }

        public void InsertLog(InterfaseRentasLog log)
        {
            _context.InterfaseRentaLogs.Add(log);
        }

        public void UpdateLog(InterfaseRentasLog log)
        {
            _context.Entry(log).State = EntityState.Modified;
        }
    }
}
