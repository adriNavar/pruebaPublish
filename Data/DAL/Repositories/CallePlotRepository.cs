using System.Collections.Generic;
using System.Data;
using GeoSit.Data.BusinessEntities.ModuloPloteo;
using System;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Data.DAL.Repositories
{
    public class CallePlotRepository : ICallePlotRepository
    {
        private readonly GeoSITMContext _context;

        public CallePlotRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public CallePlot[] GetCallePlotByIdCalle(string esquema, string tabla, string campoIdCalle, string campoNombre, string campoCodigo, long idCalle)
        {
            try
            {
                if (string.IsNullOrEmpty(campoIdCalle))
                {
                    campoIdCalle = "ID_Calle";
                }
                if (string.IsNullOrEmpty(campoNombre))
                {
                    campoNombre = "NOMBRE";
                }
                if (string.IsNullOrEmpty(campoCodigo))
                {
                    campoCodigo = "APID_ID";
                }

                return this._context.CreateSQLQueryBuilder()
                                    .AddTable(esquema, tabla, "t1")
                                    .AddFilter(campoIdCalle, idCalle, SQLOperators.EqualsTo)
                                    .AddFields(campoNombre, campoCodigo)
                                    .ExecuteQuery((IDataReader reader) =>
                                    {
                                        return new CallePlot()
                                        {
                                            IdCalle = idCalle,
                                            Nombre = reader.GetStringOrEmpty(1),
                                            Codigo = reader.GetStringOrEmpty(2)
                                        };
                                    }).ToArray();
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetCallePlotByIdCalle", ex);
                return null;
            }
        }
    }
}
