using System;

namespace GroundStation
{
	public abstract class UpperLayer
	{
		protected PIDManager pid;
		protected GlobalArea ga;
		protected AircraftPerformance ap;
		protected bool activated;
		
		private double ts;
		private double kp;
		public double Kp
		{
			get
			{
				double ans;
				lock(this.mutex)
				{
					ans = this.kp;
				}
				return ans;
			}
			set
			{
				lock(this.mutex)
				{
					this.kp = value;
				}
			}
		}
		
		private double ki;
		private double kd;
		
		private double acc;
		private double diff;
		private double prev;
		
		private object mutex;
		
		public UpperLayer (double ts, double kp, double ki, double kd)
		{
			this.pid = PIDManager.GetInstance();
			this.ga = GlobalArea.GetInstance();
			this.ap = AircraftPerformance.GetInstance();
			this.activated = false;
			
			this.ts = ts;
			this.kp = kp;
			this.ki = ki;
			this.kd = kd;
			
			this.acc = 0;
			this.diff = 0;
			this.prev = double.MinValue;
			
			this.mutex = new object();
		}
		
		public abstract void SetParam(double upperParam);
		public abstract void GetRef();
		
		public void activate()
		{
			this.activated = true;
			this.acc = 0;
			this.diff = 0;
			this.prev = double.MinValue;
		}
		
		public void deactivate()
		{
			this.activated = false;
		}
		
		protected double RefreshPid(double input)
		{
			if(this.prev != double.MinValue)
				this.diff = input - this.prev;
			this.acc += input;
			double ans;
			lock(this.mutex)
			{
				ans = this.kp*input + this.ki * this.ts * this.acc + this.kd * diff / this.ts;
			}
			this.prev = input;
			return ans;
		}


        public abstract double GetSel();
        
	}
}

