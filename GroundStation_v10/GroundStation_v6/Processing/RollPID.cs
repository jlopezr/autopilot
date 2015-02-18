using System;

namespace GroundStation
{
	public class RollPID : PID
	{
		public RollPID ()
			: base()
		{ }
		
		public RollPID (double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
			: base(ts, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue)

		{ }
		
		public override void SetValue(double input)
		{
			base.SetValue(input);
		}
	}
}

