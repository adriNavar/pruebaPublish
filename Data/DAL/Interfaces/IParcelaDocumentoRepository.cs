using GeoSit.Data.BusinessEntities.Inmuebles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoSit.Data.BusinessEntities.Documentos;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParcelaDocumentoRepository
    {
        ParcelaDocumento GetParcelaDocumentoById(long idParcelaDocumento);
        void InsertParcelaDocumento(ParcelaDocumento parcelaDocumento);
        void DeleteParcelaDocumento(ParcelaDocumento parcelaDocumento);
        IEnumerable<AtributosDocumento> GetPlanoMensura(long idParcela);
    }
}
