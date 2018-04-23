using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppCore.MyLinq.LinqToTerra
{
    public class Place
    {
        // Properties. 
        public string Name { get; private set; }
        public string State { get; private set; }
        public PlaceType PlaceType { get; private set; }

        // Constructor. 
        internal Place(string name,
                        string state,
                        PlaceType placeType)
        {
            Name = name;
            State = state;
            PlaceType = (PlaceType)placeType;
        }
    }
}
