using System;
using System.Collections.Generic;
using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Interfaces;
using System.Data;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using System.Configuration;
using GeoSit.Data.DAL.Common.Enums;

namespace GeoSit.Data.DAL.Repositories
{
    class EstadoDeudaRepository : IEstadoDeudaRepository
    {
        public IEnumerable<int> GetYears()
        {
            return new List<int> { 2014, 2015 };
        }

        public IEnumerable<EstadoDeudaServicioGeneral> GetEstadoDeudaServiciosGenerales(string padron)
        {
            using (var db = GeoSITMContext.CreateContext())
            using (var builder = db.CreateSQLQueryBuilder())
            {
                return builder.AddTable("v_inm_deuda", "t1")
                              .AddFilter(new Atributo { Campo = "unidad_tributaria", TipoDatoId = 6 }, padron, SQLOperators.EqualsTo)
                              .AddFields("unidad_tributaria", "tributo", "periodo", "vencimiento", "monto", "recargo_calculado", "total")
                              .ExecuteQuery((IDataReader reader) =>
                              {
                                  return new EstadoDeudaServicioGeneral()
                                  {
                                      Padron = reader.GetStringOrEmpty(0),
                                      Servicio = reader.GetStringOrEmpty(1),
                                      Periodo = reader.GetStringOrEmpty(2),
                                      FechaVencimiento = reader.GetNullableDateTime(3).GetValueOrDefault(),
                                      Monto = reader.GetNullableDecimal(4).GetValueOrDefault(),
                                      RecargoCalculado = reader.GetNullableDecimal(5).GetValueOrDefault(),
                                      Total = reader.GetNullableDecimal(6).GetValueOrDefault()
                                  };
                              });
            }
        }

        public IEnumerable<EstadoDeudaRenta> GetEstadoDeudaRentas(int year)
        {
            var rentas = new List<EstadoDeudaRenta>
            {
                new EstadoDeudaRenta
                {
                    Periodo = "04",
                    FechaVencimiento = new DateTime(2015, 1, 8),
                    Monto = 211.21M
                },
                new EstadoDeudaRenta
                {
                    Periodo = "05",
                    FechaVencimiento = new DateTime(2015, 2, 20),
                    Monto = 500.99M
                },
                new EstadoDeudaRenta
                {
                    Periodo = "10",
                    FechaVencimiento = new DateTime(2014, 3, 19),
                    Monto = 108.08M
                },
                new EstadoDeudaRenta
                {
                    Periodo = "12",
                    FechaVencimiento = new DateTime(2014, 5, 1),
                    Monto = 254.71M
                }
            };

            return rentas.Where(x => x.FechaVencimiento.Year == year).ToList();
        }
    }
}
