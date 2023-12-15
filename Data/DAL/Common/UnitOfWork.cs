using System;
using System.Data.Entity.Validation;
using System.Text;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Repositories;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Repositories.Temporal;
using System.Data.Entity.Infrastructure;
//using GeoSit.Data.BusinessEntities.Reportes;

namespace GeoSit.Data.DAL.Common
{
    public class UnitOfWork : IDisposable
    {
        private readonly GeoSITMContext _context;

        private IUnidadTributariaRepository _unidadTributariaRepository;
        private IUnidadTributariaExpedienteObraRepository _unidadTributariaExpedienteObraRepository;
        private IExpedienteObraRepository _expedienteObraRepository;
        private ITipoExpedienteObraRepository _tipoExpedienteObraRepository;
        private ITipoExpedienteRepository _tipoExpedienteRepository;
        private IDomicilioInmuebleRepository _domicilioRepository;
        private IDomicilioExpedienteObraRepository _domicilioExpedienteObraRepository;
        private IPlanRepository _planRepository;
        //private IReporteParcelario _reporteParcelario;
        private IActaRepository _actaRepository;
        private IEstadoExpedienteRepository _estadoExpedienteRepository;
        private IEstadoExpedienteObraRepository _estadoExpedienteObraRepository;

        private ITipoSuperficieExpedienteObraRepository _tipoSuperficieExpedienteObraRepository;
        private ITipoSuperficieRepository _tipoSuperficieRepository;
        private IDestinoRepository _destinoRepository;

        private IServicioRepository _servicioRepository;
        private IServicioExpedienteObraRepository _servicioExpedienteObraRepository;
        private IDocumentoRepository _documentoRepository;
        private IExpedienteObraDocumentoRepository _expedienteObraDocumentoRepository;

        private IRolRepository _rolRepository;
        private IPersonaRepository _personaRepository;
        private ITipoPersonaRepository _tipoPersonaRepository;
        private IPersonaExpedienteObraRepository _personaExpedienteObraRepository;

        private ILiquidacionRepository _liquidacionRepository;
        private IInspeccionRepository _inspeccionRepository;
        private IControlTecnicoRepository _controlTecnicoRepository;
        private IObservacionRepository _observacionRepository;

        private IPlantillaRepository _plantillaRepository;
        private ILayerRepository _layerRepository;
        private IPlantillaEscalaRepository _plantillaEscalaRepository;
        private IComponenteRepository _componenteRepository;
        private IAtributoRepository _atributoRepository;
        private IParcelaRepository _parcelaRepository;
        private IEspacioPublicoTemporalRepository _espacioPublicoTemporalRepository;
        private IParcelaTemporalRepository _parcelaTemporalRepository;
        private IPartidaSecuenciaRepository _partidaSecuenciaRepository;
        private IMensuraSecuenciaRepository _mensuraSecuenciaRepository;
        private IMensuraTemporalRepository _mensuraTemporalRepository;
        private IParcelaGraficaRepository _parcelaGraficaRepository;
        private ILayerGrafRepository _layerGrafRepository;
        private IPlantillaFondoRepository _plantillaFondoRepository;
        private IHojaRepository _hojaRepository;
        private IResolucionRepository _resolucionRepository;
        private INorteRepository _norteRepository;
        private IPlantillaTextoRepository _plantillaTextoRepository;
        private IFuncionAdicionalRepository _funcionAdicionalRepository;
        private IFuncAdicParametroRepository _funcAdicParametroRepository;
        private IParcelaPlotRepository _parcelaPlotRepository;
        private IPlantillaCategoriaRepository _plantillaCategoriaRepository;
        private IColeccionRepository _coleccionRepository;
        private AnalisisPredefinidoRepository _analisisPredefinidoRepository;

        private CuadraPlotRepository _cuadraPlotRepository;
        private ManzanaPlotRepository _manzanaPlotRepository;
        private CallePlotRepository _callePlotRepository;

        private ITipoParcelaRepository _tipoParcelaRepository;
        private ITipoUnidadTributariaRepository _tipoUnidadTributariaRepository;
        private ITipoNomenclaturaRepository _tipoNomenclaturaRepository;
        private IUnidadTributariaDocumentoRepository _unidadTributariaDocumentosRepository;
        private IUnidadTributariaDomicilioRepository _unidadTributariaDomiciliosRepository;
        private INomenclaturaRepository _nomenclaturaRepository;
        private IParcelaDocumentoRepository _parcelaDocumentoRepository;
        private IClaseParcelaRepository _claseParcelaRepository;
        private IEstadoParcelaRepository _estadoParcelaRepository;
        private IParcelaOperacionRepository _parcelaOperacionRepository;
        private ITipoParcelaOperacionRepository _tipoParcelaOperacionRepository;

