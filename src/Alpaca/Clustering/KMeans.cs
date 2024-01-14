using System;
using System.Threading.Tasks;

namespace UnicornAnalytics.Clustering;

public class KMeans
{
    public double[][] Centroids { get; set; }

    public int[] ClusterLabels { get; set; }

    public void Fit(double[][] data, int numClusters)
    {
        int numDims = data[0].Length;

        // Initialize centroids to random data points
        Centroids = new double[numClusters][];
        Random rand = new Random();
        for (int i = 0; i < numClusters; i++)
        {
            Centroids[i] = new double[numDims];
            int randIndex = rand.Next(0, data.Length);
            Array.Copy(data[randIndex], Centroids[i], numDims);
        }

        ClusterLabels = new int[data.Length];
        bool didChange;
        do
        {
            didChange = UpdateLabels(data);
            UpdateCentroids(data, numClusters);
        }
        while (didChange);
    }

    private bool UpdateLabels(double[][] data)
    {
        bool didChange = false;
        Parallel.For(0, data.Length, i =>
        {
            double minDist = Double.MaxValue;
            int closestCentroid = 0;
            for (int j = 0; j < Centroids.Length; j++)
            {
                double dist = Distance(data[i], Centroids[j]);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestCentroid = j;
                }
            }
            didChange |= ClusterLabels[i] != closestCentroid;
            ClusterLabels[i] = closestCentroid;
        });
        return didChange;
    }

    private void UpdateCentroids(double[][] data, int numClusters)
    {
        int numDims = data[0].Length;
        var sums = new double[numClusters][];
        var counts = new int[numClusters];
        for (int i = 0; i < numClusters; i++)
        {
            sums[i] = new double[numDims];
        }
        for (int i = 0; i < data.Length; i++)
        {
            int cluster = ClusterLabels[i];
            for (int j = 0; j < numDims; j++)
            {
                sums[cluster][j] += data[i][j];
            }
            counts[cluster]++;
        }

        for (int i = 0; i < Centroids.Length; i++)
        {
            if (counts[i] > 0)
            {
                for (int j = 0; j < numDims; j++)
                {
                    Centroids[i][j] = sums[i][j] / counts[i];
                }
            }
        }
    }

    private static double Distance(double[] vecA, double[] vecB)
    {
        double sum = 0.0;
        for (int i = 0; i < vecA.Length; i++)
            sum += Math.Pow((vecA[i] - vecB[i]), 2);
        return Math.Sqrt(sum);
    }
}
