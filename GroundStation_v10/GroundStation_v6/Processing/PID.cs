using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundStation
{
    public abstract class PID
    {

		/// <summary>
		/// Parameters.
		/// </summary>
		public enum Param
		{
			TS,
			KP,
			KI,
			KD,
			INITIAL_VAL,
			MEAN_VAL
		}

        public String Name;

		/// <summary>
		/// Indicates whether this PID <see cref="GroundStation.PID"/> is active.
		/// </summary>
		private bool active;
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="GroundStation.PID"/> is active.
		/// </summary>
		/// <value>
		/// <c>true</c> if active; otherwise, <c>false</c>.
		/// </value>/
		public bool Active 
		{
			get
			{
				return this.active;
			}
		}
		
		/// <summary>
		/// The sample time.
		/// </summary>
        private double ts;
		
        /// <summary>
        /// Indicates that this PID has its parameters filled.
        /// </summary>
		private bool filled;

        private double kp;
		public double Kp
		{
			get
			{
				return this.kp;
			}
		}
		
        private double ki;
		public double Ki
		{
			get
			{
				return this.ki;
			}
		}
        private double kd;
		public double Kd
		{
			get
			{
				return this.kd;
			}
		}
		
        private double acc;
        private double prev;
		
		private byte pwmIn;
		private byte currValue;
		protected double refValue;
		
		private int offset;
		private double spanFactor;
		
		private int minVal;
		private int maxVal;
		private int meanVal;
		public int MeanVal
		{
			get
			{
				return this.meanVal;
			}
		}
		
		public object mutex;
		
		protected double initialVal;
		public double InitialVal
		{
			get
			{
				return this.meanVal;
			}
		}

        public PID() { }

        public PID(double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
        {
			this.active = true;
			this.filled = true;
			
			this.mutex = new object();
			
            this.ts = ts;
            
            this.kp = kp;
            this.ki = ki;
            this.kd = kd;
			
			this.offset = offset;
			this.spanFactor = spanFactor;
            
			this.acc = 0;
            this.prev = 0;
			
			this.minVal = minVal;
			this.maxVal = maxVal;
			this.meanVal = meanVal;

            this.refValue = 0;
			this.initialVal = refValue;
        }

        public virtual void SetValue(double input)
        {
			lock(this.mutex)
			{
                //if (Name == "Pitch") {
                //    Console.WriteLine("["+Name+ "] INPUT:" + input +" COMMAND: "+refValue + " ERROR: "+(input-refValue));
                //}
				input = (input - this.refValue - this.initialVal);
				this.RefreshPid(-input);
			}
        }

        public virtual void SetValue(double input, int model)
        {
            lock (this.mutex)
            {
                //if (Name == "Pitch") {
                //    Console.WriteLine("["+Name+ "] INPUT:" + input +" COMMAND: "+refValue + " ERROR: "+(input-refValue));
                //}
                input = (input - this.refValue - this.initialVal);
                this.RefreshPid(-input);
            }
        }
		
		public void RefreshPid(double input)
		{
			this.acc += input;
            double diff = input - prev;
            this.prev = input;

            double ans = this.kp*input + this.ki * this.ts * this.acc + this.kd * diff / this.ts;
			
			ans += this.meanVal;
			ans = ans < this.minVal ? ans = this.minVal : ans;
			ans = ans > this.maxVal ? ans = this.maxVal : ans;
			this.currValue = (byte)Math.Round((ans - this.offset)/this.spanFactor);
		}

        private double prevans = 0;
        public void RefreshPidThrottle(double input) //Added for speed control
		{
            double error = input;
			

            if (error > 10) //Slower than selected speed->max throttle
            {
                this.currValue = (byte)(this.maxVal / this.spanFactor);
            }
            else if (error < -10) //Faster than selected speed->Idle
            {
                this.currValue = (byte)(this.minVal / this.spanFactor);
            }
            else
            {
                this.acc += input;
                double diff = input - prev;
                this.prev = input;

                double ansdiff = this.kp * input + this.ki * this.ts * this.acc + this.kd * diff / this.ts;
                double ans = this.prevans + ansdiff;
                //Console.WriteLine("Throttle:{0}", ans);
                ans += this.meanVal;
                ans = ans < this.minVal ? ans = this.minVal : ans;
                ans = ans > this.maxVal ? ans = this.maxVal : ans;
                //Console.WriteLine("Throttlesend:{0}", ans);
                this.prevans = ans;
                this.currValue = (byte)Math.Round((ans - this.offset) / this.spanFactor);
            }
		}
		
		public byte GetValue()
		{
			//Console.WriteLine("ACT: " + act.ToString());
			if(this.active)
			{
				//Console.WriteLine("GetCurrVal: " + this.currValue);
				byte ans;
				lock(this.mutex)
				{
					ans = this.currValue;
				}
				
				return ans;
			}
			//Console.WriteLine("GetPwmIn: " + this.pwmIn);
			return this.pwmIn;
		
		}
		
		public void SetRefValue(double refValue)
		{
			if(refValue == double.MaxValue)
				return;
			lock(this.mutex)
			{
				this.refValue = refValue;
			}
		}

        
		
		public void SetPwmIn(byte pwmIn)
		{
			lock(this.mutex)
			{
				this.pwmIn = pwmIn;
			}
		}
		
		public void Deactivate()
		{
			this.active = false;
		}
		
		public void Activate(double ts, double kp, double ki, double kd, int offset, double spanFactor, int minVal, int maxVal, int meanVal, double refValue)
		{
			this.ts = ts;
			this.kp = kp;
			this.kd = kd;
			this.offset = offset;
			this.spanFactor = spanFactor;
			this.minVal = minVal;
			this.maxVal = maxVal;
			this.meanVal = meanVal;
			this.filled = true;
			this.active = true;
			this.refValue = 0;
			this.initialVal = refValue;
			
			this.acc = 0;
			this.prev = 0;
			
		}
		
		public bool RefreshParam(Param p, double val)
		{
			bool ok;
			if(!this.filled)
				return false;
			switch(p)
			{
			case Param.INITIAL_VAL:
				lock(this.mutex)
				{
					this.initialVal = val;
					this.refValue = 0;
					ok = this.ReStart();
					Console.WriteLine ("OK: " + ok.ToString());
				}
				return ok;
			case Param.TS:
				lock(this.mutex)
				{
					this.ts = val;
					ok = this.ReStart();
				}
				return ok;
			case Param.KP:
				lock(this.mutex)
				{
					this.kp = val;
					ok = this.ReStart();
				}
				return ok;
			case Param.KI:
				lock(this.mutex)
				{
					this.ki = val;
					ok = this.ReStart();
				}
				return ok;
			case Param.KD:
				lock(this.mutex)
				{
					this.kd = val;
					ok = this.ReStart();
				}
				return ok;
			case Param.MEAN_VAL:
				lock(this.mutex)
				{
					this.meanVal = (int)val;
					ok = this.ReStart();
					Console.WriteLine ("OK: " + ok.ToString());
				}
				return ok;
			default:
				return false;
			}
		}
		
		public double GetParam(Param p)
		{
			if(!this.filled)
				return double.MinValue;
			double ans = double.MaxValue;
			switch(p)
			{
			case Param.INITIAL_VAL:
				lock(this.mutex)
				{
					ans = this.initialVal;
				}
				break;
			case Param.TS:
				lock(this.mutex)
				{
					ans = this.ts;
				}
				break;
			case Param.KP:
				lock(this.mutex)
				{
					ans = this.kp;
				}
				break;
			case Param.KI:
				lock(this.mutex)
				{
					ans = this.ki;
				}
				break;
			case Param.KD:
				lock(this.mutex)
				{
					ans = this.kd;
				}
				break;
			case Param.MEAN_VAL:
				lock(this.mutex)
				{
					ans = this.meanVal;
				}
				break;
			default:
				return double.MinValue;
			}
			return ans;
		}
		
		public bool ReStart()
		{
			if(!this.filled)
				return false;
			
			this.active = true;
			this.acc = 0;
			this.prev = 0;
			return true;
		}
		
		public int GetMeanVal()
		{
			return this.meanVal;
		}

		public int GetOffset()
		{
			return this.offset;
		}
		
		public double GetSpan()
		{
			return this.spanFactor;
		}
			
    }

    //TODO �apa Quitar esta clase
    class EmptyPID : PID
    {
        public EmptyPID()
        {
            Name = "Empty PID";
        }
    }
}
