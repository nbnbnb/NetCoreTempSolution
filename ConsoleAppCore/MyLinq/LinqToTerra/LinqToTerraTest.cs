using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    class LinqToTerraTest
    {
        internal static void Run()
        {
            QueryableTerraServerData<Place> terraPlaces = new QueryableTerraServerData<Place>();
            string[] places = { "Johannesburg", "Yachats", "Seattle" };

            var query = from place in terraPlaces
                        where place.Name == "Johannesburg" &&
                        place.Name.StartsWith("Johan") &&
                        places.Contains(place.Name) &&
                        places.ToList().Contains(place.Name)
                        select place.PlaceType;

            foreach (PlaceType placeType in query)
                Console.WriteLine(placeType);
        }
    }
}
