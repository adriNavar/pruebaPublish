using GeoSit.Data.DAL;
using GeoSit.Data.DAL.Common;
using Ninject;

namespace GeoSit.Web.Api
{
    public class NinjectConfigurator
    {
        public void Configure(IKernel container)
        {
            AddBindings(container);
        }

        private void AddBindings(IKernel container)
        {
            container.Bind<UnitOfWork>().ToSelf();
        }
    }
}