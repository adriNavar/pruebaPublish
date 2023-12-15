using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.InterfaseRentas
{
    public class InterfaseRentasResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        private T _data;
        public T Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                Success = _data != null;
            }

        }
    }
}
