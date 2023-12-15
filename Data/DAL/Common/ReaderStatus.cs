using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.DAL.Common
{
    public class ReaderStatus
    {
        internal bool Broke { get; private set; }
        public void Break()
        {
            this.Broke = true;
        }
    }
}
