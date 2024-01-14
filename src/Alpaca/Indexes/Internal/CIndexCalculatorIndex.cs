using System;
using System.Collections.Generic;

namespace UnicornAnalytics.Indexes.Internal;

public class CIndexCalculatorIndex
{
    public double Calculate(double[][] allData, int[] clusterIndices)
    {
        List<Tuple<double, bool>> distanceList = new List<Tuple<double, bool>>();
        int pointCount = allData.Length;

        int numOfPairs = 0;
        double actualSeparation = 0;
        for (int i = 0; i < pointCount; i++)
        {
            for (int j = i + 1; j < pointCount; j++)
            {
                double distance = EuclideanDistance(allData[i], allData[j]);
                distanceList.Add(new Tuple<double, bool>(distance, clusterIndices[i] == clusterIndices[j]));

                // If the points belong to the same cluster
                if (clusterIndices[i] == clusterIndices[j])
                {
                    actualSeparation += distance;
                    numOfPairs++;
                }
            }
        }

        // Now we sort all the distances
        distanceList.Sort((x, y) => x.Item1.CompareTo(y.Item1));

        double minSeparation = 0;
        for (int i = 0; i < numOfPairs; i++)
        {
            minSeparation += distanceList[i].Item1;
        }

        double maxSeparation = 0;
        for (int i = 0; i < numOfPairs; i++)
        {
            maxSeparation += distanceList[distanceList.Count - 1 - i].Item1;
        }

        return (actualSeparation - minSeparation) / (maxSeparation - minSeparation);
    }

    private double EuclideanDistance(double[] x, double[] y)
    {
        double distance = 0;
        for (int i = 0; i < x.Length; i++)
            distance += Math.Pow(x[i] - y[i], 2);
        return Math.Sqrt(distance);
    }
}