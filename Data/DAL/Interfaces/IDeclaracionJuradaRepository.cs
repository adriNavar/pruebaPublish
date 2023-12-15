using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.Via;
using System.Collections.Generic;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using GeoSit.Web.Api.Models;
using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using System;
using GeoSit.Data.BusinessEntities.Designaciones;
using GeoSit.Data.BusinessEntities.ObjetosAdministrativos;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IDeclaracionJuradaRepository
    {
        IEnumerable<DDJJVersion> GetVersiones();

        DDJJVersion GetVersion(int idVersion);
        Designacion GetDesignacionByUt(long idUnidadTributaria);

        IEnumerable<DDJJSorTipoCaracteristica> GetSorTipoCaracteristicas();

        IEnumerable<DDJJDominio> GetDominios(long idDeclaracionJurada);
        IEnumerable<DDJJDominio> GetDominiosByIdUnidadTributaria(long idUT);

        IEnumerable<TipoTitularidad> GetTiposTitularidad();

        IEnumerable<DDJJSorOtrasCar> GetSorOtrasCar(int idVersion);

        IEnumerable<DDJJUOtrasCar> GetUOtrasCar(int idVersion);

        INMMejora GetMejora(long idDeclaracionJurada);

        DDJJ GetDeclaracionJurada(long idDeclaracionJurada);

        DDJJDesignacion GetDDJJDesignacion(long idDeclaracionJurada);

        bool SaveDDJJSor(DDJJ ddjj, DDJJSor ddjjSor, DDJJDesignacion ddjjDesignacion, List<DDJJDominio> dominios, List<DDJJSorCar> sorCar, List<VALSuperficies> superficies, long idUsuario, string machineName, string ip);

        bool SaveDDJJU(DDJJ ddjj, DDJJU ddjjU, DDJJDesignacion ddjjDesignacion, List<DDJJDominio> dominios, long idUsuario, List<Web.Api.Models.ClaseParcela> clases, string machineName, string ip);

        bool SaveFormularioE1(DDJJ ddjj, INMMejora mejora, List<INMMejoraOtraCar> otrasCar, List<int> caracteristicas, long idUsuario, string machineName, string ip);

        bool SaveFormularioE2(DDJJ ddjj, INMMejora mejora, List<INMMejoraOtraCar> otrasCar, List<int> caracteristicas, long idUsuario, string machineName, string ip);


        METramite GetTramite(long idDeclaracionJurada);

        Mensura GetMensura(int idMensura);

        List<DDJJ> GetDeclaracionesJuradas(long idUnidadTributaria);

        List<DDJJ> GetDeclaracionesJuradasNoVigentes(long idUnidadTributaria);

        DDJJ GetDeclaracionJuradaVigenteU(long idUnidadTributaria);//

        DDJJ GetDeclaracionJuradaVigenteSoR(long idUnidadTributaria);//

        IEnumerable<OCObjeto> GetOCObjetos(int idSubtipoObjeto);

        List<VALAptitudes> GetAptitudes(int? idVersion = null);
        
        List<DDJJSorCaracteristicas> GetCaracteristicas();

        List<VALAptCar> GetAptCar();

        List<DDJJSorCar> GetSorCar(long idSor);

        List<VALSuperficies> GetValSuperficies(long idSor);

        List<INMOtraCaracteristica> GetInmOtrasCaracteristicas(long idVersion);

        List<INMDestinoMejora> GetDestinosMejoras(long idVersion);

        List<INMMejoraOtraCar> GetMejoraOtraCar(long idMejora);

        List<VALValuacion> GetValuaciones(long idUnidadTributaria);

        List<VALValuacion> GetValuacionesHistoricas(long idUnidadTributaria);

        VALValuacion GetValuacion(long idValuacion);

        bool DeleteValuacion(long idValuacion, long idUsuario);

        VALValuacion GetValuacionVigente(long idUnidadTributaria);

        bool SaveValuacion(VALValuacion valuacion, long idUsuario);

        VALDecreto GetDecretoByNumero(long nroDecreto);
        List<VALClasesParcelas> GetClasesParcelas();
        Tramite GetTramiteByNumero(long nroTramite);

        List<VALClasesParcelas> GetClasesParcelasFull();

        List<DDJJUFracciones> GetMedidaLineasFromFraccionByIdU(int idU);

        List<VALTiposMedidasLineales> GetTipoMedidaLineales();
        List<DDJJPersonaDomicilio> GetPersonaDomicilios(long idPersona);

        bool GenerarValuacion(DDJJ ddjj, long idUnidadTributaria, TipoValuacionEnum tipoValuacion, long idUsuario, string machineName, string ip);
        List<INMCaracteristica> GetInmCaracteristicas(long idVersion);
        List<INMInciso> GetInmIncisos(long idVersion);
        List<INMTipoCaracteristica> GetInmTipoCaracteristicas(long idVersion);
        List<INMMejoraCaracteristica> GetInmMejorasCaracteristicas(long idMejora);
        List<VALDecreto> GetDecretos();
        DecretoAplicado AplicarDecreto(long idDecreto, long idUsuario);
        bool GetAplicarDecretoIsRunning();
        string GetAplicarDecretoStatus();
        Via GetGrfVia(string calle);
        TramoVia GetGrfTramoVia(long idVia, int altura);
        OA.Objeto GetOAObjetoPorIdLocalidad(long idLocalidad);
        Aforo BuscarAforoAlgoritmo(long idLocalidad, string calle, long? idVia, int? altura);
        Aforo BuscarAforoPorId(long? idTramoVia, long? idVia);
        List<Aforo> BuscarAforosVia(IEnumerable<Tuple<long?, long?>> tramos_y_vias);
        
        List<EstadosConservacion> GetEstadosConservacion();

        DDJJ GetDeclaracionJuradaCompleta(long idDeclaracionJurada);
        bool Revaluar(long idUnidadTributaria, long idUsuario, string machineName, string ip);
        object ValoresAforoValido();
        void BajaMejoras(long idDDJJ, long idUsuario, string machineName, string ip);
        ValueTuple<bool, string> ValidarConsistencia(long idUnidadTributaria, long version);

        List<Objeto> GetLocalidadesByDistancia(long distanciaLocalidad);

        List<VALClasesParcelas> GetClasesParcelasBySuperficie(decimal superficie);

        string GetCroquisClaseParcela(int idClaseParcela);
        object GetClaseParcelaByIdDDJJ(long idDeclaracionJurada);

        long GetIdDepartamentoByCodigo(string codigo);
    }
}
