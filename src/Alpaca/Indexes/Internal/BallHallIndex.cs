using System;

namespace UnicornAnalytics.Indexes.Internal
{
    public class BallHallIndex
    {
        public double Calculate(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
        {
            int n = allData.Length;
            double sumOfSquares = 0.0;

            for (int i = 0; i < n; i++)
            {
                // Calculate the square of the Euclidean distance from each point to its cluster centroid
                sumOfSquares += Math.Pow(EuclideanDistance(allData[i], clustersCentroids[allDataClusterIndices[i]]), 2);
            }

            double ballHallIndex = sumOfSquares / n;
            return ballHallIndex;
        }

        private double EuclideanDistance(double[] x, double[] y)
        {
            double distance = 0;
            for (int i = 0; i < x.Length; i++)
                distance += Math.Pow(x[i] - y[i], 2);

            return Math.Sqrt(distance);
        }
    }
}
