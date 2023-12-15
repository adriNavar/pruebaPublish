namespace GeoSit.Data.DAL.Tramites.Interfaces
{
    interface IPartidaGenerator
    {
        bool IsValid();
        bool IsDefault();
        string Generate();
    }
}
