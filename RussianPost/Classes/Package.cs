using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RussianPost.Classes
{
    class Package
    {
        public Package()
        {
            this.History = new List<MovingHistory>();
        }

        public string ID { get; set; }
        public string Name { get; set; }
        public List<MovingHistory> History { get; set; }
        public sbyte Status { get; set; }           // 0 - в пути; 1 - готова к выдаче; 2 - выдано
        public string CommonStatus { get; set; }    // короткий статус посылки, вывод
        public string Description { get; set; }
        public uint Weight { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ToCity { get; set; }
    }
}
