using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace XPlane
{
    public class Write
    {
        public void WriterThread(object ps, object bytes)
        {
            //StreamReader File = new StreamReader("myFile.txt");
            while (true)
            {
                PipeStream sw = (PipeStream)ps;
                byte[] bytes2 = (byte[])bytes;
                if (bytes2 != null)
                {
                    sw.Write(bytes2, 0, bytes2.Length);
                }
            }
            
        }
    }
}
