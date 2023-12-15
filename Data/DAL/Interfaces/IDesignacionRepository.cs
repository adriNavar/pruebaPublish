using GeoSit.Data.BusinessEntities.Designaciones;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDesignacionRepository
    {
        Designacion GetDesignacion(long idParcela);
        IEnumerable<Designacion> GetDesignacionesByParcela(long id);
        IEnumerable<TipoDesignador> GetTiposDesignador();

        void DeleteDesignacion(Designacion designacion);
        void InsertDesignacion(Designacion designacion);
        void UpdateDesignacion(Designacion designacion);
    }
}
