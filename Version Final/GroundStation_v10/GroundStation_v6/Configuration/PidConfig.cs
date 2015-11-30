using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public interface PidConfig
    {
        //Sampling time
        double imuTs { get; }
        double adcTs { get; }

        //Throttle
        double tkp { get; }
        double tki { get; }
        double tkd { get; }
        double tCh { get; }
        double tOffset { get; }
        double tSpan { get; }
        double tMin { get; }
        double tMax { get; }
        double tMean { get; }
        double tInitialRef { get; }

        //Pitch
        double pkp { get; }
        double pki { get; }
        double pkd { get; }
        double pCh { get; }
        double pOffset { get; }
        double pSpan { get; }
        double pMin { get; }
        double pMax { get; }
        double pMean { get; }
        double pInitialRef { get; }

        //Roll
        double rkp { get; }
        double rki { get; }
        double rkd { get; }
        double rCh { get; }
        double rOffset { get; }
        double rSpan { get; }
        double rMin { get; }
        double rMax { get; }
        double rMean { get; }
        double rInitialRef { get; }

        //Yaw
        double ykp { get; }
        double yki { get; }
        double ykd { get; }
        double yCh { get; }
        double yOffset { get; }
        double ySpan { get; }
        double yMin { get; }
        double yMax { get; }
        double yMean { get; }
        double yInitialRef { get; }
    }
}
