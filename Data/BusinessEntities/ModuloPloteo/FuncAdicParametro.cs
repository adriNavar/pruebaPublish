
namespace GeoSit.Data.BusinessEntities.ModuloPloteo
{
    public class FuncAdicParametro
    {
        public int IdFuncAdicParametro { get; set; }

        public int IdFuncionAdicional { get; set; }

        public string Campo { get; set; }

        public string Valor { get; set; }

        public string Descripcion { get; set; }

        public FuncionAdicional FuncionAdicional { get; set; }
    }
}
