using System;
using System.Linq;

namespace AlpacaAnalytics.Indexes.Internal;

public class DaviesBouldinIndex
{
    public double Calculate(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
    {
        int k = clustersCentroids.Length;
        double[] S = new double[k];

        for (int i = 0; i < k; i++)
        {
            S[i] = allData.Where((t, j) => allDataClusterIndices[j] == i).Average(t => EuclideanDistance(t, clustersCentroids[i]));
        }

        double DBIndex = 0.0;
        for (int i = 0; i < k; i++)
        {
            double maxRatio = 0.0;
            for (int j = 0; j < k; j++)
            {
                if (j != i)
                {
                    double distance = EuclideanDistance(clustersCentroids[i], clustersCentroids[j]);
                    double ratio = (S[i] + S[j]) / distance;
                    if (ratio > maxRatio)
                    {
                        maxRatio = ratio;
                    }
                }
            }
            DBIndex += maxRatio;
        }
        DBIndex /= k;

        return DBIndex;
    }

    private double EuclideanDistance(double[] x, double[] y)
    {
        double distance = 0;
        for (int i = 0; i < x.Length; i++)
            distance += Math.Pow(x[i] - y[i], 2);
        return Math.Sqrt(distance);
    }
}