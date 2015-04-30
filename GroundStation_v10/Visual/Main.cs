using System;
using System.Threading;
using GroundStation;

namespace Visual
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Thread t = new Thread(new ThreadStart(GroundStation.Main.Run));
			t.Start();
		}
	}
}
