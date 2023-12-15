using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDomicilioInmuebleRepository
    {
        IEnumerable<Domicilio> GetDomicilioInmuebles(string nombreVia);

        Domicilio GetDomicilioInmuebleById(long idDomicilioInmueble);

        void InsertDomicilio(Domicilio domicilio);

        void UpdateDomicilio(Domicilio domicilio);

        void DeleteDomicilio(Domicilio domicilio);

        Domicilio GetDomicilioByUnidadTributariaId(long idUnidadTributaria);

        IEnumerable<Domicilio> GetDomiciliosByUnidadTributariaId(long idUnidadTributaria);
    }
}
