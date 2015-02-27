using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace XPlane
{
    public class Read
    {
        
        //public StreamWriter estext = new StreamWriter("leido.txt");
        public void ReaderThread(object pipe)
        {
            //Thread.Sleep(500);
            while(true)
            {
                StreamReader sr = new StreamReader((PipeStream)pipe);
                char[] buffer = new char[200];
                while (!sr.EndOfStream)
                {
                    int readLength = sr.Read(buffer, 0, buffer.Length);
                    // do something productive with buffer
                    //Console.WriteLine(buffer);
                    string linea = new string(buffer);
                    Console.WriteLine(linea);
                    //estext.WriteLine(linea);
                    //estext.Flush();
                }
            }
            
        }
    }
}