        private IUnidadTributariaPersonaRepository _unidadTributariaPersonaRepository;
        private IParametroRepository _parametroRepository;
        private IDominioRepository _dominioRepository;
        private ITipoInscripcionRepository _tipoInscripcionRepository;
        private IDominioTitularRepository _dominioTitularRepository;

        private IEstadoDeudaRepository _estadoDeudaRepository;

        private ITramiteRepository _tramiteRepository;
        private ITipoTramiteRepository _tipoTramiteRepository;
        private ITramiteDocumentoRepository _tramiteDocumentoRepository;
        private ITramitePersonaRepository _tramitePersonaRepository;
        private ITramiteSeccionRepository _tramiteSeccionRepository;
        private ITramiteUnidadTributariaRepository _tramiteUnidadTributariaRepository;
        private ITipoSeccionRepository _tipoSeccionRepository;

        private MesaEntradasRepository _mesaEntradasRepository;

        private IAuditoriaRepository _auditoriaRepository;
        private IInterfaseRentasLogRepository _interfaseRentasLogRepository;

        private IImagenSatelitalRepository _imagenSatelitalRepository;
        private IPloteoFrecuenteRepository _ploteoFrecuenteRepository;
        private IPloteoFrecuenteEspecialRepository _ploteoFrecuenteEspecialRepository;
        private IPlantillaViewportRepository _plantillaViewportRepository;
        private ITipoViewportRepository _tipoViewportRepository;
        private ILayerViewportReposirory _layerViewportReposirory;
        private IExpansionPlotRepository _expansionPlotRepository;
        private IPartidoRepository _partidoRepository;
        private ITipoPlanoRepository _tipoPlanoRepository;
        private ICensoRepository _censoRepository;
        private IDeclaracionJuradaRepository _declaracionJuradaRepository;
        private IDesignacionRepository _designacionRepository;
        private IValidacionDBRepository _validacionDBRepository;

        private IRegistroPropiedadInmuebleRepository _RegistroPropiedadInmuebleRepository;

        private IDivisionTemporalRepository _divisionTemporalRepository;
        private ICertificadoCatastralTemporalRepository _certificadoCatastralTemporalRepository;
        private ILibreDeDeudaTemporalRepository _libreDeDeudaTemporalRepository;
        private IDominioTemporalRepository _dominioTemporalRepository;
        private IUnidadTributariaTemporalRepository _unidadTributariaTemporalRepository;
        private IDeclaracionJuradaTemporalRepository _declaracionJuradaTemporalRepository;

        private IVIRValuacionRepository _virValuacionRepository;
        private IVIRInmuebleRepository _virInmuebleRepository;
        private IVIRVbEuUsoRepository _virVbEuUsoRepository;
        private IVIRVbEuCoefEstadoRepository _virVbEuCoefEstadoRepository;
        private IVIRVbEuTipoEdifRepository _virVbEuTipoEdifRepository;
        private IVIREquivInmDestinosMejorasRepository _virEquivInmDestinosMejoras;

        private IComprobantePagoRepository _comprobantePagoRepository;

