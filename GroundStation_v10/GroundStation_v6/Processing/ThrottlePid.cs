using System;

namespace GroundStation
{
	public class ThrottlePID : PID
	{
		public ThrottlePID ()
			: base()
		{ }
		
		public ThrottlePID (double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
			: base(ts, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue)

		{ }
		
		public override void SetValue(double input)
		{
			input = -(input - this.refValue - this.initialVal);
			this.RefreshPid(input);
		}
	}
}

