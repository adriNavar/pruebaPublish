
using GeoSit.Data.BusinessEntities.Inmuebles;
using System;

namespace GeoSit.Data.BusinessEntities.LogicalTransactionUnits
{
    public class UnidadAlfanumericoParcela
    {
        public int Operacion { get; set; }
        public string Expediente { get; set; }
        public DateTime? Fecha { get; set; }
        public long IdJurisdiccion { get; set; }
        public DateTime? Vigencia { get; set; }
        public long IdUsuario { get; set; }

        public Operaciones<Parcela> OperacionesParcelasOrigenes { get; set; }

        public Operaciones<Parcela> OperacionesParcelasDestinos { get; set; }

        public UnidadAlfanumericoParcela()
        {
            OperacionesParcelasOrigenes = new Operaciones<Parcela>();
            OperacionesParcelasDestinos = new Operaciones<Parcela>();
        }

        public void Clear()
        {
            OperacionesParcelasOrigenes.Clear();
            OperacionesParcelasDestinos.Clear();
        }
    }
}