        public UnitOfWork()
        {
            _context = GeoSITMContext.CreateContext();
        }
        public IUnidadTributariaRepository UnidadTributariaRepository
        {
            //Singleton del repositorio UnidadTributariaRespository
            get { return _unidadTributariaRepository ?? (_unidadTributariaRepository = new UnidadTributariaRepository(_context)); }
        }
        public IUnidadTributariaDocumentoRepository UnidadTributariaDocumentoRepository
        {
            //Singleton del repositorio UnidadTributariaDocumentoRepository
            get { return _unidadTributariaDocumentosRepository ?? (_unidadTributariaDocumentosRepository = new UnidadTributariaDocumentoRepository(_context)); }
        }
        public IUnidadTributariaDomicilioRepository UnidadTributariaDomicilioRepository
        {
            //Singleton del repositorio UnidadTributariaDomicilioRepository
            get { return _unidadTributariaDomiciliosRepository ?? (_unidadTributariaDomiciliosRepository = new UnidadTributariaDomicilioRepository(_context)); }
        }
        public IUnidadTributariaExpedienteObraRepository UnidadTributariaExpedienteObraRepository
        {
            //Singleton del repositorio UnidadTributariaExpedienteObraRepository
            get { return _unidadTributariaExpedienteObraRepository ?? (_unidadTributariaExpedienteObraRepository = new UnidadTributariaExpedienteObraRepository(_context)); }
        }
        public IExpedienteObraRepository ExpedienteObraRepository
        {
            //Singleton del repositorio ExpedienteObraRepository
            get { return _expedienteObraRepository ?? (_expedienteObraRepository = new ExpedienteObraRepository(_context)); }
        }
        public ITipoExpedienteObraRepository TipoExpedienteObraRepository
        {
            //Singleton del repositorio TipoExpedienteObraRepository
            get { return _tipoExpedienteObraRepository ?? (_tipoExpedienteObraRepository = new TipoExpedienteObraRepository(_context)); }
        }
        public ITipoExpedienteRepository TipoExpedienteRepository
        {
            //Singleton del repositorio TipoExpedienteRepository
            get { return _tipoExpedienteRepository ?? (_tipoExpedienteRepository = new TipoExpedienteRepository(_context)); }
        }
        public IDomicilioInmuebleRepository DomicilioRepository
        {
            //Singleton del repositorio DomicilioInmuebleRepository
            get { return _domicilioRepository ?? (_domicilioRepository = new DomicilioInmuebleRepository(_context)); }
        }
        public IDomicilioExpedienteObraRepository DomicilioExpedienteObraRepository
        {
            //Singleton del repositorio DomicilioInmuebleExpedienteObraRepository
            get { return _domicilioExpedienteObraRepository ?? (_domicilioExpedienteObraRepository = new DomicilioExpedienteObraRepository(_context)); }
        }
        public INomenclaturaRepository NomenclaturaRepository
        {
            //Singleton del repositorio NomenclaturaRepository
            get { return _nomenclaturaRepository ?? (_nomenclaturaRepository = new NomenclaturaRepository(_context)); }
        }
        public IPlanRepository PlanRepository
        {
            //Singleton del repositorio PlanRepository
            get { return _planRepository ?? (_planRepository = new PlanRepository(_context)); }
        }

        //public IReporteParcelario ReporteParcelario
        //{
        //    get { return _reporteParcelario ?? (_reporteParcelario = new ReporteParcelarioRepository(_context));}
        //}

