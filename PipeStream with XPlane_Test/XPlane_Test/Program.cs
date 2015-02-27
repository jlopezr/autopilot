using System;
using XPlane;
using System.Threading;
using System.IO;
using System.Text;

namespace XPlane_Test
{


	class MainClass
	{
		public static void Main (string[] args)
		{
			XplanePacketsId.Load (XplaneVersion.Xplane10);
			XplaneConnection connection = new XplaneConnection ();
			XplaneParser parser = new XplaneParser (connection);

			connection.OpenConnections ();
			parser.Start ();

			//connection.SendPacket(XplanePacketGenerator.JoystickPacket (0, 0, 0, 0));

			//Console.ReadKey ();




            /*PipeStream mPipeStream = new PipeStream();

            // create some threads to read and write data using PipeStream
            Thread readThread = new Thread(new ThreadStart(ReaderThread));
            Thread writeThread = new Thread(new ThreadStart(WriterThread));
            readThread.Start();
            writeThread.Start();

            writeThread.Join();
            readThread.Join();*/

		}

       /* private static void WriterThread()
        {
            StreamReader File = new StreamReader("myFile.txt");
            PipeStream sw = new PipeStream();
            string inputFile = File.ReadToEnd();
            int writeSize = 1024;
            string str = inputFile;
            for (int i = 0; i < str.Length; i += writeSize)
            {
                // select a substring of characters from the input string
                string substring = str.Substring(i,
                    (i + writeSize < str.Length) ? writeSize : str.Length - i);
                byte[] cosas = Encoding.Unicode.GetBytes(substring.ToCharArray());
                sw.Write(cosas, 0, substring.Length);
            }
        }

        private static void ReaderThread()
        {
            StreamReader sr = new StreamReader();
            char[] buffer = new char[80];
            while (!sr.EndOfStream)
            {
                int readLength = sr.Read(buffer, 0, buffer.Length);
                // do something productive with buffer
            }
        }*/
	}
}
