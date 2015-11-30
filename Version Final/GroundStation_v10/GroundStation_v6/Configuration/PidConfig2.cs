using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public class PidConfig2
    {
        //Aircraft
        public enum ACModel
        {
            C172,
            RC,
            Cirrus
        };

        public enum Ctrl
        {
            Throttle,
            Pitch,
            Roll,
            Yaw
        };

        //Sampling time
        public double imuTs = 0.1;
        public double adcTs = 0.1;

        //Throttle
        public double tkp;
        public double tki;
        public double tkd;
        public double tCh;
        public double tOffset;
        public double tSpan;
        public double tMin;
        public double tMax;
        public double tMean;
        public double tInitialRef;

        //Pitch
        public double pkp;
        public double pki;
        public double pkd;
        public double pCh;
        public double pOffset;
        public double pSpan;
        public double pMin;
        public double pMax;
        public double pMean;
        public double pInitialRef;

        //Roll
        public double rkp;
        public double rki;
        public double rkd;
        public double rCh;
        public double rOffset;
        public double rSpan;
        public double rMin;
        public double rMax;
        public double rMean;
        public double rInitialRef;
        
        //Yaw
        public double ykp;
        public double yki;
        public double ykd;
        public double yCh;
        public double yOffset;
        public double ySpan;
        public double yMin;
        public double yMax;
        public double yMean;
        public double yInitialRef;

        //Configurations for each airchraft
        private double[] C172configT = { 1, 0, -0.5, 1, 0, 8, 0, 2000, 0, 85 }; //kp ki kd channel_number offset span min max mean initialReference
        private double[] C172configP = { 35, 1, 0, 3, 0, 8, 0, 2000, 1000, 0 };
        private double[] C172configR = { 100, 1, 0, 2, 0, 8, 0, 2000, 1000, 0 };
        private double[] C172configY = { 4, 0, 0, 4, 0, 8, 0, 2000, 1000, 0 };

        private double[] RCconfigT = { 20, 0, -0.5, 1, 0, 8, 0, 2000, 0, 55 };
        private double[] RCconfigP = { 20, 15, 0, 3, 0, 8, 0, 2000, 1000, 0 };
        private double[] RCconfigR = { 20, 15, 0, 2, 0, 8, 0, 2000, 1000, 0 };
        private double[] RCconfigY = { 4, 0, 0, 4, 0, 8, 0, 2000, 1000, 0 };

        private double[] CirrusconfigT = { 1, 0, -0.5, 1, 0, 8, 0, 2000, 0, 85 };
        private double[] CirrusconfigP = { 40, 1, 0, 3, 0, 8, 0, 2000, 1000, 0 };
        private double[] CirrusconfigR = { 100, 1, 0, 2, 0, 8, 0, 2000, 1000, 0 };
        private double[] CirrusconfigY = { 4, 0, 0, 4, 0, 8, 0, 2000, 1000, 0 };

        public PidConfig2(ACModel mod)
        {
            if(mod == ACModel.C172)
            {
                SetCtrlPid(C172configT, Ctrl.Throttle, ACModel.C172);
                SetCtrlPid(C172configP, Ctrl.Pitch, ACModel.C172);
                SetCtrlPid(C172configR, Ctrl.Roll, ACModel.C172);
                SetCtrlPid(C172configY, Ctrl.Yaw, ACModel.C172);
            }
            else if(mod == ACModel.Cirrus)
            {
                SetCtrlPid(CirrusconfigT, Ctrl.Throttle, ACModel.Cirrus);
                SetCtrlPid(CirrusconfigP, Ctrl.Pitch, ACModel.Cirrus);
                SetCtrlPid(CirrusconfigR, Ctrl.Roll, ACModel.Cirrus);
                SetCtrlPid(CirrusconfigY, Ctrl.Yaw, ACModel.Cirrus);
            }
            else if (mod == ACModel.RC)
            {
                SetCtrlPid(RCconfigT, Ctrl.Throttle, ACModel.RC);
                SetCtrlPid(RCconfigP, Ctrl.Pitch, ACModel.RC);
                SetCtrlPid(RCconfigR, Ctrl.Roll, ACModel.RC);
                SetCtrlPid(RCconfigY, Ctrl.Yaw, ACModel.RC);
            }
        }

        private void SetCtrlPid(double[] config, Ctrl Con, ACModel mod)
        {
            if(Con == Ctrl.Throttle)
            {
                tkp = config[0];
                tki = config[1];
                tkd = config[2];
                tCh = config[3];
                tOffset = config[4];
                tSpan = config[5];
                tMin = config[6];
                tMax = config[7];
                tMean = config[8];
                tInitialRef = config[9];
            }
            else if (Con == Ctrl.Pitch)
            {
                pkp = config[0];
                pki = config[1];
                pkd = config[2];
                pCh = config[3];
                pOffset = config[4];
                pSpan = config[5];
                pMin = config[6];
                pMax = config[7];
                pMean = config[8];
                pInitialRef = config[9];
            }
            else if (Con == Ctrl.Roll)
            {
                rkp = config[0];
                rki = config[1];
                rkd = config[2];
                rCh = config[3];
                rOffset = config[4];
                rSpan = config[5];
                rMin = config[6];
                rMax = config[7];
                rMean = config[8];
                rInitialRef = config[9];
            }
            else if (Con == Ctrl.Yaw)
            {
                ykp = config[0];
                yki = config[1];
                ykd = config[2];
                yCh = config[3];
                yOffset = config[4];
                ySpan = config[5];
                yMin = config[6];
                yMax = config[7];
                yMean = config[8];
                yInitialRef = config[9];
            }
        }

        
    }
}
