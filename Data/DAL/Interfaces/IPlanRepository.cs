using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IPlanRepository
    {
        IEnumerable<Plan> GetPlanes();
    }
}
