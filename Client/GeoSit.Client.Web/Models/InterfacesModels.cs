using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using GeoSit.Data.BusinessEntities.Interfaces;
using GeoSit.Data.Intercambio.BusinessEntities;

namespace GeoSit.Client.Web.Models
{
    public class InterfacesModel 
    {
        public InterfacesModel()
        {
            //ActualizacionPadrones = new ActualizacionPadronesModel();
          
        }
        //public ActualizacionPadronesModel ActualizacionPadrones { get; set; }
        
        
    }

    public class ActualizacionPadronesModel
    {
        public List<TransaccionesPendientes> ListaTransaccionesPendientes { get; set; }
        public List<long> Id_Transaccion { get; set; }
        
    }

    public class DGCeITModel
    {
        public DGCeITModel()
        {
            InterfaseDGCeIT = new InterfaseDGCeITLocalModel();
            EntradaParcela = new EntradaParcelaLocalModel();
            ResultadoParcelas = new List<ResultadoParcelasLocalModel>();
            EntradaPartida = new EntradaPartidaLocalModel();
            ErrorXML = new ErrorXMLModel();
            CantElementos = new CantElementosModel();
            mensaje = "";
        }
        public string mensaje { get; set; }
        public InterfaseDGCeITLocalModel InterfaseDGCeIT { get; set; }
        public EntradaParcelaLocalModel EntradaParcela { get; set; }
        public List<ResultadoParcelasLocalModel> ResultadoParcelas { get; set; }
        public EntradaPartidaLocalModel EntradaPartida { get; set; }
        public ErrorXMLModel ErrorXML { get; set; }
        public CantElementosModel CantElementos { get; set; }

    }

    public class InterfaseDGCeITLocalModel
    {
        public long Id_InterfaseDGCeIT { get; set; }

    }

    public class EntradaParcelaLocalModel
    {
        public long? circ { get; set; }
        public long? seccion { get; set; }
        public long? sector { get; set; }
        public string tipoDiv { get; set; }
        public long? valorTipo { get; set; }
        public string parcela { get; set; }
        public DateTime? fechaDesdeParcela { get; set; }
        public DateTime? fechaHastaParcela { get; set; }
    }

    public class ResultadoParcelasLocalModel
    {

