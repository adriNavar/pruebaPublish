using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using GeoSit.Data.BusinessEntities.ObrasParticulares;
using GeoSit.Data.DAL.Interfaces;
using GeoSit.Data.BusinessEntities.Seguridad;
using GeoSit.Data.BusinessEntities.ObrasParticulares.ExpedientesObras;
using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class LiquidacionRepository : ILiquidacionRepository
    {
        private readonly GeoSITMContext _context;

        public LiquidacionRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public IEnumerable<Liquidacion> GetLiquidaciones(long idExpedienteObra)
        {

            ParametrosGenerales pg = _context.ParametrosGenerales.Where(j => j.Clave == "LiquidacionesExternas" && j.Valor == "1").FirstOrDefault();
            if (pg == null)
            {
                return _context.Liquidaciones.Where(x => x.ExpedienteObraId == idExpedienteObra);
            }
            else
            {
                return (from exp in _context.ExpedientesObra
                        join liq in _context.LiquidacionesExternas on exp.NumeroExpediente equals liq.Expediente
                        where exp.ExpedienteObraId == idExpedienteObra
                        select liq)
                         .ToList()
                         .Select(item => new Liquidacion
                         {
                             ExpedienteObraId = idExpedienteObra,
                             Fecha = item.Fecha,
                             Importe = item.Importe,
                             Numero = item.Expediente,
                             Observaciones = item.Padron
                         }).ToList();
            }
        }

        public Liquidacion GetLiquidacionById(long idLiquidacion)
        {
            return _context.Liquidaciones.Find(idLiquidacion);
        }

        public void InsertLiquidacion(Liquidacion liquidacion, ExpedienteObra expedienteObra, UnidadTributariaExpedienteObra ut)
        {
            liquidacion.ExpedienteObra = expedienteObra;
            ParametrosGenerales pg = _context.ParametrosGenerales.Where(j => j.Clave == "LiquidacionesExternas" && j.Valor == "1").FirstOrDefault();
            if (pg == null)
            {
                _context.Liquidaciones.Add(liquidacion);
            }
            else
            {
                var arrayUts = _context.UnidadesTributariasExpedienteObra.Where(x => x.ExpedienteObraId == expedienteObra.ExpedienteObraId).Select(x => x.UnidadTributariaId).ToArray();
                string padron;
                if (arrayUts.Count() > 0)
                {
                    padron = _context.UnidadesTributarias.Where(j => arrayUts.Contains(j.UnidadTributariaId)).Select(j => j.CodigoMunicipal).FirstOrDefault();
                }
                else
                {
                    padron = _context.UnidadesTributarias.Where(j => j.UnidadTributariaId == ut.UnidadTributariaId).Select(j => j.CodigoMunicipal).FirstOrDefault();
                }

                LiquidacionExterna le = new LiquidacionExterna { Padron = padron, Expediente = liquidacion.Numero, Importe = liquidacion.Importe, Fecha = liquidacion.Fecha };
                _context.LiquidacionesExternas.Add(le);
            }

        }

        public void InsertLiquidacion(Liquidacion liquidacion)
        {

            _context.Liquidaciones.Add(liquidacion);
        }

        public void DeleteLiquidacionesByExpedienteObraId(long idExpedienteObra)
        {
            ParametrosGenerales pg = _context.ParametrosGenerales.Where(j => j.Clave == "LiquidacionesExternas" && j.Valor == "1").FirstOrDefault();
            if (pg == null)
            {
                var liquidaciones = _context.Liquidaciones.Where(x => x.ExpedienteObraId == idExpedienteObra);
                foreach (var liquidacion in liquidaciones)
                {
                    _context.Entry(liquidacion).State = EntityState.Deleted;
                }
            }
            else
            {
                var arrayUts = _context.UnidadesTributariasExpedienteObra.Where(x => x.ExpedienteObraId == idExpedienteObra).Select(x => x.UnidadTributariaId).ToArray();
                var padrones = _context.UnidadesTributarias.Where(j => arrayUts.Contains(j.UnidadTributariaId)).Select(j => j.CodigoMunicipal).ToArray();
                List<LiquidacionExterna> liquidacionesext = _context.LiquidacionesExternas.Where(x => padrones.Contains(x.Padron)).ToList();

                _context.LiquidacionesExternas.RemoveRange(liquidacionesext);
                _context.SaveChanges();
            }
        }

        public void UpdateLiquidacion(Liquidacion liquidacion)
        {
            _context.Entry(liquidacion).State = EntityState.Modified;
        }

        public void DeleteLiquidacionById(long idLiquidacion)
        {
            DeleteLiquidacion(GetLiquidacionById(idLiquidacion));
        }

        public void DeleteLiquidacion(Liquidacion liquidacion)
        {
            if (liquidacion != null)
                _context.Entry(liquidacion).State = EntityState.Deleted;
        }
    }
}
