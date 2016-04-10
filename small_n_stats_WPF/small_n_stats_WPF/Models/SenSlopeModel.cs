﻿/*
 * Shawn Gilroy, 2016
 * Small n Stats Application
 * Based on conceptual work developed by Richard Parker (non-parametric statistics in time series)
 * 
 */

using System.Collections.Generic;

namespace small_n_stats_WPF.Models
{
    class SenSlopeModel
    {
        public List<double> Observations { get; set; }
        public int Length { get; set; }
        public double Sen { get; set; }
        public double SenSD { get; set; }
        public double SenVariance { get; set; }
        public double LeastSquares { get; set; }
        public double Zscore { get; set; }
        public double Tau { get; set; }
        public double TauSD { get; set; }
        public double P { get; set; }
        public double[] TS85 { get; set; }
        public double[] TS90 { get; set; }
        public double[] TS95 { get; set; }

        /*
        Experimental
        */

        public double ConoverIntercept { get; set; }
        public double MedianY { get; set; }
        public double MedianX { get; set; }
    }
}
