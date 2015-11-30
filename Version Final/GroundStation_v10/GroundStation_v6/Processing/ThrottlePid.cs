using System;

namespace GroundStation
{
	public class ThrottlePID : PID
	{
		
		public ThrottlePID (double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
			: base(ts, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue)
		{
            this.Name = "Throttle";
        }

        public override void SetValue(double input, int model)
		{
            input = -(input - this.refValue - this.initialVal);
            if(model>=20 || model==3)//Copters and jets
            {
                this.RefreshPid(input);
            }
            else//Planes
            {
                this.RefreshPidThrottle(input);
            }
            //Console.WriteLine("Speed:{0}", input);
			
        }
	}
}

