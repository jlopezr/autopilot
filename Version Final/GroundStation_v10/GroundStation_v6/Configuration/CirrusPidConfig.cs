using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class CirrusPidConfig : PidConfig
    {
        //Sampling time
        public double imuTs
        {
            get { return 0.066; }
        }
        public double adcTs
        {
            get { return 0.066; }
        }

        //Throttle
        public double tkp
        {
            get { return 100; }
        }
        public double tki
        {
            get { return 1.5; }
        }
        public double tkd
        {
            get { return 0.0; }
        }
        public double tCh
        {
            get { return 1; }
        }
        public double tOffset
        {
            get { return 0; }
        }
        public double tSpan
        {
            get { return 8; }
        }
        public double tMin
        {
            get { return 0; }
        }
        public double tMax
        {
            get { return 2000; }
        }
        public double tMean
        {
            get { return 0; }
        }
        public double tInitialRef
        {
            get { return 140; }
        }

        //Pitch
        public double pkp
        {
            get { return 15; }
        }
        public double pki
        {
            get { return 1; }
        }
        public double pkd
        {
            get { return 0; }
        }
        public double pCh
        {
            get { return 3; }
        }
        public double pOffset
        {
            get { return 0; }
        }
        public double pSpan
        {
            get { return 8; }
        }
        public double pMin
        {
            get { return 0; }
        }
        public double pMax
        {
            get { return 2000; }
        }
        public double pMean
        {
            get { return 1000; }
        }
        public double pInitialRef
        {
            get { return 0; }
        }

        //Roll
        public double rkp
        {
            get { return 20; }
        }
        public double rki
        {
            get { return 1; }
        }
        public double rkd
        {
            get { return 0; }
        }
        public double rCh
        {
            get { return 2; }
        }
        public double rOffset
        {
            get { return 0; }
        }
        public double rSpan
        {
            get { return 8; }
        }
        public double rMin
        {
            get { return 0; }
        }
        public double rMax
        {
            get { return 2000; }
        }
        public double rMean
        {
            get { return 1000; }
        }
        public double rInitialRef
        {
            get { return 0; }
        }

        //Yaw
        public double ykp
        {
            get { return 4; }
        }
        public double yki
        {
            get { return 0; }
        }
        public double ykd
        {
            get { return 0; }
        }
        public double yCh
        {
            get { return 4; }
        }
        public double yOffset
        {
            get { return 0; }
        }
        public double ySpan
        {
            get { return 8; }
        }
        public double yMin
        {
            get { return 0; }
        }
        public double yMax
        {
            get { return 2000; }
        }
        public double yMean
        {
            get { return 1000; }
        }
        public double yInitialRef
        {
            get { return 0; }
        }
    }
}
