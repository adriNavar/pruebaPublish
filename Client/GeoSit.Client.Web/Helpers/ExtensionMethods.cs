using GeoSit.Client.Web.Models;
using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using INM = GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoSit.Client.Web.Helpers.ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static string ToStringOrDefault(this object obj)
        {
            return obj == null ? "N/D" : obj.ToString();
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static List<TSource> MoveElementToFirstPosition<TSource>(this List<TSource> source, TSource element)
        {
            if (element != null)
            {
                source.RemoveAt(source.IndexOf(element));
                source.Insert(0, element);
            }
            return source;
        }

        public static IEnumerable<INM.TipoInscripcion> FilterTiposIncripcion(this IEnumerable<INM.TipoInscripcion> tipos, bool esParcelaPrescripcion)
        {
            string DESCRIPCION_POSESION = "POSESION";
            if (esParcelaPrescripcion)
            {
                tipos = tipos.Where(x => x.Descripcion.ToUpper() == DESCRIPCION_POSESION);
            }
            else
            {
                tipos = tipos.Where(x => x.Descripcion.ToUpper() != DESCRIPCION_POSESION).OrderBy(x => x.Descripcion);
            }
            return tipos;
        }

        public static IEnumerable<INM.TipoTitularidad> FilterTiposTitularidad(this IEnumerable<INM.TipoTitularidad> tipos, bool esParcelaPrescripcion)
        {
            string DESCRIPCION_POSEEDOR = "POSEEDOR";
            if (esParcelaPrescripcion)
            {
                tipos = tipos.Where(x => x.Descripcion.ToUpper() == DESCRIPCION_POSEEDOR);
            }
            else
            {
                tipos = tipos.Where(x => x.Descripcion.ToUpper() != DESCRIPCION_POSEEDOR).OrderBy(x => x.Descripcion);
            }
            return tipos;
        }

        public static bool IsPrescripcion(this INM.Parcela parcela)
        {
            long ID_CLASE_PRESCRIPCION = 2;
            return parcela.ClaseParcelaID == ID_CLASE_PRESCRIPCION;
        }
    }

    namespace DeclaracionesJuradasTemporal
    {
        internal static class DDJJXTMethods
        {
            internal static FormularioE1Model ToFormularioE1(this DDJJTemporal ddjjTemp, DDJJVersion version)
            {
                return new FormularioE1Model()
                {
                    DDJJ = ddjjTemp.ToDDJJ(version),
                    Mejora = ddjjTemp.Mejora.ToMejora(),
                    CaracteristicasToSave = ddjjTemp.Mejora.ToCaracteristicasToSave(),
                    OtrasCar = ddjjTemp.Mejora.ToOtrasCar()
                };
            }
            internal static FormularioUModel ToFormularioU(this DDJJTemporal ddjjTemp, DDJJVersion version, IEnumerable<Persona> personas, IEnumerable<Aforo> aforos)
            {
                return new FormularioUModel()
                {
                    DDJJ = ddjjTemp.ToDDJJ(version),
                    DDJJU = ddjjTemp.U.ToDDJJU(),
                    DDJJDesignacion = ddjjTemp.Designacion.ToDDJJDesignacion(),
                    ClasesJsonSerialized = JsonConvert.SerializeObject(ddjjTemp.U.Fracciones.ToDDJJUClasesSeleccionadas(aforos)),
                    dominiosJSON = JsonConvert.SerializeObject(ddjjTemp.Dominios.ToDDJJDominios(personas)),
                    CroquisBase64 = ddjjTemp.U.Croquis == null ? null : $"data:image/png;base64,{Convert.ToBase64String(ddjjTemp.U.Croquis)}",
                };
            }
            internal static FormularioSoRModel ToFormularioSoR(this DDJJTemporal ddjjTemp, DDJJVersion version, IEnumerable<Persona> personas, IEnumerable<VALAptitudes> aptitudes)
            {
                return new FormularioSoRModel()
                {
                    DDJJ = ddjjTemp.ToDDJJ(version),
                    DDJJSor = ddjjTemp.Sor.ToDDJJSoR(),
                    DDJJDesignacion = ddjjTemp.Designacion.ToDDJJDesignacion(),
                    dominiosJSON = JsonConvert.SerializeObject(ddjjTemp.Dominios.ToDDJJDominios(personas)),
                    AptitudesDisponibles = ddjjTemp.Sor.ToDDJJAptitudesInput(aptitudes)
                };
            }
            private static DDJJ ToDDJJ(this DDJJTemporal tempObj, DDJJVersion version)
            {
                return new DDJJ
                {
                    IdDeclaracionJurada = tempObj.IdDeclaracionJurada,
                    IdVersion = tempObj.IdVersion,
                    IdOrigen = tempObj.IdOrigen,
                    IdUnidadTributaria = tempObj.IdUnidadTributaria,
                    IdPoligono = tempObj.IdPoligono,
                    FechaVigencia = tempObj.FechaVigencia,

                    Version = version,
                };
            }
            private static DDJJU ToDDJJU(this DDJJUTemporal tempObj)
            {
                return new DDJJU
                {
                    AguaCorriente = tempObj.AguaCorriente,
                    Cloaca = tempObj.Cloaca,
                    IdMensura = tempObj.IdMensura,
                    IdU = tempObj.IdU,
                    NumeroHabitantes = tempObj.NumeroHabitantes,
                    SuperficiePlano = tempObj.SuperficiePlano,
                    SuperficieTitulo = tempObj.SuperficieTitulo,
                    IdUsuarioAlta = tempObj.UsuarioAlta,
                    FechaAlta = tempObj.FechaAlta,
                    IdUsuarioModif = tempObj.UsuarioModif,
                    FechaModif = tempObj.FechaModif
                };
            }
            private static DDJJDesignacion ToDDJJDesignacion(this DDJJDesignacionTemporal tempObj)
            {
                return new DDJJDesignacion
                {
                    IdDeclaracionJurada = tempObj.IdDeclaracionJurada,
                    IdTipoDesignador = tempObj.IdTipoDesignador,

                    Barrio = tempObj.Barrio,
                    Calle = tempObj.Calle,
                    Chacra = tempObj.Chacra,
                    CodigoPostal = tempObj.CodigoPostal,
                    Departamento = tempObj.Departamento,
                    Fraccion = tempObj.Fraccion,
                    Localidad = tempObj.Localidad,
                    Lote = tempObj.Lote,
                    Manzana = tempObj.Manzana,
                    Numero = tempObj.Numero,
                    Paraje = tempObj.Paraje,
                    Quinta = tempObj.Quinta,
                    Seccion = tempObj.Seccion,
                    IdBarrio = tempObj.IdBarrio,
                    IdCalle = tempObj.IdCalle,
                    IdDepartamento = tempObj.IdDepartamento,
                    IdLocalidad = tempObj.IdLocalidad,
                    IdManzana = tempObj.IdManzana,
                    IdParaje = tempObj.IdParaje,
                    IdSeccion = tempObj.IdSeccion,

                    IdUsuarioAlta = tempObj.UsuarioAlta,
                    FechaAlta = tempObj.FechaAlta,
                    IdUsuarioModif = tempObj.UsuarioModif,
                    FechaModif = tempObj.FechaModif
                };
            }
            private static List<ClaseParcela> ToDDJJUClasesSeleccionadas(this IEnumerable<DDJJUFraccionTemporal> fracciones, IEnumerable<Aforo> aforos)
            {
                return fracciones.Select(f =>
                {
                    var cp = f.MedidasLineales.First().ClaseParcelaMedidaLineal.ClaseParcela;
                    return new ClaseParcela()
                    {
                        IdClaseParcela = cp.IdClaseParcela,
                        Descripcion = cp.Descripcion,
                        TiposMedidasLineales = f.ToTiposMedidasLineales(aforos)
                    };
                }).ToList();
            }
            private static List<TipoMedidaLinealConParcela> ToTiposMedidasLineales(this DDJJUFraccionTemporal fraccionTemp, IEnumerable<Aforo> aforos)
            {
                return fraccionTemp.MedidasLineales
                                   .Select(ml =>
                                   {
                                       bool requiereAforo = ml.ClaseParcelaMedidaLineal.RequiereAforo.GetValueOrDefault() != 0;
                                       Aforo aforo = null;
                                       if (requiereAforo)
                                       {
                                           aforo = ml.BuscarAforo(aforos);
                                       }
                                       return new TipoMedidaLinealConParcela
                                       {
                                           Altura = ml.AlturaCalle?.ToString(),
                                           Calle = aforo?.Calle ?? ml.Calle,
                                           Desde = aforo?.Desde ?? string.Empty,
                                           Hasta = aforo?.Hasta ?? string.Empty,
                                           Descripcion = ml.ClaseParcelaMedidaLineal.TipoMedidaLineal.Descripcion,
                                           IdClasesParcelasMedidaLineal = ml.IdClaseParcelaMedidaLineal,
                                           IdTipoMedidaLineal = ml.ClaseParcelaMedidaLineal.IdTipoMedidaLineal,
                                           IdTramoVia = ml.IdTramoVia,
                                           IdVia = ml.IdVia,
                                           Orden = ml.ClaseParcelaMedidaLineal.Orden,
                                           Paridad = aforo?.Paridad ?? string.Empty,
                                           RequiereAforo = requiereAforo,
                                           RequiereLongitud = ml.ClaseParcelaMedidaLineal.RequiereLongitud.GetValueOrDefault() != 0,
                                           ValorAforo = ml.ValorAforo,
                                           ValorMetros = ml.ValorMetros,
                                       };
                                   })
                                   .ToList();
            }
            private static List<DDJJDominio> ToDDJJDominios(this IEnumerable<DDJJDominioTemporal> dominiosTemp, IEnumerable<Persona> personas)
            {
                return dominiosTemp.Select(dt =>
                {
                    return new DDJJDominio()
                    {
                        Fecha = dt.Fecha,
                        IdDeclaracionJurada = dt.IdDeclaracionJurada,
                        IdDominio = dt.IdDominio,
                        IdTipoInscripcion = dt.IdTipoInscripcion,
                        TipoInscripcion = dt.TipoInscripcion.Descripcion,
                        Inscripcion = dt.Inscripcion,
                        Titulares = dt.Titulares.ToDDJJDominiosTitulares(personas),
                    };
                }).ToList();
            }
            private static List<DDJJDominioTitular> ToDDJJDominiosTitulares(this IEnumerable<DDJJDominioTitularTemporal> titularesTemp, IEnumerable<Persona> personas)
            {
                return titularesTemp
                        .Select(tt =>
                        {
                            var persona = tt.BuscarPersona(personas);
                            return new DDJJDominioTitular()
                            {
                                IdDominioTitular = tt.IdDominioTitular,
                                IdDominio = tt.IdDominio,
                                IdPersona = tt.IdPersona,
                                IdTipoTitularidad = tt.IdTipoTitularidad,
                                NombreCompleto = persona.NombreCompleto,
                                PersonaDomicilio = tt.PersonaDomicilios.ToDDJJPersonaDomicilios(),
                                PorcientoCopropiedad = tt.PorcientoCopropiedad,
                                TipoNoDocumento = $"{persona.TipoDocumentoIdentidad.Descripcion} / {persona.NroDocumento}",
                                TipoTitularidad = tt.TipoTitularidad.Descripcion,
                            };
                        })
                        .ToList();
            }
            private static ICollection<DDJJPersonaDomicilio> ToDDJJPersonaDomicilios(this IEnumerable<DDJJPersonaDomicilioTemporal> personaDomicilios)
            {
                return personaDomicilios.Select(pd => new DDJJPersonaDomicilio()
                {
                    Altura = pd.Domicilio.numero_puerta,
                    Barrio = pd.Domicilio.barrio,
                    Calle = pd.Domicilio.ViaNombre,
                    CodigoPostal = pd.Domicilio.codigo_postal,
                    Departamento = pd.Domicilio.unidad,
                    IdCalle = pd.Domicilio.ViaId,
                    IdDomicilio = pd.IdDomicilio,
                    IdDominioTitular = pd.IdDominioTitular,
                    IdTipoDomicilio = pd.IdTipoDomicilio,
                    Localidad = pd.Domicilio.localidad,
                    Municipio = pd.Domicilio.municipio,
                    Pais = pd.Domicilio.pais,
                    Piso = pd.Domicilio.piso,
                    Provincia = pd.Domicilio.provincia,
                    Tipo = pd.TipoDomicilio.Descripcion
                }).ToList();
            }

            private static DDJJSor ToDDJJSoR(this DDJJSorTemporal tempObj)
            {
                return new DDJJSor
                {
                    IdMensura = tempObj.IdMensura,
                    IdSor = tempObj.IdSor,
                    NumeroHabitantes = tempObj.NumeroHabitantes,
                    DistanciaCamino = tempObj.DistanciaCamino,
                    DistanciaEmbarque = tempObj.DistanciaEmbarque,
                    DistanciaLocalidad = tempObj.DistanciaLocalidad,
                    IdCamino = tempObj.IdCamino,
                    IdLocalidad = tempObj.IdLocalidad,

                    IdUsuarioAlta = tempObj.UsuarioAlta,
                    FechaAlta = tempObj.FechaAlta,
                    IdUsuarioModif = tempObj.UsuarioModif,
                    FechaModif = tempObj.FechaModif
                };
            }
            private static List<VALAptitudInput> ToDDJJAptitudesInput(this DDJJSorTemporal tempObj, IEnumerable<VALAptitudes> aptitudes)
            {
                //.Where(apt => apt.IdVersion == version.IdVersion)
                return aptitudes.Select(apt =>
                                {
                                    var superficie = tempObj.Superficies.SingleOrDefault(s => s.IdAptitud == apt.IdAptitud);
                                    return new VALAptitudInput()
                                    {
                                        IdAptitud = apt.IdAptitud,
                                        Superficie = (superficie?.Superficie ?? 0d).ToString("0.0000"),
                                        RelieveSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.Relieve),
                                        AguasDelSubsueloSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.AguaEnSubsuelo),
                                        CapacidadesGanaderasSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.CapacidadGanadera),
                                        ColoresTierraSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.ColorDeLaTierra),
                                        EspesoresCapaArableSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.EspesorCapaArable),
                                        EstadosMonteSeleccionado = tempObj.SoRCars.ObtenerValorCaracteristica(apt, TiposCaracteristicas.CalidadMonte)
                                    };
                                }).ToList();
            }
            private static string ObtenerValorCaracteristica(this IEnumerable<DDJJSorCarTemporal> caracteristicas, VALAptitudes aptitud, TiposCaracteristicas caracteristica)
            {
                return caracteristicas
                        .SingleOrDefault(x => x.AptCar.IdAptitud == aptitud.IdAptitud &&
                                             (TiposCaracteristicas)x.AptCar.SorCaracteristica.IdSorTipoCaracteristica == caracteristica)
                        ?.AptCar
                        ?.IdSorCar.ToString() ?? string.Empty;
            }

            private static INMMejora ToMejora(this INMMejoraTemporal tempObj)
            {
                return new INMMejora()
                {
                    IdDestinoMejora = tempObj.IdDestinoMejora,
                    IdEstadoConservacion = tempObj.IdEstadoConservacion,
                };
            }
            private static Aforo BuscarAforo(this DDJJUMedidaLinealTemporal medidaLinealTemp, IEnumerable<Aforo> aforos)
            {
                return aforos.FirstOrDefault(a => a.IdTramoVia == medidaLinealTemp.IdTramoVia && a.IdVia == medidaLinealTemp.IdVia);
            }
            private static Persona BuscarPersona(this DDJJDominioTitularTemporal domTitularTemp, IEnumerable<Persona> personas)
            {
                return personas.Single(p => p.PersonaId == domTitularTemp.IdPersona);
            }
            private static List<long> ToCaracteristicasToSave(this INMMejoraTemporal tempObj)
            {
                return tempObj.MejorasCar.Select(cs => cs.IdCaracteristica).ToList();
            }
            private static List<MejoraOtrasCar> ToOtrasCar(this INMMejoraTemporal tempObj)
            {
                return tempObj.OtrasCar.Select(oc => new MejoraOtrasCar() { IdOtraCar = oc.IdOtraCar, Valor = oc.Valor }).ToList();
            }
        }
    }
}