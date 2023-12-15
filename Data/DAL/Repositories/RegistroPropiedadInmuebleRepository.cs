using Geosit.Core.Utils.Crypto;
using GeoSit.Data.BusinessEntities.Certificados;
using GeoSit.Data.BusinessEntities.Documentos;
using GeoSit.Data.BusinessEntities.GlobalResources;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.BusinessEntities.LogRPI;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.DAL.Contexts;
using GeoSit.Data.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Repositories
{
    public class RegistroPropiedadInmuebleRepository : IRegistroPropiedadInmuebleRepository
    {
        private readonly GeoSITMContext _context;

        public RegistroPropiedadInmuebleRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public object GetCertificadoCatastral(string numCertificadoCatastral)
        {


            var query = (from certfi in _context.INMCertificadosCatastrales
                         join ut in _context.UnidadesTributarias on certfi.UnidadTributariaId equals ut.UnidadTributariaId
                         where certfi.Numero == numCertificadoCatastral && certfi.FechaBaja == null && ut.FechaBaja == null
                         select new
                         {
                             Numero = certfi.Numero,
                             FechaEmision = certfi.FechaEmision,
                             FechaVigencia = DbFunctions.AddDays(certfi.FechaEmision, certfi.Vigencia).Value,
                             Descripcion = certfi.Descripcion,
                             CodigoProvincial = ut.CodigoProvincial
                         })
                        .FirstOrDefault();

            if (query == null)
            {
                throw new NullReferenceException();
            }
            else if (query.FechaVigencia.Date < DateTime.Today)
            {
                throw new IndexOutOfRangeException();
            }

            return query;

        }

        public object GetCertificadoCatastralByNumero(string numCertificadoCatastral)
        {
            long? iddoc = (from certfi in _context.INMCertificadosCatastrales
                           join utdoc in _context.UnidadesTributariasDocumento on certfi.UnidadTributariaId equals utdoc.UnidadTributariaID
                           join docs in _context.Documento on utdoc.DocumentoID equals docs.id_documento
                           where certfi.Numero == numCertificadoCatastral && certfi.FechaBaja == null && utdoc.FechaBaja == null && docs.fecha_baja_1 == null
                           select docs.id_documento).FirstOrDefault();

            if (!iddoc.HasValue)
            {
                throw new NullReferenceException();
            }

            var archivo = new DocumentoRepository(_context).GetContent(iddoc.Value);

            return $"data:{archivo.ContentType};base64,{Convert.ToBase64String(archivo.Contenido)}";

        }

        public void RegistrarLogRespuesta(RPILogRespuestas logRespuesta)
        {
            _context.RPILogRespuestas.Add(logRespuesta);
            _context.SaveChanges();
        }



        public List<object> GetPlanosMensuraByNumero(string numMensura, string letraMensura)
        {
            var planos = _context.Mensura.Where(x => x.Numero == numMensura && x.Letra == letraMensura && !x.FechaBaja.HasValue)
                         .Include("TipoMensura")
                         .Select(a => new
                         {
                             IdMensura = a.IdMensura,
                             Numero = a.Numero,
                             Letra = a.Letra,
                             FechaAprobacion = a.FechaAprobacion,
                             DescripcionTipoMensura = a.TipoMensura.Descripcion
                         })
                         .ToList<object>();

            if (!planos.Any())
            {
                throw new NullReferenceException();
            }

            return planos;

        }

        public List<object> GetPlanosMensuraByNomenclatura(string nomenclatura)
        {
            var listplanos = (from planos in _context.Mensura
                              join tipomensura in _context.TipoMensura on planos.IdTipoMensura equals tipomensura.IdTipoMensura
                              join parmensura in _context.ParcelaMensura on planos.IdMensura equals parmensura.IdMensura
                              join nomenc in _context.Nomenclaturas on parmensura.IdParcela equals nomenc.ParcelaID
                              where nomenc.Nombre == nomenclatura && planos.FechaBaja == null && nomenc.FechaBaja == null && parmensura.FechaBaja == null
                              select new
                              {
                                  IdMensura = planos.IdMensura,
                                  Numero = planos.Numero,
                                  Letra = planos.Letra,
                                  FechaAprobacion = planos.FechaAprobacion,
                                  DescripcionTipoMensura = tipomensura.Descripcion
                              })
                             .ToList<object>();


            if (!listplanos.Any())
            {
                throw new NullReferenceException();
            }

            return listplanos;

        }

        public List<object> GetPlanosMensuraByNumeroPartida(string numeroPartida)
        {
            var listplanos = (from planos in _context.Mensura
                              join tipomensura in _context.TipoMensura on planos.IdTipoMensura equals tipomensura.IdTipoMensura
                              join parmensura in _context.ParcelaMensura on planos.IdMensura equals parmensura.IdMensura
                              join ut in _context.UnidadesTributarias on parmensura.IdParcela equals ut.ParcelaID
                              where ut.CodigoProvincial == numeroPartida && planos.FechaBaja == null && ut.FechaBaja == null && parmensura.FechaBaja == null
                              select new
                              {
                                  IdMensura = planos.IdMensura,
                                  Numero = planos.Numero,
                                  Letra = planos.Letra,
                                  FechaAprobacion = planos.FechaAprobacion,
                                  DescripcionTipoMensura = tipomensura.Descripcion
                              })
                             .ToList<object>();


            if (!listplanos.Any())
            {
                throw new NullReferenceException();
            }

            return listplanos;

        }

        public object GetPlanoMensuraByIdMensura(long idMensura)
        {
            long? iddoc = (from planos in _context.Mensura
                           join mensuradoc in _context.MensuraDocumento on planos.IdMensura equals mensuradoc.IdMensura
                           join docu in _context.Documento on mensuradoc.IdDocumento equals docu.id_documento
                           where planos.IdMensura == idMensura && planos.FechaBaja == null && mensuradoc.FechaBaja == null && docu.fecha_baja_1 == null
                           select docu.id_documento).FirstOrDefault();

            if (!iddoc.HasValue)
            {
                throw new NullReferenceException();
            }

            var archivo = new DocumentoRepository(_context).GetContent(iddoc.Value);

            return $"data:{archivo.ContentType};base64,{Convert.ToBase64String(archivo.Contenido)}";

        }

        public object VerificarPermisoDeAcceso(long idUsuario)
        {
            var usuario = (from us in _context.Usuarios
                          where us.Id_Usuario == idUsuario
                          select us).FirstOrDefault();

            if (usuario != null)
            {

                var permiso = (from up in _context.UsuariosPerfiles
                               join pf in _context.PerfilesFunciones on up.Id_Perfil equals pf.Id_Perfil
                               join f in _context.Funciones on pf.Id_Funcion equals f.Id_Funcion
                               where up.Id_Usuario == idUsuario && up.Fecha_Baja == null && pf.Fecha_Baja == null && f.Id_Funcion == 605
                               select new
                               {
                                   up.Id_Usuario,

                               }).FirstOrDefault();

                if (permiso == null)
                {
                    throw new IndexOutOfRangeException();
                }

                return permiso;
            }
            else
            {
                throw new NullReferenceException();
            }

        }

        public void RegistrarLogConsulta(RPILogConsultas logConsulta)
        {
            _context.RPILogConsultas.Add(logConsulta);
            _context.SaveChanges();
        }
    }
}






