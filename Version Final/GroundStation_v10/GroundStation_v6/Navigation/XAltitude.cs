using System;

namespace GroundStation
{
	public class XAltitude : UpperLayer
	{
		//Si la l'altitud actual difereix menys
		//de deltaAlt metres de l'objectiu assumim
		//que ja hem arribat a l'objectiu
		private const int deltaAlt = 2;
		
		private double selAlt;
		
		
		
		//El contructor seteja la referencia inicial
		//del pid
		public XAltitude (double initialRef, double ts, double kp, double ki, double kd)
			: base(ts, kp, ki, kd)
		{
			this.selAlt = initialRef;
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
			
			double currAlt = this.ga.Adc.altitude;
			
			//Positive if climbing, otherwise negative 
			double diffAlt = this.selAlt - currAlt;
			
			//En el cas que estiguem a l'altitud desitjada,
			//tornem double.MaxValue per indicar que no 
			//volem canviar la referencia.
			if(Math.Abs(diffAlt) < 1)
			{
                this.pid.SetRef(PIDManager.Ctrl.THROTTLE, double.MaxValue);
				return;
			}
			
			double ans;
			
			ans = this.RefreshPid(diffAlt);
			
			this.pid.SetRef(PIDManager.Ctrl.THROTTLE, ans);
            //Console.WriteLine("Pitch: {0}", ans);
            //Console.WriteLine("Alt: {0}", currAlt);
		}

        public override double GetSel()
        {
            return selAlt;
        }
	}
}

