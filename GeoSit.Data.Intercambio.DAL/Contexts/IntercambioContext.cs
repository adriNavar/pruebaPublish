using GeoSit.Core.Logging;
using GeoSit.Core.Logging.Loggers;
using GeoSit.Data.Intercambio.BusinessEntities;
using GeoSit.Data.Intercambio.DAL.Mappers;
using System;
using System.Configuration;
using System.Data.Entity;

//using GeoSit.Data.DAL.Interfaces;

namespace GeoSit.Data.Intercambio.DAL.Contexts
{
    public class IntercambioContext : DbContext
    {
        private readonly LoggerManager _loggerManager = null;
        public static IntercambioContext CreateContext()
        {

            IntercambioContext ctx = null;
            switch (ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["USE_CONNECTION"]].ProviderName.ToLower())
            {
                case "npgsql":
                    ctx = new PostgresGeoSIT();
                    break;
                case "oracle.manageddataaccess.client":
                    ctx = new ORAGeoSIT();
                    break;
            }

            ctx.Database.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["TimeOutDB"]);
            return ctx;
         }
        public IntercambioContext(string connectionString) : base(connectionString)
        {
            _loggerManager = new LoggerManager();
            _loggerManager.Add(new Log4NET(ConfigurationManager.AppSettings["log4net.config"].ToString(), "DefaultLogger", "ErrorLogger"));

            Configuration.ProxyCreationEnabled = false;
            Database.Log = _loggerManager.LogInfo;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //string muni = (string)Session["cod_municipio"];

            modelBuilder.HasDefaultSchema(ConfigurationManager.AppSettings["DATABASE"]);
            modelBuilder.Configurations.Add(new ReporteParcelarioMapper());
            modelBuilder.Configurations.Add(new ReporteParselarioMunicipalMapper());
            modelBuilder.Configurations.Add(new MunicipioMapper());
            modelBuilder.Configurations.Add(new EstadoMapper());
            modelBuilder.Configurations.Add(new DiferenciaMapper());
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ReporteParcelario> ReporteParcelarios { get; set; }
        public DbSet<ReporteParcelarioMunicipio> ReporteParcelarioMunicipal { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Diferencia> Diferencias { get; set; }
    }
}
