using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GeoSit.Data.DAL.Repositories
{
    public class ValidacionDBRepository : IValidacionDBRepository
    {
        private readonly GeoSITMContext _context;

        internal ValidacionDBRepository(GeoSITMContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Este método parmite validar un grupo de validaciones quedándose con la función de mayor severidad
        /// en caso de que haya un grupo tenga la misma validación para funciones diferentes y distinto 
        /// nivel de severidad.
        /// 
        /// Ejemplo: la validación X se ejecuta en la funciones 1 y 2. La función 1 es advertencia. La 2, error.
        /// El sistema sólo evaluará la validación X para la función 2, que es la de mayor severidad.
        /// </summary>
        /// <param name="objetoValidable"></param>
        public ResultadoValidacion ValidarFuncionGrupo(ObjetoValidable objetoValidable, out List<string> errores)
        {
            short idTipoObjeto = (short)objetoValidable.TipoObjeto;
            long idObjeto = (long)objetoValidable.IdObjeto;
            short grupoFuncion = (short)objetoValidable.Grupo;

            /*var query = from reg in (from val in _context.Validaciones
                                     join func in _context.ValidacionesFunciones on val.IdValidacion equals func.IdValidacion
                                     join param in _context.ValidacionesParametros on val.IdValidacion equals param.IdValidacion
                                     join grfnc in _context.ValidacionesGruposFunciones on func.IdFuncion equals grfnc.IdFuncion
                                     where val.Activa && func.Activa && val.IdTipoObjeto == idTipoObjeto && grfnc.IdGrupo == grupoFuncion
                                     group new { param, func } by val into grp
                                     select new { val = grp.Key, grupo = grp.Where(a => a.func.IdTipoMensaje == grp.Max(b => b.func.IdTipoMensaje)) })
                        select new ConfigValidacion
                        {
                            Configuracion = reg.val,
                            Funcion = reg.grupo.FirstOrDefault(x => x.func.IdTipoMensaje == reg.grupo.Max(v => v.func.IdTipoMensaje)).func,
                            Parametros = reg.grupo.Select(x => x.param)
                        };*/

            var query = from reg in (from valSub in _context.ValidacionesSubtipos
                                     join val in _context.Validaciones on valSub.IdValidacion equals val.IdValidacion
                                     join func in _context.ValidacionesFunciones on val.IdValidacion equals func.IdValidacion
                                     join param in _context.ValidacionesParametros on val.IdValidacion equals param.IdValidacion
                                     join grfnc in _context.ValidacionesGruposFunciones on func.IdFuncion equals grfnc.IdFuncion
                                     where val.Activa == true && valSub.Activa == true && func.Activa == true && val.IdTipoObjeto == idTipoObjeto && 
                                     valSub.IdValidacionSubtipo == idObjeto && grfnc.IdGrupo == grupoFuncion && val.FechaBaja == null && valSub.FechaBaja == null && 
                                     func.FechaBaja == null && param.FechaBaja == null
                                     group new { param, func } by val into grp
                                     select new { val = grp.Key, grupo = grp.Where(a => a.func.IdTipoMensaje == grp.Max(b => b.func.IdTipoMensaje)) })
                        select new ConfigValidacion
                        {
                            Configuracion = reg.val,
                            Funcion = reg.grupo.FirstOrDefault(x => x.func.IdTipoMensaje == reg.grupo.Max(v => v.func.IdTipoMensaje)).func,
                            Parametros = reg.grupo.Select(x => x.param)
                        };

            return procesar(query, objetoValidable, out errores);
        }
        public ResultadoValidacion Validar(ObjetoValidable objetoValidable, out List<string> errores)
        {
            short idTipoObjeto = (short)objetoValidable.TipoObjeto;
            long idObjeto = (long)objetoValidable.IdObjeto;
            short? idFuncion = objetoValidable.Funcion == FuncionValidable.Todas ? (short?)null : (short)objetoValidable.Funcion;

            var validaciones = from valSub in _context.ValidacionesSubtipos
                               join val in _context.Validaciones on valSub.IdValidacion equals val.IdValidacion
                               join func in _context.ValidacionesFunciones on val.IdValidacion equals func.IdValidacion
                               join param in _context.ValidacionesParametros on val.IdValidacion equals param.IdValidacion
                               where val.IdTipoObjeto == idTipoObjeto && valSub.IdValidacionSubtipo == idObjeto &&
                               (func.IdFuncion == idFuncion || idFuncion == null) && val.Activa == true && valSub.Activa == true && func.Activa == true &&
                               val.FechaBaja == null && valSub.FechaBaja == null && func.FechaBaja == null && param.FechaBaja == null
                               group param by new { val, func } into grp
                               select new ConfigValidacion { Configuracion = grp.Key.val, Funcion = grp.Key.func, Parametros = grp };

            return procesar(validaciones, objetoValidable, out errores);
        }

        private ResultadoValidacion procesar(IQueryable<ConfigValidacion> validaciones, ObjetoValidable objetoValidable, out List<string> errores)
        {
            errores = new List<string>();

            var propiedades = objetoValidable
                                    .GetType()
                                    .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                                    .Select(p => new { propiedad = p.Name, valor = p.GetValue(objetoValidable) })
                                    .ToDictionary(p => p.propiedad, p => p.valor);

            if (_context.Database.Connection.State != ConnectionState.Open)
            {
                _context.Database.Connection.Open();
            }

            short maxErrLevel = 0;
            foreach (var validacion in validaciones.ToList())
            {
                using (var cmd = _context.Database.Connection.CreateCommand())
                {
                    string mensaje = validacion.Configuracion.Mensaje;
                    cmd.CommandText = validacion.Configuracion.Sentencia;
                    cmd.Parameters
                        .AddRange(validacion.Parametros
                                            .Select(param =>
                                            {
                                                var dbParam = cmd.CreateParameter();
                                                dbParam.ParameterName = param.Parametro;
                                                dbParam.Value = propiedades[param.Propiedad];

                                                mensaje = mensaje.Replace($"{param.Parametro}@", $"{propiedades[param.Propiedad]}");

                                                return dbParam;
                                            }).ToArray());

                    object ret = cmd.ExecuteScalar();
                    if (ret != DBNull.Value && !Convert.ToBoolean(ret))
                    {
                        string tipo = "Error";
                        maxErrLevel = Math.Max(maxErrLevel, validacion.Funcion.IdTipoMensaje);
                        switch ((TipoMensaje)validacion.Funcion.IdTipoMensaje)
                        {
                            case TipoMensaje.Advertencia:
                                tipo = "Advertencia";
                                break;
                            case TipoMensaje.Bloqueo:
                                tipo = "Bloqueo";
                                break;
                            default: //error
                                break;
                        }
                        errores.Add($"({tipo}) {mensaje}");
                    }
                }
            }
            ResultadoValidacion resultado = ResultadoValidacion.Error;
            switch ((TipoMensaje)maxErrLevel)
            {
                case TipoMensaje.Ok:
                    resultado = ResultadoValidacion.Ok;
                    break;
                case TipoMensaje.Advertencia:
                    resultado = ResultadoValidacion.Advertencia;
                    break;
                case TipoMensaje.Bloqueo:
                    resultado = ResultadoValidacion.Bloqueo;
                    break;
                default: //error
                    break;
            }
            return resultado;
        }
    }

    public class ConfigValidacion
    {
        public Validacion Configuracion { get; set; }
        public ValidacionFuncion Funcion { get; set; }
        public IEnumerable<ValidacionParametro> Parametros { get; set; }
        public ConfigValidacion() { }
    }
}