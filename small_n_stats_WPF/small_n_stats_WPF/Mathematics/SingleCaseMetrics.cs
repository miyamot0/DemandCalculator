using small_n_stats_WPF.Models;
using System.Collections.Generic;
using System.Linq;

namespace small_n_stats_WPF.Mathematics
{
    class SingleCaseMetrics
    {
        public List<double> baselineArray { get; set; }
        public List<double> treatmentArray { get; set; }
        private const double z95 = 1.96;
        private const double z90 = 1.645;
        private const double z85 = 1.44;
        private const double z80 = 1.28;

        /****************************************************************/
        /* Non-Parametric Calculations                                  */
        /****************************************************************/

        /* Non parametric index using highest/lowest point in baseline */
        public double getPND()
        {
            double blPoint;
            bool isAscending = isChangeAscending();
            int PND = 0;

            if (isAscending)
            {
                blPoint = baselineArray.Max();
            }
            else
            {
                blPoint = baselineArray.Min();
            }

            for (int count = 0; count < treatmentArray.Count; count++)
            {
                if (isAscending && treatmentArray[count] >= blPoint)
                {
                    PND++;
                }

                if (!isAscending && treatmentArray[count] <= blPoint)
                {
                    PND++;
                }
            }

            return ((double)PND / (double)treatmentArray.Count) * 100;
        }

        /* Non parametric index using median */
        public PercentExceedingMedianModel getPEM()
        {
            List<double> tempArr = new List<double>(baselineArray);
            tempArr.Sort();

            double blPoint = getMedian(tempArr);
            bool isAscending = isChangeAscending();
            int PEM = 0;

            for (int count = 0; count < treatmentArray.Count; count++)
            {
                if (isAscending && treatmentArray[count] > blPoint)
                {
                    PEM++;
                }

                if (!isAscending && treatmentArray[count] < blPoint)
                {
                    PEM++;
                }
            }

            PercentExceedingMedianModel returnValue = new PercentExceedingMedianModel();
            returnValue.PEM80 = getSingleProportionsConfidence(PEM, treatmentArray.Count, z80);
            returnValue.PEM85 = getSingleProportionsConfidence(PEM, treatmentArray.Count, z85);
            returnValue.PEM90 = getSingleProportionsConfidence(PEM, treatmentArray.Count, z90);
            returnValue.PEM95 = getSingleProportionsConfidence(PEM, treatmentArray.Count, z95);

            return returnValue;
        }

        /* Test of Single Proportions - Binomial distribution (for PEM) - Permits CI's for PEM */
        private PercentExceedingMedianModel.PEM getSingleProportionsConfidence(double value, int total, double confidenceLevel)
        {
            double proportion = value / (double)total;
            double z = confidenceLevel;
            double inner = (z * (1.0 - z)) / total;
            inner = System.Math.Abs(inner);
            double stdErr = System.Math.Sqrt(inner);
            double chunk = (z * stdErr);

            return new PercentExceedingMedianModel.PEM
            {
                ConfidenceLevel = confidenceLevel,
                StandardError = stdErr,
                LowerBound = (proportion - chunk) * 100,
                PercentExceedingMedian = proportion * 100,
                UpperBound = (proportion + chunk) * 100
            };
        }

        /* Non parametric using non-overlap as proportions for Risk Reduction method */
        public ImprovementRateDifferenceModel getIRD()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            bool isAscending = isChangeAscending();
            OverlapOutputModel overlaps;

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            if (isAscending)
            {
                overlaps = getAscendingOverlap();
            }
            else
            {
                overlaps = getDescendingOverlap();
            }

            ImprovementRateDifferenceModel returnValue = new ImprovementRateDifferenceModel();
            returnValue.IRD80 = getIndependentProportionsConfidence(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z80);

            returnValue.IRD85 = getIndependentProportionsConfidence(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z85);

            returnValue.IRD90 = getIndependentProportionsConfidence(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z90);

            returnValue.IRD95 = getIndependentProportionsConfidence(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z95);

            return returnValue;
        }

