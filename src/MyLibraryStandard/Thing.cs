using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyLibraryStandard
{
    public class Thing
    {
        public int Get(int number) => JsonConvert.DeserializeObject<int>($"{number}");
    }
}
