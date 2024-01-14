using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Indexes.External
{
    public class AdjustedRandIndex
    {
        public double Calculate(int[] clusterIndices1, int[] clusterIndices2)
        {
            if (clusterIndices1.Length != clusterIndices2.Length)
            {
                throw new ArgumentException("Size of both cluster arrays should be equal");
            }

            int N = clusterIndices1.Length;
            double[,] contingencyMatrix = new double[N, N];

            // Create the contingency table
            for (int i = 0; i < N; i++)
            {
                contingencyMatrix[clusterIndices1[i], clusterIndices2[i]]++;
            }

            int[] rowSums = new int[N];
            int[] colSums = new int[N];

            // Calculate the row and column sums
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    rowSums[i] += (int)contingencyMatrix[i, j];
                    colSums[j] += (int)contingencyMatrix[i, j];
                }
            }

            double sumCombRow = 0.0;
            double sumCombCol = 0.0;
            double sumComb = 0.0;

            for (int i = 0; i < N; i++)
            {
                sumComb += Combination2((int)contingencyMatrix[i, i]);
                sumCombRow += Combination2(rowSums[i]);
                sumCombCol += Combination2(colSums[i]);
            }

            double index = sumComb - sumCombRow * sumCombCol / Combination2(N);
            double maxIndex = 0.5 * (sumCombRow + sumCombCol) - sumCombRow * sumCombCol / Combination2(N);
            double ARI = index / maxIndex;

            return ARI;
        }

        private double Combination2(int n)
        {
            // combinations of n items, taken 2 at a time, i.e. "n choose 2"
            if (n < 2)
                return 0;

            return (double)(n * (n - 1)) / 2;
        }
    }
}
