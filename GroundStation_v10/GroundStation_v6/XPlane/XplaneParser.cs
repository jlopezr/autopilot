using System;
using System.Text;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace XPlane
{
	public class XplaneParser
	{
		private XplaneConnection connection;
		private delegate void SetData(Byte[] paket, int i);
		private SetData[] parsers;
		private Thread th;
		private bool exit;
        private float[] Position;
        private float[] Angles;
        private float AoA=-999;
        private float[] Atmosphere;
        private float Speeds = -999;
        private float ZuluTime = -999;
        private string latlet;
        private string lonlet;
        private byte[] IMU;
        private byte[] ADC;
        //private byte[] PWM;
        private bool firstTime = true;
        private byte[] elapsedTimeb;
        int PacketNum = 0;

        

        public PipeStream mPipeStream; // the shared stream
        private byte[] Pipebytes;
        //private bool firstpak = true;

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
            parsers[XplanePacketsId.Times] += this.UpdateTimes;
		}

		public void Start()
		{
			ThreadStart ts = new ThreadStart(ReceivePacket);
			th = new Thread(ts);
			exit = false;
			th.Start();
		}

        double elapsedtime;
		private void ReceivePacket() //Lo llama el Start
		{
			byte[] data = new byte[5120];
			int recv = 0;
            DateTime prevtime = DateTime.UtcNow;
			while ((recv = connection.ReceivePacket(ref data)) > 0 && !exit)
			{
				ProcessPacket(data);
                DateTime time2 = DateTime.UtcNow;

                if (firstTime)
                {
                    elapsedtime = 0;
                    firstTime=false;
                    prevtime = time2;
                }
                else
                {
                    elapsedtime = ((time2 - prevtime).TotalMilliseconds);
                    double timedebug = (time2 - prevtime).TotalMilliseconds;
                    //Console.WriteLine("time {0}", timedebug);
                    prevtime = time2;
                }
			}
		}


		private void ProcessPacket(byte[] data)
		{
			//La estructura es |__5 Bytes de cabecera__|__36 Bytes Datos__|__36 Bytes Datos__|__....
			//Los datos son | 1B x Tipo Paquete | 3B x 0 | 4B x Single Data | 4B x Single Data|...
            bool p = true; //pimera vez que entra en cada bucle
			for (int i = 5; i < data.Length; i = i + 36)
			{
                if (p)
                {
                    p = false;
                    //Cada vez que entra recopila todos los datos anteriores los trata (y los saca por la pipe...)
                    if (Angles != null && AoA != -999 && Speeds != -999 && Atmosphere != null && Position != null && ZuluTime != -999)//??????????????????????????????????????????????????????????????
                    {
                        PacketNum++;
                        double control = PacketNum * 10000;
                        elapsedTimeb = BitConverter.GetBytes(Convert.ToInt32((elapsedtime /*+ control*/) * 10));
                        //IMU
                        byte[] header = new byte[5];
                        header[0] = 1;
                        header[1] = 1;
                        header[2] = 1;
                        header[3] = 0;
                        header[4] = 1;//time
                        byte[] realtime = elapsedTimeb;
                        Angles[0] = (Angles[0] + 180) * 10000;
                        Angles[1] = (Angles[1] + 180) * 10000;
                        //Angles[2] = (Angles[2] + 180) * 10000; //beta
                        Angles[2] = Angles[3] * 10000; //hdg //No suma 180. El programa quiere heading entre +-180 y le resta 180

                        IMU = header.Concat(realtime).ToArray();
                        IMU = IMU.Concat(BitConverter.GetBytes(Convert.ToInt32(Angles[1]))).ToArray(); 
                        IMU = IMU.Concat(BitConverter.GetBytes(Convert.ToInt32(Angles[0]))).ToArray();
                        IMU = IMU.Concat(BitConverter.GetBytes(Convert.ToInt32(Angles[2]))).ToArray();

                        //ADC
                        header[0] = 1;
                        header[1] = 1;
                        header[2] = 1;
                        header[3] = 3;
                        header[4] = 1;//time
                        Atmosphere[0] = (float)(Atmosphere[0] / 0.000295301 * 1000);
                        Atmosphere[1] = (float)((Atmosphere[1] + 273) * 1000);
                        Atmosphere[2] = (float)(Atmosphere[2] * 47.88020833333 * 1000);
                        
                        /*ADC = header.Concat(BitConverter.GetBytes(Convert.ToInt16(Atmosphere[0]))).ToArray(); //Deben ocupar 2 bytes--> valores de -32768 a 32767
                        ADC = ADC.Concat(BitConverter.GetBytes(Convert.ToInt16(Atmosphere[1]))).ToArray();
                        ADC = ADC.Concat(BitConverter.GetBytes(Convert.ToInt16(Atmosphere[2]))).ToArray();*/

                        ADC = header.Concat(BitConverter.GetBytes(Convert.ToInt32(Atmosphere[0]))).ToArray();
                        ADC = ADC.Concat(BitConverter.GetBytes(Convert.ToInt32(Atmosphere[1]))).ToArray();
                        ADC = ADC.Concat(BitConverter.GetBytes(Convert.ToInt32(Atmosphere[2]))).ToArray();

                        //PWM
                        /*header[0] = 1;
                        header[1] = 1;
                        header[2] = 1;
                        header[3] = 4;
                        header[4] = 1;//time*/

                        //GPS
                        byte[] headergps = new byte[6];
                        headergps[0] = 1;
                        headergps[1] = 1;
                        headergps[2] = 1;
                        headergps[3] = 8;
                        headergps[5] = 1;//time
                        string type = "$GPRMC";
                        string valid = "A";
                        if (Position[0] < 0)
                        {
                            Position[0] = -Position[0];
                            latlet = "S";
                        }
                        else
                        {
                            latlet = "N";
                        }
                        if (Position[1] < 0)
                        {
                            Position[1] = -Position[1];
                            lonlet = "W";
                        }
                        else
                        {
                            lonlet = "E";
                        }

                        //Convertimos la posicion a string y lo juntamos todo separado por ","
                        float ZuluTimemin = (float)((ZuluTime - Math.Truncate(ZuluTime)) * 60);
                        float ZuluTimesec = (float)(Math.Truncate((ZuluTimemin - Math.Truncate(ZuluTimemin)) * 60));
                        ZuluTimemin = (float)Math.Truncate(ZuluTimemin);
                        ZuluTime = (float)((Math.Truncate(ZuluTime) * 10000) + (ZuluTimemin * 100) + ZuluTimesec);

                        float latrmc = (float)((Math.Truncate(Position[0]) * 100) + (Math.Truncate((Position[0] - Math.Truncate(Position[0])) * 60 * 100) / 100));
                        float lonrmc = (float)((Math.Truncate(Position[1]) * 100) + (Math.Truncate((Position[1] - Math.Truncate(Position[1])) * 60 * 100) / 100));
                        string Positionlat = Convert.ToString(latrmc);
                        string Positionlon = Convert.ToString(lonrmc);
                        string ZuluTstr = Convert.ToString(ZuluTime);
                        string SpeedsSTR = Convert.ToString(Speeds);
                        string trackstr = Convert.ToString(AoA);
                        //string[] GPSE = new string[] {type,ZuluTstr,valid,Positionlat,latlet,Positionlon,lonlet,SpeedsSTR,trackstr};
                        string [] GPSE = new string [9];
                        GPSE[0] = type;
                        GPSE[1] = ZuluTstr;
                        GPSE[2] = valid;
                        GPSE[3] = Positionlat;
                        GPSE[4] = latlet;
                        GPSE[5] = Positionlon;
                        GPSE[6] = lonlet;
                        GPSE[7] = SpeedsSTR;
                        GPSE[8] = trackstr; //faltan datos del mensaje GPRMC pero el programa no los necesita
                        string GPSdata = string.Join(".", GPSE);
                        

                        //Falta convertir a byte (pasando o no por char) contar los bytes, poner el length en headergps[4] y juntarlo todo
                        char[] GPSchar = GPSdata.ToCharArray();
                        byte[] GPSbytes = Encoding.Unicode.GetBytes(GPSchar);
                        int Length = GPSbytes.Length;
                        headergps[4] = (byte)Length;
                        


                        //PipeStream
                        Pipebytes = IMU.Concat(ADC).ToArray();
                        Pipebytes = Pipebytes.Concat(headergps).ToArray();
                        Pipebytes = Pipebytes.Concat(GPSbytes).ToArray();

                        
                    }
                }
                

                if (this.parsers[data[i]] != null)
                {
                    this.parsers[data[i]](data, i);
                }	
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
			//Console.WriteLine ("Accelerations:{0},{1},{2}", uavAccelerationX, uavAccelerationY, uavAccelerationZ);
		}

		private void UpdatePosition(byte[] data, int i)//
		{
            Position = new float[2];
			double Latitude = BitConverter.ToSingle(data, i + 4);
			double Longitude = BitConverter.ToSingle(data, i + 4 + 4);
			float Altitude = (float)(BitConverter.ToSingle (data, i + 4 + 4 + 4)); //in feet
            Position[0] = (float)Latitude;
            Position[1] = (float)Longitude;;
			//Console.WriteLine ("Position:{0},{1},{2}", Latitude, Longitude, Altitude);
		}

		public void UpdateActualThrottle(byte[] data, int i)
		{
			float actualThrottle = BitConverter.ToSingle(data, i + 4);
		}

		private void UpdateAngles(byte[] data, int i)//
		{
            Angles = new float[4];
			float Pitch = (float)(BitConverter.ToSingle(data, i + 4) );
			float Roll = (float)(BitConverter.ToSingle(data, i + 4 + 4));
			float Yaw = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4));
            Angles[0] = Pitch;
            Angles[1] = Roll;
            Angles[3] = Yaw;
			//Console.WriteLine ("Angles:{0},{1},{2}", Pitch, Roll, Yaw);
		}

        private void UpdateAnglesAoA(byte[] data, int i)//
        {
            float Hpath = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4));
            Angles[2] = (float)(BitConverter.ToSingle(data, i + 4 + 4)); //Heading
            AoA = Hpath;
            //Console.WriteLine("Track:{0}", Hpath);
        }

        private void UpdateSpeeds(byte[] data, int i)//
        {
            float GrndSpd = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4));
            Speeds = GrndSpd;
            //Console.WriteLine("Ground Speed:{0}", GrndSpd); //kt
        }

        private void UpdateAtmosphere(byte[] data, int i)//
        {
            Atmosphere = new float[3];
            float AMprs = (float)(BitConverter.ToSingle(data, i + 4)); //inHg
            float AMtemp = (float)(BitConverter.ToSingle(data, i + 4 + 4)); //ºC
            float Q = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4 + 4)); //psf
            Atmosphere[0] = AMprs;
            Atmosphere[1] = AMtemp;
            Atmosphere[2] = Q;
            //Console.WriteLine("Pressures and temperature:{0},{1},{2}", AMprs, AMtemp, Q);
        }

        private void UpdateTimes(byte[] data, int i)//
        {
            ZuluTime = (float)(BitConverter.ToSingle(data, i + 4 + 4 + 4 + 4 + 4 + 4 + 4));
            //Console.WriteLine("Local Time:{0}", ZuluTime);
        }

        public byte[] givemebytes(byte[] lastb)
        {
            bool tof = true;
            byte[] sendbytes = Pipebytes;
            while (tof)
            {
                sendbytes = Pipebytes;
                if (sendbytes != null)
                {
                    if (sendbytes.Length > 47)
                    {
                        if (lastb == sendbytes)
                        {
                            tof = true;
                        }
                        else
                        {
                            tof = false;
                        }
                    }
                }
            }


            return sendbytes;
        }
	}
}

