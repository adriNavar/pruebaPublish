using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace GeoSit.Client.Web.Models
{
    public class DeclaracionesJuradasModel
    {
        public bool ValuacionVigente { get; set; }
        public long IdUnidadTributaria { get; set; }
        public string PartidaInmobiliaria { get; set; }
        public decimal ValorTierra { get; set; }
        public DateTime VigenciaValorTierra { get; set; }
        public decimal? ValorMejoras { get; set; }
        public DateTime VigenciaValorMejoras { get; set; }
        public decimal ValorFiscalTotal { get; set; }
        public DateTime VigenciaValorFiscalTotal { get; set; }
        public string UltimoDecretoAplicado { get; set; }
        public string ListaDecretos { get; set; }
    }

    public class MantenimientoParcelarioModel
    {
        public long IdUnidadTributaria { get; set; }

        public string PartidaInmobiliaria { get; set; }
        public string Departamento { get; set; }
        public string Distrito { get; set; }
        public string Seccion { get; set; }
        public string Manzana { get; set; }
        public string Parcela { get; set; }


        public decimal ValorTierra { get; set; }
        public DateTime VigenciaValorTierra { get; set; }
        public string UltimoDecretoAplicado { get; set; }
        public decimal ValorMejoras { get; set; }
        public DateTime VigenciaValorMejoras { get; set; }
        public decimal ValorFiscalTotal { get; set; }
        public DateTime VigenciaValorFiscalTotal { get; set; }
    }

    public class DDJJModel
    {
        public long IdDeclaracionJurada { get; set; }
        public long IdVersion { get; set; }
        public string TipoDDJJ { get; set; }
        public string Tipo { get; set; }
        public object VigenciaDesde { get; set; }
        public object VigenciaHasta { get; set; }
        public string Version { get; set; }
        public string Valor { get; set; }
        public string Tramite { get; set; }
        public long? IdTramite { get; set; }
        public string Origen { get; set; }
    }

    public class ValuacionesModel : DeclaracionesJuradasModel
    {
        public long IdValuacion { get; set; }
        public long? IdDeclaracionJurada { get; set; }

        public DateTime FechaDesde { get; set; }

        public DateTime? FechaHasta { get; set; }

        public string Superficie { get; set; }

        public string DecretoAplicado { get; set; }

        public string Tramite { get; set; }

        public bool ReadOnly { get; set; }
    }

    public class DDJJValuacion : DDJJModel
    {
        public long IdValuacion { get; set; }
        public string Fecha { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public decimal ValorTierra { get; set; }
        public decimal? ValorMejoras { get; set; }
        public decimal VFT { get; set; }
        public double? Superficie { get; set; }
        public string Decreto { get; set; }
        public bool Vigente { get; set; }
    }


    public class DeclaracionesJuradasListaModel
    {
        public long UnidadTributariaId { get; set; }
        public List<DDJJModel> Lista { get; set; }
    }

    public class ValuacionesListaModel
    {
        public List<DDJJValuacion> Lista { get; set; }
    }

    public class SeleccionFormularioModel
    {
        public List<SelectListItem> Formularios { get; set; }
        public string Versiones { get; set; }
        public string FormularioSeleccionado { get; set; }
        public int IdVersion { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public long IdUnidadTributaria { get; set; }
        public bool ReadOnly { get; set; }
        public string PartidaInmobiliaria { get; set; }
        public int? IdTramite { get; set; }
        public string Poligono { get; set; }
        public string UnidadesTributariasOrigen { get; set; }
        public int? IdLocalidad { get; set; }
        public long? IdClaseParcela { get; set; }
    }

    public class InscripcionDominioModel
    {
        public long IdDominio { get; set; }
        public long IdDeclaracionJurada { get; set; }
        public long IdTipoInscripcion { get; set; }
        public string TipoInscripcion { get; set; }
        public IEnumerable<TipoInscripcion> TipoInscripciones { get; set; }
        public string Inscripcion { get; set; }
        public string Fecha { get; set; }
        public string FechaHora { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class PropietariosModel
    {
        public short IdDominioTitular { get; set; }
        public long IdDominio { get; set; }
        public long IdTipoTitularidad { get; set; }
        public string TipoPersona { get; set; }
        public SelectList TiposTitularidadList { get; set; }
        public long IdPersona { get; set; }
        public string NombreCompleto { get; set; }
        public string TipoNoDocumento { get; set; }
        public decimal PorcientoCopropiedad { get; set; }
        public string DomicilioFisico { get; set; }
        public decimal PorcientoCopropiedadTotal { get; set; }
        public bool IsDeleted { get; set; }

    }

    public class DomiciliosModel
    {
        public int Id { get; set; }
        public int PersonaId { get; set; }
        public string Tipo { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Barrio { get; set; }
        public string Calle { get; set; }
        public string Altura { get; set; }
        public string Piso { get; set; }
        public string Departamento { get; set; }
        public string CodigoPostal { get; set; }
    }

    public class ClasesParcelaModel
    {
        public int ClaseParcelaId { get; set; }
        public string Descripcion { get; set; }
    }

    public class FormularioSoRModel
    {
        public FormularioSoRModel()
        {
            DDJJ = new DDJJ();
            DDJJSor = new DDJJSor();
            DDJJDesignacion = new DDJJDesignacion();
            Tramite = new METramite();
            this.AptitudesDisponibles = new List<VALAptitudInput>();
        }

        public bool ReadOnly { get; set; }
        public DDJJ DDJJ { get; set; }
        public DDJJSor DDJJSor { get; set; }
        public DDJJDesignacion DDJJDesignacion { get; set; }
        public METramite Tramite { get; set; }
        public ParametrosGeneralesModel ParametrosGenerales { get; set; }

        // Cabecera
        public string PartidaInmobiliaria { get; set; }
        public List<SelectListItem> Departamentos { get; set; }
        public List<SelectListItem> Localidades { get; set; }
        public long IdUnidadTributaria { get; set; }



        // Rubro 1 Inc. C y D

        public string dominiosJSON { get; set; }
        public List<DDJJDominio> Dominios { get; set; }
        public string Mensura { get; set; }
        public List<DDJJSorOtrasCar> SorOtrasCar { get; set; }
        public long? IdClaseParcela { get; set; }

        // Rubro 1 Inc. B  

        // Rubro 5
        public string SuperficieTotal
        {
            get
            {
                return (AptitudesDisponibles ?? new List<VALAptitudInput>()).Sum(a => double.TryParse(a.Superficie, out double sup) ? sup : 0).ToString("0.0000");
            }
        }
        public List<SelectListItem> Relieves { get; set; }
        public List<SelectListItem> EspesoresCapaArable { get; set; }
        public List<SelectListItem> ColoresTierra { get; set; }
        public List<SelectListItem> AguasDelSubsuelo { get; set; }
        public List<SelectListItem> CapacidadesGanaderas { get; set; }
        public List<SelectListItem> EstadosMonte { get; set; }

        public List<SelectListItem> AllLocalidades { get; set; }

        // Esta es la lista de aptitudes disponibles para la versión de la ddjj.
        public List<VALAptitudInput> AptitudesDisponibles { get; set; }


        public List<SelectListItem> GetSelectItemsToValue(List<SelectListItem> lst, string val)
        {
            foreach (SelectListItem i in lst)
            {
                i.Selected = false;
                if (i.Value == val) i.Selected = true;
            }

            return lst;

        }


        public string LugarEmbarque { get; set; }
        public string CaminoMasProximo { get; set; }
        public List<SelectListItem> Caminos { get; set; }
    }

    public enum AptitudType
    {
        Alta = 2,
        MedianamenteAlta = 3,
        Baja = 4,
        MuyBaja = 5,
        Anegadiza = 6,
        AfloramientoTosca = 7,
        Lagunas = 8,
        Carrizales = 9,
        Monte = 10
    }

    public enum AptitudTypeInput
    {
        AllDropDowns = 1,
        OnlySuperficie = 2,
        OnlyDropEstado = 3
    }

    public class VALAptitudInput
    {
        public long IdAptitud { get; set; }

        public long? Numero { get; set; }

        public string Descripcion { get; set; }

        public string Superficie { get; set; }

        public AptitudTypeInput InputType { get; set; }



        public string RelieveSeleccionado { get; set; }

        public string EspesoresCapaArableSeleccionado { get; set; }

        public string ColoresTierraSeleccionado { get; set; }

        public string AguasDelSubsueloSeleccionado { get; set; }

        public string CapacidadesGanaderasSeleccionado { get; set; }

        public string EstadosMonteSeleccionado { get; set; }
    }

    public class FormularioUModel
    {

        public FormularioUModel()
        {
            DDJJ = new DDJJ();
            DDJJU = new DDJJU();
            DDJJDesignacion = new DDJJDesignacion();
            Tramite = new METramite();
        }
        public bool ReadOnly { get; set; }
        public DDJJ DDJJ { get; set; }
        public DDJJU DDJJU { get; set; }
        public DDJJDesignacion DDJJDesignacion { get; set; }
        public METramite Tramite { get; set; }

        public List<DDJJUOtrasCar> UOtrasCar { get; set; }

        // Cabecera
        public string PartidaInmobiliaria { get; set; }
        public List<SelectListItem> Departamentos { get; set; }
        public List<SelectListItem> Localidades { get; set; }
        public string PoligonoMensuraID { get; set; }

        public long IdUnidadTributaria { get; set; }

        // Rubro 1 Inc. C y D
        public string dominiosJSON { get; set; }
        public long UnidadTributariaId { get; set; }
        public string Mensura { get; set; }
        public List<GeoSit.Web.Api.Models.ClaseParcela> ClasesDisponibles { get; set; }

        public List<VALClasesParcelas> ClasesDisponiblesEdicion { get; set; }
        public long? IdClaseParcela { get; set; }

        // Rubro 1 Inc. B
        public string Calle { get; set; }
        public List<SelectListItem> Calles { get; set; }

        // Rubro 1 Inc. E y F
        //public bool AguaCorriente
        //{
        //    get { return DDJJU.AguaCorriente.HasValue && DDJJU.AguaCorriente.Value == 1; }
        //    set { DDJJU.AguaCorriente = value ? 1 : 0; }
        //}

        //public bool Cloaca
        //{
        //    get { return DDJJU.Cloaca.HasValue && DDJJU.Cloaca.Value == 1; }
        //    set { DDJJU.Cloaca = value ? 1 : 0; }
        //}

        // Rubro 2
        public List<GeoSit.Web.Api.Models.ClaseParcela> ClasesSeleccionada { get; set; }
        public List<SelectListItem> Clases { get; set; }
        public string CroquisBase64 { get; set; }
        public List<TipoMedidaLineal> TiposMedidaLineal { get; set; }
        public decimal? SuperficiePlano { get; set; }
        public decimal? SuperficieTitulo { get; set; }


        // Only save rubro 2
        public string ClasesJsonSerialized { get; set; }

    }


    public class CaracteristicaValue
    {
        public int Id { get; set; }

        public int ValorSeleccionado { get; set; }

    }

    public class FormularioE_BaseModel
    {

        public FormularioE_BaseModel()
        {
            Fachadas = new List<SelectListItem>();
            DDJJ = new DDJJ();
            Mejora = new INMMejora();
            Tramite = new METramite();
            OtrasCar = new List<MejoraOtrasCar>();
            EstadosConservacion = new List<EstadosConservacion>();
        }

        public bool ReadOnly { get; set; }

        // Cabecera

        public DDJJ DDJJ { get; set; }
        public INMMejora Mejora { get; set; }
        public METramite Tramite { get; set; }

        public List<MejoraOtrasCar> OtrasCar { get; set; }


        public string PartidaInmobiliaria { get; set; }

        public string DestinoEdificioSeleccionado { get; set; }

        public List<SelectListItem> DestinoEdificio { get; set; }

        public string PoligonoMensuraID { get; set; }


        public DateTime FechaVigencia { get; set; }

        // Rubro 1: Caracteristicas


        public List<INMTipoCaracteristica> TipoCaracteristicas { get; set; }

        public List<INMInciso> Incisos { get; set; }

        public List<INMCaracteristica> Caracteristicas { get; set; }

        public List<INMMejoraCaracteristica> MejoraCaracteristicas { get; set; }

        public List<long> CaracteristicasToSave { get; set; }


        public List<CaracteristicaValue> CaracteristicasSeleccionadas { get; set; }

        public List<SelectListItem> Fachadas { get; set; }

        public string FachadaSeleccionada { get; set; }


        // Rubro 2

        public List<EstadosConservacion> EstadosConservacion { get; set; }
        public int EstadoConservadoEdificio { get; set; }
        public List<INMCaracteristica> GetOptionsValuesCaracteristicas(INMTipoCaracteristica t, INMInciso i)
        {
            List<INMCaracteristica> result = Caracteristicas.Where(x => x.IdInciso.Equals(i.IdInciso) && x.IdTipoCaracteristica.Equals(t.IdTipoCaracteristica)).OrderBy(x => x.Descripcion).ToList();
            //result.Insert(0, new INMCaracteristica() { Descripcion = i.Descripcion, IdCaracteristica = -1 });

            return result;
        }

        public string IsSelected(INMCaracteristica t)
        {
            List<INMMejoraCaracteristica> result = this.MejoraCaracteristicas.Where(x => x.IdCaracteristica.Equals(t.IdCaracteristica)).ToList();

            if (result.Count > 0)
            {
                return " selected ";
            }

            return string.Empty;
        }

        public bool IsAnySelected(List<INMCaracteristica> lst)
        {
            List<INMCaracteristica> result = Caracteristicas.Where(w => lst.Any(b => b.IdCaracteristica.Equals(w.IdCaracteristica))).ToList();

            if (result.Count > 0)
            {
                return true;
            }

            return false;
        }

    }

    public class MejoraOtrasCar
    {
        public long IdOtraCar { get; set; }
        public string Descripcion { get; set; }
        public decimal? Valor { get; set; }
        public bool Requerido { get; set; }
        public long? Orden { get; set; }
        public bool EsReal { get; set; }
        public bool? Hidden { get; set; }
    }

    public class ActualizacionDecretoModel
    {
        public long idDecreto { get; set; }
        public List<SelectListItem> DecretosList { get; set; }
        public bool IsRunning { get; set; }
    }

    public enum EstadoEdificio : int
    {
        Bueno = 1,
        Regular = 2,
        Malo = 3
    }

    public class FormularioE1Model : FormularioE_BaseModel
    {

        public FormularioE1Model() : base()
        {
            this.MejoraCaracteristicas = new List<INMMejoraCaracteristica>();
        }


    }


    public class FormularioE2Model : FormularioE_BaseModel
    {
        public FormularioE2Model() : base()
        {
            this.MejoraCaracteristicas = new List<INMMejoraCaracteristica>();
        }


    }

    public class DDJJInicio
    {
        public long? IdUnidadTributaria { get; set; }
        public long? IdTipoParcela { get; set; }
        public int? IdTipoUnidadTributaria { get; set; }
        public int IdLocalidad { get; set; }
        public string PartidaInmobiliaria { get; set; }
        public string Poligono { get; set; }
        public long? IdClaseParcela { get; set; }
        public IEnumerable<DominioPrecarga> DominiosOriginales { get; set; }
    }

    public class DominioPrecarga
    {
        public bool EsParcela { get; set; }
        public long Id { get; set; }
    }
}