using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryStandard
{
    public class Thing
    {
        public int Get(int number) => 
            Newtonsoft.Json.JsonConvert.DeserializeObject<int>($"{number}");
    }
}
