using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneLibrary
{
    public class Situation
    {
        float yaw;
        float pitch;
        float roll;
        float AoA;
        float speed;

        public void set_yaw(float a) //Ahora Yaw=HDG(true)
        {
            yaw = a;
        }

        public float get_yaw()
        {
            return(yaw);
        }

        public float read_yaw()
        {
            CommsUDP C = new CommsUDP();
            Situation S = C.ReadSituation();
            return (S.yaw);
        }

        public void set_pitch(float b)
        {
            pitch = b;
        }

        public float get_pitch()
        {
            return(pitch);
        }

        public float read_pitch()
        {
            CommsUDP C = new CommsUDP();
            Situation S = C.ReadSituation();
            return (S.pitch);
        }

        public void set_roll(float c)
        {
            roll = c;
        }

        public float get_roll()
        {
            return(roll);
        }

        public float read_roll()
        {
            CommsUDP C = new CommsUDP();
            Situation S = C.ReadSituation();
            return (S.roll);
        }

        public void set_AoA(float d)
        {
            AoA = d;
        }

        public float get_AoA()
        {
            return(AoA);
        }

        public float read_AoA()
        {
            CommsUDP C = new CommsUDP();
            Situation S = C.ReadSituation();
            return (S.AoA);
        }

        public void set_speed(float e)
        {
            speed = e;
        }

        public float get_speed()
        {
            return(speed);
        }

        public float read_speed()
        {
            CommsUDP C = new CommsUDP();
            Situation S = C.ReadSituation();
            return (S.speed);
        }

        public Situation(float a, float b, float c, float d, float e)
        {
            yaw = a;
            pitch = b;
            roll = c;
            AoA = d;
            speed = e;
        }
        public Situation()
        {
            
        }
    }
}
