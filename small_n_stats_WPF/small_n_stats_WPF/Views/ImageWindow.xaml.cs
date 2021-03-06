﻿//----------------------------------------------------------------------------------------------
// <copyright file="ImageWindow.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Demand Curve Calculator.
//
// Demand Curve Calculator is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Demand Curve Calculator is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Demand Curve Calculator.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Demand Curve Calculator is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.IO;
using System.Windows;

namespace small_n_stats_WPF.Views
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public string filePath = null;

        public ImageWindow()
        {
            InitializeComponent();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "SVG Image|*.svg|TIFF Image|*.tiff|PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            string mExt = Path.GetExtension(saveFileDialog1.FileName);

            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            ImageSvgConverter converter = new ImageSvgConverter(settings);

            if (mExt.Equals(".jpg"))
            {
                converter.EncoderType = ImageEncoderType.JpegBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".png"))
            {
                converter.EncoderType = ImageEncoderType.PngBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".gif"))
            {
                converter.EncoderType = ImageEncoderType.GifBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".tiff"))
            {
                converter.EncoderType = ImageEncoderType.TiffBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".svg"))
            {
                File.Copy(filePath, saveFileDialog1.FileName, true);
            }
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
