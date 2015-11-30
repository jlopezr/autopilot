using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class C172ACPerformance : ACPerformance
    {
        public double StallSpeed
        {
            get { return 45; }
        }
        public double MaxBank
        {
            get { return 30; }
        }
    }
}
