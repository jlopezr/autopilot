using System;
using System.Collections.Generic;
using System.Text;

namespace GroundStation
{
	/// <summary>
	/// ImuEulerMessage class manages the airborne IMU telemetry data
	/// i.e. roll, pitch, yaw. Moreover, it also manages the 3-axis
	/// accelerometer raw data.
	/// </summary>
    public class ImuEulerMessage : Message
    {
		/// <summary>
		/// The roll field
		/// </summary>
        public double roll;
		
		/// <summary>
		/// The pitch field
		/// </summary>
        public double pitch;
		
		/// <summary>
		/// The yaw field.
		/// </summary>
        public double yaw;
		
		
		
		/// <summary>
		/// Initializes a new instance of the <see cref="GroundStation.ImuEulerMessage"/> class.
		/// </summary>
		/// <param name='time'>
		/// Timestamp
		/// </param>
		/// <param name='b'>
		/// Message as a byte array
		/// </param>
        public ImuEulerMessage()
        : base ()
        { }
		
		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <param name='b'>
		/// Message as a byte array
		/// </param>
        public override void CreateMessage(ulong time, byte[] b)
        {
			this.time = time;
			//Console.WriteLine("Time: " + this.time);
            //Array.Reverse(b, 1, 4);
            this.roll = BitConverter.ToUInt32(b, 0) / 10000.0 - 180.0;
			
            //Array.Reverse(b, 5, 4);
            double val = BitConverter.ToUInt32(b, 4) / 10000.0 - 180.0;
			this.pitch = val;
            
			//Array.Reverse(b, 9, 4);
            double valy = BitConverter.ToUInt32(b, 8) / 10000.0;
            if (valy > 180)
            {
                valy = valy - 360;
            }
            this.yaw = valy;
			
			//Console.WriteLine("Roll: " + this.roll);
			//Console.WriteLine("Pitch: " + this.pitch);
			//Console.WriteLine("Yaw: " + this.yaw);
            
			
			/*Array.Reverse(b, 13, 4);
            this.accelX.V = BitConverter.ToInt32(b, 13) / 2560000.0;
            Array.Reverse(b, 17, 4);
            this.accelY.V = BitConverter.ToInt32(b, 17) / 2560000.0;
            Array.Reverse(b, 21, 4);
            this.accelZ.V = BitConverter.ToInt32(b, 21) / 2560000.0;*/
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
                this.roll = Convert.ToDouble(words[1]);
                this.pitch = Convert.ToDouble(words[2]);
                this.yaw = Convert.ToDouble(words[3]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in IMU: " + e.Message);
            }
        }
		
		/// <summary>
		/// Makes an exact copy of imu
		/// </summary>
		/// <returns>
		/// The copy.
		/// </returns>
		/// <param name='imu'>
		/// The imu message to be copied
		/// </param>
        public static ImuEulerMessage DeepCopy(ImuEulerMessage imu)
        {
            ImuEulerMessage ans = new ImuEulerMessage();
			ans.time = imu.time;
            ans.roll = imu.roll;
            ans.pitch = imu.pitch;
            ans.yaw = imu.yaw;

            return ans;
        }
    }
}