        /* Inverse of normal distribution.  Limited but can get a Z from alpha parameter */
        /* Credit Abramowitz and Stegun (1965). Handbook of Mathematical Functions */
        private double NormInverse(double q)
        {
            if (q == 0.5 || (q.ToString() == "0.5"))
            {
                return 0;
            }

            q = 1.0 - q;

            double p = (q > 0.0 && q < 0.5) ? q : (1.0 - q);
            double t = System.Math.Sqrt(System.Math.Log(1.0 / System.Math.Pow(p, 2.0)));

            double c0 = 2.515517;
            double c1 = 0.802853;
            double c2 = 0.010328;

            double d1 = 1.432788;
            double d2 = 0.189269;
            double d3 = 0.001308;

            double x = t - (c0 + c1 * t + c2 * System.Math.Pow(t, 2.0)) / (1.0 + d1 * t + d2 * System.Math.Pow(t, 2.0) + d3 * System.Math.Pow(t, 3.0));

            if (q > .5)
            {
                x *= -1.0;
            }

            return x;
        }

        /* Test of Two Independent Proportions */
        /* Newcombe, R. G. (1998) Interval Estimation for the Difference Between Independent Proportions, Statistics in Medicine, 17:873-890*/
        private ImprovementRateDifferenceModel.IRD getIndependentProportionsConfidence(int measureOne, int n1, int measureTwo, int n2, double mZ)
        {
            double ProportionOne = (double)measureOne / (double)n1;
            double ProportionTwo = (double)measureTwo / (double)n2;

            double invertedProportionOne = 1.0 - ProportionOne;
            double invertedProportionTwo = 1.0 - ProportionTwo;

            double ProportionDifference = ProportionOne - ProportionTwo;

            double zSquared = mZ * mZ;

            double lower1 = (1 / (2 * (n1 + zSquared))) * (2 * n1 * ProportionOne + zSquared - mZ * System.Math.Pow((zSquared + 4 * n1 * ProportionOne * invertedProportionOne), 0.5));
            double upper1 = (1 / (2 * (n1 + zSquared))) * (2 * n1 * ProportionOne + zSquared + mZ * System.Math.Pow((zSquared + 4 * n1 * ProportionOne * invertedProportionOne), 0.5));

            double lower2 = (1 / (2 * (n2 + zSquared))) * (2 * n2 * ProportionTwo + zSquared - mZ * System.Math.Pow((zSquared + 4 * n2 * ProportionTwo * invertedProportionTwo), 0.5));
            double upper2 = (1 / (2 * (n2 + zSquared))) * (2 * n2 * ProportionTwo + zSquared + mZ * System.Math.Pow((zSquared + 4 * n2 * ProportionTwo * invertedProportionTwo), 0.5));

            double preLower = System.Math.Sqrt(System.Math.Abs((lower1 * (1 - lower1) / n1) + (upper2 * (1 - upper2) / n2)));
            double preUpper = System.Math.Sqrt(System.Math.Abs((upper1 * (1 - upper1) / n1) + (lower2 * (1 - lower2) / n2)));

            return new ImprovementRateDifferenceModel.IRD
            {
                ConfidenceLevel = mZ,
                LowerBound = ProportionDifference - (mZ * preLower),
                ImprovementRateDifference = ProportionDifference,
                UpperBound = ProportionDifference + (mZ * preUpper)
            };
        }

        /* Non parametric using non-overlap between all phases (complete separation proportion) */
        public double getPAND()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            bool isAscending = isChangeAscending();
            OverlapOutputModel overlaps;

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            if (isAscending)
            {
                overlaps = getAscendingOverlap();
            }
            else
            {
                overlaps = getDescendingOverlap();
            }

            int over = overlaps.BaselineOverlaps + overlaps.TreatmentOverlaps;

            System.Console.WriteLine("bl: " + overlaps.BaselineOverlaps);
            System.Console.WriteLine("tx: " + overlaps.TreatmentOverlaps);

            int totalPoints = blN + txN;

