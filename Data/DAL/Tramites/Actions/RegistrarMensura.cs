using GeoSit.Data.BusinessEntities.MesaEntradas;
using GeoSit.Data.BusinessEntities.Temporal;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Tramites.Actions.Abstract;
using GeoSit.Data.BusinessEntities.GlobalResources;
using System;
using System.Linq;

namespace GeoSit.Data.DAL.Tramites.Actions
{
    class RegistrarMensura : Accion
    {
        public RegistrarMensura(METramite tramite, GeoSITMContext contexto)
            : base(tramite, contexto) { }

        public override bool Execute()
        {
            var queryEntradasMensuraRegistradas = (from entrada in GetEntradas(Convert.ToInt32(Entradas.MensuraRegistrada))
                                                   join mensura in Contexto.Mensura on entrada.IdObjeto equals mensura.IdMensura
                                                   select mensura).ToList();

            var numeroLetra = new Repositories.MesaEntradasRepository(Contexto).GenerarMensura(Contexto.Objetos.Find(Tramite.IdJurisdiccion).ObjetoPadreId ?? 0);
            var mensuraTemporal = Contexto.MensurasTemporal.Add(new MensuraTemporal()
            {
                IdTramite = Tramite.IdTramite,
                IdTipoMensura = ObtenerTipoMensura(),
                IdEstadoMensura = 1,
                FechaPresentacion = Tramite.FechaInicio,
                FechaAprobacion = null,
                Numero = numeroLetra.First(),
                Letra = numeroLetra.Last(),
                Descripcion = string.Join("-", numeroLetra),
                MensurasRelacionadas = string.Join(",", queryEntradasMensuraRegistradas.Select(mr => $"{mr.Numero}-{mr.Letra}")),
                Observaciones = Tramite.Motivo,
                IdUsuarioAlta = Tramite.UsuarioModif,
                FechaAlta = Tramite.FechaModif,
                IdUsuarioModif = Tramite.UsuarioModif,
                FechaModif = Tramite.FechaModif
            });

            Contexto.MensurasRelacionadasTemporal
                    .AddRange(queryEntradasMensuraRegistradas
                                .Select(mr => new MensuraRelacionadaTemporal()
                                {
                                    FechaAlta = Tramite.FechaModif,
                                    IdUsuarioAlta = Tramite.UsuarioModif,
                                    FechaModif = Tramite.FechaModif,
                                    IdUsuarioModif = Tramite.UsuarioModif,

                                    IdTramite = Tramite.IdTramite,
                                    IdMensuraOrigen = mr.IdMensura,
                                    MensuraDestino = mensuraTemporal
                                }));
            queryEntradasMensuraRegistradas.Clear();
            queryEntradasMensuraRegistradas = null;

            Contexto.SaveChanges();
            return true;
        }

        private long ObtenerTipoMensura()
        {
            string tipoMensura;
            if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionEdificacionAConstruirRegimenPreHorizontalidadMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionEdificacionAConstruirRegimenPreHorizontalidadMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionEdificacionExistenteAConstruirRegimenPHMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionEdificacionExistenteAConstruirRegimenPHMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionEdificacionExistenteRegimenPHMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionEdificacionExistenteRegimenPHMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionRegimenConjuntoInmobiliarioCementerioPrivadoMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionConjuntoInmobiliarioCementerioPrivadoMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionRegimenConjuntoInmobiliarioMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionConjuntosInmobiliariosMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.FraccionExtensionMayorRegimenDerechoRealSuperficie))
            {
                tipoMensura = TiposMensuras.MensuraFraccionExtensionMayorRegimenDerechoRealSuperficie;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.Mensura))
            {
                tipoMensura = TiposMensuras.Mensura;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraProyectoUnificacionYRedistribucionParcelaria))
            {
                tipoMensura = TiposMensuras.MensuraProyectoUnificacionYRedistribucionParcelaria;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraUnificacionYRedistribucionParcelaria))
            {
                tipoMensura = TiposMensuras.MensuraUnificacionYRedistribucionParcelaria;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYDeslinde))
            {
                tipoMensura = TiposMensuras.MensuraYDeslinde;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYDivision) || Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYDivisionUrbana))
            {
                tipoMensura = TiposMensuras.MensuraYDivision;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYDivisionRegimenConjuntoInmobiliario))
            {
                tipoMensura = TiposMensuras.MensuraYDivisionRegimenConjuntoInmobiliario;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYDivisionRegimenConjuntoInmobiliarioCementerioPrivado))
            {
                tipoMensura = TiposMensuras.MensuraYDivisionRegimenConjuntoInmobiliarioCementerioPrivado;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYProyectoUnificacion))
            {
                tipoMensura = TiposMensuras.MensuraYProyectoUnificacion;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraYUnificacion))
            {
                tipoMensura = TiposMensuras.MensuraYUnificacion;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.ModificacionEdificacionExistenteRegimenPHMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraModificacionEdificacionExistenteRegimenPHMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.ModificacionEstadoParcelarioMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraModificacionEstadoParcelarioMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.ParcelaPlanoRegistrado))
            {
                tipoMensura = TiposMensuras.MensuraParcelaPlanoRegistrado;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.PrescripcionAdquisitiva))
            {
                tipoMensura = TiposMensuras.MensuraPrescripcionAdquisitiva;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.PrescripcionAdquisitivaDivision))
            {
                tipoMensura = TiposMensuras.MensuraPrescripcionAdquisitivaYDivision;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.DivisionPrescripcionAdquisitivaMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraDivisionPrescripcionAdquisitivaMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.ProyectoUnificacionMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraProyectoUnificacionMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.ProyectoUnificacionYRedistribucionParcelariaMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraProyectoUnificacionYRedistribucionParcelariaMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.RectificacionMensura))
            {
                tipoMensura = TiposMensuras.MensuraRectificacion;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.UnificacionMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraUnificacionMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.UnificacionYRedistribucionParcelariaMensuraRegistrada))
            {
                tipoMensura = TiposMensuras.MensuraUnificacionYRedistribucionParcelariaMensuraRegistrada;
            }
            else if (Tramite.IdObjetoTramite == Convert.ToInt64(ObjetosTramites.MensuraServidumbre))
            {
                tipoMensura = TiposMensuras.MensuraServidumbre;
            }
            else
            {
                tipoMensura = TiposMensuras.MensuraSinIdentificar;
            }
            return Convert.ToInt64(tipoMensura);
        }
    }
}
