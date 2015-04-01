using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;

namespace GroundStation
{
	public class NavManager
	{
		private static NavManager instance = null;
		
		public static NavManager GetInstance()
		{
			if(instance == null)
				instance = new NavManager();
			return instance;
		}
		
		public enum Mode
		{
			MANUAL,
			DIRECTED,
			AUTONOMOUS,
			CALIBRATION_THROTTLE,
			CALIBRATION_ROLL,
			CALIBRATION_PITCH,
			CALIBRATION_YAW
		};
		
		private Mode currMode;
		public UpperLayer alt;
		public UpperLayer head;
		public UpperLayer speed;
		public LatNav latNav;
		public ConsoleInput ci;
		private PIDManager pid;
		
		private bool isFirst;
		
		public void Initialize ()
		{
			this.pid = PIDManager.GetInstance();
			this.ReadConfig();
			if(this.isFirst == true)
			{
				this.isFirst = false;
				this.ci = new ConsoleInput(this);
				ThreadStart thsCi = new ThreadStart(this.ci.Run);
				Thread th = new Thread(thsCi);
				th.Start();
			}
		}
		
		private NavManager()
		{
			this.isFirst = true;
		}
		
		public void Switch(Mode m)
		{
			this.currMode = m;
			switch(this.currMode)
			{
			case Mode.AUTONOMOUS:
				this.alt.activate();
				this.head.activate();
				this.speed.activate();
				this.latNav.activate();
				break;
			case Mode.DIRECTED:
			case Mode.MANUAL:
			case Mode.CALIBRATION_THROTTLE:
			case Mode.CALIBRATION_ROLL:
			case Mode.CALIBRATION_PITCH:
			case Mode.CALIBRATION_YAW:
				this.alt.deactivate();
				this.head.deactivate();
				this.speed.deactivate();
				break;
			}
		}
		
		public Mode GetCurrentMode()
		{
			return this.currMode;
		}
			
		public void SetAltitude(double altRef)
		{
			if(this.alt != null && this.currMode == Mode.AUTONOMOUS)
				this.alt.SetParam(altRef);
		}
		
		public void SetHeading(double headRef )
		{
			if(this.head != null && this.currMode == Mode.AUTONOMOUS)
				this.head.SetParam(headRef);
		}
		
		public void SetSpeed(double speedRef)
		{
			if(this.speed != null && this.currMode == Mode.AUTONOMOUS)
				this.speed.SetParam(speedRef);
		}
		
		public void SetPosition(WgsPoint pos)
		{
			if(this.latNav != null && this.currMode == Mode.AUTONOMOUS)
				this.latNav.CurrPos = pos;
		}
		
		public void UpdateAltRef()
		{
			if(this.alt != null && this.currMode == Mode.AUTONOMOUS)
			 this.alt.GetRef();
		}
		
		public void UpdateHeadRef()
		{
			if(this.alt != null && this.currMode == Mode.AUTONOMOUS)
				this.head.GetRef();
		}
		
		public void UpdateSpeedRef()
		{
			if(this.alt != null && this.currMode == Mode.AUTONOMOUS)
				this.speed.GetRef();
		}
		
		//TODO: No em queda clar com fer el throttle...
		public void UpdateThrottleRef(double t)
		{
			if(this.currMode == Mode.DIRECTED)
				this.pid.SetRef(PIDManager.Ctrl.THROTTLE, t);	
		}
		
		public void UpdateRollRef(double r)
		{
			if(this.currMode == Mode.DIRECTED)
				this.pid.SetRef(PIDManager.Ctrl.ROLL, r);
		}
		
		public void UpdatePitchRef(double p)
		{
			if(this.currMode == Mode.DIRECTED)
				this.pid.SetRef(PIDManager.Ctrl.PITCH, p);
		}
		
		public void UpdateYawRef(double y)
		{
			if(this.currMode == Mode.DIRECTED)
				this.pid.SetRef(PIDManager.Ctrl.YAW, y);
		}
		
		private void ReadConfig()
		{
			StreamReader sr = new StreamReader("NavConfig.txt");
			double gpsTs = 0, adcTs = 0, kp = -1, ki = -1, kd = -1, initialRef;
			WgsPoint orig = new WgsPoint();
			string line;
			while((line = sr.ReadLine()) != null)
			{
				if(line.StartsWith("#") || line == "")
					continue;
				string[] words = line.Split(new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				switch(words[0])
				{
				case "gpsTs":
					gpsTs = Convert.ToDouble(words[1]);
					break;
				case "adcTs":
                    adcTs = Convert.ToDouble(words[1]);
					break;
				case "altitude":
                    kp = Convert.ToDouble(words[1]);
                    ki = Convert.ToDouble(words[2]);
                    kd = Convert.ToDouble(words[3]);
                    initialRef = Convert.ToDouble(words[4]);
                    double maxPitch = Convert.ToDouble(words[5]);
                    double minPitch = Convert.ToDouble(words[6]);
					this.alt = new Altitude(initialRef, adcTs, kp, ki, kd, maxPitch, minPitch);
					break;
				case "heading":
                    kp = Convert.ToDouble(words[1]);
                    ki = Convert.ToDouble(words[2]);
                    kd = Convert.ToDouble(words[3]);
                    initialRef = Convert.ToDouble(words[4]);
                    double maxRoll = Convert.ToDouble(words[5]);
					this.head = new Heading(initialRef, adcTs, kp, ki, kd, maxRoll);
					break;
				case "latNav":
                    kp = Convert.ToDouble(words[1]);
                    ki = Convert.ToDouble(words[2]);
                    kd = Convert.ToDouble(words[3]);
					double lat = Convert.ToDouble(words[4]);
                    double lon = Convert.ToDouble(words[5]);
					orig = new WgsPoint(lat, lon, 0);
					break;
				case "FP":
					int n = int.Parse(words[1]);
					Queue<WgsPoint> fp = new Queue<WgsPoint>();
					for(int i = 0; i < n; i++)
					{
						line = sr.ReadLine();
						words = line.Split(' ');
                        double la = Convert.ToDouble(words[0]);
                        double lo = Convert.ToDouble(words[1]);
						WgsPoint aux = new WgsPoint(la, lo, 0);
						fp.Enqueue(aux);
					}
					
					this.latNav = new LatNav(orig, fp, gpsTs, kp, ki, kd);
					break;
				default:
					break;
				}
			}
		}
	}
}

