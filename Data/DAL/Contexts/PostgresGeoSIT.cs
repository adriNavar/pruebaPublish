using GeoSit.Data.Mappers.CustomConventions;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.SQLQueryBuilders;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using System.Data.Common;
using System.Data.Entity;

namespace GeoSit.Data.DAL.Contexts
{
    public class PostgresGeoSIT : GeoSITMContext
    {
        internal PostgresGeoSIT() : base("POSTGRESGeoSIT") { }
        public override DbParameter CreateParameter(string name, object value) => new NpgsqlParameter(name, value);

        public override DbParameter CreateResultParameter(string name) => new NpgsqlParameter(name, NpgsqlDbType.Refcursor) { Direction = ParameterDirection.Output };

        public override ISQLQueryBuilder CreateSQLQueryBuilder() => new PostgresQueryBuilder(this);

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<LowerCaseNamingConvention>(); //esto es para no tener que reescribir todos los mappers teniendo en cuenta que postgres es case sensitive
            base.OnModelCreating(modelBuilder);
        }
    }
}
