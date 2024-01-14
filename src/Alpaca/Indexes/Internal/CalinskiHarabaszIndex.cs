using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class CalinskiHarabaszIndex
{
    public double Calculate(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
    {
        double betweenCluster = BetweenCluster(clustersCentroids, allData);
        double withinCluster = WithinCluster(clustersCentroids, allData, allDataClusterIndices);
        int n = allData.Length;
        int k = clustersCentroids.Length;
        double score = betweenCluster / withinCluster / ((n - k) / (double)(k - 1));
        return score;
    }

    private double BetweenCluster(double[][] clustersCentroids, double[][] allData)
    {
        double[] dataCentroid = new double[clustersCentroids[0].Length];
        for (int i = 0; i < clustersCentroids[0].Length; i++)
        {
            double elementMean = allData.Average(data => data[i]);
            dataCentroid[i] = elementMean;
        }

        double sum = 0;
        for (int i = 0; i < clustersCentroids.Length; i++)
        {
            int clusterSize = allData.Length;
            sum += clusterSize * Math.Pow(EuclideanDistance(clustersCentroids[i], dataCentroid), 2);
        }

        return sum;
    }

    private double WithinCluster(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
    {
        double sum = 0;
        for (int i = 0; i < clustersCentroids.Length; i++)
        {
            for (int j = 0; j < allDataClusterIndices.Length; j++)
            {
                if (allDataClusterIndices[j] == i)
                {
                    sum += Math.Pow(EuclideanDistance(allData[j], clustersCentroids[i]), 2);
                }
            }
        }
        return sum;
    }

    private double EuclideanDistance(double[] x, double[] y)
    {
        double distance = 0;
        for (int i = 0; i < x.Length; i++)
            distance += Math.Pow(x[i] - y[i], 2);
        return Math.Sqrt(distance);
    }
}
