using System;
using System.Collections.Generic;
using System.Linq;

namespace UnicornAnalytics.Clustering
{
    public class MeanShift
    {
        private readonly double _bandwidth;

        public double[][] Centers { get; private set; }
        public int[] Labels { get; private set; }

        public MeanShift(double bandwidth)
        {
            _bandwidth = bandwidth;
        }

        public void Fit(double[][] data)
        {
            List<double[]> centers = new List<double[]>();
            foreach (double[] point in data)
            {
                centers.Add(ShiftPoint(point, data, _bandwidth));
            }

            for (int i = 0; i < centers.Count; i++)
            {
                for (int j = i + 1; j < centers.Count; j++)
                {
                    if (EuclideanDistance(centers[i], centers[j]) < _bandwidth)
                    {
                        centers[j] = centers[i];
                    }
                }
            }

            Centers = centers.Distinct(new CenterComparer()).ToArray();

            Labels = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                double minDist = double.PositiveInfinity;
                for (int j = 0; j < Centers.Length; j++)
                {
                    double dist = EuclideanDistance(data[i], Centers[j]);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        Labels[i] = j;
                    }
                }
            }
        }

        private double[] ShiftPoint(double[] point, IEnumerable<double[]> points, double bandwidth)
        {
            double[] shiftedPoint = new double[point.Length];
            double scale = 0;
            foreach (double[] p in points)
            {
                double distance = EuclideanDistance(point, p);
                double weight = GaussianKernel(distance, bandwidth);
                for (int i = 0; i < shiftedPoint.Length; i++)
                {
                    shiftedPoint[i] += weight * p[i];
                }
                scale += weight;
            }

            for (int i = 0; i < shiftedPoint.Length; i++)
            {
                shiftedPoint[i] /= scale;
            }

            return shiftedPoint;
        }

        private double EuclideanDistance(double[] a, double[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                double diff = a[i] - b[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }

        private double GaussianKernel(double distance, double bandwidth)
        {
            return (1 / (bandwidth * Math.Sqrt(2 * Math.PI))) * Math.Exp(-0.5 * Math.Pow(distance / bandwidth, 2));
        }

        private class CenterComparer : IEqualityComparer<double[]>
        {
            public bool Equals(double[] x, double[] y)
            {
                return x.SequenceEqual(y);
            }

            public int GetHashCode(double[] obj)
            {
                return obj.Sum().GetHashCode();
            }
        }
    }
}
