using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneLibrary
{
    public class Controls
    {
        float ail;
        float elev;
        float rud;
        float thr;

        public void set_ail(float a)
        {
            ail = a;
        }

        public float get_ail()
        {
            return (ail);
        }

        public float read_ail()
        {
            CommsUDP Ca = new CommsUDP();
            Controls Con = Ca.ReadControls();
            return (Con.ail);
        }

        public void set_elev(float b)
        {
            elev = b;
        }

        public float get_elev()
        {
            return (elev);
        }

        public float read_elev()
        {
            CommsUDP Ca = new CommsUDP();
            Controls Con = Ca.ReadControls();
            return (Con.elev);
        }

        public void set_rud(float c)
        {
            rud = c;
        }

        public float get_rud()
        {
            return (rud);
        }

        public float read_rud()
        {
            CommsUDP Ca = new CommsUDP();
            Controls Con = Ca.ReadControls();
            return (Con.rud);
        }
        

        public void set_thr(float d)
        {
            thr = d;
        }

        public float get_thr()
        {
            return (thr);
        }

        public float read_thr()
        {
            CommsUDP Ca = new CommsUDP();
            Controls Con = Ca.ReadControls();
            return (Con.thr);
        }

        public Controls(float a, float b, float c, float d)
        {
            ail = a;
            elev = b;
            rud = c;
            thr = d;
        }
        public Controls()
        {
            ail = -999;
            elev = -999;
            rud = -999;
            thr = -999;
        }
    }
}
