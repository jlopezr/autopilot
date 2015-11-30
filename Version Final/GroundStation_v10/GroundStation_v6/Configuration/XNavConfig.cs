﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class XNavConfig
    {
        //Sampling time
        public double adcTs = 0.066;
        public double gpsTs = 0.066;

        //Altitude
        public double akp;
        public double aki;
        public double akd;
        public double aInitialRef;

        //Heading
        public double hkp;
        public double hki;
        public double hkd;
        public double MaxRoll;
        public double hInitialRef;

        //Speed
        public double skp;
        public double ski;
        public double skd;
        public double MinPitch;
        public double MaxPitch;
        public double sInitialRef;

        //LatNav
        public double lkp;
        public double lki;
        public double lkd;
        public double OriginLat;
        public double OriginLon;

        //Flight Plan
        public int WPnum; //Number of waypoints
        public double[] FPLatitude;
        public double[] FPLongitude;

        //Configurations
        private double[] Speedconfig = { -0.4, -0.01, 0, 0, 10, -10 }; //kp ki kd initialReference maxPitch minPitch
        private double[] Altitudeconfig = { 1.2, 0, 0, 00}; //kp ki kd initialReference
        private double[] Headingconfig = { 0.2, 0, 0, 0, 30}; //kp ki kd initialReference maxRoll
        private double[] LatNavconfig = { 0.015, 0, 0, 41.292216, 2.063400 }; //kp ki kd origPosLat origPosLon
        private double[] FPLat = { 41.310580, 41.299816, 41.292458, 41.272202, 41.281305, 41.292458, 41.308461, 41.298105};
        private double[] FPLon = { 2.119361, 2.126444, 2.103294, 2.075522, 2.070151, 2.103294, 2.094436, 2.059405 };

        public XNavConfig()
        {
            akp = Altitudeconfig[0];
            aki = Altitudeconfig[1];
            akd = Altitudeconfig[2];
            aInitialRef = Altitudeconfig[3];
            

            hkp = Headingconfig[0];
            hki = Headingconfig[1];
            hkd = Headingconfig[2];
            hInitialRef = Headingconfig[3];
            MaxRoll = Headingconfig[4];


            skp = Speedconfig[0];
            ski = Speedconfig[1];
            skd = Speedconfig[2];
            sInitialRef = Speedconfig[3];
            MaxPitch = Speedconfig[4];
            MinPitch = Speedconfig[5];
            

            lkp = LatNavconfig[0];
            lki = LatNavconfig[1];
            lkd = LatNavconfig[2];
            OriginLat = LatNavconfig[3];
            OriginLon = LatNavconfig[4];

            WPnum = FPLon.Length;
            FPLatitude = FPLat;
            FPLongitude = FPLon;
        }
    }
}
