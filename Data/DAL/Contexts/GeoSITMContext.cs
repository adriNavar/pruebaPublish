using GeoSit.Core.Logging;
using GeoSit.Core.Logging.Loggers;
using GeoSit.Data.BusinessEntities.Actas;
using GeoSit.Data.BusinessEntities.Generico;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.BusinessEntities.InterfaseRentas;
using GeoSit.Data.BusinessEntities.Mantenimiento;
using GeoSit.Data.BusinessEntities.Mapa;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.BusinessEntities.ObrasPublicas;
using GeoSit.Data.BusinessEntities.ReclamosDiarios;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.BusinessEntities.Via;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.Mappers.Oracle;
using GeoSit.Data.Mappers.Oracle.Interfaces;
using GeoSit.Data.Mappers.Oracle.MesaEntradas;
using GeoSit.Data.Mappers.Oracle.ObrasPublicas;
using GeoSit.Data.Mappers.Oracle.Ploteo;
using GeoSit.Data.Mappers.Oracle.ReclamosDiarios;
using GeoSit.Data.Mappers.Oracle.Seguridad;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Designaciones;
//using GeoSit.Data.BusinessEntities.Reportes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using DOCUMENTOS = GeoSit.Data.BusinessEntities.Documentos;
using OA = GeoSit.Data.BusinessEntities.ObjetosAdministrativos;
using PERSONAS = GeoSit.Data.BusinessEntities.Personas;
using SUBSISTEMASWEB = GeoSit.Data.BusinessEntities.SubSistemaWeb;

using Z.EntityFramework.Plus;
using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System.Linq;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.Mappers.Oracle.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Certificados;
using ValDB = GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.Mappers.Oracle.ValidacionesDB;
using GeoSit.Data.BusinessEntities.LogRPI;
using TEMP = GeoSit.Data.BusinessEntities.Temporal;
using TMP_MAPR = GeoSit.Data.Mappers.Oracle.Temporal;

namespace GeoSit.Data.DAL.Contexts
{
    public abstract class GeoSITMContext : DbContext, IDisposable
    {
        private enum FILTROS
        {
            BAJA_LOGICA
        }
        public static GeoSITMContext CreateContext(bool filtrarFechaBaja = true)
        {
            GeoSITMContext ctx = null;
            switch (ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["USE_CONNECTION"]].ProviderName.ToLower())
            {
                case "npgsql":
                    ctx = new PostgresGeoSIT();
                    break;
                case "oracle.manageddataaccess.client":
                    ctx = new ORAGeoSIT();
                    break;
            }
            if (filtrarFechaBaja)
            {
                ctx.Filter(FILTROS.BAJA_LOGICA).Enable();
            }
            return ctx;
        }
        public abstract ISQLQueryBuilder CreateSQLQueryBuilder();
        public abstract DbParameter CreateParameter(string name, object value);
        public abstract DbParameter CreateResultParameter(string name);

        private readonly LoggerManager _loggerManager = null;

        public virtual DbSet<ControlTecnico> ControlesTecnicos { get; set; }
        public virtual DbSet<DomicilioExpedienteObra> DomiciliosExpedienteObra { get; set; }
        public virtual DbSet<EstadoExpedienteObra> EstadosExpedienteObra { get; set; }
        public virtual DbSet<InspeccionExpedienteObra> InspeccionesExpedienteObra { get; set; }
        public virtual DbSet<PlanoExpedienteObra> PlanosExpedienteObra { get; set; }
        public virtual DbSet<UnidadTributariaExpedienteObra> UnidadesTributariasExpedienteObra { get; set; }
        public virtual DbSet<Liquidacion> Liquidaciones { get; set; }
        public virtual DbSet<LiquidacionExterna> LiquidacionesExternas { get; set; }
        public virtual DbSet<Rol> Roles { get; set; }
        public virtual DbSet<MenuItem> MenuItem { get; set; }
        public virtual DbSet<Componente> Componente { get; set; }
        public virtual DbSet<Atributo> Atributo { get; set; }
        public virtual DbSet<Jerarquia> Jerarquia { get; set; }
        public virtual DbSet<TipoOperacion> TipoOperacion { get; set; }
        public virtual DbSet<Agrupacion> Agrupacion { get; set; }
        public virtual DbSet<FileDescriptor> FileDescriptor { get; set; }
        public virtual DbSet<FileColumn> FileColumn { get; set; }
        public virtual DbSet<FileData> FileData { get; set; }
        public virtual DbSet<DatoExternoConfiguracion> DatoExternoConfiguracion { get; set; }
        public virtual DbSet<DatoExterno> DatoExterno { get; set; }
        public virtual DbSet<ConfiguracionFiltro> ConfiguracionFiltro { get; set; }
        public virtual DbSet<ConfiguracionFiltroGrafico> ConfiguracionFiltroGrafico { get; set; }
        public virtual DbSet<MapaTematicoConfiguracion> MapaTematicoConfiguracion { get; set; }
        public virtual DbSet<ConfiguracionRango> ConfiguracionRango { get; set; }
        public virtual DbSet<Coleccion> Coleccion { get; set; }
        public virtual DbSet<ColeccionComponente> ColeccionComponente { get; set; }
        public virtual DbSet<ObjetoResultado> ObjetoResultados { get; set; }
        public virtual DbSet<Predefinido> Predefinido { get; set; }



