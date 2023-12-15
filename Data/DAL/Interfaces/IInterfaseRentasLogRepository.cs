using GeoSit.Data.BusinessEntities.InterfaseRentas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IInterfaseRentasLogRepository
    {
        InterfaseRentasLog GetById(long logId);

        IEnumerable<InterfaseRentasLog> GetAll();        

        void InsertLog(InterfaseRentasLog log);

        void UpdateLog(InterfaseRentasLog log);
    }
}
