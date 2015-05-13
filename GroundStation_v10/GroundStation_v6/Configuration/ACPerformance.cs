using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class ACPerformance
    {
        //Aircraft
        public enum ACModel
        {
            C172,
            RC,
            Cirrus
        };

        public double stallSpeed;
        public double maxBank;

        private double[] C172Perf = { 55, 30};
        private double[] CirrusPerf = { 70, 30 };
        private double[] RCPerf = { 20, 30 };

        public ACPerformance(ACModel mod)
        {
            if (mod == ACModel.C172)
            {
                stallSpeed = C172Perf[0];
                maxBank = C172Perf[1];
            }
            else if (mod == ACModel.Cirrus)
            {
                stallSpeed = CirrusPerf[0];
                maxBank = CirrusPerf[1];
            }
            else if (mod == ACModel.RC)
            {
                stallSpeed = RCPerf[0];
                maxBank = RCPerf[1];
            }

        }
    }
}
