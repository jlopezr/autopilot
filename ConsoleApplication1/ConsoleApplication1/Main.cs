
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using System.Net;
using System.Net.Sockets;
using PlaneLibrary;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            CommsUDP cm = new CommsUDP();
            Controls c = new Controls();

            Console.WriteLine("Press ESC to stop");
            do
            {
                double coef = 1;
                double deg1 = 0.4;
                double deg2 = 0.5;
                float ail; //Entre 1 y -1
                float elev; //Entre 1 y -1
                
                Situation s = new Situation();


                while (!Console.KeyAvailable)
                {
                    // prueba de controles con una señal senoidal y muestra datos por consola
                    ail = (float) Math.Sin(deg1*coef);
                    elev = (float)Math.Sin(deg2*coef);
                    c.set_ail(ail);
                    c.set_elev(elev);
                    cm.SendControls(c);
                    s = cm.ReadSituation();
                    coef = coef + 0.1;
                    Console.WriteLine("This is the current situation (pitch/roll/yaw/AoA/speed)");
                    Console.WriteLine(s.get_pitch() + " deg,  " + s.get_roll() + " deg,  " + s.get_yaw() + " deg,  " + s.get_AoA() + " deg,  " + s.get_speed() + " kt");
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

           
            //al acabar se debe enviar un control nulo (-999) para poder retomar los controles en el simulador
            c.set_ail(-999);
            c.set_elev(-999);
            cm.SendControls(c);
        }
    }
}