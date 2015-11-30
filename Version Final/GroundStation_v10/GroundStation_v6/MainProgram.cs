using System;
using System.Collections.Generic;
using System.Text;
using GroundStation;
using System.IO;
using System.Threading;
using XPlane; 

namespace GroundStation
{
     public class MainProgram
     {
        

		/*private static readonly string path = "/dev/";
		private static readonly string pwmIn = "ttyUSB1";
		private static readonly string pwmOut = "ttyUSB2";
		/*private static readonly string telIn = "ttyUSB0";
		private static readonly int b19200 = 19200;
		private static readonly int b57600 = 57600;*/
		private static ulong time = 0;
        //private static int syncpack = 1; //Increases 1 for every packet
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
            //Set ref. pressure
            bool correct = false;
            double refpress1 = -1;
            int refpress = -1;

            while (!correct)
            {
                Console.WriteLine("Introduce reference pressure in mmHg (example: 29,92)");
                Console.WriteLine("- For default pressure");
                string linep = Console.ReadLine();
                if(linep=="-")
                {
                    linep = "29,92";
                }
                try
                {
                    refpress1 = Convert.ToDouble(linep);
                    if (refpress1 < 0)
                    {
                        Console.WriteLine("Invalid value, pressure must be positive");
                    }
                    if(refpress1>50)
                    {
                        Console.WriteLine("Invalid value, pressure too high");
                    }
                    else
                    {
                        refpress = (int)(refpress1 / 0.000295301);
                        correct = true;
                    }
                }
                catch (FormatException e)
                {
                    Console.WriteLine("Input string is not a sequence of digits.");
                }
            }

            //Set A/C Model
            bool acorrect = false;
            int model = 0; //C172=1  RC=2   Cirrus=3
            while (!acorrect)
            {
                Console.WriteLine("Introduce Aircraft model (Q, C172, RC or Cirrus)"); //Models needed in AircraftPerformance and PIDManager
                string lineac = Console.ReadLine();
                if ((lineac != "C172") && (lineac != "RC") && (lineac != "Cirrus") && (lineac != "Q"))
                {
                    Console.WriteLine("Wrong model");
                }
                else
                {
                    acorrect = true;
                    if(lineac=="C172") //Use 1-19 for planes and >20 for Heli/Quadcopters 
                    {
                        model = 1;
                    }
                    if (lineac == "RC")
                    {
                        model = 2;
                    }
                    if (lineac == "Cirrus")
                    {
                        model = 3;
                    }
                    if (lineac == "Q")
                    {
                        model = 20;
                    }
                }
                
            }
            AircraftPerformance ACp = AircraftPerformance.GetInstance(model);

            //DateTime begin1 = DateTime.UtcNow;
            GlobalArea ga = GlobalArea.GetInstance();
            Database db = Database.GetInstance();
			PIDManager pid = PIDManager.GetInstance(model);
            //pid.SetModel(model);
            NavManager nav = NavManager.GetInstance(); //NavManager for planes
            XNavManager xnav = XNavManager.GetInstance(); //NavManager for Heli/Quadcopters
			AdcMessage adc = new AdcMessage();
            adc.ChangeP0(refpress);
			ImuEulerMessage imu = new ImuEulerMessage();
			PwmMessage pwm = new PwmMessage();
			GpsPosMessage pos = new GpsPosMessage();
            if (model >= 20) //Use NavManager for Helicopters
            {
                xnav.Initialize();
            }
            else
            {
                nav.Initialize();
            }
			


            
            /*Input p;
			//p = new SerialInput(path + telIn, b19200);
			p = new LogInput();*/

            XplanePacketsId.Load(XplaneVersion.Xplane10);
            
            XplaneConnection connection = new XplaneConnection(); 
            //XplaneConnection connection = new XplaneConnection("10.211.55.2");

            XplaneParser parser = new XplaneParser(connection);

            connection.OpenConnections();
            parser.Start();


            PipeStream p = new PipeStream();

            Writer w = new Writer();
            w.output = p;
            w.Start(parser);


            StreamWriter ReferenceW;
            Path path = Path.GetInstance();
            ReferenceW = new StreamWriter(path.GetPath() + @"/ref.txt", true);

