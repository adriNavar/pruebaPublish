using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace GeoSit.Data.Mappers.CustomConventions
{
    public class LowerCaseNamingConvention : IStoreModelConvention<EdmProperty>, IStoreModelConvention<EntitySet>
    {
        public void Apply(EntitySet item, DbModel model)
        {
            item.Table = item.Table.ToLowerInvariant();
            item.Schema = item.Schema.ToLowerInvariant();
        }

        public void Apply(EdmProperty item, DbModel model)
        {
            item.Name = item.Name.ToLowerInvariant();
        }
    }
}