        public IActaRepository ActaRepository
        {
            //Singleton del repositorio ActaRepository
            get { return _actaRepository ?? (_actaRepository = new ActaRepository(_context)); }
        }
        public IEstadoExpedienteRepository EstadoExpedienteRepository
        {
            //Singleton del repositorio EstadoExpedienteRepository
            get { return _estadoExpedienteRepository ?? (_estadoExpedienteRepository = new EstadoExpedienteRepository(_context)); }
        }
        public IEstadoExpedienteObraRepository EstadoExpedienteObraRepository
        {
            //Singleton del repositorio EstadoExpedienteObraRepository
            get { return _estadoExpedienteObraRepository ?? (_estadoExpedienteObraRepository = new EstadoExpedienteObraRepository(_context)); }
        }
        public ITipoSuperficieExpedienteObraRepository TipoSuperficieExpedienteObraRepository
        {
            //Singleton del repositorio TipoSuperficieExpedienteObraRepository
            get { return _tipoSuperficieExpedienteObraRepository ?? (_tipoSuperficieExpedienteObraRepository = new TipoSuperficieExpedienteObraRepository(_context)); }
        }
        public ITipoSuperficieRepository TipoSuperficieRepository
        {
            //Singleton del repositorio TipoSuperficieRepository
            get { return _tipoSuperficieRepository ?? (_tipoSuperficieRepository = new TipoSuperficieRepository(_context)); }
        }
        public IDestinoRepository DestinoRepository
        {
            //Singleton del repositorio DestinoRepository
            get { return _destinoRepository ?? (_destinoRepository = new DestinoRepository(_context)); }
        }
        public IServicioRepository ServicioRepository
        {
            //Singleton del repositorio ServicioRepository
            get { return _servicioRepository ?? (_servicioRepository = new ServicioRepository(_context)); }
        }
        public IServicioExpedienteObraRepository ServicioExpedienteObraRepository
        {
            //Singleton del repositorio ServicioExpedienteObraRepository
            get { return _servicioExpedienteObraRepository ?? (_servicioExpedienteObraRepository = new ServicioExpedienteObraRepository(_context)); }
        }
        public IDocumentoRepository DocumentoRepository
        {
            //Singleton del repositorio DocumentoRepository
            get { return _documentoRepository ?? (_documentoRepository = new DocumentoRepository(_context)); }
        }
        public IExpedienteObraDocumentoRepository ExpedienteObraDocumentoRepository
        {
            //Singleton del repositorio ExpedienteObraDocumentoRepository
            get { return _expedienteObraDocumentoRepository ?? (_expedienteObraDocumentoRepository = new ExpedienteObraDocumentoRepository(_context)); }
        }
        public IRolRepository RolRepository
        {
            //Singleton del repositorio RolRepository
            get { return _rolRepository ?? (_rolRepository = new RolRepository(_context)); }
        }
        public IPersonaRepository PersonaRepository
        {
            //Singleton del repositorio PersonaRepository
            get { return _personaRepository ?? (_personaRepository = new PersonaRepository(_context)); }
        }
        public ITipoPersonaRepository TipoPersonaRepository
        {
            //Singleton del repositorio PersonaRepository
            get { return _tipoPersonaRepository ?? (_tipoPersonaRepository = new TipoPersonaRepository(_context)); }
        }
        public IPersonaExpedienteObraRepository PersonaExpedienteObraRepository
        {
            //Singleton del repositorio PersonaInmuebleExpedienteObraRepository
            get { return _personaExpedienteObraRepository ?? (_personaExpedienteObraRepository = new PersonaExpedienteObraRepository(_context)); }
        }
        public ILiquidacionRepository LiquidacionRepository
        {
            //Singleton del repositorio LiquidacionRepository
            get { return _liquidacionRepository ?? (_liquidacionRepository = new LiquidacionRepository(_context)); }
        }
        public IInspeccionRepository InspeccionRepository
        {
            //Singleton del repositorio InspeccionRepository
            get { return _inspeccionRepository ?? (_inspeccionRepository = new InspeccionRepository(_context)); }
        }
        public IControlTecnicoRepository ControlTecnicoRepository
        {
            //Singleton del repositorio ControlTecnicoRepository
            get { return _controlTecnicoRepository ?? (_controlTecnicoRepository = new ControlTecnicoRepository(_context)); }
        }
        public IObservacionRepository ObservacionRepository
        {
            //Singleton del repositorio ObservacionRepository
            get { return _observacionRepository ?? (_observacionRepository = new ObservacionRepository(_context)); }
        }
        public IPlantillaRepository PlantillaRepository
        {
            //Singleton del repositorio plantilla
            get { return _plantillaRepository ?? (_plantillaRepository = new PlantillaRepository(_context)); }
        }
        public ILayerRepository LayerRepository
        {
            //Singleton del repositorio layer
            get { return _layerRepository ?? (_layerRepository = new LayerRepository(_context)); }
        }
        public IPlantillaEscalaRepository PlantillaEscalaRepository
        {
            //Singleton del repositorio plantilla escala
            get { return _plantillaEscalaRepository ?? (_plantillaEscalaRepository = new PlantillaEscalaRepository(_context)); }
        }
        public IComponenteRepository ComponenteRepository
        {
            //Singleton del repositorio componente
            get { return _componenteRepository ?? (_componenteRepository = new ComponenteRepository(_context)); }
        }
        public IAtributoRepository AtributoRepository
        {
            //Singleton del repositorio atributo
            get { return _atributoRepository ?? (_atributoRepository = new AtributoRepository(_context)); }
        }
        public IParcelaRepository ParcelaRepository
        {
            //Singleton del repositorio parcela
            get { return _parcelaRepository ?? (_parcelaRepository = new ParcelaRepository(_context)); }
        }
        public IParcelaTemporalRepository ParcelaTemporalRepository
        {
            //Singleton del repositorio parcela Temporal
            get { return _parcelaTemporalRepository ?? (_parcelaTemporalRepository = new ParcelaTemporalRepository(_context)); }
        }
        public IEspacioPublicoTemporalRepository EspacioPublicoTemporalRepository
        {
            //Singleton del repositorio espacio publico Temporal
            get { return _espacioPublicoTemporalRepository ?? (_espacioPublicoTemporalRepository = new EspacioPublicoTemporalRepository(_context)); }
        }

