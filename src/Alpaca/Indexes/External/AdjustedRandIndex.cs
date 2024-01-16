using System;
using System.Linq;

namespace UnicornAnalytics.Indexes.External
{
    public class AdjustedRandIndex
    {
        public double Calculate(int[] clusterIndices1, int[] clusterIndices2)
        {
            if (clusterIndices1.Length != clusterIndices2.Length)
            {
                throw new ArgumentException("Size of both cluster arrays should be equal");
            }

            int maxIndex = Math.Max(clusterIndices1.Max(), clusterIndices2.Max()) + 1;
            double[,] contingencyMatrix = new double[maxIndex, maxIndex];

            // Create the contingency table
            for (int i = 0; i < clusterIndices1.Length; i++)
            {
                contingencyMatrix[clusterIndices1[i], clusterIndices2[i]]++;
            }

            int[] rowSums = new int[maxIndex];
            int[] colSums = new int[maxIndex];

            // Calculate the row and column sums
            for (int i = 0; i < maxIndex; i++)
            {
                for (int j = 0; j < maxIndex; j++)
                {
                    rowSums[i] += (int)contingencyMatrix[i, j];
                    colSums[j] += (int)contingencyMatrix[i, j];
                }
            }

            double sumCombRow = 0.0;
            double sumCombCol = 0.0;
            double sumComb = 0.0;

            for (int i = 0; i < maxIndex; i++)
            {
                sumComb += Combination2((int)contingencyMatrix[i, i]);
                sumCombRow += Combination2(rowSums[i]);
                sumCombCol += Combination2(colSums[i]);
            }

            double index = sumComb - sumCombRow * sumCombCol / Combination2(clusterIndices1.Length);
            double maxIndexScore = 0.5 * (sumCombRow + sumCombCol) - (sumCombRow * sumCombCol) / Combination2(clusterIndices1.Length);
            double ARI = index / maxIndexScore;

            return ARI;
        }

        private double Combination2(int n)
        {
            // combinations of 2
            if (n < 2)
                return 0;

            return (double)(n * (n - 1)) / 2;
        }
    }
}
