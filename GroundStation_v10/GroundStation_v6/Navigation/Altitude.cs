using System;

namespace GroundStation
{
	public class Altitude : UpperLayer
	{
		//Si la l'altitud actual difereix menys
		//de deltaAlt metres de l'objectiu assumim
		//que ja hem arribat a l'objectiu
		private const int deltaAlt = 2;
		
		private double selAlt;
		
		private double minPitch;
		private double maxPitch;
		
		//El contructor seteja la referencia inicial
		//del pid
		public Altitude (double initialRef, double ts, double kp, double ki, double kd, double maxPitch, double minPitch)
			: base(ts, kp, ki, kd)
		{
			this.selAlt = initialRef;
			this.minPitch = minPitch;
			this.maxPitch = maxPitch;
		}
		
		//Norma: Només pujo si la velocitat està per sobre
		//d'un cert llindar.
		//Norma: Els canvis d'altitud es controlen 
		//directament amb el pitch, sense tocar el throttle
		public override void SetParam (double upperParam)
		{
			this.selAlt = upperParam;
		}
		
		public override void GetRef ()
		{
			if(!this.activated)
				return;
			
			double currTas = this.ga.Adc.tas;
			double currAlt = this.ga.Adc.altitude;
			double currPitch = this.ga.Imu.pitch;
			
			//Positive if climbing, otherwise negative 
			double diffAlt = this.selAlt - currAlt;
			
			//En el cas que estiguem a l'altitud desitjada,
			//tornem double.MaxValue per indicar que no 
			//volem canviar la referencia.
			if(Math.Abs(diffAlt) < 2)
			{
				this.pid.SetRef(PIDManager.Ctrl.PITCH, double.MaxValue);
				return;
			}
			//En el cas en que la velocitat no estigui per
			//sobre d'un cert llindar i volguem pujar, 
			//Baixem el pitx un grau si aquest es positiu 
			//i avisem per consola del nou valor de pitch
			double ans;
			if(diffAlt > 0 && currTas < (this.ap.stallTas + 5))
			{
				if (currPitch > 0)
					ans = currPitch -1;
				else 
					ans = 0;
				Console.WriteLine("WARNING: Altitude not set due to low airspeed --> pitch reference set to " + ans + " degrees");
				this.pid.SetRef(PIDManager.Ctrl.PITCH, ans);
				return;
			}
			ans = this.RefreshPid(diffAlt);
			ans = ans > this.maxPitch ? this.maxPitch : ans;
			ans = ans < this.minPitch ? this.minPitch : ans;
			
			this.pid.SetRef(PIDManager.Ctrl.PITCH, ans);
            //Console.WriteLine("Pitch: {0}", ans);
            //Console.WriteLine("Alt: {0}", currAlt);
		}
	}
}

