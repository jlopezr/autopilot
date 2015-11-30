using System;

namespace GroundStation
{
    /// <summary>
    /// This class implements all needed methods to manage waypoints reffered to WGS84 geoid.
    /// </summary>
    public class WgsPoint : Point
    {
        /// <summary>
        /// Polar Radius of Curvature for WGS84 Geoid.
        /// </summary>
        private static readonly double PRC = 6399593.626;

        /// <summary>
        /// Squared Second Eccentricity for WGS84 Geoid.
        /// </summary>
        private static readonly double SSE = 0.006739497;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public WgsPoint()
            : base()
        { }
        /// <summary>
        /// Construct WgsPoint class from projected coordinates.
        /// You need to know the reference meridian (timeZone).
        /// </summary>
        /// <param name="utmX">Waypoint UTM projected, X coordinate.</param>
        /// <param name="utmY">Waypoint UTM projected, Y coordinate.</param>
        /// <param name="altitude">Waypoint altitude</param>
        /// <param name="timeZone">Waypoint time zone</param>
        /// <param name="hemisphere">The referred hemmisphere: "N" for northern hemisphere, otherwise "S"</param>
        public WgsPoint(double utmX, double utmY, Nullable<double> altitude, int timeZone, char hemisphere)
            : base(utmX, utmY, altitude, timeZone, PRC, SSE, hemisphere)
        { }

        public WgsPoint(double utmX, double utmY, Nullable<double> altitude, double refMeridian, char hemisphere)
            : base(utmX, utmY, altitude, refMeridian, PRC, SSE, hemisphere)
        { }

        /// <summary>
        /// Construct WgsPoint class from geografic coordinates.
        /// </summary>
        /// <param name="latitude">Waypoint latitude.</param>
        /// <param name="longitude">Waypoint longitude.</param>
        /// <param name="altitude">Waypoint altitude.</param>
        public WgsPoint(double latitude, double longitude, Nullable<double> altitude)
            : base(latitude, longitude, altitude, PRC, SSE)
        { }

        public WgsPoint(double latitude, double longitude, Nullable<double> altitude, double refMeridian)
            : base(latitude, longitude, altitude, refMeridian, PRC, SSE)
        { }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <returns>The same waypoint.</returns>
        public override Point fromWgs(WgsPoint p)
        {
            return p;
        }

        /// <summary>
        /// Do nothing.
        /// </summary>
        /// <returns>The same waypoint.</returns>
        public override WgsPoint toWgs()
        {
            return this;
        }

        public static WgsPoint operator +(WgsPoint p, Vector v)
        {
            double x = p.utmX + v.X;
            double y = p.utmY + v.Y;
            return new WgsPoint(x, y, p.altitude, p.refMeridian, p.hemisphere);
        }

        public static WgsPoint operator +(Vector v, WgsPoint p)
        {
            double x = p.utmX + v.X;
            double y = p.utmY + v.Y;
            return new WgsPoint(x, y, p.altitude, p.refMeridian, p.hemisphere);
        }

        public static WgsPoint ToWgs(Position p)
        {
            double lat = p.Latitude * 180.0 / Math.PI;
            double lon = p.Longitude * 180.0 / Math.PI;
            double alt = p.Altitude;

            return new WgsPoint(lat, lon, alt);
        }

        public static WgsPoint ToWgs(Waypoint wp)
        {
            double lat = wp.latitude * 180 / Math.PI;
            double lon = wp.longitude * 180 / Math.PI;
            double alt = wp.altitude;

            return new WgsPoint(lat, lon, alt);
        }
    }
}

}

