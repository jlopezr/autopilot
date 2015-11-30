using System;
using System.IO;

namespace GroundStation
{
	public class AircraftPerformance
	{
		//Stall Speed [kt]
		public double stallTas;
		//Maximum bank angle [deg]
		public double maxBank;
		//Pitch angles [maxDown, maxUp] eg. -15, 10
		public double[] maxPitch;
		//
		
		
		public static string path = "AircraftPerformance.txt";
		
		private static AircraftPerformance instance = null;
		
		public static AircraftPerformance GetInstance()
		{
            if (instance == null)
                Console.WriteLine("Error: null AircraftPerformance instance");
			return instance;
		}

        public static AircraftPerformance GetInstance(int m)
        {
            if (instance == null)
            {
                ACPerformance p;
                if (m == 1) //Models are used here
                {
                    p = new C172ACPerformance();
                    instance = new AircraftPerformance(p);
                }
                if (m == 2)
                {
                    p = new RCACPerformance();
                    instance = new AircraftPerformance(p);
                }
                if (m == 3)
                {
                    p = new CirrusACPerformance();
                    instance = new AircraftPerformance(p);
                }
                if (m == 20)
                {
                    p = new QXACPerformance();
                    instance = new AircraftPerformance(p);
                }
                
            }

            return instance;
        }
		
        private AircraftPerformance (ACPerformance perf)
        {
            this.stallTas = perf.StallSpeed;
            this.maxBank = perf.MaxBank;
        }
        


		private void AircraftPerformanceText ()
		{
			StreamReader sr = new StreamReader(path);
			
			string line = "";
			
			while((line = sr.ReadLine()) != null)
			{
				if(line.StartsWith("#"))
					continue;
				string[] words = line.Split(new char[]{' ', '\t'});
				if(words.Length != 2)
					continue;
				switch(words[0])
				{
				case "stallTas":
					this.stallTas = double.Parse(words[1]);
					break;
				case "maxBank":
					this.maxBank = double.Parse(words[1]);
					break;
				}
			}
		}
		
		
	}
}

