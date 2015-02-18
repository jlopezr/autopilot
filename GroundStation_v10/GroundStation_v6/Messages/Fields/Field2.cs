using System;

namespace GroundStation
{
	public class Field2
	{
		/// <summary>
		/// Current value.
		/// </summary>
        private double v;
		
		/// <summary>
		/// Gets or sets the current value v.
		/// </summary>
		/// <value>
		/// The current value
		/// </value>
        public double V
        {
            get
            {
                return this.v;
            }

            set
            {
                this.v = value;
                this.PreProcessValue();
            }
        }
		
		/// <summary>
		/// The minimum expected value.
		/// </summary>
        public double min;
		
		/// <summary>
		/// The maximum expected value.
		/// </summary>
        public double max;
		
		/// <summary>
		/// The previous received value.
		/// </summary>
        private double prevValue;
		
		private double maxVar;
		
		private bool isFirst = true;
		
		private int maxVarFactor;
		
		public Field2 (double min, double max, double prevValue)
		{
			this.min = min;
			this.max = max;
			this.prevValue = prevValue;
			this.maxVar = double.MaxValue;
			this.maxVarFactor = 1;
		}
		
		public Field2 (double min, double max, double prevValue, double maxVar)
		{
			this.min = min;
			this.max = max;
			this.prevValue = prevValue;
			this.maxVar = maxVar;
			this.maxVarFactor = 1;
		}
		
		private void PreProcessValue()
		{
			double currMaxVar = this.maxVar * this.maxVarFactor;
			//Console.WriteLine("F:" + this.v.ToString("0.000"));
			if(this.min < this.v && this.max > this.v && (this.isFirst|| Math.Abs(this.v - this.prevValue) < currMaxVar))
			{
				this.maxVarFactor = 1;
				this.prevValue = this.v;	
			}
			else
			{
				this.maxVarFactor++;	
				this.v = this.prevValue;
			}
			isFirst = false;
		}
			
		/// <summary>
		/// Creates a clone  of the field f
		/// </summary>
		/// <returns>
		/// The copy.
		/// </returns>
		/// <param name='f'>
		/// The field to clone
		/// </param>
        public static Field2 DeepCopy(Field2 f)
        {
            Field2 ans = new Field2(f.min, f.max, f.prevValue);
            ans.v = f.v;
            return ans;
		}
	}
}

