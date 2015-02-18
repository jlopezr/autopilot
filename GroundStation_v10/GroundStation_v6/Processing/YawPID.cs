using System;

namespace GroundStation
{
	public class YawPID : PID
	{
		public YawPID ()
			: base()
		{ }
		
		public YawPID (double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
			: base(ts, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue)

		{ }
		
		public override void SetValue(double input)
		{
			lock(this.mutex)
			{
				int sign = Math.Sign(input);
				input = (input - this.refValue - this.initialVal);
				if(input > 180)
				{
					input = (360 - (input));
					if(sign > 0)	
						input = -input;
				}
				else if(input < -180)
					input = (360 + (input));
		        
				this.RefreshPid(input);
			}
		}
	}
}