        public IPartidaSecuenciaRepository PartidaSecuenciaRepository
        {
            //Singleton del repositorio partida secuencia
            get { return _partidaSecuenciaRepository ?? (_partidaSecuenciaRepository = new PartidaSecuenciaRepository(_context)); }
        }

        public IMensuraSecuenciaRepository MensuraSecuenciaRepository
        {
            //Singleton del repositorio partida secuencia
            get { return _mensuraSecuenciaRepository ?? (_mensuraSecuenciaRepository = new MensuraSecuenciaRepository(_context)); }
        }

        public IMensuraTemporalRepository MensuraTemporalRepository
        {
            //Singleton del repositorio pmensura Temporal
            get { return _mensuraTemporalRepository ?? (_mensuraTemporalRepository = new MensuraTemporalRepository(_context)); }
        }

        public IParcelaGraficaRepository ParcelaGraficaRepository
        {
            //Singleton del repositorio parcela
            get { return _parcelaGraficaRepository ?? (_parcelaGraficaRepository = new ParcelaGraficaRepository(_context)); }
        }
        public ILayerGrafRepository LayerGrafRepository
        {
            //Singleton del repositorio layerGraf
            get { return _layerGrafRepository ?? (_layerGrafRepository = new LayerGrafRepository(_context)); }
        }
        public IPlantillaFondoRepository PlantillaFondoRepository
        {
            //Singleton del repositorio plantilla
            get { return _plantillaFondoRepository ?? (_plantillaFondoRepository = new PlantillaFondoRepository(_context)); }
        }
        public IHojaRepository HojaRepository
        {
            //Singleton del repositorio plantilla
            get { return _hojaRepository ?? (_hojaRepository = new HojaRepository(_context)); }
        }
        public IResolucionRepository ResolucionRepository
        {
            //Singleton del repositorio resolucion
            get { return _resolucionRepository ?? (_resolucionRepository = new ResolucionRepository(_context)); }
        }
        public INorteRepository NorteRepository
        {
            //Singleton del repositorio Norte
            get { return _norteRepository ?? (_norteRepository = new NorteRepository(_context)); }
        }
        public IPlantillaTextoRepository PlantillaTextoRepository
        {
            //Singleton del repositorio PlantillaTexto
            get { return _plantillaTextoRepository ?? (_plantillaTextoRepository = new PlantillaTextoRepository(_context)); }
        }
        public IFuncionAdicionalRepository FuncionAdicionalRepository
        {
            //Singleton del repositorio funcion adicional
            get { return _funcionAdicionalRepository ?? (_funcionAdicionalRepository = new FuncionAdicionalRepository(_context, FuncAdicParametroRepository)); }
        }
        public IFuncAdicParametroRepository FuncAdicParametroRepository
        {
            //Singleton del repositorio FuncAdicParametro
            get { return _funcAdicParametroRepository ?? (_funcAdicParametroRepository = new FuncAdicParametroRepository(_context)); }
        }
        public IParcelaPlotRepository ParcelaPlotRepository
        {
            //Singleton del repositorio ParcelaPlot
            get { return _parcelaPlotRepository ?? (_parcelaPlotRepository = new ParcelaPlotRepository(_context)); }
        }

        public IAnalisisPredefinidoRepository AnalisisPredefinidoRepository
        {
            //Singleton del repositorio Analisis Predefinido
            get { return _analisisPredefinidoRepository ?? (_analisisPredefinidoRepository = new AnalisisPredefinidoRepository(_context)); }
        }