            double SpanX = pid.GetSpanValue(PIDManager.Ctrl.ROLL);
            double SpanY = pid.GetSpanValue(PIDManager.Ctrl.PITCH);
            double SpanT = pid.GetSpanValue(PIDManager.Ctrl.THROTTLE);

            double MeanX = pid.GetParam(PIDManager.Ctrl.ROLL, PID.Param.MEAN_VAL);
            double MeanY = pid.GetParam(PIDManager.Ctrl.PITCH, PID.Param.MEAN_VAL);

            int i = 0;
            while (true)
            {
                //DateTime begin1 = DateTime.UtcNow;

                p.CheckStartOfMessage();  //Busca el inicio del mensaje
				//Console.WriteLine("Message Received");
                byte[] header = p.ReadNBytes(1);
                byte[] m;
                switch (header[0])
                {
                    case (byte)0: //IMU-Euler Angles
                        byte[] timebytes = p.ReadNBytes(5);
                        //float recetime = BitConverter.ToInt32(timebytes, 1);
                        //Console.WriteLine(recetime);
                        m = p.ReadNBytes(12);  //13 bytes son de la IMU y tienen: time/roll/pitch/yaw
                        float A = BitConverter.ToInt32(timebytes, 1)/10;
                        //int B = syncpack * 100000;
                        float C = A /*- B*/;
                        float resul = C;
                        //Console.WriteLine("res:{0}", resul);
                        if ((time + Convert.ToUInt64(C))<Int64.MaxValue)
                        {
                            time += Convert.ToUInt64(C);
                        }
                        //time += Convert.ToUInt64(C);
                        //if (C > Int64.MaxValue)
                        /*{
                            
                            time += Convert.ToUInt64(C);
                            //syncpack++;
                        }*/
                        imu.CreateMessage(time, m);
                        ga.Imu = imu;
                        db.Add(ga.Imu);
						pid.SetChPitch(ga.Imu);
						pid.SetChRoll(ga.Imu);
						pid.SetChYaw(ga.Imu);
						if(ga.IsReady())
						{
                            if (model >= 20) //Use NavManager for Helicopters
                            {
                                xnav.UpdateAltRef();
                                xnav.UpdateHeadRef();
                                xnav.UpdateSpeedRef();
                            }
                            else
                            {
                                nav.UpdateAltRef();
                                nav.UpdateHeadRef();
                            }
						}
                        break;
                    case (byte)3: //Adc
                        m = p.ReadNBytes(13);  //(13)bytes son del ADC: time/barometro/termometro/pitot (ocupan 2 bytes todos menos time(1)) y calcula TAS y Altitud (ocupan 2 bytes)
                        //time += m[0];
                        adc.CreateMessage(time, m);
                        ga.Adc = adc;
                        db.Add(ga.Adc);
						pid.SetChThrottle(adc, model);
						if(ga.IsReady())
						{
                            if (model >= 20) //Use NavManager for Helicopters
                            {
                                xnav.UpdateAltRef();
                                //xnav.UpdateHeadRef();
                                xnav.UpdateSpeedRef();
                            }
                            else
                            {
                                nav.UpdateAltRef();
                                //nav.UpdateHeadRef();
                            }
						}
                        break;
                    case (byte)4: //Pwm   //9 bytes: time/ch1/ch2/ch3/ch4 (valor de variables chX entre 1000 y 2000) ocupan 2 bytes
                        m = p.ReadNBytes(9);
                        //time += m[0];
                        pwm.CreateMessage(time, m);
                        ga.Pwm = pwm;
                        db.Add(ga.Pwm);
                        break;
				case (byte)8:  //GPS
						byte count = p.ReadNBytes(1)[0]; //Lee el Lengthmess(No incluye time)
						m = p.ReadNBytes(count+1); //Lee (Lengthmess+1) bytes
						//time += m[0];
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
                                if (model >= 20) //Use NavManager for Helicopters
                                {
                                    xnav.SetPosition(ga.Pos.pos);
                                }
                                else
                                {
                                    nav.SetPosition(ga.Pos.pos);
                                }
							}
						}
						break;
					
                }

