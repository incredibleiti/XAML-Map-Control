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
    /// Transforms map coordinates according to the Gnomonic Projection.
    /// </summary>
    public class StereographicProjection : AzimuthalProjection
    {
        public StereographicProjection()
            : this("AUTO2:97002") // GeoServer non-standard CRS ID
        {
        }

        public StereographicProjection(string crsId)
        {
            CrsId = crsId;
        }

        public override Point LocationToPoint(Location location)
        {
            if (location.Equals(ProjectionCenter))
            {
                return new Point();
            }

            double azimuth, distance;

            GetAzimuthDistance(ProjectionCenter, location, out azimuth, out distance);

            var mapDistance = 2d * Wgs84EquatorialRadius * Math.Tan(distance / 2d);

            return new Point(mapDistance * Math.Sin(azimuth), mapDistance * Math.Cos(azimuth));
        }

        public override Location PointToLocation(Point point)
        {
            if (point.X == 0d && point.Y == 0d)
            {
                return ProjectionCenter;
            }

            var azimuth = Math.Atan2(point.X, point.Y);
            var mapDistance = Math.Sqrt(point.X * point.X + point.Y * point.Y);
            var distance = 2d * Math.Atan(mapDistance / (2d * Wgs84EquatorialRadius));

            return GetLocation(ProjectionCenter, azimuth, distance);
        }
    }
}
