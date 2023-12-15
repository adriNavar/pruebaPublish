using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using System.Text;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Data.DAL.Repositories
{
    public class CuadraPlotRepository : ICuadraPlotRepository
    {
        private readonly GeoSITMContext _context;

        public CuadraPlotRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public CuadraPlot[] GetCuadraPlotByIdCuadra(string esquema, string tabla, string campoGeometry, string campoIdCuadra, long idCuadra)
        {
            try
            {
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoIdCuadra == string.Empty)
                {
                    campoIdCuadra = "ID_CUADRA";
                }

                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFilter(campoIdCuadra, idCuadra, SQLOperators.EqualsTo)
                                  .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1")
                                                           .ChangeToSRID(SRID.App).ToWKT(), "geom")
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      return new CuadraPlot()
                                      {
                                          FeatId = idCuadra,
                                          IdCuadra = idCuadra,
                                          Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                      };
                                  }).ToArray();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetCuadraPlotByIdCuadra", ex);
                return null;
            }
        }

        public CuadraPlot[] GetCuadraPlotByIdManzana(string esquema, string tabla, string campoGeometry, string campoIdCuadra, string campoIdManzana, string campoIdCalle, string campoAlturaMin, string campoAlturaMax, long idManzana)
        {
            try
            {
                if (campoGeometry == string.Empty)
                {
                    campoGeometry = "GEOMETRY";
                }
                if (campoIdCuadra == string.Empty)
                {
                    campoIdCuadra = "ID_CUADRA";
                }
                if (campoIdManzana == string.Empty)
                {
                    campoIdManzana = "ID_MANZANA";
                }
                if (campoIdCalle == string.Empty)
                {
                    campoIdCalle = "ID_CALLE";
                }
                if (campoAlturaMin == string.Empty)
                {
                    campoAlturaMin = "ALTURA_MIN";
                }
                if (campoAlturaMax == string.Empty)
                {
                    campoAlturaMax = "ALTURA_MAX";
                }

                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    return builder.AddTable(esquema, tabla, "t1")
                                  .AddFilter(campoIdManzana, idManzana, SQLOperators.EqualsTo)
                                  .AddFields(campoIdCuadra, campoIdCalle, campoAlturaMin, campoAlturaMax)
                                  .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo { Campo = campoGeometry }, "t1")
                                                           .ChangeToSRID(SRID.App).ToWKT(), "geom")
                                  .ExecuteQuery((IDataReader reader) =>
                                  {
                                      return new CuadraPlot()
                                      {
                                          FeatId = reader.GetNullableInt64(0).Value,
                                          IdCuadra = reader.GetNullableInt64(0).Value,
                                          IdCalle = reader.GetNullableInt64(1).GetValueOrDefault(),
                                          AlturaMin = reader.GetNullableInt32(2).GetValueOrDefault(),
                                          AlturaMax = reader.GetNullableInt32(3).GetValueOrDefault(),
                                          Geom = reader.GetGeometryFromField(reader.GetOrdinal("geom"))
                                      };
                                  }).ToArray();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetCuadraPlotByIdManzana", ex);
                return null;
            }
        }

        public CuadraPlot[] GetCuadraPlotByObjetoBase(Componente componenteBase, string idObjetoBase, string esquema, string tabla, string campoGeometry, string campoId, string campoIdManzana, string campoIdCalle, string campoAlturaMin, string campoAlturaMax, string campoIdParidad, int filtroGeografico)
        {
            throw new NotImplementedException();
        }
    }
}
