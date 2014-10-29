﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © 2014 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Windows;
using System.Windows.Media;

namespace MapControl
{
    internal partial class TileContainer
    {
        private Matrix GetTileIndexMatrix(double scale)
        {
            var transform = ViewportTransform.Matrix;
            transform.Invert(); // view to map coordinates
            transform.Translate(180d, -180d);
            transform.Scale(scale, -scale); // map coordinates to tile indices

            return transform;
        }

        private void UpdateViewportTransform(double scale, Point mapOrigin)
        {
            var transform = new Matrix(1d, 0d, 0d, 1d, -mapOrigin.X, -mapOrigin.Y);
            transform.Scale(scale, -scale);
            transform.Rotate(rotation);
            transform.Translate(viewportOrigin.X, viewportOrigin.Y);

            ViewportTransform.Matrix = transform;
        }

        /// <summary>
        /// Sets a RenderTransform with origin at tileGrid.X and tileGrid.Y to minimize rounding errors.
        /// </summary>
        private void UpdateRenderTransform()
        {
            var scale = Math.Pow(2d, zoomLevel - tileZoomLevel);
            var transform = new Matrix(1d, 0d, 0d, 1d, tileGrid.X * TileSource.TileSize, tileGrid.Y * TileSource.TileSize);
            transform.Scale(scale, scale);
            transform.Translate(tileLayerOffset.X, tileLayerOffset.Y);
            transform.RotateAt(rotation, viewportOrigin.X, viewportOrigin.Y);

            ((MatrixTransform)RenderTransform).Matrix = transform;
        }
    }
}
