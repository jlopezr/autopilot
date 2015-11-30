using System;
using System.Threading;
using GroundStation;

namespace Exec
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Thread t = new Thread(new ThreadStart(GroundStation.MainProgram.Run));
			t.Start();
		}
	}
}
