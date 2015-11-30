using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class QXACPerformance : ACPerformance
    {
        public double StallSpeed
        {
            get { return 0; }
        }
        public double MaxBank
        {
            get { return 30; }
        }
    }
}