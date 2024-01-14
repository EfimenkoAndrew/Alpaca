using System;

namespace UnicornAnalytics.Indexes.External
{
    public class HubertIndex
    {
        public double Calculate(int[] labelsTrue, int[] labelsPred)
        {
            if (labelsTrue.Length != labelsPred.Length)
            {
                throw new ArgumentException("Size of both label arrays must be equal.");
            }

            int n = labelsTrue.Length;
            int[,] contingencyMatrix = new int[n, n];

            for (int i = 0; i < n; i++)
            {
                contingencyMatrix[labelsTrue[i], labelsPred[i]]++;
            }

            double sumComb = 0.0, sumCombCol = 0.0, sumCombRow = 0.0;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sumComb += Combination2(contingencyMatrix[i, j]);
                }

                sumCombRow += Combination2(RowSum(contingencyMatrix, i));
                sumCombCol += Combination2(ColSum(contingencyMatrix, n, i));
            }

            double index = sumComb - sumCombRow * sumCombCol / Combination2(n);
            double maxIndex = 0.5 * (sumCombRow + sumCombCol) - sumCombRow * sumCombCol / Combination2(n);
            double ARI = index / maxIndex;

            return ARI;
        }

        private double Combination2(int n)
        {
            if (n < 2) return 0;
            return (double)(n * (n - 1)) / 2;
        }

        private int RowSum(int[,] matrix, int row)
        {
            int sum = 0;
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                sum += matrix[row, j];
            }

            return sum;
        }

        private int ColSum(int[,] matrix, int n, int col)
        {
            int sum = 0;
            for (int i = 0; i < n; i++)
            {
                sum += matrix[i, col];
            }

            return sum;
        }
    }

}
