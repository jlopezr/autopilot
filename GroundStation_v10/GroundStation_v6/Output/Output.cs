﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using System.IO;

namespace GroundStation
{
    public class Output
    {
        private StreamWriter gpsGeo;
        private StreamWriter gpsUtm;
		private StreamWriter gpsPos;
		private StreamWriter gpsDop;
        private StreamWriter imuEuler;
        //private StreamWriter imuRaw;
        private StreamWriter adc;
		private StreamWriter pwm;
		
		private Path path;

        public Output()
        {
			this.path = Path.GetInstance();
            this.adc = new StreamWriter(this.path.GetPath() + @"/adc.txt", true);
            this.gpsUtm = new StreamWriter(this.path.GetPath() + @"/gpsUtm.txt", true);
            this.gpsGeo = new StreamWriter(this.path.GetPath() + @"/gpsGeo.txt", true);
			this.gpsDop = new StreamWriter(this.path.GetPath() + @"/gpsDop.txt", true);
			this.gpsPos = new StreamWriter(this.path.GetPath() + @"/gpsPos.txt", true);
            this.imuEuler = new StreamWriter(this.path.GetPath() + @"/imuEuler.txt", true);
            //this.imuRaw = new StreamWriter(this.path.GetPath() + @"/imuRaw.txt", true);
			this.pwm = new StreamWriter(this.path.GetPath() + @"/pwm.txt", true);
        }

        public void Flush(Database db)
        {
            foreach (Message m in db.gpsList)
            {
                GpsMessage gpsMess = m as GpsMessage;
                this.gpsUtm.WriteLine(gpsMess.time + "\t" + gpsMess.pos.getUtmX() + "\t" + gpsMess.pos.getUtmY() + "\t" + gpsMess.gndSpeed.V + "\t" + gpsMess.trackAngle.V);
                this.gpsGeo.WriteLine(gpsMess.time + "\t" + gpsMess.pos.getLatitude() + "\t" + gpsMess.pos.getLongitude() + "\t" + gpsMess.gndSpeed.V + "\t" + gpsMess.trackAngle.V);
            }
			
			foreach(Message m in db.dopList)
			{
				GpsDopMessage dopMess = m as GpsDopMessage;
				this.gpsDop.WriteLine(dopMess.time + "\t" + dopMess.pdop.V + "\t" + dopMess.hdop.V + "\t" + dopMess.vdop.V);
			}
			
			foreach(Message m in db.posList)
			{
				GpsPosMessage posMess = m as GpsPosMessage;
				this.gpsPos.WriteLine(posMess.time + "\t" + posMess.latitude.V + "\t" + posMess.longitude.V + "\t" + posMess.gndSpeed.V + "\t" + 
				                      posMess.trackAngle.V + "\t" + posMess.latDev + "\t" + posMess.distDest);
			}

            foreach (Message m in db.imuEulerList)
            {
                ImuEulerMessage imuMess = m as ImuEulerMessage;
                this.imuEuler.WriteLine(imuMess.time + "\t" + imuMess.roll.V + "\t" + imuMess.pitch.V + "\t" + imuMess.yaw.V);
            }

            foreach (Message m in db.imuRawList)
            {
                //ImuRawMessage imuMess = m as ImuRawMessage;
                //this.imuRaw.WriteLine(imuMess.time + "\t" + imuMess.accelX + "\t" + imuMess.accelY + "\t" + imuMess.accelZ + "\t" + imuMess.magnetomX + "\t" + imuMess.magnetomY + "\t" + imuMess.magnetomZ);
            }

            foreach (Message m in db.adcList)
            {
                AdcMessage adcMess = m as AdcMessage;
                this.adc.WriteLine(adcMess.time + "\t" + adcMess.barometer.V + "\t" + adcMess.thermometer.V + "\t" + adcMess.pitot.V + "\t" + adcMess.tas + "\t" + adcMess.altitude);
            }
          

            this.imuEuler.Flush();
            //this.imuRaw.Flush();
            this.gpsUtm.Flush();
            this.gpsGeo.Flush();
			this.gpsDop.Flush();
			this.gpsPos.Flush();
            this.adc.Flush();
			this.pwm.Flush();
			
            db.Initialize();
        }

        public void Close()
        {
            this.adc.Close();
            this.gpsGeo.Close();
            this.gpsUtm.Close();
            this.imuEuler.Close();
            //this.imuRaw.Close();
        }
		
		public void WritePwm(ulong time, byte[] val)
		{
			if(this.pwm != null)
				this.pwm.WriteLine(time + "\t" + val[0] + "\t" + val[1] + "\t" + val[2] + "\t" + val[3]);
		}
    }
}
