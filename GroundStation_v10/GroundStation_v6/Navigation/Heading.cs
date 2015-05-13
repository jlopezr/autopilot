using System;

namespace GroundStation
{
	public class Heading : UpperLayer
	{
		private double selHeading;
		private bool cw;
		
		private double maxRoll;
		
		public Heading (double initialRef, double ts, double kp, double ki, double kd, double maxRoll)
			: base(ts, kp, ki, kd)
		{
			this.selHeading = initialRef;
			this.maxRoll = maxRoll;
		}
		
		public override void SetParam (double upperParam)
		{
			if(upperParam > 180.0)
				upperParam -= 360.0;
			this.selHeading = upperParam;
			this.GetRef();	
		}
		
		//
		public override void GetRef ()
		{
			if(!this.activated)
				return;
			double currHeading = this.ga.Imu.yaw;
			//double diffHeading = (double)((this.selHeading - currHeading)%180);
            double diffHeading = (double)((this.selHeading - currHeading));
            if(diffHeading < -180)
            {
                diffHeading = diffHeading + 360;
            }
            else if (diffHeading > 180)
            {
                diffHeading = diffHeading - 360;
            }
            //Console.WriteLine("Selected HDG: {1}  HDG diff: {0}  Actual HDG: {2}", diffHeading, selHeading, currHeading);
			double vStall = this.ap.stallTas;
			double currTas = this.ga.Adc.tas;
			double currRoll = this.ga.Imu.roll;
			
			if(diffHeading < 0)
				this.cw = false;
			else
				this.cw = true;
			double ans;
			if(currTas < vStall + 5)
			{
				
				if (currRoll > 1)
					ans = currRoll -1;
				else if (currRoll < 1)
					ans = currRoll + 1;
				else
					ans = 0;
				Console.WriteLine("WARNING: Heading change not set due to low airspeed --> roll reference set to " + ans + " degrees");
				this.pid.SetRef(PIDManager.Ctrl.ROLL, ans);
				this.pid.SetRef(PIDManager.Ctrl.YAW, this.selHeading);
			}
			ans = this.RefreshPid(diffHeading);
			
			ans = ans > this.maxRoll ? this.maxRoll : ans;
			ans = ans < -this.maxRoll ? -this.maxRoll : ans;
			
			this.pid.SetRef(PIDManager.Ctrl.ROLL, ans);
			this.pid.SetRef(PIDManager.Ctrl.YAW, this.selHeading);
		}
	}
}

