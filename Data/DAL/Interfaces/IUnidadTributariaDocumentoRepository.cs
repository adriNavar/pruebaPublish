using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IUnidadTributariaDocumentoRepository
    {
        UnidadTributariaDocumento GetUnidadTributariaDocumentoByID(long unidadTributariaDocumentoId);
        void InsertUnidadTributariaDocumento(UnidadTributariaDocumento unidadTributariaDocumento);
        void RemoveUnidadTributariaDocumento(UnidadTributariaDocumento unidadTributariaDocumento);
    }
}
