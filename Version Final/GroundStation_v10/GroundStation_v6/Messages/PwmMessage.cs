using System;
using System.Collections.Generic;

using System.Text;

namespace GroundStation
{
	/// <summary>
	/// PwmMessage class manages airborne received PWM 
	/// data for each servo channel i.e. ch1, ch2, ch3
	/// ch4.
	/// </summary>    
	public class PwmMessage : Message
    {
		/// <summary>
		/// The ch1 field
		/// </summary>
        public double ch1;
		
		/// <summary>
		/// The ch2 field
		/// </summary>
        public double ch2;
		
		/// <summary>
		/// The ch3 field 
		/// </summary>
        public double ch3;
		
		/// <summary>
		/// The ch4 field
		/// </summary>
        public double ch4;
		

		/// <summary>
		/// Initializes a new instance of the <see cref="GroundStation.PwmMessage"/> class.
		/// </summary>
		/// <param name='time'>
		/// Timestamp
		/// </param>
		/// <param name='b'>
		/// Message as an array of bytes
		/// </param>
        public PwmMessage()
            : base()
        { }

		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <param name='b'>
		/// Message as an array of bytes
		/// </param>
        public override void CreateMessage(ulong time, byte[] b)
        {
            Array.Reverse(b, 1, 2);
            this.ch1 = BitConverter.ToUInt16(b, 1);
            Array.Reverse(b, 3, 2);
            this.ch2 = BitConverter.ToUInt16(b, 3);
            Array.Reverse(b, 5, 2);
            this.ch3 = BitConverter.ToUInt16(b, 5);
            Array.Reverse(b, 7, 2);
            this.ch4 = BitConverter.ToUInt16(b, 7);
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
                this.ch1 = Convert.ToUInt16(words[1]);
                this.ch2 = Convert.ToUInt16(words[2]);
                this.ch3 = Convert.ToUInt16(words[3]);
                this.ch4 = Convert.ToUInt16(words[4]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in PWM: " + e.Message);
            }
        }
		
		/// <summary>
		/// Makes an exact copy of pwm
		/// </summary>
		/// <returns>
		/// The copy.
		/// </returns>
		/// <param name='pwm'>
		/// The pwm to be copied.
		/// </param>
        public static PwmMessage DeepCopy(PwmMessage pwm)
        {
            PwmMessage ans = new PwmMessage();
            ans.ch1 = pwm.ch1;
            ans.ch2 = pwm.ch2;
            ans.ch3 = pwm.ch3;
            ans.ch4 = pwm.ch4;
            return ans;
        }
    }
}
