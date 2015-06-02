using System;

namespace GroundStation
{
	//La velocitat la tenim directament relacionada
	//amb el throttle a través de la funció de
	//transferència de la planta.
	public class XSpeed : UpperLayer
	{

        private double minPitch;
        private double maxPitch;

		//Si la velocitat respecte el vent
		//difereix en menys de 2 m/s considerem
		//que ja l'hem assolit
		private const int deltaSpeed = 2;
		
		//Velocitat seleccionada
		private double selSpeed;


        public XSpeed(double initialRef, double ts, double kp, double ki, double kd, double maxPitch, double minPitch)
			: base(ts, kp, ki, kd)
		{
			this.selSpeed = initialRef;
            this.minPitch = minPitch;
            this.maxPitch = maxPitch;
		}
		
		public override void SetParam (double upperParam)
		{
			this.selSpeed = upperParam;
		}
		
		public override void GetRef ()
		{
			if(!activated)
				return;

            double currTas = this.ga.Adc.tas;
            double diffSpeed = this.selSpeed - currTas;

            double ans;
            ans = this.RefreshPid(diffSpeed);
            //Console.WriteLine("diffspeed {0}", diffSpeed);
            ans = ans > this.maxPitch ? this.maxPitch : ans;
            ans = ans < this.minPitch ? this.minPitch : ans;
			this.pid.SetRef(PIDManager.Ctrl.PITCH, ans);
            //Console.WriteLine("Selspeed {0}", this.selSpeed);
		}
	}
}

