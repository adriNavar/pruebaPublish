﻿using GeoSit.Data.BusinessEntities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.SubSistemaWeb
{
    public class WebLinks : IEntity 
    {
        public long idLink { get; set; }
        public string Descripcion { get; set; }
        public string URL { get; set; }

    }
}
