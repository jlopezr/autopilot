using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;


namespace XPlane
{
    public class Write
    {
        public void WriterThread(object obj)//, object bytes)
        {
            Tuple<PipeStream, byte[]> tuple = (Tuple<PipeStream, byte[]>)obj;
            PipeStream sw = tuple.Item1;
            byte[] bytes = tuple.Item2;
            while (true)
            {
                byte[] bytes2 = (byte[])bytes;
                if (bytes2 != null)
                {
                    sw.Write(bytes2, 0, bytes2.Length);
                }
            }
            
        }
    }
}
