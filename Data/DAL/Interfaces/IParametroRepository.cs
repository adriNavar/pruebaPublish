using GeoSit.Data.BusinessEntities.Seguridad;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IParametroRepository
    {
        ParametrosGenerales GetParametro(long idParametro);
        ParametrosGenerales GetParametro(string clave);
        string GetValor(long idParametro);
        string GetValor(string clave);
        string GetParametroByDescripcion(string descripcion);
    }
}
