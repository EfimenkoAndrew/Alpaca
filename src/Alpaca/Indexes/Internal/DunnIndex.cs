using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Indexes.Internal
{
    public class DunnIndex
    {
        public double Calculate(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
        {
            double minInterClusterDistance = double.MaxValue;
            double maxIntraClusterDistance = double.MinValue;

            int K = clustersCentroids.Length;

            // Calculate minimum inter-cluster distance
            for (int i = 0; i < K; i++)
            {
                for (int j = i + 1; j < K; j++)
                {
                    double distance = EuclideanDistance(clustersCentroids[i], clustersCentroids[j]);
                    if (distance < minInterClusterDistance)
                    {
                        minInterClusterDistance = distance;
                    }
                }
            }

            // Calculate maximum intra-cluster distance
            for (int i = 0; i < allData.Length; i++)
            {
                for (int j = i + 1; j < allData.Length; j++)
                {
                    if (allDataClusterIndices[i] == allDataClusterIndices[j]) // Check if they are in the same cluster
                    {
                        double distance = EuclideanDistance(allData[i], allData[j]);
                        if (distance > maxIntraClusterDistance)
                        {
                            maxIntraClusterDistance = distance;
                        }
                    }
                }
            }

            return minInterClusterDistance / maxIntraClusterDistance;
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
