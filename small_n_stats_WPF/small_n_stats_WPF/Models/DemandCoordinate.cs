//----------------------------------------------------------------------------------------------
// <copyright file="DemandCoordinate.cs" 
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

namespace small_n_stats_WPF.Models
{
    public class DemandCoordinate
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double P { get; set; }
        public double Expend { get; set; }

        public DemandCoordinate() { }

        public DemandCoordinate MakeDemandCoordinate(double x, double y, double p)
        {
            return new DemandCoordinate { X = x, Y = y, P = p, Expend = (x * y) };
        }
    }
}
