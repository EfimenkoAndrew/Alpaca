using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.Internal
{
    public class HubertIndex
    {
        public double Calculate(double[][] data, int[] clusters)
        {
            int dataSize = data.Length;
            double[][] distanceMatrix = GenerateDistanceMatrix(data, dataSize);
            distanceMatrix = NormalizeDistanceMatrix(distanceMatrix);
            double numerator = 0.0;
            double denominator = 0.0;

            for (int i = 0; i < dataSize; i++)
            {
                for (int j = 0; j < dataSize; j++)
                {
                    if (i != j)
                    {
                        double s = (clusters[i] == clusters[j]) ? 1.0 : -1.0;
                        numerator += s * distanceMatrix[i][j];
                        denominator += distanceMatrix[i][j];
                    }
                }
            }

            return numerator / denominator;
        }

        private double[][] GenerateDistanceMatrix(double[][] data, int dataSize)
        {
            double[][] distanceMatrix = new double[dataSize][];
            for (int i = 0; i < dataSize; i++)
            {
                distanceMatrix[i] = new double[dataSize];
                for (int j = 0; j < i; j++)
                {
                    distanceMatrix[i][j] = distanceMatrix[j][i] = EuclideanDistance(data[i], data[j]);
                }
            }

            return distanceMatrix;
        }

        private double[][] NormalizeDistanceMatrix(double[][] distanceMatrix)
        {
            double maxDistance = distanceMatrix.Max(r => r.Max());
            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                for (int j = 0; j < distanceMatrix[i].Length; j++)
                {
                    distanceMatrix[i][j] /= maxDistance;
                }
            }
            return distanceMatrix;
        }
        
        private double EuclideanDistance(double[] point1, double[] point2)
        {
            return Math.Sqrt(point1.Zip(point2, (d1, d2) => Math.Pow(d1 - d2, 2)).Sum());
        }
    }

}
