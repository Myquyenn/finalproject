using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Project
{
    public class History : IComparable<History>
    {
        public String Intput { get; set; }
        public String Output { get; set; }
        public DateTime TimeEvent { get; set; }
        public History() { }
        public History(String Intput, String Output)
        {
            this.Intput = Intput;
            this.Output = Output;
            TimeEvent = DateTime.Now;
        }

        public int CompareTo(History other)
        {
            if (TimeEvent > other.TimeEvent)
                return -1;
            else if (TimeEvent < other.TimeEvent)
                return 1;
            else
                return 0;
        }
    }
}
