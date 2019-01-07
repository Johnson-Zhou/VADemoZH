using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BingMapsRESTToolkit;

namespace weatherskill.ServiceClients
{
    public class BingMapService : IGeoQueryService
    {
        private string _bingMapKey = "";

        public BingMapService(string key)
        {
            _bingMapKey = key;
        }

        public async Task<GeoInfo> QueryGeoInfoByLocation(string location)
        {
            GeoInfo info = null;

            var request = new GeocodeRequest()
            {
                Query = location,
                IncludeIso2 = false,
                IncludeNeighborhood = false,
                MaxResults = 1,
                BingMapsKey = _bingMapKey
            };

            var response = await BingMapsRESTToolkit.ServiceManager.GetResponseAsync(request);
            if (response.StatusCode == 200 && response.ResourceSets.Length > 0 && response.ResourceSets[0].Resources.Length > 0)
            {
                info = new GeoInfo { Location = location, Latitude = 0, Longitude = 0 };
                var result = response.ResourceSets[0].Resources[0] as Location;
                if (result.Point.Coordinates.Length >= 2)
                {
                    info.Latitude = result.Point.Coordinates[0];
                    info.Longitude = result.Point.Coordinates[1];
                }
            }

            return info;
        }
    }
}
