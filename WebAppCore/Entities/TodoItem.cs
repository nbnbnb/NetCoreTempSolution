using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppCore.Entities
{
    public class TodoItem
    {
        public bool IsDone { get; set; }

        public int Priority { get; set; }

        public string Name { get; set; }
    }
}
