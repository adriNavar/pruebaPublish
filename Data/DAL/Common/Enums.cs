namespace Geosit.Data.DAL.DDJJyValuaciones.Enums
{
    public enum TipoMedidaLinealEnum
    {
        FRENTE = 1,
        FONDO = 2,
        FRENTE1 = 3,
        FONDO1 = 4,
        FRENTE2 = 5,
        FONDO2 = 6,
        CONTRAFRENTE = 7,
        FONDOA1 = 8,
        FONDOA2 = 9,
        FONDOB1 = 10,
        FONDOB2 = 11,
        FRENTE3 = 12,
        COEFICIENTE = 13,
        FONDOSALIENTE = 14,
    }

    public enum ClasesEnum
    {
        PARCELA_RECTANGULAR_NO_EN_ESQUINA_HASTA_2000M2 = 1,
        PARCELA_INTERNA_CON_ACCESO_A_PASILLO_HASTA_2000M2 = 2,
        PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_HASTA_2000M2 = 3,
        PARCELA_CON_MARTILLO_AL_FRENTE_HASTA_2000M2 = 4,
        PARCELA_CON_MARTILLO_AL_FONDO_HASTA_2000M2 = 5,
        PARCELA_ROMBOIDAL_HASTA_2000M2 = 6,
        PARCELA_CON_FRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2 = 7,
        PARCELA_CON_CONTRAFRENTE_EN_FALSA_ESCUADRA_HASTA_2000M2 = 8,
        PARCELA_CON_FRENTE_A_CALLES_OPUESTAS_HASTA_2000M2 = 9,
        PARCELA_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2 = 10,
        PARCELA_EN_ESQUINA_CON_FRENTE_A_DOS_CALLES_HASTA_900M2 = 11,
        PARCELA_CON_FRENTE_A_DOS_CALLES_OPUESTAS_MAYOR_A_2000M2 = 12,
        PARCELA_NO_EN_ESQUINA_CON_SUPERFICIE_ENTRE_2000M2_Y_15000M2 = 13,
        PARCELA_EN_ESQUINA_DE_2000M2_Y_15000M2 = 14,
        PARCELA_CON_SUPERFICIE_MAYOR_A_15000M2 = 15,
        PARCELA_TRIANGULAR_CON_FRENTE_A_UNA_CALLE_HASTA_2000M2 = 16,
        PARCELA_TRIANGULAR_CON_VERTICE_A_UNA_CALLE_HASTA_2000M2 = 17,
        PARCELA_TRAPEZOIDAL_CON_FRENTE_MAYOR_A_UNA_CALLE_HASTA_2000M2 = 18,
        PARCELA_TRAPEZOIDAL_CON_FRENTE_MENOR_A_UNA_CALLE_HASTA_2000M2 = 19,
        PARCELA_TRIANGULAR_CON_FRENTE_A_TRES_CALLES_HASTA_2000M2 = 20,
        PARCELA_EN_ESQUINA_CON_SUP_ENTRE_900M2_Y_2000M2 = 21,
        PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_Y_SUP_ENTRE_900M2_Y_2000M2 = 22,
        PARCELA_ESPECIAL = 23,
        PARCELA_EN_ESQUINA_CON_FRENTE_A_TRES_CALLES_HASTA_900M2 = 24,
        PARCELA_CON_FRENTE_A_DOS_CALLES_NO_OPUESTAS_MAYOR_A_2000M2 = 25,
        PARCELA_CON_SALIENTE_LATERAL_HASTA_2000M2 = 26,
        PARCELA_EN_TODA_LA_MANZANA_Y_SUP_ENTRE_2000M2_Y_15000M2 = 27,
        PARCELA_CON_FRENTE_A_TRES_CALLES_MAYOR_A_2000M2 = 28
    }

    public enum OrigenEnum
    {
        NoIdentificada = 0,
        Migracion = 1,
        Presentada = 2
    }

    public enum TipoDesignadorEnum
    {
        Catastro = 0,
        Titulo = 1
    }

    public enum TipoParcelaEnum
    {
        A_Determinar = 0,
        Urbana = 1,
        Suburbana = 2,
        Rural = 3
    }

    public enum TipoValuacionEnum
    {
        Urbana = 0,
        Sor = 1,
        Mejoras = 2,
        Revaluacion = 3
    }

    public enum TipoRevaluacionEnum
    {
        Urbana = 0,
        Sor = 1,
        Indeterminado = -1
    }

    public enum TipoUnidadTributariaEnum
    {
        Comun = 1,
        PropiedaHorizontal = 2,
        UnidadFuncionalPH = 3
    }

    public enum OtrasCaracteristicasV1
    {
        AguaCaliente = 26,
        AnioConstruccion = 35,
        Ascensores = 27,
        Banio2Ambientes = 22,
        BanioServicio = 23,
        InstalacionIncendio = 25,
        PiletasNatacion = 24,
        SuperficieCubierta = 32,
        SuperficieNegocio = 34,
        SuperficieSemiCubierta = 33,
    }
    public enum OtrasCaracteristicasV2
    {
        AnioConstruccion = 39,
        CamarasFrigorificas = 29,
        InstalacionIncendio = 31,
        Pavimento = 28,
        SuperficieCubierta = 36,
        SuperficieNegocio = 38,
        SuperficieSemiCubierta = 37,
        TanquesLiquidosGases = 30
    }
}

namespace GeoSit.Data.DAL.Common.Enums
{
    public enum SQLAggregatedFunction
    {
        Sum = 1,
        Avg = 2,
        Count = 3,
        Min = 4,
        Max = 5
    }
    public enum SQLSort
    {
        Asc,
        Desc,
        No
    }
    public enum SQLConnectors
    {
        And,
        Or,
        None
    }
    public enum SQLOperators
    {
        Contains = 1 << 0,
        EndsWith = 1 << 1,
        EqualsTo = 1 << 2,
        Exists = 1 << 3,
        GreaterThan = 1 << 4,
        In = 1 << 5,
        IsNotNull = 1 << 6,
        IsNull = 1 << 7,
        LowerThan = 1 << 8,
        None = 1 << 9,
        NotContains = 1 << 10,
        NotEndsWith = 1 << 11,
        NotEqualsTo = 1 << 12,
        NotIn = 1 << 13,
        NotStartsWith = 1 << 14,
        StartsWith = 1 << 15
    }
    public enum SQLJoin
    {
        Inner,
        Left,
        Rigth
    }
    public enum SQLSpatialRelationships
    {
        AnyInteract = 1 << 0,
        Contains = 1 << 1,
        CoveredBy = 1 << 2,
        Covers = 1 << 3,
        Crosses = 1 << 4,
        Equal = 1 << 5,
        Inside = 1 << 6,
        Overlaps = 1 << 7,
        Touch = 1 << 8,
        Disjoint = 1 << 9
    }
    public enum SRID
    {
        DB,
        App,
        LL84
    }
}
