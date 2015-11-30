using System;

using System.Text;

namespace GroundStation
{
	public class GpsPosMessage : Message
	{
		/// <summary>
		/// The latitude field
		/// </summary>
        public double latitude;
		
		/// <summary>
		/// The longitude field
		/// </summary>
        public double longitude;
		
		/// <summary>
		/// The ground speed field
		/// </summary>
        public double gndSpeed;
		
		/// <summary>
		/// The track angle field
		/// </summary>
        public double trackAngle;
		
		public double latDev;
		public double distDest;
		
		/// <summary>
		/// Position object (lat, lon, UTM_X, UTM_Y)
		/// </summary>
        public WgsPoint pos;
		
		
		
		public GpsPosMessage ()
		: base()
		{ }
		
		/// <summary>
		/// Creates the message.
		/// </summary>
		/// <param name='b'>
		/// Message as a byte array
		/// </param>
        public override void CreateMessage(ulong time, byte[] b)
        {
			try{
				this.time = time;
                byte[] c = new byte[b.Length - 1];

                for (int i = 1; i < b.Length; i++)
                {
                    c[i - 1] = b[i];
                }
                char[] strgps = Encoding.Unicode.GetChars(c);
                string m = new string(strgps);
				//Console.WriteLine("GPS POS: " + m);
				string[] words = m.Split('.'); //Partimos por puntos para no separar decimales
				
				double lat = double.Parse(words[3]);
				lat /= 100.0;
				double latTrunc = Math.Truncate(lat);
				lat = (lat - latTrunc)/60.0*100.0;
				lat += latTrunc;
				lat = words[4] == "N" ? lat : -lat;
	 			this.latitude = lat;
				
				double lon = double.Parse(words[5]);
				lon /= 100.0;
				double lonTrunc = Math.Truncate(lon);
				lon = (lon - lonTrunc)/60.0*100.0;
				lon += lonTrunc;
				lon = words[6] == "E" ? lon : -lon;
				this.longitude = lon;
				
				this.gndSpeed = double.Parse(words[7]) * 1852.0/3600.0; //kt a m/s
				
				double track = double.Parse(words[8]);
				if(track > 180.0)
				{
					track -= 360.0;
				}
				this.trackAngle = track;
				
				this.pos = new WgsPoint(this.latitude, this.longitude, 0);
			}
			catch(Exception)
			{}
		}
		
		public override void CreateMessage (string m)
		{
			throw new NotImplementedException ();
		}
		
		public GpsPosMessage DeepCopy()
		{
			GpsPosMessage ans = new GpsPosMessage();
			ans.time = this.time;
			ans.latitude = this.latitude;
			ans.longitude = this.longitude;
			ans.gndSpeed = this.gndSpeed;
			ans.trackAngle = this.trackAngle;
			ans.pos = this.pos.DeepCopy();
			ans.latDev = this.latDev;
			ans.distDest = this.distDest;
			//ans.magVar = Field2.DeepCopy(this.magVar);
			return ans;
		}
	}
}

