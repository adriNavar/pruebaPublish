using GeoSit.Data.DAL.Common.Enums;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Web.Api.Services.Interfaces;
using System;
using System.Linq;
using System.Threading;

namespace GeoSit.Web.Api.Services
{
    internal class SessionsCleanup : IStandaloneService
    {
        private const int MINUTES = 2;

        private readonly Timer _cleanupTimer;
        private readonly Mutex _mtxProcessing;
        public SessionsCleanup()
        {
            _cleanupTimer = new Timer(doCleanup, null, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            _mtxProcessing = new Mutex();
        }
        public void Start()
        {
            _cleanupTimer.Change(TimeSpan.FromMinutes(.5), Timeout.InfiniteTimeSpan);
        }

        private void doCleanup(object state)
        {
            try
            {
                _mtxProcessing.WaitOne();
                DateTime window = DateTime.Now.AddMinutes(-MINUTES);
                using (var ctx = GeoSITMContext.CreateContext())
                {
                    var usuariosInactivos = (from usuarioActivo in ctx.UsuariosActivos
                                             where usuarioActivo.Heartbeat < window
                                             select usuarioActivo).ToList();

                    foreach (var inactivo in usuariosInactivos)
                    {
                        bool liberarEspacio = usuariosInactivos.IndexOf(inactivo) == usuariosInactivos.Count;

                        ctx.Entry(inactivo).State = System.Data.Entity.EntityState.Deleted;
                        try
                        {
                            using (var builder = ctx.CreateSQLQueryBuilder())
                            {
                                builder.AddTable("mt_objeto_resultado", "")
                                       .AddFilter("id_usuario", inactivo.Id_Usuario, SQLOperators.EqualsTo)
                                       .AddFilter("token_sesion", $"'{inactivo.Token}'", SQLOperators.EqualsTo, SQLConnectors.And)
                                       .ExecuteDelete(liberarEspacio);
                            }
                        }
                        catch (Exception ex)
                        {
                            Global.GetLogger().LogError($"DeleteObjetoResultado({inactivo.Id_Usuario},{inactivo.Token})", ex);
                        }
                    }
                    ctx.SaveChanges();
                }
            }
            finally
            {
                _cleanupTimer.Change(TimeSpan.FromMinutes(MINUTES), Timeout.InfiniteTimeSpan);
                _mtxProcessing.ReleaseMutex();
            }
        }
        public void Stop()
        {
            while (!_mtxProcessing.WaitOne(100))
            {
                Thread.Sleep(100);
            }
            _cleanupTimer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
        }
    }
}