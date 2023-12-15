using GeoSit.Data.Intercambio.DAL.Conventions;
using System.Data.Entity;

namespace GeoSit.Data.Intercambio.DAL.Contexts
{
    public class PostgresGeoSIT : IntercambioContext
    {
        internal PostgresGeoSIT() : base("POSTGRESGeoSIT") { }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //string muni = (string)Session["cod_municipio"];
            modelBuilder.Conventions.Add<LowerCaseNamingConvention>(); //esto es para no tener que reescribir todos los mappers teniendo en cuenta que postgres es case sensitive
            base.OnModelCreating(modelBuilder);
        }
    }
}
