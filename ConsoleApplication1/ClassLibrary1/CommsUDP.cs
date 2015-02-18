using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace PlaneLibrary
{
    public class CommsUDP
    {
        public Controls ReadControls()
        {
            //Leer, traducir a float y devolver

            Controls Ctrl = new Controls();
            
            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 49003);
            UdpClient newsock = new UdpClient(ipep);

            //Console.WriteLine("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            //Console.WriteLine("X-Plane Data Read: \n\n");

            data = newsock.Receive(ref sender);

            /*for (int index = 0; index < data.Length; index++)
            {
                Console.Write("{0},", data[index]);
            }
            Console.Write("\n\n\n");*/

            for (int index = 5; index < (data.Length - 36); index += 36) 
            {
                int ident = 0;
                //int start;
                
                ident = data[index];
                index = index + 4;
                
               
                
                byte[] p1 = { data[index], data[index + 1], data[index + 2], data[index + 3] };
                byte[] p2 = { data[index + 4], data[index + 5], data[index + 6], data[index + 7] };
                byte[] p3 = { data[index + 8], data[index + 9], data[index + 10], data[index + 11] };
                byte[] p4 = { data[index + 12], data[index + 13], data[index + 14], data[index + 15] };
                byte[] p5 = { data[index + 16], data[index + 17], data[index + 18], data[index + 19] };
                byte[] p6 = { data[index + 20], data[index + 21], data[index + 22], data[index + 23] };
                byte[] p7 = { data[index + 24], data[index + 25], data[index + 26], data[index + 27] };
                byte[] p8 = { data[index + 28], data[index + 29], data[index + 30], data[index + 31] };

                float f1 = System.BitConverter.ToSingle(p1, 0);
                float f2 = System.BitConverter.ToSingle(p2, 0);
                float f3 = System.BitConverter.ToSingle(p3, 0);
                float f4 = System.BitConverter.ToSingle(p4, 0);
                float f5 = System.BitConverter.ToSingle(p5, 0);
                float f6 = System.BitConverter.ToSingle(p6, 0);
                float f7 = System.BitConverter.ToSingle(p7, 0);
                float f8 = System.BitConverter.ToSingle(p8, 0);


                if (ident == 3 || ident == 17 || ident == 18)
                {
                    //index = index + 36;
                }

                if (ident == 11)
                {
                    Ctrl.set_elev(f1);
                    Ctrl.set_ail(f2);
                    Ctrl.set_rud(f3);
                    //index = index + 36;
                }
                    
                if (ident == 25)
                {
                    Ctrl.set_thr(f1);
                    //index = index + 36;
                }

                index = index - 4;
                
                //Console.Write("{0},   ", f1);
            }

                

            /*byte[] p = {0, 192, 121, 196};

            float f1 = System.BitConverter.ToSingle(p, 0);
            Console.Write("{0},", f1);*/

            //Console.ReadKey(true);
            newsock.Close();
            /*byte last=
            double doubleVal = System.Convert.ToDouble(last);
            System.Console.WriteLine("{0} as a double is: {1}.",byteVal, doubleVal);*/
            return Ctrl;
        }


        public Situation ReadSituation()
        {
            //Leer, traducir a float y devolver

            Situation Stn = new Situation();

            byte[] data = new byte[1024];
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 49003);
            UdpClient newsock = new UdpClient(ipep);

            Console.WriteLine("Waiting for a client...");
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

            Console.WriteLine("X-Plane Data Read: \n\n");

            data = newsock.Receive(ref sender);

            /*for (int index = 0; index < data.Length; index++)
            {
                Console.Write("{0},", data[index]);
            }
            Console.Write("\n\n\n");*/
            int ident = 0;
            for (int index = 5; index < (data.Length - 36); index += 36)
            {
                
                //int start;

                
                ident = Convert.ToInt32(data[index]);
                index = index + 4;
                
                
                /*byte[] p1 = { data[index], data[index + 1], data[index + 2], data[index + 3] };
                byte[] p2 = { data[index + 4], data[index + 5], data[index + 6], data[index + 7] };
                byte[] p3 = { data[index + 8], data[index + 9], data[index + 10], data[index + 11] };
                byte[] p4 = { data[index + 12], data[index + 13], data[index + 14], data[index + 15] };
                byte[] p5 = { data[index + 16], data[index + 17], data[index + 18], data[index + 19] };
                byte[] p6 = { data[index + 20], data[index + 21], data[index + 22], data[index + 23] };
                byte[] p7 = { data[index + 24], data[index + 25], data[index + 26], data[index + 27] };
                byte[] p8 = { data[index + 28], data[index + 29], data[index + 30], data[index + 31] };*/

                float f1 = System.BitConverter.ToSingle(data, index);
                float f2 = System.BitConverter.ToSingle(data, index + 4);
                float f3 = System.BitConverter.ToSingle(data, index + 8);
                float f4 = System.BitConverter.ToSingle(data, index + 12);
                float f5 = System.BitConverter.ToSingle(data, index + 16);
                float f6 = System.BitConverter.ToSingle(data, index + 20);
                float f7 = System.BitConverter.ToSingle(data, index + 24);
                float f8 = System.BitConverter.ToSingle(data, index + 28);


                if (ident == 11 || ident == 25 )
                {
                    //index = index + 36;
                }

                if (ident == 3)
                {
                    Stn.set_speed(f1);
                    //index = index + 36;
                }

                if (ident == 17)
                {
                    Stn.set_pitch(f1);
                    Stn.set_roll(f2);
                    Stn.set_yaw(f3); //Esto es HDG(true)
                    //index = index + 36;
                }
                    
                if (ident == 18)
                {
                    Stn.set_AoA(f1);
                    //index = index + 36;
                }


                index = index - 4;
                //Console.Write("{0},   ", f1);
            }



            /*byte[] p = {0, 192, 121, 196};

            float f1 = System.BitConverter.ToSingle(p, 0);
            Console.Write("{0},", f1);*/

            //Console.ReadKey(true);
            newsock.Close();
            /*byte last=
            double doubleVal = System.Convert.ToDouble(last);
            System.Console.WriteLine("{0} as a double is: {1}.",byteVal, doubleVal);*/
            return Stn;
        }

        
        
        public void SendControls(Controls c)
        {
            //traducir a bytes, organizarlos y enviar

            //byte[] XFData = new byte[1024]; // Array to hold entire data string to be sent to X-Flight

            UdpClient server = new UdpClient("127.0.0.1", 49000);


            // --- 11: Flight Controls ---
            // Pitch: 0.20
            //byte[] XFData = { 68, 65, 84, 65, 0, 11, 0, 0, 0, 205, 204, 76, 62, 0, 192, 121, 196, 0, 192, 121, 196, 0, 192, 121, 196, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // Pitch: 0.00
            //byte[] XFData = { 68, 65, 84, 65, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 192, 121, 196, 0, 192, 121, 196, 0, 192, 121, 196, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            float ail = c.get_ail();
            float elev = c.get_elev();
            float rud = c.get_rud();
            float thr = c.get_thr();

            byte[] ailb = BitConverter.GetBytes(ail);
            byte[] elevb = BitConverter.GetBytes(elev);
            byte[] rudb = BitConverter.GetBytes(rud);
            byte[] thrb = BitConverter.GetBytes(thr);
            byte[] nulb = {192, 121, 196, 0};

            byte[] genheader = { 68, 65, 84, 65, 0 };
            byte[] conheader = { 11, 0, 0, 0 };
            byte[] thrheader = { 25, 0, 0, 0 };

            byte[] XFData = new byte[41];
            System.Buffer.BlockCopy(genheader, 0, XFData, 0, genheader.Length);
            System.Buffer.BlockCopy(conheader, 0, XFData, genheader.Length, 4);
            System.Buffer.BlockCopy(elevb, 0, XFData, 9, 4);
            System.Buffer.BlockCopy(ailb, 0, XFData, 13, 4);
            System.Buffer.BlockCopy(rudb, 0, XFData, 17, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 21, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 25, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 29, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 33, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 37, 4);

            server.Send(XFData, XFData.Length);

            System.Buffer.BlockCopy(genheader, 0, XFData, 0, genheader.Length);
            System.Buffer.BlockCopy(thrheader, 0, XFData, genheader.Length, 4);
            System.Buffer.BlockCopy(thrb, 0, XFData, 9, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 13, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 17, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 21, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 25, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 29, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 33, 4);
            System.Buffer.BlockCopy(nulb, 0, XFData, 37, 4);  //Alguna manera mejor de hacer esto?

            // Send the data to X-Plane
            server.Send(XFData, XFData.Length);
            Console.WriteLine("Controls sent");

            server.Close();
            //Console.ReadKey(true); // Wait for keypress to close program



        }
        
        
        //Añadir sendSituation para el AP
        
        
        public CommsUDP()
        {
            
        }

        
    }
}