        //Inicio Parcela
        public string TipoActualizacion { get; set; }
        public long FeatId { get; set; }
        public long IdClaseParcela { get; set; }
        public string DescriClaseParcela { get; set; }
        public long Numero {get; set;}
        public string Letra { get; set; }
        public long ParDescriptor { get; set; }
        public float SupGrafico { get; set; }
        public float SupMensura { get; set; }
        public float SupTitulo { get; set; }
        public float SupCenso { get; set; }
        public long UnidadSupMensura { get; set; }
        public long DepDescriptor { get; set; }
        public string NombreDepto { get; set; }
        public long EjiDescriptor { get; set; }
        public long CirDescriptor { get; set; }
        public long SecDescriptor { get; set; }
        public long SctDescriptor { get; set; }
        public long DivDescriptor { get; set; }
        public long IdTipoDivision { get; set; }
        public long ColDescriptor { get; set; }
        public long DepFeatId { get; set; }
        public long EjiFeatId { get; set; }
        public long CirFeatId { get; set; }
        public long SecFeatId { get; set; }
        public long SctFeatId { get; set; }
        public long DivFeatId { get; set; }
        public long ColFeatId { get; set; }
        public long NombreEst { get; set; }
        public string NcaDepto { get; set; }
        public string NcaPueblo { get; set; }
        public string NcaSeccion { get; set; }
        public string NcaFraccion { get; set; }
        public string NcaFracRural { get; set; }
        public string NcaChacra { get; set; }
        public string NcaFracUrbana { get; set; }
        public string NcaQuinta { get; set; }
        public string NcaSeccionUrbana { get; set; }
        public string NcaSolar { get; set; }
        public string NcaMacizo { get; set; }
        public string NcaManzana { get; set; }
        public string NcaColonia { get; set; }
        public string NcaBarrio { get; set; }
        public string NcaParcela { get; set; }
        public string NcaLote { get; set; }
        public string NcaNomenclaAnterior { get; set; }
        public string NcaDivision { get; set; }
        public long NcaTipoDivision { get; set; }
        public string NvaDepto { get; set; }
        public string NvaCircunscripcion { get; set; }
        public string NvaSector { get; set; }
        public string NvaParcela { get; set; }
        public string NplDepDescriptor { get; set; }
        public string NplEjiDescriptor { get; set; }
        public string NplCirDescriptor { get; set; }
        public string NplSecDescriptor { get; set; }
        public string NplSctDescriptor { get; set; }
        public string NplDivDescriptor { get; set; }
        public string NplIdTipoDivision { get; set; }
        public string NplDivDescriptor2 { get; set; }
        public string NplIdTipoDivision2 { get; set; }
        public string NplColDescriptor { get; set; }
        public string NplParcela { get; set; }
        public string NplTipoLote { get; set; }
        public string NplLote { get; set; }
        public string NplLegua { get; set; }
        public string NplTipoLoteDesc { get; set; }
        public DateTime FechaAlta { get; set; }
        public string FechaModificacion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public long IdUsuarioAlta { get; set; }
        public long IdUsuarioModificacion { get; set; }
        public long IdUsuarioBaja { get; set; }
        public long IdEstado { get; set; }
        public long IdFuente { get; set; }
        public long IdTipoParcela { get; set; }
        public long IdTipoEjido { get; set; }
        public string ExpCreacion { get; set; }
        public string ExpBaja { get; set; }
        public string DescMedidas { get; set; }
        public string DescLinderos { get; set; }
        public bool HasGeometry { get; set; }
        public long GType { get; set; }
        public string NroPartida { get; set; }
        public long Animales { get; set; }
        public long Frente { get; set; }
        public long Fondo { get; set; }
        public long ValorTierra { get; set; }
        public long ValorInmueble { get; set; }
        public long Caption { get; set; }
        public string Nomenclatura { get; set; }
        public string NomenclaturaNCA { get; set; }
        public string TipoDivision { get; set; }
        public string NcaTipoDivisionDesc { get; set; }
        public string Estado { get; set; }
        public string CaptionNomenc { get; set; }
        public long CantUnidades { get; set; }
        public long CantUnidadesAfectadas { get; set; }
        public long parDelegActual { get; set; }
        public long cpaCodigo { get; set; }
        public long IdUsResponsable { get; set; }
        public DateTime FechaHoraIni { get; set; }
        public long IdExpeTemp { get; set; }
        public string FeatIdsOrigen { get; set; }
        public long EstadoParcela { get; set; }
        public long CantUnidadesGuardadas { get; set; }
        public long NumeroPartida { get; set; }
        public string FraccionRural { get; set; }
        public string Chacra { get; set; }
        public string FraccionUrbana { get; set; }
        public string Quinta { get; set; }
        public string SeccionUrbana { get; set; }
        public string Solar { get; set; }
        public string Macizo { get; set; }
        public string Manzana { get; set; }
        public string Colonia { get; set; }
        public string Barrio { get; set; }
        //Fin Parcela

        //Inicio Partida
        public long Id { get; set; }
        //public long Numero { get; set; }
        public long DigVer { get; set; }
        public long PorcCopropiedad { get; set; }
        //public string ExpCreacion { get; set; }
        //public string ExpBaja { get; set; }
        public DateTime VigDesde { get; set; }
        public DateTime VigHasta { get; set; }
        public long Tomo { get; set; }
        public long Folio { get; set; }
        public long Finca { get; set; }
        public long Matricula { get; set; }
        public string ExpAdjudicacion { get; set; }
        public string Ley { get; set; }
        public long TipoInscripcion { get; set; }
        public long NroInscripcion { get; set; }
        //public DateTime FechaAlta { get; set; }
        //public string FechaModificacion { get; set; }
        //public DateTime FechaBaja { get; set; }
        //public long IdUsuarioAlta { get; set; }
        //public long IdUsuarioModificacion { get; set; }
        //public long IdUsuarioBaja { get; set; }
        //public long IdFuente { get; set; }
        public long FeatIdParcela { get; set; }
        public long IdTipoUnidad { get; set; }
        public string TipoOperacion { get; set; }
        public string ExpOperacion { get; set; }
        
