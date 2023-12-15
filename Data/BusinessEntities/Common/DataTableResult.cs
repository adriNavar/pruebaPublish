namespace GeoSit.Data.BusinessEntities.Common
{
    public class DataTableResult<T>
    {
        public int draw { get; set; }
        public T[] data { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered => recordsTotal;// data == null ? 0 : data.Length;
    }
}
