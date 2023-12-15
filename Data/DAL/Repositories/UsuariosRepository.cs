using GeoSit.Data.DAL.Contexts;
using System;
using System.Linq;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Linq.Expressions;

namespace GeoSit.Data.DAL.Repositories
{
    public class UsuariosRepository
    {
        readonly GeoSITMContext _context;
        public UsuariosRepository(GeoSITMContext context)
        {
            _context = context;
        }

        public bool EsUsuarioAdmin(long idUsuario)
        {
            long idPerfilAdmin = Convert.ToInt64(_context.ParametrosGenerales.Single(x => x.Clave == "ID_PERFIL_ADMINISTRADOR").Valor);
            return (from usuario in _context.Usuarios
                    join perfil in _context.UsuariosPerfiles on usuario.Id_Usuario equals perfil.Id_Usuario
                    where perfil.Id_Perfil == idPerfilAdmin && perfil.Id_Usuario == idUsuario && perfil.Fecha_Baja == null
                    select 1).Any();

        }

        public Usuarios GetUsuarioByIdFecha(long id, long ticks)
        {
            DateTime fecha = new DateTime(ticks);
            DateTime.TryParse(_context.ParametrosGenerales.SingleOrDefault(p => p.Clave == "FECHA_PROCESO_MIGRACION")?.Valor, out DateTime fechaMigracion);

            Expression<Func<Usuarios, bool>> filtroByIdUsuario = u => u.Id_Usuario == id;
            if (fecha < fechaMigracion)
            {
                filtroByIdUsuario = u => u.IdISICAT == id;
            }
            return _context.Usuarios.SingleOrDefault(filtroByIdUsuario) ?? new Usuarios() { Apellido = "Usuario No Migrado", Nombre =$"ID {id}" };
        }
    }
}
