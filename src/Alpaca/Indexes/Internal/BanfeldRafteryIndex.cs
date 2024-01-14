using System;

namespace UnicornAnalytics.Indexes.Internal
{
    public class BanfeldRafteryIndex
    {
        public double Calculate(double[][] clustersCentroids, double[][] allData, int[] allDataClusterIndices)
        {
            double sum = 0;
            int N = allData.Length;
            int K = clustersCentroids.Length;
            int[] clusterSizes = new int[K];

            for (int i = 0; i < N; i++)
                clusterSizes[allDataClusterIndices[i]]++;

            for (int i = 0; i < K; i++)
            {
                double mean = 0;
                for (int j = 0; j < N; j++)
                {
                    if (allDataClusterIndices[j] == i) //belongs to the same cluster
                        mean += EuclideanDistance(allData[j], clustersCentroids[i]);
                }
                mean /= clusterSizes[i];

                double variance = 0;
                for (int j = 0; j < N; j++)
                {
                    if (allDataClusterIndices[j] == i) //belongs to the same cluster
                        variance += Math.Pow(EuclideanDistance(allData[j], clustersCentroids[i]) - mean, 2);
                }
                variance /= clusterSizes[i] - 1;

                sum += Math.Log(clusterSizes[i]) * Math.Sqrt(variance);
            }
            return sum;
        }

        private double EuclideanDistance(double[] x, double[] y)
        {
            double sumOfSquares = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sumOfSquares += Math.Pow(x[i] - y[i], 2);
            }
            return Math.Sqrt(sumOfSquares);
        }
    }
}