        //Inicio Domicilio
        //public long Id { get; set; }
        public long NroPuerta { get; set; }
        public string Piso { get; set; }
        public string Depto { get; set; }
        public string Ubicacion { get; set; }
        public string CPA { get; set; }
        public long CodigoPostal { get; set; }
        //public long DepDescriptor { get; set; }
        //public long EjiDescriptor { get; set; }
        public string Localidad { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Ejido { get; set; }
        public string Pais { get; set; }
        public long CodigoPais { get; set; }
        public long IdTipo { get; set; }
        public long IdPartida { get; set; }
        public long FeatIdEjeCalle { get; set; }
        public string Observaciones { get; set; }
        //public DateTime FechaAlta { get; set; }
        //public string FechaModificacion { get; set; }
        //public DateTime FechaBaja { get; set; }
        //public long IdUsuarioAlta { get; set; }
        //public long IdUsuarioModificacion { get; set; }
        //public long IdUsuarioBaja { get; set; }
        public long IdCalle { get; set; }
        public string Calle { get; set; }
        public long IdDocumento { get; set; }
        //Fin Domicilio

        public long PdaPH { get; set; }
        public string UnidadFuncional { get; set; }
        public long Copropiedad { get; set; }
        public string DescInmueble { get; set; }
        public long TributaEn { get; set; }
        public long IdTipoInmueble { get; set; }
        public string TipoInmueble { get; set; }
        public long IdExencionImpositiva { get; set; }

        //Inicio Valuacion
        //public long Id { get; set; }
        //public long IdPartida { get; set; }
        public long IdCompTierra { get; set; }
        public long IdTipoTierra { get; set; }
        //public long FeatIdParcela { get; set; }
        public DateTime VigenciaDesde { get; set; }
        public string VigenciaHasta { get; set; }
        //public float Copropiedad { get; set; }
        //public long ValorTierra { get; set; }
        public float ValorTierraProp { get; set; }
        public float ValorMejoras { get; set; }
        public float ValorMejorasProp { get; set; }
        public long ValorMejorasExclusivo { get; set; }
        //public float ValorInmueble { get; set; }
        //public string Observaciones { get; set; }
        public long IdMonedaTierra { get; set; }
        public string MonedaTierra { get; set; }
        public long IdMonedaMejoras { get; set; }
        public string MonedaMejoras { get; set; }
        public long SuperficieTierra { get; set; }
        public long SuperficieMejoras { get; set; }
        public long CodExenImp { get; set; }
        public string DescriCodExenImp { get; set; }
        public string Origen { get; set; }
        //public DateTime FechaAlta { get; set; }
        //public long IdUsuarioAlta { get; set; }
        //public DateTime FechaBaja { get; set; }
        //public long IdUsuarioBaja { get; set; }
        public long AnimalesLegua { get; set; }
        public long CoefFrenteFondo { get; set; }
        public long CoefReceptividad { get; set; }
        public string Base { get; set; }
        //Fin Valuacion

        public long SupCubierta { get; set; }
        public long SupSemicubierta { get; set; }
        public long SupDescubierta { get; set; }
        public string Poligono { get; set; }
        public string TipoUnidad { get; set; }
        //public string Estado { get; set; }
        //public long Ejido { get; set; }
        //public string Departamento { get; set; }
        public long DepartamentoDesc { get; set; }
        public long EstadoTemp { get; set; }
        //public bool HasGeometry { get; set; }
        //Fin Partida
    }

    public class EntradaPartidaLocalModel
    {
        public long? partidaDesde { get; set;}
        public long? partidaHasta{ get; set; }
        public DateTime? fechaDesdePartida { get; set; }
        public DateTime? fechaHastaPartida { get; set; }

    }

    public class ErrorXMLModel
    {
        public long codigo { get; set; }
        public string titulo { get; set; }
        public string descripcion { get; set; }
        
    }

    public class CantElementosModel
    {
        public string elemento { get; set; }
        public long cantidad { get; set; }

    }

    public class DiferenciasModel
    {
        public List<Diferencia> ListaDiferencias { get; set; }
        public List<Estado> ListaEstados { get; set; }
    }
}
