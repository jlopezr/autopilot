using System;
using System.Collections.Generic;
using System.IO;

namespace GroundStation
{
	public class LatNav
	{
		private NavManager nav;
		
		private Queue<WgsPoint> dest;
		private WgsPoint currPos;
		
		private double bearing;
		
		private object mutex;
		private object mutex2;
		
		private StreamWriter latErrSw;
		private StreamWriter distDestSw;
		
		public WgsPoint Dest
		{
			get
			{
				WgsPoint ans;
				lock(this.mutex)
				{
					ans = this.dest.Peek().DeepCopy();
				}
				return ans;
			}
			set
			{
				lock(this.mutex)
				{
					WgsPoint newDest = value.DeepCopy();
					this.dest.Enqueue(newDest);
				}
				this.GetRef();
			}
		}
		
		public WgsPoint CurrPos
		{
			get
			{
				WgsPoint ans;
				lock(this.mutex)
				{
					ans = this.currPos.DeepCopy();
				}
				return ans;
			}
			set
			{
				lock(this.mutex)
				{
					this.currPos = value.DeepCopy();
					if(this.dest.Count > 1)
						this.WpReached();
					this.bearing = this.CalculateNewBearing(this.currPos, this.dest.Peek());
				}
				this.GetRef();
			}
		}
		
		private double ts;
		private double kp;
		public double Kp
		{
			get
			{
				double ans;
				lock(this.mutex2)
				{
					ans = this.kp;
				}
				return ans;
			}
			set
			{
				lock(this.mutex2)
				{
					this.kp = value;
				}
			}
		}
		
		private double ki;
		private double kd;
		
		private double prev;
		private double acc;
		
		private Segment2D s;
		
		private bool act;
		
		public double distDest;
		public double latDev;
		
		public LatNav (WgsPoint origin, Queue<WgsPoint> dest, double ts, double kp, double ki, double kd)
		{
			this.mutex = new object();
			this.mutex2 = new object();
			this.nav = NavManager.GetInstance();
			
			this.ts = ts;
			this.kp = kp;
			this.ki = ki;
			this.kd = kd;
			
			this.dest = dest;
			this.s = new Segment2D(origin, dest.Peek());
			this.bearing = this.CalculateNewBearing(this.dest.Peek(), origin);
			
			this.acc = 0;
			this.prev = double.MinValue;
			
			Path p = Path.GetInstance();
			this.latErrSw = new StreamWriter(p.GetPath() + "/LatErr.txt");
			this.distDestSw = new StreamWriter(p.GetPath() + "/DistDest.txt");
		}
		
		private void GetRef()
		{
			double dist;
			lock(this.mutex)
			{
				dist = this.s.GetHorDistance(this.currPos);
				this.latDev = dist;
				this.latErrSw.WriteLine(dist);
				this.latErrSw.Flush();
			
				double ans = this.RefreshPid(dist);
				ans = ans > 30 ? ans = 30 : ans;
				ans = ans < -30 ? ans = -30 : ans;
				Console.WriteLine("OUT LatNav: " + ans);
				Console.WriteLine("OUT dist err: " + dist);
				if(this.s.GetSide(this.currPos))
					this.nav.SetHeading(this.bearing + ans);
				else
					this.nav.SetHeading(this.bearing - ans);
			}
		}
		
		public void activate()
		{
			this.act = true;
		}
		
		public void deactivate()
		{
			this.act = false;
		}
		
		public bool IsActive()
		{
			return this.act;
		}
			
		private double RefreshPid(double input)
		{
			this.acc += input;
            double diff = prev != double.MinValue ? input - prev : 0;
            this.prev = input;
			
			double ans;
			lock(this.mutex2)
			{
				ans =  this.kp*input + this.ki * ts * this.acc + this.kd * diff / this.ts;
			}
			return ans;
		}
		
		// Calculate the bearing between two points
        public double CalculateNewBearing(WgsPoint wp1, WgsPoint wp2)
        {
            double az1 = 0, az2 = 0, dist = 0;
			double alt = (wp1.getAltitude().Value + wp2.getAltitude().Value) / 2;
			double lat1 = wp1.getLatitude();
			double lon1 = wp1.getLongitude();
			double lat2 = wp2.getLatitude();
			double lon2 = wp2.getLongitude();
			
			Geodesy.geo_inverse_wgs_84(alt, lat1, lon1, lat2, lon2, out az1, out az2, out dist);
            //True heading -->magnetic heading correction
            az1 += -0.45;

            return az1;
        }
		
		public void WpReached()
		{
			double az1 = 0, az2 = 0, dist = 0;
			double alt = (this.dest.Peek().getAltitude().Value + this.currPos.getAltitude().Value) / 2;
			double lat1 = this.currPos.getLatitude();
			double lon1 = this.currPos.getLongitude();
			double lat2 = this.dest.Peek().getLatitude();
			double lon2 = this.dest.Peek().getLongitude();
			Geodesy.geo_inverse_wgs_84(alt, lat1, lon1, lat2, lon2, out az1, out az2, out dist);
			this.distDest = dist;
			this.distDestSw.WriteLine(dist);
			this.distDestSw.Flush();
            Console.WriteLine("Dist to WP {0}", dist);
			if(dist < 200) //Modify here maximum distance to WP before turning
			{
				WgsPoint last = this.dest.Dequeue();
				this.s = new Segment2D(last, this.dest.Peek());
				this.bearing = this.CalculateNewBearing(last, this.dest.Peek());
			}
		}
	}
}

