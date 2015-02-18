using System;

namespace GroundStation
{
	public class Segment2D
	{
        public WgsPoint pi;
        public WgsPoint pf;
        public WgsPoint intersect;
        public double v1, v2, v3; //Direction vector of the track
        public double A, B, D; //Coefficients of the perpendicular plain to the track
        public double v11, v22; //Coefficients of the aircraft position vector

        /// <summary>
        /// Constructor. Sets the two points that define the straight line.
        /// </summary>
        /// <param name="pi">A point from the straight line</param>
        /// <param name="pf">Another point from the straight line</param>
        public Segment2D(WgsPoint pi, WgsPoint pf)
        {
            this.ForceTimeZone(ref pi, ref pf);
            this.pi = pi;
            this.pf = pf;
            this.SetDirectionVector();
        }

        /// <summary>
        /// Forces to equal te time zone of both points --> p2.TimeZone() = p1.TimeZone();
        /// </summary>
        private void ForceTimeZone(ref WgsPoint p1, ref WgsPoint p2)
        {
            int forcedTimeZone = p1.getTimeZone();
            if (p2.getTimeZone() == forcedTimeZone)
                return;
            /*double s, az1, az2;
            Geodesy.geo_inverse_wgs_84((p1.getAltitude().Value + p2.getAltitude().Value) / 2, p1.getLatitude(), p1.getLongitude(), p2.getLatitude(), p2.getLongitude(), out az1, out az2, out s);
            double p2UtmX = p1.getUtmX() + s * Math.Sin(az1*Math.PI/180.0);
            double p2UtmY = p1.getUtmY() + s * Math.Cos(az1*Math.PI/180.0);
            return new WgsPoint(p2UtmX, p2UtmY, p2.getAltitude(), p1.getTimeZone(), p1.GetHemisphere());*/
            double refMeridian = (p1.getLongitude() + p2.getLongitude()) / 2;
            p1 = new WgsPoint(p1.getLatitude(), p1.getLongitude(), p1.getAltitude(), refMeridian);
            p2 = new WgsPoint(p2.getLatitude(), p2.getLongitude(), p2.getAltitude(), refMeridian);
        }

        /// <summary>
        /// Calculates the direction vector (V) coefficients belonging to the straight line.
        /// V = {v1,v2,v3}
        /// pi = {pi1,pi2,pi3}
        /// pf = {pf1,pf2,pf3}
        /// v1 = pf1-pi1
        /// v2 = pf2-pi2
        /// </summary>
        private void SetDirectionVector()
        {
            this.v1 = pf.getUtmX() - pi.getUtmX();
            this.v2 = pf.getUtmY() - pi.getUtmY();
            this.v3 = pf.getAltitude().Value - pi.getAltitude().Value;
        }

        /// <summary>
        /// Calculates the coefficients of the perpendicular plain (S).
        /// S: A*x + B*y + D = 0
        /// From the Euclidean Geometry about plains at right angles to direction vectors:
        /// A = v1
        /// B = v2
        /// We only have to calculate coefficient D in order to ensure that point p is contained by the plain:
        /// p = {p1,p2}
        /// D = -(A*p1 + B*p2)
        /// </summary>
        /// <param name="p">The point contained by the plain</param>

        private void SetPlainCoefficients(WgsPoint p)
        {
            this.A = this.v1;
            this.B = this.v2;
            this.D = (double)((-1) * (this.A * p.getUtmX() + this.B * p.getUtmY()));
        }

        /// <summary>
        /// Calculates the intersection point between a straight line, defined by a point (P) and a direction vector (V), 
        /// and a perpendicular plain (S). 
        /// Using a parametric equation:
        /// V = {v1,v2}
        /// P = {p1,p2}
        /// S: A*x + B*y + D = 0
        /// x = p1 + v1*t
        /// y = p2 + v2*t
        /// Hence:
        /// t = -(A*p1 + B*p2 + D)/(A*v1 + B*v2)
        /// </summary>

        private void GetIntersectionPoint()
        {
            double t = (double)((-1) * (this.A * pi.getUtmX() + this.B * pi.getUtmY() + this.D) / (this.v1 * this.A + this.v2 * this.B));
            double x = pi.getUtmX() + this.v1 * t;
            double y = pi.getUtmY() + this.v2 * t;
            double z = pi.getAltitude().Value + this.v3 * t;
            this.intersect = new WgsPoint(x, y, z, this.pi.getTimeZone(), 'N');
        }

