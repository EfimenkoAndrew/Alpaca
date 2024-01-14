using System;

namespace AlpacaAnalytics.Indexes.Internal;

public class SilhouetteIndex
{
    public double Calculate(double[][] allData, int[] clusterIndices)
    {
        double silhouetteSum = 0.0;
        int pointCount = allData.Length;

        for (int i = 0; i < pointCount; i++)
        {
            double averageDistanceWithinCluster = CalculateAverageDistance(i, allData, clusterIndices, clusterIndices[i]);
            double averageDistanceToNextCluster = double.MaxValue;

            for (int j = 0; j < pointCount; j++)
            {
                if (clusterIndices[j] != clusterIndices[i])
                {
                    double distanceToOtherCluster = CalculateAverageDistance(i, allData, clusterIndices, clusterIndices[j]);
                    if (distanceToOtherCluster < averageDistanceToNextCluster)
                    {
                        averageDistanceToNextCluster = distanceToOtherCluster;
                    }
                }
            }

            double silhouette = (averageDistanceToNextCluster - averageDistanceWithinCluster) / Math.Max(averageDistanceWithinCluster, averageDistanceToNextCluster);
            silhouetteSum += silhouette;
        }

        return silhouetteSum / pointCount;
    }

    private double CalculateAverageDistance(int index, double[][] allData, int[] clusterIndices, int clusterIndex)
    {
        double sum = 0.0;
        int count = 0;

        for (int i = 0; i < allData.Length; i++)
        {
            if (clusterIndices[i] == clusterIndex)
            {
                sum += EuclideanDistance(allData[index], allData[i]);
                count++;
            }
        }

        return sum / count;
    }

    private double EuclideanDistance(double[] x, double[] y)
    {
        double distance = 0.0;

        for (int i = 0; i < x.Length; i++)
        {
            distance += Math.Pow(x[i] - y[i], 2);
        }

        return Math.Sqrt(distance);
    }
}