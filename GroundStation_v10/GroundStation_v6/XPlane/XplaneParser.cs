using System;
using System.Threading;

namespace XPlane
{
	public class XplaneParser
	{
		private XplaneConnection connection;
		private delegate void SetData(Byte[] paket, int i);
		private SetData[] parsers;
		private Thread th;
		private bool exit;

		public XplaneParser (XplaneConnection connection)
		{
			this.parsers = new SetData[256];
			this.connection = connection;

			parsers[XplanePacketsId.Accelerations] += this.UpdateAccelerations;
			parsers[XplanePacketsId.Position] += this.UpdatePosition; //Este debe enviar lat y lon
			parsers[XplanePacketsId.Angles] += this.UpdateAngles; //Yaw pitch y hdg
			parsers[XplanePacketsId.ActThrottle] += this.UpdateActualThrottle;
            parsers[XplanePacketsId.AnglesAoA] += this.UpdateAnglesAoA; //Hpath(deg)
            parsers[XplanePacketsId.Atmosphere] += this.UpdateAtmosphere; //Temp(ºC) Atmpress (inHg) Pitot(psf)
            parsers[XplanePacketsId.Speeds] += this.UpdateSpeeds; //GrndSpd(kt)
		}

		public void Start()
		{
			ThreadStart ts = new ThreadStart(ReceivePacket);
			th = new Thread(ts);
			exit = false;
			th.Start();
		}

		private void ReceivePacket()
		{
			byte[] data = new byte[5120];
			int recv = 0;

			while ((recv = connection.ReceivePacket(ref data)) > 0 && !exit)
			{
				ProcessPacket(data);
			}
		}

		private void ProcessPacket(byte[] data)
		{
			//La estructura es |__5 Bytes de cabecera__|__36 Bytes Datos__|__36 Bytes Datos__|__....
			//Los datos son | 1B x Tipo Paquete | 3B x 0 | 4B x Single Data | 4B x Single Data|...
			for (int i = 5; i < data.Length; i = i + 36)
			{
				if (this.parsers[data[i]] != null)
					this.parsers[data[i]](data, i);
			}
		}

		public void Stop()
		{
			exit = true;
			th.Join();
		}

		private void UpdateAccelerations(byte[] data, int i)
		{
			float uavAccelerationZ = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4) * Math.PI / 180.0);
			float uavAccelerationX = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4 + 4) * Math.PI / 180.0);
			float uavAccelerationY = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4 + 4 + 4) * Math.PI / 180.0);
			Console.WriteLine ("Accelerations:{0},{1},{2}", uavAccelerationX, uavAccelerationY, uavAccelerationZ);
		}

		private void UpdatePosition(byte[] data, int i)
		{
			double Latitude = BitConverter.ToSingle(data, i + 4) * Math.PI / 180.0;
			double Longitude = BitConverter.ToSingle(data, i + 4 + 4) * Math.PI / 180.0;
			float Altitude = (float)(BitConverter.ToSingle (data, i + 4 + 4 + 4)); //in feet
			Console.WriteLine ("Position:{0},{1},{2}", Latitude, Longitude, Altitude);
		}

		public void UpdateActualThrottle(byte[] data, int i)
		{
			float actualThrottle = BitConverter.ToSingle(data, i + 4);
		}

		private void UpdateAngles(byte[] data, int i)
		{
			float Pitch = (float)(BitConverter.ToSingle(data, i + 4) * Math.PI / 180.0);
			float Roll = (float)(BitConverter.ToSingle(data, i + 4 + 4) * Math.PI / 180.0);
			float Yaw = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4) * Math.PI / 180.0);
			Console.WriteLine ("Angles:{0},{1},{2}", Pitch, Roll, Yaw);
		}

        private void UpdateAnglesAoA(byte[] data, int i)
        {
            float Hpath = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4) * Math.PI / 180.0);
            Console.WriteLine("Track:{0}", Hpath);
        }

        private void UpdateSpeeds(byte[] data, int i)
        {
            float GrndSpd = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4));
            Console.WriteLine("Ground Speed:{0}", GrndSpd);
        }

        private void UpdateAtmosphere(byte[] data, int i)
        {
            float AMprs = (float)(BitConverter.ToSingle(data, i + 4));
            float AMtemp = (float)(BitConverter.ToSingle(data, i + 4 + 4));
            float Q = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4 + 4));
            Console.WriteLine("Pressures and temperature:{0},{1},{2}", AMprs, AMtemp, Q);
        }
	}
}