        /// <summary>
        /// Calculates the euclidean distance (d) between two points (p1,p2)
        /// p1 = {p11,p12,p13}
        /// p2 = {p21,p22,p23}
        /// d = sqrt((p21-p11)^2 + (p22-p12)^2 + (p23-p13)^2)
        /// </summary>
        /// <param name="p">The position of the aircraft</param>
        /// <returns></returns>
        public double GetHorDistance(WgsPoint p)
        {
            p = new WgsPoint(p.getLatitude(), p.getLongitude(), p.getAltitude(), this.pi.GetRefMeridian());
            this.SetPlainCoefficients(p);
            this.GetIntersectionPoint();
            double horDistance = Math.Sqrt(
                Math.Pow((p.getUtmX() - this.intersect.getUtmX()), 2) +
                Math.Pow((p.getUtmY() - this.intersect.getUtmY()), 2));
            return horDistance;
        }

        public double GetVerDistance(WgsPoint p)
        {
            p = new WgsPoint(p.getLatitude(), p.getLongitude(), p.getAltitude(), this.pi.GetRefMeridian());
            this.SetPlainCoefficients(p);
            this.GetIntersectionPoint();
            double verDistance = p.getAltitude().Value - this.intersect.getAltitude().Value;
            return verDistance;
        }


        /// <summary>
        /// Returns the distances and the angles to the plane and point defined by pi and intersect, when compared to another point p.
        /// </summary>
        /// <param name="p">Airplane position</param>
        /// <returns>horDistance, verDistance, alpha, beta, b, DistToPlain</returns>
        public double[] getDistancesAngles(WgsPoint p)
        {
            p = new WgsPoint(p.getLatitude(), p.getLongitude(), p.getAltitude(), this.pi.GetRefMeridian());
            this.SetPlainCoefficients(p);
            this.GetIntersectionPoint();
            double DistToPlain = this.GetDistanceToPlain();

            double absDistance = Math.Sqrt(
                Math.Pow((p.getUtmX() - this.intersect.getUtmX()), 2) +
                Math.Pow((p.getUtmY() - this.intersect.getUtmY()), 2) 
                );

            double horDistance = Math.Sqrt(
                Math.Pow((p.getUtmX() - this.intersect.getUtmX()), 2) +
                Math.Pow((p.getUtmY() - this.intersect.getUtmY()), 2));

            double verDistance = p.getAltitude().Value - this.intersect.getAltitude().Value;

            double vx = this.pi.getUtmX() - p.getUtmX();
            double vy = this.pi.getUtmY() - p.getUtmY();

            double a = Math.Sqrt(
                    Math.Pow(p.getUtmX() - this.intersect.getUtmX(), 2) +
                    Math.Pow(p.getUtmY() - this.intersect.getUtmY(), 2));
            double b = Math.Sqrt(Math.Pow(vx, 2) + Math.Pow(vy, 2));
            double c = Math.Sqrt(
                    Math.Pow(this.pi.getUtmX() - this.intersect.getUtmX(), 2) +
                    Math.Pow(this.pi.getUtmY() - this.intersect.getUtmY(), 2));
            double alpha = ToDeg(CosinusLaw(a, b, c));

            if (this.GetSide(p))	//Change sign if airplane is located to the left of the track
                alpha *= -1;

            return new double[] { horDistance, verDistance, alpha, b, DistToPlain };
        }

        /// <summary>
        /// This method returns the distance to the plain defined by pi and intersect
        /// </summary>
        /// <returns></returns>
        public double GetDistanceToPlain()
        {
            return Math.Sqrt(Math.Pow(intersect.getUtmX() - pi.getUtmX(), 2) +
                                Math.Pow(intersect.getUtmY() - pi.getUtmY(), 2));
        }

        public bool GetSide(WgsPoint p)
        {
            p = new WgsPoint(p.getLatitude(), p.getLongitude(), p.getAltitude(), this.pi.GetRefMeridian());
            double ans = (double)((-1) * (this.pf.getUtmY() - this.pi.getUtmY()) * (p.getUtmX() - this.pi.getUtmX()) + (this.pf.getUtmX() - this.pi.getUtmX()) * (p.getUtmY() - this.pi.getUtmY()));

            return ans > 0;
        }

        #region Math Utils
        public static double ToDeg(double d)
        {
            return d * 180.0 / Math.PI;
        }
        public static double ToRad(double d)
        {
            return d * Math.PI / 180.0;
        }
        public static double CosinusLaw(double a, double b, double c)
        {
            return Math.Acos((Math.Pow(b, 2) + Math.Pow(c, 2) - Math.Pow(a, 2)) / (2 * b * c));
        }
        #endregion
    }
}
