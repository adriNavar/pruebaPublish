using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaDomicilioRepository
    {
        void InsertUnidadTributariaDomicilioRepository(UnidadTributariaDomicilio unidadTributariaDomicilio);
        UnidadTributariaDomicilio GetUnidadTributariaDomicilioRepositoryById(long idUnidadTributaria, long idDomicilio);
        void DeleteUnidadTributariaDomicilio(UnidadTributariaDomicilio unidadTributariaDomicilio);
        IQueryable<UnidadTributariaDomicilio> GetUnidadTributuriaDombyId(long idUnidadTributaria);
        UnidadTributariaDomicilio GetUnidadTributuriaDombyIdDom(long idDom);

    }
}