        public IPlantillaCategoriaRepository PlantillaCategoriaRepository
        {
            //Singleton del repositorio PlantillaCategoria
            get { return _plantillaCategoriaRepository ?? (_plantillaCategoriaRepository = new PlantillaCategoriaRepository(_context)); }
        }
        public IColeccionRepository ColeccionRepository
        {
            get { return _coleccionRepository ?? (_coleccionRepository = new ColeccionRepository(_context)); }
        }
        public ITipoParcelaRepository TipoParcelaRepository
        {
            //Singleton del repositorio TipoParcelaRepository
            get { return _tipoParcelaRepository ?? (_tipoParcelaRepository = new TipoParcelaRepository(_context)); }
        }
        public ITipoUnidadTributariaRepository TipoUnidadTributariaRepository
        {
            //Singleton del repositorio TipoParcelaRepository
            get { return _tipoUnidadTributariaRepository ?? (_tipoUnidadTributariaRepository = new TipoUnidadTributariaRepository(_context)); }
        }
        public ITipoNomenclaturaRepository TiposNomenclaturasRepository
        {
            //Singleton del repositorio TiposNomenclaturasRepository
            get { return _tipoNomenclaturaRepository ?? (_tipoNomenclaturaRepository = new TipoNomenclaturaRepository(_context)); }
        }
        public IParcelaDocumentoRepository ParcelaDocumentoRepository
        {
            //Singleton del repositorio ParcelaDocumentoRepository
            get { return _parcelaDocumentoRepository ?? (_parcelaDocumentoRepository = new ParcelaDocumentoRepository(_context)); }
        }
        public IClaseParcelaRepository ClaseParcelaRepository
        {
            //Singleton del repositorio ClaseParcelaRepository
            get { return _claseParcelaRepository ?? (_claseParcelaRepository = new ClaseParcelaRepository(_context)); }
        }
        public IEstadoParcelaRepository EstadosParcelaRepository
        {
            //Singleton del repositorio EstadosParcelaRepository
            get { return _estadoParcelaRepository ?? (_estadoParcelaRepository = new EstadoParcelaRepository(_context)); }
        }
        public IParcelaOperacionRepository ParcelaOperacionRepository
        {
            //Singleton del repositorio ParcelaOperacionRepository
            get { return _parcelaOperacionRepository ?? (_parcelaOperacionRepository = new ParcelaOperacionRepository(_context)); }
        }
        public ITipoParcelaOperacionRepository TipoParcelaOperacionRepository
        {
            //Singleton del repositorio TipoParcelaOperacionRepository
            get { return _tipoParcelaOperacionRepository ?? (_tipoParcelaOperacionRepository = new TipoParcelaOperacionRepository(_context)); }
        }
        public IUnidadTributariaPersonaRepository UnidadTributariaPersonaRepository
        {
            //Singleton del repositorio UnidadTributariaPersonaRepository
            get { return _unidadTributariaPersonaRepository ?? (_unidadTributariaPersonaRepository = new UnidadTributariaPersonaRepository(_context)); }
        }
        public IParametroRepository ParametroRepository
        {
            //Singleton del repositorio ParametroRepository
            get { return _parametroRepository ?? (_parametroRepository = new ParametroRepository(_context)); }
        }
        public IDominioRepository DominioRepository
        {
            //Singleton del repositorio DominioRepository
            get { return _dominioRepository ?? (_dominioRepository = new DominioRepository(_context)); }
        }
        public ITipoInscripcionRepository TipoInscripcionRepository
        {
            //Singleton del repositorio TipoInscripcionRepository
            get { return _tipoInscripcionRepository ?? (_tipoInscripcionRepository = new TipoInscripcionRepository(_context)); }
        }
        public IDominioTitularRepository DominioTitularRepository
        {
            //Singleton del repositorio DominioTitularRepository
            get { return _dominioTitularRepository ?? (_dominioTitularRepository = new DominioTitularRepository(_context)); }
        }
        public IEstadoDeudaRepository EstadoDeudaRepository
        {
            //Singleton del repositorio EstadoDeudaRepository
            get { return _estadoDeudaRepository ?? (_estadoDeudaRepository = new EstadoDeudaRepository()); }
        }
        public ITramiteRepository TramiteRepository
        {
            get { return _tramiteRepository ?? (_tramiteRepository = new TramiteRepository(_context)); }
        }
        public ITipoTramiteRepository TipoTramiteRepository
        {
            get { return _tipoTramiteRepository ?? (_tipoTramiteRepository = new TipoTramiteRepository(_context)); }
        }
        public ITramiteDocumentoRepository TramiteDocumentoRepository
        {
            get { return _tramiteDocumentoRepository ?? (_tramiteDocumentoRepository = new TramiteDocumentoRepository(_context)); }
        }
        public ITramitePersonaRepository TramitePersonaRepository
        {
            get { return _tramitePersonaRepository ?? (_tramitePersonaRepository = new TramitePersonaRepository(_context)); }
        }
        public ITramiteSeccionRepository TramiteSeccionRepository
        {
            get { return _tramiteSeccionRepository ?? (_tramiteSeccionRepository = new TramiteSeccionRepository(_context)); }
        }
        public ITramiteUnidadTributariaRepository TramiteUnidadTributariaRepository
        {
            get { return _tramiteUnidadTributariaRepository ?? (_tramiteUnidadTributariaRepository = new TramiteUnidadTributariaRepository(_context)); }
        }
        public ITipoSeccionRepository TipoSeccionRepository
        {
            get { return _tipoSeccionRepository ?? (_tipoSeccionRepository = new TipoSeccionRepository(_context)); }
        }
        public MesaEntradasRepository MesaEntradasRepository
        {
            get { return _mesaEntradasRepository ?? (_mesaEntradasRepository = new MesaEntradasRepository(_context)); }
        }
        public IAuditoriaRepository AuditoriaRepository
        {
            get { return _auditoriaRepository ?? (_auditoriaRepository = new AuditoriaRepository(_context)); }
        }

