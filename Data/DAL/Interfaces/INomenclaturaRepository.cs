using GeoSit.Data.BusinessEntities.Inmuebles;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface INomenclaturaRepository
    {
        Nomenclatura GetNomenclatura(string nomenclatura);
        Nomenclatura GetNomenclaturaById(long id);
        void InsertNomenclatura(Nomenclatura nomenclatura);
        void UpdateNomenclatura(Nomenclatura nomenclatura);
        void DeleteNomenclatura(Nomenclatura nomenclatura);
        Nomenclatura GetNomenclatura(long idParcela, long idTipoNomenclatura);
        string Generar(long idParcela, long tipo);
    }
}
