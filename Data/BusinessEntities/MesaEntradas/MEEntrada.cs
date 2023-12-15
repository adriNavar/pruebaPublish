using GeoSit.Data.BusinessEntities.ILogicInterfaces;
using System;

namespace GeoSit.Data.BusinessEntities.MesaEntradas
{
    public class MEEntrada : IBajaLogica
    {
        //public static readonly int Parcela = Convert.ToInt32(GlobalResources.Entradas.Parcela);
        //public const string UnidadFuncional = "Unidad funcional";
        //public const string MensuraRegistrada = "Mensura registrada";
        //public const string DescripcionDelInmueble = "Descripción del inmueble";
        //public const string DDJJ = "DDJJ";
        //public const string Manzana = "Manzana";
        //public const string Persona = "Persona";
        //public const string Via = "Vía";
        //public const string LibreDeDeuda = "Libre de deuda";
        //public const string Titulo = "Título";
        //public const string ComprobanteDePago = "Comprobante de pago";

        public int IdEntrada { get; set; }
        public string Descripcion { get; set; }
        public long UsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public long UsuarioModif { get; set; }
        public DateTime FechaModif { get; set; }
        public long? UsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
        public bool EsGrafico { get; set; }
        public long IdComponente { get; set; }
    }
}
