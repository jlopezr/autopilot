using System;
using System.Collections.Generic;
using System.Text;

using USAL.Points;

namespace GroundStation
{
    [Serializable]
    public class Position
    {
        /// <summary>(m) Range[UAV Range]</summary>
        public float Press_Altitude;

        /// <summary>
        /// Zulu Time (GMT) in decimal hours.
        /// </summary>
        public double zuluTime;

        //In Flight Gear Simulator Press_Altitude will be ground level altitude
        public Position()
        {
        }

        /// <summary>WGS84 Inertial reference frame ECEF Geo-position</summary>
        /// <param name="Latitude">(rad) Range[0,6.2832)</param>
        /// <param name="Longitude">(rad) Range[0,6.2832)</param>
        /// <param name="Altitude">(m) Range[UAV Range]</param>
        /// <param name="Press_Altitude">FG: Ground level altitude (m) Range[UAV Range]</param>
        public Position(double Latitude, double Longitude, float Altitude, float Press_Altitude, double zuluTime)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Altitude = Altitude;
            this.Press_Altitude = Press_Altitude;
            this.zuluTime = zuluTime;
        }

        public Position(double Latitude, double Longitude, float Altitude, float Press_Altitude)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Altitude = Altitude;
            this.Press_Altitude = Press_Altitude;
        }

        /// <summary>
        /// OTR: Generates an USAL Position from an USAL Waypoint
        /// </summary>
        /// <param name="waypoint"></param>
        public Position(Waypoint waypoint)
        {
            this.Latitude = waypoint.latitude;
            this.Longitude = waypoint.longitude;
            this.Altitude = waypoint.altitude;
            this.Press_Altitude = waypoint.altitude;
        }

        /// <summary> The Altitude As a general definition, altitude is a distance measurement,
        /// usually in the vertical or "up" direction, between a reference datum and a point or object.
        /// The reference datum, in our case, means the height above sea level of a location.
        public virtual float Altitude
        {
            get;
            set;
        }

        /// <summary>The Latitude of a location on the Earth is the angular distance of
        /// that location south or north of the equator.
        /// Units (International System of Units): radians
        /// Range: -PI to PI</summary>
        public double Latitude
        {
            get;
            set;
        }

        /// <summary>The Longitude specifies the east-west position of a point on the
        /// Earth's surface.
        /// Units (International System of Units): radians
        /// Range: -PI to PI</summary>
        public double Longitude
        {
            get;
            set;
        }

        public static Position DeepCopy(Position p)
        {
            Position ans = new Position();
            ans.Altitude = p.Altitude;
            ans.Latitude = p.Latitude;
            ans.Longitude = p.Longitude;
            ans.Press_Altitude = p.Press_Altitude;
            if (p.zuluTime != 0)
                ans.zuluTime = p.zuluTime;
            return ans;
        }

        public string getuavPosition()
        {
            if (this.zuluTime != 0)
                return (this.Latitude + " " + this.Longitude + " " + this.Altitude + " " + this.Press_Altitude + " " + this.zuluTime);
            return (this.Latitude + " " + this.Longitude + " " + this.Altitude + " " + this.Press_Altitude);
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("POS\t{0}\t{1}\t{2}\t{3}\t{4}", this.zuluTime, this.Latitude, this.Longitude, this.Altitude, this.Press_Altitude);
        }

        #region Point type conversion methods

        public static Position ToPosition(WgsPoint wp)
        {
            Position ans = new Position();
            ans.Latitude = wp.getLatitude();
            ans.Longitude = wp.getLongitude();
            if (wp.getAltitude().HasValue)
            {
                ans.Altitude = (float)wp.getAltitude().Value;
                ans.Press_Altitude = (float)wp.getAltitude().Value;
            }
            return ans;
        }

        #endregion Point type conversion methods

        #region Position based geometry methods

        /// <summary>
        /// OTR: Returns a Position that is at a given azimuth, elevation and horizontal distance from Position initial
        /// </summary>
        public virtual Position GetDestinationPosition(double brng, double distance, double elevation)
        {
            double R = 6371009;
            double dist = distance / R;  // convert dist to angular distance in radians
            brng = brng * Math.PI / 180;  //
            double lat1 = this.Latitude;
            double lon1 = this.Longitude;
            float alt1 = this.Altitude;

            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dist) +
                        Math.Cos(lat1) * Math.Sin(dist) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dist) * Math.Cos(lat1),
                               Math.Cos(dist) - Math.Sin(lat1) * Math.Sin(lat2));
            float alt2 = alt1 + (float)(distance * Math.Tan(elevation * Math.PI / 180));

            lon2 = (lon2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI;  // normalise to -180..+180º

            return new Position(lat2, lon2, alt2, alt2);
        }       
     
        /// <summary>
        /// OTR: Returns the distance in meters from Position initial to Position final (Haversine Formula)
        /// </summary>
        public virtual double GetDistanceTo(Position final)
        {
            double R = 6371009; //Earth Radius in m
            double lat1 = this.Latitude;
            double lon1 = this.Longitude;
            double lat2 = final.Latitude;
            double lon2 = final.Longitude;
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;
            double dAlt = (double)final.Altitude - (double)this.Altitude;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return Math.Sqrt(Math.Pow(d, 2) + Math.Pow(dAlt, 2));
        }

        /// <summary>
        /// OTR: Returns the elevation in degrees from Position initial to Position final
        /// </summary>
        public virtual double GetElevationTo(Position final)
        {
            double dAlt = final.Altitude - this.Altitude;
            double d = this.GetDistanceTo(final);
            return (Math.Asin(dAlt / d) * 180 / Math.PI) % 360;
        }

        /// <summary>
        /// OTR: Returns the heading in degrees from Position initial to Position final
        /// </summary>
        public virtual double GetHeadingTo(Position final)
        {
            double lat1 = this.Latitude;
            double lon1 = this.Longitude;
            double lat2 = final.Latitude;
            double lon2 = final.Longitude;
            double dLon = lon2 - lon1;

            double y = Math.Sin(dLon) * Math.Cos(lat2);
            double x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);
            double brng = Math.Atan2(y, x);

            return (brng * 180 / Math.PI + 360) % 360;
        }

        /// <summary>
        /// OTR: Returns the horizontal distance in meters from Position initial to Position final (Haversine Formula)
        /// </summary>
        public virtual double GetHorizontalDistanceTo(Position final)
        {
            double R = 6371009; //Earth Radius in m
            double lat1 = this.Latitude;
            double lon1 = this.Longitude;
            double lat2 = final.Latitude;
            double lon2 = final.Longitude;
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Pow(Math.Sin(dLon / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c;
            return d;
        }

        /// <summary>
        /// OTR: Returns the closest Position to THIS that is contained in the segment defined by Positions initial and final
        /// </summary>
        public virtual Position ProjectPosition(Position initial, Position final)
        {
            double a = this.GetDistanceTo(initial);
            double b = this.GetDistanceTo(final);
            double track = initial.GetDistanceTo(final);
            double coveredTrack = (Math.Pow(a, 2) - Math.Pow(b, 2) + Math.Pow(track, 2)) / (2 * track);

            double segmentHeading = initial.GetHeadingTo(final);
            double segmentElevation = initial.GetElevationTo(final);

            return initial.GetDestinationPosition(segmentHeading, coveredTrack, segmentElevation);
        }

        #endregion Position based geometry methods
    }
}