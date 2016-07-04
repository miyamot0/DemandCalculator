//----------------------------------------------------------------------------------------------
// <copyright file="DemandFunctionSolvers.cs" 
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

using small_n_stats_WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace small_n_stats_WPF.Mathematics
{
    class DemandFunctionSolvers
    {
        /// <summary>
        /// Hursh & Silverburg's exponetial function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentialDemandFunction()
        {
            return Properties.Resources.ExponentialDemandFunctions;
        }

        /// <summary>
        /// Hursh & Silverburg's exponetial function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentialDemandFunctionKFittings()
        {
            return Properties.Resources.ExponentialDemandFunctionsKFitted;
        }

        /// <summary>
        /// Hursh & Silverburg's exponetial function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentialDemandFunctionKSet()
        {
            return Properties.Resources.ExponentialDemandFunctionsKSet;
        }

        /// <summary>
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunction()
        {
            return Properties.Resources.ExponentiatedDemandFunctions;
        }

        /// <summary>
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunctionKFittings()
        {
            return Properties.Resources.ExponentiatedDemandFunctionsKFitted;
        }

        /// <summary>
        /// Koffarnus' exponentiated function is called from resources here (as string), for use in R function calls
        /// </summary>
        public static string GetExponentiatedDemandFunctionKSet()
        {
            return Properties.Resources.ExponentiatedDemandFunctionKSet;
        }

        /// <summary>
        /// Graphing code for logged exponential model
        /// </summary>
        public static string GetExponentialGraphingFunction()
        {
            return Properties.Resources.GraphingExponential;
        }

        /// <summary>
        /// Graphing code for logged exponentiated model
        /// </summary>
        public static string GetExponentiatedGraphingFunction()
        {
            return Properties.Resources.GraphingExponentiated;
        }

        /// <summary>
        /// Stein check for systematic purchase data
        /// Original Author: Brent Kaplan
        /// </summary>
        public static string GetSteinSystematicCheck()
        {
            return Properties.Resources.SteinSystematicCheck;
        }

        public static double GetAverageLogK(string[,] yValues)
        {
            List<double> holdK = new List<double>();
            List<double> holdY = new List<double>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                holdY.Clear();

                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    holdY.Add(double.Parse(yValues[i, j]));
                }

                double lowY = holdY.Where(v => v > 0).OrderBy(v => v).First();
                double highY = holdY.Where(v => v > 0).OrderBy(v => v).Last();

                holdK.Add((Math.Log10(highY) - Math.Log10(lowY)) + 0.5);
            }

            return holdK.Average();
        }

        public static string GetQ0E(List<DemandCoordinate> demandPoints)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>(demandPoints);
            double minX = temp.OrderBy(s => s.X).FirstOrDefault().X;

            var temp2 = temp.Where(s => s.X == minX);

            if (temp2 == null)
            {
                return "NA";
            }
            else
            {
                double Q0e = temp2.OrderBy(s => s.Y).FirstOrDefault().Y;
                return Q0e.ToString();
            }
        }

        public static string GetQ0EGroup(List<double> xValues, string[,] yValues, int row)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                temp.Add(new DemandCoordinate
                {
                    X = xValues[i],
                    Y = double.Parse(yValues[i, row]),
                    P = 1 + i,
                    Expend = (xValues[i] * double.Parse(yValues[i, row]))
                });
            }

            return GetQ0E(temp);
        }

        public static string GetQ0EGroup(List<double> xValues, string[,] yValues)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    temp.Add(new DemandCoordinate
                    {
                        X = xValues[i],
                        Y = double.Parse(yValues[i, j]),
                        P = 1 + i,
                        Expend = (xValues[i] * double.Parse(yValues[i, j]))
                    });
                }
            }

            return GetQ0E(temp);
        }

        public static string GetBP0(List<DemandCoordinate> demandPoints)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>(demandPoints);
            var temp2 = temp.Where(s => s.Y == 0);

            if (temp2 == null || temp2.ToList().Count < 1)
            {
                return "NA";
            }
            else
            {
                double BP0 = temp2.OrderBy(s => s.X).FirstOrDefault().X;
                return BP0.ToString();
            }
        }

        public static string GetBP0Group(List<double> xValues, string[,] yValues, int row)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                temp.Add(new DemandCoordinate
                {
                    X = xValues[i],
                    Y = double.Parse(yValues[i, row]),
                    P = 1 + i,
                    Expend = (xValues[i] * double.Parse(yValues[i, row]))
                });
            }

            return GetBP0(temp);
        }

        public static string GetBP0Group(List<double> xValues, string[,] yValues)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    temp.Add(new DemandCoordinate
                    {
                        X = xValues[i],
                        Y = double.Parse(yValues[i, j]),
                        P = 1 + i,
                        Expend = (xValues[i] * double.Parse(yValues[i, j]))
                    });
                }
            }

            return GetBP0(temp);
        }

        public static string GetBP1(List<DemandCoordinate> demandPoints)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>(demandPoints);
            var temp2 = temp.Where(s => s.Y != 0);

            if (temp2 == null || temp2.ToList().Count < 1)
            {
                return "NA";
            }
            else
            {
                double BP1 = temp2.OrderByDescending(s => s.X).FirstOrDefault().X;
                return BP1.ToString();
            }
        }

        public static string GetBP1Group(List<double> xValues, string[,] yValues, int row)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                temp.Add(new DemandCoordinate
                {
                    X = xValues[i],
                    Y = double.Parse(yValues[i, row]),
                    P = 1 + i,
                    Expend = (xValues[i] * double.Parse(yValues[i, row]))
                });
            }

            return GetBP1(temp);
        }

        public static string GetBP1Group(List<double> xValues, string[,] yValues)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    temp.Add(new DemandCoordinate
                    {
                        X = xValues[i],
                        Y = double.Parse(yValues[i, j]),
                        P = 1 + i,
                        Expend = (xValues[i] * double.Parse(yValues[i, j]))
                    });
                }
            }

            return GetBP1(temp);
        }

        public static string GetOmaxE(List<DemandCoordinate> demandPoints)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>(demandPoints);
            var temp2 = temp.Where(s => s.Expend != 0);

            if (temp2 == null || temp2.ToList().Count < 1)
            {
                return "NA";
            }
            else
            {
                double O = temp2.OrderByDescending(s => s.Expend).FirstOrDefault().Expend;
                return O.ToString();
            }
        }

        public static string GetOmaxEGroup(List<double> xValues, string[,] yValues, int row)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                temp.Add(new DemandCoordinate
                {
                    X = xValues[i],
                    Y = double.Parse(yValues[i, row]),
                    P = 1 + i,
                    Expend = (xValues[i] * double.Parse(yValues[i, row]))
                });
            }

            return GetOmaxE(temp);
        }

        public static string GetOmaxEGroup(List<double> xValues, string[,] yValues)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    temp.Add(new DemandCoordinate
                    {
                        X = xValues[i],
                        Y = double.Parse(yValues[i, j]),
                        P = 1 + i,
                        Expend = (xValues[i] * double.Parse(yValues[i, j]))
                    });
                }
            }

            return GetOmaxE(temp);
        }

        public static string GetPmaxE(List<DemandCoordinate> demandPoints)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>(demandPoints);
            var temp2 = temp.Where(s => s.Expend != 0);

            if (temp2 == null || temp2.ToList().Count < 1)
            {
                return "NA";
            }
            else
            {
                double P = temp2.OrderByDescending(s => s.Expend).FirstOrDefault().X;
                return P.ToString();
            }
        }

        public static string GetPmaxEGroup(List<double> xValues, string[,] yValues, int row)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int i = 0; i < yValues.GetLength(0); i++)
            {
                temp.Add(new DemandCoordinate
                {
                    X = xValues[i],
                    Y = double.Parse(yValues[i, row]),
                    P = 1 + i,
                    Expend = (xValues[i] * double.Parse(yValues[i, row]))
                });
            }

            return GetPmaxE(temp);
        }

        public static string GetPmaxEGroup(List<double> xValues, string[,] yValues)
        {
            List<DemandCoordinate> temp = new List<DemandCoordinate>();

            for (int j = 0; j < yValues.GetLength(1); j++)
            {
                for (int i = 0; i < yValues.GetLength(0); i++)
                {
                    temp.Add(new DemandCoordinate
                    {
                        X = xValues[i],
                        Y = double.Parse(yValues[i, j]),
                        P = 1 + i,
                        Expend = (xValues[i] * double.Parse(yValues[i, j]))
                    });
                }
            }

            return GetPmaxE(temp);
        }
    }
}
