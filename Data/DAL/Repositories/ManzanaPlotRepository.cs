using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using System.Linq;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common;

namespace GeoSit.Data.DAL.Repositories
{
    public class ManzanaPlotRepository : IManzanaPlotRepository
    {
        private readonly GeoSITMContext _context;

        public ManzanaPlotRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public ManzanaPlot[] GetManzanaPlotByIdManzana(string esquema, string tabla, string campoGeometry, string campoIdManzana, long idManzana)
        {
            try
            {
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoIdManzana == string.Empty)
                {
                    campoIdManzana = "ID_MANZANA";
                }

                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFilter(campoIdManzana, idManzana, SQLOperators.EqualsTo)
                                  .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1").Area(), "superficie")
                                  .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1").ToWKT(), "geom")
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      return new ManzanaPlot()
                                      {
                                          FeatId = idManzana,
                                          Superficie = reader.GetNullableDouble(reader.GetOrdinal("superficie")).GetValueOrDefault(),
                                          Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                      };
                                  }).ToArray();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetManzanaPlotByIdManzana", ex);
                return null;
            }
        }

        public ManzanaPlot[] GetManzanaPlotByCoords(string esquema, string tabla, string campoGeometry, string campoIdManzana, double x, double y)
        {
            try
            {
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoIdManzana == string.Empty)
                {
                    campoIdManzana = "ID_MANZANA";
                }

                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFilter(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1"),
                                             builder.CreateGeometryFieldBuilder(string.Format("POINT({0} {1})", x, y), SRID.App).ChangeToSRID(SRID.DB),
                                             SQLSpatialRelationships.AnyInteract)
                                  .AddFields(campoIdManzana)
                                  .ExecuteQuery((IDataReader reader) => new ManzanaPlot() { FeatId = reader.GetNullableInt64(0).Value })
                                  .ToArray();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetManzanaPlotByIdManzana", ex);
                return null;
            }
        }

        public bool GetExisteEspacioVerde(string esquema, string tabla, string campoGeometry, string campoIdManzana, long idManzana, int idOCTipo)
        {
            try
            {
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoIdManzana == string.Empty)
                {
                    campoIdManzana = "ID_MANZANA";
                }

                using (var ctx = GeoSITMContext.CreateContext())
                using (var builder = ctx.CreateSQLQueryBuilder())
                {
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFilter(campoIdManzana, idManzana, SQLOperators.EqualsTo)
                                  .AddFormattedField("count({0})", new Atributo { Campo = campoIdManzana })
                                  .AddTable(esquema, "ct_oc_objeto", "t2")
                                  .AddFilter("id_oc_tipo", idOCTipo, SQLOperators.EqualsTo, SQLConnectors.And)
                                  .AddFilter(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1"),
                                             builder.CreateGeometryFieldBuilder(new Atributo { Campo = "geometry" }, "t2"),
                                             SQLSpatialRelationships.AnyInteract, SQLConnectors.And)
                                  .ExecuteQuery((IDataReader reader, ReaderStatus status) =>
                                  {
                                      status.Break();
                                      return reader.GetNullableInt32(0).Value;
                                  }).Single() != 0;
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetExisteEspacioVerde", ex);
                return false;
            }
        }
    }
}
