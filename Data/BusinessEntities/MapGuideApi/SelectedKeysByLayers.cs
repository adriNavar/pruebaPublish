using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoSit.Data.BusinessEntities.MapGuideApi
{
    public class SelectedKeysByLayers
    {
        public string SessionId{get;set;} 
        public string MapName{get;set;}
        public string[] Layers{get;set;}
        public string[] Keys{get;set;}
    }
}
