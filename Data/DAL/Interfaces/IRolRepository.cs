using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IRolRepository
    {
        IEnumerable<Rol> GetRoles();
    }
}
