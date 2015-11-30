using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public interface ACPerformance
    {
        double StallSpeed { get; }
        double MaxBank { get; }
    }
}
