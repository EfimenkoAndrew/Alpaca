using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class DaviesBouldinIndex
{
    private double GetCentroid(double[] cluster)
    {
        return cluster.Average();
    }

    private double GetAvgIntraClusterDistance(double[] cluster, double centroid)
    {
        double sum = 0;
        foreach (double point in cluster)
        {
            sum += Math.Abs(point - centroid);
        }
        return sum / cluster.Length;
    }

    private double GetInterClusterDistance(double centroid1, double centroid2)
    {
        return Math.Abs(centroid1 - centroid2);
    }

    public double Calculate(double[][] data, int[] clusterMarkers)
    {
        double DBIndex = 0;
        int numClusters = clusterMarkers.Distinct().Count();

        double[] intraDistances = new double[numClusters];
        double[] centroids = new double[numClusters];

        for (int i = 0; i < numClusters; i++)
        {
            var indices = Enumerable.Range(0, clusterMarkers.Length)
                .Where(j => clusterMarkers[j] == i)
                .ToArray();
            double[] cluster = indices.SelectMany(index => data[index]).ToArray();
            double centroid = GetCentroid(cluster);
            centroids[i] = centroid;
            intraDistances[i] = GetAvgIntraClusterDistance(cluster, centroid);
        }

        double maxRatio = 0;
        for (int i = 0; i < numClusters; i++)
        {
            for (int j = 0; j < numClusters; j++)
            {
                if (i != j)
                {
                    double interClusterDistance = GetInterClusterDistance(centroids[i], centroids[j]);
                    double ratio = (intraDistances[i] + intraDistances[j]) / interClusterDistance;
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                    }
                }
            }
        }

        DBIndex = maxRatio;

        return DBIndex;
    }
}
