using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RussianPost.Classes
{
    class PackageMin
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string LastState { get; set; }
        public bool ReadyToGet { get; set; }
    }
}
