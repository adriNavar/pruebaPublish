using System;
using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.ObrasParticulares;


namespace GeoSit.Data.DAL.Interfaces
{
    public interface IExpedienteObraRepository
    {
        IEnumerable<ExpedienteObra> GetExpedienteObras();

        List<ExpedienteObra> GetExpedienteObras(long unidadTributariaId, string numeroLegajoIni, string numeroLegajoFin,
            string numeroExpedienteIni, string numeroExpedienteFin, string fechaLegajoIni, string fechaLegajoFin,
            string fechaExpedienteIni, string fechaExpedienteFin, long personaId, long estadoId);

        string GetNumeroLegajoSiguiente();

        string GetNumeroExpedienteSiguiente();
        ExpedienteObra GetExpedienteObraByNumeroLegajo(string numeroLegajo);

        ExpedienteObra GetExpedienteObraByNumeroExpediente(string numeroExpediente);

        ExpedienteObra GetExpedienteObraById(long idExpedienteObra);

        INMCertificadoCatastral GetCertificadoCatastral(long idCertificadoCatastral);
        List<INMCertificadoCatastral> GetCertificadosCatastral();
        void InsertExpedienteObra(ExpedienteObra expedienteObra);
        void InsertCertificadoCatastral(INMCertificadoCatastral certificadoCatastral);

        void UpdateExpedienteObra(ExpedienteObra expedienteObra);

        void DeleteExpedienteObra(long idExpedienteObra, long idUsuarioBaja, DateTime fechaBaja);

        void DeleteExpedienteObra(ExpedienteObra expedienteObra);
    }
}
