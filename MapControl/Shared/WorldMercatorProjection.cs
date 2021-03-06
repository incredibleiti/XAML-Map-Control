﻿// XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// © 2017 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
#if WINDOWS_UWP
using Windows.Foundation;
#else
using System.Windows;
#endif

namespace MapControl
{
    /// <summary>
    /// Transforms map coordinates according to the "World Mercator" Projection, EPSG:3395.
    /// Longitude values are transformed linearly to X values in meters, by multiplying with MetersPerDegree.
    /// Latitude values are transformed according to the elliptical versions of the Mercator equations,
    /// as shown in "Map Projections - A Working Manual" (https://pubs.usgs.gov/pp/1395/report.pdf), p.44.
    /// </summary>
    public class WorldMercatorProjection : MapProjection
    {
        public static double MinLatitudeDelta = 1d / Wgs84EquatorialRadius; // corresponds to 1 meter
        public static int MaxIterations = 10;

        public WorldMercatorProjection()
            : this("EPSG:3395")
        {
        }

        public WorldMercatorProjection(string crsId)
        {
            CrsId = crsId;
            LongitudeScale = MetersPerDegree;
            MaxLatitude = YToLatitude(180d);
        }

        public override double GetViewportScale(double zoomLevel)
        {
            return DegreesToViewportScale(zoomLevel) / MetersPerDegree;
        }

        public override Point GetMapScale(Location location)
        {
            var lat = location.Latitude * Math.PI / 180d;
            var eSinLat = Wgs84Eccentricity * Math.Sin(lat);
            var scale = ViewportScale * Math.Sqrt(1d - eSinLat * eSinLat) / Math.Cos(lat);

            return new Point(scale, scale);
        }

        public override Point LocationToPoint(Location location)
        {
            return new Point(
                MetersPerDegree * location.Longitude,
                MetersPerDegree * LatitudeToY(location.Latitude));
        }

        public override Location PointToLocation(Point point)
        {
            return new Location(
                YToLatitude(point.Y / MetersPerDegree),
                point.X / MetersPerDegree);
        }

        public override Location TranslateLocation(Location location, Point translation)
        {
            var scaleX = MetersPerDegree * ViewportScale;
            var scaleY = scaleX / Math.Cos(location.Latitude * Math.PI / 180d);

            return new Location(
                location.Latitude - translation.Y / scaleY,
                location.Longitude + translation.X / scaleX);
        }

        public static double LatitudeToY(double latitude)
        {
            if (latitude <= -90d)
            {
                return double.NegativeInfinity;
            }

            if (latitude >= 90d)
            {
                return double.PositiveInfinity;
            }

            var lat = latitude * Math.PI / 180d;

            return Math.Log(Math.Tan(lat / 2d + Math.PI / 4d) * ConformalFactor(lat)) / Math.PI * 180d;
        }

        public static double YToLatitude(double y)
        {
            var t = Math.Exp(-y * Math.PI / 180d);
            var lat = Math.PI / 2d - 2d * Math.Atan(t);
            var latDelta = 1d;

            for (int i = 0; i < MaxIterations && latDelta > MinLatitudeDelta; i++)
            {
                var newLat = Math.PI / 2d - 2d * Math.Atan(t * ConformalFactor(lat));

                latDelta = Math.Abs(newLat - lat);
                lat = newLat;
            }

            return lat / Math.PI * 180d;
        }

        private static double ConformalFactor(double lat)
        {
            var eSinLat = Wgs84Eccentricity * Math.Sin(lat);

            return Math.Pow((1d - eSinLat) / (1d + eSinLat), Wgs84Eccentricity / 2d);
        }
    }
}
