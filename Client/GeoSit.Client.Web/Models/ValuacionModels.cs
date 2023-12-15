using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel;
using GeoSit.Data.BusinessEntities.Valuaciones;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Client.Web.Models.ResponsableFiscal;

namespace GeoSit.Client.Web.Models
{
    public class ValuacionModel 
    {
        public ValuacionModel()
        {
            TierraPorSuperficie = new TierraPorSuperficieModel();
            TierraPorModulo = new TierraPorModuloModel();
            MejoraValoresBasicos = new MejoraValoresBasicosModel();
            MetadatoCoeficienteTierra = new MetadatoCoeficienteTierraModel();
            TipoCoeficienteTierra = new TipoCoeficienteTierraModel();
            ValorCoeficiente = new ValorCoeficienteModel();
            RangoCoeficiente = new RangoCoeficienteModel();
            ValorCoeficienteMejora = new ValorCoeficienteMejoraModel();
            RangoCoeficienteMejora = new RangoCoeficienteMejoraModel();
            MetadatoMejoraCoeficiente = new MetadatoMejoraCoeficienteModel();
            TipoCoeficienteMejora = new TipoCoeficienteMejoraModel();
            Parcela = new ParcelaModel();
            BusquedaPadron = new  BusquedaPadronModel();
            TipoParcela = new TipoParcelaModel();
            TierraPorStore = new TierraPorStoreModel();
            MejoraPorStore = new MejoraPorStoreModel();
            CoeficienteStore = new CoeficienteStoreModel();
            EdicionParcela = new EdicionParcelaModel();            
            MetodoValuacionMejora = "";
            Mensaje = "";
        }
        public string Mensaje { get; set; }
        public String IdFiltroParcela { get; set; }
        public long TipoValuacionId { get; set; }
        public String MetodoValuacionMejora { get; set; }
        
        public TierraPorModuloModel TierraPorModulo { get; set; }
        public TierraPorStoreModel TierraPorStore { get; set; }
        public MejoraPorStoreModel MejoraPorStore { get; set; }
        public CoeficienteStoreModel CoeficienteStore { get; set; }
        public TierraPorSuperficieModel TierraPorSuperficie { get; set; }
        public MetadatoMejoraCoeficienteModel MetadatoMejoraCoeficiente { get; set; }
        public MejoraValoresBasicosModel MejoraValoresBasicos { get; set; }
        public Decimal SuperficieSemiCubiertaPorcentaje { get; set; }
        public MetadatoCoeficienteTierraModel MetadatoCoeficienteTierra { get; set; }
        public TipoCoeficienteTierraModel TipoCoeficienteTierra { get; set; }
        public ValorCoeficienteModel ValorCoeficiente { get; set; }
        public RangoCoeficienteModel RangoCoeficiente { get; set; }
        public TipoCoeficienteMejoraModel TipoCoeficienteMejora { get; set; }
        public ValorCoeficienteMejoraModel ValorCoeficienteMejora { get; set; }
        public RangoCoeficienteMejoraModel RangoCoeficienteMejora { get; set; }
        public ParcelaModel Parcela { get; set; }
        public BusquedaPadronModel BusquedaPadron { get; set; }
        public TipoParcelaModel TipoParcela { get; set; }
        public EdicionParcelaModel EdicionParcela { get; set; }
        public ResponsableFiscalViewModel[] ResponsablesFiscales { get; set; }
    }

   
    public class TierraPorSuperficieModel
    {

        public TierraPorSuperficieModel()
        {

            Zona = new List<long>();
            Valor = new List<Decimal>();
            
            Via = new List<string>();
            Eje_Via = new List<string>();
            NombreVia = new List<string>();
            Altura_Desde = new List<string>();
            Altura_Hasta = new List<string>();
            Paridad = new List<string>();

        }
        public long IdTierraSuperficie { get; set; }
        public List<long> Zona { get; set; }
        
