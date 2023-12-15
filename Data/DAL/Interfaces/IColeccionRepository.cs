using GeoSit.Data.BusinessEntities.MapasTematicos;
using GeoSit.Data.BusinessEntities.Seguridad;
using System.Collections.Generic;

namespace GeoSit.Data.DAL.Interfaces
{
    public interface IColeccionRepository
    {
        IEnumerable<Coleccion> GetColecciones();
        IEnumerable<Coleccion> GetColeccionesByUserId(long userId);
        void NuevaColeccion(Coleccion coleccion);
        void AgregarComponentesColeccion(Coleccion coleccionOrigen, Coleccion coleccionDestino, bool validrDuplicados);
        Coleccion GetColeccionById(long coleccionId);
        void GetComponentes(Coleccion coleccion);
        void GetAtributos(Componente componente);
        bool ValidarNombreColeccion(long usuarioId, string nombreColeccion);
        List<Componente> GetComponentesRelacionados(long idComponente);
        List<Jerarquia> ObtenerJerarquiasHijas(long idJerarquia);
        List<Jerarquia> ObtenerJerarquiasPadres(long idJerarquia);
        Atributo GetAtributosById(long idAtributo);
        Jerarquia ObtenerJerarquiaPadre(long idJerarquia);
        Jerarquia ObtenerJerarquiaHijo(long idJerarquia);
        bool QuitarObjetoColeccion(long usuarioId, long objetoId, long componenteId, long coleccionId);
        bool AgregarObjetoColeccion(long usuarioId, long objetoId, long componenteId, long coleccionId);
        ColeccionComponente GetColeccionComponenteByObjetoId(long objetoId);
        bool AgregarColeccionComponente(ColeccionComponente colecCompo);
        IEnumerable<Coleccion> GetColeccionesUsuarioByComponentesPrincipales(long idUsuario, long[] componentesPrincipales);
        void GuardarColeccion(Coleccion coleccion);
        Coleccion GuardarColeccion(CargasTecnicas cargaTecnica);
    }
}
