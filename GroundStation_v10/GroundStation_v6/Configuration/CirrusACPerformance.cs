using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    class CirrusACPerformance : ACPerformance
    {
        public double StallSpeed
        {
            get { return 70; }
        }
        public double MaxBank
        {
            get { return 30; }
        }
    }
}
