using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface ILiquidacionRepository
    {
        IEnumerable<Liquidacion> GetLiquidaciones(long idExpedienteObra);

        //Liquidacion GetLiquidacionesReturn(long idExpediente);

        Liquidacion GetLiquidacionById(long idLiquidacion);

        void InsertLiquidacion(Liquidacion liquidacion, ExpedienteObra expedienteObra, UnidadTributariaExpedienteObra ut);

        void InsertLiquidacion(Liquidacion liquidacion);

        void UpdateLiquidacion(Liquidacion liquidacion);

        void DeleteLiquidacionesByExpedienteObraId(long idExpedienteObra);

        void DeleteLiquidacionById(long idLiquidacion);

        void DeleteLiquidacion(Liquidacion liquidacion);
    }
}
