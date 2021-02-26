using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S1_Вариант3.Model;

namespace S1_Вариант3.Classes
{
    public class FlightDetail
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string FlightNumbers { get; set; }
        public string Price { get; set; }
        public short NumberOfStops { get; set; }

        public List<Flight_Scheludes> UsingScheludes { get; set; }
    }
}
