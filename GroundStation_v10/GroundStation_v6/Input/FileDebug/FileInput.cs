using System;
using System.IO;

using System.Threading;

namespace GroundStation
{
	public abstract class FileInput
	{
		protected StreamReader sr;
		
		protected byte[] data;
		
		public bool end;
		
		public byte[] Data
		{
			get
			{
				if(!end)
				{
					byte[] ans = new byte[data.Length];
					this.data.CopyTo(ans,0);
					Thread t = new Thread(new ThreadStart(this.UpdateInfo));
					t.Start();
					return ans;
				}
				return null;
			}
		}
		
		public FileInput (string path)
		{
			this.sr = new StreamReader(path);
			this.end = false;
		}
		
		protected void UpdateInfo()
		{
			try
			{
				this.ReadLine();
				this.ToByteArray();
			}
			catch(Exception)
			{
				Console.WriteLine("END " + this.GetType());
				this.end = true;
			}
				
		}
		
		protected abstract bool ReadLine();
		protected abstract void ToByteArray();
	}
}

