using System;
using System.Collections.Generic;
using System.Text;
using GroundStation;
using System.IO;
using System.Threading;
using XPlane;

namespace GroundStation
{
     public class Main
     {
		private static readonly string path = "/dev/";
		private static readonly string pwmIn = "ttyUSB1";
		private static readonly string pwmOut = "ttyUSB2";
		/*private static readonly string telIn = "ttyUSB0";
		private static readonly int b19200 = 19200;*/
		private static readonly int b57600 = 57600;
		private static ulong time = 0;
        private static Output op;
        public static void Run() //Se ejecuta desde Exec/Main
        {
            op = new Output();
			ThreadStart thsTel = new ThreadStart (RunTelemetry);
			Thread tTel = new Thread(thsTel);
			tTel.Start();
			GC.KeepAlive(tTel);
			
			/*ThreadStart thsPwm = new ThreadStart(RunPwm);
			Thread tPwm = new Thread(thsPwm);
			tPwm.Start();
			GC.KeepAlive(tPwm);*/
	    }
		
		private static void RunTelemetry()
		{
            GlobalArea ga = GlobalArea.GetInstance();
            Database db = Database.GetInstance();
			PIDManager pid = PIDManager.GetInstance();
			NavManager nav = NavManager.GetInstance();
			AdcMessage adc = new AdcMessage();
			ImuEulerMessage imu = new ImuEulerMessage();
			PwmMessage pwm = new PwmMessage();
			GpsMessage gps = new GpsMessage();
			GpsDopMessage dop = new GpsDopMessage();
			GpsPosMessage pos = new GpsPosMessage();
			nav.Initialize();
            
            /*Input p;
			//p = new SerialInput(path + telIn, b19200);
			p = new LogInput();*/

            XplanePacketsId.Load(XplaneVersion.Xplane10);
            XplaneConnection connection = new XplaneConnection();
            XplaneParser parser = new XplaneParser(connection);

            connection.OpenConnections();
            parser.Start();
            PipeStream p = new PipeStream();

            Writer w = new Writer();
            w.output = p;
            w.Start(parser);

            int i = 0;
            while (true)
            {
                p.CheckStartOfMessage();  //Busca el inicio del mensaje
				//Console.WriteLine("Message Received");
                byte[] header = p.ReadNBytes(1);
                byte[] m;
                switch (header[0])
                {
                    case (byte)0: //IMU-Euler Angles
                        m = p.ReadNBytes(13);  //13 bytes son de la IMU y tienen: time/roll/pitch/yaw/accX/accY/accZ
                        time += m[0];
                        imu.CreateMessage(time, m);
                        ga.Imu = imu;
                        db.Add(ga.Imu);
						pid.SetChPitch(ga.Imu);
						pid.SetChRoll(ga.Imu);
						pid.SetChYaw(ga.Imu);
						if(ga.IsReady())
						{
							nav.UpdateAltRef();
							nav.UpdateHeadRef();
						}
                        break;
                    case (byte)3: //Adc
                        m = p.ReadNBytes(13);  //7 (13)bytes son del ADC: time/barometro/termometro/pitot (ocupan 2 bytes todos menos time(1)) y calcula TAS y Altitud (ocupan 2 bytes)
                        time += m[0];
                        adc.CreateMessage(time, m);
                        ga.Adc = adc;
                        db.Add(ga.Adc);
						pid.SetChThrottle(adc);
						if(ga.IsReady())
						{
							nav.UpdateAltRef();
							//nav.UpdateHeadRef();
						}
                        break;
                    case (byte)4: //Pwm   //9 bytes: time/ch1/ch2/ch3/ch4 (valor de variables chX entre 1000 y 2000) ocupan 2 bytes
                        m = p.ReadNBytes(9);
                        time += m[0];
                        pwm.CreateMessage(time, m);
                        ga.Pwm = pwm;
                        db.Add(ga.Pwm);
                        break;
                    case (byte)5: //Gps (deprecated)  //13 bytes del GPS: time/lat/lon/gndspeed/trackangle (1/4/4/2/2) bytes  NO LO USA??
                        m = p.ReadNBytes(13);
                        time += m[0];
                        gps.CreateMessage(time, m);
                        ga.Gps = gps;
                        db.Add(ga.Gps);
						if(ga.IsReady())
							nav.SetPosition(ga.Gps.pos);
                        break;
                    case (byte)6: //GpsDop  NO LO USA??
						m = p.ReadNBytes(p.ReadNBytes(1)[0]-4);
						time += m[0];
                        dop.CreateMessage(time, m);
                        ga.Dop = dop;
                        db.Add(ga.Dop);
                        break;
                    case (byte)7: //GpsPos  NO LO USA??
                        m = p.ReadNBytes(p.ReadNBytes(1)[0]-4);
                        time += m[0];
                        pos.CreateMessage(time, m);
                        
						if(ga.IsReady())
						{
							nav.SetPosition(pos.pos);
							pos.distDest = nav.latNav.distDest;
							pos.latDev = nav.latNav.latDev;
						}
						else
						{
							pos.distDest = -100;
							pos.latDev = -100;
						}
						ga.Pos = pos;
                        db.Add(ga.Pos);
                        break;
				case (byte)8:  //GPS  ESTE ES EL QUE USA
						byte count = p.ReadNBytes(1)[0]; //Lee el Lenghtmess(No incluye time)
						m = p.ReadNBytes(count+1); //Lee (Lenghtmess+1) bytes
						time += m[0];
                        byte[] c = new byte[12];
						
						for(int j = 1; j < 13; j++)
						{
							c[j-1] = m[j];
						}
						char[] cr1 = Encoding.Unicode.GetChars(c);
						string h = new string(cr1);
						
						if(h == "$GPRMC")
						{
							pos.CreateMessage(time, m);
							ga.Pos = pos;
                        	db.Add(ga.Pos);
							if(ga.IsReady())
							{
								nav.SetPosition(ga.Pos.pos);
							}
						}
						else if(h == "$GPGSA") //No funciona?
						{
	                        dop.CreateMessage(time, m);
	                        ga.Dop = dop;
	                        db.Add(ga.Dop);
						}
						break;
					
                }

                //Enviamos controles a XPlane
                byte[] pidctrl = new byte[4];
                pidctrl[0] = pid.GetCh(1);
                pidctrl[1] = pid.GetCh(2);
                pidctrl[2] = pid.GetCh(3);
                pidctrl[3] = pid.GetCh(4);
                float joyposX = ((float)(pidctrl[2] * 8) - 1000) / 1000;
                float joyposY = ((float)(pidctrl[1] * 8) - 1000) / 1000;
                Console.WriteLine("JX:{0}", joyposX);
                Console.WriteLine("JY:{0}", joyposY);
                byte[] ctlmess = XplanePacketGenerator.JoystickPacket(-999, joyposX, 0, joyposY);
                connection.SendPacket(ctlmess);

                /*Console.WriteLine("control message");
                Console.WriteLine(Convert.ToDouble(pidctrl[0]));
                Console.WriteLine(Convert.ToDouble(pidctrl[1]));
                Console.WriteLine(Convert.ToDouble(pidctrl[2]));
                Console.WriteLine(Convert.ToDouble(pidctrl[3]));//*/

                if (i == 10)
                {
                    op.Flush(db);  //Cada 10 ciclos guarda los datos en texto
                    i = 0;
                }
                i++;
            }
        } 
    	
