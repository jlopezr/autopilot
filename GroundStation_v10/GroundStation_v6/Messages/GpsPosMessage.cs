using System;

namespace GroundStation
{
	public class GpsPosMessage : Message
	{
		/// <summary>
		/// The latitude field
		/// </summary>
        public Field2 latitude;
		
		/// <summary>
		/// The longitude field
		/// </summary>
        public Field2 longitude;
		
		/// <summary>
		/// The ground speed field
		/// </summary>
        public Field2 gndSpeed;
		
		/// <summary>
		/// The track angle field
		/// </summary>
        public Field2 trackAngle;
		
		public double latDev;
		public double distDest;
		
		/// <summary>
		/// Position object (lat, lon, UTM_X, UTM_Y)
		/// </summary>
        public WgsPoint pos;
		
		/// <summary>
		/// Minimum expected latitude [deg]
		/// </summary>
        private const int latMin = 40;
		
		/// <summary>
		/// Maximum expected latitude [deg]
		/// </summary>
        private const int latMax = 43;
		
		/// <summary>
		/// Initial previous latitude value [deg]
		/// </summary>
        private const int latPrevValue = 41;
		
		/// <summary>
		/// Initial previous latitude value [deg]
		/// </summary>
        private const int latPrevPrevValue = 42;
		
		/// <summary>
		/// Maximum  expected latitude variation [deg/sample]
		/// </summary>
        private const double latMaxVar = 0.02;
		
		/// <summary>
		/// Minimum expected longitude [deg]
		/// </summary>
        private const int lonMin = 2;
		
		/// <summary>
		/// Maximum expected longitude [deg]
		/// </summary>
        private const int lonMax = 5; 
		
		/// <summary>
		/// Initial previous longitude value [deg]
		/// </summary>
        private const int lonPrevValue = 3;
		
		/// <summary>
		/// Initial previous longitude value [deg]
		/// </summary>
        private const int lonPrevPrevValue = 3;
		
		/// <summary>
		/// Maximum expected longitude variation [deg/sample]
		/// </summary>
        private const double lonMaxVar = 0.02;
		
		/// <summary>
		/// Minimum expected ground speed [m/s]
		/// </summary>
        private const int gndSpeedMin = 0;
		
		/// <summary>
		/// Maximum expected ground speed [m/s]
		/// </summary>
        private const int gndSpeedMax = 40;
		
		/// <summary>
		/// Initial previous ground speed value [m/s]
		/// </summary>
        private const int gndSpeedPrevValue = 0;
		
		/// <summary>
		/// Initial previous ground speed value [m/s]
		/// </summary>
        private const int gndSpeedPrevPrevValue = 0;
		
		/// <summary>
		/// Maximum expected ground speed variation [m/(s·sample)]
		/// </summary>
        private const int gndSpeedMaxVar = 20;
		
		/// <summary>
		/// Minimum expected track angle [deg]
		/// </summary>
        private const int trackAngleMin = -180;
		
		/// <summary>
		/// Maximum expected track angle [deg]
		/// </summary>
        private const int trackAngleMax = 180;
		
		/// <summary>
		/// Initial previous track angle value [deg]
		/// </summary>
        private const int trackAnglePrevValue = 0;
		
		/// <summary>
		/// Initial previous track angle value [deg]
		/// </summary>
		private const int trackAnglePrevPrevValue = 0;
		
		/// <summary>
		/// Maximum expected track angle variation [deg/sample]
		/// </summary>
        private const int trackAngleMaxVar = 180;
		
		private const int magVarMin = -90;
		private const int magVarMax = 90;
		private const int magVarPrevValue = 0;
		private const int magVarMaxVar = 1;
		
		public GpsPosMessage ()
		: base()
		{
			this.latitude = new Field2(latMin, latMax, latPrevValue, latMaxVar);
            this.longitude = new Field2(lonMin, lonMax, lonPrevValue, lonMaxVar);
            this.gndSpeed = new Field2(gndSpeedMin, gndSpeedMax, gndSpeedPrevValue, gndSpeedMaxVar);
            this.trackAngle = new Field2(trackAngleMin, trackAngleMax,  trackAngleMaxVar);
			
			//this.magVar = new Field2(magVarMin, magVarMax, magVarPrevValue, magVarMaxVar);
		}
		
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
				char[] c = new char[b.Length-1];
				
				for(int i = 1; i < b.Length; i++)
				{
					c[i-1] = (char)b[i];
				}
				string m = new string(c);
				//Console.WriteLine("GPS POS: " + m);
				string[] words = m.Split('.'); //Partimos por puntos para no separar decimales
				
				double lat = double.Parse(words[3]);
				lat /= 100.0;
				double latTrunc = Math.Truncate(lat);
				lat = (lat - latTrunc)/60.0*100.0;
				lat += latTrunc;
				lat = words[4] == "N" ? lat : -lat;
	 			this.latitude.V = lat;
				
				double lon = double.Parse(words[5]);
				lon /= 100.0;
				double lonTrunc = Math.Truncate(lon);
				lon = (lon - lonTrunc)/60.0*100.0;
				lon += lonTrunc;
				lon = words[6] == "E" ? lon : -lon;
				this.longitude.V = lon;
				
				this.gndSpeed.V = double.Parse(words[7]) * 1852.0/3600.0; //kt a m/s
				
				double track = double.Parse(words[8]);
				if(track > 180.0)
				{
					track -= 360.0;
				}
				this.trackAngle.V = track;
				
				this.pos = new WgsPoint(this.latitude.V, this.longitude.V, 0);
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
			ans.latitude = Field2.DeepCopy(this.latitude);
			ans.longitude = Field2.DeepCopy(this.longitude);
			ans.gndSpeed = Field2.DeepCopy(this.gndSpeed);
			ans.trackAngle = Field2.DeepCopy(this.trackAngle);
			ans.pos = this.pos.DeepCopy();
			ans.latDev = this.latDev;
			ans.distDest = this.distDest;
			//ans.magVar = Field2.DeepCopy(this.magVar);
			return ans;
		}
	}
}

