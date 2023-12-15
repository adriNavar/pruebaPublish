using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.SQLQueryBuilders;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Data.Common;

namespace GeoSit.Data.DAL.Contexts
{
    public class ORAGeoSIT : GeoSITMContext
    {
        public ORAGeoSIT() : base("ORAGeoSIT")  { }

        public override DbParameter CreateParameter(string name, object value) => new OracleParameter(name, value);

        public override DbParameter CreateResultParameter(string name) => new OracleParameter(name, OracleDbType.RefCursor, ParameterDirection.Output);

        public override ISQLQueryBuilder CreateSQLQueryBuilder() => new OracleQueryBuilder(this);
    }
}
