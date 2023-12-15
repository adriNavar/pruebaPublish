namespace GeoSit.Data.BusinessEntities.ValidacionesDB.Enums
{
    public enum TipoMensaje : short
    {
        Ok = 0,
        Advertencia = 1,
        Bloqueo = 2,
        Error = 3
    }
    public enum ResultadoValidacion : short
    {
        Ok = 0,
        Advertencia = 1,
        Bloqueo = 2,
        Error = 3
    }

    public enum GrupoValidable : short
    {
        ConfirmarTramite = 1
    }
    public enum FuncionValidable : short
    {
        Ninguna = -1,
        Todas = 0,
        GuardarTramite = 1,
        ConfirmarTramite = 2,
        ProcesarTramite = 3,
        PresentarTramite = 4,
        FinalizarTramite = 5
    }

    public enum TipoObjetoValidable : short
    {
        Tramite = 1,
        ParcelaGrafica = 2,
        EdicionDivision = 3,
        EdicionJurisdiccion = 4,
        EdicionSeccion = 5,
        EdicionMunicipio = 6,
        EdicionBarrio = 7,
    }
}