using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    /// <summary>
    /// 根据 locations 获取 Place 集合
    /// </summary>
    internal static class WebServiceHelper
    {
        internal static Place[] GetPlacesFromTerraServer(List<string> locations)
        {
            List<Place> allPlaces = new List<Place>();

            // For each location, call the Web service method to get data. 
            foreach (string location in locations)
            {
                Place[] places = CallGetPlaceListMethod(location);
                allPlaces.AddRange(places);
            }

            return allPlaces.ToArray();
        }

        private static Place[] CallGetPlaceListMethod(string location)
        {
            Place[] places = new Place[3];
            places[0] = new Place(location, "A" + location, PlaceType.BayGulf);
            places[1] = new Place(location, "B" + location, PlaceType.HillMountain);
            places[2] = new Place(location, "C" + location, PlaceType.ParkBeach);
            return places;
        }
    }
}
