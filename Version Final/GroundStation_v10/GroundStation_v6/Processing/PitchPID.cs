using System;

namespace GroundStation
{
	public class PitchPID : PID
	{
		
		public PitchPID (double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
			: base(ts, kp, ki, kd, offset, spanFactor, minVal, maxVal, meanVal, refValue)
		{
            this.Name = "Pitch";
        }

		public override void SetValue(double input)
		{
			base.SetValue(input);
            //Console.WriteLine("Pitch {0}", input);
		}
	}
}