        public List<string> Via { get; set; }
        public List<string> Eje_Via { get; set; }
        public List<Decimal> Valor { get; set; }
        public List<string> NombreVia { get; set; }
        public List<string> Altura_Desde { get; set; }
        public List<string> Altura_Hasta { get; set; }
        public List<string> Paridad { get; set; }
        
        

    }
    public class CoeficienteStoreModel
    {
        public CoeficienteStoreModel()
        {

            Parametro1 = new List<String>();
            Parametro1Desde = new List<String>();
            Parametro1Hasta = new List<String>();
            Parametro2 = new List<String>();
            Parametro2Desde = new List<String>();
            Parametro2Hasta = new List<String>();
            Cantidad = new List<String>();
            ValorComparacion1 = new List<String>();
            ValorComparacion2 = new List<String>();

        }
        public List<String> Parametro1 { get; set; }
        public List<String> Parametro1Desde { get; set; }
        public List<String> Parametro1Hasta { get; set; }
        public List<String> Parametro2 { get; set; }
        public List<String> Parametro2Desde { get; set; }
        public List<String> Parametro2Hasta { get; set; }
        public List<String> Cantidad { get; set; }
        public List<String> ValorComparacion1 { get; set; }
        public List<String> ValorComparacion2 { get; set; }
        public String TipoParametro1 { get; set; }
        public String TipoParametro2 { get; set; }


    }
    public class TierraPorStoreModel
    {
        public TierraPorStoreModel()
        {

            Parametro1 = new List<String>();
            Parametro1Desde = new List<String>();
            Parametro1Hasta = new List<String>();
            Parametro2 = new List<String>();
            Parametro2Desde = new List<String>();
            Parametro2Hasta = new List<String>();
            Cantidad = new List<String>();
            ValorComparacion1 = new List<String>();
            ValorComparacion2 = new List<String>();

        }
        public List<String> Parametro1 { get; set; }
        public List<String> Parametro1Desde { get; set; }
        public List<String> Parametro1Hasta { get; set; }
        public List<String> Parametro2 { get; set; }
        public List<String> Parametro2Desde { get; set; }
        public List<String> Parametro2Hasta { get; set; }
        public List<String> Cantidad { get; set; }
        public List<String> ValorComparacion1 { get; set; }
        public List<String> ValorComparacion2 { get; set; }
        public String TipoParametro1 { get; set; }
        public String TipoParametro2 { get; set; }
        

    }

    public class MejoraPorStoreModel
    {
        public MejoraPorStoreModel()
        {

            Parametro1 = new List<string>();
            Parametro1Desde = new List<string>();
            Parametro1Hasta = new List<string>();
            Parametro2 = new List<string>();
            Parametro2Desde = new List<string>();
            Parametro2Hasta = new List<string>();
            Cantidad = new List<string>();
            ValorComparacion1 = new List<string>();
            ValorComparacion2 = new List<string>();

        }
        public List<string> Parametro1 { get; set; }
        public List<string> Parametro1Desde { get; set; }
        public List<string> Parametro1Hasta { get; set; }
        public List<string> Parametro2 { get; set; }
        public List<string> Parametro2Desde { get; set; }
        public List<string> Parametro2Hasta { get; set; }
        public List<string> Cantidad { get; set; }
        public List<string> ValorComparacion1 { get; set; }
        public List<string> ValorComparacion2 { get; set; }
        public string TipoParametro1 { get; set; }
        public string TipoParametro2 { get; set; }
        

    }
    public class TierraPorModuloModel
    {
        public TierraPorModuloModel()
        {

         Zona = new List<long>();
         Superficie = new List<string>();
         SuperficieDesde = new List<string>();
         SuperficieHasta = new List<string>();
         CantidadModulos = new List<long>();
         ValorComparacion = new List<string>();

        }
        public long IdTierraModulo { get; set; }
        public List<long> Zona { get; set; }
        public List<string> Superficie { get; set; }
        public List<string> SuperficieDesde { get; set; }
        public List<string> SuperficieHasta { get; set; }
        public List<long> CantidadModulos { get; set; }
        public List<string> ValorComparacion { get; set; }
    }

 
    public class MejoraValoresBasicosModel
    {
        public MejoraValoresBasicosModel()
        {

            ValorBasicoMejora1 = new List<string>();
            ValorBasicoMejora2 = new List<string>();
            ValorBasicoMejora3 = new List<Decimal>();

        }
        public long IdMejoraValorBasico { get; set; }
        public List<string> ValorBasicoMejora1 { get; set; }
        public List<string> ValorBasicoMejora2 { get; set; }
        public List<Decimal> ValorBasicoMejora3 { get; set; }

    }

