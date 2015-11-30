using System;
using System.Threading;

namespace GroundStation
{
	public delegate void ChangedEventHandler(object sender, EventArgs e);
	public class Telemetry
	{
		public event ChangedEventHandler NewData;
		
		private Thread adcTh;
		private Thread gpsTh;
		private Thread imuTh;
		
		private FileInput adcIn;
		private FileInput gpsIn;
		private FileInput imuIn;
		
		private object mutex;
		
		private byte[] data;
		
		public byte[] Data
		{
			get
			{
				byte[] ans;
				lock(this.mutex)
				{
					ans = new byte[this.data.Length];
					this.data.CopyTo(ans,0);
				}
				return ans;
			}
			set
			{
				lock(this.mutex)
				{
					this.data = new byte[value.Length];
					value.CopyTo(this.data,0);
				}
			}
		}
		
		public Telemetry()
		{
			this.mutex = new object();
			
			this.imuIn = new ImuReader("Input/imu.txt");
			this.imuTh = new Thread(new ThreadStart(this.ImuLoop));
			this.imuTh.Start();
			
			this.gpsIn = new GpsReader("Input/gps.txt");
			this.gpsTh = new Thread(new ThreadStart(this.GpsLoop));
			this.gpsTh.Start();
			
			this.adcIn = new AdcReader("Input/adc.txt");
			this.adcTh = new Thread(new ThreadStart(this.AdcLoop));
			this.adcTh.Start();
		}
		
		protected virtual void OnNewData(EventArgs e)
		{
			this.NewData(this, e);
		}
			
		private void ImuLoop()
		{
			Thread.Sleep(1000);
			while(true)
			{
				if(this.imuIn.end)
					break;
				//Thread.Sleep(20);
				this.Data = this.imuIn.Data;
				//Console.WriteLine("IMU Message generated");
				this.OnNewData(EventArgs.Empty);
			}
		}
		
		private void GpsLoop()
		{
			Thread.Sleep(1100);
			while(true)
			{
				if(this.gpsIn.end)
					break;
				//Thread.Sleep(200);
				this.Data = this.gpsIn.Data;
				//Console.WriteLine("GPS Message generated");
				this.OnNewData(EventArgs.Empty);
				
			}
		}
		
		private void AdcLoop()
		{
			Thread.Sleep(1000);
			while(true)
			{
				if(this.adcIn.end)
					break;
				//Thread.Sleep(500);
				this.Data = this.adcIn.Data;
				//Console.WriteLine("ADC Message generated");
				this.OnNewData(EventArgs.Empty);
			}
		}
	}
}

