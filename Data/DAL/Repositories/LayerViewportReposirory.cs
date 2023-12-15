using GeoSit.Data.DAL.Contexts;

namespace GeoSit.Data.DAL.Repositories
{
    public class LayerViewportReposirory : ILayerViewportReposirory
    {
        private readonly GeoSITMContext _context;

        public LayerViewportReposirory(GeoSITMContext context)
        {
            _context = context;
        }

    }
}