        public ICuadraPlotRepository CuadraPlotRepository
        {
            get { return _cuadraPlotRepository ?? (_cuadraPlotRepository = new CuadraPlotRepository(_context)); }
        }

        public IManzanaPlotRepository ManzanaPlotRepository
        {
            get { return _manzanaPlotRepository ?? (_manzanaPlotRepository = new ManzanaPlotRepository(_context)); }
        }

        public ICallePlotRepository CallePlotRepository
        {
            get { return _callePlotRepository ?? (_callePlotRepository = new CallePlotRepository(_context)); }
        }

        public IInterfaseRentasLogRepository InterfaseRentasLogRepository
        {
            get { return _interfaseRentasLogRepository ?? (_interfaseRentasLogRepository = new InterfaseRentasLogRepository(_context)); }
        }

        public IImagenSatelitalRepository ImagenSatelitalRepository
        {
            //Singleton del repositorio 
            get { return _imagenSatelitalRepository ?? (_imagenSatelitalRepository = new ImagenSatelitalRepository(_context)); }
        }

        public IPloteoFrecuenteRepository PloteoFrecuenteRepository
        {
            get { return _ploteoFrecuenteRepository ?? (_ploteoFrecuenteRepository = new PloteoFrecuenteRepository(_context)); }
        }
        public IPloteoFrecuenteEspecialRepository PloteoFrecuenteEspecialRepository
        {
            get { return _ploteoFrecuenteEspecialRepository ?? (_ploteoFrecuenteEspecialRepository = new PloteoFrecuenteEspecialRepository(_context)); }
        }
        public IPlantillaViewportRepository PlantillaViewportRepository
        {
            get { return _plantillaViewportRepository ?? (_plantillaViewportRepository = new PlantillaViewportRepository(_context)); }
        }
        public ITipoViewportRepository TipoViewportRepository
        {
            get { return _tipoViewportRepository ?? (_tipoViewportRepository = new TipoViewportRepository(_context)); }
        }
        public ILayerViewportReposirory LayerViewportReposirory
        {
            get { return _layerViewportReposirory ?? (_layerViewportReposirory = new LayerViewportReposirory(_context)); }
        }

        public IExpansionPlotRepository ExpansionPlotRepository
        {
            get { return _expansionPlotRepository ?? (_expansionPlotRepository = new ExpansionPlotRepository(_context)); }
        }

        public ITipoPlanoRepository TipoPlanoRepository
        {
            get { return _tipoPlanoRepository ?? (_tipoPlanoRepository = new TipoPlanoRepository(_context)); }
        }

        public IPartidoRepository PartidoRepository
        {
            get { return _partidoRepository ?? (_partidoRepository = new PartidoRepository(_context)); }
        }

        public ICensoRepository CensoRepository
        {
            get { return _censoRepository ?? (_censoRepository = new CensoRepository(_context)); }
        }

        public IDeclaracionJuradaRepository DeclaracionJuradaRepository
        {
            get { return _declaracionJuradaRepository ?? (_declaracionJuradaRepository = new DeclaracionJuradaRepository(_context)); }
        }

        public IDesignacionRepository DesignacionRepository
        {
            get { return _designacionRepository ?? (_designacionRepository = new DesignacionRepository(_context)); }
        }
        public IValidacionDBRepository ValidacionDBRepository
        {
            get { return _validacionDBRepository ?? (_validacionDBRepository = new ValidacionDBRepository(_context)); }
        }

