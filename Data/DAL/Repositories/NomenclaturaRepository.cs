using System.Linq;
using GeoSit.Data.BusinessEntities.Inmuebles;
using GeoSit.Data.DAL.Interfaces;
using System;
using GeoSit.Data.DAL.Contexts;
using System.Data.Entity;
using GeoSit.Data.BusinessEntities.MapasTematicos;

namespace GeoSit.Data.DAL.Repositories
{
    public class NomenclaturaRepository : INomenclaturaRepository
    {
        private const long NOMENCLATURA_CATASTRO = 3;

        private readonly GeoSITMContext _context;

        public NomenclaturaRepository(GeoSITMContext context)
        {
            _context = context;
        }
        public Nomenclatura GetNomenclatura(string nomenclatura)
        {
            return _context.Nomenclaturas.FirstOrDefault(x => x.Nombre == nomenclatura);
        }
        public Nomenclatura GetNomenclaturaById(long id)
        {
            return _context.Nomenclaturas.Find(id);
        }
        public void InsertNomenclatura(Nomenclatura nomenclatura)
        {
            nomenclatura.UsuarioAltaID = nomenclatura.UsuarioModificacionID;
            nomenclatura.FechaModificacion = DateTime.Now;
            nomenclatura.FechaAlta = nomenclatura.FechaModificacion;
            _context.Nomenclaturas.Add(nomenclatura);
        }
        public void UpdateNomenclatura(Nomenclatura nomenclatura)
        {
            nomenclatura.FechaModificacion = DateTime.Now;
            _context.Entry(nomenclatura).State = EntityState.Modified;
            _context.Entry(nomenclatura).Property(p => p.UsuarioAltaID).IsModified = false;
            _context.Entry(nomenclatura).Property(p => p.FechaAlta).IsModified = false;
        }
        public void DeleteNomenclatura(Nomenclatura nomenclatura)
        {
            nomenclatura.UsuarioBajaID = nomenclatura.UsuarioModificacionID;
            nomenclatura.FechaModificacion = DateTime.Now;
            nomenclatura.FechaBaja = nomenclatura.FechaModificacion;
            _context.Entry(nomenclatura).State = EntityState.Modified;
            _context.Entry(nomenclatura).Property(p => p.UsuarioAltaID).IsModified = false;
            _context.Entry(nomenclatura).Property(p => p.FechaAlta).IsModified = false;
        }
        public Nomenclatura GetNomenclatura(long idParcela, long idTipoNomenclatura)
        {
            return _context.Nomenclaturas.FirstOrDefault(n => n.ParcelaID == idParcela && n.TipoNomenclaturaID == idTipoNomenclatura);
        }
        public string Generar(long idParcela, long tipo)
        {
            if (tipo == NOMENCLATURA_CATASTRO)
            {
                using (var builder = _context.CreateSQLQueryBuilder())
                {
                    string wkt = builder.AddTable("inm_parcela_grafica", "pg")
                                        .AddFilter("id_parcela", idParcela, Common.Enums.SQLOperators.EqualsTo)
                                        .AddFilter("fecha_baja", null, Common.Enums.SQLOperators.IsNull, Common.Enums.SQLConnectors.And)
                                        .AddGeometryField(builder.CreateGeometryFieldBuilder(new Atributo() { Campo = "geometry" }, "pg").ToWKT(), "geomwkt")
                                        .MaxResults(1)
                                        .ExecuteQuery((reader, status) =>
                                        {
                                            return reader.GetString(reader.GetOrdinal("geomwkt"));
                                        })
                                        .SingleOrDefault();

                    return new MesaEntradasRepository(_context).GenerarNomenclaturaParcela(wkt);
                }
            }
            return string.Empty;
        }
    }
}
