﻿// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// Copyright © Clemens Fischer 2012-2013
// Licensed under the Microsoft Public License (Ms-PL)

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#else
using System.Windows;
using System.Windows.Media;
#endif

namespace MapControl
{
    /// <summary>
    /// Fills a rectangular area with an ImageBrush from the Source property.
    /// </summary>
    public class MapImage : MapRectangle
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(ImageSource), typeof(MapImage),
            new PropertyMetadata(null, (o, e) => ((MapImage)o).SourceChanged((ImageSource)e.NewValue)));

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        private void SourceChanged(ImageSource image)
        {
            Fill = new ImageBrush { ImageSource = image };
        }
    }
}