                //Enviamos controles a XPlane
                byte[] pidctrl = new byte[4];
                pidctrl[0] = pid.GetCh(1);
                pidctrl[1] = pid.GetCh(2);
                pidctrl[2] = pid.GetCh(3);
                pidctrl[3] = pid.GetCh(4);

                
               
                /*float joyposX = ((float)(Convert.ToInt32(pidctrl[1]) * 8) - 1000) / 1000; 
                float joyposY = ((float)(Convert.ToInt32(pidctrl[2]) * 8) - 1000) / 1000;
                float Throttle = (float)((float)Convert.ToInt32(pidctrl[0]) * 8 / 2000);*/
                


                float joyposX = (float)(((Convert.ToInt32(pidctrl[1]) * SpanX) - MeanX) / MeanX);
                float joyposY = (float)(((Convert.ToInt32(pidctrl[2]) * SpanY) - MeanY) / MeanY);
                float Throttle = (float)((float)Convert.ToInt32(pidctrl[0]) * SpanT / 2000);


                if(model==20)
                {
                    float reSpan = 8;                 //Used to increase sensibility for the quadcopter
                    joyposX = (float)(((Convert.ToInt32(pidctrl[1]) * SpanX) - (MeanX)) / (MeanX * reSpan));
                    joyposY = (float)(((Convert.ToInt32(pidctrl[2]) * SpanY) - (MeanY)) / (MeanY * reSpan));
                    Throttle = (float)((float)Convert.ToInt32(pidctrl[0]) * SpanT / 2000);
                }

                joyposX = joyposX > 0.35 ? joyposX = 0.35f : joyposX;
                joyposX = joyposX < -0.35 ? joyposX = -0.35f : joyposX;
                joyposY = joyposY > 0.35 ? joyposY = 0.35f : joyposY;
                joyposY = joyposY < -0.35 ? joyposY = -0.35f : joyposY;
                //Console.WriteLine("JX:{0}", joyposX);
                //Console.WriteLine("JY:{0}", joyposY);
                
                //byte[] ctlmess = XplanePacketGenerator.JoystickPacket(-999, 0, 0, 0); //Centrar controles

                
                byte[] ctlmess = XplanePacketGenerator.JoystickPacket(Throttle, joyposX, (byte)0, joyposY);
                

                

                //byte[] ctlmess = XplanePacketGenerator.JoystickPacket(-999, joyposX, -999, joyposY);
                connection.SendPacket(ctlmess);
                


                /*Console.WriteLine("control message");
                Console.WriteLine(Convert.ToDouble(pidctrl[0]));
                Console.WriteLine(Convert.ToDouble(pidctrl[1]));
                Console.WriteLine(Convert.ToDouble(pidctrl[2]));
                Console.WriteLine(Convert.ToDouble(pidctrl[3]));*/

                double[] pidref = new double[4]; //throttel roll pitch yaw
                pidref[0] = pid.GetSel(1);
                pidref[1] = pid.GetSel(2);
                pidref[2] = pid.GetSel(3);
                pidref[3] = pid.GetSel(4);


                double[] Navref = new double[3];
                if(model<20)
                {
                    Navref[0] = nav.GetSel(0); //Altitude
                    Navref[1] = nav.GetSel(1); //Heading
                    ReferenceW.WriteLine(time + "\t" + pidref[0] + "\t" + pidref[1] + "\t" + pidref[2] + "\t" + pidref[3] + "\t" + Navref[0] + "\t" + Navref[1]);
                }
                else
                {
                    Navref[0] = xnav.GetSel(0); //Altitude
                    Navref[1] = xnav.GetSel(1); //Heading
                    Navref[2] = xnav.GetSel(2); //Speed 
                    ReferenceW.WriteLine(time + "\t" + Navref[2] + "\t" + pidref[1] + "\t" + pidref[2] + "\t" + pidref[3] + "\t" + Navref[0] + "\t" + Navref[1]);
                }
                

                

                if (i == 10)
                {
                    op.Flush(db);  //Cada 10 ciclos guarda los datos en texto
                    i = 0;
                }
                i++;
                //DateTime end1 = DateTime.UtcNow;
                //Console.WriteLine("Measured time: " + (end1 - begin1).TotalMilliseconds + " ms.");
            }
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
