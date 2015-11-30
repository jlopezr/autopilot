using System;
using System.Collections.Generic;

namespace GroundStation
{
	public class ImuReader : FileInput
	{
		private byte time;
		private double roll;
		private double pitch;
		private double yaw;
		
		public ImuReader (string path)
			: base(path)
		{
			this.data = new byte[17];
			this.time = 0;
			this.ReadLine();
			this.ToByteArray();
		}
		
		protected override bool ReadLine()
		{
			string[] words = this.sr.ReadLine().Split('\t');
			if(words.Length != 4)
				return false;
			this.time = (byte)(int.Parse(words[0]) - this.time);
			this.roll = double.Parse(words[1]);
			this.pitch = double.Parse(words[2]);
			this.yaw = double.Parse(words[3]);
			return true;
		}
		
		protected override void ToByteArray()
		{
			List<byte> ans = new List<byte>();
			int r = (int)Math.Round((this.roll + 180.0) * 10000.0);
			int p = (int)Math.Round((this.pitch + 180.0) * 10000.0);
			int y = (int)Math.Round((this.yaw + 180.0) * 10000.0);
			
			byte[] aux;
			
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)0);
			
			ans.Add(this.time);
			
			aux = BitConverter.GetBytes(r);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			aux = BitConverter.GetBytes(p);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			aux = BitConverter.GetBytes(y);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			this.data = ans.ToArray();

		}
	}
}

