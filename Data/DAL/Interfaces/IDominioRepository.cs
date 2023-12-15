using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDominioRepository
    {
        IEnumerable<DominioUT> GetDominios(long idUnidadTributaria, bool esHistorico = false);
        IEnumerable<DominioUT> GetDominiosHistorico(long idUnidadTributaria); 
        IEnumerable<DominioUT> GetDominiosFromView(long idUnidadTributaria);

        Dominio GetDominioById(long idDominio, bool baja = true);

        Dominio GetDominio(long idUt, string inscripcion, bool baja = true);

        IEnumerable<Dominio> GetDominioByDominioTitular(List<DominioTitular> domTitular);

        void InsertDominio(Dominio dominio);

        void UpdateDominio(Dominio dominio);

        void DeleteDominiosByUnidadTributariaId(long idUnidadTributaria);

        void DeleteDominio(Dominio dominio);

        ICollection<Dominio> GetDominiosUFByParcela(long idParcela, bool esHistorico = false);
    }
}
