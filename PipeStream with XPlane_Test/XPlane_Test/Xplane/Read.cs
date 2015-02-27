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
        int tiempo = 0;
        public void ReaderThread(object pipe)
        {
            while(true)
            {
                PipeStream pipebyte = (PipeStream)pipe ;
                //StreamReader sr = new StreamReader((PipeStream)pipe);
                byte[] buffer = new byte[1024];
                while (pipebyte.Length>0)
                {
                    int readLength = pipebyte.Read(buffer, 0, 350);
                    // do something productive with buffer
                    
                    byte[] p = buffer;
                    



                    //descifrar mensaje (solo ADC) para comprobar que lo hemos recibido bien

                    byte[] header = { p[0], p[1], p[2], p[3] };
                    if (header[0] == (byte)1 && header[1] == (byte)1 && header[2] == (byte)1 && header[3] == (byte)0)
                    {
                        int time = p[4];
                        tiempo += time;
                        float roll = (float)((BitConverter.ToSingle(p, 5) / 10000.0) - 180.0);
                        float pitch = (float)((BitConverter.ToSingle(p, 9) / 10000.0) - 180.0);
                        float yaw = (float)((BitConverter.ToSingle(p, 13) / 10000.0) - 180.0);
                        Console.WriteLine("Header:{0}{1}{2}{3} Time:{4} Pitch:{5} Roll:{6} Yaw:{7} ", header[0], header[1], header[2], header[3],tiempo,roll,pitch,yaw);
                    }

                    
                }
            }
            
        }
    }
}
