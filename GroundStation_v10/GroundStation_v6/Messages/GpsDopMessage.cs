using System;

namespace GroundStation
{
	public class GpsDopMessage : Message
	{
		
		public Field2 pdop;
		public Field2 hdop;
		public Field2 vdop;
		
		private const int pdopMin = 0;
		private const int pdopMax = 100;
		private const int pdopPrevVal = 10;
		private const int pdopMaxVar = 900;
		
		private const int hdopMin = 0;
		private const int hdopMax = 100;
		private const int hdopPrevVal = 10;
		private const int hdopMaxVar = 900;
		
		private const int vdopMin = 0;
		private const int vdopMax = 100;
		private const int vdopPrevVal = 10;
		private const int vdopMaxVar = 900;
		
		
		public GpsDopMessage ()
			: base()
		{
			this.pdop = new Field2(pdopMin, pdopMax, pdopPrevVal, pdopMaxVar);
			this.hdop = new Field2(hdopMin, hdopMax, hdopPrevVal, hdopMaxVar);
			this.vdop = new Field2(vdopMin, vdopMax, vdopPrevVal, vdopMaxVar);
		}
		
		public override void CreateMessage(ulong time, byte[] b)
		{
			try{
			this.time = time;
			char[] c = new char[b.Length];
			
			for(int i = 1; i < b.Length; i++)
			{
				c[i] = (char)b[i];
			}
			string m = new string(c);
			string[] words = m.Split(new char[]{',', '*'});
			
			//this.pdop.V = double.Parse(words[15]);
			//this.hdop.V = double.Parse(words[16]);
			//this.vdop.V = double.Parse(words[17]);
			
			this.pdop.V = 0;
			this.hdop.V = 0;
			this.vdop.V = 0;
			}
			catch(Exception)
			{}
		}
		
		public override void CreateMessage (string m)
		{
			throw new NotImplementedException ();
		}
		
		public GpsDopMessage DeepCopy()
		{
			GpsDopMessage ans = new GpsDopMessage();
			ans.time = this.time;
			ans.pdop = Field2.DeepCopy(this.pdop);
			ans.hdop = Field2.DeepCopy(this.hdop);
			ans.vdop = Field2.DeepCopy(this.vdop);
			return ans;
		}
	}
}

