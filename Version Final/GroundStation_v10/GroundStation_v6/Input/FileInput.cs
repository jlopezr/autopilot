using System;
using System.Threading;
using System.Collections.Generic;

namespace GroundStation
{
	public class LogInput : Input
	{
		private Telemetry t;
		
		private List<byte> data;
		
		private object mutex;
		
		public LogInput ()
		{
			this.mutex = new object();
			
			this.data = new List<byte>();
			
			this.t = new Telemetry();
			this.t.NewData += NewDataListener;
		}

		private void  NewDataListener (object sender, EventArgs e)
		{
			Monitor.Enter(this.mutex);
			this.data.AddRange(t.Data);
			Monitor.Pulse(this.mutex);
			Monitor.Exit(this.mutex);
		}
		
		public override byte[] ReadNBytes (int n)
		{
			Monitor.Enter(this.mutex);
			while(this.data.Count < n)
				Monitor.Wait(this.mutex);
			List<byte> ans = this.data.GetRange(0,n);
			this.data.RemoveRange(0,n);
			Monitor.Exit(this.mutex);
			return ans.ToArray();
		}
	}
}

