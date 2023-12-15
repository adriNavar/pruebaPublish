namespace GeoSit.Data.BusinessEntities.Inmuebles
{
    public class ParcelaSuperficies
    {
        public DGCMejorasConstrucciones DGCMejorasConstrucciones { get; set; }
        public DGCMejorasOtras DGCMejorasOtras { get; set; }
        public RelevamientoMejorasConstrucciones RelevamientoMejorasConstrucciones { get; set; }
        public RelevamientoMejorasOtras RelevamientoMejorasOtras { get; set; }
        public AtributosParcela AtributosParcela { get; set; }
        public RelevamientoParcela RelevamientoParcela { get; set; }
        public ParcelaSuperficies()
        {
            DGCMejorasConstrucciones = new DGCMejorasConstrucciones();
            DGCMejorasOtras = new DGCMejorasOtras();
            RelevamientoMejorasConstrucciones = new RelevamientoMejorasConstrucciones();
            RelevamientoMejorasOtras = new RelevamientoMejorasOtras();
            AtributosParcela = new AtributosParcela();
            RelevamientoParcela = new RelevamientoParcela();
        }
    }

    public class DGCMejorasConstrucciones
    {
        public decimal Cubierta { get; set; }
        public decimal Negocio { get; set; }
        public decimal Semicubierta { get; set; }

        public decimal Total { get { return Cubierta + Negocio + Semicubierta; } }
    }

    public class DGCMejorasOtras
    {
        public decimal Pavimento { get; set; }
        public decimal Piscina { get; set; }

        public decimal Total { get { return Pavimento + Piscina; } }
    }

    public class RelevamientoMejorasConstrucciones
    {
        public decimal Cubierta { get; set; }
        public decimal Galpon { get; set; }
        public decimal Semicubierta { get; set; }

        public decimal Total { get { return Cubierta + Galpon + Semicubierta; } }
    }

    public class RelevamientoMejorasOtras
    {
        public decimal Construccion { get; set; }
        public decimal Deportiva { get; set; }
        public decimal Piscina { get; set; }
        public decimal Precaria { get; set; }

        public decimal Total { get { return Construccion + Deportiva + Piscina + Precaria; } }
    }

    public class AtributosParcela 
    {
        public decimal Catastro { get; set; }
        public decimal Estimada { get; set; }
        public decimal Mensura { get; set; }
        public decimal Titulo { get; set; }

        public decimal Total { get { return Catastro + Estimada + Mensura + Titulo; } }
    }
    public class RelevamientoParcela
    {
        public decimal Relevada { get; set; }
    }
}
