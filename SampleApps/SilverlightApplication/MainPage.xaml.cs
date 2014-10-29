﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using MapControl;

namespace SilverlightApplication
{
    public partial class MainPage : UserControl
    {
        private TileLayerCollection tileLayers;

        public MainPage()
        {
            //BingMapsTileLayer.ApiKey = ...

            InitializeComponent();

            tileLayers = (TileLayerCollection)Resources["TileLayers"];
            tileLayerComboBox.SelectedIndex = 0;
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            var location = map.ViewportPointToLocation(e.GetPosition(map));
            var latitude = (int)Math.Round(location.Latitude * 60000d);
            var longitude = (int)Math.Round(Location.NormalizeLongitude(location.Longitude) * 60000d);
            var latHemisphere = 'N';
            var lonHemisphere = 'E';

            if (latitude < 0)
            {
                latitude = -latitude;
                latHemisphere = 'S';
            }

            if (longitude < 0)
            {
                longitude = -longitude;
                lonHemisphere = 'W';
            }

            mouseLocation.Text = string.Format(CultureInfo.InvariantCulture,
                "{0}  {1:00} {2:00.000}\n{3} {4:000} {5:00.000}",
                latHemisphere, latitude / 60000, (double)(latitude % 60000) / 1000d,
                lonHemisphere, longitude / 60000, (double)(longitude % 60000) / 1000d);
        }

        private void MapMouseLeave(object sender, MouseEventArgs e)
        {
            mouseLocation.Text = string.Empty;
        }

        private void TileLayerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)tileLayerComboBox.SelectedItem;

            map.TileLayer = tileLayers[(string)selectedItem.Tag];

            var paragraph = new Paragraph();

            foreach (var inline in map.TileLayer.DescriptionInlines)
            {
                paragraph.Inlines.Add(inline);
            }

            mapLegend.Blocks.Clear();
            mapLegend.Blocks.Add(paragraph);
        }

        private void SeamarksChecked(object sender, RoutedEventArgs e)
        {
            map.TileLayers.Add(tileLayers["Seamarks"]);
        }

        private void SeamarksUnchecked(object sender, RoutedEventArgs e)
        {
            map.TileLayers.Remove(tileLayers["Seamarks"]);
        }
    }
}
