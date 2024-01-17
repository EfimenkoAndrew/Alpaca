using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class CalinskiHarabaszIndex
{
    public double Calculate(double[][] data, int[] clusterMarkers)
    {
        int n = data.Length; // total number of data points
        int k = clusterMarkers.Distinct().Count(); // number of clusters

        double[] overallCentroid = CalculateCentroid(data); // overall data centroid

        double SSB = 0, SSW = 0;

        for (int i = 0; i < k; i++)
        {
            double[][] clusterData = data.Where((v, j) => clusterMarkers[j] == i).ToArray();
            double[] clusterCentroid = CalculateCentroid(clusterData); // cluster centroid

            SSB += clusterData.Length * CalculateSquareDistance(clusterCentroid, overallCentroid);

            foreach (double[] point in clusterData)
            {
                SSW += CalculateSquareDistance(point, clusterCentroid);
            }
        }

        double betweenGroupDispersion = SSB / (k - 1);
        double withinGroupDispersion = SSW / (n - k);
        return betweenGroupDispersion / withinGroupDispersion;
    }

    public double[] CalculateCentroid(double[][] data)
    {
        int dimensions = data[0].Length;

        double[] centroid = new double[dimensions];

        foreach (double[] point in data)
        {
            for (int i = 0; i < dimensions; i++)
            {
                centroid[i] += point[i];
            }
        }

        for (int i = 0; i < dimensions; i++)
        {
            centroid[i] /= data.Length;
        }

        return centroid;
    }

    public double CalculateSquareDistance(double[] a, double[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Points must have the same dimensionality");

        double sumSquares = 0;

        for (int i = 0; i < a.Length; i++)
        {
            double difference = a[i] - b[i];
            sumSquares += difference * difference;
        }

        return sumSquares;
    }
}

