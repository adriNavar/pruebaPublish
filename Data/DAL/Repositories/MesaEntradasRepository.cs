using GeoSit.Data.BusinessEntities.Common;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.MesaEntradas.DTO;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Common;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using GeoSit.Data.BusinessEntities.DeclaracionesJuradas;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.Personas;
using GeoSit.Data.BusinessEntities.Via;
using static GeoSit.Data.BusinessEntities.Common.Enumerators;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Implementaciones;
using GeoSit.Data.BusinessEntities.ValidacionesDB.Enums;
using GeoSit.Data.BusinessEntities.ValidacionesDB;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Common.ExtensionMethods.Atributos;
using GeoSit.Data.BusinessEntities.MapasTematicos;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Geosit.Data.DAL.DDJJyValuaciones.Enums;
using System.Data;

namespace GeoSit.Data.DAL.Repositories
{
    public class MesaEntradasRepository : BaseRepository<METramite, int>
    {
        public enum Grilla { EnProceso, Pendientes, Procesados, Todos, Reasignados }

        public MesaEntradasRepository(GeoSITMContext ctx)
            : base(ctx) { }

        public DataTableResult<GrillaTramite> RecuperarTramites(Grilla grilla, DataTableParameters parametros, long idUsuario)
        {
            var joins = new Expression<Func<METramite, dynamic>>[] { x => x.Iniciador, x => x.Prioridad, x => x.Tipo, x => x.Objeto, x => x.Estado, x => x.Movimientos.Select(m => m.SectorOrigen), x => x.Movimientos.Select(m => m.SectorDestino) };
            var filtros = new List<Expression<Func<METramite, bool>>>();
            var sorts = new List<SortClause<METramite>>();

            var ctx = this.GetContexto();
            var ultimosMovimientos = from ultmov in ctx.MovimientosTramites
                                     where ultmov.IdMovimiento == (from mov in ctx.MovimientosTramites
                                                                   where mov.IdTramite == ultmov.IdTramite
                                                                   group mov.IdMovimiento by mov.IdTramite into grp
                                                                   select grp.Max(id => id)).FirstOrDefault()
                                     select ultmov;

            int idSectorUsuario = this.GetContexto().Usuarios.Find(idUsuario)?.IdSector ?? 0;

            switch (grilla)
            {
                case Grilla.EnProceso:
                    {
                        //1=Provisorio, 3=Iniciado, 4=En Proceso, 6=Anulado, 9=Finalizado, 11=Procesado
                        var estados = new[] { 1, 3, 4, 11 };
                        //se muestran trámites que hayan sido modificados por el usuario y que se
                        //encuentren en los estados especificados o, si está "Finalizado" o "Anulado", que no 
                        //haya sido derivado a otro sector
                        filtros.Add(t => t.UsuarioModif == idUsuario &&
                                            (estados.Contains(t.IdEstado) ||
                                            (t.IdEstado == 9 || t.IdEstado == 6) && ultimosMovimientos.Any(mov => mov.IdTramite == t.IdTramite &&
                                                                                             mov.IdSectorDestino == idSectorUsuario /*&&
                                                                                             //mov.IdSectorDestino == mov.IdSectorOrigen*/)));
                    }
                    break;
                case Grilla.Pendientes:
                    {
                        //2=Presentado, 5=Derivado, 6=Anulado, 8=Rechazado, 9=Finalizado, 11=Procesado
                        var estados = new[] { 2, 5, 6, 8, 9 };
                        var estadosSoloPorSectorUsuario = new[] { 2, 5, 8 };
                        //estados que van sólo si están en el sector del usuario
                        filtros.Add(t => estados.Contains(t.IdEstado) &&
                                         ultimosMovimientos.Any(mov => mov.IdTramite == t.IdTramite &&
                                                                       mov.IdSectorDestino == idSectorUsuario) &&
                                         //si es Derivado, Presentado o Rechazado va siempre
                                         (estadosSoloPorSectorUsuario.Contains(t.IdEstado) ||
                                            //si es Presentado, Anulado o Finalizado, sólo si no lo hizo el usuario
                                            t.UsuarioModif != idUsuario));
                    }
                    break;
                case Grilla.Procesados:
                    {
                        //4=En Proceso, 5=Derivado, 6=Anulado, 7=Despachado, 8=Rechazado, 9=Finalizado, 10=Archivado, 11=Procesado,12=Entregado
                        var estados = new[] { 10, 12 };
                        var estadosPorUsuario = new[] { 5, 6, 7, 8, 9 };
                        var estadosPorSectorUsuario = new[] { 4, 5, 6, 7, 8, 9 };
                        var estadosMeDaIgualSectorActual = new[] { 4, 7 };

                        var movimientosNoUltimosPorSectorUsuario = from mov in this.GetContexto().MovimientosTramites
                                                                   where mov.IdSectorDestino == idSectorUsuario &&
                                                                   !ultimosMovimientos.Any(um => mov.IdTramite == um.IdTramite &&
                                                                                                mov.IdSectorDestino == um.IdSectorDestino)
                                                                   select mov;

                        filtros.Add(t => //estados que van sin ninguna otra consideración
                                         estados.Contains(t.IdEstado) ||

                                         //estados que van si fueron del usuario
                                         estadosPorUsuario.Contains(t.IdEstado) && t.UsuarioModif == idUsuario &&
                                                //si es Despachado va siempre, si no, sólo va si sale del sector del usuario
                                                (t.IdEstado == 7 || ultimosMovimientos.Any(mov => mov.IdTramite == t.IdTramite &&
                                                                                                  mov.IdSectorOrigen == idSectorUsuario &&
                                                                                                  mov.IdSectorOrigen != mov.IdSectorDestino)) ||

                                         //tengo en cuenta si alguna vez pasaron por el sector del usuario
                                         estadosPorSectorUsuario.Contains(t.IdEstado) && t.UsuarioModif != idUsuario &&
                                                //si alguna vez estuvo En Proceso o Despachado va siempre, si no,
                                                (estadosMeDaIgualSectorActual.Contains(t.IdEstado) ||
                                                 //sólo lo incluyo si no está actualmente en el sector
                                                 movimientosNoUltimosPorSectorUsuario.Any(mov => mov.IdTramite == t.IdTramite)));
                    }
                    break;
                case Grilla.Todos:
                    {
                        filtros.Add(t => t.UsuarioAlta == idUsuario);
                    }
                    break;
                case Grilla.Reasignados:
                    {
                        //1=Provisorio, 3=Iniciado, 4=En Proceso, 6=Anulado, 9=Finalizado, 11=Procesado
                        var estados = new[] { 1, 3, 4, 6, 9, 11 };
                        //filtra los trámites que estén en estado 1,3,4,11,6,9,
                        //que no sean del actual usuario
                        //y que sus ultimos movimientos tengan como sectorOrigen y sectorDestino el idSectorUsuario
                        filtros.Add(t => t.UsuarioModif != idUsuario &&
                                    estados.Contains(t.IdEstado) && ultimosMovimientos.Any(mov => mov.IdTramite == t.IdTramite &&
                                                                                           mov.IdSectorOrigen == idSectorUsuario &&
                                                                                           mov.IdSectorDestino == mov.IdSectorOrigen));
                    }
                    break;
                default:
                    break;
            }

            foreach (var column in parametros.columns.Where(c => !string.IsNullOrEmpty(c.search.value)))
            {
                switch (column.name)
                {
                    case "IdTramite":
                        {
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => tramite.IdTramite == val);
                        }
                        break;
                    case "Numero":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(tramite => tramite.Numero.ToLower().Contains(val));
                        }
                        break;
                    case "Iniciador":
                        {
                            string val = column.search.value.ToLower();
                            filtros.Add(tramite => tramite.Iniciador.NombreCompleto.ToLower().Contains(val));
                        }
                        break;
                    case "Tipo":
                        {
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => tramite.IdTipoTramite == val);
                        }
                        break;
                    case "Objeto":
                        {
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => tramite.IdObjetoTramite == val);
                        }
                        break;
                    case "Estado":
                        {
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => tramite.IdEstado == val);
                        }
                        break;
                    case "SectorOrigen":
                        { //esto aplica para grillas "en proceso", "pendientes" y "reasignados"
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => ultimosMovimientos.Any(mov => mov.IdTramite == tramite.IdTramite && mov.IdSectorOrigen == val));
                        }
                        break;
                    case "SectorDestino":
                        { //esto aplica para grillas "procesados"
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => ultimosMovimientos.Any(mov => mov.IdTramite == tramite.IdTramite && mov.IdSectorDestino == val));
                        }
                        break;
                    case "Prioridad":
                        {
                            int val = int.Parse(column.search.value);
                            filtros.Add(tramite => tramite.IdPrioridad == val);
                        }
                        break;
                    case "FechaVenc":
                        {
                            DateTime val = DateTime.Parse(column.search.value).Date;
                            filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaVenc) == val);
                        }
                        break;
                    case "FechaUltAct":
                        {
                            DateTime val = DateTime.Parse(column.search.value).Date;
                            filtros.Add(tramite => ultimosMovimientos.Any(mov => mov.IdTramite == tramite.IdTramite && DbFunctions.TruncateTime(mov.FechaAlta) == val));
                        }
                        break;
                    default:
                        break;
                }
            }
            bool asc = parametros.order.FirstOrDefault().dir == "asc";
            switch (parametros.columns[parametros.order.FirstOrDefault().column].name)
            {
                case "Numero":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Numero, ASC = asc });
                    break;
                case "Iniciador":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Iniciador.NombreCompleto, ASC = asc });
                    break;
                case "Tipo":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Tipo.Descripcion, ASC = asc });
                    break;
                case "Objeto":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Objeto.Descripcion, ASC = asc });
                    break;
                case "Estado":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Estado.Descripcion, ASC = asc });
                    break;
                case "Prioridad":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Prioridad.Descripcion, ASC = asc });
                    break;
                case "FechaVenc":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.FechaVenc, ASC = asc });
                    break;
                case "FechaUltAct":
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Movimientos.Max(m => m.FechaAlta), ASC = asc });
                    break;
                case "SectorOrigen": //esto aplica para grillas "en proceso" y "pendientes"
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Movimientos.Max(m => m.SectorOrigen.Nombre), ASC = asc });
                    break;
                case "SectorDestino": //esto aplica para grillas "procesados"
                    sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.Movimientos.Max(m => m.SectorDestino.Nombre), ASC = asc });
                    break;
                default:
                    OrdenDefaultASC = asc;
                    break;
            }
            return ObtenerPagina(GetBaseQuery(joins, filtros, sorts), parametros, mapParaGrilla);
        }

        public Mensura GetMensuraByIdDDJJ(long idDDJJ)
        {
            var ddjjU = this.GetContexto().DDJJU;
            var ddjjSor = this.GetContexto().DDJJSor;
            return (from m in this.GetContexto().Mensura
                    where m.FechaBaja == null &&
                          (ddjjU.Any(ddjju => ddjju.FechaBaja == null && ddjju.IdDeclaracionJurada == idDDJJ && ddjju.IdMensura == m.IdMensura) ||
                           ddjjSor.Any(ddjjsor => ddjjsor.FechaBaja == null && ddjjsor.IdDeclaracionJurada == idDDJJ && ddjjsor.IdMensura == m.IdMensura))
                    select m).FirstOrDefault();
        }

        public Persona GetTramitePersonaEmail(int idTramite)
        {
            var query = (from p in this.GetContexto().Persona
                         join t in this.GetContexto().TramitesMesaEntrada on p.PersonaId equals t.IdIniciador
                         where t.IdTramite == idTramite
                         select p).FirstOrDefault();

            return query;
        }

        public METramite GetTramiteEmail(int idTramite)
        {
            var query = (from t in this.GetContexto().TramitesMesaEntrada
                         where t.IdTramite == idTramite
                         select t).FirstOrDefault();

            return query;
        }

        public MEObjetoTramite GetPlantilla(int idObjetoTramite)
        {
            var query = (from p in this.GetContexto().ObjetosTramites
                         where p.IdObjetoTramite == idObjetoTramite
                         select p).FirstOrDefault();

            return query;
        }

        public List<METipoMovimiento> GetAccionesGenerales(int cantTramites, long idUsuario, bool grillaReasignable)
        {
            var funciones = GetFuncionesByUsuario(idUsuario);
            var lstTipoMovimiento = new List<METipoMovimiento>();
            var usuario = this.GetContexto().Usuarios.First(u => u.Id_Usuario == idUsuario);
            bool esProfesional = int.TryParse(this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_EXTERNO").FirstOrDefault()?.Valor, out int idSectorProfesional) &&
                                   usuario.IdSector == idSectorProfesional;
            AgregarAccionPermitida(EnumTipoMovimiento.Crear, EnumEvento.CrearTramite, lstTipoMovimiento, funciones);
            if (grillaReasignable && !esProfesional && cantTramites > 0)
            {
                AgregarAccionPermitida(EnumTipoMovimiento.Reasignar, EnumEvento.ReasignarTramite, lstTipoMovimiento, funciones);
            }
            if (cantTramites == 1)
            {
                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.ConsultarTramite, EnumEvento.ImprimirCaratula, EnumEvento.ImprimirInformeDetallado });
            }
            return lstTipoMovimiento;
        }
        private void AgregarAccionesNoMovimientos(List<METipoMovimiento> lstTipoMovimiento, IEnumerable<long> funciones, IEnumerable<EnumEvento> eventosRequeridos)
        {
            var tipoMovimientoByEvento = new Dictionary<long, Tuple<int, string>>()
            {
                { (long)EnumEvento.ConsultarTramite, Tuple.Create(102,"Consultar") },
                { (long)EnumEvento.ImprimirCaratula, Tuple.Create(100,"Imprimir Carátula")},
                { (long)EnumEvento.ImprimirInformeDetallado, Tuple.Create(101,"Imprimir Informe Detallado") },
                { (long)EnumEvento.ImprimirInformeAdjudicacion, Tuple.Create(103,"Imprimir Informe Adjudicación") },
                { (long)EnumEvento.NotificarTramite, Tuple.Create(104,"Notificar") }
            };

            var idsEvento = eventosRequeridos.Select(a => (long)a).ToArray();
            var query = (from evento in this.GetContexto().SEEventos
                         where idsEvento.Contains(evento.Id_Evento)
                         select new { idEvento = evento.Id_Evento, idFuncion = evento.Id_Funcion }).ToArray();

            query = query.Where(reg => funciones.Any(f => f == reg.idFuncion)).ToArray();
            lstTipoMovimiento.AddRange(query.Select(evt => new METipoMovimiento()
            {
                IdTipoMovimiento = tipoMovimientoByEvento[evt.idEvento].Item1,
                Descripcion = tipoMovimientoByEvento[evt.idEvento].Item2
            }).OrderBy(mov => mov.Descripcion));

        }
        public List<METipoMovimiento> GetAccionesNoMovimientosUsuarioEdicion(long idTramite, long idUsuario)
        {
            var lista = new List<METipoMovimiento>();
            if (Math.Max(idTramite, 0) == 0)
            {
                return lista;
            }
            var funciones = GetFuncionesByUsuario(idUsuario);
            AgregarAccionesNoMovimientos(lista, funciones, new[] { EnumEvento.ImprimirCaratula, EnumEvento.ImprimirInformeDetallado });
            return lista;
        }
        public List<METipoMovimiento> GetAccionesByTramite(long id, long idUsuario, bool grillaReasignable)
        {
            var lstTipoMovimiento = new List<METipoMovimiento>();

            var tramite = this.GetContexto().TramitesMesaEntrada.Include("Movimientos").Single(t => t.IdTramite == id);
            var ultimoMov = tramite.Movimientos.Single(m => m.FechaAlta == tramite.Movimientos.Max(f => f.FechaAlta));
            var usuario = this.GetContexto().Usuarios.First(u => u.Id_Usuario == idUsuario);
            //DateTime? fechaUltDespacho = null;
            var fechaUltDespacho = (tramite.Movimientos != null && tramite.Movimientos.Count > 0 ? tramite.Movimientos.SingleOrDefault(m => m.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar && m.IdMovimiento == tramite.Movimientos.Where(p => p.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar).Max(f => f.IdMovimiento))?.FechaAlta : null);

            bool esProfesional = int.TryParse(this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_EXTERNO").FirstOrDefault()?.Valor, out int idSectorProfesional) &&
                                    usuario.IdSector == idSectorProfesional;

            var funciones = GetFuncionesByUsuario(usuario.Id_Usuario);

            bool reasignar = funciones.Contains(518);

            switch ((EnumEstadoTramite)tramite.IdEstado)
            {
                case EnumEstadoTramite.Provisorio:
                    {
                        if (!grillaReasignable)
                        {
                            AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                            if (!esProfesional)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Confirmar, EnumEvento.ConfirmarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                            }
                            else
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Presentar, EnumEvento.PresentarTramite, lstTipoMovimiento, funciones);
                            }
                            AgregarAccionPermitida(EnumTipoMovimiento.Anular, EnumEvento.AnularTramite, lstTipoMovimiento, funciones);
                        }
                    }
                    break;
                case EnumEstadoTramite.Presentado:
                    {
                        if (!esProfesional)
                        {
                            AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                            AgregarAccionPermitida(EnumTipoMovimiento.Confirmar, EnumEvento.ConfirmarTramite, lstTipoMovimiento, funciones);
                            AgregarAccionPermitida(EnumTipoMovimiento.Anular, EnumEvento.AnularTramite, lstTipoMovimiento, funciones);
                            AgregarAccionPermitida(EnumTipoMovimiento.Despachar, EnumEvento.DespacharTramite, lstTipoMovimiento, funciones);
                            AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                        }
                    }
                    break;
                case EnumEstadoTramite.Iniciado:
                    {
                        if (!grillaReasignable)
                        {
                            if (!esProfesional)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Procesar, EnumEvento.ProcesarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Anular, EnumEvento.AnularTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Derivar, EnumEvento.DerivarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Anular_Carga_Libro, EnumEvento.AnularCargaLibroTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Despachar, EnumEvento.DespacharTramite, lstTipoMovimiento, funciones);
                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                            }
                        }

                    }
                    break;
                case EnumEstadoTramite.En_proceso:
                    {
                        if (!grillaReasignable)
                        {
                            if (tramite.UsuarioModif == idUsuario && !esProfesional)
                            {


                                AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Procesar, EnumEvento.ProcesarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Anular, EnumEvento.AnularTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Derivar, EnumEvento.DerivarTramite, lstTipoMovimiento, funciones);
                                //AgregarAccionPermitida(EnumTipoMovimiento.Finalizar, EnumEvento.FinalizarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Despachar, EnumEvento.DespacharTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Rechazar, EnumEvento.RechazarTramite, lstTipoMovimiento, funciones);


                            }
                            if (!esProfesional)
                            {

                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                            }
                        }
                    }
                    break;
                case EnumEstadoTramite.Derivado:
                    {
                        if (!esProfesional)
                        {
                            if (ultimoMov.IdSectorDestino == usuario.IdSector && ultimoMov.UsuarioAlta != usuario.Id_Usuario)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Recibir, EnumEvento.RecibirTramite, lstTipoMovimiento, funciones);
                            }
                            else if (ultimoMov.UsuarioAlta == usuario.Id_Usuario)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Anular_derivacion, EnumEvento.AnularDerivaciónTramite, lstTipoMovimiento, funciones);
                            }
                            AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                        }
                    }
                    break;
                case EnumEstadoTramite.Anulado:
                    {
                        if (!grillaReasignable)
                        {
                            if (esProfesional)
                            {
                                if (ultimoMov.IdSectorDestino == usuario.IdSector)
                                {
                                    AgregarAccionPermitida(EnumTipoMovimiento.Archivar, EnumEvento.ArchivarTramite, lstTipoMovimiento, funciones);
                                }
                            }
                            else
                            {

                                if (ultimoMov.IdSectorOrigen == usuario.IdSector && ultimoMov.IdSectorOrigen != ultimoMov.IdSectorDestino)
                                { // procesados saliente
                                    AgregarAccionPermitida(EnumTipoMovimiento.Anular_derivacion, EnumEvento.AnularDerivaciónTramite, lstTipoMovimiento, funciones);
                                }
                                else if (tramite.UsuarioModif == idUsuario)
                                { // en proceso
                                    AgregarAccionPermitida(EnumTipoMovimiento.Archivar, EnumEvento.ArchivarTramite, lstTipoMovimiento, funciones);
                                    AgregarAccionPermitida(EnumTipoMovimiento.Derivar, EnumEvento.DerivarTramite, lstTipoMovimiento, funciones);
                                }
                                else if (tramite.UsuarioModif != idUsuario && ultimoMov.IdSectorDestino == usuario.IdSector)
                                { //pendientes
                                    AgregarAccionPermitida(EnumTipoMovimiento.Recibir, EnumEvento.RecibirTramite, lstTipoMovimiento, funciones);
                                }

                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });

                            }
                        }
                    }
                    break;
                case EnumEstadoTramite.Despachado:
                    {
                        if (esProfesional)
                        {
                            if (fechaUltDespacho.HasValue)
                            {
                                var fechaVigenciaReingreso = fechaUltDespacho.Value.AddDays(365);
                                if (fechaVigenciaReingreso >= DateTime.Today)
                                {
                                    if (ultimoMov.IdSectorDestino == usuario.IdSector)
                                    {
                                        AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                                        AgregarAccionPermitida(EnumTipoMovimiento.Reingresar, EnumEvento.ReingresarTramite, lstTipoMovimiento, funciones);
                                    }
                                }
                            }
                            else
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                            }
                        }
                        else
                        {
                            AgregarAccionPermitida(EnumTipoMovimiento.Editar, EnumEvento.EditarTramite, lstTipoMovimiento, funciones);
                            if (ultimoMov.IdSectorOrigen == usuario.IdSector)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Reingresar, EnumEvento.ReingresarTramite, lstTipoMovimiento, funciones);
                            }
                            AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                        }
                    }
                    break;
                case EnumEstadoTramite.Rechazado:
                    {
                        if (!esProfesional)
                        {
                            if (ultimoMov.IdSectorDestino == usuario.IdSector && ultimoMov.IdSectorDestino != ultimoMov.IdSectorOrigen)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Recibir, EnumEvento.RecibirTramite, lstTipoMovimiento, funciones);
                            }
                            AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                        }
                    }
                    break;
                case EnumEstadoTramite.Finalizado:
                    {
                        if (!grillaReasignable)
                        {
                            if (!esProfesional)
                            {

                                if (ultimoMov.IdSectorOrigen == usuario.IdSector && ultimoMov.IdSectorOrigen != ultimoMov.IdSectorDestino)
                                { // procesados saliente
                                    AgregarAccionPermitida(EnumTipoMovimiento.Anular_derivacion, EnumEvento.AnularDerivaciónTramite, lstTipoMovimiento, funciones);
                                }
                                else if (tramite.UsuarioModif == idUsuario)
                                { // en proceso
                                    AgregarAccionPermitida(EnumTipoMovimiento.Archivar, EnumEvento.ArchivarTramite, lstTipoMovimiento, funciones);
                                    AgregarAccionPermitida(EnumTipoMovimiento.Derivar, EnumEvento.DerivarTramite, lstTipoMovimiento, funciones);
                                    AgregarAccionPermitida(EnumTipoMovimiento.Entregar, EnumEvento.EntregarTramite, lstTipoMovimiento, funciones);
                                }
                                else if (tramite.UsuarioModif != idUsuario && ultimoMov.IdSectorDestino == usuario.IdSector)
                                { //pendientes
                                    AgregarAccionPermitida(EnumTipoMovimiento.Recibir, EnumEvento.RecibirTramite, lstTipoMovimiento, funciones);
                                }

                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });

                            }
                        }
                    }
                    break;
                case EnumEstadoTramite.Archivado:
                    {
                        if (esProfesional)
                        {
                            if (ultimoMov.IdSectorDestino == usuario.IdSector)
                            {
                                AgregarAccionPermitida(EnumTipoMovimiento.Desarchivar, EnumEvento.DesarchivarTramite, lstTipoMovimiento, funciones);
                            }
                        }
                        else
                        {
                            AgregarAccionPermitida(EnumTipoMovimiento.Desarchivar, EnumEvento.DesarchivarTramite, lstTipoMovimiento, funciones);
                            AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });
                        }
                    }
                    break;
                case EnumEstadoTramite.Procesado:
                    {
                        if (!grillaReasignable)
                        {
                            if (tramite.IdTipoTramite == (int)EnumTipoTramite.Mensura)
                            {
                                var mensura = GetTramiteMensura(tramite.IdTramite);
                                if (mensura != null && mensura.IdMensura > 0)
                                {
                                    AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.ImprimirInformeAdjudicacion });
                                }
                            }
                            if (!esProfesional)
                            {

                                AgregarAccionPermitida(EnumTipoMovimiento.Anular, EnumEvento.AnularTramite, lstTipoMovimiento, funciones);
                                AgregarAccionPermitida(EnumTipoMovimiento.Finalizar, EnumEvento.FinalizarTramite, lstTipoMovimiento, funciones);
                                AgregarAccionesNoMovimientos(lstTipoMovimiento, funciones, new[] { EnumEvento.NotificarTramite });

                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return lstTipoMovimiento;
        }

        private void AgregarAccionPermitida(EnumTipoMovimiento estadoTramite, EnumEvento evento, List<METipoMovimiento> lstTipoMovimiento, IEnumerable<long> funciones)
        {
            if (funciones.Any(id => id.Equals(this.GetContexto().SEEventos.Find((long)evento).Id_Funcion)))
            {
                lstTipoMovimiento.Add(this.GetContexto().TiposMovimientos.Find((int)estadoTramite));
            }
        }

        public List<METipoMovimiento> GetAccionesByTramites(long[] aTramitesId, long idUsuario, bool grillaReasignable)
        {
            var lstAcciones = new List<METipoMovimiento>();
            if (aTramitesId.Any())
            {
                lstAcciones = GetAccionesByTramite(aTramitesId[0], idUsuario, grillaReasignable);
                for (int i = 1; i < aTramitesId.Length; i++)
                {
                    lstAcciones = lstAcciones.Intersect(GetAccionesByTramite(aTramitesId[i], idUsuario, grillaReasignable)).ToList();
                }
            }
            return lstAcciones;
        }

        private List<long> GetFuncionesByUsuario(long idUsuario)
        {
            var perfilesFunciones = this.GetContexto().PerfilesFunciones;
            var funciones = this.GetContexto().Funciones;
            var usuariosPerfiles = this.GetContexto().UsuariosPerfiles;

            var query = from up in usuariosPerfiles
                        join pf in perfilesFunciones on up.Id_Perfil equals pf.Id_Perfil
                        join f in funciones on pf.Id_Funcion equals f.Id_Funcion
                        where up.Id_Usuario == idUsuario && up.Fecha_Baja == null && pf.Fecha_Baja == null
                        select f.Id_Funcion;

            return query.Distinct().ToList();
        }

        public int EstadoAnteriorTramite(int idTramite)
        {
            var movimiento = (from mm in this.GetContexto().MovimientosTramites
                              where mm.IdTramite == idTramite && mm.IdTipoMovimiento != 6 && mm.IdTipoMovimiento != 4
                              && mm.IdTipoMovimiento != 8 && mm.IdTipoMovimiento != 2
                              select mm);

            var IdEstadoAnterior = movimiento.OrderByDescending(x => x.FechaAlta).FirstOrDefault().IdEstado;

            return IdEstadoAnterior;
        }

        public bool EjecutarAccion(long idUsuario, AccionParameters accionParameters)
        {
            EnumTipoMovimiento tipoMovimiento = (EnumTipoMovimiento)int.Parse(accionParameters.accion);
            int[] aTramite = accionParameters.tramites;
            string observ = accionParameters.observ;
            FuncionValidable funcionValidable = FuncionValidable.Ninguna;

            var usuario = this.GetContexto().Usuarios.Find(idUsuario);
            if (!int.TryParse(accionParameters.sector, out int idSectorDestino))
            {
                idSectorDestino = (int)usuario.IdSector;
            }

            foreach (var tram in aTramite)
            {
                int idEstado = 0;
                string eventoNombre = string.Empty;
                switch (tipoMovimiento)
                {
                    case EnumTipoMovimiento.Derivar:
                        idEstado = (int)EnumEstadoTramite.Derivado;
                        eventoNombre = Eventos.DerivarTramite;
                        break;
                    case EnumTipoMovimiento.Reasignar:
                        idEstado = EstadoAnteriorTramite(tram);
                        eventoNombre = Eventos.ReasignarTramite;
                        break;
                    case EnumTipoMovimiento.Recibir:
                        idEstado = EstadoAnteriorTramite(tram);
                        eventoNombre = Eventos.RecibirTramite;
                        break;
                    case EnumTipoMovimiento.Anular_derivacion:
                        idEstado = EstadoAnteriorTramite(tram);
                        eventoNombre = Eventos.AnularDerivaciónTramite;
                        break;
                    case EnumTipoMovimiento.Despachar:
                        idEstado = (int)EnumEstadoTramite.Despachado;
                        eventoNombre = Eventos.DespacharTramite;
                        break;
                    case EnumTipoMovimiento.Rechazar:
                        idEstado = (int)EnumEstadoTramite.Rechazado;
                        eventoNombre = Eventos.RechazarTramite;
                        break;
                    case EnumTipoMovimiento.Reingresar:
                        idEstado = EstadoAnteriorTramite(tram);
                        eventoNombre = Eventos.ReingresarTramite;
                        break;
                    case EnumTipoMovimiento.Anular:
                        idEstado = (int)EnumEstadoTramite.Anulado;
                        eventoNombre = Eventos.AnularTramite;
                        break;
                    case EnumTipoMovimiento.Finalizar:
                        idEstado = (int)EnumEstadoTramite.Finalizado;
                        eventoNombre = Eventos.FinalizarTramite;
                        funcionValidable = FuncionValidable.FinalizarTramite;
                        break;
                    case EnumTipoMovimiento.Archivar:
                        idEstado = (int)EnumEstadoTramite.Archivado;
                        eventoNombre = Eventos.ArchivarTramtie;
                        break;
                    case EnumTipoMovimiento.Desarchivar:
                        idEstado = (int)EnumEstadoTramite.Finalizado;
                        eventoNombre = Eventos.DesarchivarTramite;
                        break;
                    case EnumTipoMovimiento.Recibir_Presentado:
                        idEstado = EstadoAnteriorTramite(tram);
                        eventoNombre = Eventos.RecibirPresentadoTramite;
                        break;
                    case EnumTipoMovimiento.Anular_Carga_Libro:
                        idEstado = (int)EnumEstadoTramite.Provisorio;
                        eventoNombre = Eventos.AnularCapaLibroTramite;
                        break;
                    case EnumTipoMovimiento.Confirmar:
                        idEstado = (int)EnumEstadoTramite.Iniciado;
                        eventoNombre = Eventos.ConfirmarTramite;
                        funcionValidable = FuncionValidable.ConfirmarTramite;
                        break;
                    case EnumTipoMovimiento.Entregar:
                        idEstado = (int)EnumEstadoTramite.Entregado;
                        eventoNombre = Eventos.EntregarTramite;
                        break;
                    case EnumTipoMovimiento.Procesar:
                        idEstado = (int)EnumEstadoTramite.Procesado;
                        eventoNombre = Eventos.ProcesarTramite;
                        funcionValidable = FuncionValidable.ProcesarTramite;
                        break;
                    case EnumTipoMovimiento.Presentar:
                        idEstado = (int)EnumEstadoTramite.Presentado;
                        eventoNombre = Eventos.PresentarTramite;
                        funcionValidable = FuncionValidable.PresentarTramite;
                        break;
                    default:
                        break;
                }
                if (idEstado > 0)
                {
                    int idMovimientoDerivacion = (int)EnumEstadoTramite.Derivado;

                    var query = from t in this.GetContexto().TramitesMesaEntrada
                                join mv in this.GetContexto().MovimientosTramites on t.IdTramite equals mv.IdTramite
                                where t.IdTramite == tram
                                group mv by t into grp
                                select new
                                {
                                    tramite = grp.Key,
                                    anteUltimoEstado = (int?)grp.OrderByDescending(m => m.FechaAlta).Skip(1).Take(1).FirstOrDefault().IdEstado,
                                    sectorAnterior = (int?)grp.Where(m => m.IdSectorOrigen != idSectorDestino).OrderByDescending(m => m.FechaAlta).Take(1).FirstOrDefault().IdSectorOrigen,
                                    sectorOrigenDerivacion = (int?)grp.Where(m => m.IdEstado == idMovimientoDerivacion).OrderByDescending(m => m.FechaAlta).Take(1).FirstOrDefault().IdSectorOrigen
                                };

                    var auditorias = new List<Auditoria>();
                    var validaciones = new List<string>();
                    var hayErroresBloqueantes = false;
                    ResultadoValidacion resultadoValidacionCompleta = ResultadoValidacion.Ok;

                    foreach (var grupo in query.ToList())
                    {
                        if (!TryEjecucion(grupo.tramite, usuario, grupo.anteUltimoEstado, grupo.sectorAnterior, grupo.sectorOrigenDerivacion ?? (int)usuario.IdSector, idEstado,
                                            tipoMovimiento, idSectorDestino, observ, eventoNombre, funcionValidable,
                                            accionParameters._Ip, accionParameters._Machine_Name, hayErroresBloqueantes,
                                            out ResultadoValidacion resultadoValidacion, out List<string> errores, out Auditoria auditoria))
                        {
                            hayErroresBloqueantes = true;
                            resultadoValidacionCompleta |= resultadoValidacion;
                        }
                        if (auditoria != null)
                        {
                            auditorias.Add(auditoria);
                        }
                        validaciones.AddRange(errores);

                        if (tipoMovimiento == EnumTipoMovimiento.Reasignar)
                        {
                            grupo.tramite.UsuarioModif = Convert.ToInt64(accionParameters.usuario);
                        }
                    }
                    if (resultadoValidacionCompleta != ResultadoValidacion.Bloqueo && resultadoValidacionCompleta != ResultadoValidacion.Error)
                    {
                        this.GetContexto().SaveChanges(auditorias);
                    }
                    if (resultadoValidacionCompleta != ResultadoValidacion.Ok)
                    {
                        throw new ValidacionException(resultadoValidacionCompleta, validaciones);
                    }
                }
            }
            return true;
        }

        public bool GetNotaEditable(long idNota, long idUsuario)
        {
            var nota = this.GetContexto().TramitesDocumentos.Single(x => x.id_documento == idNota);
            return idUsuario == nota?.UsuarioAlta || new UsuariosRepository(this.GetContexto()).EsUsuarioAdmin(idUsuario);
        }

        public string GetPersonaByIdUt(long idUnidadTributaria)
        {
            var dominios = (from ut in this.GetContexto().UnidadesTributarias
                            join dom in this.GetContexto().Dominios on ut.UnidadTributariaId equals dom.UnidadTributariaID
                            where ut.UnidadTributariaId == idUnidadTributaria && dom.FechaBaja == null
                            select dom)
                           .Include("Titulares")
                           .Include("Titulares.Persona")
                           .ToList();

            string[] personas = dominios.Select(t => t.Titulares.OrderByDescending(x => x.PorcientoCopropiedad).Select(z => z.Persona.NombreCompleto).FirstOrDefault()).ToArray();

            string persona = string.Join(", ", personas);


            return persona;

        }

        public int TramiteSave(bool esConfirmacion, bool esPresentacion, bool esReingresar, METramiteParameters tramiteParameters)
        {
            DateTime fechaActual = DateTime.Now;
            var tramiteActual = default(METramite);
            var usuario = this.GetContexto().Usuarios.Find(tramiteParameters.Tramite._Id_Usuario);
            var tramite = tramiteParameters.Tramite;

            Dictionary<string, List<Documento>> procesarDocumentos()
            {
                var archivos = new Dictionary<string, List<Documento>>();
                void evaluarArchivo(string operacion, METramiteDocumento tramiteDocumento)
                {
                    if (string.IsNullOrEmpty(tramiteDocumento.Documento.nombre_archivo))
                    {
                        return;
                    }
                    if (!archivos.TryGetValue(operacion, out List<Documento> valores))
                    {
                        valores = new List<Documento>();
                        archivos.Add(operacion, valores);
                    }

                    valores.Add(tramiteDocumento.Documento);
                }
                IEnumerable<METramiteDocumento> tramitesDocumentosNuevos = (tramiteParameters.TramitesDocumentos != null ? tramiteParameters.TramitesDocumentos.Where(d => d.id_documento == 0) : new METramiteDocumento[0]);
                IEnumerable<METramiteDocumento> tramitesDocumentosExistentes = new METramiteDocumento[0];
                IEnumerable<METramiteDocumento> tramitesDocumentosEliminados = new METramiteDocumento[0];
                if (tramiteActual.TramiteDocumentos != null)
                {
                    if (tramiteParameters.TramitesDocumentos != null)
                    {
                        tramitesDocumentosExistentes = tramiteActual.TramiteDocumentos.Where(d => tramiteParameters.TramitesDocumentos.Any(p => p.id_documento == d.id_documento)).ToArray();
                    }
                    tramitesDocumentosEliminados = tramiteActual.TramiteDocumentos.Except(tramitesDocumentosExistentes);
                }
                foreach (var tramitesDocumentosEliminado in tramitesDocumentosEliminados)
                {
                    tramitesDocumentosEliminado.FechaBaja = fechaActual;
                    tramitesDocumentosEliminado.UsuarioBaja = usuario.Id_Usuario;
                    tramitesDocumentosEliminado.FechaModif = fechaActual;
                    tramitesDocumentosEliminado.UsuarioModif = usuario.Id_Usuario;
                    tramitesDocumentosEliminado.Documento.fecha_baja_1 = fechaActual;
                    tramitesDocumentosEliminado.Documento.id_usu_baja = usuario.Id_Usuario;
                    tramitesDocumentosEliminado.Documento.id_usu_modif = usuario.Id_Usuario;
                    tramitesDocumentosEliminado.Documento.fecha_modif = fechaActual;
                    evaluarArchivo("B", tramitesDocumentosEliminado);
                }
                if (tramitesDocumentosNuevos.Count() > 0 && tramiteActual.TramiteDocumentos == null)
                {
                    tramiteActual.TramiteDocumentos = new List<METramiteDocumento>();
                }
                foreach (var tramitesDocumentosNuevo in tramitesDocumentosNuevos)
                {
                    METramiteDocumento obj;
                    obj = new METramiteDocumento()
                    {
                        FechaAlta = fechaActual,
                        UsuarioAlta = usuario.Id_Usuario,
                        FechaModif = fechaActual,
                        UsuarioModif = usuario.Id_Usuario,
                        FechaAprobacion = tramitesDocumentosNuevo.FechaAprobacion,
                        Documento = new Documento()
                        {
                            fecha = fechaActual,
                            id_tipo_documento = tramitesDocumentosNuevo.Documento.id_tipo_documento,
                            descripcion = tramitesDocumentosNuevo.Documento.descripcion,
                            observaciones = tramitesDocumentosNuevo.Documento.observaciones,
                            nombre_archivo = tramitesDocumentosNuevo.Documento.nombre_archivo,
                            extension_archivo = tramitesDocumentosNuevo.Documento.extension_archivo,
                            id_usu_alta = usuario.Id_Usuario,
                            fecha_alta_1 = fechaActual,
                            id_usu_modif = usuario.Id_Usuario,
                            fecha_modif = fechaActual
                        }
                    };

                    tramiteActual.TramiteDocumentos.Add(obj);
                    evaluarArchivo("E", obj);

                }
                foreach (var tramiteDocumentoExistente in tramitesDocumentosExistentes)
                {
                    var tramiteDocumentoUI = tramiteParameters.TramitesDocumentos.Single(td => td.id_documento == tramiteDocumentoExistente.id_documento);

                    tramiteDocumentoExistente.FechaModif = fechaActual;
                    tramiteDocumentoExistente.UsuarioModif = usuario.Id_Usuario;

                    var documento = tramiteDocumentoExistente.Documento;

                    tramiteDocumentoExistente.FechaAprobacion = tramiteDocumentoUI.FechaAprobacion;

                    if (string.Compare($"{documento.nombre_archivo}{documento.extension_archivo}", $"{tramiteDocumentoUI.Documento.nombre_archivo}{tramiteDocumentoUI.Documento.extension_archivo}", false) != 0)
                    {
                        evaluarArchivo("B", tramiteDocumentoExistente);
                    }

                    documento.fecha_modif = fechaActual;
                    documento.id_usu_modif = usuario.Id_Usuario;
                    documento.nombre_archivo = tramiteDocumentoUI.Documento.nombre_archivo;
                    documento.observaciones = tramiteDocumentoUI.Documento.observaciones;
                    documento.descripcion = tramiteDocumentoUI.Documento.descripcion;
                    documento.id_tipo_documento = tramiteDocumentoUI.Documento.id_tipo_documento;
                    documento.extension_archivo = tramiteDocumentoUI.Documento.extension_archivo;

                    evaluarArchivo("E", tramiteDocumentoExistente);

                }

                return archivos;
            }
            void procesarArchivos()
            {
                var archivos = procesarDocumentos();
                string getDocumentoFilename(Documento doc) => $"{doc.nombre_archivo}{doc.extension_archivo}";

                var paramUploadTempFolder = this.GetContexto().ParametrosGenerales.ToList().Where(x => x.Clave == "RUTA_DOCUMENTOS_TEMPORAL").FirstOrDefault();
                string sourcePath = Path.Combine(paramUploadTempFolder.Valor, tramite.IdTramite.ToString(), "temporales");

                var paramUploadFolder = this.GetContexto().ParametrosGenerales.ToList().Where(x => x.Clave == "RUTA_DOCUMENTOS").FirstOrDefault();
                string targetPath = Directory.CreateDirectory(Path.Combine(paramUploadFolder.Valor, tramiteActual.IdTramite.ToString(), "documentos")).FullName;
                if (archivos.TryGetValue("B", out List<Documento> elementos))
                {
                    foreach (var archivo in elementos)
                    {
                        string origen = Path.Combine(string.IsNullOrEmpty(archivo.ruta) ? targetPath : archivo.ruta, getDocumentoFilename(archivo));
                        if (File.Exists(origen))
                        {
                            File.Delete(origen);
                        }
                    }
                }
                if (archivos.TryGetValue("E", out elementos))
                {
                    foreach (var archivo in elementos)
                    {
                        string origen = Path.Combine(sourcePath, getDocumentoFilename(archivo));
                        if (File.Exists(origen))
                        {
                            archivo.ruta = Path.Combine(targetPath, Path.GetFileName(origen));
                            File.Move(origen, archivo.ruta);
                        }
                    }
                }
                if (tramite.IdTramite != tramiteActual.IdTramite)
                {
                    sourcePath = Directory.GetParent(sourcePath).FullName;
                }
                if (Directory.Exists(sourcePath))
                {
                    Directory.Delete(sourcePath, true);
                }
            }

            string tipoOperacion = TiposOperacion.Modificacion;

            string observ = "";

            bool nuevo = tramite.IdTramite <= 0;

            if (new[] { (int)EnumEstadoTramite.Procesado, (int)EnumEstadoTramite.Finalizado, (int)EnumEstadoTramite.Archivado }.Contains(tramite.IdEstado))
            {
                try
                {
                    tramiteActual = this.GetContexto().TramitesMesaEntrada
                                    .Include(x => x.TramiteDocumentos.Select(a => a.Documento))
                                    .Single(x => x.IdTramite == tramite.IdTramite);

                    procesarArchivos();
                    this.GetContexto().SaveChanges();

                    return tramiteActual.IdTramite;

                }
                catch (Exception ex)
                {
                    this.GetContexto().GetLogger().LogError("TramiteSave-EditarNotas", ex);
                    throw;
                }
            }

            using (Stream lockFile = adquirirLockSecuenciaTramite(nuevo))
            using (var dbTrans = this.GetContexto().Database.BeginTransaction())
            {
                try
                {
                    var idTipoMovimiento = (int)(nuevo ? EnumTipoMovimiento.Crear : EnumTipoMovimiento.Editar);
                    var idSectorDestino = usuario.IdSector.GetValueOrDefault();
                    var idUsuarioModif = usuario.Id_Usuario;
                    if (nuevo)
                    {
                        var paramSecuencia = this.GetContexto().ParametrosGenerales.Single(pg => pg.Clave == "NUMERO_SECUENCIA_TRAMITE");
                        int currVal = int.Parse(paramSecuencia.Valor);
                        tramiteActual = this.GetContexto().TramitesMesaEntrada.Add(new METramite
                        {
                            Numero = GetTramiteNumero(currVal),
                            IdEstado = (int)EnumEstadoTramite.Provisorio,
                            FechaAlta = fechaActual,
                            UsuarioAlta = usuario.Id_Usuario,
                            FechaInicio = fechaActual,
                            FechaIngreso = fechaActual,
                            Movimientos = new List<MEMovimiento>(),
                            TramiteDocumentos = new List<METramiteDocumento>(),
                            Desgloses = new List<MEDesglose>(),
                            TramiteEntradas = new List<METramiteEntrada>()
                        });
                        paramSecuencia.Valor = (currVal + 1).ToString();
                        tipoOperacion = TiposOperacion.Alta;
                    }
                    else
                    {
                        tramiteActual = this.GetContexto()
                                            .TramitesMesaEntrada
                                            .Include("Iniciador")
                                            .Include("Movimientos")
                                            .Include("TramiteRequisitos")
                                            .Include("Desgloses")
                                            .Include("TramiteDocumentos")
                                            .Include("TramiteDocumentos.Documento")
                                            .Single(t => t.IdTramite == tramite.IdTramite);

                        tramiteActual.IdEstado = tramite.IdEstado;
                        tramiteActual.TramiteEntradas = new List<METramiteEntrada>();

                        this.GetContexto().Entry(tramiteActual).Property(x => x.UsuarioAlta).IsModified = false;
                        this.GetContexto().Entry(tramiteActual).Property(x => x.FechaAlta).IsModified = false;

                        this.GetContexto().Entry(tramiteActual).Property(x => x.FechaInicio).IsModified = false;
                        this.GetContexto().Entry(tramiteActual).Property(x => x.FechaIngreso).IsModified = false;
                        this.GetContexto().Entry(tramiteActual).Property(x => x.FechaLibro).IsModified = false;
                        if (esReingresar)
                        {
                            short cantDias = this.GetContexto().PrioridadesTramites.Find(tramite.IdPrioridad).CantDias;
                            tramiteActual.FechaVenc = CalcularFechaVenc(fechaActual.Date, cantDias);
                            idTipoMovimiento = (int)Enumerators.EnumTipoMovimiento.Reingresar;
                            var ultDespacho = tramiteActual.Movimientos.Where(m => m.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar && m.IdMovimiento == tramiteActual.Movimientos.Where(p => p.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar).Max(f => f.IdMovimiento)).FirstOrDefault();
                            idSectorDestino = ultDespacho.IdSectorOrigen;
                            idUsuarioModif = ultDespacho.UsuarioAlta;
                            tramiteActual.IdEstado = EstadoAnteriorTramite(tramiteActual.IdTramite);
                        }
                        else
                        {
                            this.GetContexto().Entry(tramiteActual).Property(x => x.FechaVenc).IsModified = false;
                        }
                    }

                    tramiteActual.IdPrioridad = tramite.IdPrioridad;
                    tramiteActual.IdJurisdiccion = tramite.IdJurisdiccion;
                    tramiteActual.IdLocalidad = tramite.IdLocalidad;
                    tramiteActual.IdTipoTramite = tramite.IdTipoTramite;
                    tramiteActual.IdObjetoTramite = tramite.IdObjetoTramite;
                    tramiteActual.Motivo = tramite.Motivo;
                    tramiteActual.IdIniciador = tramite.IdIniciador;
                    tramiteActual.IdUnidadTributaria = tramite.IdUnidadTributaria;


                    tramiteActual.FechaModif = fechaActual;
                    tramiteActual.UsuarioModif = usuario.Id_Usuario;

                    tramiteActual.Movimientos.Add(new MEMovimiento
                    {
                        IdSectorDestino = idSectorDestino,
                        IdSectorOrigen = usuario.IdSector.GetValueOrDefault(),
                        IdTipoMovimiento = idTipoMovimiento,
                        IdEstado = tramiteActual.IdEstado,
                        Observacion = observ,
                        FechaAlta = tramiteActual.FechaModif,
                        UsuarioAlta = tramiteActual.UsuarioModif
                    });

                    #region Requisitos
                    IEnumerable<METramiteRequisito> requisitosEliminados = new METramiteRequisito[0];
                    IEnumerable<int> requisitosNuevos = tramiteParameters.TramitesRequisitos ?? new int[0];
                    if (tramiteActual.TramiteRequisitos != null)
                    {
                        IEnumerable<METramiteRequisito> requisitosNoModificados = new METramiteRequisito[0];
                        if (tramiteParameters.TramitesRequisitos != null)
                        {
                            requisitosNoModificados = tramiteActual.TramiteRequisitos.Where(tr => tramiteParameters.TramitesRequisitos.Contains(tr.IdObjetoRequisito));
                            requisitosNuevos = tramiteParameters.TramitesRequisitos.Where(p => !tramiteActual.TramiteRequisitos.Any(tr => tr.IdObjetoRequisito == p));
                        }
                        requisitosEliminados = tramiteActual.TramiteRequisitos.Except(requisitosNoModificados);
                    }
                    foreach (var reqNoCumplido in requisitosEliminados)
                    {
                        reqNoCumplido.FechaBaja = fechaActual;
                        reqNoCumplido.UsuarioBaja = tramiteActual.UsuarioModif;
                        reqNoCumplido.FechaModif = fechaActual;
                        reqNoCumplido.UsuarioModif = tramiteActual.UsuarioModif;
                    }
                    if (requisitosNuevos.Count() > 0 && tramiteActual.TramiteRequisitos == null)
                    {
                        tramiteActual.TramiteRequisitos = new List<METramiteRequisito>();
                    }
                    foreach (var reqNuevo in requisitosNuevos)
                    {
                        tramiteActual.TramiteRequisitos.Add(new METramiteRequisito()
                        {
                            IdObjetoRequisito = reqNuevo,
                            FechaAlta = fechaActual,
                            UsuarioAlta = tramiteActual.UsuarioModif,
                            FechaModif = fechaActual,
                            UsuarioModif = tramiteActual.UsuarioModif
                        });
                    }
                    #endregion

                    #region Desgloses
                    //Desgloses
                    IEnumerable<MEDesglose> desglosesNuevos = (tramiteParameters.Desgloses != null ? tramiteParameters.Desgloses.Where(d => d.IdDesglose == 0) : new MEDesglose[0]);
                    IEnumerable<MEDesglose> desglosesNoModificados = new MEDesglose[0];
                    IEnumerable<MEDesglose> desglosesEliminados = new MEDesglose[0];
                    if (tramiteActual.Desgloses != null)
                    {
                        if (tramiteParameters.Desgloses != null)
                        {
                            desglosesNoModificados = tramiteActual.Desgloses.Where(d => tramiteParameters.Desgloses.Any(p => p.IdDesglose == d.IdDesglose));
                        }
                        desglosesEliminados = tramiteActual.Desgloses.Except(desglosesNoModificados);
                    }
                    foreach (var desgloseEliminado in desglosesEliminados)
                    {
                        desgloseEliminado.FechaBaja = fechaActual;
                        desgloseEliminado.UsuarioBaja = tramiteActual.UsuarioModif;
                        desgloseEliminado.FechaModif = fechaActual;
                        desgloseEliminado.UsuarioModif = tramiteActual.UsuarioModif;
                    }
                    if (desglosesNuevos.Count() > 0 && tramiteActual.Desgloses == null)
                    {
                        tramiteActual.Desgloses = new List<MEDesglose>();
                    }
                    foreach (var desgloseNuevo in desglosesNuevos)
                    {
                        tramiteActual.Desgloses.Add(new MEDesglose()
                        {
                            FechaAlta = fechaActual,
                            UsuarioAlta = tramiteActual.UsuarioModif,
                            FechaModif = fechaActual,
                            UsuarioModif = tramiteActual.UsuarioModif,
                            FolioDesde = desgloseNuevo.FolioDesde,
                            FolioHasta = desgloseNuevo.FolioHasta,
                            FolioCant = desgloseNuevo.FolioCant,
                            IdDesgloseDestino = desgloseNuevo.IdDesgloseDestino,
                            Observaciones = desgloseNuevo.Observaciones
                        });
                    }
                    #endregion

                    procesarArchivos();

                    this.GetContexto().SaveChanges(new Auditoria(tramiteActual.UsuarioModif,
                                                                    tramite.IdTramite <= 0 ? Eventos.AltadeTramites : Eventos.EditarTramite,
                                                                    tramite.IdTramite <= 0 ? Mensajes.AltadeTramitesOK : Mensajes.ModificarTramitesOK,
                                                                    tramite._Machine_Name, tramite._Ip, "S",
                                                                    tramite.IdTramite <= 0 ? null : this.GetContexto().Entry(tramiteActual).OriginalValues.ToObject(),
                                                                    this.GetContexto().Entry(tramiteActual).Entity, "Trámite", 1, tipoOperacion));

                    #region Datos Especificos New
                    var entradasProcesadas = new List<long>();
                    // Lo muevo aca porque necesito asociar a las tablas temporales el id tramite
                    if (tramiteParameters?.DatosEspecificos != null)
                    {
                        var dictionary = new List<RelacionDto>(); //Diccionario accesorio para chequear si el objeto esta creado o no
                                                                  // Asigno a CrearTramiteEntradasTemporal, el proceso de carga temporal
                        CrearTramiteEntradasTemporal(dictionary, tramiteParameters.DatosEspecificos, tramiteActual, usuario, entradasProcesadas);
                        this.GetContexto().SaveChanges();
                    }
                    var entradasFinales = (tramiteActual.TramiteEntradas ?? new List<METramiteEntrada>()).Select(te => te.IdTramiteEntrada).ToArray();
                    var tramiteRelacion = this.GetContexto().TramitesEntradasRelacion;
                    var entradasBorradas = (from tramiteEntrada in this.GetContexto().TramitesEntradas
                                            join entrada in this.GetContexto().ObjetosEntrada on tramiteEntrada.IdObjetoEntrada equals entrada.IdObjetoEntrada
                                            where tramiteEntrada.IdTramite == tramiteActual.IdTramite && !entradasFinales.Contains(tramiteEntrada.IdTramiteEntrada)
                                            select new
                                            {
                                                tramiteEntrada.IdTramiteEntrada,
                                                tramiteEntrada.IdComponente,
                                                idObjeto = tramiteEntrada.IdObjeto.Value,
                                                entrada.IdEntrada,
                                                tienePadre = tramiteRelacion.Any(x => x.IdTramiteEntrada == tramiteEntrada.IdTramiteEntrada)
                                            }).ToArray();

                    if (entradasBorradas.Any())
                    {
                        string entradas = string.Join(",", entradasBorradas.Select(e => e.IdTramiteEntrada));
                        using (var qb = this.GetContexto().CreateSQLQueryBuilder())
                        {
                            qb.AddTable("me_tramite_entrada_relacion", null)
                                .AddFilter("id_tramite_entrada", entradas, Common.Enums.SQLOperators.In)
                                .AddFilter("id_tramite_entrada_padre", entradas, Common.Enums.SQLOperators.In, Common.Enums.SQLConnectors.Or)
                                .ExecuteDelete();
                        }

                        using (var qb = this.GetContexto().CreateSQLQueryBuilder())
                        {
                            qb.AddTable("me_tramite_entrada", null)
                                .AddFilter("id_tramite_entrada", entradas, Common.Enums.SQLOperators.In)
                                .AddFilter("id_tramite", tramiteActual.IdTramite, Common.Enums.SQLOperators.EqualsTo, Common.Enums.SQLConnectors.And)
                                .ExecuteDelete();
                        }
                        //Preguntar que se hace comprobante de pago cuando se borra la entrada, si se borra de la tabla me_comprobante_pago o no
                        entradasBorradas = entradasBorradas.Where(e => e.IdEntrada != int.Parse(Entradas.Persona) && !entradasProcesadas.Contains(e.IdTramiteEntrada)).ToArray();

                        foreach (var grupo in entradasBorradas.GroupBy(e => new { e.IdComponente, e.IdEntrada }))
                        {
                            if (grupo.Key.IdEntrada == Convert.ToInt32(Entradas.ComprobantePago))
                            {
                                var ids = grupo.Select(e => e.idObjeto).ToArray();
                                var query = from cp in this.GetContexto().ComprobantePago
                                            where ids.Contains(cp.IdComprobantePago)
                                            select cp;

                                foreach (var cp in query.ToArray())
                                {
                                    cp.FechaBaja = cp.FechaModif = tramiteActual.FechaModif;
                                    cp.IdUsuarioBaja = cp.IdUsuarioModif = tramiteActual.UsuarioModif;
                                }

                                this.GetContexto().SaveChanges();
                            }
                            else if (grupo.Key.IdEntrada == Convert.ToInt32(Entradas.DDJJ))
                            {
                                var ids = grupo.Select(e => e.idObjeto).ToArray();
                                var query = from ddjj in this.GetContexto().DeclaracionesJuradasTemporal
                                            where ids.Contains(ddjj.IdDeclaracionJurada)
                                            select ddjj;

                                LimpiarDDJJs(query);
                                GetContexto().SaveChanges();
                            }
                            else
                            {
                                string idsObjetos = string.Join(",", grupo.Select(e => e.idObjeto));
                                long idComponente = grupo.Key.IdComponente;
                                var componente = this.GetContexto().Componente.Include(c => c.Atributos).SingleOrDefault(c => c.ComponenteId == idComponente);
                                Atributo campoClave = null;
                                try
                                {
                                    campoClave = componente.Atributos.GetAtributoClave();

                                }
                                catch (ApplicationException ex)
                                {
                                    this.GetContexto().GetLogger().LogError("Componente (id: " + componente.ComponenteId + ") mal configurado.", ex);
                                    throw;
                                }
                                if (grupo.Key.IdEntrada == Convert.ToInt32(Entradas.Parcela))
                                {
                                    long idComponenteUt = long.Parse(this.GetContexto().ParametrosGenerales.SingleOrDefault(p => p.Clave == "ID_COMPONENTE_UNIDAD_TRIBUTARIA").Valor);
                                    string tablaUt = this.GetContexto().Componente.Find(idComponenteUt).TablaTemporal;

                                    foreach (var parcela in grupo)
                                    {
                                        LimpiarRelacionesParcela(tramiteActual, parcela.idObjeto, campoClave, new[] { componente.TablaTemporal, componente.TablaGrafica, tablaUt, "inm_nomenclatura" });
                                    }

                                }
                                if (grupo.Key.IdEntrada == Convert.ToInt32(Entradas.UnidadTributaria))
                                {
                                    long idComponenteParcela = long.Parse(this.GetContexto().ParametrosGenerales.SingleOrDefault(p => p.Clave == "ID_COMPONENTE_PARCELA").Valor);
                                    var campoClaveParcela = this.GetContexto().Atributo.Include(a => a.Componente).GetAtributoClaveByComponente(idComponenteParcela);
                                    foreach (var ut in grupo)
                                    {
                                        if (!ut.tienePadre)
                                        {
                                            var parcela = this.GetContexto().UnidadesTributariasTemporal.Find(ut.idObjeto, tramiteActual.IdTramite);
                                            LimpiarRelacionesParcela(tramiteActual, parcela.ParcelaID.Value, campoClaveParcela, new[] { campoClaveParcela.Componente.TablaTemporal, componente.TablaTemporal, campoClaveParcela.Componente.TablaGrafica });
                                        }
                                    }

                                }
                                else if (grupo.Key.IdEntrada == Convert.ToInt32(Entradas.Titulo))
                                {
                                    BorrarRegistrosTemporales(campoClave, "inm_dominio_titular", idsObjetos, Common.Enums.SQLOperators.In, tramite.IdTramite);
                                }
                                BorrarRegistrosTemporales(campoClave, componente.TablaTemporal ?? componente.Tabla, idsObjetos, Common.Enums.SQLOperators.In, tramiteActual.IdTramite);
                            }
                        }
                    }


                    #endregion

                    try
                    {
                        var obj = new ObjetoValidable()
                        {
                            IdTramite = tramiteActual.IdTramite,
                            TipoObjeto = TipoObjetoValidable.Tramite,
                            IdObjeto = tramiteActual.IdObjetoTramite
                        };
                        var valRepo = new ValidacionDBRepository(this.GetContexto());
                        List<string> erroresValidacion;
                        ResultadoValidacion resultado;
                        if (esConfirmacion)
                        {
                            obj.Grupo = GrupoValidable.ConfirmarTramite;
                            resultado = valRepo.ValidarFuncionGrupo(obj, out erroresValidacion);
                        }
                        else
                        {
                            obj.Funcion = FuncionValidable.GuardarTramite;
                            resultado = valRepo.Validar(obj, out erroresValidacion);
                        }
                        if (resultado == ResultadoValidacion.Advertencia || resultado == ResultadoValidacion.Ok)
                        {
                            if (int.TryParse(this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_EXTERNO").FirstOrDefault()?.Valor, out int idSectorProfesional)
                                 && idSectorProfesional == usuario.IdSector)
                            {
                                resultado = ResultadoValidacion.Ok;
                            }

                            if (esConfirmacion)
                            {
                                /*
                                 * Ernesto.......
                                 * llamo puntualmente a este metodo para que la confirmacion haga lo mismo tanto desde dentro del form de edicion 
                                 * como para el boton de acciones de la grilla principal.
                                 * 
                                 * está resuelto muy a lo cabeza, pero si me pongo a emprolijar todo voy a estar un par de días más y no quiero seguir 
                                 * perdiendo tiempo.
                                 * las validaciones se ejecutarán otra vez,si, ya se, pero si llegó a este punto, es porque ya las cumplió, 
                                 * o sea que no me calienta demasiado. Si alguien tiene problemas con ésto, que venga y que me diga.
                                 */
                                TryEjecucion(tramiteActual, usuario, null, null, null, (int)EnumEstadoTramite.Iniciado, EnumTipoMovimiento.Confirmar, usuario.IdSector.Value,
                                             string.Empty, Eventos.ConfirmarTramite, FuncionValidable.ConfirmarTramite, tramite._Ip, tramite._Machine_Name, false,
                                             out _, out _, out Auditoria auditoria);

                                this.GetContexto().SaveChanges(auditoria);
                            }
                            else if (esPresentacion && tramite.IdEstado != (int)Enumerators.EnumEstadoTramite.Presentado)
                            {

                                TryEjecucion(tramiteActual, usuario, null, null, null, (int)EnumEstadoTramite.Presentado, EnumTipoMovimiento.Presentar, usuario.IdSector.Value,
                                                string.Empty, Eventos.PresentarTramite, FuncionValidable.Ninguna, tramite._Ip, tramite._Machine_Name, false,
                                                out _, out _, out Auditoria auditoria);

                                this.GetContexto().SaveChanges(auditoria);
                            }
                            dbTrans.Commit();
                        }
                        else
                        {
                            dbTrans.Rollback();
                        }
                        if (resultado != ResultadoValidacion.Ok)
                        {
                            throw new ValidacionTramiteException(obj.IdTramite.Value, resultado, erroresValidacion);
                        }
                        return tramiteActual.IdTramite;
                    }
                    catch (ValidacionTramiteException)
                    {
                        throw;
                    }
                    catch (Exception)
                    {
                        dbTrans.Rollback();
                        throw;
                    }
                }
                finally
                {
                    lockFile.Close();
                }
            }
        }

        private bool esNodoRaiz(MEDatosEspecificos item)
        {
            //item.ParentGuids == null || !item.ParentGuids.Any() || (item.ParentGuids.FirstOrDefault() == Guid.Empty && item.ParentGuids.Count() == 1);

            //var hasDirtyParents = item.ParentGuids.Any(t => t != Guid.Empty); //Mapping issues :/
            //var hasInsertedParents = item.ParentIdTramiteEntradas.Any();

            //return !hasDirtyParents && !hasInsertedParents;

            return !item.ParentGuids.Any(t => t != Guid.Empty);
        }

        private string GetTramiteNumero(int numero)
        {
            //El formato del N° de Trámite es:
            //AAA-BBBB-CCCCCC/DD
            //Donde:
            //AAA: es “124” (es el código de organismo asignado a la DGC en la Provincia de Corrientes)
            //BBBB: es el día en formato “ddmm” completando con “0” (por ejemplo hoy, “0204”)
            //CCCCCC: es el número de trámite que se reinicia cada año(*) completando los dígitos con “0”
            //DD: son los dos últimos dígitos del año
            DateTime fecha = DateTime.Today;
            string aaa = "124";
            string bbbb = fecha.Day.ToString("00") + fecha.Month.ToString("00");
            string dd = fecha.Year.ToString().Substring(2);

            string cccccc = numero.ToString("000000");
            return $"{aaa}-{bbbb}-{cccccc}/{dd}"; ;
        }

        protected override DbSet<METramite> ObtenerDbSet()
        {
            return this.GetContexto().TramitesMesaEntrada;
        }

        protected override Expression<Func<METramite, object>> OrdenDefault()
        {
            return tramite => tramite.IdTramite;
        }

        private GrillaTramite mapParaGrilla(METramite tramite)
        {
            var ultimoMov = tramite.Movimientos.Single(m => m.IdMovimiento == tramite.Movimientos.Max(f => f.IdMovimiento));
            return new GrillaTramite()
            {
                IdTramite = tramite.IdTramite,
                Numero = tramite.Numero,
                Iniciador = tramite.Iniciador != null ? tramite.Iniciador.NombreCompleto : string.Empty,
                Tipo = tramite.Tipo.Descripcion,
                Objeto = tramite.Objeto.Descripcion,
                Estado = tramite.Estado.Descripcion,
                Prioridad = tramite.Prioridad.Descripcion,
                SectorOrigen = ultimoMov.SectorOrigen.Nombre,
                SectorDestino = ultimoMov.SectorDestino.Nombre,
                FechaUltAct = ultimoMov.FechaAlta.ToString("dd/MM/yyyy HH:mm"),
                FechaVenc = tramite.FechaVenc.HasValue ? tramite.FechaVenc.Value.ToString("dd/MM/yyyy HH:mm") : string.Empty
            };
        }

        public METramite GetTramiteById(int idTramite, bool includeEntradas)
        {
            try
            {
                var joins = new List<Expression<Func<METramite, dynamic>>>()
                {
                    x => x.Estado, x => x.Iniciador, x => x.Prioridad, x => x.Tipo, x => x.Objeto,
                    x => x.UnidadTributaria, x => x.Localidad, x => x.Jurisdiccion, x => x.Desgloses.Select(d => d.DesgloseDestino),
                    x => x.TramiteDocumentos.Select(m => m.Documento.Tipo), x => x.TramiteDocumentos.Select(m => m.Usuario_Alta),
                    x => x.TramiteRequisitos.Select(r => r.ObjetoRequisito.Requisito),
                    x => x.Movimientos.Select(m => m.TipoMovimiento), x => x.Movimientos.Select(m => m.Estado),
                    x => x.Movimientos.Select(m => m.SectorDestino), x => x.Movimientos.Select(m => m.Remito)
                };
                if (includeEntradas)
                {
                    joins.AddRange(new Expression<Func<METramite, dynamic>>[]
                    {
                        x => x.TramiteEntradas.Select(e => e.TramiteEntradaRelaciones),
                        x => x.TramiteEntradas.Select(e => e.ObjetoEntrada),
                    });
                }
                var tramite = GetBaseQuery(joins, new Expression<Func<METramite, bool>>[] { t => t.IdTramite == idTramite }).AsNoTracking().Single();

                DateTime.TryParse(GetContexto().ParametrosGenerales.SingleOrDefault(p => p.Clave == "FECHA_PROCESO_MIGRACION")?.Valor, out DateTime fechaMigracion);

                foreach (var movimiento in tramite.Movimientos)
                {
                    Expression<Func<Usuarios, bool>> filtroByIdUsuario = u => u.Id_Usuario == movimiento.UsuarioAlta;
                    if (movimiento.FechaAlta < fechaMigracion)
                    {
                        filtroByIdUsuario = u => u.IdISICAT == movimiento.UsuarioAlta;
                    }
                    movimiento.Usuario_Alta = GetContexto().Usuarios.Single(filtroByIdUsuario);
                }

                return tramite;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError($"GetTramiteById/{idTramite}", ex);
                throw;
            }
        }

        public List<METramiteEntrada> GetEntradasByTramite(int idTramite)
        {
            var joins = new Expression<Func<METramite, dynamic>>[]
            {
                x => x.TramiteEntradas.Select(e => e.ObjetoEntrada),
                x => x.TramiteEntradas.Select(e => e.TramiteEntradaRelaciones),
                x => x.TramiteEntradas.Select(e => e.TramiteEntradaRelaciones.Select(ter=>ter.TramiteEntradaPadre.ObjetoEntrada)),
            };
            var tramite = GetBaseQuery(joins, new Expression<Func<METramite, bool>>[] { t => t.IdTramite == idTramite }).AsNoTracking().Single();

            return tramite.TramiteEntradas.ToList();
        }

        public List<METramiteDocumento> GetAllTramiteDocumentoByTramite(int idTramite)
        {
            try
            {
                return this.GetContexto().TramitesDocumentos.Include("Documento").Where(td => td.IdTramite == idTramite).ToList();
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError($"GetTramiteGetAllTramiteDocumentoByTramiteById/{idTramite}", ex);
                throw;
            }
        }

        //public int TramiteConfirmar(long idUsuario, METramite tramite)
        //{
        //    string tipoOperacion = TiposOperacion.Modificacion;
        //    DateTime fechaActual = DateTime.Now;

        //    var usuario = this.GetContexto().Usuarios.Find(idUsuario);

        //    METramite tramiteActual = this.GetContexto().TramitesMesaEntrada.Include("Movimientos").Single(t => t.IdTramite == tramite.IdTramite);

        //    this.GetContexto().Entry(tramiteActual).Property(x => x.UsuarioAlta).IsModified = false;
        //    this.GetContexto().Entry(tramiteActual).Property(x => x.FechaAlta).IsModified = false;

        //    tramiteActual.IdEstado = (int)EnumEstadoTramite.Iniciado;
        //    tramiteActual.FechaLibro = fechaActual;

        //    //TODO ver el tema de los feriados en CalcularFechaVenc()
        //    short cantDias = this.GetContexto().PrioridadesTramites.Find(tramiteActual.IdPrioridad).CantDias;
        //    tramiteActual.FechaVenc = CalcularFechaVenc(fechaActual.Date, cantDias);

        //    tramiteActual.FechaModif = fechaActual;
        //    tramiteActual.UsuarioModif = idUsuario;

        //    tramiteActual.Movimientos.Add(new MEMovimiento
        //    {
        //        IdSectorDestino = (int)usuario.IdSector,
        //        IdSectorOrigen = (int)usuario.IdSector,
        //        IdTipoMovimiento = (int)EnumTipoMovimiento.Confirmar,
        //        IdEstado = tramiteActual.IdEstado,
        //        Observacion = string.Empty,
        //        FechaAlta = fechaActual,
        //        UsuarioAlta = idUsuario
        //    });

        //    this.GetContexto().SaveChanges(new Auditoria(idUsuario, Eventos.ConfirmarTramite, Mensajes.ConfirmarTramiteOK, tramite._Machine_Name,
        //                                                         tramite._Ip, "S",
        //                                                         this.GetContexto().Entry(tramiteActual).OriginalValues.ToObject(),
        //                                                         this.GetContexto().Entry(tramiteActual).Entity,
        //                                                         "Trámite", 1, tipoOperacion));

        //    return 1;
        //}

        public bool DesgloseSave(long idUsuario, MEDesglose desglose)
        {
            MEDesglose desgloseActual = null;

            string tipoOperacion = TiposOperacion.Modificacion;
            var fechaActual = DateTime.Now;

            var usuario = this.GetContexto().Usuarios.Find(idUsuario);

            if (desglose.IdDesglose == 0)
            {
                desgloseActual = this.GetContexto().Desgloses.Add(new MEDesglose
                {
                    FechaAlta = fechaActual,
                    UsuarioAlta = idUsuario
                });

                tipoOperacion = TiposOperacion.Alta;
            }
            else
            {
                desgloseActual = this.GetContexto().Desgloses.Single(d => d.IdDesglose == desglose.IdDesglose);

                this.GetContexto().Entry(desgloseActual).Property(x => x.UsuarioAlta).IsModified = false;
                this.GetContexto().Entry(desgloseActual).Property(x => x.FechaAlta).IsModified = false;
            }

            desgloseActual.IdTramite = desglose.IdTramite;
            desgloseActual.IdDesgloseDestino = desglose.IdDesgloseDestino;

            desgloseActual.FechaModif = fechaActual;
            desgloseActual.UsuarioModif = idUsuario;

            this.GetContexto().SaveChanges(new Auditoria(idUsuario, Eventos.EditarTramite, Mensajes.ModificarTramitesOK, desglose._Machine_Name,
                                                                 desglose._Ip, "S",
                                                                 (desglose.IdDesglose == 0 ? null : this.GetContexto().Entry(desgloseActual).OriginalValues.ToObject()),
                                                                 this.GetContexto().Entry(desglose).Entity,
                                                                 "Desglose", 1, tipoOperacion));

            return true;
        }

        // Ernesto.......
        // metodo refactoreado pero MUY a lo cabeza.
        // si en algun momento se da, habría que hacerlo bien
        private bool TryEjecucion(METramite tramite, Usuarios usuario, int? anteUltimoEstado, int? sectorAnterior,
            int? sectorOrigenDerivacion, int idEstado, EnumTipoMovimiento tipoMovimiento, int idSectorDestino,
            string observacion, string evento, FuncionValidable funcionValidable, string ip, string machineName,
            bool soloValidar, out ResultadoValidacion resultadoValidacion, out List<string> errores, out Auditoria auditoria)
        {
            auditoria = null;
            DateTime fechaActual = DateTime.Now;
            resultadoValidacion = ResultadoValidacion.Ok;
            var idUsuarioModif = usuario.Id_Usuario;
            bool? procesadoOk = null;
            try
            {
                resultadoValidacion = new ValidacionDBRepository(this.GetContexto()).Validar(new ObjetoValidable()
                {
                    IdTramite = tramite.IdTramite,
                    TipoObjeto = TipoObjetoValidable.Tramite,
                    Funcion = funcionValidable,
                    IdObjeto = tramite.IdObjetoTramite
                }, out errores);
                if (soloValidar || resultadoValidacion == ResultadoValidacion.Bloqueo || resultadoValidacion == ResultadoValidacion.Error)
                { //si hay errores de validacion (error o bloqueo) previos o actuales, solo evalúo las validaciones. ignoro lo demas
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }

            short cantDias = this.GetContexto().PrioridadesTramites.Find(tramite.IdPrioridad).CantDias;
            bool esProfesional = int.TryParse(this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_EXTERNO").FirstOrDefault()?.Valor, out int idSectorProfesional) &&
                                    usuario.IdSector == idSectorProfesional;

            if (tipoMovimiento == EnumTipoMovimiento.Derivar && new[] { EnumEstadoTramite.Finalizado, EnumEstadoTramite.Anulado }.Contains((EnumEstadoTramite)tramite.IdEstado))
            {
                idEstado = tramite.IdEstado;
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Desarchivar)
            {
                idEstado = anteUltimoEstado.Value;
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Anular_Carga_Libro)
            {
                tramite.FechaVenc = fechaActual;
                tramite.FechaLibro = null;
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Confirmar)
            {
                tramite.FechaLibro = fechaActual;

                //TODO ver el tema de los feriados en CalcularFechaVenc()
                tramite.FechaVenc = CalcularFechaVenc(fechaActual.Date, cantDias);
            }
            else if (sectorAnterior.HasValue && tipoMovimiento == EnumTipoMovimiento.Rechazar)
            {
                idSectorDestino = sectorAnterior.Value;
            }
            else if (sectorOrigenDerivacion.HasValue && tipoMovimiento == EnumTipoMovimiento.Anular_derivacion)
            {
                idSectorDestino = sectorOrigenDerivacion.Value;
                observacion = null;
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Procesar)
            {
                try
                {
                    Procesar(tramite);
                    procesadoOk = true;
                }
                catch (ValidacionException ex)
                {
                    resultadoValidacion = ex.ErrorValidacion;
                    errores = ex.Errores.ToList();
                    procesadoOk = false;
                }
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Reingresar)
            {
                tramite.FechaVenc = CalcularFechaVenc(fechaActual.Date, cantDias);
                if (esProfesional)
                {
                    var tramiteMovimientos = this.GetContexto().MovimientosTramites.Where(m => m.IdTramite == tramite.IdTramite).ToList();
                    var ultDespacho = tramiteMovimientos.Where(m => m.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar && m.IdMovimiento == tramiteMovimientos.Where(p => p.IdTipoMovimiento == (int)Enumerators.EnumTipoMovimiento.Despachar).Max(f => f.IdMovimiento)).FirstOrDefault();
                    idSectorDestino = ultDespacho.IdSectorOrigen;
                    idUsuarioModif = ultDespacho.UsuarioAlta;
                    var idEstadoAntDespacho = tramiteMovimientos.Where(m => m.IdMovimiento < ultDespacho.IdMovimiento).OrderByDescending(m => m.IdMovimiento).Take(1).FirstOrDefault()?.IdEstado;
                    if (idEstadoAntDespacho.HasValue)
                    {
                        idEstado = idEstadoAntDespacho.Value;
                    }
                }
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Presentar)
            {
                if (esProfesional)
                {
                    if (tramite.IdTipoTramite == (int)EnumTipoTramite.Mensura)
                    {
                        var sIdSectorMensura = this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_ME_MENSURA").FirstOrDefault()?.Valor;
                        idSectorDestino = (sIdSectorMensura != null ? Convert.ToInt32(sIdSectorMensura) : 0);
                    }
                    else
                    {
                        var sIdSectorGeneral = this.GetContexto().ParametrosGenerales.Where(p => p.Clave == "ID_SECTOR_ME_GENERAL").FirstOrDefault()?.Valor;
                        idSectorDestino = (sIdSectorGeneral != null ? Convert.ToInt32(sIdSectorGeneral) : 0);
                    }
                }
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Finalizar)
            {
                using (var builder = this.GetContexto().CreateSQLQueryBuilder())
                {
                    builder.AddNoTable()
                           .AddFields(new Atributo { Funcion = $"pkg_tramites.impactar_tramite({tramite.IdTramite},{usuario.Id_Usuario})", Campo = "finalizarjson" })
                           .ExecuteQuery((IDataReader reader) => true);
                }
                try
                {
                    new ParcelaRepository(this.GetContexto()).RefreshVistaMaterializadaParcela();
                }
                catch (Exception ex)
                {
                    this.GetContexto().GetLogger().LogError("Finalizar Trámite - RefreshVistaMaterializadaParcela", ex);
                }
            }
            else if (tipoMovimiento == EnumTipoMovimiento.Anular)
            {
                using (var builder = GeoSITMContext.CreateContext().CreateSQLQueryBuilder())
                {
                    builder.AddNoTable()
                           .AddFields(new Atributo { Funcion = $"Pkg_Tramites.Borrar_Tramite({tramite.IdTramite})", Campo = "anularjson" })
                           .ExecuteQuery((IDataReader reader) => true);
                }
            }

            tramite.IdEstado = idEstado;
            tramite.FechaModif = fechaActual;
            tramite.UsuarioModif = idUsuarioModif;

            this.GetContexto().MovimientosTramites.Add(new MEMovimiento
            {
                Tramite = tramite,
                IdSectorDestino = idSectorDestino,
                IdSectorOrigen = (int)usuario.IdSector,
                IdTipoMovimiento = (int)tipoMovimiento,
                IdEstado = idEstado,
                Observacion = observacion,
                FechaAlta = fechaActual,
                UsuarioAlta = usuario.Id_Usuario
            });

            auditoria = new Auditoria(usuario.Id_Usuario, evento, Mensajes.ModificarTramitesOK,
                                        machineName, ip, "S", this.GetContexto().Entry(tramite).OriginalValues.ToObject(),
                                        this.GetContexto().Entry(tramite).Entity, "Trámite", 1, TiposOperacion.Modificacion);

            return procesadoOk.GetValueOrDefault(true); //devuelve false sólo cuando hubo un error en el procesamiento del tramite
        }

        private DateTime CalcularFechaVenc(DateTime fechaDesde, short cantDias)
        {
            return AddWorkDays(fechaDesde, cantDias);
        }
        private DateTime AddWorkDays(DateTime date, short workingDays)
        {
            short direction = (short)(workingDays < 0 ? -1 : 1);
            DateTime newDate = date;
            while (workingDays != 0)
            {
                newDate = newDate.AddDays(direction);
                if (newDate.DayOfWeek != DayOfWeek.Saturday &&
                    newDate.DayOfWeek != DayOfWeek.Sunday &&
                    !IsFeriado(newDate))
                {
                    workingDays -= direction;
                }
            }
            return newDate;
        }
        private bool IsFeriado(DateTime fecha)
        {
            return this.GetContexto().Feriados.Any(f => f.Fecha == fecha && f.Fecha_Baja == null);
        }

        private Stream adquirirLockSecuencia(bool exclusivo, string lockname)
        {
            if (exclusivo)
            {
                DateTime inicio = DateTime.Now;
                lockname = $"{lockname}.bin";
                while (DateTime.Now.Subtract(TimeSpan.FromMinutes(5)) <= inicio)
                {
                    try
                    {
                        return File.Create(Path.Combine(Path.GetTempPath(), lockname), 1, FileOptions.WriteThrough | FileOptions.DeleteOnClose);
                    }
                    catch
                    {
                        Thread.Sleep(100);
                    }
                }
                throw new TimeoutException($"No se pudo adquirir el lock '{lockname}' para guardar el registro.");
            }
            else
            {
                return new MemoryStream();
            }
        }
        private Stream adquirirLockSecuenciaTramite(bool exclusivo) => adquirirLockSecuencia(exclusivo, $"locktramite");
        private Stream adquirirLockSecuenciaRemito(bool exclusivo, int idSector) => adquirirLockSecuencia(exclusivo, $"lockremito{idSector}");
        private Stream adquirirLockSecuenciaPartida(bool exclusivo, long idJurisdiccion, long idTipoParcela) => adquirirLockSecuencia(exclusivo, $"lockpartida{idJurisdiccion}{idTipoParcela}");
        private Stream adquirirLockSecuenciaMensura(bool exclusivo, long idDepartamento) => adquirirLockSecuencia(exclusivo, $"lockmensura{idDepartamento}");

        public List<MEMovimiento> GetMovimientosRemito(MERemitoParameters remitoParameters)
        {
            try
            {
                var tipos = new[] {
                    (int)EnumTipoMovimiento.Derivar,
                    (int)EnumTipoMovimiento.Rechazar,
                    (int)EnumTipoMovimiento.Despachar,
                    (int)EnumTipoMovimiento.Reingresar
                };
                return this.GetContexto().MovimientosTramites
                                            .Include("Estado")
                                            .Include("Tramite")
                                            .Include("Tramite.Iniciador")
                                            .Where(m => m.IdRemito == null
                                                    && m.FechaAlta >= remitoParameters.FechaDesde
                                                    && m.FechaAlta <= remitoParameters.FechaHasta
                                                    && m.IdSectorOrigen == remitoParameters.IdSectorOrigen
                                                    && m.IdSectorDestino == remitoParameters.IdSectorDestino
                                                    && tipos.Any(tipo => tipo == m.IdTipoMovimiento))
                                            .ToList();
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("GetMovimientosRemito", ex);
                throw;
            }
        }

        public List<MEMovimiento> GetMovimientosByIds(int[] ids)
        {
            try
            {
                return this.GetContexto().MovimientosTramites
                                            .Include("Estado")
                                            .Include("Tramite")
                                            .Include("Tramite.Iniciador")
                                            .Where(m => ids.Contains(m.IdMovimiento)).ToList();
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("GetMovimientosRemito", ex);
                throw;
            }
        }

        public int RemitoSave(MERemitoParameters remitoParameters)
        {
            using (Stream lockFile = adquirirLockSecuenciaRemito(true, remitoParameters.IdSectorOrigen))
            {
                var sector = this.GetContexto().Sectores.Single(s => s.IdSector == remitoParameters.IdSectorOrigen);

                var movimientos = new List<MEMovimiento>();
                foreach (var idmov in remitoParameters.MovimientosId)
                {
                    var mov = new MEMovimiento { IdMovimiento = idmov };
                    this.GetContexto().Entry(mov).State = EntityState.Unchanged;
                    movimientos.Add(mov);
                }
                MERemito remito = this.GetContexto().Remitos.Add(new MERemito
                {
                    Numero = GetRemitoNumero(sector),
                    IdSectorOrigen = remitoParameters.IdSectorOrigen,
                    IdSectorDestino = remitoParameters.IdSectorDestino,
                    FechaAlta = DateTime.Now,
                    UsuarioAlta = remitoParameters._Id_Usuario,
                    Movimientos = movimientos
                });
                sector.UltRemito += 1;

                this.GetContexto().SaveChanges(new Auditoria(remitoParameters._Id_Usuario,
                                                             Eventos.AgregarRemito,
                                                             Mensajes.AgregarRemitoOK,
                                                             remitoParameters._Machine_Name,
                                                             remitoParameters._Ip, "S", null,
                                                             this.GetContexto().Entry(remito).Entity,
                                                             "Remito", 1, TiposOperacion.Alta));
                lockFile.Close();

                return remito.IdRemito;
            }
        }

        private string GetRemitoNumero(Sector sector)
        {
            string ss = sector.IdSector.ToString("000");
            string rr = sector.UltRemito.ToString("000000");
            //string yy = fecha.Year.ToString().Substring(2);
            //$"N{ss}-{rr}-{yy}";
            return $"{ss}-{rr}";
        }

        public MERemito GetRemitoById(int idRemito)
        {
            try
            {
                return this.GetContexto().Remitos
                                        .Include("SectorOrigen")
                                        .Include("SectorDestino")
                                        .Include("Movimientos")
                                        .Include("Movimientos.TipoMovimiento")
                                        .Include("Movimientos.Estado")
                                        .Include("Movimientos.Tramite")
                                        .Include("Movimientos.Tramite.Iniciador")
                                        .Single(r => r.IdRemito == idRemito);
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError($"GetRemitoById/{idRemito}", ex);
                throw;
            }
        }

        public MERemito GetRemitoByNumero(string numero)
        {
            try
            {
                return this.GetContexto().Remitos
                                        .Include("SectorOrigen")
                                        .Include("SectorDestino")
                                        .Include("Movimientos")
                                        .Include("Movimientos.TipoMovimiento")
                                        .Include("Movimientos.Estado")
                                        .Include("Movimientos.Tramite")
                                        .Include("Movimientos.Tramite.Iniciador")
                                        .SingleOrDefault(r => r.Numero == numero);
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError($"GetRemitoByNumero/{numero}", ex);
                throw;
            }
        }

        public List<METramite> RecuperarTramitesByFiltro(Dictionary<string, string> valores)
        {
            var joins = new Expression<Func<METramite, dynamic>>[] { x => x.Iniciador, x => x.Prioridad, x => x.Tipo, x => x.Objeto, x => x.Estado, x => x.Movimientos.Select(m => m.SectorOrigen), x => x.Iniciador };
            var filtros = new List<Expression<Func<METramite, bool>>>();
            var sorts = new List<SortClause<METramite>>();

            foreach (var valor in valores)
            {
                if (!string.IsNullOrEmpty(valor.Value))
                {
                    switch (valor.Key)
                    {
                        case "fechaIngresoDesde":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaIngreso) >= val);
                            }
                            break;
                        case "fechaIngresoHasta":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaIngreso) <= val);
                            }
                            break;
                        case "fechaLibroDesde":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaLibro) >= val);
                            }
                            break;
                        case "fechaLibroHasta":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaLibro) <= val);
                            }
                            break;
                        case "fechaVencDesde":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaVenc) >= val);
                            }
                            break;
                        case "fechaVencHasta":
                            {
                                DateTime val = DateTime.Parse(valor.Value).Date;
                                filtros.Add(tramite => DbFunctions.TruncateTime(tramite.FechaVenc) <= val);
                            }
                            break;
                        case "idJurisdiccion":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdJurisdiccion == val);
                                }
                            }
                            break;
                        case "idLocalidad":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdLocalidad == val);
                                }
                            }
                            break;
                        case "idPrioridad":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdPrioridad == val);
                                }
                            }
                            break;
                        case "idTipoTramite":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdTipoTramite == val);
                                }
                            }
                            break;
                        case "idObjetoTramite":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdObjetoTramite == val);
                                }
                            }
                            break;
                        case "idEstado":
                            {
                                if (valor.Value != null)
                                {
                                    int val = int.Parse(valor.Value);
                                    filtros.Add(tramite => tramite.IdEstado == val);
                                }
                            }
                            break;
                        case "iniciadorId":
                            {
                                int val = int.Parse(valor.Value);
                                filtros.Add(tramite => tramite.IdIniciador == val);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.FechaInicio, ASC = true });
            return GetBaseQuery(joins, filtros, sorts).ToList();
        }

        public List<METramite> RecuperarTramitesPendientesConfirmar()
        {
            var joins = new Expression<Func<METramite, dynamic>>[] { x => x.Iniciador, x => x.Prioridad, x => x.Tipo, x => x.Objeto, x => x.Estado, x => x.Movimientos.Select(m => m.SectorOrigen), x => x.Iniciador };
            var filtros = new List<Expression<Func<METramite, bool>>>();
            var sorts = new List<SortClause<METramite>>();

            filtros.Add(tramite => tramite.FechaLibro == null);
            filtros.Add(tramite => tramite.IdEstado == (int)EnumEstadoTramite.Provisorio);

            sorts.Add(new SortClause<METramite>() { Expresion = tramite => tramite.FechaInicio, ASC = true });

            return GetBaseQuery(joins, filtros, sorts).ToList();
        }

        public bool AbrirLibroDiario(MELibroDiarioParameters libroDiarioParameters)
        {
            string tipoOperacion = TiposOperacion.Modificacion;
            //var fechaActual = DateTime.Now;

            var usuario = this.GetContexto().Usuarios.Find(libroDiarioParameters._Id_Usuario);

            var parametroActual = this.GetContexto().ParametrosGenerales.First(p => p.Clave == "LIBRO_DIARIO_ABIERTO");
            parametroActual.Valor = "1";

            var parametroActualFecha = this.GetContexto().ParametrosGenerales.First(p => p.Clave == "LIBRO_DIARIO_FECHA");
            parametroActualFecha.Valor = libroDiarioParameters.FechaLibro.ToShortDateString();

            this.GetContexto().SaveChanges(new Auditoria(usuario.Id_Usuario, Eventos.AbrirLibroDiario, Mensajes.AbrirLibroDiarioOK, libroDiarioParameters._Machine_Name,
                                                                 libroDiarioParameters._Ip, "S",
                                                                 this.GetContexto().Entry(parametroActual).OriginalValues.ToObject(),
                                                                 this.GetContexto().Entry(parametroActual).Entity,
                                                                 "ParametrosGenerales", 1, tipoOperacion));

            return true;
        }

        public bool CerrarLibroDiario(MELibroDiarioParameters libroDiarioParameters)
        {
            string tipoOperacion = TiposOperacion.Modificacion;
            //var fechaActual = DateTime.Now;

            var usuario = this.GetContexto().Usuarios.Find(libroDiarioParameters._Id_Usuario);

            var parametroActual = this.GetContexto().ParametrosGenerales.First(p => p.Clave == "LIBRO_DIARIO_ABIERTO");
            parametroActual.Valor = "0";

            var parametroActualFecha = this.GetContexto().ParametrosGenerales.First(p => p.Clave == "LIBRO_DIARIO_FECHA");
            parametroActualFecha.Valor = libroDiarioParameters.FechaLibro.ToShortDateString();

            this.GetContexto().SaveChanges(new Auditoria(usuario.Id_Usuario, Eventos.CerrarLibroDiario, Mensajes.CerrarLibroDiarioOK, libroDiarioParameters._Machine_Name,
                                                                 libroDiarioParameters._Ip, "S",
                                                                 this.GetContexto().Entry(parametroActual).OriginalValues.ToObject(),
                                                                 this.GetContexto().Entry(parametroActual).Entity,
                                                                 "ParametrosGenerales", 1, tipoOperacion));
            return true;
        }

        public Dictionary<long, string> ObtenerExpedientesDDJJByIdUnidadTributaria(long idUnidadTributaria)
        {
            long idComponente = Convert.ToInt64(this.GetContexto().ParametrosGenerales.SingleOrDefault(p => p.Clave == "ID_COMPONENTE_DDJJ").Valor);
            var query = from entrada in this.GetContexto().TramitesEntradas
                            //join ddjj in this.GetContexto().DDJJ on entrada.IdObjeto equals ddjj.IdDeclaracionJurada
                        join tramite in this.GetContexto().TramitesMesaEntrada on entrada.IdTramite equals tramite.IdTramite
                        join valuacion in this.GetContexto().VALValuacion on entrada.IdObjeto equals valuacion.IdDeclaracionJurada
                        where entrada.IdComponente == idComponente && valuacion.IdUnidadTributaria == idUnidadTributaria
                        select new { valuacion.IdValuacion, tramite.Numero };

            return query.ToList().ToDictionary(r => r.IdValuacion, r => r.Numero);

        }

        public string[] GenerarMensura(long idDepartamento)
        {
            using (Stream lockFile = adquirirLockSecuenciaMensura(true, idDepartamento))
            {
                try
                {
                    var secuencia = this.GetContexto().MensuraSecuencias
                                        .Where(x => x.Departamento == idDepartamento)
                                        .FirstOrDefault();

                    string letra = string.Empty, numero = string.Empty;
                    if (secuencia != null)
                    {
                        letra = secuencia.LetraMensura;

                        bool bExist = false;
                        do
                        {
                            numero = $"{++secuencia.Valor}";
                            var query = from ut in this.GetContexto().Mensura
                                        where ut.Numero == numero && ut.Letra == letra
                                        select ut.Numero;

                            var queryTemp = from ut in this.GetContexto().MensurasTemporal
                                            where ut.Numero == numero && ut.Letra == letra
                                            select ut.Numero;

                            bExist = query.Any() || queryTemp.Any();
                        } while (bExist);
                        this.GetContexto().SaveChanges();
                    }
                    else
                    {
                        numero = "Sin";
                        letra = "Letra";
                    }
                    return new string[] { numero, letra };
                }
                catch (Exception ex)
                {
                    return new string[] { "ERROR", ex.Message };
                }
                finally
                {
                    lockFile.Close();
                }
            }
        }

        public string GenerarPartidaInmobiliaria(string[] valores)
        {
            try
            {
                return GenerarPartidaInmobiliaria(Convert.ToInt64(valores.ElementAt(valores.Length - 1)), Convert.ToInt64(valores.ElementAt(valores.Length - 2)));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public string GenerarPartidaInmobiliaria(long idJurisdiccion, long tipo)
        {
            using (Stream lockFile = adquirirLockSecuenciaPartida(true, idJurisdiccion, tipo))
            {
                var jurisdiccion = this.GetContexto().Objetos.Find(idJurisdiccion);
                try
                {
                    string nextSecuencia = string.Empty;
                    PartidaSecuencia secuencia = this.GetContexto().PartidaSecuencias
                                                     .FirstOrDefault(x => x.Jurisdiccion == idJurisdiccion && x.TipoParcela == tipo);
                    if (secuencia != null)
                    {
                        bool bExist = false;
                        do
                        {
                            nextSecuencia = $"{jurisdiccion.Codigo}{++secuencia.Valor:000000}{tipo}";

                            var queryCorriente = from ut in this.GetContexto().UnidadesTributarias
                                                 where ut.CodigoProvincial == nextSecuencia
                                                 orderby ut.FechaAlta ascending //Código de DGCyC corrientes
                                                 select ut.CodigoProvincial;

                            var queryTemporal = from ut in this.GetContexto().UnidadesTributariasTemporal
                                                where ut.CodigoProvincial == nextSecuencia
                                                orderby ut.FechaAlta ascending //Código de DGCyC corrientes
                                                select ut.CodigoProvincial;

                            bExist = queryCorriente.Any() || queryTemporal.Any();
                        } while (bExist);
                        this.GetContexto().SaveChanges();
                        return nextSecuencia;
                    }
                    else
                    {
                        throw new Exception($"Secuencia no inicializada para la jurisdicción {jurisdiccion.Codigo} y el tipo {tipo}");
                    }
                }
                catch (Exception ex)
                {
                    GetContexto().GetLogger().LogError($"GenerarPartidaInmobiliaria({jurisdiccion.Codigo}, {tipo})", ex);
                    throw;
                }
                finally
                {
                    lockFile.Close();
                }
            }

        }
        public string GenerarNomenclaturaParcela(string[] valores)
        {
            if (valores.Last().Contains(","))
            {
                try
                {
                    return GenerarNomenclaturaParcela(valores.Last());
                }
                catch (Exception ex)
                {
                    return "ERROR: " + ex.Message;
                }
            }
            return string.Empty;
        }
        public string GenerarNomenclaturaParcela(string wkt)
        {
            try
            {
                if (string.IsNullOrEmpty(wkt))
                {
                    return string.Empty;
                }
                long mts = 2;
                string nomenclatura;
                do
                {
                    using (var qbuilder = this.GetContexto().CreateSQLQueryBuilder())
                    {
                        nomenclatura = qbuilder.AddFunctionTable($"GENERAR_NOMENCLATURA_PARCELA('SRID=5348;{wkt}',{mts++})", null)
                                               .AddFormattedField("*")
                                               .ExecuteQuery(reader =>
                                               {
                                                   int fieldIdx = 0;
                                                   while (fieldIdx < reader.FieldCount)
                                                   {
                                                       bool intersect = (bool)reader.GetValue(++fieldIdx);
                                                       bool usada = (bool)reader.GetValue(++fieldIdx);
                                                       if (intersect && !usada)
                                                       {
                                                           return reader.GetValue(fieldIdx - 2).ToString();
                                                       }
                                                       fieldIdx++;
                                                   }
                                                   return string.Empty;
                                               })
                                               .SingleOrDefault();
                    }
                } while (string.IsNullOrEmpty(nomenclatura));
                return nomenclatura;
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("MesaEntradaRepositoriy.GenerarNomenclaturaParcela", ex);
                GetContexto().GetLogger().LogInfo($"{string.Join(Environment.NewLine, new[] { "MesaEntradaRepositoriy.GenerarNomenclaturaParcela =>", wkt })}");
                throw;
            }
        }

        public IEnumerable<MERequisitoCumplido> GetObjetosByRequisito(int idObjetoTramite, int idTramite = 0)
        {
            try
            {
                if (idTramite == 0)
                {
                    var query = from objetoR in this.GetContexto().ObjetosRequisitos
                                join requisito in this.GetContexto().Requisitos on objetoR.IdRequisito equals requisito.IdRequisito
                                where objetoR.IdObjetoTramite == idObjetoTramite
                                orderby new { objetoR.Obligatorio, requisito.Descripcion }
                                select new
                                {
                                    requisito.Descripcion,
                                    objetoR.IdObjetoRequisito,
                                    objetoR.Obligatorio,
                                };


                    return query.ToList().Select(r => new MERequisitoCumplido
                    {
                        IdObjetoRequisito = r.IdObjetoRequisito,
                        Descripcion = r.Descripcion,
                        Cumplido = 0,
                        Obligatorio = r.Obligatorio
                    });

                }
                else
                {
                    var query = from objetoR in this.GetContexto().ObjetosRequisitos
                                join requisito in this.GetContexto().Requisitos on objetoR.IdRequisito equals requisito.IdRequisito
                                join tramiteR in this.GetContexto().TramitesRequisitos on new { objetoR.IdObjetoRequisito, IdTramite = idTramite } equals new { tramiteR.IdObjetoRequisito, tramiteR.IdTramite } into lj
                                from requeCumplido in lj.DefaultIfEmpty()
                                where objetoR.IdObjetoTramite == idObjetoTramite
                                orderby new { objetoR.Obligatorio, requisito.Descripcion }
                                select new
                                {
                                    requisito.Descripcion,
                                    objetoR.IdObjetoRequisito,
                                    objetoR.Obligatorio,
                                    cumplido = requeCumplido != null
                                };

                    return query.ToList().Select(r => new MERequisitoCumplido
                    {
                        IdObjetoRequisito = r.IdObjetoRequisito,
                        Descripcion = r.Descripcion,
                        Cumplido = r.cumplido ? 1 : 0,
                        Obligatorio = r.Obligatorio
                    });

                }
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError($"GetObjetosByRequisito/{idObjetoTramite}", ex);
                throw;
            }
        }

        public void Procesar(METramite tramite)
        {
            try
            {
                Tramites.ProcessorTramiteBuilder.GetProcessor(tramite, GetContexto()).Process();
            }
            catch (Exception ex)
            {
                GetContexto().GetLogger().LogError("Procesar", ex);
                throw;
            }
        }

        private void CrearTramiteEntradasTemporal(IList<RelacionDto> relaciones, MEDatosEspecificos[] datosEspecificos, METramite tramite, Usuarios usuario, List<long> entradasProcesadas)
        {
            foreach (var item in datosEspecificos)
            {
                //TODO: fix temporal.. esto no deberia venir null
                if (item.ParentGuids == null)
                    item.ParentGuids = new List<Guid>();

                if (item.ParentIdTramiteEntradas == null)
                    item.ParentIdTramiteEntradas = new List<int>();


                var entrada = this.GetContexto().Entradas.FirstOrDefault(t => t.IdEntrada == item.TipoObjeto.Id);
                var objetoEntrada = this.GetContexto().ObjetosEntrada.FirstOrDefault(t => t.IdEntrada == entrada.IdEntrada && t.IdObjetoTramite == tramite.IdObjetoTramite);

                //Caso base
                if (esNodoRaiz(item))
                {
                    long? idObjeto = CrearEditarDatoEspecificoTemporal(entrada, item, datosEspecificos, tramite, usuario, relaciones, entradasProcesadas);

                    METramiteEntrada tramiteEntrada;
                    if (item.IdTramiteEntrada != null)
                    {
                        tramiteEntrada = this.GetContexto().TramitesEntradas.FirstOrDefault(t => t.IdTramiteEntrada == item.IdTramiteEntrada);
                    }
                    else
                    {
                        tramite.TramiteEntradas.Add(new METramiteEntrada
                        {
                            UsuarioAlta = usuario.Id_Usuario,
                            FechaAlta = DateTime.Now,
                            IdComponente = entrada.IdComponente,
                            IdObjetoEntrada = objetoEntrada.IdObjetoEntrada
                        });
                        tramiteEntrada = tramite.TramiteEntradas.Last();
                    }
                    tramiteEntrada.IdObjeto = idObjeto;
                    tramiteEntrada.UsuarioModif = usuario.Id_Usuario;
                    tramiteEntrada.FechaModif = DateTime.Now;

                    this.GetContexto().SaveChanges();
                    relaciones.Add(new RelacionDto { Guid = item.Guid, IdTramiteEntrada = item.IdTramiteEntrada, TramiteEntrada = tramiteEntrada });
                }
                else //Recursion
                {
                    long? idObjeto = CrearEditarDatoEspecificoTemporal(entrada, item, datosEspecificos, tramite, usuario, relaciones, entradasProcesadas);

                    METramiteEntrada tramiteEntradaHijo;
                    if (item.IdTramiteEntrada != null)
                    {
                        tramiteEntradaHijo = this.GetContexto().TramitesEntradas
                            .Include("TramiteEntradaRelaciones")
                            .FirstOrDefault(t => t.IdTramiteEntrada == item.IdTramiteEntrada);
                    }
                    else
                    {
                        tramite.TramiteEntradas.Add(new METramiteEntrada
                        {
                            IdComponente = entrada.IdComponente,
                            IdObjetoEntrada = objetoEntrada.IdObjetoEntrada,
                            UsuarioAlta = usuario.Id_Usuario,
                            FechaAlta = DateTime.Now,
                            TramiteEntradaRelaciones = (item.ParentGuids.Any() || item.ParentIdTramiteEntradas.Any()) ? new List<METramiteEntradaRelacion>() : null
                        });
                        tramiteEntradaHijo = tramite.TramiteEntradas.Last();
                    }

                    tramiteEntradaHijo.IdObjeto = idObjeto;


                    tramiteEntradaHijo.UsuarioModif = usuario.Id_Usuario;
                    tramiteEntradaHijo.FechaModif = tramite.FechaModif;

                    var entradasPadresVigentes = new HashSet<long>();

                    foreach (var itemPadre in item.ParentGuids) //Elementos padres "nuevos"
                    {
                        bool matchEntradaPadre(dynamic relacion) => relacion.Guid == itemPadre;
                        //Chequear que el parent esten en el dictionary
                        if (!relaciones.Any((Func<dynamic, bool>)matchEntradaPadre))
                        {
                            //Si no existe, lo creo.
                            var padres = datosEspecificos.Where((Func<dynamic, bool>)matchEntradaPadre).Select(a => a as MEDatosEspecificos);
                            CrearTramiteEntradasTemporal(relaciones, padres.ToArray(), tramite, usuario, entradasProcesadas);
                        }

                        //Agrego la relacion al padre
                        var entradaPadre = relaciones.FirstOrDefault((Func<dynamic, bool>)matchEntradaPadre).TramiteEntrada;
                        if (!tramiteEntradaHijo.TramiteEntradaRelaciones.Any(tr => tr.IdTramiteEntradaPadre == entradaPadre.IdTramiteEntrada))
                        {
                            tramiteEntradaHijo.TramiteEntradaRelaciones.Add(new METramiteEntradaRelacion
                            {
                                TramiteEntrada = tramiteEntradaHijo,
                                TramiteEntradaPadre = relaciones.FirstOrDefault((Func<dynamic, bool>)matchEntradaPadre).TramiteEntrada,
                                UsuarioAlta = usuario.Id_Usuario,
                                FechaAlta = tramite.FechaModif,
                                UsuarioModif = usuario.Id_Usuario,
                                FechaModif = tramite.FechaModif
                            });
                        }
                        entradasPadresVigentes.Add(entradaPadre.IdTramiteEntrada);
                        relaciones.Add(new RelacionDto { Guid = item.Guid, IdTramiteEntrada = item.IdTramiteEntrada, TramiteEntrada = tramiteEntradaHijo });
                    }
                    foreach (var entradaPadre in tramiteEntradaHijo.TramiteEntradaRelaciones.Where(x => !entradasPadresVigentes.Contains(x.TramiteEntradaPadre?.IdTramiteEntrada ?? x.IdTramiteEntradaPadre)).ToArray())
                    {
                        //tramiteEntradaHijo.TramiteEntradaRelaciones.Remove(entradaPadre);
                        this.GetContexto().Entry(entradaPadre).State = EntityState.Deleted;
                    }

                }
            }
        }

        private void BorrarRegistrosTemporales(Atributo campoClave, string nombreTabla, string objetos, Common.Enums.SQLOperators operador, int idTramite)
        {
            using (var qb = this.GetContexto().CreateSQLQueryBuilder())
            {
                qb.AddTable("temporal", nombreTabla, null)
                    .AddFilter(campoClave.Campo, objetos, operador)
                    .AddFilter("id_tramite", idTramite, Common.Enums.SQLOperators.EqualsTo, Common.Enums.SQLConnectors.And)
                    .ExecuteDelete();
            }
        }

        private void CopiarRegistros(Atributo campoClave, string nombreTabla, long idObjeto, int IdTramite)
        {
            CopiarRegistros(new[] { campoClave }, nombreTabla, new long[] { idObjeto }, IdTramite);
        }
        private void CopiarRegistros(IEnumerable<Atributo> camposClaves, string nombreTabla, IEnumerable<long> ids, int IdTramite)
        {

            using (var queryEsquema = this.GetContexto().CreateSQLQueryBuilder())
            using (var insertBuilder = this.GetContexto().CreateSQLQueryBuilder())
            using (var querybuilder = this.GetContexto().CreateSQLQueryBuilder())
            {
                try
                {
                    var Tabla = new Componente()
                    {
                        ComponenteId = camposClaves.FirstOrDefault().ComponenteId,
                        Esquema = ConfigurationManager.AppSettings["DATABASE"],
                        Tabla = nombreTabla
                    };
                    var camposParcela = queryEsquema.GetTableFields(Tabla.Esquema, Tabla.Tabla)
                                .Select(c => new KeyValuePair<Atributo, object>(new Atributo() { Campo = c }, c))
                                .Concat(new[] { new KeyValuePair<Atributo, object>(new Atributo() { Campo = "id_tramite" }, IdTramite) });

                    querybuilder.AddTable(Tabla, null);
                    int i = 0;
                    Common.Enums.SQLConnectors conector = Common.Enums.SQLConnectors.None;
                    foreach (var campoClave in camposClaves)
                    {
                        campoClave.Componente = Tabla;
                        querybuilder.AddFilter(campoClave, ids.ElementAt(i), Common.Enums.SQLOperators.EqualsTo, conector);
                        i++;
                        conector = Common.Enums.SQLConnectors.And;
                    }


                    var fechaBaja = camposParcela.Select(c => c.Key).SingleOrDefault(c => c.Campo.ToLower() == "fecha_baja");

                    if (fechaBaja != null)
                    {
                        fechaBaja.Componente = Tabla;
                        querybuilder.AddFilter(fechaBaja, null, Common.Enums.SQLOperators.IsNull, Common.Enums.SQLConnectors.And);

                    }

                    insertBuilder.AddTable("temporal", Tabla.Tabla, null)
                        .AddQueryToInsert(querybuilder, camposParcela.ToArray())
                        .ExecuteInsert();

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        private long? CrearEditarDatoEspecificoTemporal(MEEntrada entrada, MEDatosEspecificos item, MEDatosEspecificos[] arbolObjetos, METramite tramite, Usuarios usuarioAlta, IList<RelacionDto> relaciones, List<long> entradasProcesadas)
        {
            entradasProcesadas = entradasProcesadas ?? new List<long>();

            // Se tomo como base CrearEditarDatoEspecifico
            var tramiteEntrada = this.GetContexto().TramitesEntradas.FirstOrDefault(t => t.IdTramiteEntrada == item.IdTramiteEntrada);

            if (entrada.IdEntrada == Convert.ToInt32(Entradas.Parcela))
            {
                var hdnOperacion = item.Propiedades.FirstOrDefault(t => t.Id == "hdnOperacionObjetoEspecifico")?.Value;
                long hdnObjeto = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona")?.Value);

                if (hdnOperacion == OperacionParcela.Origen.ToString())
                {
                    if (tramiteEntrada?.IdObjeto != hdnObjeto)
                    {
                        try
                        {
                            long idComponenteParcela = long.Parse(this.GetContexto().ParametrosGenerales.FirstOrDefault(p => p.Clave == "ID_COMPONENTE_PARCELA").Valor);
                            var campoClave = this.GetContexto().Atributo.Include(a => a.Componente).GetAtributoClaveByComponente(idComponenteParcela);
                            var tablas = new[] { campoClave.Componente.TablaTemporal, campoClave.Componente.TablaGrafica, "inm_unidad_tributaria", "inm_nomenclatura", "inm_designacion" };
                            this.GetContexto().Entry(campoClave).State = EntityState.Detached;
                            var objetoEntrada = this.GetContexto().ObjetosEntrada.FirstOrDefault(x => x.IdEntrada == entrada.IdEntrada);
                            bool esMismoRegistro = false;
                            if (tramiteEntrada != null)
                            {
                                entradasProcesadas.Add(tramiteEntrada.IdTramiteEntrada);
                                LimpiarRelacionesParcela(tramite, tramiteEntrada.IdObjeto.Value, campoClave, tablas);
                            }
                            else if ((tramiteEntrada = this.GetContexto().TramitesEntradas.FirstOrDefault(p => p.IdObjeto == hdnObjeto && p.IdTramite == tramite.IdTramite && p.IdObjetoEntrada == objetoEntrada.IdObjetoEntrada)) != null)
                            {
                                esMismoRegistro = true;
                                item.IdTramiteEntrada = tramiteEntrada.IdTramiteEntrada;
                            }
                            else
                            {
                                var utsAnteriores = this.GetContexto().UnidadesTributariasTemporal.Where(ut => ut.ParcelaID == hdnObjeto && ut.IdTramite == tramite.IdTramite);
                                if (utsAnteriores.Any())
                                {
                                    int idEntradaUt = int.Parse(Entradas.UnidadTributaria);
                                    int idObjetoUt = this.GetContexto().ObjetosEntrada.FirstOrDefault(o => o.IdEntrada == idEntradaUt).IdObjetoEntrada;
                                    var entradasAnteriores = this.GetContexto().TramitesEntradas.Where(p => utsAnteriores.Any(ut => ut.UnidadTributariaId == p.IdObjeto) && p.IdTramite == tramite.IdTramite && p.IdObjetoEntrada == idObjetoUt);
                                    this.GetContexto().TramitesEntradas.RemoveRange(entradasAnteriores);
                                    LimpiarRelacionesParcela(tramite, utsAnteriores.First().ParcelaID.Value, campoClave, tablas);
                                }
                            }
                            if (!esMismoRegistro)
                            {
                                foreach (string tabla in tablas)
                                {
                                    CopiarRegistros(campoClave, tabla, hdnObjeto, tramite.IdTramite);
                                }

                                var utParcela = GetContexto()
                                                    .UnidadesTributarias
                                                    .Single(ut => ut.ParcelaID == hdnObjeto && ut.TipoUnidadTributariaID != (int)TipoUnidadTributariaEnum.UnidadFuncionalPH && ut.FechaBaja == null);

                                CopiarRegistros(new Atributo() { Campo = "ID_UNIDAD_TRIBUTARIA" }, "inm_dominio", utParcela.UnidadTributariaId, tramite.IdTramite);

                                foreach (var dominio in GetContexto().Dominios.Where(d => d.UnidadTributariaID == utParcela.UnidadTributariaId && d.FechaBaja == null).ToArray())
                                {
                                    CopiarRegistros(new Atributo() { Campo = "ID_DOMINIO" }, "inm_dominio_titular", dominio.DominioID, tramite.IdTramite);
                                }
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    return hdnObjeto;
                }

                var zona = item.Propiedades.FirstOrDefault(t => t.Id == "Zona")?.Value;
                var tipo = item.Propiedades.FirstOrDefault(t => t.Id == "Tipo")?.Value;
                var plano = item.Propiedades.FirstOrDefault(t => t.Id == "Plano")?.Value;

                ParcelaTemporal obj;
                if (tramiteEntrada == null) //Alta parcela
                {
                    obj = new ParcelaTemporal
                    {
                        IdTramite = tramite.IdTramite,
                        TipoParcelaID = Convert.ToInt64(zona),
                        ClaseParcelaID = Convert.ToInt64(tipo),
                        PlanoId = plano,
                        //TODO:
                        //SUPERFICIE = en caso de tener una DDJJ U como objeto hijo = el valor obtenido de VAL_DDJJ_U.SUP_PLANO o SUP_TITULO(en ese orden).
                        //En caso de tener una DDJJ SOR como objeto hijo = la sumatoria de VAL_SUPERFICIES.SUPERFICIE para dicha DDJJ SOR.En otro caso,
                        //superficie = 0.
                        Superficie = 0,
                        EstadoParcelaID = 98,
                        OrigenParcelaID = 2,
                        UsuarioAltaID = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        UsuarioModificacionID = usuarioAlta.Id_Usuario,
                        FechaModificacion = DateTime.Now
                    };

                    this.GetContexto().ParcelasTemporal.Add(obj);

                }
                else //Modificacion parcela
                {
                    long hdnObjetoH = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona")?.Value);

                    obj = this.GetContexto().ParcelasTemporal.FirstOrDefault(t => t.ParcelaID == tramiteEntrada.IdObjeto
                                                                                && t.IdTramite == tramiteEntrada.IdTramite);
                    //obj = this.GetContexto().ParcelasTemporal.FirstOrDefault(t => t.ParcelaID == hdnObjetoH
                    //                                        && t.IdTramite == tramite.IdTramite);

                    obj.TipoParcelaID = Convert.ToInt64(zona);
                    obj.ClaseParcelaID = Convert.ToInt64(tipo);
                    obj.PlanoId = plano;
                    //TODO:
                    //SUPERFICIE = en caso de tener una DDJJ U como objeto hijo = el valor obtenido de VAL_DDJJ_U.SUP_PLANO o SUP_TITULO(en ese orden).
                    //En caso de tener una DDJJ SOR como objeto hijo = la sumatoria de VAL_SUPERFICIES.SUPERFICIE para dicha DDJJ SOR.En otro caso,
                    //superficie = 0.
                    obj.Superficie = 0;
                    obj.EstadoParcelaID = 98;
                    obj.OrigenParcelaID = 2;
                    obj.UsuarioModificacionID = usuarioAlta.Id_Usuario;
                    obj.FechaModificacion = DateTime.Now;
                }

                this.GetContexto().SaveChanges();
                return obj.ParcelaID;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.UnidadTributaria))
            {
                long hdnObjeto = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "hdnIdUnidadFuncional")?.Value);

                var hdnOperacion = item.Propiedades.FirstOrDefault(t => t.Id == "hdnOperacionObjetoEspecifico")?.Value;

                if (hdnOperacion == OperacionParcela.Origen.ToString())
                {
                    if (tramiteEntrada?.IdObjeto != hdnObjeto)
                    {
                        try
                        {
                            long idComponenteParcela = long.Parse(this.GetContexto().ParametrosGenerales.FirstOrDefault(p => p.Clave == "ID_COMPONENTE_PARCELA").Valor);
                            long idComponenteUT = long.Parse(this.GetContexto().ParametrosGenerales.FirstOrDefault(p => p.Clave == "ID_COMPONENTE_UNIDAD_TRIBUTARIA").Valor);

                            var componenteUt = this.GetContexto().Componente.Find(idComponenteUT);

                            var campoClave = this.GetContexto().Atributo.Include(a => a.Componente).GetAtributoClaveByComponente(idComponenteParcela);
                            var tablasClaveParcela = new[] { campoClave.Componente.TablaTemporal, campoClave.Componente.TablaGrafica };
                            var tablas = tablasClaveParcela.Append(componenteUt.TablaTemporal).ToArray();
                            this.GetContexto().Entry(campoClave).State = EntityState.Detached;
                            var objetoEntrada = this.GetContexto().ObjetosEntrada.FirstOrDefault(x => x.IdEntrada == entrada.IdEntrada);
                            bool esMismoRegistro = false;
                            if (tramiteEntrada != null)
                            {
                                long parcelaIdAnterior = this.GetContexto().UnidadesTributariasTemporal.Find(tramiteEntrada.IdObjeto.Value, tramite.IdTramite).ParcelaID.Value;
                                entradasProcesadas.Add(tramiteEntrada.IdTramiteEntrada);
                                LimpiarRelacionesParcela(tramite, parcelaIdAnterior, campoClave, tablas);
                            }
                            else if ((tramiteEntrada = this.GetContexto().TramitesEntradas.FirstOrDefault(p => p.IdObjeto == hdnObjeto && p.IdTramite == tramite.IdTramite && p.IdObjetoEntrada == objetoEntrada.IdObjetoEntrada)) != null)
                            {
                                esMismoRegistro = true;
                                item.IdTramiteEntrada = tramiteEntrada.IdTramiteEntrada;
                            }
                            else
                            {
                                long? parcelaIdAnterior = this.GetContexto().UnidadesTributariasTemporal.Find(hdnObjeto, tramite.IdTramite)?.ParcelaID.Value;
                                if (parcelaIdAnterior.HasValue)
                                {
                                    int idEntradaParcela = int.Parse(Entradas.Parcela);
                                    int idObjetoEntradaParcela = this.GetContexto().ObjetosEntrada.FirstOrDefault(o => o.IdEntrada == idEntradaParcela).IdObjetoEntrada;
                                    var tramiteEntradaAnterior = this.GetContexto().TramitesEntradas.FirstOrDefault(p => p.IdObjeto == parcelaIdAnterior && p.IdTramite == tramite.IdTramite && p.IdObjetoEntrada == idObjetoEntradaParcela);
                                    this.GetContexto().Entry(tramiteEntradaAnterior).State = EntityState.Deleted;
                                    LimpiarRelacionesParcela(tramite, parcelaIdAnterior.Value, campoClave, tablas);
                                }
                            }
                            if (!esMismoRegistro)
                            {
                                long parcelaIdNueva = this.GetContexto().UnidadesTributarias.Find(hdnObjeto).ParcelaID.Value;
                                foreach (var tabla in tablasClaveParcela)
                                {
                                    CopiarRegistros(campoClave, tabla, parcelaIdNueva, tramite.IdTramite);
                                }

                                campoClave = this.GetContexto().Atributo.GetAtributoClaveByComponente(idComponenteUT);
                                this.GetContexto().Entry(campoClave).State = EntityState.Detached;
                                CopiarRegistros(campoClave, componenteUt.TablaTemporal, hdnObjeto, tramite.IdTramite);
                            }

                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    return hdnObjeto;
                }

                var tipo = item.Propiedades.FirstOrDefault(t => t.Id == "Tipo")?.Value;
                var plano = item.Propiedades.FirstOrDefault(t => t.Id == "Plano")?.Value;
                var piso = item.Propiedades.FirstOrDefault(t => t.Id == "Piso")?.Value;
                var unidad = item.Propiedades.FirstOrDefault(t => t.Id == "Unidad")?.Value;
                var superficie = item.Propiedades.FirstOrDefault(t => t.Id == "Superficie")?.Value;
                var vigencia = item.Propiedades.FirstOrDefault(t => t.Id == "vigencia")?.Value;


                UnidadTributariaTemporal obj;

                if (tramiteEntrada == null) //Unidad Tributaria Destino Alta
                {
                    var tramite_entrada_padre = relaciones.FirstOrDefault(t => t.Guid == item.ParentGuids.Single());

                    obj = new UnidadTributariaTemporal
                    {
                        TipoUnidadTributariaID = Convert.ToInt32(tipo),
                        PlanoId = plano,
                        Piso = piso,
                        Unidad = unidad,
                        Superficie = Convert.ToDouble(superficie),
                        UsuarioAltaID = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        UsuarioModificacionID = usuarioAlta.Id_Usuario,
                        FechaModificacion = DateTime.Now,
                        IdTramite = tramite.IdTramite,
                        ParcelaID = tramite_entrada_padre.TramiteEntrada.IdObjeto,
                        Vigencia = Convert.ToDateTime(vigencia),
                    };

                    this.GetContexto().UnidadesTributariasTemporal.Add(obj);


                }
                else //Modificacion unidad funcional
                {
                    obj = this.GetContexto().UnidadesTributariasTemporal.FirstOrDefault(t => t.UnidadTributariaId == tramiteEntrada.IdObjeto);

                    obj.PlanoId = plano;
                    obj.Piso = piso;
                    obj.Unidad = unidad;
                    obj.Superficie = Convert.ToDouble(superficie);
                    obj.UsuarioModificacionID = usuarioAlta.Id_Usuario;
                    obj.FechaModificacion = DateTime.Now;
                    obj.TipoUnidadTributariaID = Convert.ToInt32(tipo);
                    obj.Vigencia = Convert.ToDateTime(vigencia);
                }

                this.GetContexto().SaveChanges();
                return obj.UnidadTributariaId;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.MensuraRegistrada))
            {
                long nroMensura = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "hdnIdMensura")?.Value);
                try
                {
                    var cmpMensura = Convert.ToInt64(this.GetContexto().ParametrosGenerales.FirstOrDefault(p => p.Clave == "ID_COMPONENTE_MENSURA")?.Valor);
                    var campoClave = this.GetContexto().Atributo.Include(a => a.Componente).GetAtributoClaveByComponente(cmpMensura);
                    string tabla = campoClave.Componente.TablaTemporal;
                    this.GetContexto().Entry(campoClave).State = EntityState.Detached;
                    var objetoEntrada = this.GetContexto().ObjetosEntrada.FirstOrDefault(x => x.IdEntrada == entrada.IdEntrada);
                    bool esMismoRegistro = false;
                    if (tramiteEntrada != null)
                    {
                        BorrarRegistrosTemporales(campoClave, tabla, tramiteEntrada.IdObjeto.ToString(), Common.Enums.SQLOperators.EqualsTo, tramite.IdTramite);
                    }
                    else if ((tramiteEntrada = this.GetContexto().TramitesEntradas.FirstOrDefault(p => p.IdObjeto == nroMensura && p.IdTramite == tramite.IdTramite && p.IdObjetoEntrada == objetoEntrada.IdObjetoEntrada)) != null)
                    {
                        esMismoRegistro = true;
                        item.IdTramiteEntrada = tramiteEntrada.IdTramiteEntrada;
                    }
                    if (!esMismoRegistro)
                    {
                        CopiarRegistros(campoClave, tabla, nroMensura, tramite.IdTramite);

                    }

                    return nroMensura;

                }
                catch (Exception)
                {

                    throw;
                }
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.DescripcionInmueble))
            {
                //var objPadre = arbolObjetos.FirstOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));

                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                var tipoObjetoPadre = this.GetContexto().Entradas.FirstOrDefault(t => t.IdEntrada == objPadre.TipoObjeto.Id);

                if (!new[] { int.Parse(Entradas.Parcela), int.Parse(Entradas.UnidadTributaria) }.Contains(tipoObjetoPadre.IdEntrada))
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre de tipo Parcela o Unidad funcional");
                }

                if (!long.TryParse(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona")?.Value, out long parcelaId))
                { //si el padre es una unidad tributaria, recupero la parcela
                    long idUT = long.Parse(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdUnidadFuncional")?.Value);
                    parcelaId = this.GetContexto().UnidadesTributariasTemporal.Find(idUT, tramite.IdTramite).ParcelaID.Value;
                }

                var mensuraTemporal = (from pmt in this.GetContexto().ParcelaMensurasTemporal
                                       where pmt.IdParcela == parcelaId && pmt.IdTramite == tramite.IdTramite
                                       select pmt).FirstOrDefault();

                long mensuraId;

                if (mensuraTemporal == null)
                {

                    mensuraId = (from pm in this.GetContexto().ParcelaMensura
                                 where pm.IdParcela == parcelaId && pm.FechaBaja == null
                                 orderby pm.FechaAlta descending
                                 select pm.IdMensura).FirstOrDefault();

                    if (mensuraId != 0)
                    {

                        CopiarRegistros(new Atributo() { Campo = "id_mensura" }, "inm_mensura", mensuraId, tramite.IdTramite);
                        CopiarRegistros(new[] { new Atributo() { Campo = "id_mensura" }, new Atributo() { Campo = "id_parcela" } }, "inm_parcela_mensura", new[] { mensuraId, parcelaId }, tramite.IdTramite);

                    }
                    else
                    {
                        throw new InvalidOperationException($"La parcela no tiene una mensura asociada");
                    }

                }
                else
                {
                    mensuraId = mensuraTemporal.IdMensura;
                }
                var unidadTributaria = this.GetContexto().UnidadesTributariasTemporal.FirstOrDefault(t => t.ParcelaID == parcelaId);

                //var parcelaMensura = this.GetContexto().ParcelaMensurasTemporal.FirstOrDefault(t => t.IdParcela == parcelaId);

                var personaEntity = this.GetContexto().Entradas.FirstOrDefault(t => t.Descripcion == "Persona");
                var personasCargadas = arbolObjetos.Where(t => t.TipoObjeto.Id == personaEntity.IdEntrada);

                if (!personasCargadas.Any())
                {
                    throw new ArgumentException("Debe existir una persona cargada dentro del tramite");
                }
                var persona = personasCargadas.FirstOrDefault();
                //var idPersona = persona.Propiedades.Where(t => t.Id == "hdnIdPersona");
                var idPersona = Convert.ToInt64(persona.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPersona")?.Value);
                //var objPersona = this.GetContexto().Persona.FirstOrDefault(t => t.PersonaId == Convert.ToInt32(idPersona));
                var objPersona = this.GetContexto().Persona.FirstOrDefault(t => t.PersonaId == idPersona);

                INMCertificadoCatastralTemporal obj = this.GetContexto().INMCertificadosCatastralesTemporal.FirstOrDefault(t => t.MensuraId == mensuraId && t.IdTramite == tramite.IdTramite);

                if (tramiteEntrada == null || obj == null) //Alta descripcion del inmueble
                {
                    obj = this.GetContexto().INMCertificadosCatastralesTemporal.Add(new INMCertificadoCatastralTemporal
                    {
                        UsuarioAltaId = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        IdTramite = tramite.IdTramite,
                    });
                }

                obj.UnidadTributariaId = unidadTributaria.UnidadTributariaId; //de acuerdo al ID de la Unidad Tributaria de la parcela o UF padre(debe haber una)
                obj.MensuraId = mensuraId; //de acuerdo a la ultima mensura relacionada a la parcela padre (si el objeto padre es una UF, se debe obtener la parcela relacionada)
                obj.SolicitanteId = objPersona.PersonaId; //de acuerdo al ID de la persona cargada al arbol de objetos (debe haber una)
                obj.Descripcion = item.Propiedades.FirstOrDefault(t => t.Id == "Descripcion")?.Value;
                obj.Motivo = tramite.Motivo;
                obj.UsuarioModifId = usuarioAlta.Id_Usuario;
                obj.FechaModif = DateTime.Now;

                this.GetContexto().SaveChanges();

                return obj.CertificadoCatastralId;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.DDJJ))
            {
                #region Validación de nodo padre
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));
                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                string hdnOperacion = objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnOperacionObjetoEspecifico")?.Value;
                var tipoObjetoPadre = this.GetContexto().Entradas.FirstOrDefault(t => t.IdEntrada == objPadre.TipoObjeto.Id);

                if ((tipoObjetoPadre.IdEntrada != int.Parse(Entradas.Parcela) || hdnOperacion != OperacionParcela.Origen.ToString()) && tipoObjetoPadre.IdEntrada != int.Parse(Entradas.UnidadTributaria))
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre de tipo Unidad Funcional o Parcela. En caso de ser una Parcela, la misma ya debe existir en el parcelario vigente.");
                }
                #endregion

                var mainJObject = JObject.Parse(item.Propiedades.Single(p => p.Id == "DeclaracionJurada").Value);
                #region Instanciación de DDJJ
                DDJJTemporal ddjjTemporal = tramiteEntrada == null
                                            ? null
                                            : this.GetContexto().DeclaracionesJuradasTemporal
                                                                .SingleOrDefault(x => x.IdDeclaracionJurada == tramiteEntrada.IdObjeto.Value && x.IdTramite == tramite.IdTramite);

                if (ddjjTemporal == null) //Alta DDJJ
                {
                    ddjjTemporal = this.GetContexto()
                                       .DeclaracionesJuradasTemporal
                                       .Add(new DDJJTemporal
                                       {
                                           IdVersion = long.Parse(item.Propiedades.Single(p => p.Id == "IdVersion").Value),
                                           IdTramite = tramite.IdTramite,
                                           IdOrigen = 2, //presentada
                                           UsuarioAlta = usuarioAlta.Id_Usuario,
                                           FechaAlta = tramite.FechaModif,
                                       });
                }
                #endregion
                #region Actualización de Datos
                var ddjjJObject = mainJObject["DDJJ"];
                bool esNuevaDDJJ = this.GetContexto().Entry(ddjjTemporal).State == EntityState.Added;
                #region Obtención de IdUnidadTributaria
                var entradaPadre = relaciones.FirstOrDefault(t => t.Guid == objPadre.Guid);
                long idUT;
                if (tipoObjetoPadre.IdEntrada == int.Parse(Entradas.Parcela))
                {
                    idUT = this.GetContexto().UnidadesTributariasTemporal.First(ut => ut.ParcelaID == entradaPadre.TramiteEntrada.IdObjeto).UnidadTributariaId;
                }
                else
                {
                    idUT = entradaPadre.TramiteEntrada.IdObjeto.Value;
                }
                ddjjTemporal.IdUnidadTributaria = idUT;
                var unidadTributaria = this.GetContexto().UnidadesTributariasTemporal.Find(idUT, tramite.IdTramite);
                #endregion
                #region Datos Generales
                ddjjTemporal.IdPoligono = ddjjJObject["IdPoligono"].ToString();
                try
                {
                    try
                    {
                        ddjjTemporal.FechaVigencia = ddjjJObject.Value<DateTime>("FechaVigencia");
                    }
                    catch
                    {
                        ddjjTemporal.FechaVigencia = DateTime.Parse(ddjjJObject.Value<string>("FechaVigencia"), System.Globalization.CultureInfo.CurrentCulture);
                    }
                }
                catch (Exception)
                {
                    GetContexto().GetLogger().LogError("Fecha de Vigencia DDJJ Incorrecta", new Exception($"valor:{ddjjJObject.Value<string>("FechaVigencia")}"));
                    throw;
                }
                ddjjTemporal.UsuarioModif = usuarioAlta.Id_Usuario;
                ddjjTemporal.FechaModif = DateTime.Now;
                #endregion
                #region DDJJ Tierra
                if (new[] { long.Parse(VersionesDDJJ.U), long.Parse(VersionesDDJJ.SoR) }.Contains(ddjjTemporal.IdVersion))
                {
                    decimal superficieDDJJ = 0m;

                    #region DDJJ U
                    if (long.Parse(VersionesDDJJ.U) == ddjjTemporal.IdVersion)
                    {
                        if (esNuevaDDJJ)
                        {
                            ddjjTemporal.U = new DDJJUTemporal()
                            {
                                IdTramite = ddjjTemporal.IdTramite,

                                Fracciones = new List<DDJJUFraccionTemporal>(),

                                UsuarioAlta = ddjjTemporal.UsuarioModif,
                                FechaAlta = ddjjTemporal.FechaModif
                            };
                        }
                        else
                        {
                            this.GetContexto().Entry(ddjjTemporal)
                                .Reference(x => x.U).Query()
                                .Include(x => x.Fracciones.Select(f => f.MedidasLineales.Select(ml => ml.ClaseParcelaMedidaLineal)))
                                .Load();
                        }

                        #region Datos Generales U
                        var u = mainJObject["DDJJU"].ToObject<DDJJU>();

                        superficieDDJJ = u.SuperficiePlano ?? u.SuperficieTitulo ?? 0m;

                        ddjjTemporal.U.AguaCorriente = u.AguaCorriente;
                        ddjjTemporal.U.Cloaca = u.Cloaca;
                        ddjjTemporal.U.NumeroHabitantes = u.NumeroHabitantes;
                        ddjjTemporal.U.SuperficiePlano = u.SuperficiePlano;
                        ddjjTemporal.U.SuperficieTitulo = u.SuperficieTitulo;

                        ddjjTemporal.U.UsuarioModif = ddjjTemporal.UsuarioModif;
                        ddjjTemporal.U.FechaModif = ddjjTemporal.FechaModif;

                        string imagenBase64 = mainJObject["CroquisBase64"].ToString();
                        if (!string.IsNullOrEmpty(imagenBase64))
                        {
                            ddjjTemporal.U.Croquis = Convert.FromBase64String(imagenBase64.Substring(imagenBase64.IndexOf(",") + 1));
                        }
                        #endregion
                        #region Fracciones
                        long nroFraccion = 1;
                        var fraccionesFinales = JArray.Parse(mainJObject["ClasesJsonSerialized"].Value<string>()).Select(c => c.ToObject<Web.Api.Models.ClaseParcela>());
                        foreach (var fraccionFinal in fraccionesFinales)
                        {
                            var fraccion = ddjjTemporal.U.Fracciones.FirstOrDefault(f => f.MedidasLineales.Any(ml => ml.ClaseParcelaMedidaLineal.IdClaseParcela == fraccionFinal.IdClaseParcela));
                            if (fraccion == null)
                            {
                                fraccion = new DDJJUFraccionTemporal()
                                {
                                    IdTramite = tramite.IdTramite,
                                    MedidasLineales = new List<DDJJUMedidaLinealTemporal>(),

                                    UsuarioAlta = ddjjTemporal.UsuarioAlta,
                                    FechaAlta = ddjjTemporal.FechaModif
                                };
                                ddjjTemporal.U.Fracciones.Add(fraccion);
                            }
                            fraccion.NumeroFraccion = nroFraccion++;

                            fraccion.UsuarioModif = ddjjTemporal.UsuarioModif;
                            fraccion.FechaModif = ddjjTemporal.FechaModif;

                            foreach (var tml in fraccionFinal.TiposMedidasLineales)
                            {
                                var ml = fraccion.MedidasLineales.SingleOrDefault(x => x.IdClaseParcelaMedidaLineal == tml.IdClasesParcelasMedidaLineal);
                                if (ml == null)
                                {
                                    ml = new DDJJUMedidaLinealTemporal()
                                    {
                                        IdTramite = tramite.IdTramite,
                                        IdClaseParcelaMedidaLineal = tml.IdClasesParcelasMedidaLineal,

                                        UsuarioAlta = ddjjTemporal.UsuarioModif,
                                        FechaAlta = ddjjTemporal.FechaModif,
                                    };
                                    fraccion.MedidasLineales.Add(ml);
                                }
                                ml.AlturaCalle = long.TryParse(tml.Altura, out long altura) ? (long?)altura : null;
                                ml.Calle = tml.Calle;
                                ml.IdTramoVia = tml.IdTramoVia;
                                ml.IdVia = tml.IdVia;
                                ml.ValorAforo = tml.ValorAforo;
                                ml.ValorMetros = tml.ValorMetros;

                                ml.UsuarioModif = ddjjTemporal.UsuarioModif;
                                ml.FechaModif = ddjjTemporal.FechaModif;
                            }
                        }
                        #endregion
                    }
                    #endregion
                    #region DDJJ SoR
                    else if (long.Parse(VersionesDDJJ.SoR) == ddjjTemporal.IdVersion)
                    {
                        if (esNuevaDDJJ)
                        {
                            ddjjTemporal.Sor = new DDJJSorTemporal()
                            {
                                IdTramite = ddjjTemporal.IdTramite,

                                Superficies = new List<VALSuperficieTemporal>(),
                                SoRCars = new List<DDJJSorCarTemporal>(),

                                UsuarioAlta = ddjjTemporal.UsuarioModif,
                                FechaAlta = ddjjTemporal.FechaModif
                            };
                        }
                        else
                        {
                            this.GetContexto().Entry(ddjjTemporal)
                                .Reference(x => x.Sor).Query()
                                .Include(x => x.SoRCars)
                                .Include(x => x.Superficies).Load();
                        }
                        /*
                            sorcar,
                            superficies,
                         */
                        #region Datos Generales SoR
                        var sor = mainJObject["DDJJSor"].ToObject<DDJJSor>();
                        ddjjTemporal.Sor.DistanciaCamino = sor.DistanciaCamino;
                        ddjjTemporal.Sor.DistanciaEmbarque = sor.DistanciaEmbarque;
                        ddjjTemporal.Sor.DistanciaLocalidad = sor.DistanciaLocalidad;
                        ddjjTemporal.Sor.IdCamino = sor.IdCamino;
                        ddjjTemporal.Sor.IdLocalidad = sor.IdLocalidad;
                        ddjjTemporal.Sor.NumeroHabitantes = sor.NumeroHabitantes;

                        ddjjTemporal.Sor.UsuarioModif = ddjjTemporal.UsuarioModif;
                        ddjjTemporal.Sor.FechaModif = ddjjTemporal.FechaModif;
                        #endregion
                        #region Características SoR
                        var aptCars = this.GetContexto().VALAptCar.Where(ac => ac.FechaBaja == null).ToArray();
                        var caracteristicas = new[] //ascoooooo, pero es lo que hay....
                        {
                            "AguasDelSubsueloSeleccionado",
                            "CapacidadesGanaderasSeleccionado",
                            "ColoresTierraSeleccionado",
                            "EspesoresCapaArableSeleccionado",
                            "EstadosMonteSeleccionado",
                            "RelieveSeleccionado"
                        };
                        foreach (var aptitud in mainJObject["AptitudesDisponibles"].ToArray())
                        {
                            long idAptitud = aptitud["IdAptitud"].Value<long>();
                            if (double.TryParse(aptitud["Superficie"].ToString(), out double valSuperficie) && valSuperficie > 0d)
                            {
                                #region SorCars
                                foreach (var caracteristica in caracteristicas.Where(c => aptitud.Children<JProperty>().Any(p => p.Name == c)))
                                {
                                    if (!long.TryParse(aptitud[caracteristica].Value<string>(), out long idSorCar))
                                    {
                                        continue;
                                    }

                                    long idAptCar = aptCars.Single(a => a.IdAptitud == idAptitud && a.IdSorCar == idSorCar).IdAptCar;
                                    var sorCar = ddjjTemporal.Sor.SoRCars.SingleOrDefault(sc => sc.IdAptCar == idAptCar);
                                    if (sorCar == null)
                                    {
                                        sorCar = new DDJJSorCarTemporal()
                                        {
                                            IdTramite = tramite.IdTramite,
                                            IdAptCar = idAptCar,
                                            UsuarioAlta = tramite.UsuarioModif,
                                            FechaAlta = tramite.FechaModif,
                                        };
                                        ddjjTemporal.Sor.SoRCars.Add(sorCar);
                                    }
                                    sorCar.UsuarioModif = tramite.UsuarioModif;
                                    sorCar.FechaModif = tramite.FechaModif;
                                }
                                #endregion
                                #region Superficies
                                superficieDDJJ += Convert.ToDecimal(valSuperficie);

                                var superficie = ddjjTemporal.Sor.Superficies.SingleOrDefault(sc => sc.IdAptitud == idAptitud);
                                if (superficie == null)
                                {
                                    superficie = new VALSuperficieTemporal()
                                    {
                                        IdTramite = tramite.IdTramite,
                                        IdAptitud = idAptitud,

                                        UsuarioAlta = tramite.UsuarioModif,
                                        FechaAlta = tramite.FechaModif
                                    };
                                    ddjjTemporal.Sor.Superficies.Add(superficie);
                                }
                                superficie.Superficie = valSuperficie;

                                superficie.UsuarioModif = tramite.UsuarioModif;
                                superficie.FechaModif = tramite.FechaModif;
                                #endregion
                            }
                        }
                        foreach (var obj in ddjjTemporal.Sor.SoRCars.Cast<object>().Concat(ddjjTemporal.Sor.Superficies).Where(e => this.GetContexto().Entry(e).State == EntityState.Unchanged).ToArray())
                        {
                            this.GetContexto().Entry(obj).State = EntityState.Deleted;
                        }
                        #endregion
                    }
                    #endregion
                    #region Designacion
                    var designacion = mainJObject["DDJJDesignacion"].ToObject<DDJJDesignacion>();
                    if (esNuevaDDJJ)
                    {
                        ddjjTemporal.Designacion = new DDJJDesignacionTemporal()
                        {
                            IdTramite = ddjjTemporal.IdTramite,
                            IdTipoDesignador = (short)TipoDesignadorEnum.Catastro,

                            UsuarioAlta = ddjjTemporal.UsuarioModif,
                            FechaAlta = ddjjTemporal.FechaModif
                        };
                    }
                    else
                    {
                        this.GetContexto().Entry(ddjjTemporal).Reference(x => x.Designacion).Load();
                    }
                    ddjjTemporal.Designacion.Barrio = designacion.Barrio;
                    ddjjTemporal.Designacion.Calle = designacion.Calle;
                    ddjjTemporal.Designacion.Chacra = designacion.Chacra;
                    ddjjTemporal.Designacion.CodigoPostal = designacion.CodigoPostal;
                    ddjjTemporal.Designacion.Departamento = designacion.Departamento;
                    ddjjTemporal.Designacion.Fraccion = designacion.Fraccion;
                    ddjjTemporal.Designacion.IdBarrio = designacion.IdBarrio;
                    ddjjTemporal.Designacion.IdCalle = designacion.IdCalle;
                    ddjjTemporal.Designacion.IdDepartamento = designacion.IdDepartamento;
                    ddjjTemporal.Designacion.IdLocalidad = designacion.IdLocalidad;
                    ddjjTemporal.Designacion.IdManzana = designacion.IdManzana;
                    ddjjTemporal.Designacion.IdParaje = designacion.IdParaje;
                    ddjjTemporal.Designacion.IdSeccion = designacion.IdSeccion;
                    ddjjTemporal.Designacion.Localidad = designacion.Localidad;
                    ddjjTemporal.Designacion.Lote = designacion.Lote;
                    ddjjTemporal.Designacion.Manzana = designacion.Manzana;
                    ddjjTemporal.Designacion.Numero = designacion.Numero;
                    ddjjTemporal.Designacion.Paraje = designacion.Paraje;
                    ddjjTemporal.Designacion.Quinta = designacion.Quinta;
                    ddjjTemporal.Designacion.Seccion = designacion.Seccion;

                    ddjjTemporal.Designacion.UsuarioModif = ddjjTemporal.UsuarioModif;
                    ddjjTemporal.Designacion.FechaModif = ddjjTemporal.FechaModif;
                    #endregion
                    #region Dominios
                    var dominiosFinales = JArray.Parse(mainJObject["dominiosJSON"].Value<string>()).Select(d => d.ToObject<DDJJDominio>());

                    if (esNuevaDDJJ)
                    {
                        ddjjTemporal.Dominios = new List<DDJJDominioTemporal>();
                    }
                    else
                    {
                        this.GetContexto().Entry(ddjjTemporal)
                            .Collection(x => x.Dominios).Query()
                            .Include(x => x.Titulares)
                            .Include(x => x.Titulares.Select(t => t.PersonaDomicilios))
                            .Load();

                        this.GetContexto().DeclaracionesJuradasDominiosTemporal.RemoveRange(ddjjTemporal.Dominios.Where(d => !dominiosFinales.Any(f => f.IdDominio == d.IdDominio)));
                    }

                    foreach (var dominioFinal in dominiosFinales)
                    {
                        var dominio = ddjjTemporal.Dominios.SingleOrDefault(d => d.IdDominio == dominioFinal.IdDominio);
                        if (dominio == null)
                        {
                            dominio = new DDJJDominioTemporal()
                            {
                                IdTramite = ddjjTemporal.IdTramite,
                                Titulares = new List<DDJJDominioTitularTemporal>(),

                                UsuarioAlta = ddjjTemporal.UsuarioModif,
                                FechaAlta = ddjjTemporal.FechaModif,
                            };
                            ddjjTemporal.Dominios.Add(dominio);
                        }
                        else
                        {
                            this.GetContexto().DeclaracionesJuradasDominiosTitularesTemporal
                                .RemoveRange(dominio.Titulares
                                                    .Where(t => !dominioFinal.Titulares.Any(f => f.IdPersona == t.IdPersona)));
                        }
                        dominio.Fecha = dominioFinal.Fecha;
                        dominio.IdTipoInscripcion = dominioFinal.IdTipoInscripcion;
                        dominio.Inscripcion = dominioFinal.Inscripcion;

                        #region Titulares
                        foreach (var titularFinal in dominioFinal.Titulares)
                        {
                            var titular = dominio.Titulares.SingleOrDefault(d => d.IdPersona == titularFinal.IdPersona);
                            if (titular == null)
                            {
                                titular = new DDJJDominioTitularTemporal()
                                {
                                    IdTramite = ddjjTemporal.IdTramite,

                                    UsuarioAlta = ddjjTemporal.UsuarioModif,
                                    FechaAlta = ddjjTemporal.FechaModif,
                                };
                                dominio.Titulares.Add(titular);
                            }
                            else
                            {
                                this.GetContexto().DeclaracionesJuradasPersonasDomiciliosTemporal
                                    .RemoveRange(titular.PersonaDomicilios
                                                        .Where(d => !titularFinal.PersonaDomicilio.Any(f => f.IdDominioTitular == d.IdDominioTitular && f.IdDomicilio == d.IdDomicilio)));
                            }
                            titular.PersonaDomicilios = titular.PersonaDomicilios ?? new List<DDJJPersonaDomicilioTemporal>();

                            titular.IdPersona = titularFinal.IdPersona;
                            titular.IdTipoTitularidad = titularFinal.IdTipoTitularidad;
                            titular.PorcientoCopropiedad = titularFinal.PorcientoCopropiedad;

                            titular.UsuarioModif = ddjjTemporal.UsuarioModif;
                            titular.FechaModif = ddjjTemporal.FechaModif;

                            foreach (var personaDomFinal in titularFinal.PersonaDomicilio)
                            {
                                var personaDomicilio = titular.PersonaDomicilios.SingleOrDefault(pd => pd.IdDomicilio == personaDomFinal.IdDomicilio);

                                if (personaDomicilio == null)
                                {
                                    personaDomicilio = new DDJJPersonaDomicilioTemporal()
                                    {
                                        IdTramite = ddjjTemporal.IdTramite,

                                        UsuarioAlta = ddjjTemporal.UsuarioModif,
                                        FechaAlta = ddjjTemporal.FechaModif,
                                    };

                                    if (personaDomFinal.IdDomicilio <= 0)
                                    {
                                        personaDomicilio.Domicilio = personaDomFinal.Domicilio;
                                        personaDomicilio.Domicilio.TipoDomicilio = null;

                                        personaDomicilio.Domicilio.ViaId = personaDomicilio.Domicilio.ViaId == 0 ? null : personaDomicilio.Domicilio.ViaId;
                                        personaDomicilio.Domicilio.IdLocalidad = personaDomicilio.Domicilio.IdLocalidad == 0 ? null : personaDomicilio.Domicilio.IdLocalidad;
                                        personaDomicilio.Domicilio.UsuarioAltaId = ddjjTemporal.UsuarioModif;
                                        personaDomicilio.Domicilio.FechaAlta = ddjjTemporal.FechaModif;
                                        personaDomicilio.Domicilio.UsuarioModifId = ddjjTemporal.UsuarioModif;
                                        personaDomicilio.Domicilio.FechaModif = ddjjTemporal.FechaModif;
                                    }
                                    titular.PersonaDomicilios.Add(personaDomicilio);
                                }
                                personaDomicilio.IdDomicilio = personaDomFinal.IdDomicilio;
                                personaDomicilio.IdTipoDomicilio = personaDomFinal.IdTipoDomicilio;

                                personaDomicilio.UsuarioModif = ddjjTemporal.UsuarioModif;
                                personaDomicilio.FechaModif = ddjjTemporal.FechaModif;
                            }
                        }
                        #endregion

                        dominio.UsuarioModif = ddjjTemporal.UsuarioModif;
                        dominio.FechaModif = ddjjTemporal.FechaModif;
                    }
                    #endregion
                    #region Actualizacion Superficie de Parcela
                    var parcela = (from ut in GetContexto().UnidadesTributariasTemporal
                                   join par in GetContexto().ParcelasTemporal on ut.ParcelaID equals par.ParcelaID
                                   where ut.UnidadTributariaId == idUT && ut.IdTramite == tramite.IdTramite && par.IdTramite == tramite.IdTramite
                                   select par).Single();


                    if ((TipoUnidadTributariaEnum)unidadTributaria.TipoUnidadTributariaID == TipoUnidadTributariaEnum.Comun)
                    {
                        unidadTributaria.Superficie = (double)superficieDDJJ;
                    }

                    parcela.Superficie = superficieDDJJ;
                    #endregion
                }
                #endregion
                #region DDJJ de Mejoras
                else if (new[] { long.Parse(VersionesDDJJ.E1), long.Parse(VersionesDDJJ.E2) }.Contains(ddjjTemporal.IdVersion))
                {
                    if (esNuevaDDJJ)
                    {
                        ddjjTemporal.Mejora = new INMMejoraTemporal()
                        {
                            IdTramite = ddjjTemporal.IdTramite,

                            MejorasCar = new List<INMMejoraCaracteristicaTemporal>(),
                            OtrasCar = new List<INMMejoraOtraCarTemporal>(),

                            UsuarioAlta = ddjjTemporal.UsuarioModif,
                            FechaAlta = ddjjTemporal.FechaModif
                        };
                    }
                    else
                    {
                        this.GetContexto().Entry(ddjjTemporal)
                            .Reference(x => x.Mejora).Query()
                            .Include(m => m.OtrasCar)
                            .Include(m => m.MejorasCar).Load();
                    }
                    #region Datos Generales Mejora
                    var mejora = mainJObject["Mejora"].ToObject<INMMejora>();
                    ddjjTemporal.Mejora.IdDestinoMejora = mejora.IdDestinoMejora;
                    ddjjTemporal.Mejora.IdEstadoConservacion = mejora.IdEstadoConservacion;

                    ddjjTemporal.Mejora.UsuarioModif = ddjjTemporal.Mejora.UsuarioModif;
                    ddjjTemporal.Mejora.FechaModif = ddjjTemporal.Mejora.FechaModif;
                    #endregion
                    #region Características Mejoras
                    var caracteristicasFinales = ((JArray)mainJObject["CaracteristicasToSave"])?.Select(c => c.Value<long>()) ?? new long[0];

                    foreach (var caracteristicaEliminada in ddjjTemporal.Mejora.MejorasCar.Where(mc => !caracteristicasFinales.Contains(mc.IdCaracteristica)).ToArray())
                    {
                        this.GetContexto().INMMejorasCaracteristicasTemporal.Remove(caracteristicaEliminada);
                    }
                    foreach (long caracteristicaNueva in caracteristicasFinales.Where(c => !ddjjTemporal.Mejora.MejorasCar.Any(mc => mc.IdCaracteristica == c)))
                    {
                        ddjjTemporal.Mejora.MejorasCar.Add(new INMMejoraCaracteristicaTemporal()
                        {
                            IdTramite = ddjjTemporal.IdTramite,
                            IdCaracteristica = caracteristicaNueva,
                            UsuarioAlta = ddjjTemporal.UsuarioModif,
                            FechaAlta = ddjjTemporal.FechaModif,
                            UsuarioModif = ddjjTemporal.UsuarioModif,
                            FechaModif = ddjjTemporal.FechaModif
                        });
                    }
                    #endregion
                    #region Otras Caracteristicas
                    decimal valor = 0m;
                    var otrasCarFinales = ((JArray)mainJObject["OtrasCar"]).Select(c => c.ToObject<INMMejoraOtraCar>());
                    foreach (var otraCarFinal in otrasCarFinales)
                    {
                        var otraCar = ddjjTemporal.Mejora.OtrasCar.SingleOrDefault(oc => oc.IdOtraCar == otraCarFinal.IdOtraCar);
                        if (otraCar == null)
                        {
                            otraCar = new INMMejoraOtraCarTemporal()
                            {
                                IdTramite = ddjjTemporal.IdTramite,
                                IdOtraCar = otraCarFinal.IdOtraCar,

                                UsuarioAlta = ddjjTemporal.UsuarioModif,
                                FechaAlta = ddjjTemporal.FechaModif,
                            };
                            ddjjTemporal.Mejora.OtrasCar.Add(otraCar);
                        }
                        otraCar.Valor = decimal.TryParse(otraCarFinal.Valor?.ToString(), out valor) ? (decimal?)valor : null;
                        otraCar.UsuarioModif = ddjjTemporal.UsuarioModif;
                        otraCar.FechaModif = ddjjTemporal.FechaModif;
                    }
                    #endregion
                }
                #endregion
                #region DDJJ Incorrecta
                else
                {
                    throw new InvalidOperationException($"El tipo de DDJJ no es correcto");
                }
                #endregion
                #endregion

                this.GetContexto().SaveChanges();

                return ddjjTemporal.IdDeclaracionJurada;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.Manzana))
            {
                var plano = item.Propiedades.FirstOrDefault(t => t.Id == "Plano")?.Value;

                DivisionTemporal obj;

                if (tramiteEntrada == null) //Alta Manzana
                {
                    obj = new DivisionTemporal
                    {
                        TipoDivisionId = 0,
                        IdTramite = tramite.IdTramite,
                        Nombre = string.Empty,
                        Descripcion = string.Empty,
                        ObjetoPadreId = tramite.IdLocalidad.Value,//de acuerdo al ID de objeto padre,
                        PlanoId = plano, //de acuerdo al valor en el campo “ID s / Plano”
                        UsuarioAltaId = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        UsuarioModificacionId = usuarioAlta.Id_Usuario,
                        FechaModificacion = DateTime.Now
                    };

                    this.GetContexto().DivisionesTemporal.Add(obj);
                }
                else //Edicion manzana
                {
                    obj = this.GetContexto().DivisionesTemporal.FirstOrDefault(t => t.FeatId == tramiteEntrada.IdObjeto);
                    obj.TipoDivisionId = 0;
                    obj.Nombre = string.Empty;
                    obj.Descripcion = string.Empty;
                    //obj.ObjetoPadreId = //de acuerdo al ID de objeto padre,
                    obj.PlanoId = plano; //de acuerdo al valor en el campo “ID s / Plano”
                    obj.UsuarioModificacionId = usuarioAlta.Id_Usuario;
                    obj.FechaModificacion = DateTime.Now;
                }

                this.GetContexto().SaveChanges();

                return obj.FeatId;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.Persona))
            {
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));

                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                bool personaExistente = long.TryParse(item.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPersona")?.Value, out long idPersona);

                if (personaExistente)
                {
                    return idPersona;
                }
                else
                {
                    throw new InvalidOperationException($"La persona no existe");
                }
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.Via))
            {
                var tipo = item.Propiedades.FirstOrDefault(t => t.Id == "Tipo")?.Value;
                var plano = item.Propiedades.FirstOrDefault(t => t.Id == "Plano")?.Value;

                Via obj;

                if (tramiteEntrada == null) //Alta Via
                {
                    obj = new Via
                    {
                        TipoViaId = Convert.ToInt32(tipo), //de acuerdo al valor elegido en el campo “Tipo”,
                        Nombre = " ",
                        PlanoId = plano, // de acuerdo al valor ingresado en el campo “ID s / Plano”,
                        IdUsuarioAlta = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        IdUsuarioModif = usuarioAlta.Id_Usuario,
                        FechaModif = DateTime.Now
                    };

                    this.GetContexto().Via.Add(obj);
                }
                else //Edicion Via
                {
                    obj = this.GetContexto().Via.FirstOrDefault(t => t.ViaId == tramiteEntrada.IdObjeto);

                    obj.TipoViaId = Convert.ToInt32(tipo); //de acuerdo al valor elegido en el campo “Tipo”,
                    obj.Nombre = " ";
                    obj.PlanoId = plano; // de acuerdo al valor ingresado en el campo “ID s / Plano”
                    obj.IdUsuarioModif = usuarioAlta.Id_Usuario;
                    obj.FechaModif = DateTime.Now;
                }

                this.GetContexto().SaveChanges();

                return obj.ViaId;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.LibreDeuda))
            {
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));

                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                var tipoObjetoPadre = this.GetContexto().Entradas.FirstOrDefault(t => t.IdEntrada == objPadre.TipoObjeto.Id);

                if (!new[] { int.Parse(Entradas.Parcela), int.Parse(Entradas.UnidadTributaria) }.Contains(tipoObjetoPadre.IdEntrada))
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre de tipo Parcela o Unidad funcional");
                }

                var fechaEmision = item.Propiedades.FirstOrDefault(t => t.Id == "fecha-emision")?.Value;
                var fechaVigencia = item.Propiedades.FirstOrDefault(t => t.Id == "fecha-vigencia")?.Value;
                var enteEmisor = item.Propiedades.FirstOrDefault(t => t.Id == "EnteEmisor")?.Value;
                var nroCertificado = item.Propiedades.FirstOrDefault(t => t.Id == "NroCertificado")?.Value;
                var superficie = item.Propiedades.FirstOrDefault(t => t.Id == "Superficie")?.Value;
                var valuacion = item.Propiedades.FirstOrDefault(t => t.Id == "Valuacion")?.Value;
                var deuda = item.Propiedades.FirstOrDefault(t => t.Id == "Deuda")?.Value;



                UnidadTributariaTemporal unidadTributariaPadre;
                if (objPadre.Propiedades.Any(t => t.Id == "hdnIdPartidaPersona"))
                {//Si el padre es parcela, la busco de esta forma
                    long hdnIdPartidaPersona = Convert.ToInt64(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona").Value);
                    unidadTributariaPadre = this.GetContexto().UnidadesTributariasTemporal.FirstOrDefault(t => t.ParcelaID == hdnIdPartidaPersona && t.IdTramite == tramite.IdTramite);
                }
                else
                {//el padre es un UT
                    long hdnIdUnidadFuncional = Convert.ToInt64(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdUnidadFuncional").Value);
                    unidadTributariaPadre = this.GetContexto().UnidadesTributariasTemporal.Find(hdnIdUnidadFuncional, tramite.IdTramite);
                }

                INMLibreDeDeudaTemporal obj = this.GetContexto().INMLibresDeDeudasTemporal.FirstOrDefault(x => x.IdTramite == tramite.IdTramite && x.IdUnidadTributaria == unidadTributariaPadre.UnidadTributariaId);
                if (tramiteEntrada == null || obj == null) //Alta libre de deuda
                {
                    obj = new INMLibreDeDeudaTemporal
                    {
                        IdTramite = tramite.IdTramite,
                        IdUsuarioAlta = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                    };

                    this.GetContexto().INMLibresDeDeudasTemporal.Add(obj);
                }

                obj.IdUnidadTributaria = unidadTributariaPadre.UnidadTributariaId; //de acuerdo al ID de la Unidad Tributaria de la parcela o UF padre(debe haber una)
                obj.FechaEmision = Convert.ToDateTime(fechaEmision);
                obj.FechaVigencia = Convert.ToDateTime(fechaVigencia);
                obj.IdEnteEmisor = Convert.ToInt32(enteEmisor);
                obj.NroCertificado = nroCertificado;
                obj.Superficie = Convert.ToDecimal(superficie); //obtenido por interfaz
                obj.Valuacion = Convert.ToDecimal(valuacion); //obtenido por interfaz
                obj.Deuda = Convert.ToDecimal(deuda); //obtenido por interfaz
                obj.IdUsuarioModif = usuarioAlta.Id_Usuario;
                obj.FechaModif = DateTime.Now;

                this.GetContexto().SaveChanges();

                return obj.IdLibreDeuda;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.Titulo))
            {
                //var objPadre = arbolObjetos.FirstOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));
                //UnidadTributaria unidadTributariaPadre = null;
                UnidadTributariaTemporal unidadTributariaPadre = null;
                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                if (objPadre.TipoObjeto.Id == Convert.ToInt32(Entradas.Parcela))
                {
                    //Si la unidad tributaria padre es parcela, la busco de esta forma
                    var hdnIdPartidaPersona = Convert.ToInt32(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona").Value);
                    unidadTributariaPadre = this.GetContexto().UnidadesTributariasTemporal.FirstOrDefault(t => t.ParcelaID == hdnIdPartidaPersona);
                }
                else
                if (objPadre.TipoObjeto.Id == Convert.ToInt32(Entradas.UnidadTributaria))
                {
                    var hdnIdUnidadTributaria = Convert.ToInt32(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdUnidadFuncional").Value);
                    unidadTributariaPadre = this.GetContexto().UnidadesTributariasTemporal.FirstOrDefault(t => t.UnidadTributariaId == hdnIdUnidadTributaria);
                }
                else
                {
                    throw new ArgumentException("Tipo de Dato no valido");
                }

                long idDominio = tramiteEntrada?.IdObjeto ?? long.MinValue;
                var obj = this.GetContexto()
                                .DominiosTemporal
                                .Include(d => d.Titulares).FirstOrDefault(t => t.IdTramite == tramite.IdTramite && t.DominioID == idDominio);

                if (tramiteEntrada == null || obj == null) //Alta titulo
                {
                    obj = this.GetContexto().DominiosTemporal.Add(new DominioTemporal
                    {
                        IdTramite = tramite.IdTramite,
                        IdUsuarioAlta = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        Titulares = new List<DominioTitularTemporal>()
                    });
                }
                obj.Inscripcion = item.Propiedades.FirstOrDefault(t => t.Id == "Inscripcion")?.Value;
                obj.TipoInscripcionID = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "Tipo")?.Value);
                obj.Fecha = Convert.ToDateTime(item.Propiedades.FirstOrDefault(t => t.Id == "fecha")?.Value);
                obj.UnidadTributariaID = unidadTributariaPadre.UnidadTributariaId;
                obj.IdUsuarioModif = usuarioAlta.Id_Usuario;
                obj.FechaModif = DateTime.Now;

                var titulares = arbolObjetos.Where(o => o.ParentGuids.Contains(item.Guid) && o.TipoObjeto.Id == int.Parse(Entradas.Persona))
                                .Select(t => new
                                {
                                    idPersona = Convert.ToInt64(t.Propiedades.FirstOrDefault(a => a.Id == "hdnIdPersona")?.Value),
                                    tipoTitularidad = Convert.ToInt16(t.Propiedades.FirstOrDefault(a => a.Id == "TipoTitularidad")?.Value),
                                    porcentajeTitularidad = Convert.ToDecimal(t.Propiedades.FirstOrDefault(a => a.Id == "Titularidad")?.Value)
                                });

                foreach (var titular in titulares)
                {
                    var dominioTitular = obj.Titulares.SingleOrDefault(t => t.PersonaID == titular.idPersona);

                    if (dominioTitular == null)
                    {
                        dominioTitular = new DominioTitularTemporal()
                        {
                            IdTramite = tramite.IdTramite,
                            PersonaID = titular.idPersona,
                            FechaAlta = DateTime.Now,
                            UsuarioAltaID = tramite.UsuarioModif

                        };

                        obj.Titulares.Add(dominioTitular);

                    }

                    dominioTitular.PorcientoCopropiedad = titular.porcentajeTitularidad;
                    dominioTitular.TipoTitularidadID = titular.tipoTitularidad;
                    dominioTitular.FechaModificacion = DateTime.Now;
                    dominioTitular.UsuarioModificacionID = tramite.UsuarioModif;

                }

                var borrados = obj.Titulares.Where(o => !titulares.Any(t => t.idPersona == o.PersonaID)).ToList();

                for (int i = 0; i < borrados.Count; i++)
                {
                    this.GetContexto().Entry(borrados[i]).State = EntityState.Deleted;
                }

                this.GetContexto().SaveChanges();

                return obj.DominioID;
            }
            else
            if (entrada.IdEntrada == Convert.ToInt32(Entradas.ComprobantePago))
            {
                var tipoTasa = item.Propiedades.FirstOrDefault(t => t.Id == "TipoTasa")?.Value;
                var identificacion = item.Propiedades.FirstOrDefault(t => t.Id == "Identificacion")?.Value;
                var tipoTramite = item.Propiedades.FirstOrDefault(t => t.Id == "TipoTramite")?.Value;
                var fechaVencimiento = item.Propiedades.FirstOrDefault(t => t.Id == "fecha-vencimiento")?.Value;
                var fechaLiquidacion = item.Propiedades.FirstOrDefault(t => t.Id == "fecha-liquidacion")?.Value;
                var fechaPago = item.Propiedades.FirstOrDefault(t => t.Id == "fecha-pago")?.Value;
                var medioPago = item.Propiedades.FirstOrDefault(t => t.Id == "MedioPago")?.Value;
                var importe = item.Propiedades.FirstOrDefault(t => t.Id == "Importe")?.Value;
                var estadoPago = item.Propiedades.FirstOrDefault(t => t.Id == "EstadoPago")?.Value;

                MEComprobantePago obj;

                if (tramiteEntrada == null) //Alta comprobante de pago
                {
                    obj = new MEComprobantePago
                    {
                        IdTipoTasa = Convert.ToInt32(tipoTasa),
                        IdTramite = Convert.ToInt64(identificacion),
                        TipoTramiteDgr = tipoTramite,
                        FechaVencimiento = Convert.ToDateTime(fechaVencimiento),
                        FechaLiquidacion = Convert.ToDateTime(fechaLiquidacion),
                        FechaPago = Convert.ToDateTime(fechaPago),
                        MedioPago = medioPago,
                        Importe = Convert.ToDouble(importe),
                        EstadoPago = estadoPago,
                        IdUsuarioAlta = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                        IdUsuarioModif = usuarioAlta.Id_Usuario,
                        FechaModif = DateTime.Now
                    };

                    this.GetContexto().ComprobantePago.Add(obj);
                }
                else //Edicion comprobante de pago
                {
                    obj = this.GetContexto().ComprobantePago.FirstOrDefault(t => t.IdComprobantePago == tramiteEntrada.IdObjeto);

                    obj.IdTipoTasa = Convert.ToInt32(tipoTasa);
                    obj.IdTramite = Convert.ToInt64(identificacion);
                    obj.TipoTramiteDgr = tipoTramite;
                    obj.FechaVencimiento = Convert.ToDateTime(fechaVencimiento);
                    obj.FechaLiquidacion = Convert.ToDateTime(fechaLiquidacion);
                    obj.FechaPago = Convert.ToDateTime(fechaPago);
                    obj.MedioPago = medioPago;
                    obj.Importe = Convert.ToDouble(importe);
                    obj.EstadoPago = estadoPago;
                    obj.IdUsuarioModif = usuarioAlta.Id_Usuario;
                    obj.FechaModif = DateTime.Now;
                }

                this.GetContexto().SaveChanges();

                return obj.IdComprobantePago;
            }
            else
             if (entrada.IdEntrada == Convert.ToInt32(Entradas.EspacioPublico))
            {
                var objPadre = arbolObjetos.LastOrDefault(t => item.ParentGuids.Any(j => j == t.Guid));

                if (objPadre == null)
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre");
                }

                var tipoObjetoPadre = this.GetContexto().Entradas.FirstOrDefault(t => t.IdEntrada == objPadre.TipoObjeto.Id);

                if (!new[] { int.Parse(Entradas.Parcela) }.Contains(tipoObjetoPadre.IdEntrada))
                {
                    throw new InvalidOperationException($"El item {entrada.Descripcion} debe tener un objeto padre de tipo Parcela");
                }

                var superficie = item.Propiedades.FirstOrDefault(t => t.Id == "Superficie")?.Value;

                ParcelaTemporal parcelaPadre;

                long hdnIdPartidaPersona = Convert.ToInt64(objPadre.Propiedades.FirstOrDefault(t => t.Id == "hdnIdPartidaPersona").Value);
                parcelaPadre = this.GetContexto().ParcelasTemporal.FirstOrDefault(t => t.ParcelaID == hdnIdPartidaPersona && t.IdTramite == tramite.IdTramite);

                long IdEspacioPublico = Convert.ToInt64(item.Propiedades.FirstOrDefault(t => t.Id == "IdEspacioPublico").Value);

                EspacioPublicoTemporal obj = this.GetContexto().EspaciosPublicosTemporal.FirstOrDefault(x => x.IdTramite == tramite.IdTramite && x.EspacioPublicoID == IdEspacioPublico);
                if (tramiteEntrada == null || obj == null) //Alta Espacio Público
                {
                    obj = new EspacioPublicoTemporal
                    {
                        IdTramite = tramite.IdTramite,
                        UsuarioAltaID = usuarioAlta.Id_Usuario,
                        FechaAlta = DateTime.Now,
                    };

                    this.GetContexto().EspaciosPublicosTemporal.Add(obj);
                }

                obj.Superficie = Convert.ToDecimal(superficie); //obtenido por interfaz
                obj.ParcelaID = parcelaPadre.ParcelaID;
                obj.UsuarioModificacionID = usuarioAlta.Id_Usuario;
                obj.FechaModificacion = DateTime.Now;

                this.GetContexto().SaveChanges();

                return obj.EspacioPublicoID;

            }
            else
            {
                throw new ArgumentException();
            }
        }

        public MensuraTemporal GetTramiteMensura(long IdTramite)
        {
            var ctx = this.GetContexto();
            var mensura = (from men in ctx.MensurasTemporal
                           where men.IdTramite == IdTramite && !(from men2 in ctx.Mensura
                                                                 where men.IdMensura == men2.IdMensura
                                                                 select men2).Any()
                           select men).FirstOrDefault();

            return mensura;
        }

        private void LimpiarRelacionesParcela(METramite tramite, long idParcela, Atributo campoClave, string[] tablas)
        {
            var query = from pmt in this.GetContexto().ParcelaMensurasTemporal
                        where pmt.IdParcela == idParcela && pmt.IdTramite == tramite.IdTramite
                        select pmt.IdMensura;

            if (query.Any())
            {
                long idCmp = long.Parse(this.GetContexto().ParametrosGenerales.FirstOrDefault(p => p.Clave == "ID_COMPONENTE_DESC_INMUEBLE").Valor);
                var cmp = this.GetContexto().Componente.Find(idCmp);
                string mensuras = string.Join(",", query.ToArray());
                foreach (string tablaMensura in new[] { "inm_parcela_mensura", cmp.TablaTemporal, "inm_mensura" })
                {
                    BorrarRegistrosTemporales(new Atributo() { Campo = "id_mensura" }, tablaMensura, mensuras, Common.Enums.SQLOperators.In, tramite.IdTramite);
                }

            }

            var queryLd = from ld in this.GetContexto().INMLibresDeDeudasTemporal
                          join ut in this.GetContexto().UnidadesTributariasTemporal on new { id = ld.IdUnidadTributaria, ld.IdTramite } equals new { id = ut.UnidadTributariaId, ut.IdTramite }
                          where ut.IdTramite == tramite.IdTramite && ut.ParcelaID == idParcela
                          select ld;

            var queryDi = from di in this.GetContexto().INMCertificadosCatastralesTemporal
                          join ut in this.GetContexto().UnidadesTributariasTemporal on new { id = di.UnidadTributariaId, di.IdTramite } equals new { id = ut.UnidadTributariaId, ut.IdTramite }
                          where ut.IdTramite == tramite.IdTramite && ut.ParcelaID == idParcela
                          select di;

            var queryDDJJ = from ddjj in this.GetContexto().DeclaracionesJuradasTemporal
                            join ut in this.GetContexto().UnidadesTributariasTemporal on new { id = ddjj.IdUnidadTributaria, ddjj.IdTramite } equals new { id = ut.UnidadTributariaId, ut.IdTramite }
                            where ut.IdTramite == tramite.IdTramite && ut.ParcelaID == idParcela
                            select ddjj;

            var queryDominios = from dominio in this.GetContexto().DominiosTemporal
                                join titular in this.GetContexto().DominiosTitularesTemporal on new { dominio.DominioID, dominio.IdTramite } equals new { titular.DominioID, titular.IdTramite }
                                join ut in this.GetContexto().UnidadesTributariasTemporal on new { id = dominio.UnidadTributariaID, dominio.IdTramite } equals new { id = ut.UnidadTributariaId, ut.IdTramite }
                                where ut.IdTramite == tramite.IdTramite && ut.ParcelaID == idParcela
                                group titular by dominio into gp
                                select new { dominio = gp.Key, titulares = gp };

            var queryDesignaciones = from designacion in this.GetContexto().DesignacionesTemporal
                                     where designacion.IdParcela == idParcela
                                     select designacion;

            var queryEspacioPublico = from espacioPublico in this.GetContexto().EspaciosPublicosTemporal
                                      where espacioPublico.ParcelaID == idParcela
                                      select espacioPublico;

            GetContexto().INMLibresDeDeudasTemporal.RemoveRange(queryLd);
            GetContexto().INMCertificadosCatastralesTemporal.RemoveRange(queryDi);
            GetContexto().DominiosTitularesTemporal.RemoveRange(queryDominios.SelectMany(g => g.titulares));
            GetContexto().DominiosTemporal.RemoveRange(queryDominios.Select(g => g.dominio));
            GetContexto().DesignacionesTemporal.RemoveRange(queryDesignaciones);
            GetContexto().EspaciosPublicosTemporal.RemoveRange(queryEspacioPublico);

            LimpiarDDJJs(queryDDJJ);
            GetContexto().SaveChanges();
            foreach (string tabla in tablas.Reverse())
            {
                BorrarRegistrosTemporales(campoClave, tabla, idParcela.ToString(), Common.Enums.SQLOperators.EqualsTo, tramite.IdTramite);
            }
        }

        private void LimpiarDDJJs(IQueryable<DDJJTemporal> query)
        {
            foreach (var ddjj in query.ToArray())
            {
                if (new[] { long.Parse(VersionesDDJJ.U), long.Parse(VersionesDDJJ.SoR) }.Contains(ddjj.IdVersion))
                {
                    this.GetContexto().Entry(ddjj).Reference(x => x.Designacion).Load();
                    this.GetContexto().Entry(ddjj).Collection(x => x.Dominios)
                        .Query().Include(d => d.Titulares)
                                .Include(d => d.Titulares.Select(t => t.PersonaDomicilios)).Load();
                    if (long.Parse(VersionesDDJJ.U) == ddjj.IdVersion)
                    {
                        this.GetContexto().Entry(ddjj).Reference(x => x.U).Query()
                                          .Include(x => x.Fracciones.Select(f => f.MedidasLineales))
                                          .Load();

                        this.GetContexto().DeclaracionesJuradasUTemporal.Remove(ddjj.U);
                    }
                    else
                    {
                        this.GetContexto().Entry(ddjj).Reference(x => x.Sor).Query()
                                          .Include(x => x.SoRCars)
                                          .Include(x => x.Superficies)
                                          .Load();

                        this.GetContexto().DeclaracionesJuradasSoRTemporal.Remove(ddjj.Sor);
                    }
                }
                else
                {
                    this.GetContexto().Entry(ddjj).Reference(x => x.Mejora).Query()
                        .Include(m => m.OtrasCar)
                        .Include(m => m.MejorasCar)
                        .Load();

                    this.GetContexto().MejorasTemporal.Remove(ddjj.Mejora);
                }
                this.GetContexto().DeclaracionesJuradasTemporal.Remove(ddjj);
            }
        }
    }

    internal static class XTMethodsMesaEntradas
    {
        internal static void AgregarTipoMovimientoByEvento(this List<METipoMovimiento> lista, EnumEvento evento)
        {
            int id = 0;
            string nombre = string.Empty;
            switch (evento)
            {
                case EnumEvento.DerivarTramite:
                    break;
                case EnumEvento.RecibirTramite:
                    break;
                case EnumEvento.ConfirmarTramite:
                    break;
                case EnumEvento.AnularCargaLibroTramite:
                    break;
                case EnumEvento.RechazarTramite:
                    break;
                case EnumEvento.FinalizarTramite:
                    break;
                case EnumEvento.AnularTramite:
                    break;
                case EnumEvento.DespacharTramite:
                    break;
                case EnumEvento.ArchivarTramite:
                    break;
                case EnumEvento.RecibirPresentadoTramite:
                    break;
                case EnumEvento.AnularDerivaciónTramite:
                    break;
                case EnumEvento.DesarchivarTramite:
                    break;
                case EnumEvento.ReingresarTramite:
                    break;
                case EnumEvento.EditarTramite:
                    break;
                case EnumEvento.CrearTramite:
                    break;
                case EnumEvento.ImprimirCaratula:
                    break;
                case EnumEvento.ImprimirInformeDetallado:
                    break;
                case EnumEvento.ConsultarTramite:
                    break;
                case EnumEvento.EntregarTramite:
                    break;
                case EnumEvento.ProcesarTramite:
                    break;
                default:
                    break;
            }
            lista.Add(new METipoMovimiento { IdTipoMovimiento = id, Descripcion = nombre });
        }
    }
}