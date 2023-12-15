using System.Collections.Generic;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Data.BusinessEntities.Valuaciones;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IMensuraSecuenciaRepository
    {
        void UpdateMensuraSecuencia(MensuraSecuencia mensura);
        void InsertMensuraSecuencia(MensuraSecuencia mensura);

    }
}