using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;
public class CalinskiHarabaszIndex
{
    public double Calculate(double[][] data, int[] clusterTags, double[][] centroids)
    {
        int numberOfClusters = centroids.Length;
        int numberOfDataPoints = data.Length;
        double betweenClusterVariance = ComputeBetweenClusterVariance(data, centroids, clusterTags);
        double withinClusterVariance = ComputeWithinClusterVariance(data, centroids, clusterTags);

        if (numberOfClusters == 1 || withinClusterVariance == 0)
        {
            return 0;
        }
        return (betweenClusterVariance / withinClusterVariance) * (numberOfDataPoints - numberOfClusters) / (numberOfClusters - 1);
    }

    private double ComputeWithinClusterVariance(double[][] data, double[][] centroids, int[] clusterTags)
    {
        double variance = 0.0;
        for (int i = 0; i < data.Length; i++)
        {
            double[] dataPoint = data[i];
            double[] centroid = centroids[clusterTags[i]];
            variance += EuclideanDistanceSquared(dataPoint, centroid);
        }
        return variance;
    }

    private double ComputeBetweenClusterVariance(double[][] data, double[][] centroids, int[] clusterTags)
    {
        double[] overallCentroid = CalculateOverallCentroid(centroids);
        double variance = 0.0;
        int[] clusterSizes = new int[centroids.Length];

        for (int i = 0; i < clusterTags.Length; i++)
            clusterSizes[clusterTags[i]]++;

        for (int i = 0; i < centroids.Length; i++)
        {
            variance += clusterSizes[i] * EuclideanDistanceSquared(centroids[i], overallCentroid);
        }

        return variance;
    }

    private double EuclideanDistanceSquared(double[] pointA, double[] pointB)
    {
        return pointA.Zip(pointB, (a, b) => (a - b) * (a - b)).Sum();
    }

    private double[] CalculateOverallCentroid(double[][] centroids)
    {
        return Enumerable.Range(0, centroids[0].Length)
            .Select(i => centroids.Average(point => point[i]))
            .ToArray();
    }
}

