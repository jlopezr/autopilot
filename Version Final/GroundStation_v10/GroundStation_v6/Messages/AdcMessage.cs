using System;
using System.Collections.Generic;

using System.Text;

namespace GroundStation
{
	/// <summary>
	/// AdcMessage class manages ADC airborne telemetry data
	/// i.e. barometer, thermometer and pitot. Moreover, it
	/// calculates True Airspeed and barometric altitude (MSL)
	/// </summary>
    public class AdcMessage : Message
    {
		/// <summary>
		/// The barometer field
		/// </summary>
        public double barometer;
		
		/// <summary>
		/// The thermometer field
		/// </summary>
        public double thermometer;
		
		/// <summary>
		/// The pitot field
		/// </summary>
        public double pitot;
        
		/// <summary>
		/// The true airspeed (TAS)
		/// </summary>
        public double tas;
		
		/// <summary>
		/// The barometric altitude (MSL)
		/// </summary>
        public double altitude;
		
		/// <summary>
		/// Constant a0. Standard speed of sound at sea level. [m/s]  Esta en kt no en m/s
		/// </summary>
        private const double a0 = 661.4788;
		
		/// <summary>
		/// Constant T0. Standard temperature at sea level. [K]
		/// </summary>
        private const double T0 = 288.15;
		
		/// <summary>
		/// Constant p0. Standard static pressure at sea level. [Pa]
		/// </summary>
        private int P0 = 101325;
		
		/// <summary>
		/// Constant lambda. Standard thermal gradient. [K/m]
		/// </summary>
        private const double lambda = -0.0065;
		
		/// <summary>
		/// Constant R. Ideal gas constant  [m²/(s²·K)]
		/// </summary>
        private const int R = 287;
		
		/// <summary>
		/// Constant g. Standard gravity at sea level [m/s²]
		/// </summary>
        private const double g = 9.80665;
		
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GroundStation.AdcMessage"/> class.
		/// </summary>
		/// <param name='time'>
		/// Timestamp.
		/// </param>
		/// <param name='b'>
		/// Message as an array of bytes
		/// </param>
        public AdcMessage()
            : base()
        { }

		
		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <param name='bArray'>
		/// Message as an array of bytes
		/// </param>
        public override void CreateMessage(ulong time, byte[] bArray)
        {
            double b, t, p;
			this.time = time;
            //Array.Reverse(bArray, 1, 4);
            b = BitConverter.ToInt32(bArray, 1);
            //Array.Reverse(bArray, 5, 4);
            t = BitConverter.ToInt32(bArray, 5);
            //Array.Reverse(bArray, 9, 4);
            p = BitConverter.ToInt32(bArray, 9);
            this.ConvertData(b, t, p);
            this.CalculateExtras();
			
			/*//Console.WriteLine("Time: " + this.time);
			//Console.WriteLine("Bar: " + this.barometer.V);
			//Console.WriteLine("Temp: " + this.thermometer.V);
			//Console.WriteLine("Pitot: " + this.pitot.V);
			Console.WriteLine("Alt: " + this.altitude);
			//Console.WriteLine("TAS: " + this.tas);*/
        }
		
		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <param name='m'>
		/// Message as a string
		/// </param>
        public override void CreateMessage(string m)
        {
            string[] words = m.Split(new char[] { ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                double b, t, p;
                b = Convert.ToUInt16(words[1]);
                t = Convert.ToUInt16(words[2]);
                p = Convert.ToUInt16(words[3]);
                this.ConvertData(b, t, p);
                this.CalculateExtras();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in ADC: " + e.Message);
            }
        }
		
		
        private void ConvertData(double b, double t, double p)
        {
            //llegan multiplicados por 1000 para tener 3 decimales
            this.barometer = b/1000;  
            this.thermometer = t/1000;
            this.pitot = p/1000;
        }
		
		/// <summary>
		/// Calculates TAS and barometric altitude.
		/// </summary>
        private void CalculateExtras()
        {
            //tas[kt] = a0[kt] * sqrt(5*((qc[Hg]/P[Hg]+1)^2/7 - 1)*T[K]/T0[K]
            this.tas = a0 * Math.Sqrt(5 * (Math.Pow(this.pitot / this.barometer + 1, 2 / 7.0) - 1) * this.thermometer / T0);
            //Pz = P0*((T0+lambda*z)/T0)^(-g/(R*lambda))
            //z = T0*((P0/Pz)^(R*lambda/g)-1)/lambda
            this.altitude = T0 * (Math.Pow((P0 / this.barometer), R * lambda / g) - 1) / lambda;
        }
		
		/// <summary>
		/// Makes and exact copy of the adc message
		/// </summary>
		/// <returns>
		/// The copy.
		/// </returns>
		/// <param name='adc'>
		/// The adc message to copy
		/// </param>
        public static AdcMessage DeepCopy(AdcMessage adc)
        {
            AdcMessage ans = new AdcMessage();
            ans.time = adc.time;
            ans.altitude = adc.altitude;
            ans.tas = adc.tas;

            ans.barometer = adc.barometer;
            ans.thermometer = adc.thermometer;
            ans.pitot = adc.pitot;

            return ans;
        }

        public void ChangeP0(int refpress)
        {
            P0 = refpress;
        }

    }
}
