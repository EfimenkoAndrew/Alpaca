using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class SilhouetteIndex
{
    public double Calculate(double[][] data, int[] labels)
    {
        int n = data.Length;

        double totalScore = 0.0;
        for (int i = 0; i < n; i++)
        {
            double ai = AverageDistance(data[i], labels[i], data, labels);
            double bi = double.MaxValue;

            foreach (int label in labels.Distinct())
            {
                if (label != labels[i])
                {
                    double b = AverageDistance(data[i], label, data, labels);
                    bi = Math.Min(bi, b);
                }
            }

            double score = (bi - ai) / Math.Max(ai, bi);
            totalScore += score;
        }

        return totalScore / n;
    }

    private double AverageDistance(double[] point, int label, double[][] data, int[] labels)
    {
        double totalDistance = 0.0;
        int count = 0;

        for (int i = 0; i < data.Length; i++)
        {
            if (labels[i] == label)
            {
                totalDistance += EuclideanDistance(point, data[i]);
                count++;
            }
        }

        return totalDistance / count;
    }

    private double EuclideanDistance(double[] a, double[] b)
    {
        if (a.Length != b.Length)
        {
            throw new ArgumentException("The two points must have the same dimensions.");
        }

        double sum = 0.0;
        for (int i = 0; i < a.Length; i++)
        {
            double diff = a[i] - b[i];
            sum += diff * diff;
        }

        return Math.Sqrt(sum);
    }
}
