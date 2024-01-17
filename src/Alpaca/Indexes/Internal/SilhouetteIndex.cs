using System;
using System.Collections.Generic;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal;

public class SilhouetteIndex
{
    public double Calculate(double[][] data, int[] labels)
    {
        var silhouetteScores = new List<double>();

        for (int i = 0; i < data.Length; i++)
        {
            var otherIndicesInCluster = Enumerable.Range(0, data.Length)
                .Where(idx => idx != i && labels[idx] == labels[i])
                .ToList();

            if (otherIndicesInCluster.Count == 0)
            {
                // If the cluster contains only one data point, the silhouette score is 0.
                silhouetteScores.Add(0);
                continue;
            }

            double a = AverageDistance(data[i], otherIndicesInCluster.Select(idx => data[idx]));
            double minB = labels.Where((label, idx) => label != labels[i])
                .Distinct()
                .Select(label => AverageDistance(data[i], GetIndicesWithLabel(labels, label).Select(idx => data[idx])))
                .Min();

            silhouetteScores.Add((minB - a) / Math.Max(a, minB));
        }

        return silhouetteScores.Average();
    }

    private double AverageDistance(double[] point, IEnumerable<double[]> otherPoints)
    {
        return otherPoints.Select(p => Distance(point, p)).Average();
    }

    private double Distance(double[] pointA, double[] pointB)
    {
        // Euclidean distance (make sure the dimensions of pointA and pointB match)
        return Math.Sqrt(pointA.Zip(pointB, (a, b) => Math.Pow(a - b, 2)).Sum());
    }

    private IEnumerable<int> GetIndicesWithLabel(int[] labels, int label)
    {
        return Enumerable.Range(0, labels.Length).Where(idx => labels[idx] == label);
    }
}