    public class MetadatoMejoraCoeficienteModel
    {
        public long IdMejoraCoeficiente { get; set; }
        

    }

  
    public class MetadatoCoeficienteTierraModel
    {
        public long IdMetadatoCoeficienteTierra { get; set; }
        
    }

    public class TipoCoeficienteTierraModel
    {
        //[Required(ErrorMessage="El nombre del coeficiente es requerido")]
        
        public string IdTipoCoeficienteTierra { get; set; }
        public String TipoCoeficiente { get; set; }
    }

    public class ValorCoeficienteModel
    {
        public ValorCoeficienteModel()
        {
            Valor1Coeficiente = new List<string>();
            Valor2Coeficiente = new List<string>();
            Valor3Coeficiente = new List<Decimal>();
        }
        public long IdValorCoeficiente { get; set; }
        public List<string> Valor1Coeficiente { get; set; }
        public List<string> Valor2Coeficiente { get; set; }
        public List<Decimal> Valor3Coeficiente { get; set; }

    }

    public class RangoCoeficienteModel
    {
        public RangoCoeficienteModel()
        {
            Rango1Coeficiente = new List<string>();
            Rango2Coeficiente = new List<string>();
            Rango3Coeficiente = new List<Decimal>();
        }
        public long IdRangoCoeficiente { get; set; }
        public List<string> Rango1Coeficiente { get; set; }
        public List<string> Rango2Coeficiente { get; set; }
        public List<Decimal> Rango3Coeficiente { get; set; }

    }

    public class TipoCoeficienteMejoraModel
    {
        //[Required(ErrorMessage = "El nombre del coeficiente es requerido")]

        public string IdTipoCoeficienteMejora { get; set; }
        public String TipoCoeficiente { get; set; }

    }

    public class ValorCoeficienteMejoraModel
    {
        public ValorCoeficienteMejoraModel()
        {
            Valor1CoeficienteMejora = new List<string>();
            Valor2CoeficienteMejora = new List<string>();
            Valor3CoeficienteMejora = new List<Decimal>();
        }
        public long IdValorCoeficienteMejora { get; set; }
        public List<string> Valor1CoeficienteMejora { get; set; }
        public List<string> Valor2CoeficienteMejora { get; set; }
        public List<Decimal> Valor3CoeficienteMejora { get; set; }

    }

    public class RangoCoeficienteMejoraModel
    {
        public RangoCoeficienteMejoraModel()
        {
            Rango1CoeficienteMejora = new List<string>();
            Rango2CoeficienteMejora = new List<string>();
            Rango3CoeficienteMejora = new List<float>();
        }
        public long IdRangoCoeficienteMejora { get; set; }
        public List<string> Rango1CoeficienteMejora { get; set; }
        public List<string> Rango2CoeficienteMejora { get; set; }
        public List<float> Rango3CoeficienteMejora { get; set; }

    }

    public class ParcelaModel
    {
        public long Tipo_Parcela { get; set; }
        public long Id_Inmueble { get; set; }
        public string Partida { get; set; }
        public string Ejido { get; set; }
        public string Sector { get; set; }
        public string Fraccion { get; set; }
        public string Circunscripcion { get; set; }
        public string Manzana { get; set; }
        public string Parcela { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Nomenclatura { get; set; }
    }
    public class EdicionParcelaModel
    {
        public EdicionParcelaModel()
        {
            C1M = 1;
            C2M = 1;
            C3M = 1;
            C4M = 1;
        }
        public long IdPadron { get; set; }
        public long UnidadTributariaID { get; set; }
        public String Tipo_Parcela { get; set; }
        public long Id_Parcela { get; set; }
        public long? Id_Zona_Atributo { get; set; }
        public string Nomenclatura { get; set; }
        public String Desc_Zona_Atributo { get; set; }
        public string Partida { get; set; }
        public string Titular { get; set; }
        public bool ValModulos { get; set; }
        public List<ValuacionPadronDetalle> listaVal { get; set; } // esto tiene todas las valuaciones existentes

        public List<ValuacionPadronDetalle> listaPadrones { get; set; } // esto tiene solo una valuacion por cada padron, solo para la grilla de valuaciones!
        public List<UnidadTributaria> listaUts { get; set; }

