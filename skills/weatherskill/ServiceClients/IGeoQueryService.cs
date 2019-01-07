using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace weatherskill.ServiceClients
{
    public class GeoInfo
    {
        public string Location;
        public double Latitude;
        public double Longitude;
    }

    public interface IGeoQueryService
    {
       Task<GeoInfo> QueryGeoInfoByLocation(string location);
    }
}
