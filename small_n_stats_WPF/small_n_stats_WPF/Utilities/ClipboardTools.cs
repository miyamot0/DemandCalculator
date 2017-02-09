//----------------------------------------------------------------------------------------------
// <copyright file="ClipboardTools.cs" 
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

using Microsoft.VisualBasic.FileIO;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace small_n_stats_WPF.Utilities
{
    class ClipboardTools
    {
        /// <summary>
        /// Clipboard parsing class.  
        /// Detects clipboard object, parses as such using VB IO library.
        /// Returns block-/jagged-style collection of string arrays, for use in grid paste
        /// </summary> 
        public static List<string[]> ReadAndParseClipboardData()
        {
            List<string[]> clipboardData = new List<string[]>();

            if (Clipboard.GetDataObject().GetData(DataFormats.CommaSeparatedValue) != null)
            {
                using (TextFieldParser parser = new TextFieldParser(new StringReader((string)Clipboard.GetDataObject().GetData(DataFormats.CommaSeparatedValue))))
                {
                    parser.SetDelimiters(new string[] { "," });
                    parser.HasFieldsEnclosedInQuotes = true;

                    while (!parser.EndOfData)
                    {
                        clipboardData.Add(parser.ReadFields());
                    }
                }
            }
            else if (Clipboard.GetDataObject().GetData(DataFormats.Text) != null)
            {
                using (TextFieldParser parser = new TextFieldParser(new StringReader((string)Clipboard.GetDataObject().GetData(DataFormats.Text))))
                {
                    parser.SetDelimiters(new string[] { "\t" });
                    parser.HasFieldsEnclosedInQuotes = true;

                    while (!parser.EndOfData)
                    {
                        clipboardData.Add(parser.ReadFields());
                    }
                }
            }

            return clipboardData;
        }
    }
}