        public Decimal C1T { get; set; }
        public Decimal C2T { get; set; }
        public Decimal C3T { get; set; }
        public Decimal C4T { get; set; }

        public float CantModulos { get; set; }
        public Decimal ValorMetro2 { get; set; }
        public float Medida { get; set; }
        public Decimal ValorTierra { get; set; }
        public Decimal ValorMejoras { get; set; }

        public Decimal ValorBasicoMejora { get; set; }
        public Decimal C1M { get; set; }
        public Decimal C2M { get; set; }
        public Decimal C3M { get; set; }
        public Decimal C4M { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime VigenciaDesde { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? VigenciaHasta { get; set; }

        //public Decimal ValorBasicoMejora { get; set; }
        public Decimal PorcentajeCodominio { get; set; }
        public Decimal CoefSemiC { get; set; }
        public List<ValuacionMejoraModel> listaMejoras { get; set; }
    }

    public class ValuacionMejoraModel
    {
        public long MejoraID { get; set; }
        public long? UnidadTributariaID { get; set; }
        public long ParcelaID { get; set; }
        public long SubCategoriaID { get; set; }
        public String Parametro2 { get; set; }
        public String UnidadMedida { get; set; }
        public String Parametro1 { get; set; }
        public Decimal Medida { get; set; }
        public Decimal MedidaSemiCubierta { get; set; }
        public int? Anio { get; set; }
        public Decimal Valor { get; set; }
        public Decimal ValorBasico { get; set; }
        public Decimal C1 { get; set; }
        public Decimal C2 { get; set; }
        public Decimal C3 { get; set; }
        public Decimal C4 { get; set; }
        public Decimal CoeficienteSemi { get; set; }
        public bool eliminar { get; set; }
        public Parcela Parcela { get; set; }
        public UnidadTributaria UnidadTributaria { get; set; } 
    }

    public class BusquedaPadronModel
    {
        public BusquedaPadronModel() {

            VigenciaDesde = System.DateTime.Now;
            VigenciaHasta = System.DateTime.Now;
        }

        public long IdValuacionPadronTemporal { get; set; }
        public long IdPadron { get; set; }
        public long CantInmuebles { get; set; }
        public long CantErrores { get; set; }
        public string ErroresPath { get; set; }
        
        public DateTime VigenciaDesde { get; set; }
        public DateTime VigenciaHasta { get; set; }

        public float SumaSuperficieTierra { get; set; }
        public float SumaSuperificeCubierta { get; set; }
        public float SumaSuperficieSemiCubierta { get; set; }
        public Decimal SumaValorTierra { get; set; }
        public Decimal SumaValorMejora { get; set; }
        public Decimal SumaValorTotal { get; set; }

        public float MaxSuperficieTierra { get; set; }
        public float MaxSuperificeCubierta { get; set; }
        public float MaxSuperficieSemiCubierta { get; set; }
        public Decimal MaxValorTierra { get; set; }
        public Decimal MaxValorMejora { get; set; }
        public Decimal MaxValorTotal { get; set; }

        public float MinSuperficieTierra { get; set; }
        public float MinSuperificeCubierta { get; set; }
        public float MinSuperficieSemiCubierta { get; set; }
        public Decimal MinValorTierra { get; set; }
        public Decimal MinValorMejora { get; set; }
        public Decimal MinValorTotal { get; set; }

        public float PromSuperficieTierra { get; set; }
        public float PromSuperificeCubierta { get; set; }
        public float PromSuperficieSemiCubierta { get; set; }
        public Decimal PromValorTierra { get; set; }
        public Decimal PromValorMejora { get; set; }
        public Decimal PromValorTotal { get; set; }

        public float NulosSuperficieTierra { get; set; }
        public float NulosSuperificeCubierta { get; set; }
        public float NulosSuperficieSemiCubierta { get; set; }
        public float NulosValorTierra { get; set; }
        public float NulosValorMejora { get; set; }
        public float NulosValorTotal { get; set; }

        public long idUsuario { get; set; }
        public bool esConsolidado { get; set; }

    }

    public class TipoParcelaModel
    {
        public long TipoParcelaID { get; set; }
        public string Descripcion { get; set; }

    }

}