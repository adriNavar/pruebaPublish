using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GeoSit.Data.BusinessEntities.MapasTematicos;
//using GeoSit.Data.BusinessEntities.Reportes;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Common.ExtensionMethods.Data;
using System.Data;
using GeoSit.Data.Intercambio.BusinessEntities;
//using GeoSit.Data.Intercambio.DAL.Contexts;


namespace GeoSit.Data.DAL.Repositories
{
    public class ReporteParcelarioRepository : IReporteParcelario
    {
        private readonly GeoSITMContext _context;

        public ReporteParcelarioRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public List<ReporteParcelario> GetReporteParcelario(string id_municipio)
        {
            try
            {
                using (var builder = this._context.CreateSQLQueryBuilder())
                {
                    var componenteReporteParcelario = new Componente()
                    {
                        ComponenteId = long.MaxValue,
                        Tabla = "VW_REPORTE_PARCELARIO_PROVINCIAL",
                        Esquema = ConfigurationManager.AppSettings["DATABASEEXCEL"]
                    };
                    /*  TipoDatoId
                            1	Boolean
                            3	Long
                            2	Integer
                            4	Double
                            5	Date
                            6	String
                            7	Float
                            8	Geometry
                            9	XML
                            10	XSD
                            11	check*/
                    var atributosRepParcelario = new[]
                    {
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="id_parcela", TipoDatoId = 3},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="municipio", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="partida", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="nomenclatura", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="tipo", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="ubicacion", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="coordenadas", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="ph", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="dominio", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="fecha_valuacion", TipoDatoId = 5},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="valor_tierra", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="valor_mejoras", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="valor_total", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_tierra_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_tierra_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="unidad_medida", TipoDatoId = 6},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_mejora_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_mejora_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_cubierta_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_semicub_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_negocio_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_piscina_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_pavimiento_regis", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_cubierta_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_semicub_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_galpon_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_piscina_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_deportiva_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_en_const_relev", TipoDatoId = 4},
                        new Atributo() { ComponenteId=componenteReporteParcelario.ComponenteId, Campo="sup_precaria_relev", TipoDatoId = 4}
                    };

                    var atributomunicipio = new Atributo() { ComponenteId = componenteReporteParcelario.ComponenteId, Campo = "municipio", TipoDatoId = 6 };

                    return builder.AddTable(componenteReporteParcelario, "rp")
                                               .AddFields(atributosRepParcelario)
                                               .AddFilter(atributomunicipio, id_municipio, SQLOperators.EqualsTo)
                                               .ExecuteQuery((IDataReader reader) =>
                                               {
                                                   return new ReporteParcelario()
                                                   {
                                                       IdParcela = reader.GetInt64(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "id_parcela", true) == 0).Campo)),
                                                       Municipio = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "municipio", true) == 0).Campo)),
                                                       Partida = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "partida", true) == 0).Campo)),
                                                       Nomenclatura = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "nomenclatura", true) == 0).Campo)),
                                                       Tipo = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "tipo", true) == 0).Campo)),
                                                       Ubicacion = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "ubicacion", true) == 0).Campo)),
                                                       Coordenadas = reader.GetStringOrEmpty(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "coordenadas", true) == 0).Campo)),
                                                       Ph = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "ph", true) == 0).Campo)),
                                                       Dominio = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "dominio", true) == 0).Campo)),
                                                       FechaValuacion = reader.GetDateTime(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "fecha_valuacion", true) == 0).Campo)),
                                                       ValorTierra = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "valor_tierra", true) == 0).Campo)),
                                                       ValorMejoras = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "valor_mejoras", true) == 0).Campo)),
                                                       ValorTotal = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "valor_total", true) == 0).Campo)),
                                                       SuperficieTierraRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_tierra_regis", true) == 0).Campo)),
                                                       SuperficieTierraRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_tierra_relev", true) == 0).Campo)),
                                                       UnidadMedida = reader.GetString(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "unidad_medida", true) == 0).Campo)),
                                                       SuperficieMejoraRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_mejora_regis", true) == 0).Campo)),
                                                       SuperficieMejoraRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_mejora_relev", true) == 0).Campo)),
                                                       SuperficieCubiertaRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_cubierta_regis", true) == 0).Campo)),
                                                       SuperficieSemicubiertaRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_semicub_regis", true) == 0).Campo)),
                                                       SuperficieNegocioRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_negocio_regis", true) == 0).Campo)),
                                                       SuperficiePiscinaRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_piscina_regis", true) == 0).Campo)),
                                                       SuperficiePavimentoRegistrada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_pavimiento_regis", true) == 0).Campo)),
                                                       SuperficieCubiertaRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_cubierta_relev", true) == 0).Campo)),
                                                       SuperficieSemicubiertaRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_semicub_relev", true) == 0).Campo)),
                                                       SuperficieGalponRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_galpon_relev", true) == 0).Campo)),
                                                       SuperficiePiscinaRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_piscina_relev", true) == 0).Campo)),
                                                       SuperficieDeportivaRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_deportiva_relev", true) == 0).Campo)),
                                                       SuperficieEnConstruccionRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_en_const_relev", true) == 0).Campo)),
                                                       SuperficiePrecariaRelevada = reader.GetDecimal(reader.GetOrdinal(atributosRepParcelario.Single(a => string.Compare(a.Campo, "sup_precaria_relev", true) == 0).Campo))
                                                   }; 
                                               }).ToList();
                }
            }
            catch (Exception ex)
            {
                _context.GetLogger().LogError("GetReporteParcelario", ex);
                return null;
            }
        }
    }
}