        public IRegistroPropiedadInmuebleRepository RegistroPropiedadInmuebleRepository
        {
            //Singleton del repositorio 
            get { return _RegistroPropiedadInmuebleRepository ?? (_RegistroPropiedadInmuebleRepository = new RegistroPropiedadInmuebleRepository(_context)); }
        }

        public IDivisionTemporalRepository DivisionTemporalRepository
        {
            //Singleton del repositorio division Temporal
            get { return _divisionTemporalRepository ?? (_divisionTemporalRepository = new DivisionTemporalRepository(_context)); }
        }

        public ICertificadoCatastralTemporalRepository CertificadoCatastralTemporalRepository
        {
            //Singleton del repositorio certificadoCatastral Temporal
            get { return _certificadoCatastralTemporalRepository ?? (_certificadoCatastralTemporalRepository = new CertificadoCatastralTemporalRepository(_context)); }
        }

        public ILibreDeDeudaTemporalRepository LibreDeDeudaTemporalRepository
        {
            //Singleton del repositorio libreDeDeudaTemporal
            get { return _libreDeDeudaTemporalRepository ?? (_libreDeDeudaTemporalRepository = new LibreDeDeudaTemporalRepository(_context)); }
        }

        public IDominioTemporalRepository DominioTemporalRepository
        {
            //Singleton del repositorio dominioTemporal
            get { return _dominioTemporalRepository ?? (_dominioTemporalRepository = new DominioTemporalRepository(_context)); }
        }

        public IUnidadTributariaTemporalRepository UnidadTributariaTemporalRepository
        {
            get { return _unidadTributariaTemporalRepository ?? (_unidadTributariaTemporalRepository = new UnidadTributariaTemporalRepository(_context)); }
        }
        public IDeclaracionJuradaTemporalRepository DeclaracionJuradaTemporalRepository
        {
            get { return _declaracionJuradaTemporalRepository ?? (_declaracionJuradaTemporalRepository = new DeclaracionJuradaTemporalRepository(_context)); }
        }

        public IVIRValuacionRepository VIRValuacionRepository
        {
            get { return _virValuacionRepository ?? (_virValuacionRepository = new VIRValuacionRepository(_context)); }
        }

        public IVIRInmuebleRepository VIRInmuebleRepository
        {
            get { return _virInmuebleRepository ?? (_virInmuebleRepository = new VIRInmuebleRepository(_context)); }
        }

        public IVIRVbEuCoefEstadoRepository VIRVbEuCoefEstadoRepository
        {
            get { return _virVbEuCoefEstadoRepository ?? (_virVbEuCoefEstadoRepository = new VIRVbEuCoefEstadoRepository(_context)); }
        }

        public IVIRVbEuUsoRepository VIRVbEuUsoRepository
        {
            get { return _virVbEuUsoRepository ?? (_virVbEuUsoRepository = new VIRVbEuUsoRepository(_context)); }
        }

        public IVIRVbEuTipoEdifRepository VIRVbEuTipoEdifRepository
        {
            get { return _virVbEuTipoEdifRepository ?? (_virVbEuTipoEdifRepository = new VIRVbEuTipoEdifRepository(_context)); }
        }

        public IVIREquivInmDestinosMejorasRepository VIREquivInmDestinosMejorasRepository
        {
            get { return _virEquivInmDestinosMejoras ?? (_virEquivInmDestinosMejoras = new VIREquivInmDestinosMejorasRepository(_context)); }
        }

        public IComprobantePagoRepository ComprobantePagoRepository
        {
            get { return _comprobantePagoRepository ?? (_comprobantePagoRepository = new ComprobantePagoRepository(_context)); }
        }

        public void Save(Auditoria auditoria)
        {
            SaveChanges(_context, auditoria);
        }
        public void Save()
        {
            SaveChanges(_context, null);
        }
        /// <summary>
        /// Wrapper for SaveChanges adding the Validation Messages to the generated exception
        /// </summary>
        /// <param name="context">The context.</param>
        private void SaveChanges(GeoSITMContext context, Auditoria auditoria)
        {
            try
            {
                context.SaveChanges(auditoria);
            }
            catch (DbEntityValidationException ex)
            {
                // Display database validation errors
                var sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" + sb, ex
                    ); // Add the original exception as the innerException
            }
        }

        public DbEntityEntry GetDbEntityEntry<T>(T entity) where T : class
        {
            return _context.Entry(entity);
        }
        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

    }
}