        //SEGURIDAD
        public virtual DbSet<ParametrosGenerales> ParametrosGenerales { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<UsuariosHist> UsuariosHist { get; set; }
        public virtual DbSet<TipoDoc> TipoDoc { get; set; }
        public virtual DbSet<UsuariosRegistro> UsuariosRegistro { get; set; }
        public virtual DbSet<UsuariosPerfiles> UsuariosPerfiles { get; set; }
        public virtual DbSet<UsuariosDistritos> UsuariosDistritos { get; set; }
        public virtual DbSet<Perfiles> Perfiles { get; set; }
        public virtual DbSet<PerfilesHist> PerfilesHist { get; set; }
        public virtual DbSet<Horarios> Horarios { get; set; }
        public virtual DbSet<HorariosDetalle> HorariosDetalle { get; set; }
        public virtual DbSet<Distritos> Distritos { get; set; }
        public virtual DbSet<Regiones> Regiones { get; set; }
        public virtual DbSet<Funciones> Funciones { get; set; }
        public virtual DbSet<SEEvento> SEEventos { get; set; }
        public virtual DbSet<SETipoOperacion> SETipoOperaciones { get; set; }
        public virtual DbSet<Entornos> Entornos { get; set; }
        public virtual DbSet<PerfilesFunciones> PerfilesFunciones { get; set; }
        public virtual DbSet<PerfilesComponentes> PerfilesComponentes { get; set; }
        public virtual DbSet<PerfilesUsuarios> PerfilesUsuarios { get; set; }
        public virtual DbSet<Feriados> Feriados { get; set; }
        public virtual DbSet<Auditoria> Auditoria { get; set; }
        public virtual DbSet<Sector> Sectores { get; set; }
        //FIN DE SEGURIDAD

        //OBRAS PUBLICAS
        public virtual DbSet<PLN_Atributo> PLN_Atributo { get; set; }
        public virtual DbSet<PLN_ZonaAtributo> ZonaAtributo { get; set; }
        public virtual DbSet<ObjetoInfraestructura> ObjetoInfraestructura { get; set; }
        public virtual DbSet<SubtipoObjetoInfraestructura> SubtipoObjetoInfraestructura { get; set; }
        public virtual DbSet<TipoObjetoInfraestructura> TipoObjetoInfraestructura { get; set; }
        public virtual DbSet<TramiteSeccion> Tramite_Seccion { get; set; }
        public virtual DbSet<Tramite> Tramites { get; set; }
        public virtual DbSet<TipoTramite> Tipo_Tramite { get; set; }
        public virtual DbSet<TramiteTipoSeccion> Tipo_Seccion { get; set; }
        public virtual DbSet<TramiteDocumento> Tramite_Documento { get; set; }
        public virtual DbSet<TramitePersona> Tramite_Persona { get; set; }
        public virtual DbSet<TramiteUnidadTributaria> Tramite_UTS { get; set; }
        public virtual DbSet<TramiteRol> Tramite_Rol { get; set; }
        public virtual DbSet<TramitePermisos> Tramite_Permiso { get; set; }
        //FIN DE OBRAS PUBLICAS
        public virtual DbSet<OA.Division> Divisiones { get; set; }
        //NUEVO ENTIDAD CARGASTECNICAS
        public virtual DbSet<CargasTecnicas> CargasTecnicas { get; set; }
        public virtual DbSet<AnalisisTecnicos> AnalisisTecnicos { get; set; }

        public virtual DbSet<OA.Objeto> Objetos { get; set; }
        public virtual DbSet<OA.TipoDivision> TiposDivision { get; set; }
        public virtual DbSet<OA.TipoObjeto> TiposObjeto { get; set; }
        public virtual DbSet<MapLayer> MapLayers { get; set; }

        // FUNCIONES GENERALES 
        public virtual DbSet<OA.TipoProfesion> TipoProfesion { get; set; }
        public virtual DbSet<OA.TipoDomicilio> TipoDomicilio { get; set; }
        public virtual DbSet<DOCUMENTOS.TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<OA.Pais> Pais { get; set; }
        public virtual DbSet<OA.Provincia> Provincia { get; set; }
        public virtual DbSet<OA.Localidad> Localidad { get; set; }
        public virtual DbSet<OA.Nacionalidad> Nacionalidad { get; set; }
        public virtual DbSet<PERSONAS.Profesion> Profesion { get; set; }
        public virtual DbSet<PERSONAS.Persona> Persona { get; set; }
        public virtual DbSet<DOCUMENTOS.Documento> Documento { get; set; }
        public virtual DbSet<OA.Domicilio> Domicilios { get; set; }
        public virtual DbSet<PERSONAS.PersonaDomicilio> PersonaDomicilio { get; set; }
        public virtual DbSet<Via> Via { get; set; }
        public virtual DbSet<TipoVia> TiposVias { get; set; }
        public virtual DbSet<TramoVia> TramoVia { get; set; }
        public virtual DbSet<ParcelaOperacion> ParcelaOperacion { get; set; }
        public virtual DbSet<TipoParcelaOperacion> TipoParcelaOperacion { get; set; }
        public virtual DbSet<Mensura> Mensura { get; set; }
        public virtual DbSet<ParcelaMensura> ParcelaMensura { get; set; }
        public virtual DbSet<TipoMensura> TipoMensura { get; set; }
        public virtual DbSet<EstadoMensura> EstadoMensura { get; set; }
        public virtual DbSet<MensuraDocumento> MensuraDocumento { get; set; }
        public virtual DbSet<MensuraRelacionada> MensurasRelacionadas { get; set; }
        public virtual DbSet<OA.JurisdiccionLocalidad> JurisdiccionLocalidad { get; set; }
        //public virtual DbSet<ReporteParcelario> ReporteParcelario { get; set; }

        // FIN FUNCIONES GENERALES 

        //SUB SISTEMAS WEB
        public virtual DbSet<SUBSISTEMASWEB.WebInstructivo> WebInstructivo { get; set; }

        public virtual DbSet<SUBSISTEMASWEB.WebLinks> WebLinks { get; set; }

        public virtual DbSet<SUBSISTEMASWEB.AyudaLinea> AyudaLinea { get; set; }

        //FIN SUBSISTEMAS WEB

        public virtual DbSet<ComponenteConfiguracion> ComponenteConfiguracion { get; set; }

        //Ploteo
        public DbSet<Parcela> Parcelas { get; set; }
        public DbSet<PartidaSecuencia> PartidaSecuencias { get; set; }
        public DbSet<MensuraSecuencia> MensuraSecuencias { get; set; }
        public DbSet<Plantilla> Plantillas { get; set; }
        public DbSet<Layer> Layers { get; set; }
        public DbSet<PlantillaEscala> PlantillaEscalas { get; set; }
        public DbSet<PlantillaFondo> PlantillaFondos { get; set; }
        public DbSet<Hoja> Hojas { get; set; }
        public DbSet<Resolucion> Resoluciones { get; set; }
        public DbSet<Norte> Nortes { get; set; }
        public DbSet<PlantillaTexto> PlantillaTextos { get; set; }
        public DbSet<FuncionAdicional> FuncionAdicionales { get; set; }
        public DbSet<FuncAdicParametro> FuncAdicParametros { get; set; }
        public DbSet<PlantillaCategoria> PlantillaCategorias { get; set; }
        public DbSet<ImagenSatelital> ImagenesSatelitales { get; set; }

        public virtual DbSet<EstadoInspeccion> EstadosInspeccion { get; set; }
        public virtual DbSet<TipoInspeccion> TiposInspeccion { get; set; }
        public virtual DbSet<Inspector> Inspectores { get; set; }
        public virtual DbSet<Inspeccion> Inspecciones { get; set; }
        public virtual DbSet<InspectorTipoInspeccion> InspectorTipoInspeccion { get; set; }
        public virtual DbSet<InspeccionUnidadesTributarias> InspeccionUnidadesTributarias { get; set; }
        public virtual DbSet<InspeccionDocumento> InspeccionDocumentos { get; set; }
        public virtual DbSet<TipoParcela> TiposParcela { get; set; }
        public virtual DbSet<ClaseParcela> ClasesParcela { get; set; }
        public virtual DbSet<OrigenParcela> OrigenesParcela { get; set; }
        public virtual DbSet<EstadoParcela> EstadosParcela { get; set; }
        public virtual DbSet<UnidadTributaria> UnidadesTributarias { get; set; }
        public virtual DbSet<TipoUnidadTributaria> TiposUnidadTributaria { get; set; }
        public virtual DbSet<Valuacion> Valuaciones { get; set; }
        public virtual DbSet<VIRInmueble> VirInmuebles { get; set; }
        public virtual DbSet<VIRValuacion> VirValuaciones { get; set; }
        public virtual DbSet<VIRVbEuUsos> VIRVbEuUsos { get; set; }
        public virtual DbSet<VIRVbEuCoefEstado> VIRVbEuCoefEstados { get; set; }
        public virtual DbSet<VIRVbEuTipoEdif> VIRVbEuTipoEdifs { get; set; }
        public virtual DbSet<VIREquivInmDestinosMejoras> VIREquivInmDestinosMejoras { get; set; }

        public virtual DbSet<EstadoExpediente> EstadosExpediente { get; set; }
        public virtual DbSet<ExpedienteObra> ExpedientesObra { get; set; }
        public virtual DbSet<INMCertificadoCatastral> INMCertificadosCatastrales { get; set; }
        public virtual DbSet<Plan> Planes { get; set; }
        public virtual DbSet<ActaPersonas> ActaPersonas { get; set; }
        public virtual DbSet<ActaDocumento> ActaDocumentos { get; set; }
        public virtual DbSet<ActaActaRel> ActaActaRels { get; set; }
        public virtual DbSet<ActaObjeto> ActaObjeto { get; set; }
        public virtual DbSet<ActaRolPersona> ActaRolPersona { get; set; }
        public virtual DbSet<Acta> Actas { get; set; }
        public virtual DbSet<ActaTipo> ActaTipos { get; set; }
        public virtual DbSet<ActaUnidadTributaria> ActaUnidadesTributarias { get; set; }
        public virtual DbSet<ActaEstado> ActaEstados { get; set; }
        public virtual DbSet<ActaDomicilio> ActaDomicilios { get; set; }
        public virtual DbSet<EstadoActa> EstadoActas { get; set; }
        public virtual DbSet<TipoSuperficie> TiposSuperficie { get; set; }
        public virtual DbSet<TipoSuperficieExpedienteObra> TipoSuperficieExpedienteObras { get; set; }
        public virtual DbSet<TipoExpediente> TiposExpediente { get; set; }
        public virtual DbSet<TipoExpedienteObra> TipoExpedienteObras { get; set; }
        public virtual DbSet<Destino> Destinos { get; set; }
        public virtual DbSet<Servicio> Servicios { get; set; }
        public virtual DbSet<ServicioExpedienteObra> ServicioExpedienteObras { get; set; }
        public virtual DbSet<ExpedienteObraDocumento> ExpedienteObraDocumentos { get; set; }
        public virtual DbSet<ObservacionExpediente> ObservacionExpedientes { get; set; }
        public virtual DbSet<PersonaExpedienteObra> PersonasExpedienteObra { get; set; }
        public virtual DbSet<TipoNomenclatura> TiposNomenclaturas { get; set; }
        public virtual DbSet<UnidadTributariaDocumento> UnidadesTributariasDocumento { get; set; }
        public virtual DbSet<ParcelaDocumento> ParcelasDocumentos { get; set; }
        public virtual DbSet<UnidadTributariaDomicilio> UnidadesTributariasDomicilio { get; set; }
        public virtual DbSet<Mejora> Mejora { get; set; }
        public virtual DbSet<ValuacionPadronDetalle> ValuacionPadronDetalle { get; set; }
        public virtual DbSet<ValuacionPadronCabecera> ValuacionPadronCabecera { get; set; }
        public virtual DbSet<TipoValorBasicoTierra> TipoValorBasicoTierra { get; set; }
        public virtual DbSet<ParcelaGrafica> ParcelaGrafica { get; set; }
        public virtual DbSet<Nomenclatura> Nomenclaturas { get; set; }
        public virtual DbSet<TipoPersona> TipoPersonas { get; set; }
        public virtual DbSet<UnidadTributariaPersona> UnidadTributariaPersonas { get; set; }
        public virtual DbSet<Dominio> Dominios { get; set; }
        public virtual DbSet<TipoInscripcion> TipoInscripciones { get; set; }
        public virtual DbSet<DominioTitular> DominioTitulares { get; set; }
        public virtual DbSet<TipoTitularidad> TiposTitularidad { get; set; }
        public virtual DbSet<PlantillaViewport> PlantillasViewports { get; set; }
        public virtual DbSet<LayerViewport> LayersViewports { get; set; }
        public virtual DbSet<PloteoFrecuenteEspecial> PloteosFrecuentesEspeciales { get; set; }
        public virtual DbSet<PloteoFrecuenteGeometria> PloteoFrecuenteGeometrias { get; set; }
        public virtual DbSet<TipoPlano> TipoPlanos { get; set; }
        public virtual DbSet<Censo> Censos { get; set; }
        public virtual DbSet<Partido> Partidos { get; set; }
        public virtual DbSet<Manzana> Manzanas { get; set; }

        //Reclamos Diarios
        public virtual DbSet<Reclamos_Actualizacion> Reclamos_Actualizacion { get; set; }
        public virtual DbSet<Reclamos_ReclamosDiarios> Reclamos_ReclamosDiarios { get; set; }
        public virtual DbSet<Reclamos_Tipo> Reclamos_Tipo { get; set; }
        public virtual DbSet<Reclamos_Motivo> Reclamos_Motivo { get; set; }
        public virtual DbSet<Reclamos_Clase> Reclamos_Clase { get; set; }
        public virtual DbSet<UbicacionPloteo> Reclamos_UbicacionPloteo { get; set; }

        // INICIO VALUACION
        public virtual DbSet<Coeficientes> Coeficientes { get; set; }
        public virtual DbSet<CoeficientesMejora> CoeficientesMejora { get; set; }
        public virtual DbSet<CoeficientesTierra> CoeficientesTierra { get; set; }
        public virtual DbSet<TipoCoeficientes> TipoCoeficientes { get; set; }
        public virtual DbSet<TipoValorBasicoMejora> TipoValorBasicoMejora { get; set; }
        public virtual DbSet<ValorBasicoMejora> ValorBasicoMejora { get; set; }
        public virtual DbSet<ValorBasicoTierraModulo> ValorBasicoTierraModulo { get; set; }
        public virtual DbSet<ValorBasicoTierraStore> ValorBasicoTierraStore { get; set; }
        public virtual DbSet<ValorBasicoMejoraStore> ValorBasicoMejoraStore { get; set; }
        public virtual DbSet<ValorBasicoTierraSuperficie> ValorBasicoTierraSuperficie { get; set; }
        public virtual DbSet<ValuacionPadronDetalleTemporal> ValuacionPadronDetalleTemporal { get; set; }
        public virtual DbSet<TipoValuacion> TipoValuacion { get; set; }
        public virtual DbSet<CoeficientesStore> CoeficientesStore { get; set; }
        // FIN VALUACION
        public virtual DbSet<EjesVia> EjesVia { get; set; }
        public virtual DbSet<EstadosConservacion> EstadosConservacion { get; set; }
        public virtual DbSet<TiposMejoras> TiposMejoras { get; set; }

        //INICIO MANTENIMIENTO
        public virtual DbSet<ComponenteTA> ComponenteTA { get; set; }
        public virtual DbSet<AtributoTA> AtributoTA { get; set; }
        //FIN MANTENIMIENTO

        public virtual DbSet<UsuariosActivos> UsuariosActivos { get; set; }

        //INICIO INTERFACES
        public virtual DbSet<TransaccionesPendientes> TransaccionesPendientes { get; set; }
        public virtual DbSet<InterfacesPadronTemp> InterfacesPadronTemp { get; set; }
        public virtual DbSet<InterfaseRentasLog> InterfaseRentaLogs { get; set; }

        //FIN INTERFACES

        //MESA ENTRADAS
        public virtual DbSet<METramite> TramitesMesaEntrada { get; set; }
        public virtual DbSet<MEPrioridadTramite> PrioridadesTramites { get; set; }
        public virtual DbSet<METipoTramite> TiposTramites { get; set; }
        public virtual DbSet<MERequisito> Requisitos { get; set; }
        public virtual DbSet<MEObjetoRequisito> ObjetosRequisitos { get; set; }
        public virtual DbSet<MEObjetoTramite> ObjetosTramites { get; set; }
        public virtual DbSet<MEEstadoTramite> EstadosTramites { get; set; }
        public virtual DbSet<MEMovimiento> MovimientosTramites { get; set; }
        public virtual DbSet<MERemito> Remitos { get; set; }
        public virtual DbSet<METipoMovimiento> TiposMovimientos { get; set; }
        public virtual DbSet<METramiteRequisito> TramitesRequisitos { get; set; }
        public virtual DbSet<METramiteDocumento> TramitesDocumentos { get; set; }
        public virtual DbSet<MEDesglose> Desgloses { get; set; }
        public virtual DbSet<MEDesgloseDestino> DesglosesDestinos { get; set; }
        public virtual DbSet<MEEntrada> Entradas { get; set; }
        public virtual DbSet<METramiteEntrada> TramitesEntradas { get; set; }
        public virtual DbSet<METramiteEntradaRelacion> TramitesEntradasRelacion { get; set; }
        public virtual DbSet<MEObjetoEntrada> ObjetosEntrada { get; set; }
        public virtual DbSet<MEComprobantePago> ComprobantePago { get; set; }


        //FIN MESA ENTRADAS

        //DECLARACIONES JURADAS

        public virtual DbSet<DDJJ> DDJJ { get; set; }
        public virtual DbSet<DDJJDesignacion> DDJJDesignacion { get; set; }
        public virtual DbSet<Designacion> Designacion { get; set; }
        public virtual DbSet<TipoDesignador> TiposDesignador { get; set; }
        public virtual DbSet<DDJJDominio> DDJJDominios { get; set; }
        public virtual DbSet<DDJJDominioTitular> DDJJDominioTitulares { get; set; }
        public virtual DbSet<DDJJOrigen> DDJJOrigen { get; set; }
        public virtual DbSet<DDJJPersonaDomicilio> DDJJPersonaDomicilios { get; set; }
        public virtual DbSet<DDJJSor> DDJJSor { get; set; }
        public virtual DbSet<DDJJSorCar> DDJJSorCar { get; set; }
        public virtual DbSet<DDJJSorCaracteristicas> DDJJSorCaracteristicas { get; set; }
        public virtual DbSet<DDJJSorOtrasCar> DDJJSorOtrasCar { get; set; }
        public virtual DbSet<DDJJSorTipoCaracteristica> DDJJSorTipoCaracteristica { get; set; }
        public virtual DbSet<DDJJVersion> DDJJVersion { get; set; }
        public virtual DbSet<VALAptitudes> VALAptitudes { get; set; }
        public virtual DbSet<VALSuperficies> VALSuperficies { get; set; }
        public virtual DbSet<VALAptCar> VALAptCar { get; set; }
        public virtual DbSet<OCObjeto> OCObjeto { get; set; }

        public virtual DbSet<DDJJU> DDJJU { get; set; }
        public virtual DbSet<DDJJUOtrasCar> DDJJUOtrasCar { get; set; }
        public virtual DbSet<DDJJUFracciones> DDJJUFracciones { get; set; }
        public virtual DbSet<DDJJUMedidaLineal> DDJJUMedidaLineal { get; set; }
        public virtual DbSet<VALClasesParcelas> VALClasesParcelas { get; set; }
        public virtual DbSet<VALClasesParcelasMedidaLineal> VALClasesParcelasMedidaLineal { get; set; }
        public virtual DbSet<VALTiposMedidasLineales> VALTiposMedidasLineales { get; set; }

        public virtual DbSet<INMCaracteristica> INMCaracteristicas { get; set; }
        public virtual DbSet<INMDestinoMejora> INMDestinoMejoras { get; set; }
        public virtual DbSet<INMInciso> INMIncisos { get; set; }
        public virtual DbSet<INMMejoraCaracteristica> INMMejoraCaracteristica { get; set; }
        public virtual DbSet<INMMejora> INMMejora { get; set; }
        public virtual DbSet<INMTipoCaracteristica> INMTipoCaracteristicas { get; set; }
        public virtual DbSet<INMMejoraOtraCar> INMMejoraOtraCar { get; set; }
        public virtual DbSet<INMOtraCaracteristica> INMOtraCaracteristica { get; set; }
        public virtual DbSet<INMLibreDeDeuda> INMLibreDeDeuda { get; set; }

        public virtual DbSet<VALDecreto> ValDecretos { get; set; }
        public virtual DbSet<VALDecretoZona> VALDecretoZona { get; set; }
        public virtual DbSet<VALDecretoJurisdiccion> VALDecretoJurisdiccion { get; set; }
        public virtual DbSet<VALValuacion> VALValuacion { get; set; }
        public virtual DbSet<VALValuacionDecreto> VALValuacionDecreto { get; set; }

        public virtual DbSet<VALCoefMenor2k> VALCoefMenor2k { get; set; }
        public virtual DbSet<VALCoefEsquinaMenor900> VALCoefEsquinaMenor900 { get; set; }
        public virtual DbSet<VALCoef2a15k> VALCoef2a15k { get; set; }
        public virtual DbSet<VALCoefMayor15k> VALCoefMayor15k { get; set; }
        public virtual DbSet<VALCoefTriangFrente> VALCoefTriangFrente { get; set; }
        public virtual DbSet<VALCoefTriangVertice> VALCoefTriangVertice { get; set; }

        public virtual DbSet<VALPuntajesCaminos> VALPuntajesCaminos { get; set; }
        public virtual DbSet<VALPuntajesEmbarques> VALPuntajesEmbarques { get; set; }
        public virtual DbSet<VALPuntajesLocalidades> VALPuntajesLocalidades { get; set; }

        public virtual DbSet<VALValoresOptRurales> VALValoresOptRurales { get; set; }
        public virtual DbSet<VALValoresOptSuburbanos> VALValoresOptSuburbanos { get; set; }
        public virtual DbSet<VALCoeficientesIncisos> VALCoeficientesIncisos { get; set; }
        public virtual DbSet<VALCoeficientesOtrasCar> VALCoeficientesOtrasCar { get; set; }
        public virtual DbSet<VALCoefDepreciacion> VALCoefDepreciacion { get; set; }
        public virtual DbSet<VALCoeficientesProrrateo> VALCoeficientesProrrateo { get; set; }
        //FIN DECLARACIONES JURADAS

        #region Validaciones Genericas DB
        public DbSet<ValDB.Validacion> Validaciones { get; set; }
        public DbSet<ValDB.ValidacionSubtipo> ValidacionesSubtipos { get; set; }
        public DbSet<ValDB.ValidacionFuncion> ValidacionesFunciones { get; set; }
        public DbSet<ValDB.ValidacionParametro> ValidacionesParametros { get; set; }
        public DbSet<ValDB.ValidacionGrupoFuncion> ValidacionesGruposFunciones { get; set; }
        #endregion

        //LOG
        public virtual DbSet<RPILogRespuestas> RPILogRespuestas { get; set; }
        public virtual DbSet<RPITipoOperacion> RPITipoOperacion { get; set; }
        public virtual DbSet<RPILogConsultas> RPILogConsultas { get; set; }
        //FINLOG

        #region Tablas Temporales
        public virtual DbSet<TEMP.DDJJTemporal> DeclaracionesJuradasTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJSorTemporal> DeclaracionesJuradasSoRTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJSorCarTemporal> DeclaracionesJuradasSoRCarsTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJUTemporal> DeclaracionesJuradasUTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJUMedidaLinealTemporal> DeclaracionesJuradasUMedidasLinealesTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJDesignacionTemporal> DeclaracionesJuradasDesignacionesTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJDominioTemporal> DeclaracionesJuradasDominiosTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJDominioTitularTemporal> DeclaracionesJuradasDominiosTitularesTemporal { get; set; }
        public virtual DbSet<TEMP.DDJJPersonaDomicilioTemporal> DeclaracionesJuradasPersonasDomiciliosTemporal { get; set; }
        public virtual DbSet<TEMP.INMMejoraCaracteristicaTemporal> INMMejorasCaracteristicasTemporal { get; set; }
        public virtual DbSet<TEMP.INMMejoraOtraCarTemporal> INMMejorasOtrasCarTemporal { get; set; }
        public virtual DbSet<TEMP.INMMejoraTemporal> MejorasTemporal { get; set; }
        public virtual DbSet<TEMP.DesignacionTemporal> DesignacionesTemporal { get; set; }
        public virtual DbSet<TEMP.DominioTemporal> DominiosTemporal { get; set; }
        public virtual DbSet<TEMP.MensuraTemporal> MensurasTemporal { get; set; }
        public virtual DbSet<TEMP.NomenclaturaTemporal> NomenclaturasTemporal { get; set; }
        public virtual DbSet<TEMP.ParcelaTemporal> ParcelasTemporal { get; set; }
        public virtual DbSet<TEMP.ParcelaOperacionTemporal> ParcelasOperacionesTemporal { get; set; }
        public virtual DbSet<TEMP.UnidadTributariaTemporal> UnidadesTributariasTemporal { get; set; }
        public virtual DbSet<TEMP.DominioTitularTemporal> DominiosTitularesTemporal { get; set; }
        public virtual DbSet<TEMP.MensuraRelacionadaTemporal> MensurasRelacionadasTemporal { get; set; }
        public virtual DbSet<TEMP.ParcelaMensuraTemporal> ParcelaMensurasTemporal { get; set; }
        public virtual DbSet<TEMP.VALValuacionTemporal> ValuacionesTemporal { get; set; }
        public virtual DbSet<TEMP.DivisionTemporal> DivisionesTemporal { get; set; }
        public virtual DbSet<TEMP.INMCertificadoCatastralTemporal> INMCertificadosCatastralesTemporal { get; set; }
        public virtual DbSet<TEMP.INMLibreDeDeudaTemporal> INMLibresDeDeudasTemporal { get; set; }
        public virtual DbSet<TEMP.EspacioPublicoTemporal> EspaciosPublicosTemporal { get; set; }
        #endregion

        public GeoSITMContext(string connection)
            : base(connection)
        {
            _loggerManager = new LoggerManager();
            _loggerManager.Add(new Log4NET(ConfigurationManager.AppSettings["log4net.config"].ToString(), "DefaultLogger", "ErrorLogger"));

            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Database.Log = _loggerManager.LogInfo;

            this.DefineQueryFilters();
        }

        internal LoggerManager GetLogger()
        {
            return _loggerManager;
        }

        public int SaveChanges(List<Auditoria> auditorias)
        {
            int grabo = base.SaveChanges();
            if (auditorias != null)
            {
                try
                {
                    Auditoria.AddRange(auditorias);
                    base.SaveChanges();
                }
                catch (Exception e)
                {
                    this.GetLogger().LogError("SaveChanges(List<Auditoria>)", e);
                }
            }
            return grabo;
        }
        public int SaveChanges(Auditoria auditoria)
        {
            int grabo = 0;
            try
            {
                grabo = base.SaveChanges();
            }
            catch(DbException e)
            {
                this.GetLogger().LogError("SaveChanges(Auditoria)", e);
                return grabo;
            }
            if (auditoria != null)
            {
                try
                {
                    Auditoria.Add(auditoria);
                    base.SaveChanges();
                }
                catch (Exception e)
                {
                    this.GetLogger().LogError("SaveChanges(Auditoria)", e);
                }
            }
            return grabo;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(ConfigurationManager.AppSettings["DATABASE"].ToString());

            modelBuilder.Configurations.Add(new PlanoExpedienteObraMapper());
            modelBuilder.Configurations.Add(new RolMapper());
            modelBuilder.Configurations.Add(new MenuItemMapper());
            modelBuilder.Configurations.Add(new ComponenteMapper());
            modelBuilder.Configurations.Add(new AtributoMapper());
            modelBuilder.Configurations.Add(new JerarquiaMapper());
            modelBuilder.Configurations.Add(new TipoOperacionMapper());
            modelBuilder.Configurations.Add(new AgrupacionMapper());
            modelBuilder.Configurations.Add(new FileDescriptorMapper());
            modelBuilder.Configurations.Add(new FileColumnMapper());
            modelBuilder.Configurations.Add(new FileDataMapper());
            modelBuilder.Configurations.Add(new DatoExternoConfiguracionMapper());
            modelBuilder.Configurations.Add(new DatoExternoMapper());
            modelBuilder.Configurations.Add(new ConfiguracionFiltroGraficoMapper());
            modelBuilder.Configurations.Add(new MapaTematicoConfiguracionMapper());
            modelBuilder.Configurations.Add(new ConfiguracionRangoMapper());
            modelBuilder.Configurations.Add(new ColeccionMapper());
            modelBuilder.Configurations.Add(new ColeccionComponenteMapper());
            modelBuilder.Configurations.Add(new ObjetoResultadoMapper());
            modelBuilder.Configurations.Add(new ConfiguracionFiltroMapper());
            modelBuilder.Configurations.Add(new PredefinidoMapper());

            //SEGURIDAD
            modelBuilder.Configurations.Add(new ParametrosGeneralesMapper());
            modelBuilder.Configurations.Add(new UsuariosMapper());
            modelBuilder.Configurations.Add(new UsuariosHistMapper());
            modelBuilder.Configurations.Add(new TipoDocMapper());
            modelBuilder.Configurations.Add(new UsuariosRegistroMapper());
            modelBuilder.Configurations.Add(new UsuariosPerfilesMapper());
            modelBuilder.Configurations.Add(new UsuariosDistritosMapper());
            modelBuilder.Configurations.Add(new PerfilesMapper());
            modelBuilder.Configurations.Add(new FuncionesMapper());
            modelBuilder.Configurations.Add(new EntornosMapper());
            modelBuilder.Configurations.Add(new HorariosMapper());
            modelBuilder.Configurations.Add(new HorariosDetalleMapper());
            modelBuilder.Configurations.Add(new PerfilesHistMapper());
            modelBuilder.Configurations.Add(new PerfilesFuncionesMapper());
            modelBuilder.Configurations.Add(new PerfilesComponentesMapper());
            modelBuilder.Configurations.Add(new PerfilesUsuariosMapper());
            modelBuilder.Configurations.Add(new FeriadosMapper());
            modelBuilder.Configurations.Add(new AuditoriaMapper());
            modelBuilder.Configurations.Add(new SETipoOperacionesMapper());
            modelBuilder.Configurations.Add(new SEEventosMapper());
            //FIN DE SEGURIDAD

            //OBRAS PUBLICAS
            modelBuilder.Configurations.Add(new PLN_ZonaAtributoMapper());
            modelBuilder.Configurations.Add(new PLN_AtributoMapper());
            modelBuilder.Configurations.Add(new ObjetoInfraestructuraMapper());
            modelBuilder.Configurations.Add(new SubtipoObjetoInfraestructuraMapper());
            modelBuilder.Configurations.Add(new TipoObjetoInfraestructuraMapper());
            modelBuilder.Configurations.Add(new TRT_Tramite_SeccionMapper());
            modelBuilder.Configurations.Add(new TRT_TramiteMapper());
            modelBuilder.Configurations.Add(new TRT_Tipo_TramiteMapper());
            modelBuilder.Configurations.Add(new TRT_Tipo_SeccionMapper());
            modelBuilder.Configurations.Add(new TRT_Tramite_DocumentoMapper());
            modelBuilder.Configurations.Add(new TRT_Tramite_PersonaMapper());
            modelBuilder.Configurations.Add(new TRT_Tramite_UtsMapper());
            modelBuilder.Configurations.Add(new TRT_RolMapper());
            //FIN DE OBRAS PUBLICAS
            modelBuilder.Configurations.Add(new DivisionAdministrativaMapper());
            modelBuilder.Configurations.Add(new TipoDivisionAdministrativaMapper());
            modelBuilder.Configurations.Add(new ObjetoAdministrativoMapper());
            modelBuilder.Configurations.Add(new TipoObjetoAdministrativoMapper());
            modelBuilder.Configurations.Add(new MapLayerMapper());
            // FUNCIONES GENERALES
            modelBuilder.Configurations.Add(new TipoProfesionMapper());
            modelBuilder.Configurations.Add(new TipoDocumentoMapper());
            modelBuilder.Configurations.Add(new TipoDomicilioMapper());
            modelBuilder.Configurations.Add(new PaisMapper());
            modelBuilder.Configurations.Add(new ProvinciaMapper());
            modelBuilder.Configurations.Add(new LocalidadMapper());
            modelBuilder.Configurations.Add(new ProfesionMapper());
            modelBuilder.Configurations.Add(new JurisdiccionLocalidadMapper());

            //NUEVO! MAPPER DISTRITO
            modelBuilder.Configurations.Add(new DistritosMapper());
            modelBuilder.Configurations.Add(new RegionesMapper());

            //NUEVO! MAPPER CARGASTECNICAS
            modelBuilder.Configurations.Add(new CargasTecnicasMapper());
            modelBuilder.Configurations.Add(new AnalisisTecnicosMapper());
            modelBuilder.Configurations.Add(new ComponenteConfiguracionMapper());

            modelBuilder.Configurations.Add(new DomicilioMapper());
            modelBuilder.Configurations.Add(new PersonaMapper());
            modelBuilder.Configurations.Add(new NacionalidadMapper());
            modelBuilder.Configurations.Add(new PersonaDomicilioMapper());
            modelBuilder.Configurations.Add(new DocumentoMapper());
            modelBuilder.Configurations.Add(new ViaMapper());
            modelBuilder.Configurations.Add(new TipoViaMapper());
            modelBuilder.Configurations.Add(new TramoViaMapper());
            modelBuilder.Configurations.Add(new ParcelaOperacionMapper());
            modelBuilder.Configurations.Add(new TipoParcelaOperacionMapper());
            // FIN FUNCIONES GENERALES

            // INICIO VALUACION
            modelBuilder.Configurations.Add(new CoeficientesMapper());
            modelBuilder.Configurations.Add(new CoeficientesMejoraMapper());
            modelBuilder.Configurations.Add(new CoeficientesTierraMapper());
            modelBuilder.Configurations.Add(new TipoCoeficientesMapper());
            modelBuilder.Configurations.Add(new TipoValorBasicoMejoraMapper());
            modelBuilder.Configurations.Add(new TipoValorBasicoTierraMapper());
            modelBuilder.Configurations.Add(new ValorBasicoMejoraMapper());
            modelBuilder.Configurations.Add(new ValorBasicoTierraModuloMapper());
            modelBuilder.Configurations.Add(new ValorBasicoTierraStoreMapper());
            modelBuilder.Configurations.Add(new ValorBasicoMejoraStoreMapper());
            modelBuilder.Configurations.Add(new ValorBasicoTierraSuperficieMapper());
            modelBuilder.Configurations.Add(new ValuacionPadronCabeceraMapper());
            modelBuilder.Configurations.Add(new ValuacionPadronDetalleMapper());
            modelBuilder.Configurations.Add(new ValuacionPadronDetalleTemporalMapper());
            modelBuilder.Configurations.Add(new TipoValuacionMapper());
            modelBuilder.Configurations.Add(new CoeficientesStoreMapper());

            modelBuilder.Configurations.Add(new MejoraMapper());
            modelBuilder.Configurations.Add(new EstadosConservacionMapper());
            modelBuilder.Configurations.Add(new TiposMejorasMapper());

            // FIN VALUACION
            modelBuilder.Configurations.Add(new EjesViaMapper());

            //INICO MANTENIMIENTO
            modelBuilder.Configurations.Add(new ComponenteTAMapper());
            modelBuilder.Configurations.Add(new AtributoTAMapper());
            //FIN MANTENIMIENTO
            modelBuilder.Configurations.Add(new UsuariosActivosMapper());

            //INICO INTERFACES
            // modelBuilder.Configurations.Add(new RelacionesMapper());
            modelBuilder.Configurations.Add(new InterfaseRentasLogMapper());
            //FIN INTERFACES

            //Ploteo
            modelBuilder.Configurations.Add(new PlantillaMapper());
            modelBuilder.Configurations.Add(new LayerMapper());
            modelBuilder.Configurations.Add(new PlantillaEscalaMapper());
            modelBuilder.Configurations.Add(new PlantillaFondoMapper());
            modelBuilder.Configurations.Add(new HojaMapper());
            modelBuilder.Configurations.Add(new ResolucionMapper());
            modelBuilder.Configurations.Add(new NorteMapper());
            modelBuilder.Configurations.Add(new PlantillaTextoMapper());
            modelBuilder.Configurations.Add(new FuncionAdicionalMapper());
            modelBuilder.Configurations.Add(new FuncAdicParametroMapper());
            modelBuilder.Configurations.Add(new PlantillaCategoriaMapper());

            //INMUEBLES
            modelBuilder.Configurations.Add(new ClaseParcelaMapper());
            modelBuilder.Configurations.Add(new EstadoParcelaMapper());
            modelBuilder.Configurations.Add(new NomenclaturaInmuebleMapper());
            modelBuilder.Configurations.Add(new OrigenParcelaMapper());
            modelBuilder.Configurations.Add(new ParcelaInmuebleMapper());
            modelBuilder.Configurations.Add(new PartidaSecuenciaMapper());
            modelBuilder.Configurations.Add(new MensuraSecuenciaMapper());
            modelBuilder.Configurations.Add(new TipoNomenclaturaMapper());
            modelBuilder.Configurations.Add(new TipoParcelaMapper());
            modelBuilder.Configurations.Add(new UnidadTributariaInmuebleMapper());
            modelBuilder.Configurations.Add(new TipoUnidadTributariaMapper());
            modelBuilder.Configurations.Add(new ValuacionMapper());
            modelBuilder.Configurations.Add(new ParcelaGraficaMapper());
            modelBuilder.Configurations.Add(new MensuraMapper());
            modelBuilder.Configurations.Add(new ParcelaMensuraMapper());
            modelBuilder.Configurations.Add(new TipoMensuraMapper());
            modelBuilder.Configurations.Add(new EstadoMensuraMapper());
            modelBuilder.Configurations.Add(new MensuraDocumentoMapper());
            modelBuilder.Configurations.Add(new MensuraRelacionadaMapper());
            modelBuilder.Configurations.Add(new VIRInmuebleMapper());
            modelBuilder.Configurations.Add(new VIRValuacionMapper());
            modelBuilder.Configurations.Add(new VIRVbEuCoefEstadoMapper());
            modelBuilder.Configurations.Add(new VIRVbEuTipoEdifMapper());
            modelBuilder.Configurations.Add(new VIRVbEuUsosMapper());
            modelBuilder.Configurations.Add(new VIREquivInmDestinosMejorasMapper()); 

            //SubSistema WEB
            modelBuilder.Configurations.Add(new WebInstructivoMapper());
            modelBuilder.Configurations.Add(new WebLinksMapper());
            modelBuilder.Configurations.Add(new AyudaLineaMapper());

            modelBuilder.Configurations.Add(new TipoInspeccionMapper());
            modelBuilder.Configurations.Add(new InspeccionMapper());
            modelBuilder.Configurations.Add(new InspectorMapper());
            modelBuilder.Configurations.Add(new InspectorTipoInspeccionMapper());
            modelBuilder.Configurations.Add(new InspeccionDocumentoMapper());
            modelBuilder.Configurations.Add(new InspeccionUnidadesTributariasMapper());
            modelBuilder.Configurations.Add(new EstadoInspeccionMapper());

            modelBuilder.Configurations.Add(new UnidadTributariaPersonaMapper());
            modelBuilder.Configurations.Add(new DominioMapper());
            modelBuilder.Configurations.Add(new TipoInscripcionMapper());
            modelBuilder.Configurations.Add(new DominioTitularMapper());
            modelBuilder.Configurations.Add(new TipoTitularidadMapper());

            //OBRAS PARTICULARES
            modelBuilder.Configurations.Add(new ExpedienteObraMapper());
            modelBuilder.Configurations.Add(new INMCertificadoCatastralMapper());
            modelBuilder.Configurations.Add(new PlanMapper());
            modelBuilder.Configurations.Add(new UnidadTributariaExpedienteObraMapper());
            modelBuilder.Configurations.Add(new EstadoExpedienteMapper());
            modelBuilder.Configurations.Add(new EstadoExpedienteObraMapper());
            modelBuilder.Configurations.Add(new TipoExpedienteMapper());
            modelBuilder.Configurations.Add(new TipoExpedienteObraMapper());
            modelBuilder.Configurations.Add(new DomicilioExpedienteObraMapper());
            modelBuilder.Configurations.Add(new TipoSuperficieMapper());
            modelBuilder.Configurations.Add(new TipoSuperficieExpedienteObraMapper());
            modelBuilder.Configurations.Add(new DestinoMapper());
            modelBuilder.Configurations.Add(new ServicioMapper());
            modelBuilder.Configurations.Add(new ServicioExpedienteObraMapper());
            modelBuilder.Configurations.Add(new EspedienteObraDocumentoMapper());
            modelBuilder.Configurations.Add(new TipoPersonaMapper());
            modelBuilder.Configurations.Add(new PersonaExpedienteObraMapper());
            modelBuilder.Configurations.Add(new LiquidacionMapper());
            modelBuilder.Configurations.Add(new LiquidacionExternaMapper());
            modelBuilder.Configurations.Add(new InspeccionExpedienteObraMapper());
            modelBuilder.Configurations.Add(new ControlTecnicoMapper());
            modelBuilder.Configurations.Add(new ObservacionExpedienteMapper());

            modelBuilder.Configurations.Add(new UnidadTributariaDocumentoMapper());
            modelBuilder.Configurations.Add(new ParcelaDocumentoMapper());
            modelBuilder.Configurations.Add(new UnidadTributariaDomicilioMapper());

            // ACTAS
            modelBuilder.Configurations.Add(new ActaPersonasMapper());
            modelBuilder.Configurations.Add(new ActaDocumentoMapper());
            modelBuilder.Configurations.Add(new ActaActaRelMapper());
            modelBuilder.Configurations.Add(new ActaObjetoMapper());
            modelBuilder.Configurations.Add(new ActaRolPersonaMapper());
            modelBuilder.Configurations.Add(new ActaMapper());
            modelBuilder.Configurations.Add(new ActaTipoMapper());
            modelBuilder.Configurations.Add(new ActaUnidadTributariaMapper());
            modelBuilder.Configurations.Add(new EstadoActaMapper());
            modelBuilder.Configurations.Add(new ActaEstadoMapper());
            modelBuilder.Configurations.Add(new ActaDomicilioMapper());

            //Reclamos Diarios
            modelBuilder.Configurations.Add(new Reclamos_ActualizacionMapper());
            modelBuilder.Configurations.Add(new Reclamos_ReclamoDiarioMapper());
            modelBuilder.Configurations.Add(new Reclamos_MotivoMapper());
            modelBuilder.Configurations.Add(new Reclamos_TipoMapper());
            modelBuilder.Configurations.Add(new Reclamos_ClaseMapper());
            modelBuilder.Configurations.Add(new UbicacionPloteoMapper());

            //INICO INTERFACES
            modelBuilder.Configurations.Add(new TransaccionesPendientesMapper());
            modelBuilder.Configurations.Add(new InterfacesPadronTempMapper());

            modelBuilder.Configurations.Add(new METramiteMapper());
            modelBuilder.Configurations.Add(new MEEstadoTramiteMapper());
            modelBuilder.Configurations.Add(new MEMovimientoMapper());
            modelBuilder.Configurations.Add(new MEObjetoTramiteMapper());
            modelBuilder.Configurations.Add(new MEPrioridadTramiteMapper());
            modelBuilder.Configurations.Add(new MERemitoMapper());
            modelBuilder.Configurations.Add(new METipoMovimientoMapper());
            modelBuilder.Configurations.Add(new METipoTramiteMapper());
            modelBuilder.Configurations.Add(new MERequisitoMapper());
            modelBuilder.Configurations.Add(new MEObjetoRequisitoMapper());
            modelBuilder.Configurations.Add(new METramiteRequisitoMapper());
            modelBuilder.Configurations.Add(new METramiteDocumentoMapper());
            modelBuilder.Configurations.Add(new MEDesgloseMapper());
            modelBuilder.Configurations.Add(new MEDesgloseDestinoMapper());
            modelBuilder.Configurations.Add(new SectorMapper());
            modelBuilder.Configurations.Add(new MEPrioridadTipoMapper());
            modelBuilder.Configurations.Add(new MEEntradaMapper());
            modelBuilder.Configurations.Add(new MEObjetoEntradaMapper());
            modelBuilder.Configurations.Add(new METramiteEntradaMapper());
            modelBuilder.Configurations.Add(new METramiteEntradaRelacionMapper());
            modelBuilder.Configurations.Add(new MEComprobantePagoMapper());


            //Ploteos
            modelBuilder.Configurations.Add(new ImagenSatelitalMapper());
            modelBuilder.Configurations.Add(new PlantillaViewportMapper());
            modelBuilder.Configurations.Add(new LayerViewportMapper());
            modelBuilder.Configurations.Add(new PloteoFrecuenteEspecialMapper());
            modelBuilder.Configurations.Add(new PloteoFrecuenteGeometriaMapper());
            modelBuilder.Configurations.Add(new TipoPlanoMapper());
            modelBuilder.Configurations.Add(new CensoMapper());
            modelBuilder.Configurations.Add(new PartidoMapper());
            modelBuilder.Configurations.Add(new ManzanaMapper());
            //Declaraciones Juradas
            modelBuilder.Configurations.Add(new DDJJDesignacionMapper());
            modelBuilder.Configurations.Add(new DDJJDominioMapper());
            modelBuilder.Configurations.Add(new DDJJDominioTitularMapper());
            modelBuilder.Configurations.Add(new DDJJMapper());
            modelBuilder.Configurations.Add(new DDJJOrigenMapper());
            modelBuilder.Configurations.Add(new DDJJPersonaDomicilioMapper());
            modelBuilder.Configurations.Add(new DDJJSorCarMapper());
            modelBuilder.Configurations.Add(new DDJJSorCaracteristicasMapper());
            modelBuilder.Configurations.Add(new DDJJSorMapper());
            modelBuilder.Configurations.Add(new DDJJSorOtrasCarMapper());
            modelBuilder.Configurations.Add(new DDJJSorTipoCarMapper());
            modelBuilder.Configurations.Add(new DDJJVersionMapper());
            //Designacion
            modelBuilder.Configurations.Add(new DesignacionMapper());
            modelBuilder.Configurations.Add(new TipoDesignadorMapper());

            modelBuilder.Configurations.Add(new VALAptitudesMapper());
            modelBuilder.Configurations.Add(new VALSuperficiesMapper());
            modelBuilder.Configurations.Add(new VALAptCarMapper());

            modelBuilder.Configurations.Add(new OCObjetoMapper());

            modelBuilder.Configurations.Add(new DDJJUMapper());
            modelBuilder.Configurations.Add(new DDJJUOtrasCarMapper());
            modelBuilder.Configurations.Add(new DDJJUFraccionesMapper());
            modelBuilder.Configurations.Add(new DDJJUMedidaLinealMapper());
            modelBuilder.Configurations.Add(new VALClasesParcelasMapper());
            modelBuilder.Configurations.Add(new VALClasesParcelasMedidaLinealMapper());
            modelBuilder.Configurations.Add(new VALTiposMedidasLinealesMapper());

            modelBuilder.Configurations.Add(new INMCaracteristicaMapper());
            modelBuilder.Configurations.Add(new INMDestinoMejoraMapper());
            modelBuilder.Configurations.Add(new INMIncisoMapper());
            modelBuilder.Configurations.Add(new INMMejoraCaracteristicaMapper());
            modelBuilder.Configurations.Add(new INMMejoraMapper());
            modelBuilder.Configurations.Add(new INMTipoCaracteristicaMapper());
            modelBuilder.Configurations.Add(new INMMejoraOtraCarMapper());
            modelBuilder.Configurations.Add(new INMOtraCaracteristicaMapper());
            modelBuilder.Configurations.Add(new INMLibreDeDeudaMapper());

            modelBuilder.Configurations.Add(new VALDecretoMapper());
            modelBuilder.Configurations.Add(new VALDecretoJurisdiccionMapper());
            modelBuilder.Configurations.Add(new VALDecretoZonaMapper());
            modelBuilder.Configurations.Add(new VALValuacionMapper());
            modelBuilder.Configurations.Add(new VALValuacionDecretoMapper());

            modelBuilder.Configurations.Add(new VALCoefMenor2kMapper());
            modelBuilder.Configurations.Add(new VALCoefEsquinaMenor900Mapper());
            modelBuilder.Configurations.Add(new VALCoef2a15kMapper());
            modelBuilder.Configurations.Add(new VALCoefMayor15kMapper());
            modelBuilder.Configurations.Add(new VALCoefTriangFrenteMapper());
            modelBuilder.Configurations.Add(new VALCoefTriangVerticeMapper());

            modelBuilder.Configurations.Add(new VALPuntajesCaminosMapper());
            modelBuilder.Configurations.Add(new VALPuntajesEmbarquesMapper());
            modelBuilder.Configurations.Add(new VALPuntajesLocalidadesMapper());

            modelBuilder.Configurations.Add(new VALValoresOptRuralesMapper());
            modelBuilder.Configurations.Add(new VALValoresOptSuburbanosMapper());
            modelBuilder.Configurations.Add(new VALCoeficientesIncisosMapper());
            modelBuilder.Configurations.Add(new VALCoeficientesOtrasCarMapper());
            modelBuilder.Configurations.Add(new VALCoefDepreciacionMapper());
            modelBuilder.Configurations.Add(new VALCoeficientesProrrateoMapper());

            //VALIDACIONES GENERICAS
            modelBuilder.Configurations.Add(new ValidacionMapper());
            modelBuilder.Configurations.Add(new ValidacionSubtipoMapper());
            modelBuilder.Configurations.Add(new ValidacionFuncionMapper());
            modelBuilder.Configurations.Add(new ValidacionParametroMapper());
            modelBuilder.Configurations.Add(new ValidacionGrupoFuncionMapper());
            //FIN VALIDACIONES GENERICAS

            //LOG
            modelBuilder.Configurations.Add(new RPILogRespuestasMapper());
            modelBuilder.Configurations.Add(new RPITipoOperacionMapper());
            modelBuilder.Configurations.Add(new RPILogConsultasMapper());

            #region Tablas Temporales
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJDominioTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJDominioTitularTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJSorCarTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJUFraccionesTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJUMedidaLinealTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJSorTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJUTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJDesignacionTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.DDJJPersonaDomicilioTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.INMMejoraCaracteristicaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.INMMejoraOtraCarTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.INMMejoraTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.VALAptCarTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DDJJ.VALSuperficieTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.Valuacion.VALValuacionTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.Valuacion.VALValuacionDecretoTemporalMapper());

            modelBuilder.Configurations.Add(new TMP_MAPR.DominioTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DominioTitularTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.ParcelaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.ParcelaOperacionTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.MensuraTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.ParcelaMensuraTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.MensuraRelacionadaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.UnidadTributariaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.NomenclaturaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DesignacionTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.DivisionTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.INMCertificadoCatastralTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.INMLibreDeDeudaTemporalMapper());
            modelBuilder.Configurations.Add(new TMP_MAPR.EspacioPublicoTemporalMapper());
            #endregion

            base.OnModelCreating(modelBuilder);
        }
        private void DefineQueryFilters()
        {
            this.Filter<IBajaLogica>(FILTROS.BAJA_LOGICA, q => q.Where(x => x.FechaBaja == null), false);
        }

        public SRIDParser GetSRIDParser()
        {
            return new SRIDParser();
        }
    }
}