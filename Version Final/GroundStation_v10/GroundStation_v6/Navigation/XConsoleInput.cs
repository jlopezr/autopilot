using System;

namespace GroundStation
{
	public class XConsoleInput
	{
		private XNavManager nav;
		private GlobalArea ga;
		private PIDManager pid;
				
		public XConsoleInput(XNavManager nav)
		{
			this.nav = nav;
			this.ga = GlobalArea.GetInstance();
			this.pid = PIDManager.GetInstance();
		}
		
		public void Run()
		{
			while(true)
			{
				
				string line = Console.ReadLine();
				if(line == string.Empty)
					continue;
				string[] words = line.Split(new char[]{' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
				if(words.Length > 2)
				{
					Console.WriteLine("Wrong format");
					Console.WriteLine();
					continue;
				}
				double val;
				this.ExecuteQuery(words, out val);
			}
		}
		public bool ExecuteQuery(string[] query, out double val)
		{
			if(query.Length == 1)
				Console.WriteLine("Received Query:" + query[0]);
			else if(query.Length > 1)
				Console.WriteLine("Received Query:" + query[0] + " " + query[1]);
			
			bool ok = false;
			val = double.NaN;
			switch(query[0])
			{
                case "help":
                    Console.WriteLine("Set parameter and new value separated by a space");
                Console.WriteLine("tm - switch to attitude mode");
				Console.WriteLine("dm - switch to directed mode");
				Console.WriteLine("am - switch to autonomous mode");
				Console.WriteLine("mm - switch to manual mode");
				Console.WriteLine("ctm - switch to calib throttle mode");
				Console.WriteLine("crm - switch to calib roll mode");
				Console.WriteLine("cpm - switch to calib pitch mode");
				Console.WriteLine("cym - switch to calib yaw mode");
				Console.WriteLine("nh - new heading");
				Console.WriteLine("nv - new speed");
				Console.WriteLine("na - new altitude");
				Console.WriteLine("nr - new roll");
				Console.WriteLine("np - new pitch");
				Console.WriteLine("ny - new yaw");
				Console.WriteLine("ih - current heading");
				Console.WriteLine("iv - current speed");
				Console.WriteLine("ia - current altitude");
				Console.WriteLine("ir - current roll");
				Console.WriteLine("ip - current pitch");
				Console.WriteLine("tts - refresh throttle sampling time");
				Console.WriteLine("tiv - refresh throttle pid initial value");
				Console.WriteLine("tkp - refresh throttle pid proportional term");
				Console.WriteLine("tki - refresh throttle pid integral term");
				Console.WriteLine("tkd - refresh throttle pid derivative term");
				Console.WriteLine("rts - refresh roll sampling time");
				Console.WriteLine("riv - refresh roll pid initial value");
				Console.WriteLine("rkp - refresh roll pid proportional term");
				Console.WriteLine("rki - refresh roll pid integral term");
				Console.WriteLine("rkd - refresh roll pid derivative term");
				Console.WriteLine("pts - refresh pitch sampling time");
				Console.WriteLine("piv - refresh pitch pid initial value");
				Console.WriteLine("pkp - refresh pitch pid proportional term");
				Console.WriteLine("pki - refresh pitch pid integral term");
				Console.WriteLine("pkd - refresh pitch pid derivative term");
				Console.WriteLine("yts - refresh yaw sampling time");
				Console.WriteLine("yiv - refresh yaw pid initial value");
				Console.WriteLine("ykp - refresh yaw pid proportional term");
				Console.WriteLine("yki - refresh yaw pid integral term");
				Console.WriteLine("ykd - refresh yaw pid derivative term");
				Console.WriteLine("tmv - refresh throttle mean value");
				Console.WriteLine("rmv - refresh roll mean value");
				Console.WriteLine("pmv - refresh pitch mean value");
				Console.WriteLine("ymv - refresh yaw mean value");
				Console.WriteLine("e.g. nh 90 ");
                break;
			case "nh":
				double h = -1;
				try
				{
					h = double.Parse(query[1]);					
				}
				catch(FormatException e)
				{
					Console.WriteLine("Error parsing heading: " + e);
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified exeption: " + e);
					Console.WriteLine();
					return false;
				}
				if(h < 0 || h >=360)
				{
					Console.WriteLine("Heading out of range. It must be range between 0 and 360 degrees [0, 360)");
					Console.WriteLine();
					return false;
				}
				this.nav.SetHeading(h);
				break;
			case "nv":
				double v = -1;
				try
				{
					v = double.Parse(query[1]);
				}
				catch(FormatException e)
				{
					Console.WriteLine ("Error parsing speed");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (v < 0 || v > 20)
				{
					Console.WriteLine("Velocity out of range. It must be range between 0 and 20 knots");
					Console.WriteLine();
					return false;
				}
				this.nav.SetSpeed(v);
				break;
			case "na":
				double a = -1;
				try
				{
					a = double.Parse(query[1]);
				}
				catch(FormatException e)
				{
					Console.WriteLine ("Error parsing altitude");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (a < 0 || a > 10000)
				{
					Console.WriteLine("Altitude out of range. It must be range between 0 and 10000 m");
					Console.WriteLine();
					return false;
				}
                Console.WriteLine("Selected altitude in feet:{0}", (int)(a/0.303));
				this.nav.SetAltitude(a);
				break;
			case "ny":
				double ny = -1;
				try
				{
					ny = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (ny < -25 || ny > 25)
				{
					Console.WriteLine("Yaw out of range. It must be range between -25 and 25 degrees");
					Console.WriteLine();
					return false;
				}
				this.nav.UpdateYawRef(ny);
				break;
			case "np":
				double np = -1;
				try
				{
					np = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (np < -15 || np > 15)
				{
					Console.WriteLine("Pitch out of range. It must be range between -15 and 15 degrees");
					Console.WriteLine();
					return false;
				}
				this.nav.UpdatePitchRef(np);
				break;
			case "nr":
				double nr = -1;
				try
				{
					nr = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (nr < -25 || nr > 25)
				{
					Console.WriteLine("Roll out of range. It must be range between -25 and 25 degrees");
					Console.WriteLine();
					return false;
				}
				this.nav.UpdateRollRef(nr);
				break;
			case "nt":
				double nt = -1;
				try
				{
					nt = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				if (nt < 0 || nt > 100)
				{
					Console.WriteLine("Throttle out of range. It must be range between 0% and 100%");
					Console.WriteLine();
					return false;
				}
				this.nav.UpdateThrottleRef(nt);
				break;
			case "ia":
				Console.WriteLine("Current Altitude: " + ga.Adc.altitude);
				val = ga.Adc.altitude;
				break;
			case "iv":
				Console.WriteLine("Current airspeed: " + ga.Adc.tas);
				val = ga.Adc.tas;
				break;
			case "ih":
				Console.WriteLine("Current yaw: " + ga.Imu.yaw);
				val = ga.Imu.yaw;
				break;
			case "ir":
				Console.WriteLine("Current roll: " + ga.Imu.roll);
				val = ga.Imu.roll;
				break;
			case "ip":
				Console.WriteLine("Current pitch: " + ga.Imu.pitch);
				val = ga.Imu.pitch;
				break;
            case "tm":
                this.nav.Switch(XNavManager.Mode.ATTITUDE);
                break;
			case "mm":
				this.nav.Switch(XNavManager.Mode.MANUAL);
				break;
			case "dm":
				this.nav.Switch(XNavManager.Mode.DIRECTED);
				break;
			case "am":
				this.nav.Switch(XNavManager.Mode.AUTONOMOUS);
				break;
			case "ctm":
				this.nav.Switch(XNavManager.Mode.CALIBRATION_THROTTLE);
				break;
			case "crm":
				this.nav.Switch(XNavManager.Mode.CALIBRATION_ROLL);
				break;
			case "cpm":
				this.nav.Switch(XNavManager.Mode.CALIBRATION_PITCH);
				break;
			case "cym":
				this.nav.Switch(XNavManager.Mode.CALIBRATION_YAW);
				break;
			case "tts":
				double tts = -1;
				try
				{
					tts = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.TS, tts);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "tiv":
				double tiv = -1;
				try
				{
					tiv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.INITIAL_VAL, tiv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "tkp":
				double tkp = -1;
				try
				{
					tkp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.KP, tkp);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "tki":
				double tki = -1;
				try
				{
					tki = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.KI, tki);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "tkd":
				double tkd = -1;
				try
				{
					tkd = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.KD, tkd);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "rts":
				double rts = -1;
				try
				{
					rts = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.TS, rts);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "riv":
				double riv = -1;
				try
				{
					riv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.INITIAL_VAL, riv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "rkp":
				double rkp = -1;
				try
				{
					rkp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.KP, rkp);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "rki":
				double rki = -1;
				try
				{
					rki = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.KI, rki);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "rkd":
				double rkd = -1;
				try
				{
					rkd = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.KD, rkd);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "pts":
				double pts = -1;
				try
				{
					pts = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.TS, pts);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "piv":
				double piv = -1;
				try
				{
					piv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.INITIAL_VAL, piv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "pkp":
				double pkp = -1;
				try
				{
					pkp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.KP, pkp);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "pki":
				double pki = -1;
				try
				{
					pki = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.KI, pki);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "pkd":
				double pkd = -1;
				try
				{
					pkd = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.KD, pkd);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "yts":
				double yts = -1;
				try
				{
					yts = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.TS, yts);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "yiv":
				double yiv = -1;
				try
				{
					yiv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.INITIAL_VAL, yiv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "ykp":
				double ykp = -1;
				try
				{
					ykp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.KP, ykp);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "yki":
				double yki = -1;
				try
				{
					yki = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.KI, yki);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "ykd":
				double ykd = -1;
				try
				{
					ykd = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.KD, ykd);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "tmv":
				double tmv = -1;
				try
				{
					tmv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.THROTTLE, PID.Param.MEAN_VAL, tmv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
				
			case "rmv":
				double rmv = -1;
				try
				{
					rmv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.ROLL, PID.Param.MEAN_VAL, rmv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "pmv":
				double pmv = -1;
				try
				{
					pmv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.PITCH, PID.Param.MEAN_VAL, pmv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "ymv":
				double ymv = -1;
				try
				{
					ymv = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				ok = this.pid.RefreshParam(PIDManager.Ctrl.YAW, PID.Param.MEAN_VAL, ymv);
				if(ok)
					Console.WriteLine("OK!");
				else
					Console.WriteLine("Error!!");
				break;
			case "lnkp":
				double lnkp = -1;
				try
				{
					lnkp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				this.nav.latNav.Kp =  lnkp;
				break;
			case "alkp":
				double alkp = -1;
				try
				{
					alkp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				this.nav.alt.Kp =  alkp;
				break;
			case "hekp":
				double hekp = -1;
				try
				{
					hekp = double.Parse(query[1]);
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				this.nav.head.Kp =  hekp;
				break;
			case "dest":
				double lat = -91;
				double lon = -181;
				try
				{
					lat = double.Parse(query[1]);
					lon = double.Parse(query[2]);
					if(lat < -90 || lat > 90 || lon < -180 || lon > 180)
						throw new FormatException("Parameters out of range");
				}
				catch(FormatException)
				{
					Console.WriteLine("Error parsing value");
					Console.WriteLine();
					return false;
				}
				catch(Exception e)
				{
					Console.WriteLine("Unspecified error: " + e.Message);
					Console.WriteLine();
					return false;
				}
				this.nav.latNav.Dest = new WgsPoint(lat, lon,0);
				break;
			}//end switch	
			return true;
		}
	}
}