		private static void RunPwm()
		{
			StreamWriter swCal = new StreamWriter("Logs/Calib");
			SerialInput si = new SerialInput(path + pwmIn, b57600);
			SerialInput so = new SerialInput(path + pwmOut, b57600);
			Thread.Sleep(1000);
			si.WriteNBytes(new byte[]{1,1,1});
			si.CheckStartOfMessage();
			so.WriteNBytes(new byte[]{1});
			
			byte[] b = new byte[4];
			byte[] ans = new byte[4];
			
			PIDManager pid = PIDManager.GetInstance();
			NavManager nav = NavManager.GetInstance();
			GlobalArea ga = GlobalArea.GetInstance();
			
			while(true)
			{
				switch(nav.GetCurrentMode())
				{
                    case NavManager.Mode.MANUAL:  //bypass, sale lo que entra multiplicando por el spanfactor y sumando el offset  throttle/roll/pitch/yaw  (1 byte cada uno)
					b = si.ReadNBytes(4);
					so.WriteNBytes(b);
					Console.WriteLine("entrada: " + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					int[] aux = new int[4];
					aux[0] = (int)Math.Round(b[0] * pid.GetSpanValue(PIDManager.Ctrl.THROTTLE) + pid.GetOffsetValue(PIDManager.Ctrl.THROTTLE));
					aux[1] = (int)Math.Round(b[1] * pid.GetSpanValue(PIDManager.Ctrl.ROLL) + pid.GetOffsetValue(PIDManager.Ctrl.ROLL));
					aux[2] = (int)Math.Round(b[2] * pid.GetSpanValue(PIDManager.Ctrl.PITCH) + pid.GetOffsetValue(PIDManager.Ctrl.PITCH));
					aux[3] = (int)Math.Round(b[3] * pid.GetSpanValue(PIDManager.Ctrl.YAW) + pid.GetOffsetValue(PIDManager.Ctrl.YAW));
						
					Console.WriteLine("entrada: " +  aux[0] + " " + aux[1] + " " + aux[2] + " " + aux[3]); 
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					op.WritePwm(time, b);
					op.WritePwm(time, b);
					break;
				case NavManager.Mode.CALIBRATION_THROTTLE:
					swCal.WriteLine(time + " " + "throt");
					b = si.ReadNBytes(4);
					Console.WriteLine("entrada:" + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					ans[0] = ThrottleCalib(pid, ga.Adc);
					ans[1] = b[1];
					ans[2] = b[2];
					ans[3] = b[3];
					so.WriteNBytes(ans);
					Console.WriteLine("sortida:" + ans[0] + " " +  ans[1] + " " + ans[2] + " " + ans[3]);
					op.WritePwm(time, b);
					op.WritePwm(time, ans);
					break;
				case NavManager.Mode.CALIBRATION_ROLL:
					swCal.WriteLine(time + " " + "roll");
					b = si.ReadNBytes(4);
					Console.WriteLine("entrada:" + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					ans[0] = b[0];
					ans[1] = RollCalib(pid, ga.Imu);
					ans[2] = b[2];
					ans[3] = b[3];
					so.WriteNBytes(ans);
					Console.WriteLine("sortida:" + ans[0] + " " +  ans[1] + " " + ans[2] + " " + ans[3]);
					op.WritePwm(time, b);
					op.WritePwm(time, ans);
					break;
				case NavManager.Mode.CALIBRATION_PITCH:
					swCal.WriteLine(time + " " + "pitch");
					b = si.ReadNBytes(4);
					Console.WriteLine("entrada:" + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					ans[0] = b[0];
					ans[1] = b[1];
					ans[2] = PitchCalib(pid, ga.Imu);
					ans[3] = b[3];
					so.WriteNBytes(ans);
					Console.WriteLine("sortida:" + ans[0] + " " +  ans[1] + " " + ans[2] + " " + ans[3]);
					op.WritePwm(time, b);
					op.WritePwm(time, ans);
					break;
                case NavManager.Mode.CALIBRATION_YAW:  //las 4 calibraciones mantienen el parametro a calibrar con un margen de 8 a 12 m/s (Throttle-speed) o +-2 deg (roll/pitch/yaw) con cambios en los controles de +-20%
					swCal.WriteLine(time + " " + "yaw");
					b = si.ReadNBytes(4);
					Console.WriteLine("entrada:" + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					ans[0] = b[0];
					ans[1] = b[1];
					ans[2] = b[2];
					ans[3] = YawCalib(pid, ga.Imu);
					Console.WriteLine("sortida:" + ans[0] + " " +  ans[1] + " " + ans[2] + " " + ans[3]);
					op.WritePwm(time, b);
					op.WritePwm(time, ans);
					so.WriteNBytes(ans);
					break;
                default:  //DIRECTED Y AUTONOMOUS
					b = si.ReadNBytes(4);
					//Console.WriteLine("entrada:" + b[0] + " " + b[1] + " " + b[2] + " " + b[3]);
					pid.SetCh(1, b[0]);
					pid.SetCh(2, b[1]);
					pid.SetCh(3, b[2]);
					pid.SetCh(4, b[3]);
					//Console.WriteLine("sortida:" + ans[0] + " " +  ans[1] + " " + ans[2] + " " + ans[3]);
					//Console.WriteLine();
					ans[0] = pid.GetCh(1);
					ans[1] = pid.GetCh(2);
					ans[2] = pid.GetCh(3);
					ans[3] = pid.GetCh(4);
					so.WriteNBytes(ans);
					op.WritePwm(time, b);
					op.WritePwm(time, ans);
					break;
				}
			}
		}
		
		private static bool stateThrottle = true;
		private static int calValueThrottle = 180; //20% span
		private static byte ThrottleCalib(PIDManager pid, AdcMessage m)
		{
			byte ans;
			if(m.tas > 12)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.THROTTLE);
				double span = pid.GetSpanValue(PIDManager.Ctrl.THROTTLE);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.THROTTLE);
				ans = (byte)(((meanVal - calValueThrottle)-offset)/span);
				stateThrottle = false;
			}
			else if(m.tas < 8)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.THROTTLE);
				double span = pid.GetSpanValue(PIDManager.Ctrl.THROTTLE);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.THROTTLE);
				ans = (byte)(((meanVal + calValueThrottle)-offset)/span);
				stateThrottle = true;
			}
			else if(!stateThrottle)  //entra aqui despues de pasar por los >12¿¿
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.THROTTLE);
				double span = pid.GetSpanValue(PIDManager.Ctrl.THROTTLE);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.THROTTLE);
				ans = (byte)(((meanVal - calValueThrottle)-offset)/span);
			}
            else  //lo mismo que el de <8 por si 8<tas<12??
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.THROTTLE);
				double span = pid.GetSpanValue(PIDManager.Ctrl.THROTTLE);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.THROTTLE);
				ans = (byte)(((meanVal + calValueThrottle)-offset)/span);
			}
			return ans;
		}
		
		private static bool stateRoll = true;
		private static int calValueRoll = 200; //20% span
		private static byte RollCalib(PIDManager pid, ImuEulerMessage m)
		{
			byte ans;
			if(m.roll.V > 2)
			{	
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.ROLL);
				double span = pid.GetSpanValue(PIDManager.Ctrl.ROLL);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.ROLL);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal - calValueRoll)-offset)/span);
				stateRoll = false;
			}
			else if(m.roll.V < -2)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.ROLL);
				double span = pid.GetSpanValue(PIDManager.Ctrl.ROLL);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.ROLL);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal + calValueRoll)-offset)/span);
				stateRoll = true;
			}
			else if(!stateRoll)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.ROLL);
				double span = pid.GetSpanValue(PIDManager.Ctrl.ROLL);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.ROLL);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal - calValueRoll)-offset)/span);
			}
			else
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.ROLL);
				double span = pid.GetSpanValue(PIDManager.Ctrl.ROLL);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.ROLL);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal + calValueRoll)-offset)/span);
			}
			return ans;
		}
		
		private static int calValuePitch = 200; //20% span
		private static bool statePitch = true;
		private static byte PitchCalib(PIDManager pid, ImuEulerMessage m)
		{
			byte ans;
			if(m.pitch.V > 2)
			{
				
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.PITCH);
				double span = pid.GetSpanValue(PIDManager.Ctrl.PITCH);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.PITCH);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal + calValuePitch)-offset)/span);
				statePitch = false;
			}
			else if(m.pitch.V < -2)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.PITCH);
				double span = pid.GetSpanValue(PIDManager.Ctrl.PITCH);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.PITCH);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal - calValuePitch)-offset)/span);
				statePitch = true;
			}
			else if(!statePitch)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.PITCH);
				double span = pid.GetSpanValue(PIDManager.Ctrl.PITCH);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.PITCH);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal + calValuePitch)-offset)/span);
			}
			else
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.PITCH);
				double span = pid.GetSpanValue(PIDManager.Ctrl.PITCH);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.PITCH);
				Console.WriteLine("MeanValue: " + meanVal);
				Console.WriteLine("Span: " + span);
				Console.WriteLine("Offset: " + offset);
				ans = (byte)(((meanVal - calValuePitch)-offset)/span);
			}
			return ans;
		}
		
		private static int calValueYaw = 210; //20% span
		private static bool stateYaw = true;
		private static byte YawCalib(PIDManager pid, ImuEulerMessage m)
		{
			byte ans;
			Console.WriteLine("INITIAL VAL: " + pid.GetParam(PIDManager.Ctrl.YAW, PID.Param.INITIAL_VAL));
			if((m.yaw.V - pid.GetParam(PIDManager.Ctrl.YAW, PID.Param.INITIAL_VAL)) > 2)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.YAW);
				double span = pid.GetSpanValue(PIDManager.Ctrl.YAW);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.YAW);
				ans = (byte)(((meanVal - calValueYaw)-offset)/span);
				stateYaw = true;
			}
			else if(m.yaw.V - pid.GetParam(PIDManager.Ctrl.YAW, PID.Param.INITIAL_VAL) < -2)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.YAW);
				double span = pid.GetSpanValue(PIDManager.Ctrl.YAW);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.YAW);
				ans = (byte)(((meanVal + calValueYaw)-offset)/span);
				stateYaw = false;
			}
			else if(!stateYaw)
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.YAW);
				double span = pid.GetSpanValue(PIDManager.Ctrl.YAW);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.YAW);
				ans = (byte)(((meanVal - calValueYaw)-offset)/span);
			}
			else
			{
				int meanVal = pid.GetMeanValue(PIDManager.Ctrl.YAW);
				double span = pid.GetSpanValue(PIDManager.Ctrl.YAW);
				int offset = pid.GetOffsetValue(PIDManager.Ctrl.YAW);
				ans = (byte)(((meanVal - calValueYaw)-offset)/span);
			}
			return ans;
		}

        class Writer
        {
            public Stream output;
            public byte[] Pipeb;
            XplaneParser parserb;
            private byte[] prevb;

            public void Start(object parserb1)
            {
                parserb = (XplaneParser)parserb1;
                Thread th = new Thread(new ThreadStart(Run));
                th.Start();
            }


            public void Run()
            {
                //byte b = 0;
                while (true)
                {
                    Pipeb = parserb.givemebytes(prevb);
                    prevb = Pipeb;
                    byte[] bytes2 = (byte[])Pipeb;
                    if (bytes2 != null)
                    {
                        output.Write(bytes2, 0, bytes2.Length);
                    }
                }

            }
        }
	}
}
