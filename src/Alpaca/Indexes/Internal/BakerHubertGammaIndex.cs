using System;
using System.Collections.Generic;
using System.Text;

namespace AlpacaAnalytics.Indexes.Internal
{
    public class BakerHubertGammaIndex
    {
        public double Calculate(int[] partitionA, int[] partitionB)
        {
            if (partitionA.Length != partitionB.Length)
            {
                throw new ArgumentException("Both partitions should have the same length");
            }

            int N = partitionA.Length;
            int Nc = 0, Nd = 0;

            for (int i = 0; i < N; i++)
            {
                for (int j = i + 1; j < N; j++)
                {
                    bool inSameClusterA = partitionA[i] == partitionA[j];
                    bool inSameClusterB = partitionB[i] == partitionB[j];

                    if (inSameClusterA && inSameClusterB)
                    {
                        Nc++;
                    }

                    if (inSameClusterA ^ inSameClusterB)
                    {
                        Nd++;
                    }
                }
            }

            return (double)(Nc - Nd) / (Nc + Nd);
        }
    }

}
