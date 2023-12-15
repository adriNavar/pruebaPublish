using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDominioTitularRepository
    {
        IEnumerable<Titular> GetDominioTitulares(long idDominio);

        IEnumerable<Titular> GetDominioTitularesFromView(long idDominio);

        DominioTitular GetDominioTitularById(long idDominio, long idPersona);

        IEnumerable<DominioTitular> GetDominioTitularByTitularId(long idTitular);

        void InsertDominioTitular(DominioTitular dominioTitular);

        void UpdateDominioTitular(DominioTitular dominioTitular);

        void DeleteDominioTitular(DominioTitular dominioTitular);
    }
}
