using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace GroundStation
{
    public class PIDManager
    {
		private static PIDManager instance = null;
		
		public enum Ctrl
		{
			THROTTLE,
			ROLL,
			PITCH,
			YAW
		};

        //PidConfig.ACModel ACmod;

        private int model;

		private object pidMutex;
        private PID rollPid;
        private PID pitchPid;
        private PID yawPid;
        private PID throttlePid;
		private PID[] pids;

		
		private object chInMutex;
		private byte[] chIn;
		public byte[] ChIn
		{
			get
			{
				lock(this.chInMutex)
				{
					return this.chIn;
				}
			}
		}
		
		private PIDManager(PidConfig p, int model1)
		{
			this.chInMutex = new object();
			this.pidMutex = new object();
			lock(this.pidMutex)
			{
				this.pids = new PID[4];
				this.chIn = new byte[4];
                //TODO Estos PIDs deberian ser creados solo en un punto. Si en la conf no esta el throttle
                //     pues claro luego peta porque no lo encuentra.
                this.pids[0] = new EmptyPID();
                this.pids[1] = new EmptyPID();
                this.pids[2] = new EmptyPID();
                this.pids[3] = new EmptyPID();
			}
            this.SetInfo(p);
            model = model1;
		}
		
		/*public static PIDManager GetInstance()
		{
			if(instance == null)
				instance = new PIDManager();
			return instance;
		}*/

        public static PIDManager GetInstance()
        {
            if (instance == null)
                Console.WriteLine("Error: null AircraftPerformance instance");
            return instance;
        }

        public static PIDManager GetInstance(int m)
        {
            if (instance == null)
            {
                PidConfig p;
                if (m == 1) //Models are used here
                {
                    p = new C172PidConfig();
                    instance = new PIDManager(p,m);
                }
                if (m == 2)
                {
                    p = new RCPidConfig();
                    instance = new PIDManager(p, m);
                }
                if (m == 3)
                {
                    p = new CirrusPidConfig();
                    instance = new PIDManager(p, m);
                }
                if (m == 20)
                {
                    p = new QXPidConfig();
                    instance = new PIDManager(p, m);
                }
            }

            return instance;
        }

        /*public void SetModel(int m)
        {
            if (m == 1)
            {
                ACmod = PidConfig.ACModel.C172;
            }
            if (m == 2)
            {
                ACmod = PidConfig.ACModel.RC;
            }
            if (m == 3)
            {
                ACmod = PidConfig.ACModel.Cirrus;
            }
            this.SetInfo();
        }*/
		        
        private void SetInfo(PidConfig config)
        {
            //PidConfig config = new PidConfig(ACmod); //Same A/C model here and in AircraftPerformance class
            int ch, offset, minVal, maxVal, meanVal;
            double spanFactor, imuTs = -1, adcTs = -1, kp, ki, kd, refValue;
            imuTs = config.imuTs;
            adcTs = config.adcTs;

            //Roll
            kp = config.rkp;
            ki = config.rki;
            kd = config.rkd;
            ch = Convert.ToInt32(config.rCh);
            offset = Convert.ToInt32(config.rOffset);
            spanFactor = config.rSpan;
            minVal = Convert.ToInt32(config.rMin);
            maxVal = Convert.ToInt32(config.rMax);
            meanVal = Convert.ToInt32(config.rMean);
            refValue = config.rInitialRef;
            lock (this.pidMutex)
            {
                this.rollPid = new RollPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
                this.pids[ch - 1] = this.rollPid;
            }

            //Pitch
            kp = config.pkp;
            ki = config.pki;
            kd = config.pkd;
            ch = Convert.ToInt32(config.pCh);
            offset = Convert.ToInt32(config.pOffset);
            spanFactor = config.pSpan;
            minVal = Convert.ToInt32(config.pMin);
            maxVal = Convert.ToInt32(config.pMax);
            meanVal = Convert.ToInt32(config.pMean);
            refValue = config.pInitialRef;
            lock (this.pidMutex)
            {
                this.pitchPid = new PitchPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
                this.pids[ch - 1] = this.pitchPid;
            }

            //Yaw
            kp = config.ykp;
            ki = config.yki;
            kd = config.ykd;
            ch = Convert.ToInt32(config.yCh);
            offset = Convert.ToInt32(config.yOffset);
            spanFactor = config.ySpan;
            minVal = Convert.ToInt32(config.yMin);
            maxVal = Convert.ToInt32(config.yMax);
            meanVal = Convert.ToInt32(config.yMean);
            refValue = config.yInitialRef;
            lock (this.pidMutex)
            {
                this.yawPid = new YawPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
                this.pids[ch - 1] = this.yawPid;
            }

            //Throttle
            kp = config.tkp;
            ki = config.tki;
            kd = config.tkd;
            ch = Convert.ToInt32(config.tCh);
            offset = Convert.ToInt32(config.tOffset);
            spanFactor = config.tSpan;
            minVal = Convert.ToInt32(config.tMin);
            maxVal = Convert.ToInt32(config.tMax);
            meanVal = Convert.ToInt32(config.tMean);
            refValue = config.tInitialRef;
            lock (this.pidMutex)
            {
                this.throttlePid = new ThrottlePID(adcTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
                this.pids[ch - 1] = this.throttlePid;
            }
        }

        private void SetInfoText()
        {
			StreamReader sr = new StreamReader("PidConfig.txt");
			string line = null;
			int ch, offset, minVal, maxVal, meanVal;
			double spanFactor, imuTs = -1, adcTs = -1, kp, ki, kd, refValue;
			while((line = sr.ReadLine()) != null)
			{
				if(line.StartsWith("#") || line == "")
					continue;
				string[] words = line.Split(new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				switch(words[0])
				{
				case "imuTs":
					imuTs = double.Parse(words[1]);	
					break;
				case "adcTs":
					adcTs = double.Parse(words[1]);
					break;
				case "roll":
					kp = double.Parse(words[1]);
					ki = double.Parse(words[2]);
					kd = double.Parse(words[3]);
					ch = int.Parse(words[4]);
					offset = int.Parse(words[5]);
					spanFactor = double.Parse(words[6]);
					minVal = int.Parse(words[7]);
					maxVal = int.Parse(words[8]);
					meanVal = int.Parse(words[9]);
					refValue = double.Parse(words[10]);
					lock(this.pidMutex)
					{
						this.rollPid = new RollPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
						this.pids[ch-1] = this.rollPid;
					}
					break;
				case "pitch":
					kp = double.Parse(words[1]);
					ki = double.Parse(words[2]);
					kd = double.Parse(words[3]);
					ch = int.Parse(words[4]);
					offset = int.Parse(words[5]);
					spanFactor = double.Parse(words[6]);
					minVal = int.Parse(words[7]);
					maxVal = int.Parse(words[8]);
					meanVal = int.Parse(words[9]);
					refValue = double.Parse(words[10]);
					lock(this.pidMutex)
					{
						this.pitchPid = new PitchPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
						this.pids[ch-1] = this.pitchPid;
					}
					break;
				case "yaw":
					kp = double.Parse(words[1]);
					ki = double.Parse(words[2]);
					kd = double.Parse(words[3]);
					ch = int.Parse(words[4]);
					offset = int.Parse(words[5]);
					spanFactor = double.Parse(words[6]);
					minVal = int.Parse(words[7]);
					maxVal = int.Parse(words[8]);
					meanVal = int.Parse(words[9]);
					refValue = double.Parse(words[10]);
					lock(this.pidMutex)
					{
						this.yawPid = new YawPID(imuTs, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue);
						this.pids[ch-1] = this.yawPid;
					}
					break;
				case "throttle":
					kp = double.Parse(words[1]);
					ki = double.Parse(words[2]);
					kd = double.Parse(words[3]);
					ch = int.Parse(words[4]);
					offset = int.Parse(words[5]);
					spanFactor = double.Parse(words[6]);
					minVal = int.Parse(words[7]);
					maxVal = int.Parse(words[8]);
					meanVal = int.Parse(words[9]);
					refValue = double.Parse(words[10]);
					lock(this.pidMutex)
					{
						this.throttlePid = new ThrottlePID(adcTs, kp, ki, kd, offset, spanFactor,minVal,maxVal,meanVal, refValue);
						this.pids[ch-1] = this.throttlePid;
					}
					break;
				}
			}
            
        }
		
		public int GetChInWidth(int n)
		{
			int ans;
			byte val;
			lock(this.chInMutex)
		    {
				val = this.chIn[n-1];
			}
			lock(this.pidMutex)
		    {
				PID pid = this.pids[n-1];
				ans = (int)Math.Round(val*pid.GetSpan() + pid.GetOffset());
			}
			return ans;
		}

        public void SetChThrottle(AdcMessage adcMessage, int model)
        {
			double vel = adcMessage.tas;
			lock(this.pidMutex)
		    {
				if(this.throttlePid != null)
					this.throttlePid.SetValue(vel, model);
			}
        }

        public void SetChRoll(ImuEulerMessage eulerMessage)
        {
			double roll = eulerMessage.roll; 
			//roll = -roll;
			lock(this.pidMutex)
		    {
				if(this.rollPid != null)
        			this.rollPid.SetValue(roll);
			}
        }
  
		public void SetChPitch(ImuEulerMessage eulerMessage)
        {
         	double pitch = eulerMessage.pitch;   
			lock(this.pidMutex)
		    {
				if(this.pitchPid != null)
					this.pitchPid.SetValue(pitch);
			}
		}

        public void SetChYaw(ImuEulerMessage eulerMessage)
        {
			double yaw = eulerMessage.yaw;
			lock(this.pidMutex)
		    {
				if(this.yawPid != null)
					this.yawPid.SetValue(yaw);
			}
		}

		public byte GetCh(int n)
		{
			byte ans;
			lock(this.pidMutex)
		    {
				PID ch = this.pids[n-1];
				ans = ch.GetValue();
			}
			return ans;
		}
		
		public int GetChWidth(int n)
		{
			int ans;
			lock(this.pidMutex)
		    {
				PID ch = this.pids[n-1];
				ans =(int)Math.Round(ch.GetValue()*ch.GetSpan()+ch.GetOffset());
			}
			return ans;
		}
		
		public void SetCh(int n, byte val)
		{
			lock(this.pidMutex)
		    {
				PID ch = this.pids[n-1];
				ch.SetPwmIn(val);
			}
			lock(this.chInMutex)
			{
				this.chIn[n-1] = val;
			}
		}
		
		public void SetRef(Ctrl ctrl, double val)
		{
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						this.throttlePid.SetRefValue(val);
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						this.rollPid.SetRefValue(val);
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						this.pitchPid.SetRefValue(val);
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						this.yawPid.SetRefValue(val);
					break;
				}
			}
		}
		
		public int GetMeanValue(Ctrl ctrl)
		{
			int ans = -1;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					ans = this.throttlePid.GetMeanVal();
					break;
				case Ctrl.ROLL:
					ans = this.rollPid.GetMeanVal();
					break;
				case Ctrl.PITCH:
					ans = this.pitchPid.GetMeanVal();
					break;
				case Ctrl.YAW:
					ans = this.yawPid.GetMeanVal();
					break;
				}
			}
			return ans;
		}
		
		public double GetSpanValue(Ctrl ctrl)
		{
			double ans = 0;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ans = this.throttlePid.GetSpan();
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ans = this.rollPid.GetSpan();
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ans = this.pitchPid.GetSpan();
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ans = this.yawPid.GetSpan();
					break;
				}	
			}
			return ans;
		}
		
		public int GetOffsetValue(Ctrl ctrl)
		{
			int ans = 0;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ans = this.throttlePid.GetOffset();
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ans = this.rollPid.GetOffset();
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ans = this.pitchPid.GetOffset();
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ans = this.yawPid.GetOffset();
					break;
				}	
			}
			return ans;
		}
		
		public bool RefreshParam(Ctrl ctrl, PID.Param param, double val)
		{
			bool ok = false;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ok = this.throttlePid.RefreshParam(param, val);
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ok =  this.rollPid.RefreshParam(param, val);
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ok =  this.pitchPid.RefreshParam(param, val);
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ok =  this.yawPid.RefreshParam(param, val);
					break;
				}
			}
			return ok;
		}
		
		public double GetParam(Ctrl ctrl, PID.Param param)
		{
			double ans = double.MaxValue;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ans = this.throttlePid.GetParam(param);
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ans =  this.rollPid.GetParam(param);
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ans =  this.pitchPid.GetParam(param);
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ans =  this.yawPid.GetParam(param);
					break;
				}
			}
			return ans;
		}
		
		public bool Activate(Ctrl ctrl)
		{
			bool ans = false;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ans = this.throttlePid.ReStart();
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ans = this.rollPid.ReStart();
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ans = this.pitchPid.ReStart();
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ans = this.yawPid.ReStart();
					break;
				}
			}
			return ans;
		}
		
		public bool Deactivate(Ctrl ctrl)
		{
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						this.throttlePid.Deactivate();
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						this.rollPid.Deactivate();
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						this.pitchPid.Deactivate();
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						this.yawPid.Deactivate();
					break;
				}
			}
			return true;
		}
		
		public bool isActive(Ctrl ctrl)
		{
			bool ans = false;
			lock(this.pidMutex)
			{
				switch(ctrl)
				{
				case Ctrl.THROTTLE:
					if(this.throttlePid != null)
						ans = this.throttlePid.Active;
					break;
				case Ctrl.ROLL:
					if(this.rollPid != null)
						ans = this.rollPid.Active;
					break;
				case Ctrl.PITCH:
					if(this.pitchPid != null)
						ans = this.pitchPid.Active;
					break;
				case Ctrl.YAW:
					if(this.yawPid != null)
						ans = this.yawPid.Active;
					break;
				}
			}
			return ans;
		}
    }
}
