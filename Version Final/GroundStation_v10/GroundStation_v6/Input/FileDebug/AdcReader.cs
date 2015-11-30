using System;
using System.IO;
using System.Collections.Generic;

namespace GroundStation
{
	public class AdcReader : FileInput
	{
		private byte time;
		private double barometer;
		private double temperature;
		private double pitot;
		
		public AdcReader (string path)
			: base(path)
		{
			this.time = 0;
			this.data = new byte[11];
			this.ReadLine();
			this.ToByteArray();
		}
		
		protected override bool ReadLine()
		{
			string[] words = this.sr.ReadLine().Split('\t');
			if(words.Length != 6)
				return false;
			this.time = (byte)(int.Parse(words[0]) - this.time);
			this.barometer = double.Parse(words[1]);
			this.temperature = double.Parse(words[2]);
			this.pitot = double.Parse(words[3]);
			return true;
		}
		
		protected override void ToByteArray()
		{
			List<byte> ans = new List<byte>();
			double b, t, p;
			b = 0.02178 * this.barometer - 229.9;
			t = 0.01 * this.temperature - 2.73;
			p = 0.001 * this.pitot - 1;
			ushort bb = (ushort)Math.Round(65536.0 / 5.0 * b);
			ushort tt = (ushort)Math.Round(65536.0 / 5.0 * t);
			ushort pp = (ushort)Math.Round(65536.0 / 5.0 * p);
			
			byte[] aux;
			
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)3);
			
			ans.Add(this.time);
			
			aux = BitConverter.GetBytes(bb);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			aux = BitConverter.GetBytes(tt);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			aux = BitConverter.GetBytes(pp);
			Array.Reverse(aux);
			ans.AddRange(aux);
			
			this.data = ans.ToArray();
		}
	}
}

