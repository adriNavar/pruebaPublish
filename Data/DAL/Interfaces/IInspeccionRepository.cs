using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using System;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IInspeccionRepository
    {
        IEnumerable<InspeccionInspector> GetInspecciones(long idExpedienteObra);

        void DeleteInspeccion(InspeccionExpedienteObra inspeccionEO);

        void InsertInspeccion(InspeccionExpedienteObra inspeccionEO);

        void InsertInspeccion(InspeccionExpedienteObra inspeccionEO, ExpedienteObra expedienteObra);
        IEnumerable<InspeccionInspector> GetInspeccionesPorPeriodo(long[] idsUsuarios, DateTime fechaDesde, DateTime fechaHasta);
        IEnumerable<InspeccionInspector> GetInspeccionesPorTipo(int[] idsTipos, DateTime fechaDesde, DateTime fechaHasta);
        void EOUpdateInspeccionRelacionAlta(string inspeccionId, string numero, int tipo);
    }
}
