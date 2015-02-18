using System;
using System.Collections.Generic;

namespace GroundStation
{
	public class GpsReader : FileInput
	{
		private byte time;
		private double latitude;
		private double longitude;
		private double gndSpeed;
		private double trackAngle;
		
		public GpsReader (string path)
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
			if(words.Length != 5)
				return false;

			this.time = (byte)(int.Parse(words[0]) - this.time);
			this.latitude = double.Parse(words[1]);
			this.longitude = double.Parse(words[2]);
			this.gndSpeed = double.Parse(words[3]);
			this.trackAngle = double.Parse(words[4]);
			//this.gndSpeed = 0;
			//this.trackAngle = 0;
			return true;
		}
		
		protected override void ToByteArray()
		{
			List<byte> ans = new List<byte>();
			
			double lat, latTrunc, latDec;
			double lon, lonTrunc, lonDec;
			
			latTrunc = Math.Truncate(this.latitude);
			latDec = this.latitude - latTrunc;
			latTrunc *= 100;
			latDec *=60;
			lat = latTrunc + latDec;
			
			lonTrunc = Math.Truncate(this.longitude);
			lonDec = this.longitude - lonTrunc;
			lonTrunc *= 100;
			lonDec *=60;
			lon = lonTrunc + lonDec;
			
			int gnd = (int)Math.Round(this.gndSpeed * 3600.0 / 1852.0);
			
			int track = (int)Math.Round(this.trackAngle * 100);
			if(track<0)
				track += 360;
			
			string str = "$GPRMC,,,"+lat+",N,"+lon+",E,"+gnd+","+track;
			
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)1);
			ans.Add((byte)7);
			ans.Add((byte)(str.Length+5));
			
			ans.Add(this.time);
			
			ans.AddRange (System.Text.Encoding.ASCII.GetBytes(str));
			
			this.data = ans.ToArray();
		}
	}
}

