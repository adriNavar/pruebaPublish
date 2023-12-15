using GeoSit.Data.DAL.Common.Enums;
using System;
using System.Configuration;

namespace GeoSit.Data.DAL.Common
{
    public class SRIDParser
    {
        private readonly int appSRID;
        private readonly int dbSRID;
        private readonly int ll84SRID = 4326;
        public SRIDParser()
        {
            appSRID = int.Parse(ConfigurationManager.AppSettings["AppSRID"]);
            if(!int.TryParse(ConfigurationManager.AppSettings["DBSRID"], out dbSRID))
            {
                dbSRID = ll84SRID;
            }
        }

        public int Parse(SRID srid)
        {
            switch(srid)
            {
                case SRID.App:
                    return appSRID;
                case SRID.DB:
                    return dbSRID;
                case SRID.LL84:
                    return ll84SRID;
                default:
                    return 0;
            }
        }

        public bool IsDbLatLon()
        {
            return dbSRID == ll84SRID;
        }
    }
}
