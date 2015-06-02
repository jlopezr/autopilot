using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    class RCACPerformance : ACPerformance
    {
        public double StallSpeed
        {
            get { return 20; }
        }
        public double MaxBank
        {
            get { return 30; }
        }
    }
}
