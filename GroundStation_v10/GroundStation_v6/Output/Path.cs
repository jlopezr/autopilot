using System;
using System.IO;

namespace GroundStation
{
	public class Path
	{
		private static readonly string path = Directory.GetCurrentDirectory()+System.IO.Path.DirectorySeparatorChar+"Logs";
		private string folder;
		private static Path instance = null;
		
		public static Path GetInstance()
		{
			if(instance == null)
				instance = new Path();
			return instance;
		}
		
		private Path ()
		{
			string[] dirPaths = Directory.GetDirectories(path);
			this.folder = dirPaths.Length.ToString();
			Directory.CreateDirectory(path + this.folder);
		}
		
		public string GetPath()
		{
			return (path + this.folder);
		}
	}
}

