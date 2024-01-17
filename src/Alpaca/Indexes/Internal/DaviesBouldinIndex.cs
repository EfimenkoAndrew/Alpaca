using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class DaviesBouldinIndex
{
    private double GetEuclideanDistance(double[] vectorA, double[] vectorB)
    {
        return Math.Sqrt(vectorA.Zip(vectorB, (a, b) => (a - b) * (a - b)).Sum());
    }

    public double Calculate(double[][] data, int[] clusterMarkers)
    {
        int numClusters = clusterMarkers.Max() + 1;
        double[][] centroids = new double[numClusters][];
        double[] avgIntraClusterDistances = new double[numClusters];

        for (int i = 0; i < numClusters; i++)
        {
            var clusterPoints = data.Where((t, j) => clusterMarkers[j] == i).ToArray();
            centroids[i] = clusterPoints.Aggregate(new double[clusterPoints[0].Length], (a, b) => a.Zip(b, (x, y) => x + y).ToArray());
            for (int j = 0; j < centroids[i].Length; j++)
            {
                centroids[i][j] /= clusterPoints.Length;
            }
            avgIntraClusterDistances[i] = clusterPoints.Average(p => GetEuclideanDistance(p, centroids[i]));
        }

        double dbIndex = 0;

        for (int i = 0; i < numClusters; i++)
        {
            double maxRatio = double.MinValue;

            for (int j = 0; j < numClusters; j++)
            {
                if (i != j)
                {
                    double ratio = (avgIntraClusterDistances[i] + avgIntraClusterDistances[j]) / GetEuclideanDistance(centroids[i], centroids[j]);
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                    }
                }
            }

            dbIndex += maxRatio;
        }

        return dbIndex / numClusters;
    }
}