            return (((totalPoints - over) / totalPoints) * 100);
        }

        /* Association index, using overlap measures to construct a rebalanced (robust) chi square */
        public PearsonPhiModel getPHI()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            bool isAscending = isChangeAscending();
            OverlapOutputModel overlaps;

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            if (isAscending)
            {
                overlaps = getAscendingOverlap();
            }
            else
            {
                overlaps = getDescendingOverlap();
            }

            int over = overlaps.BaselineOverlaps + overlaps.TreatmentOverlaps;

            PearsonPhiModel returnValue = new PearsonPhiModel();
            returnValue.Phi80 = getIndependentProportionsConfidencePhi(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z80);

            returnValue.Phi85 = getIndependentProportionsConfidencePhi(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z85);

            returnValue.Phi90 = getIndependentProportionsConfidencePhi(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z90);

            returnValue.Phi95 = getIndependentProportionsConfidencePhi(
            (txN - overlaps.TreatmentOverlaps),
            txN,
            overlaps.BaselineOverlaps,
            blN,
            z95);

            return returnValue;
        }

        /* Test of Two Independent Proportions, Two sets are calculated, since dependent on the larger proportion for comparison (phi version) */
        // TODO modify
        private PearsonPhiModel.Phi getIndependentProportionsConfidencePhi(int measureOne, int n1, int measureTwo, int n2, double mZ)
        {
            double p1 = (double)measureOne / (double)n1;
            double p2 = (double)measureTwo / (double)n2;

            double invP1 = 1.0 - p1;
            double invP2 = 1.0 - p2;

            double propDiffs = p1 - p2;

            double z2 = System.Math.Pow(mZ, 2);

            double lowerXa =
                ((2.0 * n1 * p1)
                + z2
                - (mZ *
                    System.Math.Sqrt(z2 + (4.0 * n1 * p1 * invP1))));

            lowerXa = lowerXa / (2.0 * (n1 + z2));      

            double upperXa =
                ((2.0 * n1 * p1)
                + z2
                + (mZ * System.Math.Sqrt(z2
                    + (4.0 * n1 * p1 * invP1))));
            upperXa = upperXa / (2.0 * (n1 + z2));  

            double lowerYa =
                ((2.0 * n1 * p1) + z2 - 1.0 - (mZ *
                    System.Math.Sqrt((z2 - 2.0 - (1.0 / n1) + 4 * p1 * ((n1 * invP1) + 1.0)))));

            lowerYa = lowerYa / (2.0 * (n1 + z2)); 


            double upperYa = ((2.0 * n1 * p1) + z2 + 1.0
                + (mZ * System.Math.Sqrt(
                    z2 - 2.0 - (1.0 / n1)
                    + 4.0 * p1
                   * ((n1 * invP1) + 1.0))));
            upperYa /= (2.0 * (n1 + z2)); 


            double lowerXb = ((2.0 * n2 * p2) + z2 -
                (mZ * System.Math.Sqrt(z2 + (4.0 * n2 * p2 * invP2))));

            lowerXb /= (2.0 * (n2 + z2));  

            double upperXb = ((2.0 * n2 * p2) + z2 +
                (mZ * System.Math.Sqrt(z2 + (4.0 * n2 * p2 * invP2))));

            upperXb /= (2.0 * (n2 + z2));  

            double lowerYb = ((2.0 * n2 * p2) + z2 - 1.0 - (mZ *
                    System.Math.Sqrt((z2 - 2.0 - (1.0 / n2) + 4.0 * p2 * ((n2 * invP2) + 1.0)))));

            lowerYb /= (2.0 * (n2 + z2));  

            double upperYb = ((2.0 * n2 * p2) + z2 - 1.0 - (mZ *
                System.Math.Sqrt(z2 - 2.0 - (1.0 / n2) + 4.0 * p2 * ((n2 * invP2) + 1.0))));

            upperYb /= (2.0 * (n2 + z2)); 

            double propOneHold;
            double propTwoHold;
            double lower1;
            double lower2;
            double upper1;
            double upper2;

            if (p2 > p1)
            {
                propOneHold = p2;
                propTwoHold = p1;
                lower1 = lowerXb;
                upper1 = upperXb;
                lower2 = lowerXa;
                upper2 = upperXa;
            }
            else
            {
                propOneHold = p1;
                propTwoHold = p2;
                lower1 = lowerXa;
                upper1 = upperXa;
                lower2 = lowerXb;
                upper2 = upperXb;
            }

            double lowerBound = System.Math.Pow((propOneHold - lower1), 2) + ((upper2 - propTwoHold) * (upper2 - propTwoHold));
            lowerBound = System.Math.Sqrt(lowerBound);

            double higherBound = System.Math.Pow((upper1 - propOneHold), 2) + ((propTwoHold - lower2) * (propTwoHold - lower2));
            higherBound = System.Math.Sqrt(higherBound);

            double difference = System.Math.Abs(p1 - p2);

            return new PearsonPhiModel.Phi
            {
                ConfidenceLevel = mZ,
                LowerBound = difference - System.Math.Abs(lowerBound),
                PearsonPhi = propDiffs,
                UpperBound = difference + System.Math.Abs(higherBound)
            };
        }

        /* Pair-based association index, essentially a ROC module rigged to determine ordinal dominance (Phase 1 v Phase 2) rather than Sensitivity v Specificity */
        public NonoverlapAllPairsModel getNAP()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            int n = blN + txN;

            int pairs = blN * txN;
            int over = 0;
            int under = 0;
            int same = 0;

            double comparer;

            for (int i = 0; i < blN; i++)
            {
                for (int j = 0; j < txN; j++)
                {
                    comparer = txCopy[j] - blCopy[i];

                    if (comparer > 0)
                    {
                        over++;
                    }
                    else if (comparer == 0)
                    {
                        same++;
                    }
                    else
                    {
                        under++;
                    }
                }
            }

            double NAP = ((double)over + ((double)same * 0.5)) / (double)pairs;

            // Credit to Parker and folks for this
            double variance = (double)pairs * ((double)blN + (double)txN + 1.0) / 3.0;

            NonoverlapAllPairsModel returnValue = new NonoverlapAllPairsModel();

            double se80 = (System.Math.Sqrt(variance) / (double)pairs) * z80 * z80;
            returnValue.NAP80 = new NonoverlapAllPairsModel.NAP
            {
                ConfidenceLevel = z80,
                StandardError = se80,
                LowerBound = NAP - se80,
                NonoverlapAllPairs = NAP,
                UpperBound = NAP + se80
            };

            double se85 = (System.Math.Sqrt(variance) / (double)pairs) * z85 * z85;
            returnValue.NAP85 = new NonoverlapAllPairsModel.NAP
            {
                ConfidenceLevel = z85,
                StandardError = se85,
                LowerBound = NAP - se85,
                NonoverlapAllPairs = NAP,
                UpperBound = NAP + se85
            };

            double se90 = (System.Math.Sqrt(variance) / (double)pairs) * z90 * z90;
            returnValue.NAP90 = new NonoverlapAllPairsModel.NAP
            {
                ConfidenceLevel = z90,
                StandardError = se90,
                LowerBound = NAP - z90,
                NonoverlapAllPairs = NAP,
                UpperBound = NAP + z90
            };

            double se95 = (System.Math.Sqrt(variance) / (double)pairs) * z95 * z95;
            returnValue.NAP95 = new NonoverlapAllPairsModel.NAP
            {
                ConfidenceLevel = z95,
                StandardError = se95,
                LowerBound = NAP - z95,
                NonoverlapAllPairs = NAP,
                UpperBound = NAP + z95
            };

            return returnValue;
        }

        /* Tau-A, unadjusted for trend */
        public TauAModel getTauA()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            int n = blN + txN;

            int pairs = blN * txN;
            int over = 0;
            int under = 0;
            int same = 0;

            double comparer;

            for (int i = 0; i < blN; i++)
            {
                for (int j = 0; j < txN; j++)
                {
                    comparer = txCopy[j] - blCopy[i];

                    if (comparer > 0)
                    {
                        over++;
                    }
                    else if (comparer == 0 || (txCopy[j].ToString() == blCopy[i].ToString()))
                    {
                        same++;
                    }
                    else
                    {
                        under++;
                    }
                }
            }

            double tau = ((double)over - (double)under) / (double)pairs;
            double variance = (double)pairs * (double)(blN + txN + 1.0) / 3.0;

            TauAModel returnValue = new TauAModel();

            double se80 = (System.Math.Sqrt(variance) / (double)pairs) * z80 * z80;
            returnValue.TAU80 = new TauAModel.Tau
            {
                ConfidenceLevel = z80,
                StandardError = se80,
                LowerBound = tau - se80,
                TauA = tau,
                UpperBound = tau + se80
            };

            double se85 = (System.Math.Sqrt(variance) / (double)pairs) * z85 * z85;
            returnValue.TAU85 = new TauAModel.Tau
            {
                ConfidenceLevel = z85,
                StandardError = se85,
                LowerBound = tau - se85,
                TauA = tau,
                UpperBound = tau + se85
            };

            double se90 = (System.Math.Sqrt(variance) / (double)pairs) * z90 * z90;
            returnValue.TAU90 = new TauAModel.Tau
            {
                ConfidenceLevel = z90,
                StandardError = se90,
                LowerBound = tau - z90,
                TauA = tau,
                UpperBound = tau + z90
            };

            double se95 = (System.Math.Sqrt(variance) / (double)pairs) * z95 * z95;
            returnValue.TAU95 = new TauAModel.Tau
            {
                ConfidenceLevel = z95,
                StandardError = se95,
                LowerBound = tau - z95,
                TauA = tau,
                UpperBound = tau + z95
            };

            return returnValue;
        }

        /* Tau-B, unadjusted for trend */
        public TauBModel getTauB()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            int blN = blCopy.Count;
            int txN = txCopy.Count;

            int n = blN + txN;

            int pairs = blN * txN;
            int over = 0;
            int under = 0;
            int same = 0;

            double comparer;

            for (int i = 0; i < blN; i++)
            {
                for (int j = 0; j < txN; j++)
                {
                    comparer = txCopy[j] - blCopy[i];

                    if (comparer > 0)
                    {
                        over++;
                    }
                    else if (comparer == 0 || (txCopy[j].ToString() == blCopy[i].ToString()))
                    {
                        same++;
                    }
                    else
                    {
                        under++;
                    }
                }
            }

            double tau = ((double)over - (double)under) / (pairs - (0.5 * same));
            double variance = (double)pairs * (double)(blN + txN + 1.0) / 3.0;

            TauBModel returnValue = new TauBModel();

            double se80 = (System.Math.Sqrt(variance) / (double)pairs) * z80 * z80;
            returnValue.TAU80 = new TauBModel.Tau
            {
                ConfidenceLevel = z80,
                StandardError = se80,
                LowerBound = tau - se80,
                TauB = tau,
                UpperBound = tau + se80
            };

            double se85 = (System.Math.Sqrt(variance) / (double)pairs) * z85 * z85;
            returnValue.TAU85 = new TauBModel.Tau
            {
                ConfidenceLevel = z85,
                StandardError = se85,
                LowerBound = tau - se85,
                TauB = tau,
                UpperBound = tau + se85
            };

            double se90 = (System.Math.Sqrt(variance) / (double)pairs) * z90 * z90;
            returnValue.TAU90 = new TauBModel.Tau
            {
                ConfidenceLevel = z90,
                StandardError = se90,
                LowerBound = tau - z90,
                TauB = tau,
                UpperBound = tau + z90
            };

            double se95 = (System.Math.Sqrt(variance) / (double)pairs) * z95 * z95;
            returnValue.TAU95 = new TauBModel.Tau
            {
                ConfidenceLevel = z95,
                StandardError = se95,
                LowerBound = tau - z95,
                TauB = tau,
                UpperBound = tau + z95
            };

            return returnValue;
        }

        /* Overlap helper, detect trending based on median comparisons */
        private bool isChangeAscending()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            blCopy.Sort();
            txCopy.Sort();

            double medianBl = getMedian(blCopy);
            double medianTx = getMedian(txCopy);

            if (medianTx > medianBl)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /* Overlap object, retain both baseline and intervention separations */
        private OverlapOutputModel getAscendingOverlap()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            blCopy.Sort();
            txCopy = txCopy.OrderByDescending(x => x).ToList();

            int firstPhaseCount = 0;
            int secondPhaseCount;
            int blHold = 0;
            int txHold = 0;
            double ceiling = 9999999999;
            int hold;

            for (int firstIterator = blCopy.Count - 1; firstIterator != (System.Math.Floor((double)blCopy.Count / 2.0) - 1); firstIterator--)
            {
                secondPhaseCount = 0;

                for (int secondIterator = 0; secondIterator < txCopy.Count; secondIterator++)
                {
                    if (blCopy[firstIterator] >= txCopy[secondIterator])
                    {
                        secondPhaseCount++;
                    }
                }

                hold = firstPhaseCount + secondPhaseCount;

                if (hold < ceiling)
                {
                    ceiling = firstPhaseCount + secondPhaseCount;
                    blHold = firstPhaseCount;
                    txHold = secondPhaseCount;
                }

                firstPhaseCount++;
            }

            return new OverlapOutputModel
            {
                BaselineOverlaps = blHold,
                TreatmentOverlaps = txHold
            };
        }

        /* Overlap object, both baseline and intervention separate */
        private OverlapOutputModel getDescendingOverlap()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            blCopy.Sort();
            txCopy.Sort();

            int firstPhaseCount = 0;
            int secondPhaseCount;
            int blHold = 0;
            int txHold = 0;
            double ceiling = 9999999999;
            int hold;

            for (int firstIterator = 0; firstIterator != (System.Math.Floor((double)blCopy.Count / 2.0)); firstIterator++)
            {
                secondPhaseCount = 0;

                for (int secondIterator = 0; secondIterator < txCopy.Count; secondIterator++)
                {
                    if (blCopy[firstIterator] <= txCopy[secondIterator])
                    {
                        secondPhaseCount++;
                    }
                }

                hold = firstPhaseCount + secondPhaseCount;

                if (hold < ceiling)
                {
                    ceiling = firstPhaseCount + secondPhaseCount;
                    blHold = firstPhaseCount;
                    txHold = secondPhaseCount;
                }

                firstPhaseCount++;
            }

            return new OverlapOutputModel
            {
                BaselineOverlaps = blHold,
                TreatmentOverlaps = txHold
            };
        }

        /* Median method, says it on the tin */
        private double getMedian(List<double> arr)
        {
            int count = arr.Count();
            int middleval = (int)System.Math.Floor((count - 1.0) / 2.0);
            double median, low, high;

            if (count % 2 == 1)
            {
                median = arr[middleval];
            }
            else
            {
                low = arr[middleval];
                high = arr[middleval + 1];
                median = ((low + high) / 2);
            }

            return median;
        }

        /****************************************************************/
        /* Parametric Calculations                                      */
        /****************************************************************/

        /* Calculation of Busk & Serlin's simple effect size */
        public double getBuskAndSerlin()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);
            double variance = getStandardDeviation(blCopy);

            double returner = txCopy.Average() - blCopy.Average();
            returner /= variance;

            return returner;
        }

        /* Calculation of Glass's delta */
        public double getGlassDelta()
        {
            List<double> txCopy = new List<double>(treatmentArray);
            double variance = getStandardDeviation(txCopy);

            double returner = treatmentArray.Average() - baselineArray.Average();
            returner /= variance;

            return returner;
        }

        /* Calculation of Hedges' g */
        public double getHedgesG()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            double blN = blCopy.Count;
            double txN = txCopy.Count;

            double blAve = baselineArray.Average();
            double txAve = treatmentArray.Average();

            double stdBl = getStandardDeviation(blCopy);
            double stdTx = getStandardDeviation(txCopy);

            double top = ((stdTx * stdTx) * (treatmentArray.Count - 1)) + ((stdBl * stdBl) * (baselineArray.Count - 1));
            double bot = blN + txN - 2;

            double pooledStdDev = System.Math.Sqrt(top / bot);

            return ((txAve - blAve) / pooledStdDev);
        }

        /* Calculation of Hedges' g (adjusted for smaller sample size) */
        public double getHedgesGAdjusted()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            double blN = blCopy.Count;
            double txN = txCopy.Count;

            double blAve = baselineArray.Average();
            double txAve = treatmentArray.Average();

            double stdBl = getStandardDeviation(blCopy);
            double stdTx = getStandardDeviation(txCopy);

            double top = ((stdTx * stdTx) * (txN - 1)) + ((stdBl * stdBl) * (blN - 1));
            double bot = blN + txN - 2;

            double pooledStdDev = System.Math.Sqrt(top / bot);

            double g = ((txAve - blAve) / pooledStdDev);

            double n = (double)blN + (double)txN;
            double p1 = 3.0;
            double p2 = ((4.0 * n) - 9.0);
            double b = p1 / p2;

            return (g * (1 - b));
        }

        /* Calculation of Cohen's d */
        public double getCohensD()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            double blN = blCopy.Count;
            double txN = txCopy.Count;

            double blAve = baselineArray.Average();
            double txAve = treatmentArray.Average();

            double stdBl = getStandardDeviation(blCopy);
            double stdTx = getStandardDeviation(txCopy);

            double variance = ((((stdBl * stdBl) * (blN - 1.0)) + ((stdTx * stdTx) * (txN - 1.0))) / (blN + txN));

            return (txAve - blAve) / System.Math.Sqrt(variance);
        }

        /* Calculation of R based on Cohen's d. */
        public PearsonRhoModel getR()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            double blN = blCopy.Count;
            double txN = txCopy.Count;

            double blAve = baselineArray.Average();
            double txAve = treatmentArray.Average();

            double stdBl = getStandardDeviation(blCopy);
            double stdTx = getStandardDeviation(txCopy);

            double variance = ((((stdBl * stdBl) * (blN - 1.0)) + ((stdTx * stdTx) * (txN - 1.0))) / (blN + txN));

            double d = (txAve - blAve) / System.Math.Sqrt(variance);

            double r2 = (d * d) / ((d * d) + 4.0);
            double r = System.Math.Sqrt(r2);

            PearsonRhoModel returnValue = new PearsonRhoModel();
            returnValue.Rho80 = getRZConfidence(r, blN, txN, z80);
            returnValue.Rho85 = getRZConfidence(r, blN, txN, z85);
            returnValue.Rho90 = getRZConfidence(r, blN, txN, z90);
            returnValue.Rho95 = getRZConfidence(r, blN, txN, z95);

            return returnValue;
        }

        /* Convert Rho metric to a Z distribution to get CI's (Fisher's Transformation) */
        private PearsonRhoModel.Rho getRZConfidence(double r, double blLength, double txLength, double mZ)
        {
            double z = (System.Math.Log((1.0 + r) / (1.0 - r))) * 0.5;
            double stdErr = (1.0 / System.Math.Sqrt((blLength + txLength) - 3.0)) * mZ;
            stdErr = getZRConfidence(stdErr);

            return new PearsonRhoModel.Rho
            {
                ConfidenceLevel = mZ,
                LowerBound = (r - stdErr),
                PearsonRho = r,
                UpperBound = (r + stdErr)
            };
        }

        /* Calculation of R2 based on a derived R */
        public PearsonRhoSquaredModel getR2()
        {
            List<double> blCopy = new List<double>(baselineArray);
            List<double> txCopy = new List<double>(treatmentArray);

            double blN = blCopy.Count;
            double txN = txCopy.Count;

            PearsonRhoModel r = getR();
            double r2 = r.Rho95.PearsonRho * r.Rho95.PearsonRho;

            PearsonRhoSquaredModel returnValue = new PearsonRhoSquaredModel();
            returnValue.RhoSquared80 = getR2Confidence(r2, z80);
            returnValue.RhoSquared85 = getR2Confidence(r2, z85);
            returnValue.RhoSquared90 = getR2Confidence(r2, z90);
            returnValue.RhoSquared95 = getR2Confidence(r2, z95);

            return returnValue;
        }

        /* R2 to T distribution (Olkin and Finn's approximation) */
        private PearsonRhoSquaredModel.RhoSquared getR2Confidence(double r2, double z)
        {
            double k = 1;
            double n = baselineArray.Count + treatmentArray.Count;

            double se = (4.0 * r2 * System.Math.Pow((1 - r2), 2) * System.Math.Pow((n - k - 1), 2)) / ((System.Math.Pow(n, 2) - 1) * (3 + n));
            se = System.Math.Pow(se, (1 / 2));

            return new PearsonRhoSquaredModel.RhoSquared
            {
                ConfidenceLevel = z,
                LowerBound = r2 - se * z,
                PearsonRhoSquared = r2,
                UpperBound = r2 + se * z
            };
        }

        /* LINQ powered Average */
        private double getAverage(List<double> arr)
        {
            return arr.Sum() / arr.Count();
        }

        /* Standard Deviation (sample) */
        private double getStandardDeviation(List<double> array)
        {
            double average = array.Average();
            double sumOfSquaresOfDifferences = array.Select(val => (val - average) * (val - average)).Sum();
            double sd = System.Math.Sqrt(sumOfSquaresOfDifferences / (array.Count - 1));

            return sd;
        }

        /* Convert Z score r (Fisher's Transformation) */
        private double getZRConfidence(double z)
        {
            return ((System.Math.Exp(2.0 * z)) - 1.0) / ((System.Math.Exp(2.0 * z)) + 1.0);
        }
    }
}
