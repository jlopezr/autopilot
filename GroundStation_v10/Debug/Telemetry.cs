using System;
using System.Threading;

using GroundStation;

namespace Debug
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
			this.imuIn = new ImuReader("imu.txt");
			
			this.imuTh = new Thread(new ThreadStart(this.ImuLoop));
			this.imuTh.Start();
		}
		
		protected virtual void OnNewData(EventArgs e)
		{
			if(this.NewData != null)
				this.NewData(this, e);
		}
			
		private void ImuLoop()
		{
			while(true)
			{
				Thread.Sleep(20);
				this.data = this.imuIn.Data;
				this.OnNewData(EventArgs.Empty);
			}
		}
	}
}

